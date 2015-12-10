namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class AnimationClipPair
    {
        public AnimationClip originalClip;
        public AnimationClip overrideClip;
    }
}

