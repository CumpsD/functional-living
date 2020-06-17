namespace FunctionalLiving.Knx.Sender.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    public class FunctionalLivingKnxExceptionHandler : DefaultExceptionHandler<FunctionalLivingKnxException>
    {
        protected override ProblemDetails GetApiProblemFor(FunctionalLivingKnxException exception) =>
            new ProblemDetails
            {
                HttpStatus = StatusCodes.Status400BadRequest,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception.Message,
                ProblemTypeUri = ProblemDetails.GetTypeUriFor(exception)
            };
    }
}
