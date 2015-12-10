namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectReferenceKeyframe
    {
        public float time;
        public Object value;
    }
}

