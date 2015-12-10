namespace UnityEditor.VersionControl
{
    using System;

    [Flags]
    public enum ResolveMethod
    {
        UseMerged = 3,
        UseMine = 1,
        UseTheirs = 2
    }
}

