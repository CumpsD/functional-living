namespace FunctionalLiving.Light
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Domain;
    using Domain.Repositories;
    using Infrastructure.Toggles;
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
        private readonly ILightHub _lightHub;

        private readonly SendToKnxSender _sendToKnxSender;
        private readonly SendToSignalR _sendToSignalR;

        private Dictionary<KnxGroupAddress, Light> _knxLightsFeedback;

        public LightCommandHandlerModule(
            ILogger<LightCommandHandlerModule> logger,
            LightsRepository lightsRepository,
            IHttpClientFactory httpClientFactory,
            ILightHub lightHub,
            SendToKnxSender sendToKnxSender,
            SendToSignalR sendToSignalR)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _lightHub = lightHub;

            _sendToKnxSender = sendToKnxSender;
            _sendToSignalR = sendToSignalR;

            _knxLightsFeedback = lightsRepository
                .Lights
                .Where(x =>
                    IsKnxLight(x) &&
                    x.KnxFeedbackObject?.Address != null)
                .ToDictionary(x => x.KnxFeedbackObject!.Address!, x => x);

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

            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var groupAddress = message.Command.Group; // e.g. 1/0/1
                    var state = message.Command.State;

                    await ProcessKnxFeedback(groupAddress, state);
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
            if (!_sendToKnxSender.FeatureEnabled)
                return;

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

        private async Task ProcessKnxFeedback(KnxGroupAddress groupAddress, byte[] state)
        {
            await _knxLightsFeedback.ProcessKnxSingleBit(
                groupAddress,
                state,
                async (light, value) =>
                {
                    light.Status = value switch
                    {
                        true => LightStatus.On,
                        false => LightStatus.Off,
                        null => LightStatus.Unknown
                    };

                    await SendToSignalR(light);
                });
        }

        private async Task SendToSignalR(Light light)
        {
            if (!_sendToSignalR.FeatureEnabled)
                return;

            switch (light.Status)
            {
                case LightStatus.Unknown:
                    await _lightHub.SendLightTurnedUnknownMessage(light.Id);
                    break;

                case LightStatus.On:
                    await _lightHub.SendLightTurnedOnMessage(light.Id);
                    break;

                case LightStatus.Off:
                    await _lightHub.SendLightTurnedOffMessage(light.Id);
                    break;
            }
        }
    }
}
