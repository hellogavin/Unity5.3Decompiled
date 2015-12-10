namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct JointSuspension2D
    {
        private float m_DampingRatio;
        private float m_Frequency;
        private float m_Angle;
        public float dampingRatio
        {
            get
            {
                return this.m_DampingRatio;
            }
            set
            {
                this.m_DampingRatio = value;
            }
        }
        public float frequency
        {
            get
            {
                return this.m_Frequency;
            }
            set
            {
                this.m_Frequency = value;
            }
        }
        public float angle
        {
            get
            {
                return this.m_Angle;
            }
            set
            {
                this.m_Angle = value;
            }
        }
    }
}

