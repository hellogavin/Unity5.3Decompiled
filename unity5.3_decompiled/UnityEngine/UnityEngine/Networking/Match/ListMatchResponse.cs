namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ListMatchResponse : BasicResponse
    {
        public ListMatchResponse()
        {
        }

        public ListMatchResponse(List<MatchDesc> otherMatches)
        {
            this.matches = otherMatches;
        }

        public override void Parse(object obj)
        {
            base.Parse(obj);
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.matches = base.ParseJSONList<MatchDesc>("matches", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.matches.Count };
            return UnityString.Format("[{0}]-matches.Count:{1}", args);
        }

        public List<MatchDesc> matches { get; set; }
    }
}

