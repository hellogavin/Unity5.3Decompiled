namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public struct jvalue
    {
        [FieldOffset(0)]
        public byte b;
        [FieldOffset(0)]
        public char c;
        [FieldOffset(0)]
        public double d;
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public int i;
        [FieldOffset(0)]
        public long j;
        [FieldOffset(0)]
        public IntPtr l;
        [FieldOffset(0)]
        public short s;
        [FieldOffset(0)]
        public bool z;
    }
}

