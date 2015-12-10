namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class AvatarUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void HumanGetColliderTransform(Avatar avatar, int index, TransformX boneX, out TransformX colliderX);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetHumanPose(Animator animator, float[] dof);
    }
}

