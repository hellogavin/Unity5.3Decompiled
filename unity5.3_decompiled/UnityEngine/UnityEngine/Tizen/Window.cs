namespace UnityEngine.Tizen
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Window
    {
        public static IntPtr windowHandle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

