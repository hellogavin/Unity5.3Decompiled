namespace UnityEngine.Serialization
{
    using System;
    using System.Collections;
    using System.Runtime.Serialization;

    internal class ListSerializationSurrogate : ISerializationSurrogate
    {
        public static readonly ISerializationSurrogate Default = new ListSerializationSurrogate();

        private static Array ArrayFromGenericList(IList list)
        {
            Array array = Array.CreateInstance(list.GetType().GetGenericArguments()[0], list.Count);
            list.CopyTo(array, 0);
            return array;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            IList list = (IList) obj;
            info.AddValue("_size", list.Count);
            info.AddValue("_items", ArrayFromGenericList(list));
            info.AddValue("_version", 0);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            IList list = (IList) Activator.CreateInstance(obj.GetType());
            int num = info.GetInt32("_size");
            if (num != 0)
            {
                IEnumerator enumerator = ((IEnumerable) info.GetValue("_items", typeof(IEnumerable))).GetEnumerator();
                for (int i = 0; i < num; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        throw new InvalidOperationException();
                    }
                    list.Add(enumerator.Current);
                }
            }
            return list;
        }
    }
}

