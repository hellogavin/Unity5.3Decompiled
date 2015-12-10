namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CreateDedicatedMatchRequest : Request
    {
        public override bool IsValid()
        {
            return ((this.matchAttributes != null) ? (this.matchAttributes.Count <= 10) : true);
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

