namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    internal class CppPropertyAttribute : Attribute
    {
        public CppPropertyAttribute(string getter)
        {
        }

        public CppPropertyAttribute(string getter, string setter)
        {
        }
    }
}

