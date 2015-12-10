namespace UnityEditor
{
    using System;

    [Obsolete("TargetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs")]
    public enum TargetGlesGraphics
    {
        Automatic = -1,
        [Obsolete("OpenGL ES 1.x is deprecated, ES 2.0 will be used instead")]
        OpenGLES_1_x = 0,
        OpenGLES_2_0 = 1,
        OpenGLES_3_0 = 2
    }
}

