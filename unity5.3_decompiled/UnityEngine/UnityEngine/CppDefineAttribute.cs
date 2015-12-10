namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=true)]
    internal class CppDefineAttribute : Attribute
    {
        public CppDefineAttribute(string symbol, string value)
        {
        }
    }
}

