namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Net.Mime;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Addressing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Infrastructure.Modules;
    using Infrastructure.Toggles;

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
        private readonly IHttpClientFactory _httpClientFactory;
        private KnxConnection _connection;
        private readonly SendToLog _sendToLog;
        private readonly SendToApi _sendToApi;

        public KnxSender(
            ILoggerFactory loggerFactory,
            ILogger<KnxSender> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<KnxConfiguration> knxConfiguration,
            SendToLog sendToLog,
            SendToApi sendToApi,
            UseKnxConnectionRouting useKnxConnectionRouting,
            UseKnxConnectionTunneling useKnxConnectionTunneling,
            DebugKnxCemi debugKnxCemi)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            _sendToLog = sendToLog;
            _sendToApi = sendToApi;

            if (useKnxConnectionTunneling.FeatureEnabled && useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Cannot enable Tunneling and Routing simultaneously.");

            if (!useKnxConnectionTunneling.FeatureEnabled && !useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Either enable Tunneling or Routing");

            if (useKnxConnectionTunneling.FeatureEnabled)
                SetupTunnelingConnection(loggerFactory, logger, knxConfiguration, debugKnxCemi);

            if (useKnxConnectionRouting.FeatureEnabled)
                SetupRoutingConnection(loggerFactory, logger, debugKnxCemi);

            _connection.SetLockIntervalMs(20);
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += () => Disconnected(_connection);
            _connection.KnxEventDelegate += (sender, args) => Event(args.SourceAddress, args.DestinationAddress, args.State);
            _connection.KnxStatusDelegate += (sender, args) => Status(args.SourceAddress, args.DestinationAddress, args.State);
        }

        private void SetupTunnelingConnection(
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

            _connection = new KnxConnectionTunneling(
                loggerFactory,
                knxConfig.RouterIp,
                knxConfig.RouterPort,
                knxConfig.LocalIp,
                knxConfig.LocalPort)
            {
                Debug = debugKnxCemi.FeatureEnabled,
            };
        }

        private void SetupRoutingConnection(
            ILoggerFactory loggerFactory,
            ILogger<KnxSender> logger,
            DebugKnxCemi debugKnxCemi)
        {
            logger.LogInformation(
                "Using '{ConnectionMode}'.",
                "Routing");

            _connection = new KnxConnectionRouting(loggerFactory)
            {
                Debug = debugKnxCemi.FeatureEnabled,
                ActionMessageCode = 0x29
            };
        }

        private static void CheckRouterIp(KnxConfiguration knxConfig)
        {
            if (string.IsNullOrWhiteSpace(knxConfig.RouterIp))
                throw new Exception("RouterIp is not defined.");
        }

        private void CheckLocalIp(KnxConfiguration knxConfig)
        {
            if (!string.IsNullOrWhiteSpace(knxConfig.LocalIp))
                return;

            knxConfig.LocalIp = GetLocalIpAddress();

            if (string.IsNullOrWhiteSpace(knxConfig.RouterIp))
                throw new Exception("RouterIp is not defined and failed to automatically determine.");

            _logger.LogInformation(
                "No LocalIp defined, automatically determined '{LocalIp}'.",
                knxConfig.LocalIp);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
            => _connection.Connect();

        private void Event(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
            => Handle(sourceAddress, destinationAddress, state);

        private void Status(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
            => Handle(sourceAddress, destinationAddress, state);

        private void Handle(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
        {
            if (_sendToLog.FeatureEnabled)
                Print(sourceAddress, destinationAddress, state);

            if (_sendToApi.FeatureEnabled)
                SendToApi(sourceAddress, destinationAddress, state);
        }

        private void Print(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
            => _logger.LogInformation(
                "Receieved message from Device '{SourceAddress}' to Group '{DestinationAddress}' ~> '{State}'",
                sourceAddress.ToString(),
                destinationAddress.ToString(),
                BitConverter.ToString(state));

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

        private void SendToApi(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
        {
            using (var client = _httpClientFactory.CreateClient(HttpModule.HttpClientName))
            using (var request = new HttpRequestMessage(HttpMethod.Post, "v1/knx"))
            using (var httpContent = CreateHttpContent(sourceAddress, destinationAddress, state))
            {
                request.Content = httpContent;

                using (var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                    response.EnsureSuccessStatusCode();
            }
        }

        private HttpContent CreateHttpContent(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            byte[] state)
        {
            var json = $@"{{
                sourceAddress: ""{sourceAddress}"",
                destinationAddress: ""{destinationAddress}"",
                state: ""{BitConverter.ToString(state)}""
            }}";

            _logger.LogDebug("Sending Knx payload: {payload}", json);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
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
