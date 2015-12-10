namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    public enum RuntimePlatform
    {
        Android = 11,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("BB10Player has been deprecated. Use BlackBerryPlayer instead (UnityUpgradable) -> BlackBerryPlayer", true)]
        BB10Player = 0x16,
        BlackBerryPlayer = 0x16,
        [Obsolete("FlashPlayer export is no longer supported in Unity 5.0+.")]
        FlashPlayer = 15,
        IPhonePlayer = 8,
        LinuxPlayer = 13,
        [Obsolete("Use WSAPlayerARM instead")]
        MetroPlayerARM = 20,
        [Obsolete("Use WSAPlayerX64 instead")]
        MetroPlayerX64 = 0x13,
        [Obsolete("Use WSAPlayerX86 instead")]
        MetroPlayerX86 = 0x12,
        [Obsolete("NaCl export is no longer supported in Unity 5.0+.")]
        NaCl = 12,
        OSXDashboardPlayer = 4,
        OSXEditor = 0,
        OSXPlayer = 1,
        OSXWebPlayer = 3,
        PS3 = 9,
        PS4 = 0x19,
        PSM = 0x1a,
        PSP2 = 0x18,
        SamsungTVPlayer = 0x1c,
        TizenPlayer = 0x17,
        tvOS = 0x1f,
        WebGLPlayer = 0x11,
        WiiU = 30,
        WindowsEditor = 7,
        WindowsPlayer = 2,
        WindowsWebPlayer = 5,
        WP8Player = 0x15,
        WSAPlayerARM = 20,
        WSAPlayerX64 = 0x13,
        WSAPlayerX86 = 0x12,
        XBOX360 = 10,
        XboxOne = 0x1b
    }
}

