namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Swashbuckle.AspNetCore.Filters;
    using Newtonsoft.Json;

    [Serializable]
    public class ListLightsResponse
    {
        /// <summary>All available lights.</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public List<ListLightsItemResponse> Lights { get; }

        public ListLightsResponse(IEnumerable<Light> lights)
            : this(lights.Select(x => new ListLightsItemResponse(x))) { }

        internal ListLightsResponse(IEnumerable<ListLightsItemResponse> lights)
        {
            Lights = lights.ToList();
        }
    }

    [Serializable]
    public class ListLightsItemResponse
    {
        /// <summary>Id of the light.</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Guid Id { get; }

        /// <summary>Description of the light.</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string Description { get; }

        [JsonProperty(Required = Required.DisallowNull)]
        public LightStatus Status { get; }

        public ListLightsItemResponse(Light light)
            : this(
                light.Id,
                light.Description,
                light.Status) { }

        internal ListLightsItemResponse(
            Guid lightId,
            string description,
            LightStatus status)
        {
            Id = lightId;
            Description = description;
            Status = status;
        }
    }

    public class ListLightsResponseExamples : IExamplesProvider<ListLightsResponse>
    {
        public ListLightsResponse GetExamples()
            => new ListLightsResponse(new []
            {
                new ListLightsItemResponse(Guid.NewGuid(),
                    "Office - Central",
                    LightStatus.On),

                new ListLightsItemResponse(Guid.NewGuid(),
                    "Office - Spots",
                    LightStatus.Off),

                new ListLightsItemResponse(Guid.NewGuid(),
                    "Kitchen - Cooking Island",
                    LightStatus.Off),
            });
    }
}
