namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Annotation
    {
        public int iconEnabled;
        public int gizmoEnabled;
        public int flags;
        public int classID;
        public string scriptClass;
    }
}

