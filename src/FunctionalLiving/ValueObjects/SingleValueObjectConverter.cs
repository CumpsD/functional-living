namespace FunctionalLiving.ValueObjects
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class SingleValueObjectConverter<T> : JsonConverter where T : GuidValueObject<T>
    {
        private static readonly ConcurrentDictionary<Type, Type> ConstructorArgumentTypes = new ConcurrentDictionary<Type, Type>();

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (!(value is GuidValueObject<T> singleValueObject))
                return;

            serializer.Serialize(writer, singleValueObject.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var parameterType = ConstructorArgumentTypes.GetOrAdd(
                objectType,
                t =>
                {
                    var constructorInfo = objectType.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
                    var parameterInfo = constructorInfo.GetParameters().Single();
                    return parameterInfo.ParameterType;
                });

            var value = serializer.Deserialize(reader, parameterType);
            return Activator.CreateInstance(objectType, value);
        }

        public override bool CanConvert(Type objectType)
            => typeof(GuidValueObject<T>).GetTypeInfo().IsAssignableFrom(objectType);
    }
}
