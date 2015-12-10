namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class LightmapSettings : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Reset();

        [Obsolete("bakedColorSpace is no longer valid. Use QualitySettings.desiredColorSpace.", false)]
        public static ColorSpace bakedColorSpace
        {
            get
            {
                return QualitySettings.desiredColorSpace;
            }
            set
            {
            }
        }

        public static LightmapData[] lightmaps { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static LightmapsMode lightmapsMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use lightmapsMode property")]
        public static LightmapsModeLegacy lightmapsModeLegacy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static LightProbes lightProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

