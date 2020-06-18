namespace FunctionalLiving.Knx.Sender.Infrastructure.Modules
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Autofac;
    using FeatureToggle;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Toggles;

    public class TogglesModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger? _logger;

        public TogglesModule(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<TogglesModule>();
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterToggle<DebugKnxCemi>(
                    DebugKnxCemi.ConfigurationPath,
                    _configuration.GetValue<bool>(DebugKnxCemi.ConfigurationPath),
                    _logger)

                .RegisterToggle<UseKnxConnectionRouting>(
                    UseKnxConnectionRouting.ConfigurationPath,
                    _configuration.GetValue<bool>(UseKnxConnectionRouting.ConfigurationPath),
                    _logger)

                .RegisterToggle<UseKnxConnectionTunneling>(
                    UseKnxConnectionTunneling.ConfigurationPath,
                    _configuration.GetValue<bool>(UseKnxConnectionTunneling.ConfigurationPath),
                    _logger);
        }
    }

    public static class RegisterToggleExtension
    {
        public static ContainerBuilder RegisterToggle<T>(
            this ContainerBuilder containerBuilder,
            string configurationPath,
            bool toggleEnabled,
            ILogger? logger) where T : IFeatureToggle
        {
            var toggleConstructor = CreateConstructor(typeof(T), typeof(bool));
            var toggle = toggleConstructor(toggleEnabled);
            containerBuilder
                .RegisterInstance(toggle)
                .As<T>();

            logger?.LogDebug(
                "Registered Toggle '{ToggleType}' with value '{ToggleValue}'.",
                typeof(T).Name,
                toggleEnabled);

            return containerBuilder;
        }

        // this delegate is just, so you don't have to pass an object array. _(params)_
        private delegate object ConstructorDelegate(params object[] args);

        private static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
        {
            // Get the constructor info for these parameters
            var constructorInfo = type.GetConstructor(parameters);

            // define a object[] parameter
            var paramExpr = Expression.Parameter(typeof(Object[]));

            // To feed the constructor with the right parameters, we need to generate an array
            // of parameters that will be read from the initialize object array argument.
            var constructorParameters = parameters.Select((paramType, index) =>
                // convert the object[index] to the right constructor parameter type.
                Expression.Convert(
                    // read a value from the object[index]
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            // just call the constructor.
            var body = Expression.New(constructorInfo, constructorParameters);

            var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
            return constructor.Compile();
        }
    }
}
