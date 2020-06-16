namespace FunctionalLiving.Api.Knx.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FluentValidation;
    using FunctionalLiving.Knx.Commands;
    using FunctionalLiving.Knx.Addressing;
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
            var groupAddress = KnxGroupAddress.Parse(message.DestinationAddress);

            return new KnxCommand(
                groupAddress,
                StringToByteArray(message.State));
        }

        private static byte[] StringToByteArray(string hex) {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
