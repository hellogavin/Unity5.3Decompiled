namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class Analytics
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Event(string category, string action, string label, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Track(string page);
    }
}

