namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    public class ThreadSafeAttribute : Attribute
    {
    }
}

