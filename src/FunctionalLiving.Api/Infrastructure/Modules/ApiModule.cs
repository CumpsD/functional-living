namespace FunctionalLiving.Api.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using FunctionalLiving.Infrastructure.Modules;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class ApiModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterModule(new LoggingModule(_configuration, _services))
                .RegisterModule(new TracingModule(_configuration, _services))
                .RegisterModule(new TogglesModule(_configuration, _loggerFactory))
                .RegisterModule(new InfluxModule(_configuration))
                .RegisterModule(new HttpModule(_configuration, _services, _loggerFactory))
                .RegisterModule(new SignalRModule(_configuration, _services))
                .RegisterModule(new RepositoriesModule())
                .RegisterModule(new FunctionalLivingCommandHandlerModule());

            containerBuilder.Populate(_services);
        }
    }
}
