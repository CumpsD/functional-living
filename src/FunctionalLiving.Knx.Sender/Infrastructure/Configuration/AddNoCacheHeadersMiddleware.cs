namespace FunctionalLiving.Knx.Sender.Infrastructure.Configuration
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    /// <summary>
    /// Add headers to the response to prevent any caching.
    /// </summary>
    public class AddNoCacheHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public AddNoCacheHeadersMiddleware(RequestDelegate next) => _next = next;

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add(HeaderNames.CacheControl, "no-store, no-cache, must-revalidate");
            context.Response.Headers.Add(HeaderNames.Pragma, "no-cache");

            return _next(context);
        }
    }
}
