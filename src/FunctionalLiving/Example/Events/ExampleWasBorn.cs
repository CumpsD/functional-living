namespace FunctionalLiving.Example.Events
{
    using System;
    using Newtonsoft.Json;
    using Be.Vlaanderen.Basisregisters.EventHandling;

    [EventName("ExampleWasBorn")]
    [EventDescription("The example was born!")]
    public class ExampleWasBorn
    {
        public Guid ExampleId { get; }

        public ExampleWasBorn(
            ExampleId exampleId)
        {
            ExampleId = exampleId;
        }

        [JsonConstructor]
        private ExampleWasBorn(
            Guid exampleId)
            : this(
                new ExampleId(exampleId)) {}
    }
}
