namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AnimatorTransition : AnimatorTransitionBase
    {
        public AnimatorTransition()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create(AnimatorTransition mono);
    }
}

