namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderFloatInfo
    {
        public string name;
        public int flags;
        public float value;
    }
}

