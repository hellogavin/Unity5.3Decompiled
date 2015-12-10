namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal sealed class MaintDatabaseRecord
    {
        public string name;
        public string dbName;
    }
}

