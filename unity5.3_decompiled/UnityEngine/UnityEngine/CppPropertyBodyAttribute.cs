namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    internal class CppPropertyBodyAttribute : Attribute
    {
        public CppPropertyBodyAttribute(string getterBody)
        {
        }

        public CppPropertyBodyAttribute(string getterBody, string setterBody)
        {
        }
    }
}

