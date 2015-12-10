namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public sealed class CreateAssetMenuAttribute : Attribute
    {
        public string fileName { get; set; }

        public string menuName { get; set; }

        public int order { get; set; }
    }
}

