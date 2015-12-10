namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class WaitForSeconds : YieldInstruction
    {
        internal float m_Seconds;
        public WaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds;
        }
    }
}

