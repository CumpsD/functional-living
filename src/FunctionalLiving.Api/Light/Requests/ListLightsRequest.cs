namespace FunctionalLiving.Api.Light.Requests
{
    using Swashbuckle.AspNetCore.Filters;

    public class ListLightsRequest
    {
    }

    public class ListLightsRequestExample : IExamplesProvider<ListLightsRequest>
    {
        public ListLightsRequest GetExamples()
            => new ListLightsRequest();
    }
}
