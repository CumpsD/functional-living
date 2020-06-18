namespace FunctionalLiving.Api.Light.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using FunctionalLiving.Light.Commands;
    using Swashbuckle.AspNetCore.Filters;
    using ValueObjects;

    public class TurnOnLightRequest
    {
        /// <summary>Light to turn on.</summary>
        [Required]
        [Display(Name = "LightId")]
        public Guid LightId { get; set; }
    }

    public class TurnOnLightRequestValidator : AbstractValidator<TurnOnLightRequest>
    {
        public TurnOnLightRequestValidator()
        {
            RuleFor(x => x.LightId)
                .NotEmpty();
        }
    }

    public class TurnOnLightRequestExample : IExamplesProvider<TurnOnLightRequest>
    {
        public TurnOnLightRequest GetExamples()
            => new TurnOnLightRequest
            {
                LightId = Guid.NewGuid()
            };
    }

    public static class TurnOnLightRequestMapping
    {
        public static TurnOnLightCommand Map(TurnOnLightRequest message)
            => new TurnOnLightCommand(
                new LightId(message.LightId));
    }
}
