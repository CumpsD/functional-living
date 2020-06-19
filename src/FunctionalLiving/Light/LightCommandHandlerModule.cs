namespace FunctionalLiving.Light
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Domain;
    using Domain.Repositories;
    using Infrastructure;
    using Infrastructure.Modules;
    using Knx.Addressing;
    using Microsoft.Extensions.Logging;

    public sealed class LightCommandHandlerModule : CommandHandlerModule
    {
        private readonly ILogger<LightCommandHandlerModule> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public LightCommandHandlerModule(
            ILogger<LightCommandHandlerModule> logger,
            LightsRepository lightsRepository,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            For<TurnOnLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var light = lightsRepository.Lights.FirstOrDefault(x => x.Id == message.Command.LightId);

                    if (IsKnxLight(light))
                        await SendToKnx(light.KnxObject!.Address, true);
                });

            For<TurnOffLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var light = lightsRepository.Lights.FirstOrDefault(x => x.Id == message.Command.LightId);

                    if (IsKnxLight(light))
                        await SendToKnx(light.KnxObject!.Address, false);
                });
        }

        private static bool IsKnxLight(Light light)
            => light != null &&
               light.BackendType == HomeAutomationBackendType.Knx &&
               light.KnxObject != null;

        private async Task SendToKnx(
            KnxGroupAddress address,
            bool value)
        {
            using (var client = _httpClientFactory.CreateClient(HttpModule.KnxSender))
            using (var request = new HttpRequestMessage(HttpMethod.Post, "v1/knx"))
            using (var httpContent = CreateHttpContent(address, value))
            {
                request.Content = httpContent;

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    response.EnsureSuccessStatusCode();
            }
        }

        private HttpContent CreateHttpContent(
            KnxGroupAddress destinationAddress,
            bool value)
        {
            var json = $@"{{
                destinationAddress: ""{destinationAddress}"",
                dataType: ""bool"",
                state: ""{value}""
            }}";

            _logger.LogDebug("Sending Knx payload: {payload}", json);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }
}
