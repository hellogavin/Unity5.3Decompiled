namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class SpeedTreeImporter : AssetImporter
    {
        public static readonly string[] windQualityNames = new string[] { "None", "Fastest", "Fast", "Better", "Best", "Palm" };

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GenerateMaterials();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_hueVariation(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_mainColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_specColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_hueVariation(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_mainColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_specColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetMaterialVersionToCurrent();

        public float alphaTestRef { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool animateCrossFading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int bestWindQuality { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float billboardTransitionCrossFadeWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool[] castShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool[] enableBump { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool[] enableHue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enableSmoothLODTransition { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float fadeOutWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool hasBillboard { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasImported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Color hueVariation
        {
            get
            {
                Color color;
                this.INTERNAL_get_hueVariation(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_hueVariation(ref value);
            }
        }

        public float[] LODHeights { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color mainColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_mainColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_mainColor(ref value);
            }
        }

        public string materialFolderPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal bool materialsShouldBeRegenerated { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool[] receiveShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ReflectionProbeUsage[] reflectionProbeUsages { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float scaleFactor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float shininess { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color specColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_specColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_specColor(ref value);
            }
        }

        public bool[] useLightProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int[] windQualities { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

