namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    public sealed class RenderSettings : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Object GetRenderSettings();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_ambientEquatorColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_ambientGroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_ambientLight(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_ambientProbe(out SphericalHarmonicsL2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_ambientSkyColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_fogColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_ambientEquatorColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_ambientGroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_ambientLight(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_ambientProbe(ref SphericalHarmonicsL2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_ambientSkyColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_fogColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Reset();

        public static Color ambientEquatorColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientEquatorColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientEquatorColor(ref value);
            }
        }

        public static Color ambientGroundColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientGroundColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientGroundColor(ref value);
            }
        }

        public static float ambientIntensity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Color ambientLight
        {
            get
            {
                Color color;
                INTERNAL_get_ambientLight(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientLight(ref value);
            }
        }

        public static AmbientMode ambientMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static SphericalHarmonicsL2 ambientProbe
        {
            get
            {
                SphericalHarmonicsL2 sl;
                INTERNAL_get_ambientProbe(out sl);
                return sl;
            }
            set
            {
                INTERNAL_set_ambientProbe(ref value);
            }
        }

        [Obsolete("Use RenderSettings.ambientIntensity instead (UnityUpgradable) -> ambientIntensity", false)]
        public static float ambientSkyboxAmount
        {
            get
            {
                return ambientIntensity;
            }
            set
            {
                ambientIntensity = value;
            }
        }

        public static Color ambientSkyColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientSkyColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientSkyColor(ref value);
            }
        }

        public static Cubemap customReflection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static DefaultReflectionMode defaultReflectionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int defaultReflectionResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float flareFadeSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float flareStrength { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool fog { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Color fogColor
        {
            get
            {
                Color color;
                INTERNAL_get_fogColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_fogColor(ref value);
            }
        }

        public static float fogDensity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float fogEndDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static FogMode fogMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float fogStartDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float haloStrength { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int reflectionBounces { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float reflectionIntensity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Material skybox { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

