namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ProfilerFrameDataIterator
    {
        private IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProfilerFrameDataIterator();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ProfilerFrameDataIterator()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern double GetFrameStartS(int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetGroupCount(int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetGroupName();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetThreadCount(int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetThreadName();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Next(bool enterChildren);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetRoot(int frame, int threadIdx);

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float durationMS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float frameTimeMS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int group { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int id { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int instanceId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string path { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float startTimeMS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

