namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ChangeSet
    {
        public static string defaultID = "-1";
        private IntPtr m_thisDummy;

        public ChangeSet()
        {
            this.InternalCreate();
        }

        public ChangeSet(string description)
        {
            this.InternalCreateFromString(description);
        }

        public ChangeSet(ChangeSet other)
        {
            this.InternalCopyConstruct(other);
        }

        public ChangeSet(string description, string revision)
        {
            this.InternalCreateFromStringString(description, revision);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ChangeSet()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCopyConstruct(ChangeSet other);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCreateFromString(string description);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCreateFromStringString(string description, string changeSetID);

        public string description { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string id { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

