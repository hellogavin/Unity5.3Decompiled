namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEditor;
    using UnityEngine;

    internal class FrameDebuggerTreeView
    {
        internal FDTreeViewDataSource m_DataSource;
        private readonly FrameDebuggerWindow m_FrameDebugger;
        internal readonly TreeView m_TreeView;

        public FrameDebuggerTreeView(FrameDebuggerEvent[] frameEvents, TreeViewState treeViewState, FrameDebuggerWindow window, Rect startRect)
        {
            this.m_FrameDebugger = window;
            this.m_TreeView = new TreeView(window, treeViewState);
            this.m_DataSource = new FDTreeViewDataSource(this.m_TreeView, frameEvents);
            FDTreeViewGUI gui = new FDTreeViewGUI(this.m_TreeView);
            this.m_TreeView.Init(startRect, this.m_DataSource, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeView.OnGUI(rect, controlID);
        }

        public void SelectFrameEventIndex(int eventIndex)
        {
            int[] selection = this.m_TreeView.GetSelection();
            if (selection.Length > 0)
            {
                FDTreeViewItem item = this.m_TreeView.FindNode(selection[0]) as FDTreeViewItem;
                if ((item != null) && (eventIndex == item.m_EventIndex))
                {
                    return;
                }
            }
            int[] selectedIDs = new int[] { eventIndex };
            this.m_TreeView.SetSelection(selectedIDs, true);
        }

        private void SelectionChanged(int[] selectedIDs)
        {
            if (selectedIDs.Length >= 1)
            {
                int id = selectedIDs[0];
                int newLimit = id;
                if (newLimit <= 0)
                {
                    FDTreeViewItem item = this.m_TreeView.FindNode(id) as FDTreeViewItem;
                    if (item != null)
                    {
                        newLimit = item.m_EventIndex;
                    }
                }
                if (newLimit > 0)
                {
                    this.m_FrameDebugger.ChangeFrameEventLimit(newLimit);
                }
            }
        }

        internal class FDTreeViewDataSource : TreeViewDataSource
        {
            private FrameDebuggerEvent[] m_FrameEvents;

            public FDTreeViewDataSource(TreeView treeView, FrameDebuggerEvent[] frameEvents) : base(treeView)
            {
                this.m_FrameEvents = frameEvents;
                base.rootIsCollapsable = false;
                base.showRootNode = false;
            }

            public override bool CanBeMultiSelected(TreeViewItem item)
            {
                return false;
            }

            private static void CloseLastHierarchyLevel(List<FDTreeHierarchyLevel> eventStack, int prevFrameEventIndex)
            {
                int index = eventStack.Count - 1;
                eventStack[index].item.children = eventStack[index].children;
                eventStack[index].item.m_EventIndex = prevFrameEventIndex;
                if (eventStack[index].item.parent != null)
                {
                    FrameDebuggerTreeView.FDTreeViewItem parent = (FrameDebuggerTreeView.FDTreeViewItem) eventStack[index].item.parent;
                    parent.m_ChildEventCount += eventStack[index].item.m_ChildEventCount;
                }
                eventStack.RemoveAt(index);
            }

            public override void FetchData()
            {
                FDTreeHierarchyLevel level = new FDTreeHierarchyLevel(0, 0, string.Empty, null);
                List<FDTreeHierarchyLevel> eventStack = new List<FDTreeHierarchyLevel> {
                    level
                };
                int num = -1;
                for (int i = 0; i < this.m_FrameEvents.Length; i++)
                {
                    string frameEventInfoName = FrameDebuggerUtility.GetFrameEventInfoName(i);
                    if (frameEventInfoName == null)
                    {
                    }
                    char[] separator = new char[] { '/' };
                    string[] strArray = ("/" + string.Empty).Split(separator);
                    int index = 0;
                    while ((index < eventStack.Count) && (index < strArray.Length))
                    {
                        if (strArray[index] != eventStack[index].item.displayName)
                        {
                            break;
                        }
                        index++;
                    }
                    while ((eventStack.Count > 0) && (eventStack.Count > index))
                    {
                        CloseLastHierarchyLevel(eventStack, i);
                    }
                    for (int j = index; j < strArray.Length; j++)
                    {
                        FDTreeHierarchyLevel level2 = eventStack[eventStack.Count - 1];
                        FDTreeHierarchyLevel level3 = new FDTreeHierarchyLevel(eventStack.Count - 1, --num, strArray[j], level2.item);
                        level2.children.Add(level3.item);
                        eventStack.Add(level3);
                    }
                    GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(i);
                    string displayName = (frameEventGameObject == null) ? string.Empty : (" " + frameEventGameObject.name);
                    FDTreeHierarchyLevel level4 = eventStack[eventStack.Count - 1];
                    int id = i + 1;
                    FrameDebuggerTreeView.FDTreeViewItem item = new FrameDebuggerTreeView.FDTreeViewItem(id, eventStack.Count - 1, level4.item, displayName) {
                        m_FrameEvent = this.m_FrameEvents[i]
                    };
                    level4.children.Add(item);
                    level4.item.m_ChildEventCount++;
                }
                while (eventStack.Count > 0)
                {
                    CloseLastHierarchyLevel(eventStack, this.m_FrameEvents.Length);
                }
                base.m_RootItem = level.item;
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }

            public void SetEvents(FrameDebuggerEvent[] frameEvents)
            {
                bool flag = (this.m_FrameEvents == null) || (this.m_FrameEvents.Length < 1);
                this.m_FrameEvents = frameEvents;
                base.m_NeedRefreshVisibleFolders = true;
                this.ReloadData();
                if (flag)
                {
                    this.SetExpandedWithChildren(base.m_RootItem, true);
                }
            }

            private class FDTreeHierarchyLevel
            {
                internal readonly List<TreeViewItem> children;
                internal readonly FrameDebuggerTreeView.FDTreeViewItem item;

                internal FDTreeHierarchyLevel(int depth, int id, string name, FrameDebuggerTreeView.FDTreeViewItem parent)
                {
                    this.item = new FrameDebuggerTreeView.FDTreeViewItem(id, depth, parent, name);
                    this.children = new List<TreeViewItem>();
                }
            }
        }

        private class FDTreeViewGUI : TreeViewGUI
        {
            private const float kSmallMargin = 4f;

            public FDTreeViewGUI(TreeView treeView) : base(treeView)
            {
            }

            protected override void DrawIconAndLabel(Rect rect, TreeViewItem itemRaw, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                string displayName;
                GUIContent content;
                FrameDebuggerTreeView.FDTreeViewItem item = (FrameDebuggerTreeView.FDTreeViewItem) itemRaw;
                float contentIndent = this.GetContentIndent(item);
                rect.x += contentIndent;
                rect.width -= contentIndent;
                if (item.m_ChildEventCount > 0)
                {
                    Rect position = rect;
                    position.width -= 4f;
                    content = EditorGUIUtility.TempContent(item.m_ChildEventCount.ToString(CultureInfo.InvariantCulture));
                    GUIStyle rowTextRight = FrameDebuggerWindow.styles.rowTextRight;
                    rowTextRight.Draw(position, content, false, false, false, false);
                    rect.width -= rowTextRight.CalcSize(content).x + 8f;
                }
                if (item.id <= 0)
                {
                    displayName = item.displayName;
                }
                else
                {
                    displayName = FrameDebuggerWindow.s_FrameEventTypeNames[(int) item.m_FrameEvent.type] + item.displayName;
                }
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = "<unknown scope>";
                }
                content = EditorGUIUtility.TempContent(displayName);
                FrameDebuggerWindow.styles.rowText.Draw(rect, content, false, false, false, selected && focused);
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                return null;
            }

            protected override void RenameEnded()
            {
            }
        }

        private class FDTreeViewItem : TreeViewItem
        {
            public int m_ChildEventCount;
            public int m_EventIndex;
            public FrameDebuggerEvent m_FrameEvent;

            public FDTreeViewItem(int id, int depth, FrameDebuggerTreeView.FDTreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
                this.m_EventIndex = id;
            }
        }
    }
}

