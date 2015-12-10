namespace UnityEngineInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MathfInternal
    {
        public static volatile float FloatMinNormal;
        public static volatile float FloatMinDenormal;
        public static bool IsFlushToZeroEnabled;
        static MathfInternal()
        {
            FloatMinNormal = 1.175494E-38f;
            FloatMinDenormal = float.Epsilon;
            IsFlushToZeroEnabled = FloatMinDenormal == 0f;
        }
    }
}

