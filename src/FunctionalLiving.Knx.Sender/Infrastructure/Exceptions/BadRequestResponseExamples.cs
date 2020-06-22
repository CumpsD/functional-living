namespace FunctionalLiving.Knx.Sender.Infrastructure.Exceptions
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;

    // TODO: Localize all errors

    public class BadRequestResponseExamples : IExamplesProvider<ValidationProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BadRequestResponseExamples(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        // TODO: Improve this example
        public ValidationProblemDetails GetExamples()
            => new ValidationProblemDetails
            {
                ProblemTypeUri = "cumps-consulting:functional-living:api:validation-error",
                Title = ProblemDetails.DefaultTitle,
                ValidationErrors = new Dictionary<string, string[]>
                {
                    { "Voornaam", new[] {"Veld is verplicht." }},
                    { "Naam", new[] {"Veld mag niet kleiner zijn dan 4 karakters.", "Veld mag niet groter zijn dan 100 karakters." }}
                },
                ProblemInstanceUri = _httpContextAccessor.HttpContext.GetProblemInstanceUri()
            };
    }
}
