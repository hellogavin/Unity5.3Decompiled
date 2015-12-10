namespace UnityEditor
{
    using System;

    public enum BuildTarget
    {
        Android = 13,
        [Obsolete("Use BlackBerry instead (UnityUpgradable) -> BlackBerry", true)]
        BB10 = -1,
        BlackBerry = 0x1c,
        iOS = 9,
        [Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
        iPhone = -1,
        [Obsolete("Use WSAPlayer instead (UnityUpgradable) -> WSAPlayer", true)]
        MetroPlayer = -1,
        Nintendo3DS = 0x23,
        PS3 = 10,
        PS4 = 0x1f,
        PSM = 0x20,
        PSP2 = 30,
        SamsungTV = 0x22,
        StandaloneGLESEmu = 14,
        StandaloneLinux = 0x11,
        StandaloneLinux64 = 0x18,
        StandaloneLinuxUniversal = 0x19,
        StandaloneOSXIntel = 4,
        StandaloneOSXIntel64 = 0x1b,
        StandaloneOSXUniversal = 2,
        StandaloneWindows = 5,
        StandaloneWindows64 = 0x13,
        Tizen = 0x1d,
        tvOS = 0x25,
        WebGL = 20,
        WebPlayer = 6,
        WebPlayerStreamed = 7,
        WiiU = 0x24,
        WP8Player = 0x1a,
        WSAPlayer = 0x15,
        XBOX360 = 11,
        XboxOne = 0x21
    }
}

