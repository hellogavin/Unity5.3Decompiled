namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ContactPoint
    {
        internal Vector3 m_Point;
        internal Vector3 m_Normal;
        internal int m_ThisColliderInstanceID;
        internal int m_OtherColliderInstanceID;
        public Vector3 point
        {
            get
            {
                return this.m_Point;
            }
        }
        public Vector3 normal
        {
            get
            {
                return this.m_Normal;
            }
        }
        public Collider thisCollider
        {
            get
            {
                return ColliderFromInstanceId(this.m_ThisColliderInstanceID);
            }
        }
        public Collider otherCollider
        {
            get
            {
                return ColliderFromInstanceId(this.m_OtherColliderInstanceID);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider ColliderFromInstanceId(int instanceID);
    }
}

