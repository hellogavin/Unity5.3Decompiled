namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class AudioHighPassFilter : Behaviour
    {
        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("AudioHighPassFilter.highpassResonaceQ is obsolete. Use highpassResonanceQ instead (UnityUpgradable) -> highpassResonanceQ", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float highpassResonaceQ
        {
            get
            {
                return this.highpassResonanceQ;
            }
            set
            {
            }
        }

        public float highpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

