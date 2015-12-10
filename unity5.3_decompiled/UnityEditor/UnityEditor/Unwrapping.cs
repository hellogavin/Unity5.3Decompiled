namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Unwrapping
    {
        public static Vector2[] GeneratePerTriangleUV(Mesh src)
        {
            UnwrapParam param = new UnwrapParam();
            UnwrapParam.SetDefaults(out param);
            return GeneratePerTriangleUV(src, param);
        }

        public static Vector2[] GeneratePerTriangleUV(Mesh src, UnwrapParam settings)
        {
            return GeneratePerTriangleUVImpl(src, settings);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Vector2[] GeneratePerTriangleUVImpl(Mesh src, UnwrapParam settings);
        public static void GenerateSecondaryUVSet(Mesh src)
        {
            MeshUtility.SetPerTriangleUV2(src, GeneratePerTriangleUV(src));
        }

        public static void GenerateSecondaryUVSet(Mesh src, UnwrapParam settings)
        {
            MeshUtility.SetPerTriangleUV2(src, GeneratePerTriangleUV(src, settings));
        }
    }
}

