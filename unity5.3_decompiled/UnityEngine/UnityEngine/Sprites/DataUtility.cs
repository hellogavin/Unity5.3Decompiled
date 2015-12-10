namespace UnityEngine.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class DataUtility
    {
        public static Vector4 GetInnerUV(Sprite sprite)
        {
            Vector4 vector;
            INTERNAL_CALL_GetInnerUV(sprite, out vector);
            return vector;
        }

        public static Vector2 GetMinSize(Sprite sprite)
        {
            Vector2 vector;
            Internal_GetMinSize(sprite, out vector);
            return vector;
        }

        public static Vector4 GetOuterUV(Sprite sprite)
        {
            Vector4 vector;
            INTERNAL_CALL_GetOuterUV(sprite, out vector);
            return vector;
        }

        public static Vector4 GetPadding(Sprite sprite)
        {
            Vector4 vector;
            INTERNAL_CALL_GetPadding(sprite, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetInnerUV(Sprite sprite, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetOuterUV(Sprite sprite, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPadding(Sprite sprite, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_GetMinSize(Sprite sprite, out Vector2 output);
    }
}

