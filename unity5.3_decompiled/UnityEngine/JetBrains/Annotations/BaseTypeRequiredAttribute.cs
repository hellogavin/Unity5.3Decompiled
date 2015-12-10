namespace JetBrains.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true), BaseTypeRequired(typeof(Attribute))]
    public sealed class BaseTypeRequiredAttribute : Attribute
    {
        public BaseTypeRequiredAttribute([NotNull] Type baseType)
        {
            this.BaseType = baseType;
        }

        [NotNull]
        public Type BaseType { get; private set; }
    }
}

