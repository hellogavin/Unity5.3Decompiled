namespace UnityEngine
{
    using System;

    [Flags]
    public enum RigidbodyConstraints2D
    {
        FreezeAll = 7,
        FreezePosition = 3,
        FreezePositionX = 1,
        FreezePositionY = 2,
        FreezeRotation = 4,
        None = 0
    }
}

