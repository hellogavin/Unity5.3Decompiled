namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimatorClipInfo
    {
        private int m_ClipInstanceID;
        private float m_Weight;
        public AnimationClip clip
        {
            get
            {
                return ((this.m_ClipInstanceID == 0) ? null : ClipInstanceToScriptingObject(this.m_ClipInstanceID));
            }
        }
        public float weight
        {
            get
            {
                return this.m_Weight;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimationClip ClipInstanceToScriptingObject(int instanceID);
    }
}

