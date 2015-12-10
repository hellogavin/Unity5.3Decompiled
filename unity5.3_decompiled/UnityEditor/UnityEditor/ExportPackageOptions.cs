namespace UnityEditor
{
    using System;

    [Flags]
    public enum ExportPackageOptions
    {
        Default = 0,
        IncludeDependencies = 4,
        IncludeLibraryAssets = 8,
        Interactive = 1,
        Recurse = 2
    }
}

