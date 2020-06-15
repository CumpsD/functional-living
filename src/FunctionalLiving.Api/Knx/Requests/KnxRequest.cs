namespace FunctionalLiving.Api.Knx.Requests
{
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using FunctionalLiving.Knx.Commands;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public class KnxRequest
    {
        /// <summary>Type of the Knx group address.</summary>
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        /// <summary>The Knx message as a string of hex bytes.</summary>
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
    }

    public class KnxRequestValidator : AbstractValidator<KnxRequest>
    {
        public KnxRequestValidator()
        {
            RuleFor(x => x.Address)
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
                Address = "1/3/17",
                State = "15-A7"
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
