using System;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName) where T : class
        {
            var value = obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);

            return value as T ?? throw new Exception();
        }
    }
}