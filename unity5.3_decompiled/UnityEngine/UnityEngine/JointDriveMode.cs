namespace UnityEngine
{
    using System;

    [Flags, Obsolete("JointDriveMode is no longer supported")]
    public enum JointDriveMode
    {
        [Obsolete("JointDriveMode.None is no longer supported")]
        None = 0,
        [Obsolete("JointDriveMode.Position is no longer supported")]
        Position = 1,
        [Obsolete("JointDriveMode.PositionAndvelocity is no longer supported")]
        PositionAndVelocity = 3,
        [Obsolete("JointDriveMode.Velocity is no longer supported")]
        Velocity = 2
    }
}

