namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class HumanPoseHandler : IDisposable
    {
        internal IntPtr m_Ptr = IntPtr.Zero;

        public HumanPoseHandler(Avatar avatar, Transform root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("HumanPoseHandler root Transform is null");
            }
            if (avatar == null)
            {
                throw new ArgumentNullException("HumanPoseHandler avatar is null");
            }
            if (!avatar.isValid)
            {
                throw new ArgumentException("HumanPoseHandler avatar is invalid");
            }
            if (!avatar.isHuman)
            {
                throw new ArgumentException("HumanPoseHandler avatar is not human");
            }
            this.Internal_HumanPoseHandler(avatar, root);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        public void GetHumanPose(ref HumanPose humanPose)
        {
            humanPose.Init();
            if (!this.Internal_GetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
            {
                Debug.LogWarning("HumanPoseHandler is not initialized properly");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_GetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_SetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);
        private bool Internal_GetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
        {
            return INTERNAL_CALL_Internal_GetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_HumanPoseHandler(Avatar avatar, Transform root);
        private bool Internal_SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
        {
            return INTERNAL_CALL_Internal_SetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);
        }

        public void SetHumanPose(ref HumanPose humanPose)
        {
            humanPose.Init();
            if (!this.Internal_SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
            {
                Debug.LogWarning("HumanPoseHandler is not initialized properly");
            }
        }
    }
}

