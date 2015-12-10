namespace UnityEngine.Cloud.Service
{
    using System;

    [Flags]
    internal enum CloudEventFlags
    {
        None,
        HighPriority,
        CacheImmediately
    }
}

