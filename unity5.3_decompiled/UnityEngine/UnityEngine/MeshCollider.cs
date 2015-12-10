namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class MeshCollider : Collider
    {
        public bool convex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Configuring smooth sphere collisions is no longer needed. PhysX3 has a better behaviour in place.")]
        public bool smoothSphereCollisions
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
    }
}

