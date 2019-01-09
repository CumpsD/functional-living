namespace FunctionalLiving
{
    using System;
    using Example;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore.Autofac;
    using Autofac;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterSqlStreamStoreCommandHandler<ExampleCommandHandlerModule>(
                    c => handler =>
					    new ExampleCommandHandlerModule(
                            c.Resolve<Func<IExamples>>(),
                            handler));
        }
    }
}
