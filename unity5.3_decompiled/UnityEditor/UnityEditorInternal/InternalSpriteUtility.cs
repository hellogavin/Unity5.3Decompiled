namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class InternalSpriteUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Rect[] GenerateAutomaticSpriteRectangles(Texture2D texture, int minRectSize, int extrudeSize);
        public static Rect[] GenerateGridSpriteRectangles(Texture2D texture, Vector2 offset, Vector2 size, Vector2 padding)
        {
            return INTERNAL_CALL_GenerateGridSpriteRectangles(texture, ref offset, ref size, ref padding);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Rect[] INTERNAL_CALL_GenerateGridSpriteRectangles(Texture2D texture, ref Vector2 offset, ref Vector2 size, ref Vector2 padding);
    }
}

