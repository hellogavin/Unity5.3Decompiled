namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class MeshUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Vector2[] ComputeTextureBoundingHull(Texture texture, int vertexCount);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ModelImporterMeshCompression GetMeshCompression(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Optimize(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetMeshCompression(Mesh mesh, ModelImporterMeshCompression compression);
        public static void SetPerTriangleUV2(Mesh src, Vector2[] triUV)
        {
            int num = InternalMeshUtil.CalcTriangleCount(src);
            int length = triUV.Length;
            if (length != (3 * num))
            {
                Debug.LogError(string.Concat(new object[] { "mesh contains ", num, " triangles but ", length, " uvs are provided" }));
            }
            else
            {
                SetPerTriangleUV2NoCheck(src, triUV);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPerTriangleUV2NoCheck(Mesh src, Vector2[] triUV);
    }
}

