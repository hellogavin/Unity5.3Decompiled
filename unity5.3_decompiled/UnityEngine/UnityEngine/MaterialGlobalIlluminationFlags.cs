namespace UnityEngine
{
    using System;

    [Flags]
    public enum MaterialGlobalIlluminationFlags
    {
        BakedEmissive = 2,
        EmissiveIsBlack = 4,
        None = 0,
        RealtimeEmissive = 1
    }
}

