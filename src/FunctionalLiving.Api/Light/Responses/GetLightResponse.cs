namespace FunctionalLiving.Api.Light.Responses
{
    using System;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Domain;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;
    using Newtonsoft.Json;

    [Serializable]
    public class GetLightResponse
    {
        /// <summary>Id of the light.</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Guid Id { get; }

        /// <summary>Description of the light.</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string Description { get; }

        [JsonProperty(Required = Required.DisallowNull)]
        public LightStatus Status { get; }

        public GetLightResponse(Light light)
            : this(
                light.Id,
                light.Description,
                light.Status) { }

        internal GetLightResponse(
            Guid lightId,
            string description,
            LightStatus status)
        {
            Id = lightId;
            Description = description;
            Status = status;
        }
    }

    public class GetLightResponseExamples : IExamplesProvider<GetLightResponse>
    {
        public GetLightResponse GetExamples()
            => new GetLightResponse(
                Guid.NewGuid(),
                "Office - Centraal",
                LightStatus.On);
    }

    public class LightNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LightNotFoundResponseExamples(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                ProblemTypeUri = "cumps-consulting:functional-living:api:not-found-error",
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Light not found.",
                ProblemInstanceUri = _httpContextAccessor.HttpContext.GetProblemInstanceUri()
            };
    }
}
