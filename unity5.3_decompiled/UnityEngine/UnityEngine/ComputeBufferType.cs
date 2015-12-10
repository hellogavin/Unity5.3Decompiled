namespace UnityEngine
{
    using System;

    [Flags]
    public enum ComputeBufferType
    {
        Append = 2,
        Counter = 4,
        Default = 0,
        DrawIndirect = 0x100,
        Raw = 1
    }
}

