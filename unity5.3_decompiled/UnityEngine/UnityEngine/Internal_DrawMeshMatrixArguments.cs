namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawMeshMatrixArguments
    {
        public int layer;
        public int submeshIndex;
        public Matrix4x4 matrix;
        public int castShadows;
        public int receiveShadows;
        public int reflectionProbeAnchorInstanceID;
    }
}

