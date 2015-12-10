namespace UnityEditor.VersionControl
{
    using System;

    [Flags]
    public enum RevertMode
    {
        Normal,
        Unchanged,
        KeepModifications
    }
}

