namespace FunctionalLiving.ValueObjects
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class LightId : GuidValueObject<LightId>
    {
        public LightId([JsonProperty("value")] Guid lightId) : base(lightId) { }
    }
}
