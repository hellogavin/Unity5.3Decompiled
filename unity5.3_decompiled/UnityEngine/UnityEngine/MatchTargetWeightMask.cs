namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MatchTargetWeightMask
    {
        private Vector3 m_PositionXYZWeight;
        private float m_RotationWeight;
        public MatchTargetWeightMask(Vector3 positionXYZWeight, float rotationWeight)
        {
            this.m_PositionXYZWeight = positionXYZWeight;
            this.m_RotationWeight = rotationWeight;
        }

        public Vector3 positionXYZWeight
        {
            get
            {
                return this.m_PositionXYZWeight;
            }
            set
            {
                this.m_PositionXYZWeight = value;
            }
        }
        public float rotationWeight
        {
            get
            {
                return this.m_RotationWeight;
            }
            set
            {
                this.m_RotationWeight = value;
            }
        }
    }
}

