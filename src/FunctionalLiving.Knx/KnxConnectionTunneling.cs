namespace FunctionalLiving.Knx
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;
    using Exceptions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Class that controls a Tunneling KNX connection, a tunneling connection is UDP based and has state.
    ///     This class will connect to the remote gateway provided and create an endpoint for the remote gateway
    ///     to connect back
    /// </summary>
    public class KnxConnectionTunneling : KnxConnection
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<KnxConnectionTunneling> _logger;

        private readonly IPEndPoint _localEndpoint;
        private readonly Timer _stateRequestTimer;
        private const int StateRequestTimerInterval = 60000;
        private UdpClient _udpClient;
        private byte _sequenceNumber;

        /// <summary>
        ///     Initializes a new KNX tunneling connection with provided values. Make sure the local system allows
        ///     UDP messages to the localIpAddress and localPort provided
        /// </summary>
        /// <param name="remoteIpAddress">Remote gateway IP address</param>
        /// <param name="remotePort">Remote gateway port</param>
        /// <param name="localIpAddress">Local IP address to bind to</param>
        /// <param name="localPort">Local port to bind to</param>
        public KnxConnectionTunneling(
            ILoggerFactory loggerFactory,
            string remoteIpAddress,
            int remotePort,
            string localIpAddress,
            int localPort)
            : base(loggerFactory, remoteIpAddress, remotePort)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<KnxConnectionTunneling>();
            _localEndpoint = new IPEndPoint(IPAddress.Parse(localIpAddress), localPort);

            ChannelId = 0x00;
            SequenceNumberLock = new object();
            _stateRequestTimer = new Timer(StateRequestTimerInterval) { AutoReset = true }; // same time as ETS with group monitor open
            _stateRequestTimer.Elapsed += StateRequest;
        }

        internal byte ChannelId { get; set; }

        internal object SequenceNumberLock { get; set; }

        internal byte GenerateSequenceNumber() => _sequenceNumber++;

        internal void RevertSingleSequenceNumber() => _sequenceNumber--;

        internal void ResetSequenceNumber()
        {
            _logger.LogDebug("Resetting sequence number.");
            _sequenceNumber = 0x00;
        }

        /// <summary>
        ///     Start the connection
        /// </summary>
        public override void Connect()
        {
            _logger.LogInformation(
                "Connecting to Knx, using local endpoint '{LocalEndpoint}'.",
                _localEndpoint);

            try
            {
                if (_udpClient != null)
                {
                    try
                    {
                        _udpClient.Close();
                        _udpClient.Client?.Dispose();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "UDP Client creation failed.");
                    }
                }

                _udpClient = new UdpClient(_localEndpoint)
                {
                    Client =
                    {
                        DontFragment = true,
                        SendBufferSize = 0,
                        ReceiveTimeout = StateRequestTimerInterval * 2
                    }
                };

                _logger.LogDebug("UDP Client created.");
            }
            catch (SocketException ex)
            {
                throw new ConnectionErrorException(ConnectionConfiguration, ex);
            }

            if (KnxReceiver == null || KnxSender == null)
            {
                KnxReceiver = new KnxReceiverTunneling(_loggerFactory, this, _udpClient, _localEndpoint);
                KnxSender = new KnxSenderTunneling(_loggerFactory, this, _udpClient, RemoteEndpoint);
            }
            else
            {
                ((KnxReceiverTunneling) KnxReceiver).SetClient(_udpClient);
                ((KnxSenderTunneling) KnxSender).SetClient(_udpClient);
            }

            KnxReceiver.Start();

            try
            {
                ConnectRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Connect failed.");
            }
        }

        /// <summary>
        ///     Stop the connection
        /// </summary>
        public override void Disconnect()
        {
            try
            {
                TerminateStateRequest();
                DisconnectRequest();
                KnxReceiver.Stop();
                _udpClient.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Disconnect failed.");
            }

            base.Disconnected();
        }

        internal override void Connected()
        {
            _logger.LogDebug("Knx Tunneling Connected.");

            base.Connected();

            InitializeStateRequest();
        }

        internal override void Disconnected()
        {
            _logger.LogDebug("Knx Tunneling Disconnected.");

            base.Disconnected();

            TerminateStateRequest();
        }

        private void InitializeStateRequest() => _stateRequestTimer.Enabled = true;

        private void TerminateStateRequest()
        {
            if (_stateRequestTimer == null)
                return;

            _stateRequestTimer.Enabled = false;
        }

        // TODO: I wonder if we can extract all these types of requests
        private void ConnectRequest()
        {
            _logger.LogDebug("Sending '{DatagramType}' datagram.", "Connect");

            // HEADER
            var datagram = new byte[26];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x05;
            datagram[04] = 0x00;
            datagram[05] = 0x1A;

            datagram[06] = 0x08;
            datagram[07] = 0x01;
            datagram[08] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[09] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[12] = (byte) (_localEndpoint.Port >> 8);
            datagram[13] = (byte) _localEndpoint.Port;
            datagram[14] = 0x08;
            datagram[15] = 0x01;
            datagram[16] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[17] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[18] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[19] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[20] = (byte) (_localEndpoint.Port >> 8);
            datagram[21] = (byte) _localEndpoint.Port;
            datagram[22] = 0x04;
            datagram[23] = 0x04;
            datagram[24] = 0x02;
            datagram[25] = 0x00;

            try
            {
                ((KnxSenderTunneling) KnxSender).SendDataSingle(datagram);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ConnectRequest SendData failed.");
            }
        }

        private void StateRequest(object sender, ElapsedEventArgs ev)
        {
            _logger.LogDebug("Sending '{DatagramType}' datagram.", "State");

            // HEADER
            var datagram = new byte[16];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x07;
            datagram[04] = 0x00;
            datagram[05] = 0x10;

            datagram[06] = ChannelId;
            datagram[07] = 0x00;
            datagram[08] = 0x08;
            datagram[09] = 0x01;
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[12] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[13] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[14] = (byte) (_localEndpoint.Port >> 8);
            datagram[15] = (byte) _localEndpoint.Port;

            try
            {
                KnxSender.SendData(datagram);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "StateRequest SendData failed.");
            }
        }

        internal void DisconnectRequest()
        {
            _logger.LogDebug("Sending '{DatagramType}' datagram.", "Disconnect");

            // HEADER
            var datagram = new byte[16];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x09;
            datagram[04] = 0x00;
            datagram[05] = 0x10;

            datagram[06] = ChannelId;
            datagram[07] = 0x00;
            datagram[08] = 0x08;
            datagram[09] = 0x01;
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[12] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[13] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[14] = (byte) (_localEndpoint.Port >> 8);
            datagram[15] = (byte) _localEndpoint.Port;

            try
            {
                KnxSender.SendData(datagram);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DisconnectRequest SendData failed.");
            }
        }

        public override void Dispose() => Dispose(true);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Disconnect();
        }
    }
}
