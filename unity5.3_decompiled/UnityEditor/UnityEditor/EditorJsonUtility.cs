namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class EditorJsonUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FromJsonOverwrite(string json, Object objectToOverwrite);
        public static string ToJson(Object obj)
        {
            return ToJson(obj, false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string ToJson(Object obj, bool prettyPrint);
    }
}

