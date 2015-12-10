namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class GUIElement : Behaviour
    {
        [ExcludeFromDocs]
        public Rect GetScreenRect()
        {
            Camera camera = null;
            Rect rect;
            INTERNAL_CALL_GetScreenRect(this, camera, out rect);
            return rect;
        }

        public Rect GetScreenRect([DefaultValue("null")] Camera camera)
        {
            Rect rect;
            INTERNAL_CALL_GetScreenRect(this, camera, out rect);
            return rect;
        }

        [ExcludeFromDocs]
        public bool HitTest(Vector3 screenPosition)
        {
            Camera camera = null;
            return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
        }

        public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera)
        {
            return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetScreenRect(GUIElement self, Camera camera, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_HitTest(GUIElement self, ref Vector3 screenPosition, Camera camera);
    }
}

