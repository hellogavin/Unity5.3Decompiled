namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class JoinMatchRequest : Request
    {
        public override bool IsValid()
        {
            return (base.IsValid() && (this.networkId != NetworkID.Invalid));
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X"), !(this.password == string.Empty) ? "YES" : "NO" };
            return UnityString.Format("[{0}]-networkId:0x{1},HasPassword:{2}", args);
        }

        public int eloScore { get; set; }

        public NetworkID networkId { get; set; }

        public string password { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }
    }
}

