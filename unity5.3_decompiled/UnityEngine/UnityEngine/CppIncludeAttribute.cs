namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=true)]
    internal class CppIncludeAttribute : Attribute
    {
        public CppIncludeAttribute(string header)
        {
        }
    }
}

