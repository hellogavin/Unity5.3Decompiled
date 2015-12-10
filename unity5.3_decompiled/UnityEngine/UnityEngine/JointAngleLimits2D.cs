namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct JointAngleLimits2D
    {
        private float m_LowerAngle;
        private float m_UpperAngle;
        public float min
        {
            get
            {
                return this.m_LowerAngle;
            }
            set
            {
                this.m_LowerAngle = value;
            }
        }
        public float max
        {
            get
            {
                return this.m_UpperAngle;
            }
            set
            {
                this.m_UpperAngle = value;
            }
        }
    }
}

