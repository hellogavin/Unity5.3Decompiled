namespace UnityEngine
{
    using System;

    public enum CollisionDetectionMode2D
    {
        Continuous = 1,
        Discrete = 0,
        [Obsolete("Enum member CollisionDetectionMode2D.None has been deprecated. Use CollisionDetectionMode2D.Discrete instead (UnityUpgradable) -> Discrete", true)]
        None = 0
    }
}

