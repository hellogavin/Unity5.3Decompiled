namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal interface ITreeViewDataSource
    {
        bool CanBeMultiSelected(TreeViewItem item);
        bool CanBeParent(TreeViewItem item);
        TreeViewItem FindItem(int id);
        int[] GetExpandedIDs();
        TreeViewItem GetItem(int row);
        int GetRow(int id);
        List<TreeViewItem> GetRows();
        bool HasFakeItem();
        void InitIfNeeded();
        void InsertFakeItem(int id, int parentID, string name, Texture2D icon);
        bool IsExpandable(TreeViewItem item);
        bool IsExpanded(TreeViewItem item);
        bool IsRenamingItemAllowed(TreeViewItem item);
        bool IsRevealed(int id);
        void OnInitialize();
        void OnSearchChanged();
        void ReloadData();
        void RemoveFakeItem();
        void RevealItem(int id);
        void SetExpanded(TreeViewItem item, bool expand);
        void SetExpandedIDs(int[] ids);
        void SetExpandedWithChildren(TreeViewItem item, bool expand);

        TreeViewItem root { get; }

        int rowCount { get; }
    }
}

