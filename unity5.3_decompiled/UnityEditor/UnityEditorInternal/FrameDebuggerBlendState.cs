namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameDebuggerBlendState
    {
        public uint renderTargetWriteMask;
        public BlendMode srcBlend;
        public BlendMode dstBlend;
        public BlendMode srcBlendAlpha;
        public BlendMode dstBlendAlpha;
        public BlendOp blendOp;
        public BlendOp blendOpAlpha;
    }
}

