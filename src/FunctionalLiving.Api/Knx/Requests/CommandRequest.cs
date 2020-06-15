namespace FunctionalLiving.Api.Example.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public class CommandRequest
    {
        /// <summary>Type van het commando.</summary>
        [Required]
        public string Type { get; set; }

        /// <summary>Het commando.</summary>
        [Required]
        public string Command { get; set; }
    }

    public class CommandRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new CommandRequest
            {
                Type = "FunctionalLiving.Example.Commands.DoExample",
                Command = "{}"
            };
        }
    }

    public static class CommandRequestMapping
    {
        public static dynamic Map(CommandRequest message)
        {
            var assembly = typeof(DomainAssemblyMarker).Assembly;
            var type = assembly.GetType(message.Type);

            return JsonConvert.DeserializeObject(message.Command, type);
        }
    }
}
