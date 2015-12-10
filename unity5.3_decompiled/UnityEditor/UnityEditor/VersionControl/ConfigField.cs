namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ConfigField
    {
        private string m_guid;
        private IntPtr m_thisDummy;

        internal ConfigField()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ConfigField()
        {
            this.Dispose();
        }

        public string description { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isRequired { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string label { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

