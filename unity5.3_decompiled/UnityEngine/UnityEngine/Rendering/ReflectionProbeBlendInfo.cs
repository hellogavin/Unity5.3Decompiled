namespace UnityEngine.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ReflectionProbeBlendInfo
    {
        public ReflectionProbe probe;
        public float weight;
    }
}

