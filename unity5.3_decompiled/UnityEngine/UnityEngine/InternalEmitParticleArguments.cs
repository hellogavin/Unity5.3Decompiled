namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct InternalEmitParticleArguments
    {
        public Vector3 pos;
        public Vector3 velocity;
        public float size;
        public float energy;
        public Color color;
        public float rotation;
        public float angularVelocity;
    }
}

