namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public class Effector2D : Behaviour
    {
        public int colliderMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool designedForNonTrigger { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal bool designedForTrigger { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal bool requiresCollider { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool useColliderMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

