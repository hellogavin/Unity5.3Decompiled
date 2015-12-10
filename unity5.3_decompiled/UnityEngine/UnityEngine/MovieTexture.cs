namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class MovieTexture : Texture
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Pause(MovieTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Play(MovieTexture self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Stop(MovieTexture self);
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

        public AudioClip audioClip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float duration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isReadyToPlay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

