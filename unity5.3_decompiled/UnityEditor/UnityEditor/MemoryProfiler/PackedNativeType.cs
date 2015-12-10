namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedNativeType
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_BaseClassId;
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int baseClassId
        {
            get
            {
                return this.m_BaseClassId;
            }
        }
    }
}

