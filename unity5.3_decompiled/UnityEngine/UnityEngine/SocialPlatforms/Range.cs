namespace UnityEngine.SocialPlatforms
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Range
    {
        public int from;
        public int count;
        public Range(int fromValue, int valueCount)
        {
            this.from = fromValue;
            this.count = valueCount;
        }
    }
}

