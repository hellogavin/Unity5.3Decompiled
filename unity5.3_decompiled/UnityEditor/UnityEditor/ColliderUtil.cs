namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class ColliderUtil
    {
        public static Matrix4x4 CalculateCapsuleTransform(CapsuleCollider cc)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_CalculateCapsuleTransform(cc, out matrixx);
            return matrixx;
        }

        public static Vector3 GetCapsuleExtents(CapsuleCollider cc)
        {
            Vector3 vector;
            INTERNAL_CALL_GetCapsuleExtents(cc, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_CalculateCapsuleTransform(CapsuleCollider cc, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetCapsuleExtents(CapsuleCollider cc, out Vector3 value);
    }
}

