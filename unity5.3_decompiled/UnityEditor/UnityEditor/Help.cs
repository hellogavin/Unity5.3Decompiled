namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Help
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BrowseURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetHelpURLForObject(Object obj);
        internal static string GetNiceHelpNameForObject(Object obj)
        {
            return GetNiceHelpNameForObject(obj, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetNiceHelpNameForObject(Object obj, bool defaultToMonoBehaviour);
        public static bool HasHelpForObject(Object obj)
        {
            return HasHelpForObject(obj, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasHelpForObject(Object obj, bool defaultToMonoBehaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ShowHelpForObject(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ShowHelpPage(string page);
    }
}

