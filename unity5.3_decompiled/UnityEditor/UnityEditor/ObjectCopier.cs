namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class ObjectCopier
    {
        public static T DeepClone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }
            if (object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            IFormatter formatter = new BinaryFormatter();
            Stream serializationStream = new MemoryStream();
            using (serializationStream)
            {
                formatter.Serialize(serializationStream, source);
                serializationStream.Seek(0L, SeekOrigin.Begin);
                return (T) formatter.Deserialize(serializationStream);
            }
        }
    }
}

