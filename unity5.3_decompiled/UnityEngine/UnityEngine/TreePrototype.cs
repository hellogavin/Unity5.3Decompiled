namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class TreePrototype
    {
        internal GameObject m_Prefab;
        internal float m_BendFactor;
        public GameObject prefab
        {
            get
            {
                return this.m_Prefab;
            }
            set
            {
                this.m_Prefab = value;
            }
        }
        public float bendFactor
        {
            get
            {
                return this.m_BendFactor;
            }
            set
            {
                this.m_BendFactor = value;
            }
        }
    }
}

