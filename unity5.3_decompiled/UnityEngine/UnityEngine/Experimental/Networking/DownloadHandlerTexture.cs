namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerTexture : DownloadHandler
    {
        public DownloadHandlerTexture()
        {
            base.InternalCreateTexture(true);
        }

        public DownloadHandlerTexture(bool readable)
        {
            base.InternalCreateTexture(readable);
        }

        protected override byte[] GetData()
        {
            return this.InternalGetData();
        }

        public Texture2D texture
        {
            get
            {
                return this.InternalGetTexture();
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Texture2D InternalGetTexture();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern byte[] InternalGetData();
        public static Texture2D GetContent(UnityWebRequest www)
        {
            return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
        }
    }
}

