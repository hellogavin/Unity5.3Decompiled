namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class DestroyMatchRequest : Request
    {
        public override bool IsValid()
        {
            return (base.IsValid() && (this.networkId != NetworkID.Invalid));
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X") };
            return UnityString.Format("[{0}]-networkId:0x{1}", args);
        }

        public NetworkID networkId { get; set; }
    }
}

