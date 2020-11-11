using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace KemengSoft.UTILS.RemoteDebug
{
    public class RemoteDebugQueryCollection : IQueryCollection
    {
        public struct Enumerator : IEnumerator<KeyValuePair<string, StringValues>>, IEnumerator, IDisposable
        {
            private Dictionary<string, StringValues>.Enumerator _dictionaryEnumerator;

            private bool _notEmpty;

            public KeyValuePair<string, StringValues> Current
            {
                get
                {
                    if (_notEmpty)
                    {
                        return _dictionaryEnumerator.Current;
                    }

                    return default(KeyValuePair<string, StringValues>);
                }
            }

            object IEnumerator.Current => Current;

            internal Enumerator(Dictionary<string, StringValues>.Enumerator dictionaryEnumerator)
            {
                _dictionaryEnumerator = dictionaryEnumerator;
                _notEmpty = true;
            }

            public bool MoveNext()
            {
                if (_notEmpty)
                {
                    return _dictionaryEnumerator.MoveNext();
                }

                return false;
            }

            public void Dispose()
            {
            }

            void IEnumerator.Reset()
            {
                if (_notEmpty)
                {
                    ((IEnumerator)_dictionaryEnumerator).Reset();
                }
            }
        }

        public static readonly RemoteDebugQueryCollection Empty = new RemoteDebugQueryCollection();

        private static readonly string[] EmptyKeys = Array.Empty<string>();

        private static readonly StringValues[] EmptyValues = Array.Empty<StringValues>();

        private static readonly Enumerator EmptyEnumerator = default(Enumerator);

        private static readonly IEnumerator<KeyValuePair<string, StringValues>> EmptyIEnumeratorType = EmptyEnumerator;

        private static readonly IEnumerator EmptyIEnumerator = EmptyEnumerator;

        private Dictionary<string, StringValues> Store
        {
            get;
            set;
        }
        public StringValues this[string key]
        {
            get
            {
                if (Store == null)
                {
                    return StringValues.Empty;
                }

                if (TryGetValue(key, out StringValues value))
                {
                    return value;
                }

                return StringValues.Empty;
            }
        }

        public int Count
        {
            get
            {
                if (Store == null)
                {
                    return 0;
                }

                return Store.Count;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                if (Store == null)
                {
                    return EmptyKeys;
                }

                return Store.Keys;
            }
        }

        public RemoteDebugQueryCollection()
        {
        }

        public RemoteDebugQueryCollection(Dictionary<string, StringValues> store)
        {
            Store = store;
        }

        public RemoteDebugQueryCollection(RemoteDebugQueryCollection store)
        {
            Store = store.Store;
        }

        public RemoteDebugQueryCollection(int capacity)
        {
            Store = new Dictionary<string, StringValues>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        public bool ContainsKey(string key)
        {
            if (Store == null)
            {
                return false;
            }

            return Store.ContainsKey(key);
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            if (Store == null)
            {
                value = default(StringValues);
                return false;
            }

            return Store.TryGetValue(key, out value);
        }

        public Enumerator GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                return EmptyEnumerator;
            }

            return new Enumerator(Store.GetEnumerator());
        }

        IEnumerator<KeyValuePair<string, StringValues>> IEnumerable<KeyValuePair<string, StringValues>>.GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                return EmptyIEnumeratorType;
            }

            return Store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Store == null || Store.Count == 0)
            {
                return EmptyIEnumerator;
            }

            return Store.GetEnumerator();
        }
    }
}
