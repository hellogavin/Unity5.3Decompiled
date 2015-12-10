namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class LODUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float CalculateDistance(Camera camera, float relativeScreenHeight, LODGroup group);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CalculateLODGroupBoundingBox(LODGroup group);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern LODVisualizationInformation CalculateVisualizationData(Camera camera, LODGroup group, int lodLevel);
    }
}

