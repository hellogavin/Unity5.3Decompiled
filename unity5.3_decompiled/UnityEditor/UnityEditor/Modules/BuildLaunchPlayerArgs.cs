namespace UnityEditor.Modules
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct BuildLaunchPlayerArgs
    {
        public BuildTarget target;
        public string playerPackage;
        public string installPath;
        public string productName;
        public BuildOptions options;
    }
}

