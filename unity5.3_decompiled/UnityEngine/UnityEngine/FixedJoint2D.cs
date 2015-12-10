namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class FixedJoint2D : AnchoredJoint2D
    {
        public float dampingRatio { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float frequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float referenceAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

