namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=false)]
    internal class CppBodyAttribute : Attribute
    {
        public CppBodyAttribute(string body)
        {
        }
    }
}

