namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class HumanTrait
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int BoneFromMuscle(int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetMuscleDefaultMax(int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetMuscleDefaultMin(int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetParentBone(int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasCollider(Avatar avatar, int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int MuscleFromBone(int i, int dofIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RequiredBone(int i);

        public static int BoneCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string[] BoneName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int MuscleCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string[] MuscleName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int RequiredBoneCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

