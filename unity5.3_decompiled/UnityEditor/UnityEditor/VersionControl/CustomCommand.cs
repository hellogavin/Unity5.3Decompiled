namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class CustomCommand
    {
        private IntPtr m_thisDummy;

        internal CustomCommand()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Task StartTask();

        public CommandContext context { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string label { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

