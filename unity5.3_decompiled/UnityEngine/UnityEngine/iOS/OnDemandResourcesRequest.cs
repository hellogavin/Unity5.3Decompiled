namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class OnDemandResourcesRequest : AsyncOperation, IDisposable
    {
        internal OnDemandResourcesRequest()
        {
        }

        public string error { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public float loadingPriority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetResourcePath(string resourceName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~OnDemandResourcesRequest()
        {
            this.Dispose();
        }
    }
}

