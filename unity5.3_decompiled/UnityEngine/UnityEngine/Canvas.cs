namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    public sealed class Canvas : Behaviour
    {
        public static  event WillRenderCanvases willRenderCanvases;

        public static void ForceUpdateCanvases()
        {
            SendWillRenderCanvases();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Material GetDefaultCanvasMaterial();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Shared default material now used for text and general UI elements, call Canvas.GetDefaultCanvasMaterial()")]
        public static extern Material GetDefaultCanvasTextMaterial();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_pixelRect(out Rect value);
        [RequiredByNativeCode]
        private static void SendWillRenderCanvases()
        {
            if (willRenderCanvases != null)
            {
                willRenderCanvases();
            }
        }

        public int cachedSortingLayerValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isRootCanvas { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool overridePixelPerfect { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool overrideSorting { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool pixelPerfect { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Rect pixelRect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_pixelRect(out rect);
                return rect;
            }
        }

        public float planeDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float referencePixelsPerUnit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int renderOrder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float scaleFactor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int sortingLayerID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string sortingLayerName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int sortingOrder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int targetDisplay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Camera worldCamera { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public delegate void WillRenderCanvases();
    }
}

