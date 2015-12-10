namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderError
    {
        public string message;
        public string messageDetails;
        public string platform;
        public string file;
        public int line;
        public int warning;
    }
}

