namespace FunctionalLiving.Api.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    public class FunctionalLivingExceptionHandler : DefaultExceptionHandler<FunctionalLivingException>
    {
        protected override ProblemDetails GetApiProblemFor(FunctionalLivingException exception) =>
            new ProblemDetails
            {
                HttpStatus = StatusCodes.Status400BadRequest,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception.Message,
                ProblemTypeUri = ProblemDetails.GetTypeUriFor(exception)
            };
    }
}
