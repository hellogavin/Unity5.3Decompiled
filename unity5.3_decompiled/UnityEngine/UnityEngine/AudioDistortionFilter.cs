namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AudioDistortionFilter : Behaviour
    {
        public float distortionLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

