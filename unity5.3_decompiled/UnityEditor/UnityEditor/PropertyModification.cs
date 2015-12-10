namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class PropertyModification
    {
        public Object target;
        public string propertyPath;
        public string value;
        public Object objectReference;
    }
}

