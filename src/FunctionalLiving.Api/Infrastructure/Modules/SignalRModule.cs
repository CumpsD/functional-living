namespace FunctionalLiving.Api.Infrastructure.Modules
{
    using Autofac;
    using Broadcast;
    using FunctionalLiving.Broadcast;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class SignalRModule : Module
    {
        public SignalRModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            var enableSignalR = configuration.GetValue<bool>("Features:EnableSignalR");
            if (!enableSignalR)
                return;

            services.AddSignalR();
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<KnxHubSender>()
                .SingleInstance()
                .As<IKnxHub>();
        }
    }
}