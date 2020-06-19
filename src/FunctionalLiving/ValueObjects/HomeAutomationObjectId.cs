namespace FunctionalLiving.ValueObjects
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Knx.Addressing;
    using Newtonsoft.Json;

    [JsonConverter(typeof(SingleValueObjectConverter<HomeAutomationObjectId>))]
    public class HomeAutomationObjectId : GuidValueObject<HomeAutomationObjectId>
    {
        private static readonly Guid HomeAutomationObjectIdNamespace = new Guid("ab78132d-c26c-440a-b9c0-ba7d5715c19c");

        public HomeAutomationObjectId([JsonProperty("value")] Guid homeAutomationObjectId) : base(homeAutomationObjectId) { }

        public static T CreateDeterministicId<T>(KnxGroupAddress address) where T : HomeAutomationObjectId
        {
            var idConstructor = CreateConstructor(typeof(T), typeof(Guid));
            return (T)idConstructor(Deterministic.Create(HomeAutomationObjectIdNamespace, $"knx-{address}"));
        }

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
