namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ClusterInput
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CheckConnectionToServer(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetAxis(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetButton(string name);
        public static Vector3 GetTrackerPosition(string name)
        {
            Vector3 vector;
            INTERNAL_CALL_GetTrackerPosition(name, out vector);
            return vector;
        }

        public static Quaternion GetTrackerRotation(string name)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetTrackerRotation(name, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetTrackerPosition(string name, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetTrackerRotation(string name, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTrackerPosition(string name, ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTrackerRotation(string name, ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAxis(string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetButton(string name, bool value);
        public static void SetTrackerPosition(string name, Vector3 value)
        {
            INTERNAL_CALL_SetTrackerPosition(name, ref value);
        }

        public static void SetTrackerRotation(string name, Quaternion value)
        {
            INTERNAL_CALL_SetTrackerRotation(name, ref value);
        }
    }
}

