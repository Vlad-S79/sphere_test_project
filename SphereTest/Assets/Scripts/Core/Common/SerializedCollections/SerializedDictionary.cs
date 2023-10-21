using System.Collections.Generic;
using UnityEngine;

namespace Core.Common.SerializedCollections
{
    [System.Serializable]
    public class SerializedDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<TKey> keys = new ();
        [SerializeField]
        private List<TValue> values = new ();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();

            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                TKey key = keys[i];
                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = values[i];
                }
            }

            return dictionary;
        }

        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public void Remove(TKey key)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        public List<TKey> GetKeys()
        {
            return keys;
        }

        public List<TValue> GetValues()
        {
            return values;
        }
    }
}