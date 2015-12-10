namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class FrictionJoint2D : AnchoredJoint2D
    {
        public float maxForce { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxTorque { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

