namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode, Obsolete("this API is not for public use.")]
    public struct CacheIndex
    {
        public string name;
        public int bytesUsed;
        public int expires;
    }
}

