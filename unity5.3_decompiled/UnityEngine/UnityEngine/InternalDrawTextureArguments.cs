namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct InternalDrawTextureArguments
    {
        public Rect screenRect;
        public Texture texture;
        public Rect sourceRect;
        public int leftBorder;
        public int rightBorder;
        public int topBorder;
        public int bottomBorder;
        public Color32 color;
        public Material mat;
    }
}

