namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameDebuggerDepthState
    {
        public int depthWrite;
        public CompareFunction depthFunc;
    }
}

