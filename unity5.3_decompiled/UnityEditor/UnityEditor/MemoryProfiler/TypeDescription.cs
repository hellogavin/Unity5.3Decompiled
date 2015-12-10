namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct TypeDescription
    {
        [SerializeField]
        internal string m_Name;
        [SerializeField]
        internal string m_Assembly;
        [SerializeField]
        internal FieldDescription[] m_Fields;
        [SerializeField]
        internal byte[] m_StaticFieldBytes;
        [SerializeField]
        internal int m_BaseOrElementTypeIndex;
        [SerializeField]
        internal int m_Size;
        [SerializeField]
        internal ulong m_TypeInfoAddress;
        [SerializeField]
        internal int m_TypeIndex;
        [SerializeField]
        internal TypeFlags m_Flags;
        public bool isValueType
        {
            get
            {
                return ((this.m_Flags & TypeFlags.kValueType) != TypeFlags.kNone);
            }
        }
        public bool isArray
        {
            get
            {
                return ((this.m_Flags & TypeFlags.kArray) != TypeFlags.kNone);
            }
        }
        public int arrayRank
        {
            get
            {
                return (((int) (this.m_Flags & -65536)) >> 0x10);
            }
        }
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        public string assembly
        {
            get
            {
                return this.m_Assembly;
            }
        }
        public FieldDescription[] fields
        {
            get
            {
                return this.m_Fields;
            }
        }
        public byte[] staticFieldBytes
        {
            get
            {
                return this.m_StaticFieldBytes;
            }
        }
        public int baseOrElementTypeIndex
        {
            get
            {
                return this.m_BaseOrElementTypeIndex;
            }
        }
        public int size
        {
            get
            {
                return this.m_Size;
            }
        }
        public ulong typeInfoAddress
        {
            get
            {
                return this.m_TypeInfoAddress;
            }
        }
        public int typeIndex
        {
            get
            {
                return this.m_TypeIndex;
            }
        }
        internal enum TypeFlags
        {
            kArray = 2,
            kArrayRankMask = -65536,
            kNone = 0,
            kValueType = 1
        }
    }
}

