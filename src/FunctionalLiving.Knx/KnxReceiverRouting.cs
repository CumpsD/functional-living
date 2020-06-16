namespace FunctionalLiving.Knx
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Microsoft.Extensions.Logging;

    internal class KnxReceiverRouting : KnxReceiver
    {
        private readonly ILogger<KnxReceiverRouting> _logger;

        private readonly IList<UdpClient> _udpClients;

        internal KnxReceiverRouting(
            ILoggerFactory loggerFactory,
            KnxConnection connection,
            IList<UdpClient> udpClients)
            : base(loggerFactory, connection)
        {
            _logger = loggerFactory.CreateLogger<KnxReceiverRouting>();
            _udpClients = udpClients;
        }

        public override void ReceiverThreadFlow()
        {
            try
            {
                foreach (var client in _udpClients)
                    client.BeginReceive(OnReceive, new object[] { client });

                // just wait to be aborted
                while (true)
                    Thread.Sleep(60000);
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

        private void OnReceive(IAsyncResult result)
        {
            IPEndPoint endPoint = null;
            var args = (object[]) result.AsyncState;
            var session = (UdpClient) args[0];

            try
            {
                var datagram = session.EndReceive(result, ref endPoint);
                ProcessDatagram(datagram);

                // We make the next call to the begin receive
                session.BeginReceive(OnReceive, args);
            }
            catch (ObjectDisposedException)
            {
                // ignore and exit, session was disposed
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OnReceive failed.");
            }
        }

        private void ProcessDatagram(byte[] datagram)
        {
            try
            {
                ProcessDatagramHeaders(datagram);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ProcessDatagram failed.");
            }
        }

        private void ProcessDatagramHeaders(byte[] datagram)
        {
            // HEADER
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5]
            };

            var cemi = new byte[datagram.Length - datagram[0]];
            Array.Copy(datagram, datagram[0], cemi, 0, datagram.Length - datagram[0]);

            ProcessCEMI(knxDatagram, cemi);
        }
    }
}
