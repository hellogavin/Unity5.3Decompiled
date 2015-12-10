namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class)]
    internal class EditorWindowTitleAttribute : Attribute
    {
        public string icon { get; set; }

        public string title { get; set; }

        public bool useTypeNameAsIconName { get; set; }
    }
}

