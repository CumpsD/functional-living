namespace FunctionalLiving.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Be.Vlaanderen.Basisregisters.Api;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Modules;
    using SqlStreamStore;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private IContainer _applicationContainer;

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureDefaultForApi<Startup>(
                    (provider, description) => new Info
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "FunctionalLiving API",
                        Description = GetApiLeadingText(description),
                        Contact = new Contact
                        {
                            Name = "David Cumps",
                            Email = "david@cumps.be",
                            Url = "https://cumps.be"
                        }
                    },
                    new []
                    {
                        typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                    },
                    _configuration.GetSection("Cors").GetChildren().Select(c => c.Value).ToArray());

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new ApiModule(_configuration, services));
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            MsSqlStreamStore streamStore)
        {
            // StartupHelpers.EnsureSqlStreamStoreSchema<Startup>(streamStore, loggerFactory);

            app
                .UseDefaultForApi(
                    _applicationContainer,
                    serviceProvider,
                    env,
                    appLifetime,
                    loggerFactory,
                    apiVersionProvider,
                    groupName => $"Cumps Consulting - Functional Living API {groupName}")

                .UseMiddleware<AddNoCacheHeadersMiddleware>();
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Cumps Consulting Functional Living API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
    }
}
