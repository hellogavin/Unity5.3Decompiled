namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ClothSkinningCoefficient
    {
        public float maxDistance;
        public float collisionSphereDistance;
    }
}

