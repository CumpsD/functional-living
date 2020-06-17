namespace FunctionalLiving.Knx.Sender.Infrastructure
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;

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
