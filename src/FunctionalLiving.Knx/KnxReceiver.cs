namespace FunctionalLiving.Knx
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Addressing;
    using Events;
    using Enums;
    using Microsoft.Extensions.Logging;

    internal abstract class KnxReceiver
    {
        private readonly ILogger<KnxReceiver> _logger;

        private Thread _receiverThread;
        private Thread _consumerThread;

        private const ThreadState StateAlive = ThreadState.Running | ThreadState.Background | ThreadState.WaitSleepJoin;

        private BlockingCollection<KnxDatagram> _rxDatagrams;

        protected KnxReceiver(
            ILoggerFactory loggerFactory,
            KnxConnection connection)
        {
            _logger = loggerFactory.CreateLogger<KnxReceiver>();
            KnxConnection = connection;
        }

        protected KnxConnection KnxConnection { get; }

        public abstract void ReceiverThreadFlow();

        private void ConsumerThreadFlow()
        {
            try
            {
                while (true)
                {
                    KnxDatagram datagram = null;

                    try
                    {
                        datagram = _rxDatagrams.Take();
                    }
                    catch (Exception e)
                    {
                        if (e is ThreadAbortException)
                            throw;
                    }

                    if (datagram != null)
                        KnxConnection.Event(new KnxEventArgs(datagram));
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        public void Start()
        {
            _rxDatagrams = new BlockingCollection<KnxDatagram>();
            _consumerThread = new Thread(ConsumerThreadFlow) { Name = "KnxEventConsumerThread", IsBackground = true };
            _consumerThread.Start();

            _receiverThread = new Thread(ReceiverThreadFlow) { Name = "KnxReceiverThread", IsBackground = true };
            _receiverThread.Start();

            _logger.LogDebug("Started Consumer and Receiver threads.");
        }

        public void Stop()
        {
            try
            {
                if (_receiverThread.ThreadState.Equals(StateAlive))
                    _receiverThread.Abort();
            }
            catch
            {
                Thread.ResetAbort();
            }

            try
            {
                if (_consumerThread.ThreadState.Equals(StateAlive))
                    _consumerThread.Abort();
            }
            catch
            {
                Thread.ResetAbort();
            }

            _rxDatagrams.Dispose();
        }

        protected void ProcessCEMI(KnxDatagram datagram, byte[] cemi)
        {
            try
            {
                // CEMI
                // +--------+--------+--------+--------+----------------+----------------+--------+----------------+
                // |  Msg   |Add.Info| Ctrl 1 | Ctrl 2 | Source Address | Dest. Address  |  Data  |      APDU      |
                // | Code   | Length |        |        |                |                | Length |                |
                // +--------+--------+--------+--------+----------------+----------------+--------+----------------+
                //   1 byte   1 byte   1 byte   1 byte      2 bytes          2 bytes       1 byte      2 bytes
                //
                //  Message Code    = 0x11 - a L_Data.req primitive
                //      COMMON EMI MESSAGE CODES FOR DATA LINK LAYER PRIMITIVES
                //          FROM NETWORK LAYER TO DATA LINK LAYER
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          | Data Link Layer Primitive | Message Code | Data Link Layer Service | Service Description | Common EMI Frame |
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          |        L_Raw.req          |    0x10      |                         |                     |                  |
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          |                           |              |                         | Primitive used for  | Sample Common    |
                //          |        L_Data.req         |    0x11      |      Data Service       | transmitting a data | EMI frame        |
                //          |                           |              |                         | frame               |                  |
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          |        L_Poll_Data.req    |    0x13      |    Poll Data Service    |                     |                  |
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          |        L_Raw.req          |    0x10      |                         |                     |                  |
                //          +---------------------------+--------------+-------------------------+---------------------+------------------+
                //          FROM DATA LINK LAYER TO NETWORK LAYER
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          | Data Link Layer Primitive | Message Code | Data Link Layer Service | Service Description |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |        L_Poll_Data.con    |    0x25      |    Poll Data Service    |                     |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |                           |              |                         | Primitive used for  |
                //          |        L_Data.ind         |    0x29      |      Data Service       | receiving a data    |
                //          |                           |              |                         | frame               |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |        L_Busmon.ind       |    0x2B      |   Bus Monitor Service   |                     |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |        L_Raw.ind          |    0x2D      |                         |                     |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |                           |              |                         | Primitive used for  |
                //          |                           |              |                         | local confirmation  |
                //          |        L_Data.con         |    0x2E      |      Data Service       | that a frame was    |
                //          |                           |              |                         | sent (does not mean |
                //          |                           |              |                         | successful receive) |
                //          +---------------------------+--------------+-------------------------+---------------------+
                //          |        L_Raw.con          |    0x2F      |                         |                     |
                //          +---------------------------+--------------+-------------------------+---------------------+

                //  Add.Info Length = 0x00 - no additional info
                //  Control Field 1 = see the bit structure above
                //  Control Field 2 = see the bit structure above
                //  Source Address  = 0x0000 - filled in by router/gateway with its source address which is
                //                    part of the KNX subnet
                //  Dest. Address   = KNX group or individual address (2 byte)
                //  Data Length     = Number of bytes of data in the APDU excluding the TPCI/APCI bits
                //  APDU            = Application Protocol Data Unit - the actual payload including transport
                //                    protocol control information (TPCI), application protocol control
                //                    information (APCI) and data passed as an argument from higher layers of
                //                    the KNX communication stack
                //
                datagram.message_code = cemi[0];
                datagram.aditional_info_length = cemi[1];

                if (datagram.aditional_info_length > 0)
                {
                    datagram.aditional_info = new byte[datagram.aditional_info_length];
                    for (var i = 0; i < datagram.aditional_info_length; i++)
                        datagram.aditional_info[i] = cemi[2 + i];
                }

                datagram.control_field_1 = new KnxControlField1(cemi[2 + datagram.aditional_info_length]);
                datagram.control_field_2 = new KnxControlField2(cemi[3 + datagram.aditional_info_length]);
                datagram.source_address = KnxIndividualAddress.Parse(new[] { cemi[4 + datagram.aditional_info_length], cemi[5 + datagram.aditional_info_length] });

                if (datagram.control_field_2.DestinationAddressType == KnxDestinationAddressType.Individual)
                {
                    // Destination is PA
                    datagram.destination_address = KnxIndividualAddress.Parse(new[] { cemi[6 + datagram.aditional_info_length], cemi[7 + datagram.aditional_info_length] });
                }
                else
                {
                    // Destination is GA
                    datagram.destination_address = KnxGroupAddress.Parse(new[] { cemi[6 + datagram.aditional_info_length], cemi[7 + datagram.aditional_info_length] }, KnxConnection.GroupAddressStyle);
                }

                datagram.data_length = cemi[8 + datagram.aditional_info_length];
                datagram.apdu = new byte[datagram.data_length + 1];

                for (var i = 0; i < datagram.apdu.Length; i++)
                    datagram.apdu[i] = cemi[9 + i + datagram.aditional_info_length];

                datagram.data = KnxHelper.GetData(datagram.data_length, datagram.apdu);

                if (KnxConnection.Debug)
                {
                    _logger.LogDebug("-----------------------------------------------------------------------------------------------------");
                    _logger.LogDebug(BitConverter.ToString(cemi));
                    _logger.LogDebug("Event Header Length: {HeaderLength}", datagram.header_length);
                    _logger.LogDebug("Event Protocol Version: {ProtocolVersion}", datagram.protocol_version);
                    _logger.LogDebug("Event Service Type: {ServiceType}", "0x" + BitConverter.ToString(datagram.service_type).Replace("-", string.Empty));
                    _logger.LogDebug("Event Total Length: {TotalLength}", datagram.total_length);

                    _logger.LogDebug("Event Message Code: {MessageCode}", datagram.message_code);
                    _logger.LogDebug("Event Aditional Info Length: {AdditionalInfoLength}", datagram.aditional_info_length);

                    if (datagram.aditional_info_length > 0)
                        _logger.LogDebug(
                            "Event Aditional Info: {AdditionalInfo}",
                            "0x" + BitConverter.ToString(datagram.aditional_info).Replace("-", string.Empty));

                    _logger.LogDebug("Event Control Field 1: {ControlField1}", datagram.control_field_1);
                    _logger.LogDebug("Event Control Field 2: {ControlField2}", datagram.control_field_2);
                    _logger.LogDebug("Event Source Address: {SourceAddress}", datagram.source_address);
                    _logger.LogDebug("Event Destination Address: {DestinationAddress}", datagram.destination_address);
                    _logger.LogDebug("Event Data Length: {DataLength}", datagram.data_length);
                    _logger.LogDebug("Event APDU: {APDU}", "0x" + BitConverter.ToString(datagram.apdu).Replace("-", string.Empty));
                    _logger.LogDebug("Event Data: {Data}", "0x" + string.Join(string.Empty, datagram.data.Select(c => ((int) c).ToString("X2"))));
                    _logger.LogDebug("-----------------------------------------------------------------------------------------------------");
                }

                if (datagram.message_code != 0x29)
                    return;

                var type = datagram.apdu[1] >> 4;

                switch (type)
                {
                    case 8:
                        _rxDatagrams.Add(datagram);
                        break;
                    case 4:
                        KnxConnection.Status(new KnxStatusArgs(datagram));
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ProcessCEMI failed.");
            }
        }
    }
}
