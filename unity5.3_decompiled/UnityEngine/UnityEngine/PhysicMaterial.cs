namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class PhysicMaterial : Object
    {
        public PhysicMaterial()
        {
            Internal_CreateDynamicsMaterial(this, null);
        }

        public PhysicMaterial(string name)
        {
            Internal_CreateDynamicsMaterial(this, name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateDynamicsMaterial([Writable] PhysicMaterial mat, string name);

        public PhysicMaterialCombine bounceCombine { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use PhysicMaterial.bounciness instead", true)]
        public float bouncyness
        {
            get
            {
                return this.bounciness;
            }
            set
            {
                this.bounciness = value;
            }
        }

        public float dynamicFriction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public float dynamicFriction2 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public PhysicMaterialCombine frictionCombine { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public Vector3 frictionDirection
        {
            get
            {
                return Vector3.zero;
            }
            set
            {
            }
        }

        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public Vector3 frictionDirection2
        {
            get
            {
                return Vector3.zero;
            }
            set
            {
            }
        }

        public float staticFriction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public float staticFriction2 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

