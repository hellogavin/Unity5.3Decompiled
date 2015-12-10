namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public sealed class AnimatorOverrideController : RuntimeAnimatorController
    {
        internal OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

        public AnimatorOverrideController()
        {
            Internal_CreateAnimationSet(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AnimationClip[] GetOriginalClips();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AnimationClip[] GetOverrideClips();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateAnimationSet([Writable] AnimatorOverrideController self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AnimationClip Internal_GetClip(AnimationClip originalClip, bool returnEffectiveClip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetClipByName(string name, AnimationClip clip);
        internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
        {
            if (controller.OnOverrideControllerDirty != null)
            {
                controller.OnOverrideControllerDirty();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void PerformOverrideClipListCleanup();

        public AnimationClipPair[] clips
        {
            get
            {
                AnimationClip[] originalClips = this.GetOriginalClips();
                Dictionary<AnimationClip, bool> dictionary = new Dictionary<AnimationClip, bool>(originalClips.Length);
                foreach (AnimationClip clip in originalClips)
                {
                    dictionary[clip] = true;
                }
                originalClips = new AnimationClip[dictionary.Count];
                dictionary.Keys.CopyTo(originalClips, 0);
                AnimationClipPair[] pairArray = new AnimationClipPair[originalClips.Length];
                for (int i = 0; i < originalClips.Length; i++)
                {
                    pairArray[i] = new AnimationClipPair();
                    pairArray[i].originalClip = originalClips[i];
                    pairArray[i].overrideClip = this.Internal_GetClip(originalClips[i], false);
                }
                return pairArray;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this.Internal_SetClip(value[i].originalClip, value[i].overrideClip);
                }
            }
        }

        public AnimationClip this[string name]
        {
            get
            {
                return this.Internal_GetClipByName(name, true);
            }
            set
            {
                this.Internal_SetClipByName(name, value);
            }
        }

        public AnimationClip this[AnimationClip clip]
        {
            get
            {
                return this.Internal_GetClip(clip, true);
            }
            set
            {
                this.Internal_SetClip(clip, value);
            }
        }

        public RuntimeAnimatorController runtimeAnimatorController { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal delegate void OnOverrideControllerDirtyCallback();
    }
}

