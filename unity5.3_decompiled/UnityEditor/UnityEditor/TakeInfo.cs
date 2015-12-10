namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TakeInfo
    {
        public string name;
        public string defaultClipName;
        public float startTime;
        public float stopTime;
        public float bakeStartTime;
        public float bakeStopTime;
        public float sampleRate;
    }
}

