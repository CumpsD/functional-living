namespace FunctionalLiving.Api.Knx.Requests
{
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using FunctionalLiving.Knx.Commands;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public class KnxRequest
    {
        /// <summary>Source Knx device address.</summary>
        [Required]
        [Display(Name = "SourceAddress")]
        public string SourceAddress { get; set; }

        /// <summary>Destination Knx group address.</summary>
        [Required]
        [Display(Name = "DestinationAddress")]
        public string DestinationAddress { get; set; }

        /// <summary>The Knx message as a string of hex bytes.</summary>
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
    }

    public class KnxRequestValidator : AbstractValidator<KnxRequest>
    {
        public KnxRequestValidator()
        {
            RuleFor(x => x.SourceAddress)
                .NotEmpty();

            RuleFor(x => x.DestinationAddress)
                .NotEmpty();

            RuleFor(x => x.State)
                .NotEmpty();
        }
    }

    public class KnxRequestExample : IExamplesProvider<KnxRequest>
    {
        public KnxRequest GetExamples()
        {
            return new KnxRequest
            {
                SourceAddress = "1.1.72",
                DestinationAddress = "0/3/6",
                State = "0x273A"
            };
        }
    }

    public static class KnxRequestMapping
    {
        public static KnxCommand Map(KnxRequest message)
        {
            // TODO: Map message to command
            return new KnxCommand();
        }
    }
}
