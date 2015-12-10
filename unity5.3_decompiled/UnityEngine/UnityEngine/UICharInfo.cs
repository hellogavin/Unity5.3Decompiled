namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct UICharInfo
    {
        public Vector2 cursorPos;
        public float charWidth;
    }
}

