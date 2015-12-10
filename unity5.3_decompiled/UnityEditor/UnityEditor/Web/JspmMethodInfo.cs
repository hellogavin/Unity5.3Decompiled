namespace UnityEditor.Web
{
    using System;

    internal class JspmMethodInfo
    {
        public string name;
        public string[] parameters;

        public JspmMethodInfo(string name, string[] parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }
    }
}

