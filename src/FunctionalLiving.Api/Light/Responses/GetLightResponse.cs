namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using Domain;
    using Swashbuckle.AspNetCore.Filters;
    using ValueObjects;

    [Serializable]
    public class GetLightResponse
    {
        public LightId Id { get; }

        public string Description { get; }

        public string Status { get; }

        public GetLightResponse(Light light)
        {
            Id = light.Id;
            Description = light.Description;
            Status = light.MapLightStatus();
        }
    }

    public class GetLightResponseExamples : IExamplesProvider<object>
    {
        // TODO: Add examples
        public object GetExamples()
            => new { };
    }

    public class LightNotFoundResponseExamples : IExamplesProvider<object>
    {
        // TODO: Add examples
        public object GetExamples()
            => new { };
    }
}
