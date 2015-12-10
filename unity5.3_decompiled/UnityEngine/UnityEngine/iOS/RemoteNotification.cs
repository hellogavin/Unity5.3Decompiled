namespace UnityEngine.iOS
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class RemoteNotification
    {
        private IntPtr notificationWrapper;

        private RemoteNotification()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Destroy();
        ~RemoteNotification()
        {
            this.Destroy();
        }

        public string alertBody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int applicationIconBadgeNumber { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasAction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string soundName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public IDictionary userInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

