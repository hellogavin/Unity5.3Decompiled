namespace UnityEditor
{
    using System;

    [Serializable]
    internal class ParentViewFolder
    {
        private const string assetsFolder = "Assets";
        public ChangeFlags changeFlags;
        public ParentViewFile[] files;
        public string guid;
        private const string libraryFolder = "Library";
        public string name;
        private const string rootDirText = "/";

        public ParentViewFolder(string name, string guid)
        {
            this.guid = guid;
            this.name = name;
            this.changeFlags = ChangeFlags.None;
            this.files = new ParentViewFile[0];
        }

        public ParentViewFolder(string name, string guid, ChangeFlags flags)
        {
            this.guid = guid;
            this.name = name;
            this.changeFlags = flags;
            this.files = new ParentViewFile[0];
        }

        public ParentViewFolder CloneWithoutFiles()
        {
            return new ParentViewFolder(this.name, this.guid, this.changeFlags);
        }

        public static string MakeNiceName(string name)
        {
            if (name.StartsWith("Assets"))
            {
                if (name != "Assets")
                {
                    name = name.Substring("Assets".Length + 1);
                    return (!(name == string.Empty) ? name : "/");
                }
                return "/";
            }
            if (name.StartsWith("Library"))
            {
                return ("../" + name);
            }
            return (!(name == string.Empty) ? name : "/");
        }
    }
}

