namespace FunctionalLiving.Api.Knx
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Converters;
    using Requests;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using BadRequestResponseExamples = Responses.BadRequestResponseExamples;
    using InternalServerErrorResponseExamples = Responses.InternalServerErrorResponseExamples;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("knx")]
    [ApiExplorerSettings(GroupName = "Knx")]
    public class KnxController : FunctionalLivingController
    {
        /// <summary>
        /// Process a Knx message.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">If the Knx message is accepted.</response>
        /// <response code="400">If the Knx message contains invalid data.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(KnxRequest), typeof(KnxRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(KnxResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> ProcessKnxMessage(
            [FromServices] ICommandHandlerResolver bus,
            [FromBody] KnxRequest request,
            CancellationToken cancellationToken = default)
        {
            await new KnxRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            return Accepted(
                await bus.Dispatch(
                    Guid.NewGuid(),
                    KnxRequestMapping.Map(request),
                    GetMetadata(),
                    cancellationToken));
        }
    }
}
