namespace UnityEditor
{
    using System;

    internal enum DownloadResolution
    {
        Unresolved,
        SkipAsset,
        TrashMyChanges,
        TrashServerChanges,
        Merge
    }
}

