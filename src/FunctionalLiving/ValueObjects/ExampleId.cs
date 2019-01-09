namespace FunctionalLiving
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class ExampleId : GuidValueObject<ExampleId>
    {
        public ExampleId([JsonProperty("value")] Guid exampleId) : base(exampleId)
        {
            if (exampleId == Guid.Empty)
                throw new NoExampleIdException("ExampleId of an example cannot be empty.");
        }
    }
}
