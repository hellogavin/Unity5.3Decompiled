namespace UnityEngine
{
    using System;

    public enum RenderingPath
    {
        DeferredLighting = 2,
        DeferredShading = 3,
        Forward = 1,
        UsePlayerSettings = -1,
        VertexLit = 0
    }
}

