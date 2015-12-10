namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [FilePath("SearchFilters", FilePathAttribute.Location.PreferencesFolder)]
    internal class SavedSearchFilters : ScriptableSingleton<SavedSearchFilters>
    {
        private bool m_AllowHierarchy;
        [SerializeField]
        private List<SavedFilter> m_SavedFilters;
        private Action m_SavedFiltersChanged;

        private int Add(string displayName, SearchFilter filter, float previewSize, int insertAfterInstanceID, bool addAsChild)
        {
            SearchFilter filter2 = null;
            if (filter != null)
            {
                filter2 = ObjectCopier.DeepClone<SearchFilter>(filter);
            }
            if ((filter2.GetState() == SearchFilter.State.SearchingInAllAssets) || (filter2.GetState() == SearchFilter.State.SearchingInAssetStore))
            {
                filter2.folders = new string[0];
            }
            int index = 0;
            if (insertAfterInstanceID != 0)
            {
                index = this.IndexOf(insertAfterInstanceID);
                if (index == -1)
                {
                    Debug.LogError("Invalid insert position");
                    return 0;
                }
            }
            int depth = this.m_SavedFilters[index].m_Depth + (!addAsChild ? 0 : 1);
            SavedFilter item = new SavedFilter(displayName, filter2, depth, previewSize) {
                m_ID = this.GetNextAvailableID()
            };
            if (this.m_SavedFilters.Count == 0)
            {
                this.m_SavedFilters.Add(item);
            }
            else
            {
                this.m_SavedFilters.Insert(index + 1, item);
            }
            this.Changed();
            return item.m_ID;
        }

        public static void AddChangeListener(Action callback)
        {
            SavedSearchFilters instance = ScriptableSingleton<SavedSearchFilters>.instance;
            instance.m_SavedFiltersChanged = (Action) Delegate.Remove(instance.m_SavedFiltersChanged, callback);
            SavedSearchFilters local2 = ScriptableSingleton<SavedSearchFilters>.instance;
            local2.m_SavedFiltersChanged = (Action) Delegate.Combine(local2.m_SavedFiltersChanged, callback);
        }

        public static int AddSavedFilter(string displayName, SearchFilter filter, float previewSize)
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.Add(displayName, filter, previewSize, GetRootInstanceID(), true);
        }

        public static int AddSavedFilterAfterInstanceID(string displayName, SearchFilter filter, float previewSize, int insertAfterID, bool addAsChild)
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.Add(displayName, filter, previewSize, insertAfterID, addAsChild);
        }

        public static bool AllowsHierarchy()
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.m_AllowHierarchy;
        }

        private TreeViewItem BuildTreeView()
        {
            this.Init();
            if (this.m_SavedFilters.Count == 0)
            {
                Debug.LogError("BuildTreeView: No saved filters! We should at least have a root");
                return null;
            }
            TreeViewItem root = null;
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            for (int i = 0; i < this.m_SavedFilters.Count; i++)
            {
                SavedFilter filter = this.m_SavedFilters[i];
                int iD = filter.m_ID;
                int depth = filter.m_Depth;
                bool isFolder = filter.m_Filter.GetState() == SearchFilter.State.FolderBrowsing;
                TreeViewItem item = new SearchFilterTreeItem(iD, depth, null, filter.m_Name, isFolder);
                if (i == 0)
                {
                    root = item;
                }
                else
                {
                    visibleItems.Add(item);
                }
            }
            TreeViewUtility.SetChildParentReferences(visibleItems, root);
            return root;
        }

        public static bool CanMoveSavedFilter(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.IsValidMove(instanceID, parentInstanceID, targetInstanceID, after);
        }

        private void Changed()
        {
            bool saveAsText = true;
            this.Save(saveAsText);
            if (this.m_SavedFiltersChanged != null)
            {
                this.m_SavedFiltersChanged();
            }
        }

        public static TreeViewItem ConvertToTreeView()
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.BuildTreeView();
        }

        private SavedFilter Find(int instanceID)
        {
            int index = this.IndexOf(instanceID);
            if (index >= 0)
            {
                return this.m_SavedFilters[index];
            }
            return null;
        }

        public static SearchFilter GetFilter(int instanceID)
        {
            SavedFilter filter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
            if ((filter != null) && (filter.m_Filter != null))
            {
                return ObjectCopier.DeepClone<SearchFilter>(filter.m_Filter);
            }
            return null;
        }

        public static string GetName(int instanceID)
        {
            SavedFilter filter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
            if (filter != null)
            {
                return filter.m_Name;
            }
            Debug.LogError(string.Concat(new object[] { "Could not find saved filter ", instanceID, " ", ScriptableSingleton<SavedSearchFilters>.instance.ToString() }));
            return string.Empty;
        }

        private int GetNextAvailableID()
        {
            List<int> list = new List<int>();
            foreach (SavedFilter filter in this.m_SavedFilters)
            {
                if (filter.m_ID >= ProjectWindowUtil.k_FavoritesStartInstanceID)
                {
                    list.Add(filter.m_ID);
                }
            }
            list.Sort();
            int item = ProjectWindowUtil.k_FavoritesStartInstanceID;
            for (int i = 0; i < 0x3e8; i++)
            {
                if (list.BinarySearch(item) < 0)
                {
                    return item;
                }
                item++;
            }
            Debug.LogError(string.Concat(new object[] { "Could not assign valid ID to saved filter ", DebugUtils.ListToString<int>(list), " ", item }));
            return (ProjectWindowUtil.k_FavoritesStartInstanceID + 0x3e8);
        }

        public static float GetPreviewSize(int instanceID)
        {
            SavedFilter filter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
            if (filter != null)
            {
                return filter.m_PreviewSize;
            }
            return -1f;
        }

        private int GetRoot()
        {
            if ((this.m_SavedFilters != null) && (this.m_SavedFilters.Count > 0))
            {
                return this.m_SavedFilters[0].m_ID;
            }
            return 0;
        }

        public static int GetRootInstanceID()
        {
            return ScriptableSingleton<SavedSearchFilters>.instance.GetRoot();
        }

        private List<SavedFilter> GetSavedFilterAndChildren(int instanceID)
        {
            List<SavedFilter> list = new List<SavedFilter>();
            int index = this.IndexOf(instanceID);
            if (index >= 0)
            {
                list.Add(this.m_SavedFilters[index]);
                for (int i = index + 1; i < this.m_SavedFilters.Count; i++)
                {
                    if (this.m_SavedFilters[i].m_Depth <= this.m_SavedFilters[index].m_Depth)
                    {
                        return list;
                    }
                    list.Add(this.m_SavedFilters[i]);
                }
            }
            return list;
        }

        private int IndexOf(int instanceID)
        {
            for (int i = 0; i < this.m_SavedFilters.Count; i++)
            {
                if (this.m_SavedFilters[i].m_ID == instanceID)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Init()
        {
            if ((this.m_SavedFilters == null) || (this.m_SavedFilters.Count == 0))
            {
                this.m_SavedFilters = new List<SavedFilter>();
                this.m_SavedFilters.Add(new SavedFilter("Favorites", null, 0, -1f));
            }
            SearchFilter filter = new SearchFilter {
                classNames = new string[0]
            };
            this.m_SavedFilters[0].m_Name = "Favorites";
            this.m_SavedFilters[0].m_Filter = filter;
            this.m_SavedFilters[0].m_Depth = 0;
            this.m_SavedFilters[0].m_ID = ProjectWindowUtil.k_FavoritesStartInstanceID;
            for (int i = 0; i < this.m_SavedFilters.Count; i++)
            {
                if (this.m_SavedFilters[i].m_ID < ProjectWindowUtil.k_FavoritesStartInstanceID)
                {
                    this.m_SavedFilters[i].m_ID = this.GetNextAvailableID();
                }
            }
            if (!this.m_AllowHierarchy)
            {
                for (int j = 1; j < this.m_SavedFilters.Count; j++)
                {
                    this.m_SavedFilters[j].m_Depth = 1;
                }
            }
        }

        public static bool IsSavedFilter(int instanceID)
        {
            return (ScriptableSingleton<SavedSearchFilters>.instance.IndexOf(instanceID) >= 0);
        }

        private bool IsValidMove(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
        {
            int index = this.IndexOf(instanceID);
            int num2 = this.IndexOf(parentInstanceID);
            int num3 = this.IndexOf(targetInstanceID);
            if (((index < 0) || (num2 < 0)) || (num3 < 0))
            {
                Debug.LogError(string.Concat(new object[] { "Move of a SavedFilter has invalid input: ", index, " ", num2, " ", num3 }));
                return false;
            }
            if (instanceID == targetInstanceID)
            {
                return false;
            }
            for (int i = index + 1; i < this.m_SavedFilters.Count; i++)
            {
                if (this.m_SavedFilters[i].m_Depth <= this.m_SavedFilters[index].m_Depth)
                {
                    break;
                }
                if ((i == num3) || (i == num2))
                {
                    return false;
                }
            }
            return true;
        }

        private void Move(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
        {
            if (this.IsValidMove(instanceID, parentInstanceID, targetInstanceID, after))
            {
                int index = this.IndexOf(instanceID);
                int num2 = this.IndexOf(parentInstanceID);
                int num3 = this.IndexOf(targetInstanceID);
                SavedFilter filter = this.m_SavedFilters[index];
                SavedFilter filter2 = this.m_SavedFilters[num2];
                int num4 = 0;
                if (this.m_AllowHierarchy)
                {
                    num4 = (filter2.m_Depth + 1) - filter.m_Depth;
                }
                List<SavedFilter> savedFilterAndChildren = this.GetSavedFilterAndChildren(instanceID);
                this.m_SavedFilters.RemoveRange(index, savedFilterAndChildren.Count);
                foreach (SavedFilter filter3 in savedFilterAndChildren)
                {
                    filter3.m_Depth += num4;
                }
                num3 = this.IndexOf(targetInstanceID);
                if (num3 != -1)
                {
                    this.m_SavedFilters.InsertRange(num3 + 1, savedFilterAndChildren);
                }
                this.Changed();
            }
        }

        public static void MoveSavedFilter(int instanceID, int parentInstanceID, int targetInstanceID, bool after)
        {
            ScriptableSingleton<SavedSearchFilters>.instance.Move(instanceID, parentInstanceID, targetInstanceID, after);
        }

        private void Remove(int instanceID)
        {
            int index = this.IndexOf(instanceID);
            if (index >= 1)
            {
                List<SavedFilter> savedFilterAndChildren = this.GetSavedFilterAndChildren(instanceID);
                if (savedFilterAndChildren.Count > 0)
                {
                    this.m_SavedFilters.RemoveRange(index, savedFilterAndChildren.Count);
                    this.Changed();
                }
            }
        }

        public static void RemoveSavedFilter(int instanceID)
        {
            ScriptableSingleton<SavedSearchFilters>.instance.Remove(instanceID);
        }

        public static void SetName(int instanceID, string name)
        {
            SavedFilter filter = ScriptableSingleton<SavedSearchFilters>.instance.Find(instanceID);
            if (filter != null)
            {
                filter.m_Name = name;
                ScriptableSingleton<SavedSearchFilters>.instance.Changed();
            }
            else
            {
                Debug.LogError(string.Concat(new object[] { "Could not set name of saved filter ", instanceID, " ", ScriptableSingleton<SavedSearchFilters>.instance.ToString() }));
            }
        }

        public override string ToString()
        {
            string str = "Saved Filters ";
            for (int i = 0; i < this.m_SavedFilters.Count; i++)
            {
                int iD = this.m_SavedFilters[i].m_ID;
                SavedFilter filter = this.m_SavedFilters[i];
                object[] args = new object[] { filter.m_Name, iD, filter.m_Depth, filter.m_PreviewSize };
                str = str + string.Format(": {0} ({1})({2})({3}) ", args);
            }
            return str;
        }

        public static void UpdateExistingSavedFilter(int instanceID, SearchFilter filter, float previewSize)
        {
            ScriptableSingleton<SavedSearchFilters>.instance.UpdateFilter(instanceID, filter, previewSize);
        }

        private void UpdateFilter(int instanceID, SearchFilter filter, float previewSize)
        {
            SavedFilter filter2 = this.Find(instanceID);
            if (filter2 != null)
            {
                SearchFilter filter3 = null;
                if (filter != null)
                {
                    filter3 = ObjectCopier.DeepClone<SearchFilter>(filter);
                    filter2.m_Filter = filter3;
                }
                filter2.m_PreviewSize = previewSize;
                this.Changed();
            }
            else
            {
                Debug.LogError(string.Concat(new object[] { "Could not find saved filter ", instanceID, " ", ScriptableSingleton<SavedSearchFilters>.instance.ToString() }));
            }
        }
    }
}

