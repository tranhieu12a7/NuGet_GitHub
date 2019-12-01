using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NugetNavigation
{
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        object this[string key] { get; }
        int Count { get; }
        IEnumerable<string> Keys { get; }
        void Add(string key, object value);
        bool ContainsKey(string key);
        T GetValue<T>(string key);
        IEnumerable<T> GetValues<T>(string key);
        bool TryGetValue<T>(string key, out T value);
    }
}
