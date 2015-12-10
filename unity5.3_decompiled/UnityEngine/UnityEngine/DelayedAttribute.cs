namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class DelayedAttribute : PropertyAttribute
    {
    }
}

