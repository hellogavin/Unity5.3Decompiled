namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class FrameDebuggerUtility
    {
        private static FrameDebuggerEventData GetFrameEventData()
        {
            FrameDebuggerEventData data;
            INTERNAL_CALL_GetFrameEventData(out data);
            return data;
        }

        public static bool GetFrameEventData(int index, out FrameDebuggerEventData frameDebuggerEventData)
        {
            frameDebuggerEventData = GetFrameEventData();
            return (frameDebuggerEventData.frameEventIndex == index);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject GetFrameEventGameObject(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetFrameEventInfoName(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern FrameDebuggerEvent[] GetFrameEvents();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetRemotePlayerGUID();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetFrameEventData(out FrameDebuggerEventData value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetRenderTargetDisplayOptions(int rtIndex, ref Vector4 channels, float blackLevel, float whiteLevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsLocalEnabled();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsRemoteEnabled();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetEnabled(bool enabled, int remotePlayerGUID);
        public static void SetRenderTargetDisplayOptions(int rtIndex, Vector4 channels, float blackLevel, float whiteLevel)
        {
            INTERNAL_CALL_SetRenderTargetDisplayOptions(rtIndex, ref channels, blackLevel, whiteLevel);
        }

        public static int count { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int eventsHash { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int limit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool locallySupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool receivingRemoteFrameEventData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

