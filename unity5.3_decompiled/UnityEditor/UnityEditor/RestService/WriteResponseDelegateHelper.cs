namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class WriteResponseDelegateHelper
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DoWriteResponse(IntPtr cppResponse, HttpStatusCode resultCode, string payload, IntPtr callbackData);
        internal static WriteResponse MakeDelegateFor(IntPtr response, IntPtr callbackData)
        {
            <MakeDelegateFor>c__AnonStorey29 storey = new <MakeDelegateFor>c__AnonStorey29 {
                response = response,
                callbackData = callbackData
            };
            return new WriteResponse(storey.<>m__3D);
        }

        [CompilerGenerated]
        private sealed class <MakeDelegateFor>c__AnonStorey29
        {
            internal IntPtr callbackData;
            internal IntPtr response;

            internal void <>m__3D(HttpStatusCode resultCode, string payload)
            {
                WriteResponseDelegateHelper.DoWriteResponse(this.response, resultCode, payload, this.callbackData);
            }
        }
    }
}

