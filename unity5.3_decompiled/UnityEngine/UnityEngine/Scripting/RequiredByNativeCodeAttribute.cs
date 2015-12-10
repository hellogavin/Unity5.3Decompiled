namespace UnityEngine.Scripting
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    internal class RequiredByNativeCodeAttribute : Attribute
    {
    }
}

