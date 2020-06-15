namespace FunctionalLiving
{
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Autofac;
    using Knx;

    public class FunctionalLivingCommandHandlerModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<KnxCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(KnxCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();

            containerBuilder
                .RegisterType<CommandHandlerResolver>()
                .As<ICommandHandlerResolver>();
        }
    }
}
