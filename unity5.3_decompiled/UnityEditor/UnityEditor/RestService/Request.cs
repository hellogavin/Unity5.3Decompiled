namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class Request
    {
        private IntPtr m_nativeRequestPtr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetParam(string paramName);

        public int Depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool Info { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int MessageType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string Payload { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string Url { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

