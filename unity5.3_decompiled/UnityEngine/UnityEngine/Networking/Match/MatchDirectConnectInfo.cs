namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class MatchDirectConnectInfo : ResponseBase
    {
        public override void Parse(object obj)
        {
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.nodeId = (NodeID) base.ParseJSONUInt16("nodeId", obj, dictJsonObj);
            this.publicAddress = base.ParseJSONString("publicAddress", obj, dictJsonObj);
            this.privateAddress = base.ParseJSONString("privateAddress", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.nodeId, this.publicAddress, this.privateAddress };
            return UnityString.Format("[{0}]-nodeId:{1},publicAddress:{2},privateAddress:{3}", args);
        }

        public NodeID nodeId { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }
    }
}

