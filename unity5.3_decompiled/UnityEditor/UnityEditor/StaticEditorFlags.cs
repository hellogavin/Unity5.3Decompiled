namespace UnityEditor
{
    using System;

    [Flags]
    public enum StaticEditorFlags
    {
        BatchingStatic = 4,
        LightmapStatic = 1,
        NavigationStatic = 8,
        OccludeeStatic = 0x10,
        OccluderStatic = 2,
        OffMeshLinkGeneration = 0x20,
        ReflectionProbeStatic = 0x40
    }
}

