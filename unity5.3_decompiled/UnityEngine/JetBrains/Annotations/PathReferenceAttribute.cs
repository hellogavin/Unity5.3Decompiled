namespace JetBrains.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathReferenceAttribute : Attribute
    {
        public PathReferenceAttribute()
        {
        }

        public PathReferenceAttribute([PathReference] string basePath)
        {
            this.BasePath = basePath;
        }

        [NotNull]
        public string BasePath { get; private set; }
    }
}

