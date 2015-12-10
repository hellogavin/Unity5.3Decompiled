namespace UnityEngine
{
    using System.Runtime.CompilerServices;

    public class RuntimeAnimatorController : Object
    {
        public AnimationClip[] animationClips { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

