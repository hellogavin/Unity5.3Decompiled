namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class AsyncOperation : YieldInstruction
    {
        internal IntPtr m_Ptr;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalDestroy();
        ~AsyncOperation()
        {
            this.InternalDestroy();
        }

        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public float progress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int priority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public bool allowSceneActivation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

