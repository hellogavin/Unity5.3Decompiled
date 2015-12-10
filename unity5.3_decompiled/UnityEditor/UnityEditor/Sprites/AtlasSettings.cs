namespace UnityEditor.Sprites
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct AtlasSettings
    {
        public TextureFormat format;
        public ColorSpace colorSpace;
        public int compressionQuality;
        public FilterMode filterMode;
        public int maxWidth;
        public int maxHeight;
        public uint paddingPower;
        public int anisoLevel;
        public bool generateMipMaps;
        public bool enableRotation;
        public bool allowsAlphaSplitting;
    }
}

