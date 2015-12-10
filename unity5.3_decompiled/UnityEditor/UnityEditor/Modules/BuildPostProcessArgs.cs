namespace UnityEditor.Modules
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct BuildPostProcessArgs
    {
        public BuildTarget target;
        public string stagingArea;
        public string stagingAreaData;
        public string stagingAreaDataManaged;
        public string playerPackage;
        public string installPath;
        public string companyName;
        public string productName;
        public Guid productGUID;
        public BuildOptions options;
        internal RuntimeClassRegistry usedClassRegistry;
    }
}

