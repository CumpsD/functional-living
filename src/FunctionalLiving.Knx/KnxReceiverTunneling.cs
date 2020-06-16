namespace FunctionalLiving.Knx
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Microsoft.Extensions.Logging;

    internal class KnxReceiverTunneling : KnxReceiver
    {
        private readonly ILogger<KnxReceiverTunneling> _logger;

        private UdpClient _udpClient;
        private IPEndPoint _localEndpoint;

        private readonly object _rxSequenceNumberLock = new object();
        private byte _rxSequenceNumber;

        internal KnxReceiverTunneling(
            ILoggerFactory loggerFactory,
            KnxConnection connection,
            UdpClient udpClient,
            IPEndPoint localEndpoint)
            : base(loggerFactory, connection)
        {
            _logger = loggerFactory.CreateLogger<KnxReceiverTunneling>();
            _udpClient = udpClient;
            _localEndpoint = localEndpoint;
        }

        private KnxConnectionTunneling KnxConnectionTunneling
            => (KnxConnectionTunneling) KnxConnection;

        public void SetClient(UdpClient client)
            => _udpClient = client;

        public override void ReceiverThreadFlow()
        {
            try
            {
                while (true)
                {
                    var datagram = _udpClient.Receive(ref _localEndpoint);

                    _logger.LogDebug(
                        "UDP Client Received: '{Datagram}'.",
                        "0x" + BitConverter.ToString(datagram).Replace("-", string.Empty));

                    ProcessDatagram(datagram);
                }
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "UDP Receive failed.");
                KnxConnectionTunneling.Disconnected();
            }
            catch (ObjectDisposedException)
            {
                // ignore, probably reconnect happening
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "UDP Receive failed.");
            }
        }

        private void ProcessDatagram(byte[] datagram)
        {
            try
            {
                _logger.LogDebug(
                    "Processing datagram '{Datagram}'.",
                    KnxHelper.GetServiceType(datagram));

                switch (KnxHelper.GetServiceType(datagram))
                {
                    case KnxHelper.SERVICE_TYPE.CONNECT_RESPONSE:
                        ProcessConnectResponse(datagram);
                        break;

                    case KnxHelper.SERVICE_TYPE.CONNECTIONSTATE_RESPONSE:
                        ProcessConnectionStateResponse(datagram);
                        break;

                    case KnxHelper.SERVICE_TYPE.TUNNELLING_ACK:
                        ProcessTunnelingAck(datagram);
                        break;

                    case KnxHelper.SERVICE_TYPE.DISCONNECT_REQUEST:
                        ProcessDisconnectRequest(datagram);
                        break;

                    case KnxHelper.SERVICE_TYPE.DISCONNECT_RESPONSE:
                        ProcessDisconnectResponse(datagram);
                        break;

                    case KnxHelper.SERVICE_TYPE.TUNNELLING_REQUEST:
                        ProcessDatagramHeaders(datagram);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ProcessDatagram failed.");
            }
        }

        private void ProcessDatagramHeaders(byte[] datagram)
        {
            // HEADER
            // TODO: Might be interesting to take out these magic numbers for the datagram indices
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5]
            };

            var channelId = datagram[7];
            if (channelId != KnxConnectionTunneling.ChannelId)
                return;

            var sequenceNumber = datagram[8];
            var process = true;
            lock (_rxSequenceNumberLock)
            {
                if (sequenceNumber <= _rxSequenceNumber)
                    process = false;

                _rxSequenceNumber = sequenceNumber;
            }

            if (process)
            {
                // TODO: Magic number 10, what is it?
                var cemi = new byte[datagram.Length - 10];
                Array.Copy(datagram, 10, cemi, 0, datagram.Length - 10);

                ProcessCEMI(knxDatagram, cemi);
            }

            ((KnxSenderTunneling) KnxConnectionTunneling.KnxSender).SendTunnelingAck(sequenceNumber);
        }

        private void ProcessDisconnectRequest(byte[] datagram)
            => KnxConnectionTunneling.DisconnectRequest();

        private void ProcessDisconnectResponse(byte[] datagram)
        {
            var channelId = datagram[6];
            if (channelId != KnxConnectionTunneling.ChannelId)
                return;

            KnxConnectionTunneling.Disconnect();
        }

        // do nothing
        private void ProcessTunnelingAck(byte[] datagram) { }

        private void ProcessConnectionStateResponse(byte[] datagram)
        {
            // HEADER
            // 06 10 02 08 00 08 -- 48 21
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5],
                channel_id = datagram[6]
            };

            var response = datagram[7];

            if (response != 0x21)
                return;

            _logger.LogDebug(
                "Received connection state response - No active connection with channel ID {ChannelId}",
                knxDatagram.channel_id);

            KnxConnection.Disconnect();
        }

        private void ProcessConnectResponse(byte[] datagram)
        {
            // HEADER
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5],
                channel_id = datagram[6],
                status = datagram[7]
            };

            if (knxDatagram.channel_id == 0x00 && knxDatagram.status == 0x24)
            {
                _logger.LogInformation(
                    "KNXLib received connect response - No more connections available");
            }
            else
            {
                KnxConnectionTunneling.ChannelId = knxDatagram.channel_id;
                KnxConnectionTunneling.ResetSequenceNumber();

                KnxConnectionTunneling.Connected();
            }
        }
    }
}
