namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public class Behaviour : Component
    {
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isActiveAndEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

