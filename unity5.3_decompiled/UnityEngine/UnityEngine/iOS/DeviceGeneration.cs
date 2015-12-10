namespace UnityEngine.iOS
{
    using System;

    public enum DeviceGeneration
    {
        iPad1Gen = 7,
        iPad2Gen = 10,
        iPad3Gen = 12,
        iPad4Gen = 0x10,
        [Obsolete("Please use iPadAir1 instead.")]
        iPad5Gen = 0x13,
        iPadAir1 = 0x13,
        iPadAir2 = 0x18,
        iPadMini1Gen = 15,
        iPadMini2Gen = 20,
        iPadMini3Gen = 0x17,
        iPadMini4Gen = 0x1c,
        iPadPro1Gen = 0x1b,
        iPadUnknown = 0x2712,
        iPhone = 1,
        iPhone3G = 2,
        iPhone3GS = 3,
        iPhone4 = 8,
        iPhone4S = 11,
        iPhone5 = 13,
        iPhone5C = 0x11,
        iPhone5S = 0x12,
        iPhone6 = 0x15,
        iPhone6Plus = 0x16,
        iPhone6S = 0x19,
        iPhone6SPlus = 0x1a,
        iPhoneUnknown = 0x2711,
        iPodTouch1Gen = 4,
        iPodTouch2Gen = 5,
        iPodTouch3Gen = 6,
        iPodTouch4Gen = 9,
        iPodTouch5Gen = 14,
        iPodTouchUnknown = 0x2713,
        Unknown = 0
    }
}

