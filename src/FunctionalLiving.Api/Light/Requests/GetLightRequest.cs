namespace FunctionalLiving.Api.Light.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using Swashbuckle.AspNetCore.Filters;

    public class GetLightRequest
    {
        /// <summary>Light to get details for.</summary>
        [Required]
        [Display(Name = "LightId")]
        public Guid LightId { get; set; }
    }

    public class GetLightRequestValidator : AbstractValidator<GetLightRequest>
    {
        public GetLightRequestValidator()
        {
            RuleFor(x => x.LightId)
                .NotEmpty();
        }
    }

    public class GetLightRequestExample : IExamplesProvider<GetLightRequest>
    {
        public GetLightRequest GetExamples()
            => new GetLightRequest
            {
                LightId = Guid.NewGuid()
            };
    }
}
