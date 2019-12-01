using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetNavigation
{
    public static class ParametersExtensions
    {
        public static bool TryGetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key, out T value)
        {
            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        value = default;
                    else if (kvp.Value.GetType() == typeof(T))
                        value = (T)kvp.Value;
                    else if (typeof(T).IsAssignableFrom(kvp.Value.GetType()))
                        value = (T)kvp.Value;
                    else
                        value = (T)Convert.ChangeType(kvp.Value, typeof(T));

                    return true;
                }
            }

            value = default;
            return false;
        }

        public static IEnumerable<T> GetValues<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key)
        {
            List<T> values = new List<T>();

            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        values.Add(default);
                    else if (kvp.Value.GetType() == typeof(T))
                        values.Add((T)kvp.Value);
                    else if (typeof(T).IsAssignableFrom(kvp.Value.GetType()))
                        values.Add((T)kvp.Value);
                    else
                        values.Add((T)Convert.ChangeType(kvp.Value, typeof(T)));
                }
            }

            return values.AsEnumerable();
        }
        public static bool ContainsKey(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            parameters.Any(x => string.Compare(x.Key, key, StringComparison.Ordinal) == 0);

        public static T GetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            (T)GetValue(parameters, key, typeof(T));

        public static object GetValue(this IEnumerable<KeyValuePair<string, object>> parameters, string key, Type type)
        {
            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        return GetDefault(type);
                    else if (kvp.Value.GetType() == type)
                        return kvp.Value;
                    else if (type.IsAssignableFrom(kvp.Value.GetType()))
                        return kvp.Value;
                    else
                        return Convert.ChangeType(kvp.Value, type);
                }
            }

            return GetDefault(type);
        }
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
