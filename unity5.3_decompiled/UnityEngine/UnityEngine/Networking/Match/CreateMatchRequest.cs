namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CreateMatchRequest : Request
    {
        public override bool IsValid()
        {
            return (((!base.IsValid() || (this.size < 2)) || (this.matchAttributes != null)) ? (this.matchAttributes.Count <= 10) : true);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.name, this.size, this.advertise, !(this.password == string.Empty) ? "YES" : "NO", (this.matchAttributes != null) ? this.matchAttributes.Count : 0 };
            return UnityString.Format("[{0}]-name:{1},size:{2},advertise:{3},HasPassword:{4},matchAttributes.Count:{5}", args);
        }

        public bool advertise { get; set; }

        public int eloScore { get; set; }

        public Dictionary<string, long> matchAttributes { get; set; }

        public string name { get; set; }

        public string password { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }

        public uint size { get; set; }
    }
}

