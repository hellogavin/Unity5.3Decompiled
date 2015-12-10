namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Time
    {
        public static int captureFramerate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float deltaTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float fixedDeltaTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float fixedTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int frameCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float maximumDeltaTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float realtimeSinceStartup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int renderedFrameCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float smoothDeltaTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float timeScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float timeSinceLevelLoad { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float unscaledDeltaTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float unscaledTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

