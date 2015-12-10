namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Message
    {
        private IntPtr m_thisDummy;

        internal Message()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~Message()
        {
            this.Dispose();
        }

        private static void Info(string message)
        {
            Debug.Log("Version control:\n" + message);
        }

        public void Show()
        {
            Info(this.message);
        }

        public string message { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Severity severity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Flags]
        public enum Severity
        {
            Data,
            Verbose,
            Info,
            Warning,
            Error
        }
    }
}

