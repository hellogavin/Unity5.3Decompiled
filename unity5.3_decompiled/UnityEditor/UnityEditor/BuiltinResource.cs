namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class BuiltinResource
    {
        public string m_Name;
        public int m_InstanceID;
    }
}

