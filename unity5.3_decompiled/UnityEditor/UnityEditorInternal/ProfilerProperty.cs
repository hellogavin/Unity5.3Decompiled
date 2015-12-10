namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ProfilerProperty
    {
        private IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProfilerProperty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Cleanup();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ProfilerProperty()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AudioProfilerInfo[] GetAudioProfilerInfo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetAudioProfilerNameByOffset(int offset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetColumn(ProfilerColumn column);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetTooltip(ProfilerColumn column);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitializeDetailProperty(ProfilerProperty source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Next(bool enterChildren);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetRoot(int frame, ProfilerColumn profilerSortColumn, ProfilerViewType viewType);

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool frameDataReady { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string frameFPS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string frameGpuTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string frameTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool HasChildren { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int[] instanceIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool onlyShowGPUSamples { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string propertyPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

