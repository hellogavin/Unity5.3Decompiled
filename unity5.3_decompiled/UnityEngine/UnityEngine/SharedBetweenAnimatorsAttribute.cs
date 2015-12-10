namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false), RequiredByNativeCode]
    public sealed class SharedBetweenAnimatorsAttribute : Attribute
    {
    }
}

