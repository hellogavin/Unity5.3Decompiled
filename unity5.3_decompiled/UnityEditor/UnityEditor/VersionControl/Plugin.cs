namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Plugin
    {
        private string m_guid;
        private IntPtr m_thisDummy;

        internal Plugin()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();

        public static Plugin[] availablePlugins { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ConfigField[] configFields { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

