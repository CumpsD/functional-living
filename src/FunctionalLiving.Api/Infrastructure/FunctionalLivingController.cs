namespace FunctionalLiving.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Be.Vlaanderen.Basisregisters.CommandHandling;

    public abstract class FunctionalLivingController : ApiController
    {
        protected IDictionary<string, object> GetMetadata()
        {
            var ip = User.FindFirst(AddRemoteIpAddressMiddleware.UrnBasisregistersVlaanderenIp)?.Value;
            var correlationId = User.FindFirst(AddCorrelationIdMiddleware.UrnBasisregistersVlaanderenCorrelationId)?.Value;

            return new Dictionary<string, object>
            {
                { "Ip", ip },
                { "CorrelationId", correlationId }
            };
        }
    }
}
