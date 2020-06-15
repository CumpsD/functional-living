namespace FunctionalLiving.Api.Infrastructure.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public class ApiModule : Module
    {
        private readonly IServiceCollection _services;

        public ApiModule(IServiceCollection services)
        {
            _services = services;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            CommandHandlerModules.Register(containerBuilder);

            containerBuilder.Populate(_services);
        }
    }
}
