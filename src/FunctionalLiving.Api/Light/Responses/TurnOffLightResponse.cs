namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using Swashbuckle.AspNetCore.Filters;

    [Serializable]
    public class TurnOffLightResponse
    {
    }

    public class TurnOffLightResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples()
            => new object();
    }
}
