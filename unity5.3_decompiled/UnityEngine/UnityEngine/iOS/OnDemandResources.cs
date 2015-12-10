namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class OnDemandResources
    {
        public static OnDemandResourcesRequest PreloadAsync(string[] tags)
        {
            return PreloadAsyncInternal(tags);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern OnDemandResourcesRequest PreloadAsyncInternal(string[] tags);

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

