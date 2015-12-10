namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct UILineInfo
    {
        public int startCharIdx;
        public int height;
        public float topY;
    }
}

