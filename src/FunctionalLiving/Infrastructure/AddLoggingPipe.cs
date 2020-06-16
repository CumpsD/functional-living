namespace FunctionalLiving.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Microsoft.Extensions.Logging;

    public static class AddLoggingPipe
    {
        public static ICommandHandlerBuilder<CommandMessage<TCommand>> AddLogging<TCommand>(
            this ICommandHandlerBuilder<CommandMessage<TCommand>> commandHandlerBuilder,
            ILogger logger)
        {
            return commandHandlerBuilder.Pipe(next => async (commandMessage, ct) =>
            {
                logger.LogDebug(
                    "Received '{CommandType}' with id '{CommandId}' ~> '{@Message}'.",
                    commandMessage.GetType().GetGenericArguments()[0].Name,
                    commandMessage.CommandId,
                    commandMessage.Command);

                return await next(commandMessage, ct);
            });
        }
    }
}
