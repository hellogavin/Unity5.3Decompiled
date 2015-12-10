namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SketchUpDataSource : TreeViewDataSource
    {
        private const int k_ProgressUpdateStep = 50;
        private SketchUpNodeInfo[] m_Nodes;

        public SketchUpDataSource(TreeView treeView, SketchUpNodeInfo[] nodes) : base(treeView)
        {
            this.m_Nodes = nodes;
            this.FetchData();
        }

        public override bool CanBeParent(TreeViewItem item)
        {
            return item.hasChildren;
        }

        public override void FetchData()
        {
            base.m_RootItem = new SketchUpNode(this.m_Nodes[0].nodeIndex, 0, null, this.m_Nodes[0].name, this.m_Nodes[0]);
            List<SketchUpNode> list = new List<SketchUpNode> {
                base.m_RootItem as SketchUpNode
            };
            this.SetExpanded(base.m_RootItem, true);
            int nodeIndex = this.m_Nodes[0].nodeIndex;
            for (int i = 1; i < this.m_Nodes.Length; i++)
            {
                SketchUpNodeInfo info = this.m_Nodes[i];
                if ((info.parent >= 0) && (info.parent <= list.Count))
                {
                    if (info.parent >= i)
                    {
                        Debug.LogError("Parent node index is greater than child node");
                    }
                    else if (nodeIndex >= info.nodeIndex)
                    {
                        Debug.LogError("Node array is not sorted by nodeIndex");
                    }
                    else
                    {
                        SketchUpNode parent = list[info.parent];
                        SketchUpNode item = new SketchUpNode(info.nodeIndex, parent.depth + 1, parent, info.name, info);
                        parent.children.Add(item);
                        this.SetExpanded(item, item.Info.enabled);
                        list.Add(item);
                        if ((i % 50) == 0)
                        {
                            EditorUtility.DisplayProgressBar("SketchUp Import", "Building Node Selection", ((float) i) / ((float) this.m_Nodes.Length));
                        }
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            base.m_NeedRefreshVisibleFolders = true;
        }

        public int[] FetchEnableNodes()
        {
            List<int> enable = new List<int>();
            this.InternalFetchEnableNodes(base.m_RootItem as SketchUpNode, enable);
            return enable.ToArray();
        }

        private void InternalFetchEnableNodes(SketchUpNode node, List<int> enable)
        {
            if (node.Enabled)
            {
                enable.Add(node.Info.nodeIndex);
            }
            foreach (TreeViewItem item in node.children)
            {
                this.InternalFetchEnableNodes(item as SketchUpNode, enable);
            }
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            return false;
        }
    }
}

