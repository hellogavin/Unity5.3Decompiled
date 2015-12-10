namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QualityCurvesTime
    {
        public float fixedTime;
        public float variableEndStart;
        public float variableEndEnd;
        public int q;
    }
}

