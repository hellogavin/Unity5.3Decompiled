namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    public enum TextureFormat
    {
        Alpha8 = 1,
        ARGB32 = 5,
        ARGB4444 = 2,
        ASTC_RGB_10x10 = 0x34,
        ASTC_RGB_12x12 = 0x35,
        ASTC_RGB_4x4 = 0x30,
        ASTC_RGB_5x5 = 0x31,
        ASTC_RGB_6x6 = 50,
        ASTC_RGB_8x8 = 0x33,
        ASTC_RGBA_10x10 = 0x3a,
        ASTC_RGBA_12x12 = 0x3b,
        ASTC_RGBA_4x4 = 0x36,
        ASTC_RGBA_5x5 = 0x37,
        ASTC_RGBA_6x6 = 0x38,
        ASTC_RGBA_8x8 = 0x39,
        ATC_RGB4 = 0x23,
        ATC_RGBA8 = 0x24,
        BGRA32 = 14,
        DXT1 = 10,
        DXT1Crunched = 0x1c,
        DXT5 = 12,
        DXT5Crunched = 0x1d,
        EAC_R = 0x29,
        EAC_R_SIGNED = 0x2a,
        EAC_RG = 0x2b,
        EAC_RG_SIGNED = 0x2c,
        ETC_RGB4 = 0x22,
        ETC_RGB4_3DS = 60,
        ETC_RGBA8_3DS = 0x3d,
        ETC2_RGB = 0x2d,
        ETC2_RGBA1 = 0x2e,
        ETC2_RGBA8 = 0x2f,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member TextureFormat.PVRTC_2BPP_RGB has been deprecated. Use PVRTC_RGB2 instead (UnityUpgradable) -> PVRTC_RGB2", true)]
        PVRTC_2BPP_RGB = -127,
        [Obsolete("Enum member TextureFormat.PVRTC_2BPP_RGBA has been deprecated. Use PVRTC_RGBA2 instead (UnityUpgradable) -> PVRTC_RGBA2", true), EditorBrowsable(EditorBrowsableState.Never)]
        PVRTC_2BPP_RGBA = -127,
        [Obsolete("Enum member TextureFormat.PVRTC_4BPP_RGB has been deprecated. Use PVRTC_RGB4 instead (UnityUpgradable) -> PVRTC_RGB4", true), EditorBrowsable(EditorBrowsableState.Never)]
        PVRTC_4BPP_RGB = -127,
        [Obsolete("Enum member TextureFormat.PVRTC_4BPP_RGBA has been deprecated. Use PVRTC_RGBA4 instead (UnityUpgradable) -> PVRTC_RGBA4", true), EditorBrowsable(EditorBrowsableState.Never)]
        PVRTC_4BPP_RGBA = -127,
        PVRTC_RGB2 = 30,
        PVRTC_RGB4 = 0x20,
        PVRTC_RGBA2 = 0x1f,
        PVRTC_RGBA4 = 0x21,
        R16 = 9,
        RFloat = 0x12,
        RGB24 = 3,
        RGB565 = 7,
        RGBA32 = 4,
        RGBA4444 = 13,
        RGBAFloat = 20,
        RGBAHalf = 0x11,
        RGFloat = 0x13,
        RGHalf = 0x10,
        RHalf = 15,
        YUY2 = 0x15
    }
}

