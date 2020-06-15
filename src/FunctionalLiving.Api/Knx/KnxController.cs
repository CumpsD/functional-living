namespace FunctionalLiving.Api.Knx
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Converters;
    using Requests;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("example")]
    [ApiExplorerSettings(GroupName = "Example")]
    public class ExampleController : FunctionalLivingController
    {
        /// <summary>
        /// Vraag een voorbeeld op.
        /// </summary>
        /// <param name="id">Identificator van het voorbeeld.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het voorbeeld gevonden is.</response>
        /// <response code="404">Als het voorbeeld niet gevonden kan worden.</response>
        /// <response code="412">Als de gevraagde minimum positie van de event store nog niet bereikt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExampleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ExampleResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ExampleNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            // Dummy
            return Ok(id);
        }

        /// <summary>
        /// Voer een generiek commando uit.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="commandId">Optionele unieke id voor het verzoek.</param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Als het verzoek aanvaard is.</response>
        /// <response code="400">Als het verzoek ongeldige data bevat.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(CommandRequest), typeof(CommandRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(CommandResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> Post(
            [FromServices] ICommandHandlerResolver bus,
            [FromCommandId] Guid commandId,
            [FromBody] CommandRequest command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // TODO: Check what this returns in the response

            // Normally this would be bus.Dispatch(...) but because of the example the command to dispatch is of type 'dynamic' which an extension method cannot handle.
            return Accepted(
                await CommandHandlerResolverExtensions.Dispatch(
                    bus,
                    commandId,
                    CommandRequestMapping.Map(command),
                    GetMetadata(),
                    cancellationToken));
        }
    }

    public class CommandResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples() => new { };
    }
}
