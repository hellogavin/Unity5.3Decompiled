namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class MatchDesc : ResponseBase
    {
        public override void Parse(object obj)
        {
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.networkId = (NetworkID) base.ParseJSONUInt64("networkId", obj, dictJsonObj);
            this.name = base.ParseJSONString("name", obj, dictJsonObj);
            this.maxSize = base.ParseJSONInt32("maxSize", obj, dictJsonObj);
            this.currentSize = base.ParseJSONInt32("currentSize", obj, dictJsonObj);
            this.isPrivate = base.ParseJSONBool("isPrivate", obj, dictJsonObj);
            this.directConnectInfos = base.ParseJSONList<MatchDirectConnectInfo>("directConnectInfos", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X"), this.name, this.averageEloScore, this.maxSize, this.currentSize, this.isPrivate, (this.matchAttributes != null) ? this.matchAttributes.Count : 0, this.directConnectInfos.Count };
            return UnityString.Format("[{0}]-networkId:0x{1},name:{2},averageEloScore:{3},maxSize:{4},currentSize:{5},isPrivate:{6},matchAttributes.Count:{7},directConnectInfos.Count:{8}", args);
        }

        public int averageEloScore { get; set; }

        public int currentSize { get; set; }

        public List<MatchDirectConnectInfo> directConnectInfos { get; set; }

        public NodeID hostNodeId { get; set; }

        public bool isPrivate { get; set; }

        public Dictionary<string, long> matchAttributes { get; set; }

        public int maxSize { get; set; }

        public string name { get; set; }

        public NetworkID networkId { get; set; }
    }
}

