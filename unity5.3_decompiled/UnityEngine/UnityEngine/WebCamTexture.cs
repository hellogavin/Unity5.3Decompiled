namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class WebCamTexture : Texture
    {
        public WebCamTexture()
        {
            Internal_CreateWebCamTexture(this, string.Empty, 0, 0, 0);
        }

        public WebCamTexture(string deviceName)
        {
            Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
        }

        public WebCamTexture(int requestedWidth, int requestedHeight)
        {
            Internal_CreateWebCamTexture(this, string.Empty, requestedWidth, requestedHeight, 0);
        }

        public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
        {
            Internal_CreateWebCamTexture(this, string.Empty, requestedWidth, requestedHeight, requestedFPS);
        }

        public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
        {
            Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
        }

        public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
        {
            Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
        }

        public Color GetPixel(int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, x, y, out color);
            return color;
        }

        public Color[] GetPixels()
        {
            return this.GetPixels(0, 0, this.width, this.height);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            Color32[] colors = null;
            return this.GetPixels32(colors);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color32[] GetPixels32([DefaultValue("null")] Color32[] colors);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPixel(WebCamTexture self, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Pause(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Play(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Stop(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand"), WrapperlessIcall]
        public extern void MarkNonReadable();
        public void Pause()
        {
            INTERNAL_CALL_Pause(this);
        }

        public void Play()
        {
            INTERNAL_CALL_Play(this);
        }

        public void Stop()
        {
            INTERNAL_CALL_Stop(this);
        }

        public string deviceName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static WebCamDevice[] devices { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool didUpdateThisFrame { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand")]
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float requestedFPS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int requestedHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int requestedWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int videoRotationAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool videoVerticallyMirrored { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

