namespace UnityEditor
{
    using System;

    [Serializable]
    internal class ParentViewFile
    {
        public ChangeFlags changeFlags;
        public string guid;
        public string name;

        public ParentViewFile(string name, string guid)
        {
            this.guid = guid;
            this.name = name;
            this.changeFlags = ChangeFlags.None;
        }

        public ParentViewFile(string name, string guid, ChangeFlags flags)
        {
            this.guid = guid;
            this.name = name;
            this.changeFlags = flags;
        }
    }
}

