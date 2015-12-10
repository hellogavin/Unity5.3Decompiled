namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct DrawGridParameters
    {
        public Vector3 pivot;
        public Color color;
        public float size;
        public float alphaX;
        public float alphaY;
        public float alphaZ;
    }
}

