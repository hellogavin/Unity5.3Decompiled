namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public class UploadHandler : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        internal UploadHandler()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateRaw(byte[] data);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalDestroy();
        ~UploadHandler()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        public byte[] data
        {
            get
            {
                return this.GetData();
            }
        }
        public string contentType
        {
            get
            {
                return this.GetContentType();
            }
            set
            {
                this.SetContentType(value);
            }
        }
        public float progress
        {
            get
            {
                return this.GetProgress();
            }
        }
        internal virtual byte[] GetData()
        {
            return null;
        }

        internal virtual string GetContentType()
        {
            return "text/plain";
        }

        internal virtual void SetContentType(string newContentType)
        {
        }

        internal virtual float GetProgress()
        {
            return 0.5f;
        }
    }
}

