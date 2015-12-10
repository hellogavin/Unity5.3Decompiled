namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AvatarMask : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AvatarMask();
        internal void Copy(AvatarMask other)
        {
            for (AvatarMaskBodyPart part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part += 1)
            {
                this.SetHumanoidBodyPartActive(part, other.GetHumanoidBodyPartActive(part));
            }
            this.transformCount = other.transformCount;
            for (int i = 0; i < other.transformCount; i++)
            {
                this.SetTransformPath(i, other.GetTransformPath(i));
                this.SetTransformActive(i, other.GetTransformActive(i));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetTransformActive(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetTransformPath(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTransformActive(int index, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTransformPath(int index, string path);

        internal bool hasFeetIK { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("AvatarMask.humanoidBodyPartCount is deprecated. Use AvatarMaskBodyPart.LastBodyPart instead.")]
        private int humanoidBodyPartCount
        {
            get
            {
                return 13;
            }
        }

        public int transformCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

