namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public class UnityAPICompatibilityVersionAttribute : Attribute
    {
        private string _version;

        public UnityAPICompatibilityVersionAttribute(string version)
        {
            this._version = version;
        }

        public string version
        {
            get
            {
                return this._version;
            }
        }
    }
}

