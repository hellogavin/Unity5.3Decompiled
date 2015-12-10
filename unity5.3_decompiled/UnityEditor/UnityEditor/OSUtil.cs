namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class OSUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAppFriendlyName(string app);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetDefaultAppPath(string fileType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetDefaultApps(string fileType);
    }
}

