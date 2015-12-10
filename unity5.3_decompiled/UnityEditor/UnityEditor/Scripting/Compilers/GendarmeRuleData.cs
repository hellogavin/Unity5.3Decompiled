namespace UnityEditor.Scripting.Compilers
{
    using System;

    internal class GendarmeRuleData
    {
        public string Details;
        public string File = string.Empty;
        public bool IsAssemblyError;
        public int LastIndex;
        public int Line;
        public string Location;
        public string Problem;
        public string Severity;
        public string Source;
        public string Target;
    }
}

