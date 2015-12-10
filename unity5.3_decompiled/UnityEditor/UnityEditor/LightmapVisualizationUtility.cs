namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngineInternal;

    internal sealed class LightmapVisualizationUtility
    {
        public static void DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, Rect drawableArea, Rect position, GITextureType textureType, bool drawSpecularUV)
        {
            INTERNAL_CALL_DrawTextureWithUVOverlay(texture, gameObject, ref drawableArea, ref position, textureType, drawSpecularUV);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetGITexture(GITextureType textureType);
        public static Vector4 GetLightmapTilingOffset(LightmapType lightmapType)
        {
            Vector4 vector;
            INTERNAL_CALL_GetLightmapTilingOffset(lightmapType, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, ref Rect drawableArea, ref Rect position, GITextureType textureType, bool drawSpecularUV);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetLightmapTilingOffset(LightmapType lightmapType, out Vector4 value);
    }
}

