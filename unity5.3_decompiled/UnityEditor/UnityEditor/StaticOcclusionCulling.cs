namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class StaticOcclusionCulling
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Cancel();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Compute();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GenerateInBackground();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetDefaultOcclusionBakeSettings();

        public static float backfaceThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool doesSceneHaveManualPortals { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float smallestHole { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float smallestOccluder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int umbraDataSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

