namespace FunctionalLiving.Knx.Sender.Requests
{
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using Swashbuckle.AspNetCore.Filters;

    public class KnxRequest
    {
        /// <summary>Destination Knx group address.</summary>
        [Required]
        [Display(Name = "DestinationAddress")]
        public string DestinationAddress { get; set; }

        /// <summary>Data type of the Knx message.</summary>
        [Required]
        [Display(Name = "DataType")]
        public string DataType { get; set; }

        /// <summary>The Knx message as a string.</summary>
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
    }

    public class KnxRequestValidator : AbstractValidator<KnxRequest>
    {
        public KnxRequestValidator()
        {
            RuleFor(x => x.DestinationAddress)
                .NotEmpty();

            RuleFor(x => x.DataType)
                .NotEmpty();

            RuleFor(x => x.State)
                .NotEmpty();
        }
    }

    public class KnxRequestExample : IExamplesProvider<KnxRequest>
    {
        public KnxRequest GetExamples()
            => new KnxRequest
            {
                DestinationAddress = "0/3/6",
                DataType = "byte[]",
                State = "0x273A"
            };
    }
}
