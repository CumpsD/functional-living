namespace FunctionalLiving.Api.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Configuration;
    using Knx;
    using Light;
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
                                Title = "Functional Living API",
                                Description = GetApiLeadingText(description),
                                Contact = new OpenApiContact
                                {
                                    Name = "David Cumps",
                                    Email = "david@cumps.be",
                                    Url = new Uri("https://cumps.be")
                                }
                            },
                            XmlCommentPaths = new[]
                            {
                                typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                                typeof(DomainAssemblyMarker).GetTypeInfo().Assembly.GetName().Name,
                            }
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
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;

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
                    Info = groupName => $"Cumps Consulting - Functional Living API {groupName}",
                    Description = _ => "Home automation integrating various technologies.",
                    ApplicationName = _ => "Functional Living",
                    HeaderTitle = groupName => "Functional Living",
                    FooterVersion = $"{version.Minor}.{version.Build}.{version.Revision}",
                    HeadContent = _ => @"
                        <style>
                            input.search-input {
                                border-bottom: 0px
                            }

                            ul[role=""navigation""] + div {
                                display: none;
                            }

                            div[data-section-id] {
                                padding: 15px 0px;
                            }

                            li[data-item-id=""section/Introduction""]
                            {
                                border-top: 1px solid rgb(225, 225, 225);
                                border-bottom: 1px solid rgb(225, 225, 225);
                            },

                            div[id*='tag/Knx']
                            {
                                padding-bottom: 0;
                            }
                        </style>",
                    CSharpClientOptions =
                    {
                        ClassName = "FunctionalLivingClient",
                        Namespace = "FunctionalLiving.Client"
                    },
                    TypeScriptClientOptions =
                    {
                        ClassName = "FunctionalLivingClient"
                    },
                    CustomExceptionHandlers = new IExceptionHandler[]
                    {
                        new FunctionalLivingExceptionHandler(),
                    },
                },
                Server =
                {
                    PoweredByName = "Cumps Consulting - Functional Living",
                    ServerName = "Cumps Consulting",
                    FrameOptionsDirective = FrameOptionsDirectives.SameOrigin,
                },
                MiddlewareHooks =
                {
                    AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                }
            })

            .UseStaticFiles()
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapHub<KnxHub>("/knx-hub");
                endpoints.MapHub<LightHub>("/light-hub");
            });
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
        {
            var text = new StringBuilder(1000);

            text.Append(
$@"Right now you are reading the documentation for version {description.ApiVersion} of the Functional Living API{string.Format(description.IsDeprecated ? ", **this API version is not supported any more**." : ".")}

# Introduction

This is the hub of integrating various home automation technologies.

Currently supported:
* Reading KNX messages.
* Sending KNX messages.");

            return text.ToString();
        }
    }
}
