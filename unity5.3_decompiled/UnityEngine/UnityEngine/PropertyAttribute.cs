namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public abstract class PropertyAttribute : Attribute
    {
        protected PropertyAttribute()
        {
        }

        public int order { get; set; }
    }
}

