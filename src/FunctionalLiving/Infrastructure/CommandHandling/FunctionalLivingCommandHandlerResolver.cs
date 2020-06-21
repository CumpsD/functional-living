namespace FunctionalLiving.Infrastructure.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.CommandHandling;

    public class FunctionalLivingCommandHandlerResolver : IFunctionalLivingCommandHandlerResolver
    {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public FunctionalLivingCommandHandlerResolver(params CommandHandlerModule[] commandHandlerModules)
        {
            foreach (var module in commandHandlerModules)
            {
                dynamic? handlerRegistrations = module.GetPropertyValue("HandlerRegistrations");
                if (handlerRegistrations == null)
                    throw new MissingFieldException($"Cannot find {module.GetType().Name}.HandlerRegistrations!");

                foreach (var handlerRegistration in handlerRegistrations)
                {
                    if (handlerRegistration == null)
                        continue;

                    var registrationType = ReflectionHelper.GetPropertyValue(handlerRegistration, "RegistrationType");
                    if (!_handlers.ContainsKey(registrationType))
                        _handlers[registrationType] = new List<object>();

                    var handlerInstance = ReflectionHelper.GetPropertyValue(handlerRegistration, "HandlerInstance");
                    _handlers[registrationType].Add(handlerInstance);
                }
            }
        }

        public List<ReturnHandler<CommandMessage<TCommand>>> Resolve<TCommand>() where TCommand : class
        {
            return _handlers.TryGetValue(typeof(ReturnHandler<CommandMessage<TCommand>>), out var handlers)
                ? handlers.Cast<ReturnHandler<CommandMessage<TCommand>>>().ToList()
                : new List<ReturnHandler<CommandMessage<TCommand>>>();
        }
    }
}
