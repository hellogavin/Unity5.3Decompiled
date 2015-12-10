namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class Router
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RegisterHandler(string route, Handler handler);
    }
}

