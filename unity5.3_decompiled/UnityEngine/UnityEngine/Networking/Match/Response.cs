namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class Response : ResponseBase, IResponse
    {
        protected Response()
        {
        }

        public override void Parse(object obj)
        {
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj != null)
            {
                this.success = base.ParseJSONBool("success", obj, dictJsonObj);
                this.extendedInfo = base.ParseJSONString("extendedInfo", obj, dictJsonObj);
                if (!this.success)
                {
                    throw new FormatException("FAILURE Returned from server: " + this.extendedInfo);
                }
            }
        }

        public void SetFailure(string info)
        {
            this.success = false;
            this.extendedInfo = info;
        }

        public void SetSuccess()
        {
            this.success = true;
            this.extendedInfo = string.Empty;
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.success, this.extendedInfo };
            return UnityString.Format("[{0}]-success:{1}-extendedInfo:{2}", args);
        }

        public string extendedInfo { get; private set; }

        public bool success { get; private set; }
    }
}

