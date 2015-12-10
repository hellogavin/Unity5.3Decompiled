namespace UnityEngine
{
    using System;

    public enum ConnectionTesterStatus
    {
        Error = -2,
        LimitedNATPunchthroughPortRestricted = 5,
        LimitedNATPunchthroughSymmetric = 6,
        NATpunchthroughAddressRestrictedCone = 8,
        NATpunchthroughFullCone = 7,
        [Obsolete("No longer returned, use newer connection tester enums instead.")]
        PrivateIPHasNATPunchThrough = 1,
        [Obsolete("No longer returned, use newer connection tester enums instead.")]
        PrivateIPNoNATPunchthrough = 0,
        PublicIPIsConnectable = 2,
        PublicIPNoServerStarted = 4,
        PublicIPPortBlocked = 3,
        Undetermined = -1
    }
}

