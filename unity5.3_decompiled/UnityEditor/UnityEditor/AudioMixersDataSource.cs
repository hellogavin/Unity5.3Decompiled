namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixersDataSource : TreeViewDataSource
    {
        private Func<List<AudioMixerController>> m_GetAllControllersCallback;

        public AudioMixersDataSource(TreeView treeView, Func<List<AudioMixerController>> getAllControllersCallback) : base(treeView)
        {
            base.showRootNode = false;
            this.m_GetAllControllersCallback = getAllControllersCallback;
        }

        public override void FetchData()
        {
            int depth = -1;
            bool flag = base.m_TreeView.state.expandedIDs.Count == 0;
            base.m_RootItem = new TreeViewItem(0x3c34eb12, depth, null, "InvisibleRoot");
            this.SetExpanded(base.m_RootItem.id, true);
            List<AudioMixerController> list = this.m_GetAllControllersCallback();
            base.m_NeedRefreshVisibleFolders = true;
            if (list.Count > 0)
            {
                List<AudioMixerItem> items = (from mixer in list select new AudioMixerItem(mixer.GetInstanceID(), 0, base.m_RootItem, mixer.name, mixer, GetInfoText(mixer))).ToList<AudioMixerItem>();
                foreach (AudioMixerItem item in items)
                {
                    this.SetChildParentOfMixerItem(item, items);
                }
                this.SetItemDepthRecursive(base.m_RootItem, -1);
                this.SortRecursive(base.m_RootItem);
                if (flag)
                {
                    base.m_TreeView.data.SetExpandedWithChildren(base.m_RootItem, true);
                }
            }
        }

        private static string GetInfoText(AudioMixerController controller)
        {
            if (controller.outputAudioMixerGroup != null)
            {
                return string.Format("({0} of {1})", controller.outputAudioMixerGroup.name, controller.outputAudioMixerGroup.audioMixer.name);
            }
            return "(Audio Listener)";
        }

        public int GetInsertAfterItemIDForNewItem(string newName, TreeViewItem parentItem)
        {
            int id = parentItem.id;
            if (parentItem.hasChildren)
            {
                for (int i = 0; i < parentItem.children.Count; i++)
                {
                    int instanceID = parentItem.children[i].id;
                    if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(instanceID)), newName) > 0)
                    {
                        return id;
                    }
                    id = instanceID;
                }
            }
            return id;
        }

        public override void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
        {
            TreeViewItem item = this.FindItem(id);
            if (item != null)
            {
                Debug.LogError(string.Concat(new object[] { "Cannot insert fake Item because id is not unique ", id, " Item already there: ", item.displayName }));
            }
            else if (this.FindItem(parentID) == null)
            {
                Debug.LogError("No parent Item found with ID: " + parentID);
            }
            else
            {
                TreeViewItem rootItem;
                this.SetExpanded(parentID, true);
                List<TreeViewItem> rows = this.GetRows();
                int indexOfID = TreeView.GetIndexOfID(rows, parentID);
                if (indexOfID >= 0)
                {
                    rootItem = rows[indexOfID];
                }
                else
                {
                    rootItem = base.m_RootItem;
                }
                int depth = rootItem.depth + 1;
                base.m_FakeItem = new TreeViewItem(id, depth, rootItem, name);
                base.m_FakeItem.icon = icon;
                int insertAfterItemIDForNewItem = this.GetInsertAfterItemIDForNewItem(name, rootItem);
                int index = TreeView.GetIndexOfID(rows, insertAfterItemIDForNewItem);
                if (index < 0)
                {
                    if (rows.Count > 0)
                    {
                        rows.Insert(0, base.m_FakeItem);
                    }
                    else
                    {
                        rows.Add(base.m_FakeItem);
                    }
                }
                else
                {
                    while (++index < rows.Count)
                    {
                        if (rows[index].depth <= depth)
                        {
                            break;
                        }
                    }
                    if (index < rows.Count)
                    {
                        rows.Insert(index, base.m_FakeItem);
                    }
                    else
                    {
                        rows.Add(base.m_FakeItem);
                    }
                }
                base.m_NeedRefreshVisibleFolders = false;
                base.m_TreeView.Frame(base.m_FakeItem.id, true, false);
                base.m_TreeView.Repaint();
            }
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            return true;
        }

        private void SetChildParentOfMixerItem(AudioMixerItem item, List<AudioMixerItem> items)
        {
            if (item.mixer.outputAudioMixerGroup != null)
            {
                AudioMixerItem item2 = TreeViewUtility.FindItemInList<AudioMixerItem>(item.mixer.outputAudioMixerGroup.audioMixer.GetInstanceID(), items) as AudioMixerItem;
                if (item2 != null)
                {
                    item2.AddChild(item);
                }
            }
            else
            {
                base.m_RootItem.AddChild(item);
            }
        }

        private void SetItemDepthRecursive(TreeViewItem item, int depth)
        {
            item.depth = depth;
            if (item.hasChildren)
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.SetItemDepthRecursive(item2, depth + 1);
                }
            }
        }

        private void SortRecursive(TreeViewItem item)
        {
            if (item.hasChildren)
            {
                item.children.Sort(new TreeViewItemAlphaNumericSort());
                foreach (TreeViewItem item2 in item.children)
                {
                    this.SortRecursive(item2);
                }
            }
        }
    }
}

