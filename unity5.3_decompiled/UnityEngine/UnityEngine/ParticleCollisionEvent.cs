namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleCollisionEvent
    {
        private Vector3 m_Intersection;
        private Vector3 m_Normal;
        private Vector3 m_Velocity;
        private int m_ColliderInstanceID;
        public Vector3 intersection
        {
            get
            {
                return this.m_Intersection;
            }
        }
        public Vector3 normal
        {
            get
            {
                return this.m_Normal;
            }
        }
        public Vector3 velocity
        {
            get
            {
                return this.m_Velocity;
            }
        }
        [Obsolete("collider property is deprecated. Use colliderComponent instead, which supports Collider and Collider2D components.")]
        public Collider collider
        {
            get
            {
                return InstanceIDToCollider(this.m_ColliderInstanceID);
            }
        }
        public Component colliderComponent
        {
            get
            {
                return InstanceIDToColliderComponent(this.m_ColliderInstanceID);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider InstanceIDToCollider(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Component InstanceIDToColliderComponent(int instanceID);
    }
}

