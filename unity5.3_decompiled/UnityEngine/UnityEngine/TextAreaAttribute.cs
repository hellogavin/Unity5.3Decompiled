namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class TextAreaAttribute : PropertyAttribute
    {
        public readonly int maxLines;
        public readonly int minLines;

        public TextAreaAttribute()
        {
            this.minLines = 3;
            this.maxLines = 3;
        }

        public TextAreaAttribute(int minLines, int maxLines)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }
    }
}

