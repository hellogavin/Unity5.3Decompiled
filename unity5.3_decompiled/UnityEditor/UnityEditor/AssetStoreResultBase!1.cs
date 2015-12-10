namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal abstract class AssetStoreResultBase<Derived> where Derived: class
    {
        private Callback<Derived> callback;
        public string error;
        public string warnings;

        public AssetStoreResultBase(Callback<Derived> cb)
        {
            this.callback = cb;
            this.warnings = string.Empty;
        }

        protected abstract void Parse(Dictionary<string, JSONValue> dict);
        public void Parse(AssetStoreResponse response)
        {
            if (response.job.IsSuccess())
            {
                if (response.job.responseCode >= 300)
                {
                    this.error = string.Format("HTTP status code {0}", response.job.responseCode);
                }
                else if (response.dict.ContainsKey("error"))
                {
                    this.error = response.dict["error"].AsString(true);
                }
                else
                {
                    this.Parse(response.dict);
                }
            }
            else
            {
                string str = (response.job != null) ? ((response.job.url != null) ? response.job.url : "null") : "nulljob";
                if (response.job.text == null)
                {
                }
                this.error = "Error receiving response from server on url '" + str + "': " + "n/a";
            }
            this.callback(((AssetStoreResultBase<Derived>) this) as Derived);
        }

        public delegate void Callback(Derived res);
    }
}

