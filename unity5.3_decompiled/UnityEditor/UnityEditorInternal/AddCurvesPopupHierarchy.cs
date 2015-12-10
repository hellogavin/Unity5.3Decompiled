namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AddCurvesPopupHierarchy
    {
        private TreeView m_TreeView;
        private AddCurvesPopupHierarchyDataSource m_TreeViewDataSource;
        private TreeViewState m_TreeViewState;

        public AddCurvesPopupHierarchy(AnimationWindowState state)
        {
            this.state = state;
        }

        public void InitIfNeeded(EditorWindow owner, Rect rect)
        {
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            else
            {
                return;
            }
            this.m_TreeView = new TreeView(owner, this.m_TreeViewState);
            this.m_TreeView.deselectOnUnhandledMouseDown = true;
            this.m_TreeViewDataSource = new AddCurvesPopupHierarchyDataSource(this.m_TreeView, this.state);
            TreeViewGUI gui = new AddCurvesPopupHierarchyGUI(this.m_TreeView, this.state, owner);
            this.m_TreeView.Init(rect, this.m_TreeViewDataSource, gui, null);
            this.m_TreeViewDataSource.UpdateData();
        }

        internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
        {
            return false;
        }

        public void OnGUI(Rect position, EditorWindow owner)
        {
            this.InitIfNeeded(owner, position);
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(position, GUIUtility.GetControlID(FocusType.Keyboard));
        }

        private AnimationWindowState state { get; set; }
    }
}

