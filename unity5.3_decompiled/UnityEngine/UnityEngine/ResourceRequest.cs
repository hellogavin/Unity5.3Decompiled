namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class ResourceRequest : AsyncOperation
    {
        internal string m_Path;
        internal Type m_Type;
        public Object asset
        {
            get
            {
                return Resources.Load(this.m_Path, this.m_Type);
            }
        }
    }
}

