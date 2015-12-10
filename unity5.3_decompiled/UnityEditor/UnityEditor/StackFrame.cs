namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StackFrame
    {
        public uint lineNumber;
        public string sourceFile;
        public string methodName;
        public string signature;
        public string moduleName;
    }
}

