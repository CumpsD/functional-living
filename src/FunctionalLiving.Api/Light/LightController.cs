namespace FunctionalLiving.Api.Light
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Domain;
    using Domain.Repositories;
    using FunctionalLiving.Infrastructure.CommandHandling;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using BadRequestResponseExamples = Infrastructure.Exceptions.BadRequestResponseExamples;
    using InternalServerErrorResponseExamples = Infrastructure.Exceptions.InternalServerErrorResponseExamples;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

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
        /// <response code="200">If the list has been retrieved.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ListLightsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ListLightsRequest), typeof(ListLightsRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListLightsResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public IActionResult ListLights(
            [FromServices] LightsRepository lightsRepository,
            CancellationToken cancellationToken = default)
            => Ok(new ListLightsResponse(lightsRepository.Lights));

        /// <summary>
        /// Gets a light.
        /// </summary>
        /// <param name="lightsRepository"></param>
        /// <param name="lightId">Id of the light to get information for.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">If the light has been retrieved.</response>
        /// <response code="404">If the light cannot be found.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet("{lightId}")]
        [ProducesResponseType(typeof(GetLightResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(GetLightRequest), typeof(GetLightRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetLightResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(LightNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> GetLight(
            [FromServices] LightsRepository lightsRepository,
            [FromRoute] Guid lightId,
            CancellationToken cancellationToken = default)
        {
            var request = new GetLightRequest
            {
                LightId = lightId
            };

            await new GetLightRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            var light = GetLightOrThrow(lightsRepository, request.LightId);

            return Ok(new GetLightResponse(light));
        }

        /// <summary>
        /// Turn on a light.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="lightsRepository"></param>
        /// <param name="lightId">Id of the light to turn on.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">If the message is accepted.</response>
        /// <response code="400">If the message contains invalid data.</response>
        /// <response code="404">If the light cannot be found.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet("{lightId}/on")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(TurnOnLightRequest), typeof(TurnOnLightRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(TurnOnLightResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(LightNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> TurnOnLight(
            [FromServices] IBus bus,
            [FromServices] LightsRepository lightsRepository,
            [FromRoute] Guid lightId,
            CancellationToken cancellationToken = default)
        {
            var request = new TurnOnLightRequest
            {
                LightId = lightId
            };

            await new TurnOnLightRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            GetLightOrThrow(lightsRepository, request.LightId);

            await bus.Dispatch(
                Guid.NewGuid(),
                TurnOnLightRequestMapping.Map(request),
                GetMetadata(),
                cancellationToken);

            return Accepted(new TurnOnLightResponse());
        }

        /// <summary>
        /// Turn off a light.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="lightsRepository"></param>
        /// <param name="lightId">Id of the light to turn off.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">If the message is accepted.</response>
        /// <response code="400">If the message contains invalid data.</response>
        /// <response code="404">If the light cannot be found.</response>
        /// <response code="500">If an internal error has occured.</response>
        /// <returns></returns>
        [HttpGet("{lightId}/off")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(TurnOnLightRequest), typeof(TurnOffLightRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(TurnOffLightResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(LightNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> TurnOffLight(
            [FromServices] IBus bus,
            [FromServices] LightsRepository lightsRepository,
            [FromRoute] Guid lightId,
            CancellationToken cancellationToken = default)
        {
            var request = new TurnOffLightRequest
            {
                LightId = lightId
            };

            await new TurnOffLightRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            GetLightOrThrow(lightsRepository, request.LightId);

            await bus.Dispatch(
                Guid.NewGuid(),
                TurnOffLightRequestMapping.Map(request),
                GetMetadata(),
                cancellationToken);

            return Accepted(new TurnOffLightResponse());
        }

        private static Light GetLightOrThrow(LightsRepository lightsRepository, Guid lightId)
        {
            var light = lightsRepository.Lights.SingleOrDefault(x => x.Id == lightId);
            if (light == null)
                throw new ApiException("Light not found.", StatusCodes.Status404NotFound);

            return light;
        }
    }
}
