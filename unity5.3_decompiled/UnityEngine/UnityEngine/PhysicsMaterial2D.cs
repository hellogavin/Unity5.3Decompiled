namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class PhysicsMaterial2D : Object
    {
        public PhysicsMaterial2D()
        {
            Internal_Create(this, null);
        }

        public PhysicsMaterial2D(string name)
        {
            Internal_Create(this, name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] PhysicsMaterial2D mat, string name);

        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float friction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

