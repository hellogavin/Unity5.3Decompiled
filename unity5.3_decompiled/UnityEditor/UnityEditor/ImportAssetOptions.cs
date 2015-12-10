namespace UnityEditor
{
    using System;

    [Flags]
    public enum ImportAssetOptions
    {
        Default = 0,
        DontDownloadFromCacheServer = 0x2000,
        ForceSynchronousImport = 8,
        ForceUncompressedImport = 0x4000,
        ForceUpdate = 1,
        ImportRecursive = 0x100
    }
}

