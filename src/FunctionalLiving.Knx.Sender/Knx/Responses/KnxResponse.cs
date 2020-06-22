namespace FunctionalLiving.Knx.Sender.Responses
{
    using System;
    using Swashbuckle.AspNetCore.Filters;

    [Serializable]
    public class KnxResponse
    {
    }

    public class KnxResponseExamples : IExamplesProvider<KnxResponse>
    {
        public KnxResponse GetExamples()
            => new KnxResponse();
    }
}
