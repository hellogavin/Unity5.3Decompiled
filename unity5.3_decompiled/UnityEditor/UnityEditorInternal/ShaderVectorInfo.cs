namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderVectorInfo
    {
        public string name;
        public int flags;
        public Vector4 value;
    }
}

