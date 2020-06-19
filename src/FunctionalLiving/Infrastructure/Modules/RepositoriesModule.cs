namespace FunctionalLiving.Infrastructure.Modules
{
    using Autofac;
    using FunctionalLiving.Domain.Repositories;

    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<LightsRepository>()
                .SingleInstance()
                .As<LightsRepository>();
        }
    }
}
