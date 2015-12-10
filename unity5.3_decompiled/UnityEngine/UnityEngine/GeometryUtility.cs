namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class GeometryUtility
    {
        public static Plane[] CalculateFrustumPlanes(Camera camera)
        {
            return CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix);
        }

        public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
        {
            Plane[] planes = new Plane[6];
            Internal_ExtractPlanes(planes, worldToProjectionMatrix);
            return planes;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_ExtractPlanes(Plane[] planes, ref Matrix4x4 worldToProjectionMatrix);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_TestPlanesAABB(Plane[] planes, ref Bounds bounds);
        private static void Internal_ExtractPlanes(Plane[] planes, Matrix4x4 worldToProjectionMatrix)
        {
            INTERNAL_CALL_Internal_ExtractPlanes(planes, ref worldToProjectionMatrix);
        }

        public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
        {
            return INTERNAL_CALL_TestPlanesAABB(planes, ref bounds);
        }
    }
}

