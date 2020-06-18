namespace FunctionalLiving.Api.Light.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using FunctionalLiving.Light.Commands;
    using Swashbuckle.AspNetCore.Filters;
    using ValueObjects;

    public class TurnOffLightRequest
    {
        /// <summary>Light to turn off.</summary>
        [Required]
        [Display(Name = "LightId")]
        public Guid LightId { get; set; }
    }

    public class TurnOffLightRequestValidator : AbstractValidator<TurnOffLightRequest>
    {
        public TurnOffLightRequestValidator()
        {
            RuleFor(x => x.LightId)
                .NotEmpty();
        }
    }

    public class TurnOffLightRequestExample : IExamplesProvider<TurnOffLightRequest>
    {
        public TurnOffLightRequest GetExamples()
            => new TurnOffLightRequest
            {
                LightId = Guid.NewGuid()
            };
    }

    public static class TurnOffLightRequestMapping
    {
        public static TurnOffLightCommand Map(TurnOffLightRequest message)
            => new TurnOffLightCommand(
                new LightId(message.LightId));
    }
}
