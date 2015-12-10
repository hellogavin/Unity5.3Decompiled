namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NormalizedCompilerStatus
    {
        public NormalizedCompilerStatusCode code;
        public string details;
    }
}

