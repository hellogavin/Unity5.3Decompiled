namespace JetBrains.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [MeansImplicitUse]
    public sealed class PublicAPIAttribute : Attribute
    {
        public PublicAPIAttribute()
        {
        }

        public PublicAPIAttribute([NotNull] string comment)
        {
            this.Comment = comment;
        }

        [NotNull]
        public string Comment { get; private set; }
    }
}

