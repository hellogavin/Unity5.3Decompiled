namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Tree : Component
    {
        public ScriptableObject data { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool hasSpeedTreeWind { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

