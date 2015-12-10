namespace UnityEngine.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal class DictionarySerializationSurrogate<TKey, TValue> : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            ((Dictionary<TKey, TValue>) obj).GetObjectData(info, context);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            IEqualityComparer<TKey> comparer = (IEqualityComparer<TKey>) info.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(comparer);
            if (info.MemberCount > 3)
            {
                KeyValuePair<TKey, TValue>[] pairArray = (KeyValuePair<TKey, TValue>[]) info.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
                if (pairArray == null)
                {
                    return dictionary;
                }
                foreach (KeyValuePair<TKey, TValue> pair in pairArray)
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }
            return dictionary;
        }
    }
}

