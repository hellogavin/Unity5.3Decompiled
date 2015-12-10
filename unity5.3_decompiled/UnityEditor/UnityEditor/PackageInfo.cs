namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct PackageInfo
    {
        public string packagePath;
        public string jsonInfo;
        public string iconURL;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern PackageInfo[] GetPackageList();
    }
}

