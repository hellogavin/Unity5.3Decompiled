namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ClusterNetwork
    {
        public static bool isDisconnected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isMasterOfCluster { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int nodeIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

