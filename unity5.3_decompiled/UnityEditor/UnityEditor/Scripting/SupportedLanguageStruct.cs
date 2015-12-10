namespace UnityEditor.Scripting
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SupportedLanguageStruct
    {
        public string extension;
        public string languageName;
    }
}

