namespace FunctionalLiving.Light
{
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Infrastructure;
    using Microsoft.Extensions.Logging;

    public sealed class LightCommandHandlerModule : CommandHandlerModule
    {
        public LightCommandHandlerModule(
            ILogger<LightCommandHandlerModule> logger)
        {
            For<TurnOnLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    // TODO: Send command to knx-sender
                });

            For<TurnOffLightCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    // TODO: Send command to knx-sender
                });
        }
    }
}
