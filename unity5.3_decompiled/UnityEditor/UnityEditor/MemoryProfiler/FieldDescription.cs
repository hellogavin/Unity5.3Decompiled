namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct FieldDescription
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal int m_Offset;
        [SerializeField]
        internal int m_TypeIndex;
        [SerializeField]
        internal bool m_IsStatic;
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int offset
        {
            get
            {
                return this.m_Offset;
            }
        }
        public int typeIndex
        {
            get
            {
                return this.m_TypeIndex;
            }
        }
        public bool isStatic
        {
            get
            {
                return this.m_IsStatic;
            }
        }
    }
}

