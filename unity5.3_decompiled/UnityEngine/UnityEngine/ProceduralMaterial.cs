namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ProceduralMaterial : Material
    {
        internal ProceduralMaterial() : base((Material) null)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CacheProceduralProperty(string inputName, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProceduralTexture GetGeneratedTexture(string textureName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture[] GetGeneratedTextures();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetProceduralBoolean(string inputName);
        public Color GetProceduralColor(string inputName)
        {
            Color color;
            INTERNAL_CALL_GetProceduralColor(this, inputName, out color);
            return color;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetProceduralEnum(string inputName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetProceduralFloat(string inputName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProceduralPropertyDescription[] GetProceduralPropertyDescriptions();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture2D GetProceduralTexture(string inputName);
        public Vector4 GetProceduralVector(string inputName)
        {
            Vector4 vector;
            INTERNAL_CALL_GetProceduralVector(this, inputName, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasProceduralProperty(string inputName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetProceduralColor(ProceduralMaterial self, string inputName, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetProceduralVector(ProceduralMaterial self, string inputName, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetProceduralColor(ProceduralMaterial self, string inputName, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetProceduralVector(ProceduralMaterial self, string inputName, ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsProceduralPropertyCached(string inputName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsProceduralPropertyVisible(string inputName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RebuildTextures();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RebuildTexturesImmediately();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetProceduralBoolean(string inputName, bool value);
        public void SetProceduralColor(string inputName, Color value)
        {
            INTERNAL_CALL_SetProceduralColor(this, inputName, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetProceduralEnum(string inputName, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetProceduralFloat(string inputName, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetProceduralTexture(string inputName, Texture2D value);
        public void SetProceduralVector(string inputName, Vector4 value)
        {
            INTERNAL_CALL_SetProceduralVector(this, inputName, ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopRebuilds();

        public int animationUpdateRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ProceduralCacheSize cacheSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isCachedDataAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isLoadTimeGenerated { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isProcessing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ProceduralLoadingBehavior loadingBehavior { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string preset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ProceduralProcessorUsage substanceProcessorUsage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

