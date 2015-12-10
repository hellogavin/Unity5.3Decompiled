namespace UnityEditor
{
    using System;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditor.VersionControl;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class AssetsTreeViewGUI : TreeViewGUI
    {
        private const float k_IconOverlayPadding = 7f;
        private static bool s_VCEnabled;

        public AssetsTreeViewGUI(TreeView treeView) : base(treeView)
        {
            base.iconOverlayGUI = (Action<TreeViewItem, Rect>) Delegate.Combine(base.iconOverlayGUI, new Action<TreeViewItem, Rect>(this.OnIconOverlayGUI));
            base.k_TopRowMargin = 4f;
        }

        public virtual void BeginCreateNewAsset(int instanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
        {
            this.ClearRenameAndNewNodeState();
            if (this.GetCreateAssetUtility().BeginNewAssetCreation(instanceID, endAction, pathName, icon, resourceFile))
            {
                this.SyncFakeItem();
                if (!base.GetRenameOverlay().BeginRename(this.GetCreateAssetUtility().originalName, instanceID, 0f))
                {
                    Debug.LogError("Rename not started (when creating new asset)");
                }
            }
        }

        public override void BeginRowGUI()
        {
            s_VCEnabled = Provider.isActive;
            float num = !s_VCEnabled ? 0f : 7f;
            base.iconRightPadding = num;
            base.iconLeftPadding = num;
            base.BeginRowGUI();
        }

        protected override void ClearRenameAndNewNodeState()
        {
            this.GetCreateAssetUtility().Clear();
            base.ClearRenameAndNewNodeState();
        }

        protected CreateAssetUtility GetCreateAssetUtility()
        {
            return base.m_TreeView.state.createAssetUtility;
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            if (item == null)
            {
                return null;
            }
            Texture icon = null;
            if (this.IsCreatingNewAsset(item.id))
            {
                icon = this.GetCreateAssetUtility().icon;
            }
            if (icon == null)
            {
                icon = item.icon;
            }
            if ((icon == null) && (item.id != 0))
            {
                icon = AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(item.id));
            }
            return icon;
        }

        protected virtual bool IsCreatingNewAsset(int instanceID)
        {
            return (this.GetCreateAssetUtility().IsCreatingNewAsset() && this.IsRenaming(instanceID));
        }

        private void OnIconOverlayGUI(TreeViewItem item, Rect overlayRect)
        {
            if (s_VCEnabled && AssetDatabase.IsMainAsset(item.id))
            {
                ProjectHooks.OnProjectWindowItem(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(item.id)), overlayRect);
            }
        }

        protected override void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
            int userData = base.GetRenameOverlay().userData;
            bool flag = this.GetCreateAssetUtility().IsCreatingNewAsset();
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                if (flag)
                {
                    this.GetCreateAssetUtility().EndNewAssetCreation(name);
                }
                else
                {
                    ObjectNames.SetNameSmartWithInstanceID(userData, name);
                }
            }
        }

        protected override void SyncFakeItem()
        {
            if (!base.m_TreeView.data.HasFakeItem() && this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(this.GetCreateAssetUtility().folder);
                base.m_TreeView.data.InsertFakeItem(this.GetCreateAssetUtility().instanceID, mainAssetInstanceID, this.GetCreateAssetUtility().originalName, this.GetCreateAssetUtility().icon);
            }
            if (base.m_TreeView.data.HasFakeItem() && !this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                base.m_TreeView.data.RemoveFakeItem();
            }
        }
    }
}

