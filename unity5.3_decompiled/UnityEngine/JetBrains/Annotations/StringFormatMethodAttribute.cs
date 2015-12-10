namespace JetBrains.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=false, Inherited=true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        public StringFormatMethodAttribute(string formatParameterName)
        {
            this.FormatParameterName = formatParameterName;
        }

        public string FormatParameterName { get; private set; }
    }
}

