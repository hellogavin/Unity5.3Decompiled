namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [Obsolete("UnityEditor.AndroidBuildSubtarget has been deprecated. Use UnityEditor.MobileTextureSubtarget instead (UnityUpgradable) -> MobileTextureSubtarget", true), EditorBrowsable(EditorBrowsableState.Never)]
    public enum AndroidBuildSubtarget
    {
        ASTC = -1,
        ATC = -1,
        DXT = -1,
        ETC = -1,
        ETC2 = -1,
        Generic = -1,
        PVRTC = -1
    }
}

