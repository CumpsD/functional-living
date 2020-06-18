namespace FunctionalLiving.Infrastructure.Modules
{
    using System;
    using System.Net.Http;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using Polly;

    public class HttpModule : Module
    {
        public static string KnxSender = "KnxSender";

        public HttpModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<HttpModule>();
            var retryCount = 5;
            var lifeTimeInMinutes = 20;

            // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
            services
                .AddHttpClient(KnxSender, client =>
                {
                    client.BaseAddress = configuration.GetValue<Uri>("KnxSender:Endpoint");
                    client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "FunctionalLiving.Api");
                })

                .SetHandlerLifetime(TimeSpan.FromMinutes(lifeTimeInMinutes))

                .ConfigurePrimaryHttpMessageHandler(c =>
                        new HttpClientHandler())

                // HttpRequestException, HTTP 5XX, and HTTP 408
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .WaitAndRetryAsync(
                        retryCount,
                        retryAttempt =>
                        {
                            var value = Math.Pow(2, retryAttempt) / 4;
                            var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                            logger?.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                            return TimeSpan.FromSeconds(randomValue);
                        }));
        }
    }
}
