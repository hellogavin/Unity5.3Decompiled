namespace UnityEditor
{
    using System;

    [Flags]
    public enum BuildAssetBundleOptions
    {
        AppendHashToAssetBundleName = 0x80,
        ChunkBasedCompression = 0x100,
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CollectDependencies = 2,
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CompleteAssets = 4,
        DeterministicAssetBundle = 0x10,
        DisableWriteTypeTree = 8,
        ForceRebuildAssetBundle = 0x20,
        IgnoreTypeTreeChanges = 0x40,
        None = 0,
        UncompressedAssetBundle = 1
    }
}

