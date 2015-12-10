namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Skybox : Behaviour
    {
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

