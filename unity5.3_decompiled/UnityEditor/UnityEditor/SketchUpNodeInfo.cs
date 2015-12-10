namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct SketchUpNodeInfo
    {
        public string name;
        public int parent;
        public bool enabled;
        public int nodeIndex;
    }
}

