namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using ValueObjects;
    using Swashbuckle.AspNetCore.Filters;

    [Serializable]
    public class ListLightsResponse
    {
        public List<ListLightsItemResponse> Lights { get; }

        public ListLightsResponse(IEnumerable<Light> lights)
        {
            Lights = lights
                .Select(x => new ListLightsItemResponse(x))
                .ToList();
        }
    }

    [Serializable]
    public class ListLightsItemResponse
    {
        public LightId Id { get; }

        public string Description { get; }

        public string Status { get; }

        public ListLightsItemResponse(Light light)
        {
            Id = light.Id;
            Description = light.Description;
            Status = light.MapLightStatus();
        }
    }

    public class ListLightsResponseExamples : IExamplesProvider<object>
    {
        // TODO: Add examples
        public object GetExamples()
            => new { };
    }
}
