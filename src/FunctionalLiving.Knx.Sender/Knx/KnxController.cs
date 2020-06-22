namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Addressing;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
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
        /// <param name="knx"></param>
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
        public async Task<IActionResult> Post(
            [FromServices] KnxSender knx,
            [FromBody] KnxRequest request,
            CancellationToken cancellationToken = default)
        {
            await new KnxRequestValidator()
                .ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            var groupAddress = KnxGroupAddress.Parse(request.DestinationAddress);

            switch (request.DataType)
            {
                case "bool":
                    var boolDatagram = bool.Parse(request.State);
                    knx.Action(groupAddress, boolDatagram);
                    break;

                case "string":
                    var stringDatagram = request.State;
                    knx.Action(groupAddress, stringDatagram);
                    break;

                case "int":
                    var intDatagram = int.Parse(request.State);
                    knx.Action(groupAddress, intDatagram);
                    break;

                case "byte":
                    var byteDatagram = StringToByteArray(request.State);
                    knx.Action(groupAddress, byteDatagram[0]);
                    break;

                case "byte[]":
                    var byteArrayDatagram = StringToByteArray(request.State);
                    knx.Action(groupAddress, byteArrayDatagram);
                    break;

                default:
                    // TODO: Throw bad request unsupported datatype
                    break;
            }

            return Accepted();
        }

        private static byte[] StringToByteArray(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
