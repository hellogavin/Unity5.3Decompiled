namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ProfilerChoise
    {
        public string Name;
        public bool Enabled;
        public Func<bool> IsSelected;
        public Action ConnectTo;
    }
}

