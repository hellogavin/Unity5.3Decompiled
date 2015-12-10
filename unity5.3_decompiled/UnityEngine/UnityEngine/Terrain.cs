namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class Terrain : Behaviour
    {
        public void AddTreeInstance(TreeInstance instance)
        {
            INTERNAL_CALL_AddTreeInstance(this, ref instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ApplyDelayedHeightmapModification();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject CreateTerrainGameObject(TerrainData assignTerrain);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Flush();
        public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
        {
            this.GetClosestReflectionProbesInternal(result);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetClosestReflectionProbesInternal(object result);
        public Vector3 GetPosition()
        {
            Vector3 vector;
            INTERNAL_CALL_GetPosition(this, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddTreeInstance(Terrain self, ref TreeInstance instance);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPosition(Terrain self, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_RemoveTrees(Terrain self, ref Vector2 position, float radius, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float INTERNAL_CALL_SampleHeight(Terrain self, ref Vector3 worldPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_legacySpecular(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_legacySpecular(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);
        internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
        {
            INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);
        }

        public float SampleHeight(Vector3 worldPosition)
        {
            return INTERNAL_CALL_SampleHeight(this, ref worldPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom);

        public static Terrain activeTerrain { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Terrain[] activeTerrains { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool bakeLightProbesForTrees { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float basemapDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool castShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool collectDetailPatches { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float detailObjectDensity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float detailObjectDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool drawHeightmap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool drawTreesAndFoliage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TerrainRenderFlags editorRenderFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int heightmapMaximumLOD { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float heightmapPixelError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float legacyShininess { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color legacySpecular
        {
            get
            {
                Color color;
                this.INTERNAL_get_legacySpecular(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_legacySpecular(ref value);
            }
        }

        public int lightmapIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector4 lightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_lightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_lightmapScaleOffset(ref value);
            }
        }

        public Material materialTemplate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public MaterialType materialType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int realtimeLightmapIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector4 realtimeLightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_realtimeLightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_realtimeLightmapScaleOffset(ref value);
            }
        }

        public ReflectionProbeUsage reflectionProbeUsage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("use basemapDistance", true)]
        public float splatmapDistance
        {
            get
            {
                return this.basemapDistance;
            }
            set
            {
                this.basemapDistance = value;
            }
        }

        public TerrainData terrainData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float treeBillboardDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float treeCrossFadeLength { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float treeDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int treeMaximumFullLODCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public enum MaterialType
        {
            BuiltInStandard,
            BuiltInLegacyDiffuse,
            BuiltInLegacySpecular,
            Custom
        }
    }
}

