namespace FunctionalLiving.Knx.Sender.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Modules;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private const string DefaultCulture = "en-GB";
        private const string SupportedCultures = "en-GB;en-US;en;nl-BE;nl";

        private IContainer _applicationContainer;

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureDefaultForApi<Startup>(
                    new StartupConfigureOptions
                    {
                        Cors =
                        {
                            Origins = _configuration
                                .GetSection("Cors")
                                .GetChildren()
                                .Select(c => c.Value)
                                .ToArray()
                        },
                        Swagger =
                        {
                            ApiInfo = (provider, description) => new OpenApiInfo
                            {
                                Version = description.ApiVersion.ToString(),
                                Title = "Functional Living Knx Sender API",
                                Description = GetApiLeadingText(description),
                                Contact = new OpenApiContact
                                {
                                    Name = "David Cumps",
                                    Email = "david@cumps.be",
                                    Url = new Uri("https://cumps.be")
                                }
                            },
                            XmlCommentPaths = new[] {typeof(Startup).GetTypeInfo().Assembly.GetName().Name}
                        },
                        Localization =
                        {
                            DefaultCulture = new CultureInfo(DefaultCulture),
                            SupportedCultures = SupportedCultures
                                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => new CultureInfo(x.Trim()))
                                .ToArray()
                        },
                        MiddlewareHooks =
                        {
                            FluentValidation = fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>(),
                        }
                    });

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(
                new ApiModule(
                    _configuration,
                    services,
                    _loggerFactory));

            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            KnxSender knxSender)
        {
            app.UseDefaultForApi(new StartupUseOptions
            {
                Common =
                {
                    ApplicationContainer = _applicationContainer,
                    ServiceProvider = serviceProvider,
                    HostingEnvironment = env,
                    ApplicationLifetime = appLifetime,
                    LoggerFactory = loggerFactory
                },
                Api =
                {
                    VersionProvider = apiVersionProvider,
                    Info = groupName => $"Cumps Consulting - Functional Living Knx Sender API {groupName}",
                    CSharpClientOptions =
                    {
                        ClassName = "FunctionalLivingKnxSenderClient",
                        Namespace = "FunctionalLiving.Client"
                    },
                    TypeScriptClientOptions =
                    {
                        ClassName = "FunctionalLivingKnxSenderClient"
                    },
                    CustomExceptionHandlers = new IExceptionHandler[]
                    {
                        new FunctionalLivingKnxExceptionHandler(),
                    },
                },
                Server =
                {
                    PoweredByName = "Cumps Consulting - Functional Living",
                    ServerName = "Cumps Consulting"
                },
                MiddlewareHooks =
                {
                    AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                }
            });

            knxSender.Start();
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            => $"Right now you are reading the documentation for version {description.ApiVersion} of the Cumps Consulting Functional Living Knx Sender API{string.Format(description.IsDeprecated ? ", **this API version is not supported any more**." : ".")}";
    }
}
