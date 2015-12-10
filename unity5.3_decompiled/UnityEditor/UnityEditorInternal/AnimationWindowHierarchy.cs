namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowHierarchy
    {
        private AnimationWindowState m_State;
        private TreeView m_TreeView;

        public AnimationWindowHierarchy(AnimationWindowState state, EditorWindow owner, Rect position)
        {
            this.m_State = state;
            this.Init(owner, position);
        }

        public void Init(EditorWindow owner, Rect rect)
        {
            if (this.m_State.hierarchyState == null)
            {
                this.m_State.hierarchyState = new AnimationWindowHierarchyState();
            }
            this.m_TreeView = new TreeView(owner, this.m_State.hierarchyState);
            this.m_State.hierarchyData = new AnimationWindowHierarchyDataSource(this.m_TreeView, this.m_State);
            this.m_TreeView.Init(rect, this.m_State.hierarchyData, new AnimationWindowHierarchyGUI(this.m_TreeView, this.m_State), null);
            this.m_TreeView.deselectOnUnhandledMouseDown = true;
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.m_State.OnHierarchySelectionChanged));
            this.m_TreeView.ReloadData();
        }

        public bool IsIDVisible(int id)
        {
            if (this.m_TreeView == null)
            {
                return false;
            }
            return (TreeView.GetIndexOfID(this.m_TreeView.data.GetRows(), id) >= 0);
        }

        internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
        {
            return true;
        }

        public void OnGUI(Rect position)
        {
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(position, GUIUtility.GetControlID(FocusType.Keyboard));
        }
    }
}

