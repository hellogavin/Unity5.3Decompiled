namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class AudioLowPassFilter : Behaviour
    {
        public AnimationCurve customCutoffCurve { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("AudioLowPassFilter.lowpassResonaceQ is obsolete. Use lowpassResonanceQ instead (UnityUpgradable) -> lowpassResonanceQ", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float lowpassResonaceQ
        {
            get
            {
                return this.lowpassResonanceQ;
            }
            set
            {
            }
        }

        public float lowpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

