namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class RenderDoc
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsInstalled();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsLoaded();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsSupported();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Load();
    }
}

