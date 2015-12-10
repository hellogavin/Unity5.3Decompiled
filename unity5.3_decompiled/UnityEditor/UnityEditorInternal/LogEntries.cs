namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class LogEntries
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClickStatusBar(int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void EndGettingEntries();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetCountsByType(ref int errorCount, ref int warningCount, ref int logCount);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetEntryCount(int row);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetEntryInternal(int row, LogEntry outputEntry);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetFirstTwoLinesEntryTextAndModeInternal(int row, ref int mask, ref string outString);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetStatusMask();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetStatusText();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetStatusViewErrorIndex();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RowGotDoubleClicked(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetConsoleFlag(int bit, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int StartGettingEntries();

        public static int consoleFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

