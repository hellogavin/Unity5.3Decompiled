namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderProperties
    {
        public ShaderFloatInfo[] floats;
        public ShaderVectorInfo[] vectors;
        public ShaderMatrixInfo[] matrices;
        public ShaderTextureInfo[] textures;
    }
}

