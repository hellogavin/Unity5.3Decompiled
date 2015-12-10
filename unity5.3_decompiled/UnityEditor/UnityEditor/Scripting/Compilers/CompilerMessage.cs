namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CompilerMessage
    {
        public string message;
        public string file;
        public int line;
        public int column;
        public CompilerMessageType type;
        public NormalizedCompilerStatus normalizedStatus;
    }
}

