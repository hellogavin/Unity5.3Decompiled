namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class AnimationClip : Motion
    {
        public AnimationClip()
        {
            Internal_CreateAnimationClip(this);
        }

        public void AddEvent(AnimationEvent evt)
        {
            this.AddEventInternal(evt);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddEventInternal(object evt);
        public void ClearCurves()
        {
            INTERNAL_CALL_ClearCurves(this);
        }

        public void EnsureQuaternionContinuity()
        {
            INTERNAL_CALL_EnsureQuaternionContinuity(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Array GetEventsInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ClearCurves(AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_EnsureQuaternionContinuity(AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localBounds(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SampleAnimation(GameObject go, float time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCurve(string relativePath, Type type, string propertyName, AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetEventsInternal(Array value);

        public AnimationEvent[] events
        {
            get
            {
                return (AnimationEvent[]) this.GetEventsInternal();
            }
            set
            {
                this.SetEventsInternal(value);
            }
        }

        public float frameRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool humanMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool legacy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float length { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Bounds localBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_localBounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_localBounds(ref value);
            }
        }

        internal float startTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal float stopTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public WrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

