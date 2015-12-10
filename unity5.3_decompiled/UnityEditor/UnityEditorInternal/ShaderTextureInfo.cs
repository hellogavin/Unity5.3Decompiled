namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderTextureInfo
    {
        public string name;
        public int flags;
        public string textureName;
        public Texture value;
    }
}

