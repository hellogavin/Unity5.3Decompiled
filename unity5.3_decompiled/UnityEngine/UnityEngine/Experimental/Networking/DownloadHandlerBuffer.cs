namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerBuffer : DownloadHandler
    {
        public DownloadHandlerBuffer()
        {
            base.InternalCreateString();
        }

        protected override byte[] GetData()
        {
            return this.InternalGetData();
        }

        protected override string GetText()
        {
            return this.InternalGetText();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern byte[] InternalGetData();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern string InternalGetText();
        public static string GetContent(UnityWebRequest www)
        {
            return DownloadHandler.GetCheckedDownloader<DownloadHandlerBuffer>(www).text;
        }
    }
}

