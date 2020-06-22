namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using Swashbuckle.AspNetCore.Filters;

    [Serializable]
    public class TurnOnLightResponse
    {
    }

    public class TurnOnLightResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples()
            => new object();
    }
}
