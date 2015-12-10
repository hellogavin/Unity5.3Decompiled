namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ProjectBrowserColumnOneTreeViewDataSource : TreeViewDataSource
    {
        private static string kProjectBrowserString = "ProjectBrowser";

        public ProjectBrowserColumnOneTreeViewDataSource(TreeView treeView) : base(treeView)
        {
            base.showRootNode = false;
            base.rootIsCollapsable = false;
            SavedSearchFilters.AddChangeListener(new Action(this.ReloadData));
        }

        public override bool CanBeMultiSelected(TreeViewItem item)
        {
            return (ProjectBrowser.GetItemType(item.id) != ProjectBrowser.ItemType.SavedFilter);
        }

        public override bool CanBeParent(TreeViewItem item)
        {
            return (!(item is SearchFilterTreeItem) || SavedSearchFilters.AllowsHierarchy());
        }

        public override void FetchData()
        {
            base.m_RootItem = new TreeViewItem(0x7fffffff, 0, null, "Invisible Root Item");
            this.SetExpanded(base.m_RootItem, true);
            List<TreeViewItem> list = new List<TreeViewItem>();
            int assetsFolderInstanceID = GetAssetsFolderInstanceID();
            int depth = 0;
            string displayName = "Assets";
            TreeViewItem parent = new TreeViewItem(assetsFolderInstanceID, depth, base.m_RootItem, displayName);
            this.ReadAssetDatabase(parent, depth + 1);
            TreeViewItem item = SavedSearchFilters.ConvertToTreeView();
            item.parent = base.m_RootItem;
            list.Add(item);
            list.Add(parent);
            base.m_RootItem.children = list;
            foreach (TreeViewItem item3 in base.m_RootItem.children)
            {
                bool @bool = EditorPrefs.GetBool(kProjectBrowserString + item3.displayName, true);
                this.SetExpanded(item3, @bool);
            }
            base.m_NeedRefreshVisibleFolders = true;
        }

        public static int GetAssetsFolderInstanceID()
        {
            string path = "Assets";
            return AssetDatabase.GetInstanceIDFromGUID(AssetDatabase.AssetPathToGUID(path));
        }

        public override bool IsExpandable(TreeViewItem item)
        {
            return (item.hasChildren && ((item != base.m_RootItem) || base.rootIsCollapsable));
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            if (this.IsVisibleRootNode(item))
            {
                return false;
            }
            return base.IsRenamingItemAllowed(item);
        }

        public bool IsVisibleRootNode(TreeViewItem item)
        {
            return ((item.parent != null) && (item.parent.parent == null));
        }

        private void ReadAssetDatabase(TreeViewItem parent, int baseDepth)
        {
            IHierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            property.Reset();
            Texture2D textured = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            Texture2D textured2 = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            while (property.Next(null))
            {
                if (property.isFolder)
                {
                    TreeViewItem item = new TreeViewItem(property.instanceID, baseDepth + property.depth, null, property.name) {
                        icon = !property.hasChildren ? textured2 : textured
                    };
                    visibleItems.Add(item);
                }
            }
            TreeViewUtility.SetChildParentReferences(visibleItems, parent);
        }

        public override bool SetExpanded(int id, bool expand)
        {
            if (!base.SetExpanded(id, expand))
            {
                return false;
            }
            InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
            if (base.m_RootItem.hasChildren)
            {
                foreach (TreeViewItem item in base.m_RootItem.children)
                {
                    if (item.id == id)
                    {
                        EditorPrefs.SetBool(kProjectBrowserString + item.displayName, expand);
                    }
                }
            }
            return true;
        }
    }
}

