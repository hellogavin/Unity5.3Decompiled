namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class EditorPrefs
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteAll();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteKey(string key);
        [ExcludeFromDocs]
        public static bool GetBool(string key)
        {
            bool defaultValue = false;
            return GetBool(key, defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);
        [ExcludeFromDocs]
        public static float GetFloat(string key)
        {
            float defaultValue = 0f;
            return GetFloat(key, defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);
        [ExcludeFromDocs]
        public static int GetInt(string key)
        {
            int defaultValue = 0;
            return GetInt(key, defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);
        [ExcludeFromDocs]
        public static string GetString(string key)
        {
            string defaultValue = string.Empty;
            return GetString(key, defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasKey(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetBool(string key, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetFloat(string key, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetInt(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetString(string key, string value);
    }
}

