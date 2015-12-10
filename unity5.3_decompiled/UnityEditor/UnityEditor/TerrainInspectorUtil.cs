namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class TerrainInspectorUtil
    {
        public static bool CheckTreeDistance(TerrainData terrainData, Vector3 position, int treeIndex, float distanceBias)
        {
            return INTERNAL_CALL_CheckTreeDistance(terrainData, ref position, treeIndex, distanceBias);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPrototypeCount(TerrainData terrainData);
        public static Vector3 GetPrototypeExtent(TerrainData terrainData, int prototypeIndex)
        {
            Vector3 vector;
            INTERNAL_CALL_GetPrototypeExtent(terrainData, prototypeIndex, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetTreePlacementSize(TerrainData terrainData, int prototypeIndex, float spacing, float treeCount);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CheckTreeDistance(TerrainData terrainData, ref Vector3 position, int treeIndex, float distanceBias);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPrototypeExtent(TerrainData terrainData, int prototypeIndex, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PrototypeIsRenderable(TerrainData terrainData, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RefreshPhysicsInEditMode();
    }
}

