namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct OffMeshLinkData
    {
        private int m_Valid;
        private int m_Activated;
        private int m_InstanceID;
        private OffMeshLinkType m_LinkType;
        private Vector3 m_StartPos;
        private Vector3 m_EndPos;
        public bool valid
        {
            get
            {
                return (this.m_Valid != 0);
            }
        }
        public bool activated
        {
            get
            {
                return (this.m_Activated != 0);
            }
        }
        public OffMeshLinkType linkType
        {
            get
            {
                return this.m_LinkType;
            }
        }
        public Vector3 startPos
        {
            get
            {
                return this.m_StartPos;
            }
        }
        public Vector3 endPos
        {
            get
            {
                return this.m_EndPos;
            }
        }
        public OffMeshLink offMeshLink
        {
            get
            {
                return this.GetOffMeshLinkInternal(this.m_InstanceID);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern OffMeshLink GetOffMeshLinkInternal(int instanceID);
    }
}

