namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class InternalMeshUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int CalcTriangleCount(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetCachedMeshSurfaceArea(MeshRenderer meshRenderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetCachedSkinnedMeshSurfaceArea(SkinnedMeshRenderer skinnedMeshRenderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPrimitiveCount(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetVertexFormat(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasNormals(Mesh mesh);
    }
}

