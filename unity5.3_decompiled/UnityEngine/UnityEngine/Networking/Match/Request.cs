namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public abstract class Request
    {
        public int version = 2;

        protected Request()
        {
        }

        public virtual bool IsValid()
        {
            return ((this.appId != AppID.Invalid) && (this.sourceId != SourceID.Invalid));
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.sourceId.ToString("X"), this.appId.ToString("X"), this.domain };
            return UnityString.Format("[{0}]-SourceID:0x{1},AppID:0x{2},domain:{3}", args);
        }

        public string accessTokenString { get; set; }

        public AppID appId { get; set; }

        public int domain { get; set; }

        public string projectId { get; set; }

        public SourceID sourceId { get; set; }
    }
}

