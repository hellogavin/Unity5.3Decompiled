namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class DirectorPlayer : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern double GetTime();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern DirectorUpdateMode GetTimeUpdateMode();
        public void Play(Playable playable)
        {
            this.PlayInternal(playable, null);
        }

        public void Play(Playable playable, object customData)
        {
            this.PlayInternal(playable, customData);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void PlayInternal(Playable playable, object customData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTime(double time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTimeUpdateMode(DirectorUpdateMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Stop();
    }
}

