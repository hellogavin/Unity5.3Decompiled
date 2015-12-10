namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ClothSphereColliderPair
    {
        private SphereCollider m_First;
        private SphereCollider m_Second;
        public ClothSphereColliderPair(SphereCollider a)
        {
            this.m_First = null;
            this.m_Second = null;
            this.first = a;
            this.second = null;
        }

        public ClothSphereColliderPair(SphereCollider a, SphereCollider b)
        {
            this.m_First = null;
            this.m_Second = null;
            this.first = a;
            this.second = b;
        }

        public SphereCollider first
        {
            get
            {
                return this.m_First;
            }
            set
            {
                this.m_First = value;
            }
        }
        public SphereCollider second
        {
            get
            {
                return this.m_Second;
            }
            set
            {
                this.m_Second = value;
            }
        }
    }
}

