namespace UnityEngine.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class UnityAnalyticsManager
    {
        public static string deviceUniqueIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string unityAdsId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool unityAdsTrackingEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

