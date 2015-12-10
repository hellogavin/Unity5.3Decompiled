namespace JetBrains.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class AssertionMethodAttribute : Attribute
    {
    }
}

