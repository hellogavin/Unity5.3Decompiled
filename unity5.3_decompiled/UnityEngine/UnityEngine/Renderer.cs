namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    public class Renderer : Component
    {
        public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
        {
            this.GetClosestReflectionProbesInternal(result);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetClosestReflectionProbesInternal(object result);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GetPropertyBlock(MaterialPropertyBlock dest);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RenderNow(int material);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPropertyBlock(MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetSubsetIndex(int index, int subSetIndexForMaterial);

        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property castShadows has been deprecated. Use shadowCastingMode instead.")]
        public bool castShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isPartOfStaticBatch { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isVisible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

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

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property lightmapTilingOffset has been deprecated. Use lightmapScaleOffset (UnityUpgradable) -> lightmapScaleOffset", true)]
        public Vector4 lightmapTilingOffset
        {
            get
            {
                return Vector4.zero;
            }
            set
            {
            }
        }

        [Obsolete("Use probeAnchor instead (UnityUpgradable) -> probeAnchor", true)]
        public Transform lightProbeAnchor
        {
            get
            {
                return this.probeAnchor;
            }
            set
            {
                this.probeAnchor = value;
            }
        }

        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_localToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        public Material material { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Material[] materials { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Transform probeAnchor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

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

        public bool receiveShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ReflectionProbeUsage reflectionProbeUsage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ShadowCastingMode shadowCastingMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Material sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Material[] sharedMaterials { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int sortingLayerID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string sortingLayerName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int sortingOrder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal int staticBatchIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal Transform staticBatchRootTransform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useLightProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToLocalMatrix(out matrixx);
                return matrixx;
            }
        }
    }
}

