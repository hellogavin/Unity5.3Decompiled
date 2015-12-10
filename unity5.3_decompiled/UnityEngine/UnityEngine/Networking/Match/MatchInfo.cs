namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class MatchInfo
    {
        public MatchInfo(CreateMatchResponse matchResponse)
        {
            this.address = matchResponse.address;
            this.port = matchResponse.port;
            this.networkId = matchResponse.networkId;
            this.accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
            this.nodeId = matchResponse.nodeId;
            this.usingRelay = matchResponse.usingRelay;
        }

        public MatchInfo(JoinMatchResponse matchResponse)
        {
            this.address = matchResponse.address;
            this.port = matchResponse.port;
            this.networkId = matchResponse.networkId;
            this.accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
            this.nodeId = matchResponse.nodeId;
            this.usingRelay = matchResponse.usingRelay;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.networkId, this.address, this.port, this.nodeId, this.usingRelay };
            return UnityString.Format("{0} @ {1}:{2} [{3},{4}]", args);
        }

        public NetworkAccessToken accessToken { get; private set; }

        public string address { get; private set; }

        public NetworkID networkId { get; private set; }

        public NodeID nodeId { get; private set; }

        public int port { get; private set; }

        public bool usingRelay { get; private set; }
    }
}

