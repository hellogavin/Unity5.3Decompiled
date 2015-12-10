namespace UnityEngine
{
    using System;

    [Flags]
    internal enum TerrainChangedFlags
    {
        DelayedHeightmapUpdate = 4,
        FlushEverythingImmediately = 8,
        Heightmap = 1,
        NoChange = 0,
        RemoveDirtyDetailsImmediately = 0x10,
        TreeInstances = 2,
        WillBeDestroyed = 0x100
    }
}

