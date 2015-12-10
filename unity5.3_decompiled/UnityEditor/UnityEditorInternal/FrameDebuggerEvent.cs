namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameDebuggerEvent
    {
        public FrameEventType type;
        public GameObject gameObject;
    }
}

