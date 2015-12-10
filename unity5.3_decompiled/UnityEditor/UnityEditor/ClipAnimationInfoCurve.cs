namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct ClipAnimationInfoCurve
    {
        public string name;
        public AnimationCurve curve;
    }
}

