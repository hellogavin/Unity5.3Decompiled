namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct PackedGCHandle
    {
        [SerializeField]
        internal ulong m_Target;
        public ulong target
        {
            get
            {
                return this.m_Target;
            }
        }
    }
}

