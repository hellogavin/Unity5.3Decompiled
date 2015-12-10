namespace UnityEngine
{
    using System;

    public sealed class GUILayoutOption
    {
        internal Type type;
        internal object value;

        internal GUILayoutOption(Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        internal enum Type
        {
            fixedWidth,
            fixedHeight,
            minWidth,
            maxWidth,
            minHeight,
            maxHeight,
            stretchWidth,
            stretchHeight,
            alignStart,
            alignMiddle,
            alignEnd,
            alignJustify,
            equalSize,
            spacing
        }
    }
}

