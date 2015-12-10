namespace UnityEngine
{
    using System;

    public enum JointProjectionMode
    {
        None = 0,
        PositionAndRotation = 1,
        [Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
        PositionOnly = 2
    }
}

