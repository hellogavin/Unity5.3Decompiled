namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class InputTracking
    {
        public static Vector3 GetLocalPosition(VRNode node)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalPosition(node, out vector);
            return vector;
        }

        public static Quaternion GetLocalRotation(VRNode node)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetLocalRotation(node, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Recenter();
    }
}

