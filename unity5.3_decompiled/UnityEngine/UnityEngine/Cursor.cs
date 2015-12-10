namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Cursor
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetCursor(Texture2D texture, ref Vector2 hotspot, CursorMode cursorMode);
        private static void SetCursor(Texture2D texture, CursorMode cursorMode)
        {
            SetCursor(texture, Vector2.zero, cursorMode);
        }

        public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            INTERNAL_CALL_SetCursor(texture, ref hotspot, cursorMode);
        }

        public static CursorLockMode lockState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool visible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

