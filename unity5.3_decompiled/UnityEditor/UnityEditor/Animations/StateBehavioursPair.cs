namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StateBehavioursPair
    {
        public AnimatorState m_State;
        public StateMachineBehaviour[] m_Behaviours;
    }
}

