namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    internal class AssetStoreResponse
    {
        public Dictionary<string, JSONValue> dict;
        internal AsyncHTTPClient job;
        public bool ok;

        private static string EncodeString(string str)
        {
            str = str.Replace("\"", "\\\"");
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\b", @"\b");
            str = str.Replace("\f", @"\f");
            str = str.Replace("\n", @"\n");
            str = str.Replace("\r", @"\r");
            str = str.Replace("\t", @"\t");
            return str;
        }

        public override string ToString()
        {
            string str = "{";
            string str2 = string.Empty;
            foreach (KeyValuePair<string, JSONValue> pair in this.dict)
            {
                string str3 = str;
                object[] objArray1 = new object[] { str3, str2, '"', EncodeString(pair.Key), "\" : ", pair.Value.ToString() };
                str = string.Concat(objArray1);
                str2 = ", ";
            }
            return (str + "}");
        }

        public bool failed
        {
            get
            {
                return !this.ok;
            }
        }

        public string message
        {
            get
            {
                if ((this.dict == null) || !this.dict.ContainsKey("message"))
                {
                    return null;
                }
                JSONValue value2 = this.dict["message"];
                return value2.AsString(true);
            }
        }
    }
}

