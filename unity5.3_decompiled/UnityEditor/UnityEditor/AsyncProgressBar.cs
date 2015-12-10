namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class AsyncProgressBar
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Display(string progressInfo, float progress);

        public static bool isShowing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float progress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string progressInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

