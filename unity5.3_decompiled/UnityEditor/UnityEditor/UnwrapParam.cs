namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct UnwrapParam
    {
        public float angleError;
        public float areaError;
        public float hardAngle;
        public float packMargin;
        internal int recollectVertices;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetDefaults(out UnwrapParam param);
    }
}

