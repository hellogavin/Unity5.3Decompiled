namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal sealed class Changeset
    {
        public int changeset;
        public string message;
        public string date;
        public string owner;
        public ChangesetItem[] items;
    }
}

