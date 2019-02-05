namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Addressing;
    using Microsoft.Extensions.Logging;

    public class KnxSender
    {
        private readonly ILogger<KnxSender> _logger;
        private readonly KnxConnectionRouting _connection;

        public KnxSender(ILogger<KnxSender> logger)
        {
            _logger = logger;

            _connection = new KnxConnectionRouting { Debug = false, ActionMessageCode = 0x29 };
            _connection.SetLockIntervalMs(20);
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += () => Disconnected(_connection);
            _connection.KnxEventDelegate += (sender, args) => Event(args.DestinationAddress, args.State);
            _connection.KnxStatusDelegate += (sender, args) => Status(args.DestinationAddress, args.State);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection.Connect();
        }

        private void Event(KnxAddress address, byte[] state) => Print(address, state);

        private void Status(KnxAddress address, byte[] state) => Print(address, state);

        private void Print(KnxAddress knxAddress, byte[] state)
        {
            var address = knxAddress.ToString();

            _logger.LogDebug("{address} - {state}", address, BitConverter.ToString(state));
        }

        private void Connected() => _logger.LogInformation("Connected!");

        private void Disconnected(KnxConnection connection)
        {
            _logger.LogInformation("Disconnected! Reconnecting");
            if (connection == null)
                return;

            Thread.Sleep(1000);
            connection.Connect();
        }
    }
}
