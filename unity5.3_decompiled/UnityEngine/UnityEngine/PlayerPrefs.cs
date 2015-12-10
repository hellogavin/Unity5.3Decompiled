namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class PlayerPrefs
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteAll();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteKey(string key);
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
        public static extern void Save();
        public static void SetFloat(string key, float value)
        {
            if (!TrySetFloat(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        public static void SetInt(string key, int value)
        {
            if (!TrySetInt(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        public static void SetString(string key, string value)
        {
            if (!TrySetSetString(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool TrySetFloat(string key, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool TrySetInt(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool TrySetSetString(string key, string value);
    }
}

