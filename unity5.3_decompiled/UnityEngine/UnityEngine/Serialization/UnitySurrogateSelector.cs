namespace UnityEngine.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    public class UnitySurrogateSelector : ISurrogateSelector
    {
        public void ChainSelector(ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        public ISurrogateSelector GetNextSelector()
        {
            throw new NotImplementedException();
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(List<>))
                {
                    selector = this;
                    return ListSerializationSurrogate.Default;
                }
                if (genericTypeDefinition == typeof(Dictionary<,>))
                {
                    selector = this;
                    return (ISerializationSurrogate) Activator.CreateInstance(typeof(DictionarySerializationSurrogate<,>).MakeGenericType(type.GetGenericArguments()));
                }
            }
            selector = null;
            return null;
        }
    }
}

