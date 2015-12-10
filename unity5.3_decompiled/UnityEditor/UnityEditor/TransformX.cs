namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TransformX
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}

