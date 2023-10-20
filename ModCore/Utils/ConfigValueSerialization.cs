using ModCore.Database.JsonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ModCore.Utils
{
    public static class ConfigValueSerialization
    {
        public static string GetConfigPropertyPath<T>(Expression<Func<GuildSettings, T>> expr)
        {
            var expression = expr.Body.ToString();
            var splitIndex = expression.IndexOf('.');
            return expression.Substring(splitIndex + 1);
        }

        public static void SetConfigValue(GuildSettings settings, string path, string value)
        {
            setValueRecursive(settings, path.Split('.').ToList(), value);
        }

        private static void setValueRecursive(object obj, IEnumerable<string> path, string value)
        {
            var type = obj.GetType();
            var prop = type.GetProperty(path.First());

            // last property, try to assign
            if(path.Count() == 1)
            {
                var converted = convertToMatchingType(prop.PropertyType, value);
                prop.SetValue(obj, converted);
                return;
            }

            // not last property, we go down another
            var next = prop.GetValue(obj);
            setValueRecursive(next, path.Skip(1), value);
        }

        private static object convertToMatchingType(Type type, string value)
        {
            if (type == typeof(int))
                return int.Parse(value);
            else if (type == typeof(bool))
                return bool.Parse(value);
            else if (type == typeof(long))
                return long.Parse(value);
            else if (type == typeof(ulong))
                return ulong.Parse(value);
            else if (type == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(value);
            else if (type == typeof(string))
                return value;

            throw new InvalidCastException($"No conversion defined for type {type}");
        }
    }
}
