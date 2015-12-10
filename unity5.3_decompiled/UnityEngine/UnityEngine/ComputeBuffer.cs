namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;

    public sealed class ComputeBuffer : IDisposable
    {
        internal IntPtr m_Ptr;

        public ComputeBuffer(int count, int stride) : this(count, stride, ComputeBufferType.Default)
        {
        }

        public ComputeBuffer(int count, int stride, ComputeBufferType type)
        {
            this.m_Ptr = IntPtr.Zero;
            InitBuffer(this, count, stride, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void DestroyBuffer(ComputeBuffer buf);
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            DestroyBuffer(this);
            this.m_Ptr = IntPtr.Zero;
        }

        ~ComputeBuffer()
        {
            this.Dispose(false);
        }

        [SecuritySafeCritical]
        public void GetData(Array data)
        {
            this.InternalGetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical, WrapperlessIcall]
        private extern void InternalGetData(Array data, int elemSize);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, SecurityCritical]
        private extern void InternalSetData(Array data, int elemSize);
        public void Release()
        {
            this.Dispose();
        }

        [SecuritySafeCritical]
        public void SetData(Array data)
        {
            this.InternalSetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
        }

        public int count { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int stride { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

