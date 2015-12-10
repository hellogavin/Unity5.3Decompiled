namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    public sealed class DisallowMultipleComponent : Attribute
    {
    }
}

