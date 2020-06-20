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
    using Knx;
    using Knx.Addressing;
    using Knx.Commands;
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

            var knxLightsFeedback = lightsRepository
                .Lights
                .Where(x =>
                    IsKnxLight(x) &&
                    x.KnxFeedbackObject?.FeedbackAddress != null)
                .ToDictionary(x => x.KnxFeedbackObject!.FeedbackAddress!, x => x);

            For<TurnOnLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var light = lightsRepository.Lights.SingleOrDefault(x => x.Id == message.Command.LightId);

                    if (IsKnxLight(light))
                        await SendToKnx(light.KnxObject!.Address, true);
                });

            For<TurnOffLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var light = lightsRepository.Lights.SingleOrDefault(x => x.Id == message.Command.LightId);

                    if (IsKnxLight(light))
                        await SendToKnx(light.KnxObject!.Address, false);
                });

            //For<KnxCommand>()
            //    .AddLogging(logger)
            //    .Handle(async (message, ct) =>
            //    {
            //        var groupAddress = message.Command.Group; // e.g. 1/0/1
            //        var state = message.Command.State;

            //        knxLightsFeedback.ProcessKnxSingleBit(
            //            groupAddress,
            //            state,
            //            (light, value) =>
            //            {
            //                light.Status = value switch
            //                {
            //                    true => LightStatus.On,
            //                    false => LightStatus.Off,
            //                    null => LightStatus.Unknown
            //                };
            //            });
            //    });
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
