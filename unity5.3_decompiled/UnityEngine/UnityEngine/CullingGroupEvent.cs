namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CullingGroupEvent
    {
        private const byte kIsVisibleMask = 0x80;
        private const byte kDistanceMask = 0x7f;
        private int m_Index;
        private byte m_PrevState;
        private byte m_ThisState;
        public int index
        {
            get
            {
                return this.m_Index;
            }
        }
        public bool isVisible
        {
            get
            {
                return ((this.m_ThisState & 0x80) != 0);
            }
        }
        public bool wasVisible
        {
            get
            {
                return ((this.m_PrevState & 0x80) != 0);
            }
        }
        public bool hasBecomeVisible
        {
            get
            {
                return (this.isVisible && !this.wasVisible);
            }
        }
        public bool hasBecomeInvisible
        {
            get
            {
                return (!this.isVisible && this.wasVisible);
            }
        }
        public int currentDistance
        {
            get
            {
                return (this.m_ThisState & 0x7f);
            }
        }
        public int previousDistance
        {
            get
            {
                return (this.m_PrevState & 0x7f);
            }
        }
    }
}

