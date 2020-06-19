namespace FunctionalLiving.Api.Light.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using FunctionalLiving.Light.Commands;
    using Swashbuckle.AspNetCore.Filters;
    using ValueObjects;

    public class ListLightsRequest
    {
    }

    public class ListLightsRequestExample : IExamplesProvider<ListLightsRequest>
    {
        public ListLightsRequest GetExamples()
            => new ListLightsRequest();
    }
}
