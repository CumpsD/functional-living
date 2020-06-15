namespace FunctionalLiving.Api.Knx.Responses
{
    using System;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;

    [Serializable]
    public class ExampleResponse
    {
        /// <summary>Id van het voorbeeld.</summary>
        public string Id { get; }

        public ExampleResponse(string id)
        {
            Id = id;
        }
    }

    public class ExampleResponseExamples : IExamplesProvider<ExampleResponse>
    {
        public ExampleResponse GetExamples()
        {
            return new ExampleResponse("EXAMPLE000000002");
        }
    }

    public class ExampleNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
        {
            return new ProblemDetails
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Onbestaand voorbeeld.",
                ProblemInstanceUri = ProblemDetails.GetProblemNumber()
            };
        }
    }
}
