namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class NavMesh
    {
        public const int AllAreas = -1;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("AddOffMeshLinks has no effect and is deprecated.")]
        public static extern void AddOffMeshLinks();
        public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
        {
            path.ClearCorners();
            return CalculatePathInternal(sourcePosition, targetPosition, areaMask, path);
        }

        internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
        {
            return INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, areaMask, path);
        }

        public static NavMeshTriangulation CalculateTriangulation()
        {
            return (NavMeshTriangulation) TriangulateInternal();
        }

        public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask)
        {
            return INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, areaMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetAreaCost(int areaIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetAreaFromName(string areaName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float GetAvoidancePredictionTime();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Use GetAreaCost instead.")]
        public static extern float GetLayerCost(int layer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Use GetAreaFromName instead.")]
        public static extern int GetNavMeshLayerFromName(string layerName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetPathfindingIterationsPerFrame();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int areaMask, NavMeshPath path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);
        public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask)
        {
            return INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, areaMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("RestoreNavMesh has no effect and is deprecated.")]
        public static extern void RestoreNavMesh();
        public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
        {
            return INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, areaMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAreaCost(int areaIndex, float cost);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetAvoidancePredictionTime(float t);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use SetAreaCost instead."), WrapperlessIcall]
        public static extern void SetLayerCost(int layer, float cost);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPathfindingIterationsPerFrame(int iter);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("use NavMesh.CalculateTriangulation() instead.")]
        public static extern void Triangulate(out Vector3[] vertices, out int[] indices);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern object TriangulateInternal();

        public static float avoidancePredictionTime
        {
            get
            {
                return GetAvoidancePredictionTime();
            }
            set
            {
                SetAvoidancePredictionTime(value);
            }
        }

        public static int pathfindingIterationsPerFrame
        {
            get
            {
                return GetPathfindingIterationsPerFrame();
            }
            set
            {
                SetPathfindingIterationsPerFrame(value);
            }
        }
    }
}

