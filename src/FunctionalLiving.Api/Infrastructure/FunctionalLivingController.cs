namespace FunctionalLiving.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Microsoft.AspNetCore.Mvc;

    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public abstract class FunctionalLivingController : ApiController
    {
        protected IDictionary<string, object> GetMetadata()
        {
            if (User == null)
                return new Dictionary<string, object>();

            return new CommandMetaData(
                    User,
                    AddRemoteIpAddressMiddleware.UrnBasisregistersVlaanderenIp,
                    AddCorrelationIdMiddleware.UrnBasisregistersVlaanderenCorrelationId)
                .ToDictionary();
        }
    }
}
