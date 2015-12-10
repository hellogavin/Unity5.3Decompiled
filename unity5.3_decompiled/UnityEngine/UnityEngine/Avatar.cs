namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class Avatar : Object
    {
        private Avatar()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetAxisLength(int humanId);
        internal Vector3 GetLimitSign(int humanId)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLimitSign(this, humanId, out vector);
            return vector;
        }

        internal Quaternion GetPostRotation(int humanId)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetPostRotation(this, humanId, out quaternion);
            return quaternion;
        }

        internal Quaternion GetPreRotation(int humanId)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetPreRotation(this, humanId, out quaternion);
            return quaternion;
        }

        internal Quaternion GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetZYPostQ(this, humanId, ref parentQ, ref q, out quaternion);
            return quaternion;
        }

        internal Quaternion GetZYRoll(int humanId, Vector3 uvw)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetZYRoll(this, humanId, ref uvw, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetLimitSign(Avatar self, int humanId, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPostRotation(Avatar self, int humanId, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPreRotation(Avatar self, int humanId, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetZYPostQ(Avatar self, int humanId, ref Quaternion parentQ, ref Quaternion q, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetZYRoll(Avatar self, int humanId, ref Vector3 uvw, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetMuscleMinMax(int muscleId, float min, float max);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetParameter(int parameterId, float value);

        public bool isHuman { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isValid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

