namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedNativeUnityEngineObject
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_InstanceId;
        [SerializeField]
        internal int m_Size;
        [SerializeField]
        internal int m_ClassId;
        [SerializeField]
        internal HideFlags m_HideFlags;
        [SerializeField]
        internal ObjectFlags m_Flags;
        public bool isPersistent
        {
            get
            {
                return ((this.m_Flags & ObjectFlags.IsPersistent) != ((ObjectFlags) 0));
            }
        }
        public bool isDontDestroyOnLoad
        {
            get
            {
                return ((this.m_Flags & ObjectFlags.IsDontDestroyOnLoad) != ((ObjectFlags) 0));
            }
        }
        public bool isManager
        {
            get
            {
                return ((this.m_Flags & ObjectFlags.IsManager) != ((ObjectFlags) 0));
            }
        }
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int instanceId
        {
            get
            {
                return this.m_InstanceId;
            }
        }
        public int size
        {
            get
            {
                return this.m_Size;
            }
        }
        public int classId
        {
            get
            {
                return this.m_ClassId;
            }
        }
        public HideFlags hideFlags
        {
            get
            {
                return this.m_HideFlags;
            }
        }
        internal enum ObjectFlags
        {
            IsDontDestroyOnLoad = 1,
            IsManager = 4,
            IsPersistent = 2
        }
    }
}

