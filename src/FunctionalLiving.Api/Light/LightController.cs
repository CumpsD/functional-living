namespace FunctionalLiving.Api.Light
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Domain.Repositories;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Converters;
    using Requests;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("lights")]
    [ApiExplorerSettings(GroupName = "Lights")]
    public class LightController : FunctionalLivingController
    {
        /// <summary>
        /// Gets all the lights.
        /// </summary>
        /// <param name="lightsRepository"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">If the list has been retreived.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ListLightsResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ListLightsRequest), typeof(ListLightsRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(ListLightsResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public IActionResult ListLights(
            [FromServices] LightsRepository lightsRepository,
            CancellationToken cancellationToken = default)
            => Ok(new ListLightsResponse(lightsRepository.Lights));

        /// <summary>
        /// Turn on a light.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="lightId"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">If the message is accepted.</response>
        /// <response code="400">If the message contains invalid data.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet("{lightId}/on")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(TurnOnLightRequest), typeof(TurnOnLightRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(TurnOnLightResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> TurnOnLight(
            [FromServices] ICommandHandlerResolver bus,
            [FromRoute] Guid lightId,
            CancellationToken cancellationToken = default)
        {
            var request = new TurnOnLightRequest
            {
                LightId = lightId
            };

            await new TurnOnLightRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            return Accepted(
                await bus.Dispatch(
                    Guid.NewGuid(),
                    TurnOnLightRequestMapping.Map(request),
                    GetMetadata(),
                    cancellationToken));
        }

        /// <summary>
        /// Turn off a light.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="lightId"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">If the message is accepted.</response>
        /// <response code="400">If the message contains invalid data.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet("{lightId}/off")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(TurnOnLightRequest), typeof(TurnOnLightRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(TurnOnLightResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> TurnOffLight(
            [FromServices] ICommandHandlerResolver bus,
            [FromRoute] Guid lightId,
            CancellationToken cancellationToken = default)
        {
            var request = new TurnOffLightRequest
            {
                LightId = lightId
            };

            await new TurnOffLightRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            return Accepted(
                await bus.Dispatch(
                    Guid.NewGuid(),
                    TurnOffLightRequestMapping.Map(request),
                    GetMetadata(),
                    cancellationToken));
        }
    }
}
