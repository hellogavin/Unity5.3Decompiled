namespace UnityEngine
{
    using System;

    [Obsolete("iPhoneNetworkReachability enumeration is deprecated. Please use NetworkReachability instead (UnityUpgradable) -> NetworkReachability", true)]
    public enum iPhoneNetworkReachability
    {
        NotReachable = 0,
        ReachableViaCarrierDataNetwork = 1,
        [Obsolete]
        ReachableViaWiFiNetwork = 2
    }
}

