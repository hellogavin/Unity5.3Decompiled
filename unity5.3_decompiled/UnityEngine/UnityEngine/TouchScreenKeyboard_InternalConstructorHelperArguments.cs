namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TouchScreenKeyboard_InternalConstructorHelperArguments
    {
        public uint keyboardType;
        public uint autocorrection;
        public uint multiline;
        public uint secure;
        public uint alert;
    }
}

