namespace FunctionalLiving.Example
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;

    public sealed class ExampleCommandHandlerModule : CommandHandlerModule
    {
        public ExampleCommandHandlerModule()
        {
            For<DoExample>()
                .Handle(async (message, ct) =>
                {
                    Console.WriteLine("Hey");
                });
        }
    }
}
