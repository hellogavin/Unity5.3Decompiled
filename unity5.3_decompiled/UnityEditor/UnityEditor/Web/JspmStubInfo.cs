namespace UnityEditor.Web
{
    using System;

    internal class JspmStubInfo
    {
        public string[] events;
        public JspmMethodInfo[] methods;
        public JspmPropertyInfo[] properties;

        public JspmStubInfo(JspmPropertyInfo[] properties, JspmMethodInfo[] methods, string[] events)
        {
            this.methods = methods;
            this.properties = properties;
            this.events = events;
        }
    }
}

