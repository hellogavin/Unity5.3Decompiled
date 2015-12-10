namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderMatrixInfo
    {
        public string name;
        public int flags;
        public Matrix4x4 value;
    }
}

