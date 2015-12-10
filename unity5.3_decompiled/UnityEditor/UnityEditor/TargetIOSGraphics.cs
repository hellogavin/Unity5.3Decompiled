namespace UnityEditor
{
    using System;

    [Obsolete("TargetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs")]
    public enum TargetIOSGraphics
    {
        Automatic = -1,
        Metal = 4,
        OpenGLES_2_0 = 2,
        OpenGLES_3_0 = 3
    }
}

