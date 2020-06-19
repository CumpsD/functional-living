namespace FunctionalLiving.ValueObjects
{
    using System;
    using Newtonsoft.Json;

    public class LightId : HomeAutomationObjectId
    {
        public LightId([JsonProperty("value")] Guid lightId) : base(lightId) { }
    }
}
