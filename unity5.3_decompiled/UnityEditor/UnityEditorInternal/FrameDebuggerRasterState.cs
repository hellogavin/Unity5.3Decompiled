namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameDebuggerRasterState
    {
        public CullMode cullMode;
        public int depthBias;
        public float slopeScaledDepthBias;
    }
}

