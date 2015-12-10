namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class ObjectTreeForSelector
    {
        private const float kBottomBarHeight = 17f;
        private const string kSearchFieldTag = "TreeSearchField";
        private const float kTopBarHeight = 27f;
        private DoubleClickedEvent m_DoubleClickedEvent;
        private int m_ErrorCounter;
        private bool m_FocusSearchFilter;
        private int m_LastSelectedID = -1;
        private int m_OriginalSelectedID;
        private EditorWindow m_Owner;
        private string m_SelectedPath = string.Empty;
        private SelectionEvent m_SelectionEvent;
        private TreeView m_TreeView;
        private TreeViewNeededEvent m_TreeViewNeededEvent;
        private TreeViewState m_TreeViewState;
        private int m_UserData;
        private static Styles s_Styles;

        private void BottomBar(Rect bottomRect)
        {
            int id = this.m_TreeView.GetSelection().FirstOrDefault<int>();
            if (id != this.m_LastSelectedID)
            {
                this.m_LastSelectedID = id;
                this.m_SelectedPath = string.Empty;
                TreeViewItem item = this.m_TreeView.FindNode(id);
                if (item != null)
                {
                    StringBuilder builder = new StringBuilder();
                    for (TreeViewItem item2 = item; (item2 != null) && (item2 != this.m_TreeView.data.root); item2 = item2.parent)
                    {
                        if (item2 != item)
                        {
                            builder.Insert(0, "/");
                        }
                        builder.Insert(0, item2.displayName);
                    }
                    this.m_SelectedPath = builder.ToString();
                }
            }
            GUI.Label(bottomRect, GUIContent.none, s_Styles.bottomBarBg);
            if (!string.IsNullOrEmpty(this.m_SelectedPath))
            {
                GUI.Label(bottomRect, GUIContent.Temp(this.m_SelectedPath), EditorStyles.miniLabel);
            }
        }

        public void Clear()
        {
            this.m_Owner = null;
            this.m_TreeViewNeededEvent = null;
            this.m_SelectionEvent = null;
            this.m_DoubleClickedEvent = null;
            this.m_OriginalSelectedID = 0;
            this.m_UserData = 0;
            this.m_TreeView = null;
            this.m_TreeViewState = null;
            this.m_ErrorCounter = 0;
            this.m_FocusSearchFilter = false;
        }

        private bool EnsureTreeViewIsValid(Rect treeViewRect)
        {
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            if (this.m_TreeView == null)
            {
                TreeSelectorData data = new TreeSelectorData {
                    state = this.m_TreeViewState,
                    treeViewRect = treeViewRect,
                    userData = this.m_UserData,
                    objectTreeForSelector = this,
                    editorWindow = this.m_Owner
                };
                this.m_TreeViewNeededEvent.Invoke(data);
                if ((this.m_TreeView != null) && (this.m_TreeView.data.root == null))
                {
                    this.m_TreeView.ReloadData();
                }
                if (this.m_TreeView == null)
                {
                    if (this.m_ErrorCounter == 0)
                    {
                        Debug.LogError("ObjectTreeSelector is missing its tree view. Ensure to call 'SetTreeView()' when the treeViewNeededCallback is invoked!");
                        this.m_ErrorCounter++;
                    }
                    return false;
                }
            }
            return true;
        }

        private void FireSelectionEvent(TreeViewItem item)
        {
            if (this.m_SelectionEvent != null)
            {
                this.m_SelectionEvent.Invoke(item);
            }
        }

        internal void FocusSearchField()
        {
            this.m_FocusSearchFilter = true;
        }

        private void FrameSelectedTreeViewItem()
        {
            this.m_TreeView.Frame(this.m_TreeView.state.lastClickedID, true, false);
        }

        public int[] GetSelection()
        {
            if (this.m_TreeView != null)
            {
                return this.m_TreeView.GetSelection();
            }
            return new int[0];
        }

        private Rect GetTreeViewRect(Rect position)
        {
            return new Rect(0f, 27f, position.width, (position.height - 17f) - 27f);
        }

        private void HandleCommandEvents()
        {
            Event current = Event.current;
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                if (current.commandName == "FrameSelected")
                {
                    if ((current.type == EventType.ExecuteCommand) && this.m_TreeView.HasSelection())
                    {
                        this.m_TreeView.searchString = string.Empty;
                        this.FrameSelectedTreeViewItem();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                if (current.commandName == "Find")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        this.FocusSearchField();
                    }
                    current.Use();
                }
            }
        }

        private void HandleKeyboard(int treeViewControlID)
        {
            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keyCode = Event.current.keyCode;
                if (((keyCode == KeyCode.UpArrow) || (keyCode == KeyCode.DownArrow)) && (GUI.GetNameOfFocusedControl() == "TreeSearchField"))
                {
                    GUIUtility.keyboardControl = treeViewControlID;
                    if (this.m_TreeView.IsLastClickedPartOfRows())
                    {
                        this.FrameSelectedTreeViewItem();
                    }
                    else
                    {
                        this.m_TreeView.OffsetSelection(1);
                    }
                    Event.current.Use();
                }
            }
        }

        public void Init(Rect position, EditorWindow owner, UnityAction<TreeSelectorData> treeViewNeededCallback, UnityAction<TreeViewItem> selectionCallback, UnityAction doubleClickedCallback, int initialSelectedTreeViewItemID, int userData)
        {
            this.Clear();
            this.m_Owner = owner;
            this.m_TreeViewNeededEvent = new TreeViewNeededEvent();
            this.m_TreeViewNeededEvent.AddPersistentListener(treeViewNeededCallback, UnityEventCallState.EditorAndRuntime);
            this.m_SelectionEvent = new SelectionEvent();
            this.m_SelectionEvent.AddPersistentListener(selectionCallback, UnityEventCallState.EditorAndRuntime);
            this.m_DoubleClickedEvent = new DoubleClickedEvent();
            this.m_DoubleClickedEvent.AddPersistentListener(doubleClickedCallback, UnityEventCallState.EditorAndRuntime);
            this.m_OriginalSelectedID = initialSelectedTreeViewItemID;
            this.m_UserData = userData;
            this.m_FocusSearchFilter = true;
            this.EnsureTreeViewIsValid(this.GetTreeViewRect(position));
            if (this.m_TreeView != null)
            {
                int[] selectedIDs = new int[] { this.m_OriginalSelectedID };
                this.m_TreeView.SetSelection(selectedIDs, true);
                if (this.m_OriginalSelectedID == 0)
                {
                    this.m_TreeView.data.SetExpandedWithChildren(this.m_TreeView.data.root, true);
                }
            }
        }

        public bool IsInitialized()
        {
            return (this.m_Owner != null);
        }

        public void OnGUI(Rect position)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Rect rect = new Rect(0f, 0f, position.width, position.height);
            Rect toolbarRect = new Rect(rect.x, rect.y, rect.width, 27f);
            Rect bottomRect = new Rect(rect.x, rect.yMax - 17f, rect.width, 17f);
            Rect treeViewRect = this.GetTreeViewRect(position);
            if (this.EnsureTreeViewIsValid(treeViewRect))
            {
                int controlID = GUIUtility.GetControlID("Tree".GetHashCode(), FocusType.Keyboard);
                this.HandleCommandEvents();
                this.HandleKeyboard(controlID);
                this.SearchArea(toolbarRect);
                this.TreeViewArea(treeViewRect, controlID);
                this.BottomBar(bottomRect);
            }
        }

        private void OnItemDoubleClicked(int id)
        {
            if (this.m_DoubleClickedEvent != null)
            {
                this.m_DoubleClickedEvent.Invoke();
            }
        }

        private void OnItemSelectionChanged(int[] selection)
        {
            if (this.m_SelectionEvent != null)
            {
                TreeViewItem item = null;
                if (selection.Length > 0)
                {
                    item = this.m_TreeView.FindNode(selection[0]);
                }
                this.FireSelectionEvent(item);
            }
        }

        private void SearchArea(Rect toolbarRect)
        {
            GUI.Label(toolbarRect, GUIContent.none, s_Styles.searchBg);
            bool flag = (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape);
            GUI.SetNextControlName("TreeSearchField");
            string str = EditorGUI.SearchField(new Rect(5f, 5f, toolbarRect.width - 10f, 15f), this.m_TreeView.searchString);
            if ((flag && (Event.current.type == EventType.Used)) && (this.m_TreeView.searchString != string.Empty))
            {
                this.m_FocusSearchFilter = true;
            }
            if ((str != this.m_TreeView.searchString) || this.m_FocusSearchFilter)
            {
                this.m_TreeView.searchString = str;
                HandleUtility.Repaint();
            }
            if (this.m_FocusSearchFilter)
            {
                EditorGUI.FocusTextInControl("TreeSearchField");
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_FocusSearchFilter = false;
                }
            }
        }

        public void SetTreeView(TreeView treeView)
        {
            this.m_TreeView = treeView;
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Remove(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnItemSelectionChanged));
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnItemSelectionChanged));
            this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Remove(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.OnItemDoubleClicked));
            this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.OnItemDoubleClicked));
        }

        private void TreeViewArea(Rect treeViewRect, int treeViewControlID)
        {
            if (this.m_TreeView.data.rowCount > 0)
            {
                this.m_TreeView.OnGUI(treeViewRect, treeViewControlID);
            }
        }

        [Serializable]
        public class DoubleClickedEvent : UnityEvent
        {
        }

        [Serializable]
        public class SelectionEvent : UnityEvent<TreeViewItem>
        {
        }

        private class Styles
        {
            public GUIStyle bottomBarBg = new GUIStyle("ProjectBrowserBottomBarBg");
            public GUIStyle searchBg = new GUIStyle("ProjectBrowserTopBarBg");

            public Styles()
            {
                this.searchBg.border = new RectOffset(0, 0, 2, 2);
                this.searchBg.fixedHeight = 0f;
                this.bottomBarBg.alignment = TextAnchor.MiddleLeft;
                this.bottomBarBg.fontSize = EditorStyles.label.fontSize;
                this.bottomBarBg.padding = new RectOffset(5, 5, 0, 0);
            }
        }

        internal class TreeSelectorData
        {
            public EditorWindow editorWindow;
            public ObjectTreeForSelector objectTreeForSelector;
            public TreeViewState state;
            public Rect treeViewRect;
            public int userData;
        }

        [Serializable]
        public class TreeViewNeededEvent : UnityEvent<ObjectTreeForSelector.TreeSelectorData>
        {
        }
    }
}

