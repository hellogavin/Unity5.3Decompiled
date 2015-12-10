namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AnimationMixerPlayable : AnimationPlayable
    {
        public AnimationMixerPlayable() : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            this.InstantiateEnginePlayable();
        }

        public AnimationMixerPlayable(bool final) : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            if (final)
            {
                this.InstantiateEnginePlayable();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InstantiateEnginePlayable();
        public bool SetInputs(AnimationClip[] clips)
        {
            if (clips == null)
            {
                throw new NullReferenceException("Parameter clips was null. You need to pass in a valid array of clips.");
            }
            AnimationPlayable[] sources = new AnimationPlayable[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                sources[i] = new AnimationClipPlayable(clips[i]);
            }
            return base.SetInputs(sources);
        }
    }
}

