namespace UnityEditor
{
    using System;

    public enum BuildTargetGroup
    {
        Android = 7,
        BlackBerry = 0x10,
        GLESEmu = 9,
        iOS = 4,
        [Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
        iPhone = 4,
        [Obsolete("Use WSA instead")]
        Metro = 14,
        Nintendo3DS = 0x17,
        PS3 = 5,
        PS4 = 0x13,
        PSM = 20,
        PSP2 = 0x12,
        SamsungTV = 0x16,
        Standalone = 1,
        Tizen = 0x11,
        tvOS = 0x19,
        Unknown = 0,
        WebGL = 13,
        WebPlayer = 2,
        WiiU = 0x18,
        WP8 = 15,
        WSA = 14,
        XBOX360 = 6,
        XboxOne = 0x15
    }
}

