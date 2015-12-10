namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Networking.Types;

    public class StopDedicatedMatchRequest : Request
    {
        public NetworkID networkId { get; set; }
    }
}

