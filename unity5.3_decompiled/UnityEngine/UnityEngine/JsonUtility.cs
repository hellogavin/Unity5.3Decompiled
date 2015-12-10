namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public static class JsonUtility
    {
        public static T FromJson<T>(string json)
        {
            return (T) FromJson(json, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern object FromJson(string json, Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string ToJson(object obj, bool prettyPrint);
    }
}

