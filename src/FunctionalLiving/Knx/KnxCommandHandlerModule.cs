namespace FunctionalLiving.Knx
{
    using System;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        public KnxCommandHandlerModule()
        {
            For<KnxCommand>()
                .Handle(async (message, ct) =>
                {
                    Console.WriteLine("Hey");
                });
        }
    }
}
