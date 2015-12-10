namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=false)]
    public class ConstructorSafeAttribute : Attribute
    {
    }
}

