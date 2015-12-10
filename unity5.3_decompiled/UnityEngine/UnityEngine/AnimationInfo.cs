namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true), EditorBrowsable(EditorBrowsableState.Never)]
    public struct AnimationInfo
    {
        public AnimationClip clip
        {
            get
            {
                return null;
            }
        }
        public float weight
        {
            get
            {
                return 0f;
            }
        }
    }
}

