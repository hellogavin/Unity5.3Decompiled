namespace UnityEngine
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class AnimationCurve
    {
        internal IntPtr m_Ptr;
        public AnimationCurve(params Keyframe[] keys)
        {
            this.Init(keys);
        }

        [RequiredByNativeCode]
        public AnimationCurve()
        {
            this.Init(null);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Cleanup();
        ~AnimationCurve()
        {
            this.Cleanup();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float Evaluate(float time);
        public Keyframe[] keys
        {
            get
            {
                return this.GetKeys();
            }
            set
            {
                this.SetKeys(value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int AddKey(float time, float value);
        public int AddKey(Keyframe key)
        {
            return this.AddKey_Internal(key);
        }

        private int AddKey_Internal(Keyframe key)
        {
            return INTERNAL_CALL_AddKey_Internal(this, ref key);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_AddKey_Internal(AnimationCurve self, ref Keyframe key);
        public int MoveKey(int index, Keyframe key)
        {
            return INTERNAL_CALL_MoveKey(this, index, ref key);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_MoveKey(AnimationCurve self, int index, ref Keyframe key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RemoveKey(int index);
        public Keyframe this[int index]
        {
            get
            {
                return this.GetKey_Internal(index);
            }
        }
        public int length { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetKeys(Keyframe[] keys);
        private Keyframe GetKey_Internal(int index)
        {
            Keyframe keyframe;
            INTERNAL_CALL_GetKey_Internal(this, index, out keyframe);
            return keyframe;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetKey_Internal(AnimationCurve self, int index, out Keyframe value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Keyframe[] GetKeys();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SmoothTangents(int index, float weight);
        public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
        {
            float outTangent = (valueEnd - valueStart) / (timeEnd - timeStart);
            Keyframe[] keys = new Keyframe[] { new Keyframe(timeStart, valueStart, 0f, outTangent), new Keyframe(timeEnd, valueEnd, outTangent, 0f) };
            return new AnimationCurve(keys);
        }

        public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(timeStart, valueStart, 0f, 0f), new Keyframe(timeEnd, valueEnd, 0f, 0f) };
            return new AnimationCurve(keys);
        }

        public WrapMode preWrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public WrapMode postWrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init(Keyframe[] keys);
    }
}

