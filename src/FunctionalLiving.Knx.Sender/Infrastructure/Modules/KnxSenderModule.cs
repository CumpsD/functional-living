namespace FunctionalLiving.Knx.Sender.Infrastructure.Modules
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class KnxSenderModule : Module
    {
        public KnxSenderModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            services
                .Configure<KnxConfiguration>(configuration.GetSection(KnxConfiguration.ConfigurationPath));
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<KnxSender>()
                .SingleInstance();
        }
    }
}
