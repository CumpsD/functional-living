namespace FunctionalLiving.Knx
{
    using System;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Infrastructure;
    using Microsoft.Extensions.Logging;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        public KnxCommandHandlerModule(ILogger<KnxCommandHandlerModule> logger)
        {
            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {

                });
        }
    }
}
