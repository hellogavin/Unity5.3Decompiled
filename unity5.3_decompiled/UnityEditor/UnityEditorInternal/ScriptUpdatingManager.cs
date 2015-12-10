namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ScriptUpdatingManager
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ReportExpectedUpdateFailure();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ReportGroupedAPIUpdaterFailure(string msg);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool WaitForVCSServerConnection(bool reportTimeout);

        public static int numberOfTimesAsked { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

