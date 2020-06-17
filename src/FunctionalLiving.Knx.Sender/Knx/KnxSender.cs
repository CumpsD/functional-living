namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Addressing;
    using Infrastructure.Modules;
    using Infrastructure.Toggles;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class KnxConfiguration
    {
        public const string ConfigurationPath = "Knx";

        public string? RouterIp { get; set; }
        public int RouterPort { get; set; } = 3671;
        public string? LocalIp { get; set; }
        public int LocalPort { get; set; } = 3671;
    }

    public class KnxSender
    {
        private readonly ILogger<KnxSender> _logger;

        private readonly KnxConnection? _connection;

        public KnxSender(
            ILoggerFactory loggerFactory,
            ILogger<KnxSender> logger,
            IOptions<KnxConfiguration> knxConfiguration,
            UseKnxConnectionRouting useKnxConnectionRouting,
            UseKnxConnectionTunneling useKnxConnectionTunneling,
            DebugKnxCemi debugKnxCemi)
        {
            _logger = logger;

            if (useKnxConnectionTunneling.FeatureEnabled && useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Cannot enable Tunneling and Routing simultaneously.");

            if (!useKnxConnectionTunneling.FeatureEnabled && !useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Either enable Tunneling or Routing");

            if (useKnxConnectionTunneling.FeatureEnabled)
                _connection = SetupTunnelingConnection(loggerFactory, logger, knxConfiguration, debugKnxCemi);

            if (useKnxConnectionRouting.FeatureEnabled)
                _connection = SetupRoutingConnection(loggerFactory, logger, debugKnxCemi);

            _connection!.SetLockIntervalMs(20);
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += () => Disconnected(_connection);
        }

        private KnxConnection SetupTunnelingConnection(
            ILoggerFactory loggerFactory,
            ILogger<KnxSender> logger,
            IOptions<KnxConfiguration> knxConfiguration,
            DebugKnxCemi debugKnxCemi)
        {
            var knxConfig = knxConfiguration.Value;

            CheckRouterIp(knxConfig);
            CheckLocalIp(knxConfig);

            logger.LogInformation(
                "Using '{ConnectionMode}' from '{LocalIp}:{LocalPort}' to '{RouterIp}:{RouterPort}'.",
                "Tunneling",
                knxConfig.LocalIp,
                knxConfig.LocalPort,
                knxConfig.RouterIp,
                knxConfig.RouterPort);

            return new KnxConnectionTunneling(
                loggerFactory,
                knxConfig.RouterIp ?? throw new ArgumentException("RouterIp is not defined."),
                knxConfig.RouterPort,
                knxConfig.LocalIp ?? throw new ArgumentException("LocalIp is not defined and failed to automatically determine."),
                knxConfig.LocalPort)
            {
                Debug = debugKnxCemi.FeatureEnabled,
            };
        }

        private KnxConnection SetupRoutingConnection(
            ILoggerFactory loggerFactory,
            ILogger<KnxSender> logger,
            DebugKnxCemi debugKnxCemi)
        {
            logger.LogInformation(
                "Using '{ConnectionMode}'.",
                "Routing");

            return new KnxConnectionRouting(loggerFactory)
            {
                Debug = debugKnxCemi.FeatureEnabled,
                ActionMessageCode = 0x29
            };
        }

        private static void CheckRouterIp(KnxConfiguration knxConfig)
        {
            if (string.IsNullOrWhiteSpace(knxConfig.RouterIp))
                throw new ArgumentException("RouterIp is not defined.");
        }

        private void CheckLocalIp(KnxConfiguration knxConfig)
        {
            if (!string.IsNullOrWhiteSpace(knxConfig.LocalIp))
                return;

            knxConfig.LocalIp = GetLocalIpAddress();

            if (string.IsNullOrWhiteSpace(knxConfig.LocalIp))
                throw new Exception("LocalIp is not defined and failed to automatically determine.");

            _logger.LogInformation(
                "No LocalIp defined, automatically determined '{LocalIp}'.",
                knxConfig.LocalIp);
        }

        public void Start()
            => _connection!.Connect();

        public void Action(KnxAddress address, bool data)
        {
            _logger.LogInformation(
                "Sending message to Group '{DestinationAddress}' ~> '{State}'",
                address.ToString(),
                data);

            _connection.Action(address, data);
        }

        public void Action(KnxAddress address, string data)
        {
            _logger.LogInformation(
                "Sending message to Group '{DestinationAddress}' ~> '{State}'",
                address.ToString(),
                data);

            _connection.Action(address, data);
        }

        public void Action(KnxAddress address, int data)
        {
            _logger.LogInformation(
                "Sending message to Group '{DestinationAddress}' ~> '{State}'",
                address.ToString(),
                data);

            _connection.Action(address, data);
        }

        public void Action(KnxAddress address, byte data)
        {
             _logger.LogInformation(
                "Sending message to Group '{DestinationAddress}' ~> '{State}'",
                address.ToString(),
                "0x" + BitConverter.ToString(new byte[] { data }).Replace("-", string.Empty));

            _connection.Action(address, data);
        }

        public void Action(KnxAddress address, byte[] data)
        {
            _logger.LogInformation(
                "Sending message to Group '{DestinationAddress}' ~> '{State}'",
                address.ToString(),
                "0x" + BitConverter.ToString(data).Replace("-", string.Empty));

            _connection.Action(address, data);
        }

        private void Connected()
            => _logger.LogInformation("Connected!");

        private void Disconnected(KnxConnection connection)
        {
            _logger.LogInformation("Disconnected! Reconnecting");
            if (connection == null)
                return;

            Thread.Sleep(1000);
            connection.Connect();
        }

        private string GetLocalIpAddress()
        {
            UnicastIPAddressInformation? mostSuitableIp = null;

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                var properties = network.GetIPProperties();

                _logger.LogDebug(
                    "Checking network interface {NetworkInterface}. Status: '{Status}', GatewayAddresses: '{GatewayAddressesCount}', UnicastAddresses: '{UnicastAddresses}'.",
                    network.Description,
                    network.OperationalStatus,
                    properties.GatewayAddresses.Count,
                    properties.UnicastAddresses.Count);

                if (network.OperationalStatus != OperationalStatus.Up &&
                    network.OperationalStatus != OperationalStatus.Unknown)
                    continue;

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    return address.Address.ToString();
                }
            }

            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }
    }
}
