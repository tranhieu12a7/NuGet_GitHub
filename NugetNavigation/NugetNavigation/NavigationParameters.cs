using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetNavigation
{
    public class NavigationParameters : INavigationParameters
    {
        private readonly List<KeyValuePair<string, object>> _entries = new List<KeyValuePair<string, object>>();
        public object this[string key]
        {
            get
            {
                foreach (var entry in _entries)
                {
                    if (string.Compare(entry.Key, key) == 0)
                    {
                        return entry.Value;
                    }
                }

                return null;
            }
        }

        public int Count => _entries.Count;

        public IEnumerable<string> Keys =>
            _entries.Select(x => x.Key);

        public void Add(string key, object value) =>
            _entries.Add(new KeyValuePair<string, object>(key, value));

        public bool ContainsKey(string key) =>
            _entries.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
            _entries.GetEnumerator();

        public T GetValue<T>(string key) =>
            _entries.GetValue<T>(key);

        public IEnumerable<T> GetValues<T>(string key) =>
            _entries.GetValues<T>(key);

        public bool TryGetValue<T>(string key, out T value) =>
            _entries.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() =>
             GetEnumerator();
    }
}
