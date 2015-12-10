namespace UnityEngine.Rendering
{
    using System;

    public enum PassType
    {
        Deferred = 10,
        ForwardAdd = 5,
        ForwardBase = 4,
        LightPrePassBase = 6,
        LightPrePassFinal = 7,
        Meta = 11,
        Normal = 0,
        ShadowCaster = 8,
        Vertex = 1,
        VertexLM = 2,
        VertexLMRGBM = 3
    }
}

