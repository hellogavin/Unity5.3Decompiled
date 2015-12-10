namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CastHelper<T>
    {
        public T t;
        public IntPtr onePointerFurtherThanT;
    }
}

