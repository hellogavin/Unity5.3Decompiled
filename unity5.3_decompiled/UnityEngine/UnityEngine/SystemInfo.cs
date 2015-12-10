namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Rendering;

    public sealed class SystemInfo
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SupportsRenderTextureFormat(RenderTextureFormat format);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SupportsTextureFormat(TextureFormat format);

        public static string deviceModel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string deviceName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static DeviceType deviceType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string deviceUniqueIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int graphicsDeviceID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string graphicsDeviceName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static GraphicsDeviceType graphicsDeviceType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string graphicsDeviceVendor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int graphicsDeviceVendorID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string graphicsDeviceVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int graphicsMemorySize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool graphicsMultiThreaded { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
        public static int graphicsPixelFillrate
        {
            get
            {
                return -1;
            }
        }

        public static int graphicsShaderLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int maxTextureSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NPOTSupport npotSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string operatingSystem { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int processorCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int processorFrequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string processorType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int supportedRenderTargetCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supports3DTextures { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsAccelerometer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsComputeShaders { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsGyroscope { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsImageEffects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsInstancing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsLocationService { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsRawShadowDepthSampling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsRenderTextures { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsRenderToCubemap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsShadows { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool supportsSparseTextures { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int supportsStencil { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Vertex program support is required in Unity 5.0+")]
        public static bool supportsVertexPrograms
        {
            get
            {
                return true;
            }
        }

        public static bool supportsVibration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int systemMemorySize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

