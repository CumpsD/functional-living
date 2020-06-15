namespace FunctionalLiving.Api.Knx.Requests
{
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public class KnxRequest
    {
        /// <summary>Type of the Knx message.</summary>
        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        /// <summary>The Knx message.</summary>
        [Required]
        [Display(Name = "Command")]
        public string Command { get; set; }
    }

    public class KnxRequestValidator : AbstractValidator<KnxRequest>
    {
        public KnxRequestValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty();
        }
    }

    public class KnxRequestExample : IExamplesProvider<KnxRequest>
    {
        public KnxRequest GetExamples()
        {
            return new KnxRequest
            {
                Type = "FunctionalLiving.Example.Commands.DoExample",
                Command = "{}"
            };
        }
    }

    public static class KnxRequestMapping
    {
        public static dynamic Map(KnxRequest message)
        {
            var assembly = typeof(DomainAssemblyMarker).Assembly;
            var type = assembly.GetType(message.Type);

            return JsonConvert.DeserializeObject(message.Command, type);
        }
    }
}
