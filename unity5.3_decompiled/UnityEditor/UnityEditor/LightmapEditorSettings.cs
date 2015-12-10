namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class LightmapEditorSettings
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetGeometryHash(Renderer renderer, out Hash128 geometryHash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetInputSystemHash(Renderer renderer, out Hash128 inputSystemHash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetInstanceHash(Renderer renderer, out Hash128 instanceHash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetInstanceResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Object GetLightmapSettings();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetSystemResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetTerrainSystemResolution(Terrain terrain, out int width, out int height, out int numChunksInX, out int numChunksInY);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasClampedResolution(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasZeroAreaMesh(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsLightmappedOrDynamicLightmappedForRendering(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Reset();

        [Obsolete("LightmapEditorSettings.aoAmount has been deprecated.", false)]
        public static float aoAmount
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.aoContrast has been deprecated.", false)]
        public static float aoContrast
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        public static float aoMaxDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float bakeResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("LightmapEditorSettings.bounceBoost has been deprecated.", false)]
        public static float bounceBoost
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounceIntensity has been deprecated.", false)]
        public static float bounceIntensity
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounces has been deprecated.", false)]
        public static int bounces
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherContrastThreshold has been deprecated.", false)]
        public static float finalGatherContrastThreshold
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherGradientThreshold has been deprecated.", false)]
        public static float finalGatherGradientThreshold
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherInterpolationPoints has been deprecated.", false)]
        public static int finalGatherInterpolationPoints
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherRays has been deprecated.", false)]
        public static int finalGatherRays
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lastUsedResolution has been deprecated.", false)]
        public static float lastUsedResolution
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lockAtlas has been deprecated.", false)]
        public static bool lockAtlas
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static int maxAtlasHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int maxAtlasWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int padding { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("LightmapEditorSettings.quality has been deprecated.", false)]
        public static LightmapBakeQuality quality
        {
            get
            {
                return LightmapBakeQuality.High;
            }
            set
            {
            }
        }

        public static ReflectionCubemapCompression reflectionCubemapCompression { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float resolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("LightmapEditorSettings.skyLightColor has been deprecated.", false)]
        public static Color skyLightColor
        {
            get
            {
                return Color.black;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.skyLightIntensity has been deprecated.", false)]
        public static float skyLightIntensity
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        public static bool textureCompression { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

