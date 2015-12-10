namespace UnityEditor
{
    using System;

    [Flags]
    internal enum CrossCompileOptions
    {
        Debugging = 4,
        Dynamic = 0,
        ExplicitNullChecks = 8,
        FastICall = 1,
        LoadSymbols = 0x10,
        Static = 2
    }
}

