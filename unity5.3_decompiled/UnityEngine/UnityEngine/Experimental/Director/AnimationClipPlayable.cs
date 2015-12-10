namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AnimationClipPlayable : AnimationPlayable
    {
        public AnimationClipPlayable(AnimationClip clip) : base(false)
        {
            base.m_Ptr = IntPtr.Zero;
            this.InstantiateEnginePlayable(clip);
        }

        public override int AddInput(AnimationPlayable source)
        {
            Debug.LogError("AnimationClipPlayable doesn't support adding inputs");
            return -1;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InstantiateEnginePlayable(AnimationClip clip);
        public override bool RemoveAllInputs()
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public override bool RemoveInput(int index)
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public override bool RemoveInput(AnimationPlayable playable)
        {
            Debug.LogError("AnimationClipPlayable doesn't support removing inputs");
            return false;
        }

        public override bool SetInput(AnimationPlayable source, int index)
        {
            Debug.LogError("AnimationClipPlayable doesn't support setting inputs");
            return false;
        }

        public override bool SetInputs(IEnumerable<AnimationPlayable> sources)
        {
            Debug.LogError("AnimationClipPlayable doesn't support setting inputs");
            return false;
        }

        public AnimationClip clip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float speed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

