namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct ChildAnimatorState
    {
        private AnimatorState m_State;
        private Vector3 m_Position;
        public AnimatorState state
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
            }
        }
        public Vector3 position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                this.m_Position = value;
            }
        }
    }
}

