namespace FunctionalLiving.Api.Infrastructure
{
    using System.Reflection;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    [ApiVersionNeutral]
    [Route("")]
    public class EmptyController : ApiController
    {
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get(
            [FromServices] IHostingEnvironment hostingEnvironment,
            CancellationToken cancellationToken)
        {
            return Request.Headers[HeaderNames.Accept].ToString().Contains("text/html")
                ? (IActionResult)new RedirectResult("/docs")
                : new OkObjectResult($"Welcome to the Functional Living Api v{Assembly.GetEntryAssembly().GetName().Version}.");
        }
    }
}
