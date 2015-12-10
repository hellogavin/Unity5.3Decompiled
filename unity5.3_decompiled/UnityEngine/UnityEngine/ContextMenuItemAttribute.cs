namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=true)]
    public class ContextMenuItemAttribute : PropertyAttribute
    {
        public readonly string function;
        public readonly string name;

        public ContextMenuItemAttribute(string name, string function)
        {
            this.name = name;
            this.function = function;
        }
    }
}

