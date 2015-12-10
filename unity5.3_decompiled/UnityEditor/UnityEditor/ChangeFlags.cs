namespace UnityEditor
{
    using System;

    [Flags]
    internal enum ChangeFlags
    {
        Created = 0x20,
        Deleted = 8,
        Modified = 1,
        Moved = 4,
        None = 0,
        Renamed = 2,
        Undeleted = 0x10
    }
}

