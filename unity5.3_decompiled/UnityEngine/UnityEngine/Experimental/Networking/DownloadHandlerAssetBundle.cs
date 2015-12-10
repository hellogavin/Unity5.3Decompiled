namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerAssetBundle : DownloadHandler
    {
        public DownloadHandlerAssetBundle(string url, uint crc)
        {
            base.InternalCreateWebStream(url, crc);
        }

        public DownloadHandlerAssetBundle(string url, uint version, uint crc)
        {
            Hash128 hash = new Hash128(0, 0, 0, version);
            base.InternalCreateWebStream(url, hash, crc);
        }

        public DownloadHandlerAssetBundle(string url, Hash128 hash, uint crc)
        {
            base.InternalCreateWebStream(url, hash, crc);
        }

        protected override byte[] GetData()
        {
            throw new NotSupportedException("Raw data access is not supported for asset bundles");
        }

        protected override string GetText()
        {
            throw new NotSupportedException("String access is not supported for asset bundles");
        }

        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public static AssetBundle GetContent(UnityWebRequest www)
        {
            return DownloadHandler.GetCheckedDownloader<DownloadHandlerAssetBundle>(www).assetBundle;
        }
    }
}

