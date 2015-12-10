namespace UnityEngine.Rendering
{
    using System;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public enum GraphicsDeviceType
    {
        Direct3D11 = 2,
        Direct3D12 = 0x12,
        Direct3D9 = 1,
        Metal = 0x10,
        Nintendo3DS = 0x13,
        Null = 4,
        OpenGL2 = 0,
        OpenGLCore = 0x11,
        OpenGLES2 = 8,
        OpenGLES3 = 11,
        PlayStation3 = 3,
        PlayStation4 = 13,
        PlayStationMobile = 15,
        PlayStationVita = 12,
        Xbox360 = 6,
        XboxOne = 14
    }
}

