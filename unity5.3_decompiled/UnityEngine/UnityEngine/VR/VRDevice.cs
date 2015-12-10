namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class VRDevice
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetNativePtr();

        public static string family { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isPresent { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string model { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

