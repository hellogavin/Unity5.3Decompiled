namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class Microphone
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void End(string deviceName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPosition(string deviceName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsRecording(string deviceName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency);

        public static string[] devices { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

