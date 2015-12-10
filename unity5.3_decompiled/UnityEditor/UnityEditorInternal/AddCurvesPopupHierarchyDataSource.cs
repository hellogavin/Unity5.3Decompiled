namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AddCurvesPopupHierarchyDataSource : TreeViewDataSource
    {
        public AddCurvesPopupHierarchyDataSource(TreeView treeView, AnimationWindowState animationWindowState) : base(treeView)
        {
            base.showRootNode = false;
            base.rootIsCollapsable = false;
            this.state = animationWindowState;
        }

        private TreeViewItem AddAnimatableObjectToHierarchy(GameObject root, EditorCurveBinding[] curveBindings, TreeViewItem parentNode, string path)
        {
            TreeViewItem item = new AddCurvesPopupObjectNode(parentNode, path, GetClassName(root, curveBindings[0])) {
                icon = AssetPreview.GetMiniThumbnail(AnimationUtility.GetAnimatedObject(root, curveBindings[0]))
            };
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
            for (int i = 0; i < curveBindings.Length; i++)
            {
                EditorCurveBinding binding = curveBindings[i];
                list2.Add(binding);
                if ((i == (curveBindings.Length - 1)) || (AnimationWindowUtility.GetPropertyGroupName(curveBindings[i + 1].propertyName) != AnimationWindowUtility.GetPropertyGroupName(binding.propertyName)))
                {
                    TreeViewItem item2 = this.CreateNode(list2.ToArray(), item);
                    if (item2 != null)
                    {
                        visibleItems.Add(item2);
                    }
                    list2.Clear();
                }
            }
            visibleItems.Sort();
            TreeViewUtility.SetChildParentReferences(visibleItems, item);
            return item;
        }

        private TreeViewItem AddGameObjectToHierarchy(GameObject gameObject, TreeViewItem parent)
        {
            string path = AnimationUtility.CalculateTransformPath(gameObject.transform, this.state.activeRootGameObject.transform);
            TreeViewItem parentNode = new AddCurvesPopupGameObjectNode(gameObject, parent, gameObject.name);
            List<TreeViewItem> visibleItems = new List<TreeViewItem>();
            if (parent == null)
            {
                base.m_RootItem = parentNode;
            }
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, this.state.activeRootGameObject);
            List<EditorCurveBinding> list2 = new List<EditorCurveBinding>();
            for (int i = 0; i < animatableBindings.Length; i++)
            {
                EditorCurveBinding item = animatableBindings[i];
                list2.Add(item);
                if (item.propertyName == "m_IsActive")
                {
                    if (item.path != string.Empty)
                    {
                        TreeViewItem item2 = this.CreateNode(list2.ToArray(), parentNode);
                        if (item2 != null)
                        {
                            visibleItems.Add(item2);
                        }
                        list2.Clear();
                    }
                    else
                    {
                        list2.Clear();
                    }
                }
                else
                {
                    bool flag = i == (animatableBindings.Length - 1);
                    bool flag2 = false;
                    if (!flag)
                    {
                        flag2 = animatableBindings[i + 1].type != item.type;
                    }
                    if (AnimationWindowUtility.IsCurveCreated(this.state.activeAnimationClip, item))
                    {
                        list2.Remove(item);
                    }
                    if ((flag || flag2) && (list2.Count > 0))
                    {
                        visibleItems.Add(this.AddAnimatableObjectToHierarchy(this.state.activeRootGameObject, list2.ToArray(), parentNode, path));
                        list2.Clear();
                    }
                }
            }
            if (showEntireHierarchy)
            {
                for (int j = 0; j < gameObject.transform.childCount; j++)
                {
                    Transform child = gameObject.transform.GetChild(j);
                    TreeViewItem item3 = this.AddGameObjectToHierarchy(child.gameObject, parentNode);
                    if (item3 != null)
                    {
                        visibleItems.Add(item3);
                    }
                }
            }
            TreeViewUtility.SetChildParentReferences(visibleItems, parentNode);
            return parentNode;
        }

        private TreeViewItem CreateNode(EditorCurveBinding[] curveBindings, TreeViewItem parentNode)
        {
            AddCurvesPopupPropertyNode node = new AddCurvesPopupPropertyNode(parentNode, curveBindings);
            if (AnimationWindowUtility.IsRectTransformPosition(node.curveBindings[0]))
            {
                node.curveBindings = new EditorCurveBinding[] { node.curveBindings[2] };
            }
            node.icon = parentNode.icon;
            return node;
        }

        public override void FetchData()
        {
            if (AddCurvesPopup.gameObject != null)
            {
                this.AddGameObjectToHierarchy(AddCurvesPopup.gameObject, null);
                this.SetupRootNodeSettings();
                base.m_NeedRefreshVisibleFolders = true;
            }
        }

        private static string GetClassName(GameObject root, EditorCurveBinding binding)
        {
            Object animatedObject = AnimationUtility.GetAnimatedObject(root, binding);
            if (animatedObject != null)
            {
                return ObjectNames.GetInspectorTitle(animatedObject);
            }
            return binding.type.Name;
        }

        private void SetupRootNodeSettings()
        {
            base.showRootNode = false;
            this.SetExpanded(this.root, true);
        }

        public void UpdateData()
        {
            base.m_TreeView.ReloadData();
        }

        public static bool showEntireHierarchy
        {
            [CompilerGenerated]
            get
            {
                return <showEntireHierarchy>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <showEntireHierarchy>k__BackingField = value;
            }
        }

        private AnimationWindowState state { get; set; }
    }
}

