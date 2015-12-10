namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteMetaData
    {
        public string name;
        public Rect rect;
        public int alignment;
        public Vector2 pivot;
        public Vector4 border;
    }
}

