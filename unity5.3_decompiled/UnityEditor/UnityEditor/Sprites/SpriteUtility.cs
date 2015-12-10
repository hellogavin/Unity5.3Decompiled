namespace UnityEditor.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class SpriteUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CreateSpritePolygonAssetAtPath(string pathName, int sides);
        internal static void GenerateOutline(Texture2D texture, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
        {
            INTERNAL_CALL_GenerateOutline(texture, ref rect, detail, alphaTolerance, holeDetection, out paths);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void GenerateOutlineFromSprite(Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Vector2[] GeneratePolygonOutlineVerticesOfSize(int sides, int width, int height);
        [Obsolete("Use Sprite.triangles API instead. This data is the same for packed and unpacked sprites.")]
        public static ushort[] GetSpriteIndices(Sprite sprite, bool getAtlasData)
        {
            return sprite.triangles;
        }

        [Obsolete("Use Sprite.vertices API instead. This data is the same for packed and unpacked sprites.")]
        public static Vector2[] GetSpriteMesh(Sprite sprite, bool getAtlasData)
        {
            return sprite.vertices;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetSpriteTexture(Sprite sprite, bool getAtlasData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector2[] GetSpriteUVs(Sprite sprite, bool getAtlasData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GenerateOutline(Texture2D texture, ref Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);
    }
}

