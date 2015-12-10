namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawArguments
    {
        public IntPtr target;
        public Rect position;
        public int isHover;
        public int isActive;
        public int on;
        public int hasKeyboardFocus;
    }
}

