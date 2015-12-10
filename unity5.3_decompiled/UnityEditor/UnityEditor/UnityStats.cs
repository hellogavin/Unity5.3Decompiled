namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class UnityStats
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetNetworkStats(int i);

        public static float audioClippingAmount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float audioDSPLoad { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float audioLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float audioStreamLoad { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int batches { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int drawCalls { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int dynamicBatchedDrawCalls { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int dynamicBatches { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float frameTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int ibUploadBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int ibUploads { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int renderTextureBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int renderTextureChanges { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int renderTextureCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float renderTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int screenBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string screenRes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int setPassCalls { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int shadowCasters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int staticBatchedDrawCalls { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int staticBatches { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int triangles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int usedTextureCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int usedTextureMemorySize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int vboTotal { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int vboTotalBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int vboUploadBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int vboUploads { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int vertices { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int visibleAnimations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int visibleSkinnedMeshes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

