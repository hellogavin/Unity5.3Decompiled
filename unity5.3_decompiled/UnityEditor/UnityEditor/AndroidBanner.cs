namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct AndroidBanner
    {
        public int width;
        public int height;
        public Texture2D banner;
    }
}

