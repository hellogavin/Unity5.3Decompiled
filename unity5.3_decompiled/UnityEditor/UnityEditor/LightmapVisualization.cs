namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class LightmapVisualization
    {
        internal static void DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, Color baseColor, Color selectedColor, float scale, Transform cloudTransform)
        {
            INTERNAL_CALL_DrawPointCloud(unselectedPositions, selectedPositions, ref baseColor, ref selectedColor, scale, cloudTransform);
        }

        internal static void DrawTetrahedra(bool shouldRecalculateTetrahedra, Vector3 cameraPosition)
        {
            INTERNAL_CALL_DrawTetrahedra(shouldRecalculateTetrahedra, ref cameraPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float GetLightmapLODLevelScale(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, ref Color baseColor, ref Color selectedColor, float scale, Transform cloudTransform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawTetrahedra(bool shouldRecalculateTetrahedra, ref Vector3 cameraPosition);

        public static bool dynamicUpdateLightProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showLightProbeCells { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showLightProbeLocations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showLightProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool useLightmaps { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

