namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameDebuggerEventData
    {
        public int frameEventIndex;
        public int vertexCount;
        public int indexCount;
        public string shaderName;
        public Shader shader;
        public int shaderInstanceID;
        public int shaderPassIndex;
        public string shaderKeywords;
        public int rendererInstanceID;
        public Mesh mesh;
        public int meshInstanceID;
        public int meshSubset;
        public string rtName;
        public int rtWidth;
        public int rtHeight;
        public int rtFormat;
        public int rtDim;
        public int rtFace;
        public short rtCount;
        public short rtHasDepthTexture;
        public FrameDebuggerBlendState blendState;
        public FrameDebuggerRasterState rasterState;
        public FrameDebuggerDepthState depthState;
        public ShaderProperties shaderProperties;
    }
}

