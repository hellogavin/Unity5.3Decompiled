namespace UnityEngine.Networking.Types
{
    using System;
    using System.ComponentModel;

    [DefaultValue(0L)]
    public enum NetworkAccessLevel : ulong
    {
        Admin = 4L,
        Invalid = 0L,
        Owner = 2L,
        User = 1L
    }
}

