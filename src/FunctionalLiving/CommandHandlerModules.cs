namespace FunctionalLiving
{
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Autofac;
    using Knx;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
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
