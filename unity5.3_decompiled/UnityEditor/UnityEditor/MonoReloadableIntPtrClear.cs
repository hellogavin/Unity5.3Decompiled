namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct MonoReloadableIntPtrClear
    {
        internal IntPtr m_IntPtr;
    }
}

