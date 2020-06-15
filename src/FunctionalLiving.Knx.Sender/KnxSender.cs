namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Addressing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Infrastructure.Modules;
    using Infrastructure.Toggles;
    using FunctionalLiving.Knx.Log;

    public class KnxConfiguration
    {
        public const string ConfigurationPath = "Knx";

        public string RouterIp { get; set; }
        public int RouterPort { get; set; } = 3671;
        public string LocalIp { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 3671;
    }

    public class KnxSender
    {
        private readonly ILogger<KnxSender> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KnxConnection _connection;
        private readonly SendToLog _sendToLog;
        private readonly SendToApi _sendToApi;

        public KnxSender(
            ILogger<KnxSender> logger,
            ILogger<KnxConnection> knxLogger,
            IHttpClientFactory httpClientFactory,
            IOptions<KnxConfiguration> knxConfiguration,
            SendToLog sendToLog,
            SendToApi sendToApi,
            UseKnxConnectionRouting useKnxConnectionRouting,
            UseKnxConnectionTunneling useKnxConnectionTunneling,
            DebugKnx debugKnx)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            _sendToLog = sendToLog;
            _sendToApi = sendToApi;

            Logger.DebugEventEndpoint = (id, message) => { knxLogger.LogDebug(message); };
            Logger.InfoEventEndpoint = (id, message) => { knxLogger.LogInformation(message); };
            Logger.WarnEventEndpoint = (id, message) => { knxLogger.LogWarning(message); };
            Logger.ErrorEventEndpoint = (id, message) => { knxLogger.LogError(message); };

            if (useKnxConnectionTunneling.FeatureEnabled && useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Cannot enable Tunneling and Routing simultaneously.");

            if (!useKnxConnectionTunneling.FeatureEnabled && !useKnxConnectionRouting.FeatureEnabled)
                throw new Exception("Either enable Tunneling or Routing");

            if (useKnxConnectionTunneling.FeatureEnabled)
            {
                _connection = new KnxConnectionTunneling(
                    knxConfiguration.Value.RouterIp,
                    knxConfiguration.Value.RouterPort,
                    knxConfiguration.Value.LocalIp,
                    knxConfiguration.Value.LocalPort)
                {
                    Debug = debugKnx.FeatureEnabled,
                };
            }

            if (useKnxConnectionRouting.FeatureEnabled)
            {
                _connection = new KnxConnectionRouting
                {
                    Debug = debugKnx.FeatureEnabled,
                    ActionMessageCode = 0x29
                };
            }

            _connection.SetLockIntervalMs(20);
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += () => Disconnected(_connection);
            _connection.KnxEventDelegate += (sender, args) => Event(args.DestinationAddress, args.State);
            _connection.KnxStatusDelegate += (sender, args) => Status(args.DestinationAddress, args.State);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
            => _connection.Connect();

        private void Event(KnxAddress address, byte[] state)
            => Handle(address, state);

        private void Status(KnxAddress address, byte[] state)
            => Handle(address, state);

        private void Handle(KnxAddress knxAddress, byte[] state)
        {
            if (_sendToLog.FeatureEnabled)
                Print(knxAddress, state);

            if (_sendToApi.FeatureEnabled)
                SendToApi(knxAddress, state);
        }

        private void Print(KnxAddress knxAddress, byte[] state)
            => _logger.LogInformation("{address} - {state}", knxAddress.ToString(), BitConverter.ToString(state));

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

        private void SendToApi(KnxAddress knxAddress, byte[] state)
        {
            using (var client = _httpClientFactory.CreateClient(HttpModule.HttpClientName))
            using (var request = new HttpRequestMessage(HttpMethod.Post, "v1/knx"))
            using (var httpContent = CreateHttpContent(knxAddress, state))
            {
                request.Content = httpContent;

                using (var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                    response.EnsureSuccessStatusCode();
            }
        }

        private HttpContent CreateHttpContent(KnxAddress knxAddress, byte[] state)
        {
            var json = $@"{{ address: ""{knxAddress}"", state: ""{BitConverter.ToString(state)}"" }}";
            _logger.LogDebug("Sending Knx payload: {payload}", json);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }
}
