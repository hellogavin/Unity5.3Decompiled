namespace UnityEngine
{
    using System;

    [Flags]
    public enum EventModifiers
    {
        Alt = 4,
        CapsLock = 0x20,
        Command = 8,
        Control = 2,
        FunctionKey = 0x40,
        None = 0,
        Numeric = 0x10,
        Shift = 1
    }
}

