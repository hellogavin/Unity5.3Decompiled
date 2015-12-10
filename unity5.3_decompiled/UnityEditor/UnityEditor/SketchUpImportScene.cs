namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SketchUpImportScene
    {
        public SketchUpImportCamera camera;
        public string name;
    }
}

