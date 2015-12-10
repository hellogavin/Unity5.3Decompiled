namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct JointSpring
    {
        public float spring;
        public float damper;
        public float targetPosition;
    }
}

