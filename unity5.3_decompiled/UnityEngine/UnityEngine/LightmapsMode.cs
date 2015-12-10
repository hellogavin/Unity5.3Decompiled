namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    public enum LightmapsMode
    {
        CombinedDirectional = 1,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Directional has been deprecated. Use SeparateDirectional instead (UnityUpgradable) -> SeparateDirectional", true)]
        Directional = 2,
        [Obsolete("Enum member LightmapsMode.Dual has been deprecated. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true), EditorBrowsable(EditorBrowsableState.Never)]
        Dual = 1,
        NonDirectional = 0,
        SeparateDirectional = 2,
        [Obsolete("Enum member LightmapsMode.Single has been deprecated. Use NonDirectional instead (UnityUpgradable) -> NonDirectional", true), EditorBrowsable(EditorBrowsableState.Never)]
        Single = 0
    }
}

