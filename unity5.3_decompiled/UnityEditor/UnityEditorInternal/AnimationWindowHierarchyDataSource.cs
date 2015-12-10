namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowHierarchyDataSource : TreeViewDataSource
    {
        public AnimationWindowHierarchyDataSource(TreeView treeView, AnimationWindowState animationWindowState) : base(treeView)
        {
            this.state = animationWindowState;
        }

        private AnimationWindowHierarchyPropertyGroupNode AddPropertyGroupToHierarchy(AnimationWindowCurve[] curves, AnimationWindowHierarchyNode parentNode)
        {
            List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
            AnimationWindowHierarchyPropertyGroupNode node = new AnimationWindowHierarchyPropertyGroupNode(curves[0].type, AnimationWindowUtility.GetPropertyGroupName(curves[0].propertyName), curves[0].path, parentNode) {
                icon = this.GetIcon(curves[0].binding),
                indent = curves[0].depth,
                curves = curves
            };
            foreach (AnimationWindowCurve curve in curves)
            {
                AnimationWindowHierarchyPropertyNode item = this.AddPropertyToHierarchy(curve, node);
                item.displayName = AnimationWindowUtility.GetPropertyDisplayName(item.propertyName);
                list.Add(item);
            }
            TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), node);
            return node;
        }

        private AnimationWindowHierarchyPropertyNode AddPropertyToHierarchy(AnimationWindowCurve curve, AnimationWindowHierarchyNode parentNode)
        {
            AnimationWindowHierarchyPropertyNode node = new AnimationWindowHierarchyPropertyNode(curve.type, curve.propertyName, curve.path, parentNode, curve.binding, curve.isPPtrCurve);
            if (parentNode.icon != null)
            {
                node.icon = parentNode.icon;
            }
            else
            {
                node.icon = this.GetIcon(curve.binding);
            }
            node.indent = curve.depth;
            node.curves = new AnimationWindowCurve[] { curve };
            return node;
        }

        public List<AnimationWindowHierarchyNode> CreateTreeFromCurves()
        {
            List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
            List<AnimationWindowCurve> list2 = new List<AnimationWindowCurve>();
            AnimationWindowCurve[] curveArray = this.state.allCurves.ToArray();
            for (int i = 0; i < curveArray.Length; i++)
            {
                AnimationWindowCurve item = curveArray[i];
                AnimationWindowCurve curve2 = (i >= (curveArray.Length - 1)) ? null : curveArray[i + 1];
                list2.Add(item);
                bool flag = (curve2 != null) && (AnimationWindowUtility.GetPropertyGroupName(curve2.propertyName) == AnimationWindowUtility.GetPropertyGroupName(item.propertyName));
                bool flag2 = ((curve2 != null) && item.path.Equals(curve2.path)) && (item.type == curve2.type);
                if (((i == (curveArray.Length - 1)) || !flag) || !flag2)
                {
                    if (list2.Count > 1)
                    {
                        list.Add(this.AddPropertyGroupToHierarchy(list2.ToArray(), (AnimationWindowHierarchyNode) base.m_RootItem));
                    }
                    else
                    {
                        list.Add(this.AddPropertyToHierarchy(list2[0], (AnimationWindowHierarchyNode) base.m_RootItem));
                    }
                    list2.Clear();
                }
            }
            return list;
        }

        public override void FetchData()
        {
            base.m_RootItem = this.GetEmptyRootNode();
            this.SetupRootNodeSettings();
            base.m_NeedRefreshVisibleFolders = true;
            if ((this.state.activeRootGameObject != null) || (this.state.activeAnimationClip != null))
            {
                List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
                if (this.state.allCurves.Count > 0)
                {
                    list.Add(new AnimationWindowHierarchyMasterNode());
                }
                list.AddRange(this.CreateTreeFromCurves());
                list.Add(new AnimationWindowHierarchyAddButtonNode());
                TreeViewUtility.SetChildParentReferences(new List<TreeViewItem>(list.ToArray()), this.root);
            }
        }

        private AnimationWindowHierarchyNode GetEmptyRootNode()
        {
            return new AnimationWindowHierarchyNode(0, -1, null, null, string.Empty, string.Empty, "root");
        }

        public Texture2D GetIcon(EditorCurveBinding curveBinding)
        {
            if ((this.state.activeRootGameObject != null) && (AnimationUtility.GetAnimatedObject(this.state.activeRootGameObject, curveBinding) != null))
            {
                return AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(this.state.activeRootGameObject, curveBinding));
            }
            return AssetPreview.GetMiniTypeThumbnail(curveBinding.type);
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            if ((item is AnimationWindowHierarchyAddButtonNode) || (item is AnimationWindowHierarchyMasterNode))
            {
                return false;
            }
            if ((item as AnimationWindowHierarchyNode).path.Length == 0)
            {
                return false;
            }
            return true;
        }

        private void SetupRootNodeSettings()
        {
            base.showRootNode = false;
            base.rootIsCollapsable = false;
            this.SetExpanded(base.m_RootItem, true);
        }

        public void UpdateData()
        {
            base.m_TreeView.ReloadData();
        }

        public bool showAll { get; set; }

        private AnimationWindowState state { get; set; }
    }
}

