namespace FunctionalLiving.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Be.Vlaanderen.Basisregisters.CommandHandling;

    public abstract class ApiBusController : ApiController
    {
        protected ICommandHandlerResolver Bus { get; }

        protected ApiBusController(ICommandHandlerResolver bus) => Bus = bus;

        protected IDictionary<string, object> GetMetadata()
        {
            var ip = User.FindFirst(AddRemoteIpAddressMiddleware.UrnBasisregistersVlaanderenIp)?.Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = User.FindFirst("urn:be:vlaanderen:functionalliving:acmid")?.Value;
            var correlationId = User.FindFirst(AddCorrelationIdMiddleware.UrnBasisregistersVlaanderenCorrelationId)?.Value;

            return new Dictionary<string, object>
            {
                { "FirstName", firstName },
                { "LastName", lastName },
                { "Ip", ip },
                { "UserId", userId },
                { "CorrelationId", correlationId }
            };
        }
    }
}
