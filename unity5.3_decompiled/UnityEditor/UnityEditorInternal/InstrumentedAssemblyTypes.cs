namespace UnityEditorInternal
{
    using System;

    [Flags]
    public enum InstrumentedAssemblyTypes
    {
        All = 0x7fffffff,
        None = 0,
        Plugins = 4,
        Script = 8,
        System = 1,
        Unity = 2
    }
}

