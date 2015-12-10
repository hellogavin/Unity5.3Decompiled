namespace UnityEngineInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class GIDebugVisualisation
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CycleSkipInstances(int skip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CycleSkipSystems(int skip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PauseCycleMode();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PlayCycleMode();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetRuntimeInputTextures();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopCycleMode();

        public static bool cycleMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool pauseCycleMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static GITextureType texType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

