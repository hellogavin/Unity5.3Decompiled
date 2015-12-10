namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Device
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetAdvertisingIdentifier();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetNoBackupFlag(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetNoBackupFlag(string path);

        public static string advertisingIdentifier
        {
            get
            {
                string advertisingIdentifier = GetAdvertisingIdentifier();
                Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, advertisingTrackingEnabled);
                return advertisingIdentifier;
            }
        }

        public static bool advertisingTrackingEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static DeviceGeneration generation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string systemVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string vendorIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

