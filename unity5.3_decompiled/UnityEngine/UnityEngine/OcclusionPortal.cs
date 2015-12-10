namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class OcclusionPortal : Component
    {
        public bool open { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

