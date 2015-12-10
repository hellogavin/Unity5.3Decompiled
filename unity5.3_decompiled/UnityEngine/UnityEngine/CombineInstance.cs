namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CombineInstance
    {
        private int m_MeshInstanceID;
        private int m_SubMeshIndex;
        private Matrix4x4 m_Transform;
        public Mesh mesh
        {
            get
            {
                return this.InternalGetMesh(this.m_MeshInstanceID);
            }
            set
            {
                this.m_MeshInstanceID = (value == null) ? 0 : value.GetInstanceID();
            }
        }
        public int subMeshIndex
        {
            get
            {
                return this.m_SubMeshIndex;
            }
            set
            {
                this.m_SubMeshIndex = value;
            }
        }
        public Matrix4x4 transform
        {
            get
            {
                return this.m_Transform;
            }
            set
            {
                this.m_Transform = value;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Mesh InternalGetMesh(int instanceID);
    }
}

