namespace FunctionalLiving.Infrastructure.CommandHandling
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.CommandHandling;

    public interface IBus
    {
    }

    public interface IFunctionalLivingCommandHandlerResolver : IBus
    {
        List<ReturnHandler<CommandMessage<TCommand>>> Resolve<TCommand>() where TCommand : class;
    }
}
