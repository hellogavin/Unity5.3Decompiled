namespace UnityEditor
{
    using System;
    using System.Linq;
    using UnityEngine;

    internal class AudioMixersTreeViewGUI : TreeViewGUI
    {
        public AudioMixersTreeViewGUI(TreeView treeView) : base(treeView)
        {
            base.k_IconWidth = 0f;
            base.k_TopRowMargin = base.k_BottomRowMargin = 2f;
        }

        public void BeginCreateNewMixer()
        {
            this.ClearRenameAndNewNodeState();
            string newAssetResourceFile = string.Empty;
            AudioMixerItem selectedItem = this.GetSelectedItem();
            if ((selectedItem != null) && (selectedItem.mixer.outputAudioMixerGroup != null))
            {
                newAssetResourceFile = selectedItem.mixer.outputAudioMixerGroup.GetInstanceID().ToString();
            }
            int instanceID = 0;
            if (this.GetCreateAssetUtility().BeginNewAssetCreation(instanceID, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", null, newAssetResourceFile))
            {
                this.SyncFakeItem();
                if (!base.GetRenameOverlay().BeginRename(this.GetCreateAssetUtility().originalName, instanceID, 0f))
                {
                    Debug.LogError("Rename not started (when creating new asset)");
                }
            }
        }

        protected override void ClearRenameAndNewNodeState()
        {
            this.GetCreateAssetUtility().Clear();
            base.ClearRenameAndNewNodeState();
        }

        protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (!isPinging)
            {
                float contentIndent = this.GetContentIndent(item);
                rect.x += contentIndent;
                rect.width -= contentIndent;
            }
            AudioMixerItem item2 = item as AudioMixerItem;
            if (item2 != null)
            {
                GUIStyle style = !useBoldFont ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
                style.padding.left = (int) ((base.k_IconWidth + base.iconTotalPadding) + base.k_SpaceBetweenIconAndText);
                style.Draw(rect, label, false, false, selected, focused);
                item2.UpdateSuspendedString(false);
                if (item2.labelWidth <= 0f)
                {
                    item2.labelWidth = style.CalcSize(GUIContent.Temp(label)).x;
                }
                Rect position = rect;
                position.x += item2.labelWidth + 8f;
                EditorGUI.BeginDisabledGroup(true);
                style.Draw(position, item2.infoText, false, false, false, false);
                EditorGUI.EndDisabledGroup();
                if (base.iconOverlayGUI != null)
                {
                    Rect rect3 = rect;
                    rect3.width = base.k_IconWidth + base.iconTotalPadding;
                    base.iconOverlayGUI(item, rect3);
                }
            }
        }

        protected CreateAssetUtility GetCreateAssetUtility()
        {
            return base.m_TreeView.state.createAssetUtility;
        }

        protected override Texture GetIconForNode(TreeViewItem node)
        {
            return null;
        }

        private AudioMixerItem GetSelectedItem()
        {
            return (base.m_TreeView.FindNode(base.m_TreeView.GetSelection().FirstOrDefault<int>()) as AudioMixerItem);
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
                    base.m_TreeView.ReloadData();
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
                int id = base.m_TreeView.data.root.id;
                AudioMixerItem selectedItem = this.GetSelectedItem();
                if (selectedItem != null)
                {
                    id = selectedItem.parent.id;
                }
                base.m_TreeView.data.InsertFakeItem(this.GetCreateAssetUtility().instanceID, id, this.GetCreateAssetUtility().originalName, this.GetCreateAssetUtility().icon);
            }
            if (base.m_TreeView.data.HasFakeItem() && !this.GetCreateAssetUtility().IsCreatingNewAsset())
            {
                base.m_TreeView.data.RemoveFakeItem();
            }
        }
    }
}

