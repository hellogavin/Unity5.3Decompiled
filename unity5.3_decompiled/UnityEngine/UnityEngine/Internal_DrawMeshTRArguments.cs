namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawMeshTRArguments
    {
        public int layer;
        public int submeshIndex;
        public Quaternion rotation;
        public Vector3 position;
        public int castShadows;
        public int receiveShadows;
        public int reflectionProbeAnchorInstanceID;
    }
}

