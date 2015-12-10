namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class RenderTexture : Texture
    {
        public RenderTexture(int width, int height, int depth)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = RenderTextureFormat.Default;
            Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
        }

        public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = format;
            Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
        }

        public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            Internal_CreateRenderTexture(this);
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = format;
            bool sRGB = readWrite == RenderTextureReadWrite.sRGB;
            if (readWrite == RenderTextureReadWrite.Default)
            {
                sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
            }
            Internal_SetSRGBReadWrite(this, sRGB);
        }

        public bool Create()
        {
            return INTERNAL_CALL_Create(this);
        }

        public void DiscardContents()
        {
            INTERNAL_CALL_DiscardContents(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DiscardContents(bool discardColor, bool discardDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetColorBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetDepthBuffer(out RenderBuffer res);
        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            int depthBuffer = 0;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            RenderTextureFormat format = RenderTextureFormat.Default;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
        {
            int antiAliasing = 1;
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [ExcludeFromDocs]
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            int antiAliasing = 1;
            return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing);
        public Vector2 GetTexelOffset()
        {
            Vector2 vector;
            Internal_GetTexelOffset(this, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Create(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_IsCreated(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Release(RenderTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateRenderTexture([Writable] RenderTexture rt);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetHeight(RenderTexture mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_GetTexelOffset(RenderTexture tex, out Vector2 output);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetWidth(RenderTexture mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetHeight(RenderTexture mono, int width);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetWidth(RenderTexture mono, int width);
        public bool IsCreated()
        {
            return INTERNAL_CALL_IsCreated(this);
        }

        public void MarkRestoreExpected()
        {
            INTERNAL_CALL_MarkRestoreExpected(this);
        }

        public void Release()
        {
            INTERNAL_CALL_Release(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ReleaseTemporary(RenderTexture temp);
        [Obsolete("SetBorderColor is no longer supported.", true)]
        public void SetBorderColor(Color color)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetGlobalShaderProperty(string propertyName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SupportsStencil(RenderTexture rt);

        public static RenderTexture active { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int antiAliasing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RenderBuffer colorBuffer
        {
            get
            {
                RenderBuffer buffer;
                this.GetColorBuffer(out buffer);
                return buffer;
            }
        }

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RenderBuffer depthBuffer
        {
            get
            {
                RenderBuffer buffer;
                this.GetDepthBuffer(out buffer);
                return buffer;
            }
        }

        [Obsolete("Use SystemInfo.supportsRenderTextures instead.")]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enableRandomWrite { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RenderTextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool generateMips { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public override int height
        {
            get
            {
                return Internal_GetHeight(this);
            }
            set
            {
                Internal_SetHeight(this, value);
            }
        }

        public bool isCubemap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isPowerOfTwo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isVolume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool sRGB { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool useMipMap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int volumeDepth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public override int width
        {
            get
            {
                return Internal_GetWidth(this);
            }
            set
            {
                Internal_SetWidth(this, value);
            }
        }
    }
}

