namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PluginDesc
    {
        public string pluginPath;
        public CPUArch architecture;
    }
}

