namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class ProfilerDriver
    {
        public static string directConnectionPort = "54999";

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BeginFrame();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BeginInstrumentFunction(string fullName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CaptureHeapshot();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearAllFrames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DirectIPConnect(string IP);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DirectURLConnect(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void EndFrame();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void EndInstrumentFunction(string fullName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAllStatisticsProperties();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int[] GetAvailableProfilers();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetConnectionIdentifier(int guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetFormattedStatisticsValue(int frame, int identifier);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetGraphStatisticsPropertiesForArea(ProfilerArea area);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNextFrameIndex(int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetOverviewText(ProfilerArea profilerArea, int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPreviousFrameIndex(int frame);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetStatisticsIdentifier(string propertyName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetStatisticsValues(int identifier, int firstFrame, float scale, float[] buffer, out float maxValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsConnectionEditor();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsIdentifierConnectable(int guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsIdentifierOnLocalhost(int guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void QueryFunctionCallees(string fullName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void QueryInstrumentableFunctions();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RequestMemorySnapshot();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RequestObjectMemoryInfo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetHistory();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAutoInstrumentedAssemblies(InstrumentedAssemblyTypes value);

        public static int connectedProfiler { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool deepProfiling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string directConnectionUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int firstFrameIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isGPUProfilerBuggyOnDriver { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isGPUProfilerSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isGPUProfilerSupportedByOS { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int lastFrameIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int maxHistoryLength { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string miniMemoryOverview { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static uint objectCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool profileEditor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool profileGPU { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string selectedPropertyPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static uint usedHeapSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

