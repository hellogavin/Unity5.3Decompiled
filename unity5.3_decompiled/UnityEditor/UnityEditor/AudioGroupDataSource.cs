namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioGroupDataSource : TreeViewDataSource
    {
        public AudioMixerController m_Controller;

        public AudioGroupDataSource(TreeView treeView, AudioMixerController controller) : base(treeView)
        {
            this.m_Controller = controller;
        }

        private void AddNodesRecursively(AudioMixerGroupController group, TreeViewItem parent, int depth)
        {
            List<TreeViewItem> list = new List<TreeViewItem>();
            for (int i = 0; i < group.children.Length; i++)
            {
                AudioMixerTreeViewNode item = new AudioMixerTreeViewNode(GetUniqueNodeID(group.children[i]), depth, parent, group.children[i].name, group.children[i]) {
                    parent = parent
                };
                list.Add(item);
                this.AddNodesRecursively(group.children[i], item, depth + 1);
            }
            parent.children = list;
        }

        public override void FetchData()
        {
            if (this.m_Controller == null)
            {
                base.m_RootItem = null;
            }
            else if (this.m_Controller.masterGroup == null)
            {
                Debug.LogError("The Master group is missing !!!");
                base.m_RootItem = null;
            }
            else
            {
                int uniqueNodeID = GetUniqueNodeID(this.m_Controller.masterGroup);
                base.m_RootItem = new AudioMixerTreeViewNode(uniqueNodeID, 0, null, this.m_Controller.masterGroup.name, this.m_Controller.masterGroup);
                this.AddNodesRecursively(this.m_Controller.masterGroup, base.m_RootItem, 1);
                base.m_NeedRefreshVisibleFolders = true;
            }
        }

        public static int GetUniqueNodeID(AudioMixerGroupController group)
        {
            return group.GetInstanceID();
        }

        public override bool IsRenamingItemAllowed(TreeViewItem node)
        {
            AudioMixerTreeViewNode node2 = node as AudioMixerTreeViewNode;
            if (node2.group == this.m_Controller.masterGroup)
            {
                return false;
            }
            return true;
        }
    }
}

