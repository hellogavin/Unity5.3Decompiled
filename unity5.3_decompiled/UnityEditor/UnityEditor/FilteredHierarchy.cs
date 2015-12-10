namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class FilteredHierarchy
    {
        [CompilerGenerated]
        private static Comparison<FilterResult> <>f__am$cache5;
        private HierarchyType m_HierarchyType;
        private FilterResult[] m_Results = new FilterResult[0];
        private SearchFilter m_SearchFilter = new SearchFilter();
        private FilterResult[] m_VisibleItems = new FilterResult[0];

        public FilteredHierarchy(HierarchyType type)
        {
            this.m_HierarchyType = type;
        }

        private void AddResults(HierarchyProperty property)
        {
            switch (this.m_SearchFilter.GetState())
            {
                case SearchFilter.State.EmptySearchFilter:
                case SearchFilter.State.SearchingInAssetStore:
                    break;

                case SearchFilter.State.FolderBrowsing:
                    this.FolderBrowsing(property);
                    break;

                case SearchFilter.State.SearchingInAllAssets:
                    this.SearchAllAssets(property);
                    break;

                case SearchFilter.State.SearchingInFolders:
                    this.SearchInFolders(property);
                    break;

                default:
                    Debug.LogError("Unhandled enum!");
                    break;
            }
        }

        public int AddSubItemsOfMainRepresentation(int mainRepresentionIndex, List<FilterResult> visibleItems)
        {
            int num = 0;
            int index = mainRepresentionIndex + 1;
            while ((index < this.m_Results.Length) && !this.m_Results[index].isMainRepresentation)
            {
                if (visibleItems != null)
                {
                    visibleItems.Add(this.m_Results[index]);
                }
                index++;
                num++;
            }
            return num;
        }

        private void CopyPropertyData(ref FilterResult result, HierarchyProperty property)
        {
            if (result == null)
            {
                result = new FilterResult();
            }
            result.instanceID = property.instanceID;
            result.name = property.name;
            result.hasChildren = property.hasChildren;
            result.colorCode = property.colorCode;
            result.isMainRepresentation = property.isMainRepresentation;
            result.hasFullPreviewImage = property.hasFullPreviewImage;
            result.iconDrawStyle = property.iconDrawStyle;
            result.isFolder = property.isFolder;
            result.type = this.hierarchyType;
            if (!property.isMainRepresentation)
            {
                result.icon = property.icon;
            }
            else if (property.isFolder && !property.hasChildren)
            {
                result.icon = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
            }
            else
            {
                result.icon = null;
            }
        }

        private void FolderBrowsing(HierarchyProperty property)
        {
            List<FilterResult> list = new List<FilterResult>();
            foreach (string str in this.m_SearchFilter.folders)
            {
                int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(str);
                if (property.Find(mainAssetInstanceID, null))
                {
                    int depth = property.depth;
                    int[] expanded = new int[] { mainAssetInstanceID };
                    while (property.Next(expanded))
                    {
                        if (property.depth <= depth)
                        {
                            break;
                        }
                        FilterResult result = new FilterResult();
                        this.CopyPropertyData(ref result, property);
                        list.Add(result);
                        if (property.hasChildren && !property.isFolder)
                        {
                            Array.Resize<int>(ref expanded, expanded.Length + 1);
                            expanded[expanded.Length - 1] = property.instanceID;
                        }
                    }
                }
            }
            this.m_Results = list.ToArray();
        }

        public List<int> GetSubAssetInstanceIDs(int mainAssetInstanceID)
        {
            for (int i = 0; i < this.m_Results.Length; i++)
            {
                if (this.m_Results[i].instanceID == mainAssetInstanceID)
                {
                    List<int> list = new List<int>();
                    for (int j = i + 1; (j < this.m_Results.Length) && !this.m_Results[j].isMainRepresentation; j++)
                    {
                        list.Add(this.m_Results[j].instanceID);
                    }
                    return list;
                }
            }
            Debug.LogError("Not main rep " + mainAssetInstanceID);
            return new List<int>();
        }

        public void RefreshVisibleItems(List<int> expandedInstanceIDs)
        {
            bool flag = this.m_SearchFilter.IsSearching();
            List<FilterResult> list = new List<FilterResult>();
            for (int i = 0; i < this.m_Results.Length; i++)
            {
                list.Add(this.m_Results[i]);
                if ((this.m_Results[i].isMainRepresentation && this.m_Results[i].hasChildren) && !this.m_Results[i].isFolder)
                {
                    bool flag3 = (expandedInstanceIDs.IndexOf(this.m_Results[i].instanceID) >= 0) || flag;
                    int num2 = this.AddSubItemsOfMainRepresentation(i, !flag3 ? null : list);
                    i += num2;
                }
            }
            this.m_VisibleItems = list.ToArray();
        }

        public void ResultsChanged()
        {
            this.m_Results = new FilterResult[0];
            if (this.m_SearchFilter.GetState() != SearchFilter.State.EmptySearchFilter)
            {
                HierarchyProperty property = new HierarchyProperty(this.m_HierarchyType);
                property.SetSearchFilter(this.m_SearchFilter);
                this.AddResults(property);
                if (this.m_SearchFilter.IsSearching())
                {
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = (result1, result2) => EditorUtility.NaturalCompare(result1.name, result2.name);
                    }
                    Array.Sort<FilterResult>(this.m_Results, <>f__am$cache5);
                }
                if (this.foldersFirst)
                {
                    for (int i = 0; i < this.m_Results.Length; i++)
                    {
                        if (!this.m_Results[i].isFolder)
                        {
                            for (int j = i + 1; j < this.m_Results.Length; j++)
                            {
                                if (this.m_Results[j].isFolder)
                                {
                                    FilterResult result = this.m_Results[j];
                                    int length = j - i;
                                    Array.Copy(this.m_Results, i, this.m_Results, i + 1, length);
                                    this.m_Results[i] = result;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (this.m_HierarchyType == HierarchyType.GameObjects)
            {
                new HierarchyProperty(HierarchyType.GameObjects).SetSearchFilter(this.m_SearchFilter);
            }
        }

        private void SearchAllAssets(HierarchyProperty property)
        {
            int num2 = Mathf.Min(property.CountRemaining(null), 0xbb8);
            property.Reset();
            int length = this.m_Results.Length;
            Array.Resize<FilterResult>(ref this.m_Results, this.m_Results.Length + num2);
            while (property.Next(null) && (length < this.m_Results.Length))
            {
                this.CopyPropertyData(ref this.m_Results[length], property);
                length++;
            }
        }

        private void SearchInFolders(HierarchyProperty property)
        {
            List<FilterResult> list = new List<FilterResult>();
            foreach (string str in ProjectWindowUtil.GetBaseFolders(this.m_SearchFilter.folders))
            {
                property.SetSearchFilter(new SearchFilter());
                int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(str);
                if (property.Find(mainAssetInstanceID, null))
                {
                    property.SetSearchFilter(this.m_SearchFilter);
                    int depth = property.depth;
                    int[] expanded = null;
                    while (property.NextWithDepthCheck(expanded, depth + 1))
                    {
                        FilterResult result = new FilterResult();
                        this.CopyPropertyData(ref result, property);
                        list.Add(result);
                    }
                }
            }
            this.m_Results = list.ToArray();
        }

        public void SetResults(int[] instanceIDs)
        {
            HierarchyProperty property = new HierarchyProperty(this.m_HierarchyType);
            property.Reset();
            Array.Resize<FilterResult>(ref this.m_Results, instanceIDs.Length);
            for (int i = 0; i < instanceIDs.Length; i++)
            {
                if (property.Find(instanceIDs[i], null))
                {
                    this.CopyPropertyData(ref this.m_Results[i], property);
                }
            }
        }

        public bool foldersFirst { get; set; }

        public HierarchyType hierarchyType
        {
            get
            {
                return this.m_HierarchyType;
            }
        }

        public FilterResult[] results
        {
            get
            {
                if (this.m_VisibleItems.Length > 0)
                {
                    return this.m_VisibleItems;
                }
                return this.m_Results;
            }
        }

        public SearchFilter searchFilter
        {
            get
            {
                return this.m_SearchFilter;
            }
            set
            {
                if (this.m_SearchFilter.SetNewFilter(value))
                {
                    this.ResultsChanged();
                }
            }
        }

        public class FilterResult
        {
            public int colorCode;
            public bool hasChildren;
            public bool hasFullPreviewImage;
            public IconDrawStyle iconDrawStyle;
            public int instanceID;
            public bool isFolder;
            public bool isMainRepresentation;
            private Texture2D m_Icon;
            public string name;
            public HierarchyType type;

            public string guid
            {
                get
                {
                    if (this.type == HierarchyType.Assets)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(this.instanceID);
                        if (assetPath != null)
                        {
                            return AssetDatabase.AssetPathToGUID(assetPath);
                        }
                    }
                    return null;
                }
            }

            public Texture2D icon
            {
                get
                {
                    if (this.m_Icon == null)
                    {
                        if (this.type == HierarchyType.Assets)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(this.instanceID);
                            if (assetPath != null)
                            {
                                return (AssetDatabase.GetCachedIcon(assetPath) as Texture2D);
                            }
                        }
                        else if (this.type == HierarchyType.GameObjects)
                        {
                            Object obj2 = EditorUtility.InstanceIDToObject(this.instanceID);
                            this.m_Icon = AssetPreview.GetMiniThumbnail(obj2);
                        }
                    }
                    return this.m_Icon;
                }
                set
                {
                    this.m_Icon = value;
                }
            }
        }
    }
}

