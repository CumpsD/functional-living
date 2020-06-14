namespace FunctionalLiving
{
    using System;
    using Example;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Autofac;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<ExampleCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(ExampleCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();

            containerBuilder
                .RegisterType<CommandHandlerResolver>()
                .As<ICommandHandlerResolver>();
        }
    }
}
