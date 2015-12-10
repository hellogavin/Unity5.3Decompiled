namespace UnityEditor.VersionControl
{
    using System;

    [Flags]
    public enum MergeMethod
    {
        MergeAll = 1,
        [Obsolete("This member is no longer supported (UnityUpgradable) -> MergeNone", true)]
        MergeNonConflicting = 2,
        MergeNone = 0
    }
}

