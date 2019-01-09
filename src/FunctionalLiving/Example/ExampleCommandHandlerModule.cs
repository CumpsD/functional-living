namespace FunctionalLiving.Example
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;

    public sealed class ExampleCommandHandlerModule : CommandHandlerModule
    {
        public ExampleCommandHandlerModule(
            Func<IExamples> getExamples,
            ReturnHandler<CommandMessage> finalHandler = null) : base(finalHandler)
        {
            For<DoExample>()
                .Handle(async (message, ct) =>
                {
                    var examples = getExamples();

                    var exampleId = message.Command.ExampleId;
                    var possibleExample = await examples.GetOptionalAsync(exampleId, ct);

                    if (!possibleExample.HasValue)
                    {
                        possibleExample = new Optional<Example>(Example.Register(exampleId));
                        examples.Add(exampleId, possibleExample.Value);
                    }

                    var example = possibleExample.Value;

                    example.DoExample(message.Command.Name);
                });
        }
    }
}
