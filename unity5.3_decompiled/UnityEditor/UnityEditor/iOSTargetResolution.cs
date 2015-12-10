namespace UnityEditor
{
    using System;

    [Obsolete("iOSTargetResolution is ignored, use Screen.SetResolution APIs")]
    public enum iOSTargetResolution
    {
        Native = 0,
        Resolution320p = 5,
        Resolution640p = 6,
        Resolution768p = 7,
        ResolutionAutoPerformance = 3,
        ResolutionAutoQuality = 4
    }
}

