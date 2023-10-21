using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Common.SerializedCollections
{
    [Serializable]
    public class SerializedHashSet<TValue>
    {
        [SerializeField] 
        private List<TValue> values = new ();

        public Dictionary<Type, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<Type, TValue>();

            int count = values.Count;
            for (int i = 0; i < count; i++)
            {
                var value = values[i];
                var type = value.GetType();
                
                if (!dictionary.ContainsKey(type))
                {
                    dictionary.Add(type, value);
                }
            }

            return dictionary;
        }

        public HashSet<TValue> ToHashSet()
        {
            return new HashSet<TValue>(values);
        }

        public bool ContainsValue(TValue value)
        {
            return values.Contains(value);
        }

        public void Add(TValue value)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        public void Remove(TValue key)
        {
            int index = values.IndexOf(key);
            if (index >= 0)
            {
                values.RemoveAt(index);
            }
        }

        public List<TValue> GetValues()
        {
            return values;
        }
    }
}