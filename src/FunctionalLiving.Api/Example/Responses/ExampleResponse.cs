namespace FunctionalLiving.Api.Example.Responses
{
    using System;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
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

    public class ExampleResponseExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ExampleResponse("EXAMPLE000000002");
        }
    }

    public class ExampleNotFoundResponseExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new BasicApiProblem
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = BasicApiProblem.DefaultTitle,
                Detail = "Onbestaand voorbeeld.",
                ProblemInstanceUri = BasicApiProblem.GetProblemNumber()
            };
        }
    }
}
