namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AnimatorUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeoptimizeTransformHierarchy(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OptimizeTransformHierarchy(GameObject go, string[] exposedTransforms);
    }
}

