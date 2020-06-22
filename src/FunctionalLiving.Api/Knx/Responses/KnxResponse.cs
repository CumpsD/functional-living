namespace FunctionalLiving.Api.Knx.Responses
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [Serializable]
    public class KnxResponse
    {
    }

    public class KnxResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples()
            => new object();
    }

    // TODO: Localize all errors

    public class ExampleNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Onbestaand voorbeeld.",
                ProblemInstanceUri = ProblemDetails.GetProblemNumber()
            };
    }

    public class BadRequestResponseExamples : IExamplesProvider<ValidationProblemDetails>
    {
        // TODO: Improve this example
        public ValidationProblemDetails GetExamples()
            => new ValidationProblemDetails
            {
                ValidationErrors = new Dictionary<string, string[]>
                {
                    { "Voornaam", new[] {"Veld is verplicht." }},
                    { "Naam", new[] {"Veld mag niet kleiner zijn dan 4 karakters.", "Veld mag niet groter zijn dan 100 karakters." }}
                }
            };
    }

    public class InternalServerErrorResponseExamples : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                ProblemTypeUri = "cumps-consulting:functional-living:api:internal-server-error",
                HttpStatus = StatusCodes.Status500InternalServerError,
                Title = ProblemDetails.DefaultTitle,
                Detail = "<more info about the internal error>",
                ProblemInstanceUri = new DefaultHttpContext().GetProblemInstanceUri()
            };
    }
}
