namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public class DownloadHandlerScript : DownloadHandler
    {
        public DownloadHandlerScript()
        {
            base.InternalCreateScript();
        }

        public DownloadHandlerScript(byte[] preallocatedBuffer)
        {
            if ((preallocatedBuffer == null) || (preallocatedBuffer.Length < 1))
            {
                throw new ArgumentException("Cannot create a preallocated-buffer DownloadHandlerScript backed by a null or zero-length array");
            }
            base.InternalCreateScript();
            this.InternalSetPreallocatedBuffer(preallocatedBuffer);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalSetPreallocatedBuffer(byte[] buffer);
    }
}

