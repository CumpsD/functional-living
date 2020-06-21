namespace FunctionalLiving.Infrastructure.CommandHandling
{
    using System;
    using System.Reflection;

    public static class ReflectionHelper
    {
        private static PropertyInfo? GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo? propInfo;
            Type? typeToInspect = type;
            do
            {
                propInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                typeToInspect = typeToInspect.BaseType;
            } while (propInfo == null && typeToInspect != null);

            return propInfo;
        }

        public static object? GetPropertyValue(this object? obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var objType = obj.GetType();
            var propInfo = GetPropertyInfo(objType, propertyName);

            if (propInfo == null)
                throw new ArgumentOutOfRangeException(nameof(propertyName), $"Couldn't find property {propertyName} in type {objType.FullName}");

            return propInfo.GetValue(obj, null);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object val)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var objType = obj.GetType();
            var propInfo = GetPropertyInfo(objType, propertyName);

            if (propInfo == null)
                throw new ArgumentOutOfRangeException(
                    nameof(propertyName),
                    $"Couldn't find property {propertyName} in type {objType.FullName}");

            propInfo.SetValue(obj, val, null);
        }
    }
}
