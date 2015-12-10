namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine.Networking.Types;

    public class CreateDedicatedMatchResponse : BasicResponse
    {
        public override void Parse(object obj)
        {
            base.Parse(obj);
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.address = base.ParseJSONString("address", obj, dictJsonObj);
            this.port = base.ParseJSONInt32("port", obj, dictJsonObj);
            this.accessTokenString = base.ParseJSONString("accessTokenString", obj, dictJsonObj);
            this.networkId = (NetworkID) base.ParseJSONUInt64("networkId", obj, dictJsonObj);
            this.nodeId = (NodeID) base.ParseJSONUInt16("nodeId", obj, dictJsonObj);
        }

        public string accessTokenString { get; set; }

        public string address { get; set; }

        public NetworkID networkId { get; set; }

        public NodeID nodeId { get; set; }

        public int port { get; set; }
    }
}

