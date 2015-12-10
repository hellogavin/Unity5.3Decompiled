namespace UnityEditor
{
    using System;

    [Serializable]
    internal class ParentViewState
    {
        public ParentViewFolder[] folders = new ParentViewFolder[0];
        public int initialSelectedItem = -1;
        public ListViewState lv;
        public int selectedFile = -1;
        public int selectedFolder = -1;
        public bool[] selectedItems;

        private void AddAssetItem(string guid, string pathName, bool isDir, ChangeFlags changeFlags, int changeset)
        {
            if (pathName != string.Empty)
            {
                if (isDir)
                {
                    string lfname = ParentViewFolder.MakeNiceName(pathName);
                    int index = IndexOf(this.folders, lfname);
                    if (index == -1)
                    {
                        ParentViewFolder item = new ParentViewFolder(lfname, guid, changeFlags);
                        ArrayUtility.Add<ParentViewFolder>(ref this.folders, item);
                    }
                    else
                    {
                        this.folders[index].changeFlags = changeFlags;
                        this.folders[index].guid = guid;
                    }
                }
                else
                {
                    ParentViewFolder folder2;
                    string str2 = ParentViewFolder.MakeNiceName(FileUtil.DeleteLastPathNameComponent(pathName));
                    string str3 = pathName.Substring(pathName.LastIndexOf("/") + 1);
                    int num2 = IndexOf(this.folders, str2);
                    if (num2 == -1)
                    {
                        folder2 = new ParentViewFolder(str2, AssetServer.GetParentGUID(guid, changeset));
                        ArrayUtility.Add<ParentViewFolder>(ref this.folders, folder2);
                    }
                    else
                    {
                        folder2 = this.folders[num2];
                    }
                    num2 = IndexOf(folder2.files, str3);
                    if (num2 != -1)
                    {
                        if ((folder2.files[num2].changeFlags & ChangeFlags.Deleted) == ChangeFlags.None)
                        {
                            folder2.files[num2].guid = guid;
                            folder2.files[num2].changeFlags = changeFlags;
                        }
                    }
                    else
                    {
                        ArrayUtility.Add<ParentViewFile>(ref folder2.files, new ParentViewFile(str3, guid, changeFlags));
                    }
                }
            }
        }

        public void AddAssetItems(AssetsItem[] assets)
        {
            foreach (AssetsItem item in assets)
            {
                this.AddAssetItem(item.guid, item.pathName, item.assetIsDir != 0, (ChangeFlags) item.changeFlags, -1);
            }
            Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
            for (int i = 0; i < this.folders.Length; i++)
            {
                Array.Sort<ParentViewFile>(this.folders[i].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
            }
        }

        public void AddAssetItems(Changeset assets)
        {
            foreach (ChangesetItem item in assets.items)
            {
                this.AddAssetItem(item.guid, item.fullPath, item.assetIsDir != 0, item.changeFlags, assets.changeset);
            }
            Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
            for (int i = 0; i < this.folders.Length; i++)
            {
                Array.Sort<ParentViewFile>(this.folders[i].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
            }
        }

        public void AddAssetItems(DeletedAsset[] assets)
        {
            foreach (DeletedAsset asset in assets)
            {
                this.AddAssetItem(asset.guid, asset.fullPath, asset.assetIsDir != 0, ChangeFlags.None, -1);
            }
            Array.Sort<ParentViewFolder>(this.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
            for (int i = 0; i < this.folders.Length; i++)
            {
                Array.Sort<ParentViewFile>(this.folders[i].files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
            }
        }

        public void Clear()
        {
            this.folders = new ParentViewFolder[0];
            this.selectedFolder = -1;
            this.selectedFile = -1;
            this.initialSelectedItem = -1;
        }

        public void ClearSelection()
        {
            for (int i = 0; i < this.selectedItems.Length; i++)
            {
                this.selectedItems[i] = false;
            }
            this.initialSelectedItem = -1;
        }

        internal static int CompareViewFile(ParentViewFile p1, ParentViewFile p2)
        {
            return string.Compare(p1.name, p2.name, true);
        }

        internal static int CompareViewFolder(ParentViewFolder p1, ParentViewFolder p2)
        {
            return string.Compare(p1.name, p2.name, true);
        }

        public int GetFoldersCount()
        {
            return this.folders.Length;
        }

        public int GetLineCount()
        {
            int num = 0;
            for (int i = 0; i < this.folders.Length; i++)
            {
                num += this.folders[i].files.Length + 1;
            }
            return num;
        }

        public bool HasTrue()
        {
            for (int i = 0; i < this.selectedItems.Length; i++)
            {
                if (this.selectedItems[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal static int IndexOf(ParentViewFile[] filesFrom, string lfname)
        {
            for (int i = 0; i < filesFrom.Length; i++)
            {
                if (string.Compare(filesFrom[i].name, lfname, true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static int IndexOf(ParentViewFolder[] foldersFrom, string lfname)
        {
            for (int i = 0; i < foldersFrom.Length; i++)
            {
                if (string.Compare(foldersFrom[i].name, lfname, true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IndexToFolderAndFile(int index, ref int folder, ref int file)
        {
            folder = 0;
            file = -1;
            for (int i = 0; i < index; i++)
            {
                if (!this.NextFileFolder(ref folder, ref file))
                {
                    return false;
                }
            }
            return true;
        }

        public bool NextFileFolder(ref int folder, ref int file)
        {
            if (folder >= this.folders.Length)
            {
                return false;
            }
            ParentViewFolder folder2 = this.folders[folder];
            if (file >= (folder2.files.Length - 1))
            {
                folder++;
                file = -1;
                if (folder >= this.folders.Length)
                {
                    return false;
                }
            }
            else
            {
                file++;
            }
            return true;
        }

        public void SetLineCount()
        {
            this.lv.totalRows = this.GetLineCount();
        }
    }
}

