namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class UploadHandlerRaw : UploadHandler
    {
        public UploadHandlerRaw(byte[] data)
        {
            base.InternalCreateRaw(data);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern string InternalGetContentType();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalSetContentType(string newContentType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern byte[] InternalGetData();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern float InternalGetProgress();
        internal override string GetContentType()
        {
            return this.InternalGetContentType();
        }

        internal override void SetContentType(string newContentType)
        {
            this.InternalSetContentType(newContentType);
        }

        internal override byte[] GetData()
        {
            return this.InternalGetData();
        }

        internal override float GetProgress()
        {
            return this.InternalGetProgress();
        }
    }
}

