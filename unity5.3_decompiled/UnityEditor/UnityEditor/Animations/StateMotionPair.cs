namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StateMotionPair
    {
        public AnimatorState m_State;
        public Motion m_Motion;
    }
}

