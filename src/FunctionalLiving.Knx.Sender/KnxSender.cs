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
    using Modules;

    public class KnxSender
    {
        private readonly ILogger<KnxSender> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KnxConnectionRouting _connection;

        public KnxSender(
            ILogger<KnxSender> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

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
            _logger.LogDebug("{address} - {state}", knxAddress.ToString(), BitConverter.ToString(state));

            SendToApi(knxAddress, state);
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
