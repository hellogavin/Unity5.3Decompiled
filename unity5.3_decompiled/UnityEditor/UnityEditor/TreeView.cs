namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    internal class TreeView
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache1B;
        internal const string kExpansionAnimationPrefKey = "TreeViewExpansionAnimation";
        private const float kSpaceForScrollBar = 16f;
        private bool m_AllowRenameOnMouseUp = true;
        private List<int> m_DragSelection = new List<int>();
        private EditorWindow m_EditorWindow;
        private readonly TreeViewItemExpansionAnimator m_ExpansionAnimator = new TreeViewItemExpansionAnimator();
        private AnimFloat m_FramingAnimFloat;
        private bool m_GrabKeyboardFocus;
        private bool m_HadFocusLastEvent;
        private int m_KeyboardControlID;
        private bool m_StopIteratingItems;
        private Rect m_TotalRect;
        private bool m_UseExpansionAnimation = EditorPrefs.GetBool("TreeViewExpansionAnimation", false);
        private bool m_UseScrollView = true;

        public TreeView(EditorWindow editorWindow, TreeViewState treeViewState)
        {
            this.m_EditorWindow = editorWindow;
            this.state = treeViewState;
        }

        private void AnimatedScrollChanged()
        {
            this.Repaint();
            this.state.scrollPos.y = this.m_FramingAnimFloat.value;
        }

        public bool BeginNameEditing(float delay)
        {
            if (this.state.selectedIDs.Count == 0)
            {
                return false;
            }
            List<TreeViewItem> rows = this.data.GetRows();
            TreeViewItem item = null;
            <BeginNameEditing>c__AnonStorey75 storey = new <BeginNameEditing>c__AnonStorey75();
            using (List<int>.Enumerator enumerator = this.state.selectedIDs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    storey.id = enumerator.Current;
                    TreeViewItem item2 = rows.Find(new Predicate<TreeViewItem>(storey.<>m__110));
                    if (item == null)
                    {
                        item = item2;
                    }
                    else if (item2 != null)
                    {
                        return false;
                    }
                }
            }
            return (((item != null) && this.data.IsRenamingItemAllowed(item)) && this.gui.BeginRename(item, delay));
        }

        private void ChangeExpandedState(TreeViewItem item, bool expand)
        {
            if (Event.current.alt)
            {
                this.data.SetExpandedWithChildren(item, expand);
            }
            else
            {
                this.data.SetExpanded(item, expand);
            }
            if (expand)
            {
                this.UserExpandedNode(item);
            }
        }

        private void ChangeScrollValue(float targetScrollPos, bool animated)
        {
            if (this.m_UseExpansionAnimation && animated)
            {
                this.m_FramingAnimFloat.value = this.state.scrollPos.y;
                this.m_FramingAnimFloat.target = targetScrollPos;
                this.m_FramingAnimFloat.speed = 3f;
            }
            else
            {
                this.state.scrollPos.y = targetScrollPos;
            }
        }

        private void DoItemGUI(TreeViewItem item, int row, float rowWidth, bool hasFocus)
        {
            if ((row < 0) || (row >= this.data.rowCount))
            {
                Debug.LogError(string.Concat(new object[] { "Invalid. Org row: ", row, " Num rows ", this.data.rowCount }));
            }
            else
            {
                bool selected = this.IsItemDragSelectedOrSelected(item);
                Rect rowRect = this.gui.GetRowRect(row, rowWidth);
                if (this.animatingExpansion)
                {
                    rowRect = this.m_ExpansionAnimator.OnBeginRowGUI(row, rowRect);
                }
                if (this.animatingExpansion)
                {
                    this.m_ExpansionAnimator.OnRowGUI(row);
                }
                this.gui.OnRowGUI(rowRect, item, row, selected, hasFocus);
                if (this.animatingExpansion)
                {
                    this.m_ExpansionAnimator.OnEndRowGUI(row);
                }
                if (this.onGUIRowCallback != null)
                {
                    float contentIndent = this.gui.GetContentIndent(item);
                    Rect rect2 = new Rect(rowRect.x + contentIndent, rowRect.y, rowRect.width - contentIndent, rowRect.height);
                    this.onGUIRowCallback(item.id, rect2);
                }
                this.HandleUnusedMouseEventsForNode(rowRect, item, row == 0);
            }
        }

        public void EndNameEditing(bool acceptChanges)
        {
            if (this.state.renameOverlay.IsRenaming())
            {
                this.state.renameOverlay.EndRename(acceptChanges);
                this.gui.EndRename();
            }
        }

        public void EndPing()
        {
            this.gui.EndPingNode();
        }

        private void EnsureRowIsVisible(int row, bool animated)
        {
            if (row >= 0)
            {
                Rect rectForFraming = this.gui.GetRectForFraming(row);
                float y = rectForFraming.y;
                float targetScrollPos = rectForFraming.yMax - this.m_TotalRect.height;
                if (this.state.scrollPos.y < targetScrollPos)
                {
                    this.ChangeScrollValue(targetScrollPos, animated);
                }
                else if (this.state.scrollPos.y > y)
                {
                    this.ChangeScrollValue(y, animated);
                }
            }
        }

        private void ExpandedStateHasChanged()
        {
            this.m_StopIteratingItems = true;
        }

        private void ExpansionAnimationEnded(TreeViewAnimationInput setup)
        {
            if (!setup.expanding)
            {
                this.ChangeExpandedState(setup.item, false);
            }
        }

        public TreeViewItem FindNode(int id)
        {
            return this.data.FindItem(id);
        }

        public void Frame(int id, bool frame, bool ping)
        {
            this.Frame(id, frame, ping, false);
        }

        public void Frame(int id, bool frame, bool ping, bool animated)
        {
            float topPixelOfRow = -1f;
            if (frame)
            {
                this.data.RevealItem(id);
                int row = this.data.GetRow(id);
                if (row >= 0)
                {
                    topPixelOfRow = this.GetTopPixelOfRow(row);
                    this.EnsureRowIsVisible(row, animated);
                }
            }
            if (ping)
            {
                int num3 = this.data.GetRow(id);
                if ((topPixelOfRow == -1f) && (num3 >= 0))
                {
                    topPixelOfRow = this.GetTopPixelOfRow(num3);
                }
                if (((topPixelOfRow >= 0f) && (num3 >= 0)) && (num3 < this.data.rowCount))
                {
                    TreeViewItem item = this.data.GetItem(num3);
                    float num4 = (this.GetContentSize().y <= this.m_TotalRect.height) ? 0f : -16f;
                    this.gui.BeginPingNode(item, topPixelOfRow, this.m_TotalRect.width + num4);
                }
            }
        }

        private float GetAnimationDuration(float height)
        {
            return ((height <= 60f) ? ((height * 0.1f) / 60f) : 0.1f);
        }

        public Vector2 GetContentSize()
        {
            return this.gui.GetTotalSize();
        }

        private bool GetFirstAndLastSelected(List<TreeViewItem> items, out int firstIndex, out int lastIndex)
        {
            firstIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (this.state.selectedIDs.Contains(items[i].id))
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }
                    lastIndex = i;
                }
            }
            return ((firstIndex != -1) && (lastIndex != -1));
        }

        internal static int GetIndexOfID(List<TreeViewItem> items, int id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        private TreeViewItem GetItemAndRowIndex(int id, out int row)
        {
            row = this.data.GetRow(id);
            if (row == -1)
            {
                return null;
            }
            return this.data.GetItem(row);
        }

        internal static int GetItemControlID(TreeViewItem item)
        {
            return (((item == null) ? 0 : item.id) + 0x989680);
        }

        private int GetLastChildRowUnder(int row)
        {
            List<TreeViewItem> rows = this.data.GetRows();
            int depth = rows[row].depth;
            for (int i = row + 1; i < rows.Count; i++)
            {
                if (rows[i].depth <= depth)
                {
                    return (i - 1);
                }
            }
            return (rows.Count - 1);
        }

        private List<int> GetNewSelection(TreeViewItem clickedItem, bool keepMultiSelection, bool useShiftAsActionKey)
        {
            List<TreeViewItem> rows = this.data.GetRows();
            List<int> allInstanceIDs = new List<int>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                allInstanceIDs.Add(rows[i].id);
            }
            List<int> selectedIDs = this.state.selectedIDs;
            int lastClickedID = this.state.lastClickedID;
            bool allowMultiSelection = this.data.CanBeMultiSelected(clickedItem);
            return InternalEditorUtility.GetNewSelection(clickedItem.id, allInstanceIDs, selectedIDs, lastClickedID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
        }

        protected virtual Rect GetRectForRows(int startRow, int endRow, float rowWidth)
        {
            Rect rowRect = this.gui.GetRowRect(startRow, rowWidth);
            Rect rect2 = this.gui.GetRowRect(endRow, rowWidth);
            return new Rect(rowRect.x, rowRect.y, rowWidth, rect2.yMax - rowRect.yMin);
        }

        public int[] GetRowIDs()
        {
            if (<>f__am$cache1B == null)
            {
                <>f__am$cache1B = item => item.id;
            }
            return this.data.GetRows().Select<TreeViewItem, int>(<>f__am$cache1B).ToArray<int>();
        }

        public int[] GetSelection()
        {
            return this.state.selectedIDs.ToArray();
        }

        private float GetTopPixelOfRow(int row)
        {
            return this.gui.GetRowRect(row, 1f).y;
        }

        public Rect GetTotalRect()
        {
            return this.m_TotalRect;
        }

        private List<int> GetVisibleSelectedIds()
        {
            int num;
            int num2;
            this.gui.GetFirstAndLastRowVisible(out num, out num2);
            if (num2 < 0)
            {
                return new List<int>();
            }
            List<int> list = new List<int>(num2 - num);
            for (int i = num; i < num2; i++)
            {
                TreeViewItem item = this.data.GetItem(i);
                list.Add(item.id);
            }
            return (from id in list
                where this.state.selectedIDs.Contains(id)
                select id).ToList<int>();
        }

        public void GrabKeyboardFocus()
        {
            this.m_GrabKeyboardFocus = true;
        }

        private void HandleUnusedEvents()
        {
            EventType type = Event.current.type;
            switch (type)
            {
                case EventType.DragUpdated:
                    if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                    {
                        this.dragging.DragElement(null, new Rect(), false);
                        this.Repaint();
                        Event.current.Use();
                    }
                    return;

                case EventType.DragPerform:
                    if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                    {
                        this.m_DragSelection.Clear();
                        this.dragging.DragElement(null, new Rect(), false);
                        this.Repaint();
                        Event.current.Use();
                    }
                    return;

                case EventType.DragExited:
                    if (this.dragging != null)
                    {
                        this.m_DragSelection.Clear();
                        this.dragging.DragCleanup(true);
                        this.Repaint();
                    }
                    return;

                case EventType.ContextClick:
                    if (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.contextClickOutsideItemsCallback != null))
                    {
                        this.contextClickOutsideItemsCallback();
                    }
                    return;
            }
            if ((type == EventType.MouseDown) && ((this.deselectOnUnhandledMouseDown && (Event.current.button == 0)) && (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.state.selectedIDs.Count > 0))))
            {
                this.SetSelection(new int[0], false);
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void HandleUnusedMouseEventsForNode(Rect rect, TreeViewItem item, bool firstItem)
        {
            int itemControlID = GetItemControlID(item);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(itemControlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (!rect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    if (Event.current.button != 0)
                    {
                        if (Event.current.button == 1)
                        {
                            bool keepMultiSelection = true;
                            this.SelectionClick(item, keepMultiSelection);
                        }
                        return;
                    }
                    GUIUtility.keyboardControl = this.m_KeyboardControlID;
                    this.Repaint();
                    if (Event.current.clickCount != 2)
                    {
                        if ((this.dragging == null) || this.dragging.CanStartDrag(item, this.m_DragSelection, Event.current.mousePosition))
                        {
                            this.m_DragSelection = this.GetNewSelection(item, true, false);
                            DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                            stateObject.mouseDownPosition = Event.current.mousePosition;
                        }
                        GUIUtility.hotControl = itemControlID;
                    }
                    else if (this.itemDoubleClickedCallback != null)
                    {
                        this.itemDoubleClickedCallback(item.id);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == itemControlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.m_DragSelection.Clear();
                        current.Use();
                        if (rect.Contains(current.mousePosition))
                        {
                            float contentIndent = this.gui.GetContentIndent(item);
                            Rect rect2 = new Rect(rect.x + contentIndent, rect.y, rect.width - contentIndent, rect.height);
                            List<int> selectedIDs = this.state.selectedIDs;
                            if (((!this.m_AllowRenameOnMouseUp || (selectedIDs == null)) || ((selectedIDs.Count != 1) || (selectedIDs[0] != item.id))) || (!rect2.Contains(current.mousePosition) || EditorGUIUtility.HasHolddownKeyModifiers(current)))
                            {
                                this.SelectionClick(item, false);
                                return;
                            }
                            this.BeginNameEditing(0.5f);
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == itemControlID) && (this.dragging != null))
                    {
                        DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                        if (delay2.CanStartDrag() && this.dragging.CanStartDrag(item, this.m_DragSelection, delay2.mouseDownPosition))
                        {
                            this.dragging.StartDrag(item, this.m_DragSelection);
                            GUIUtility.hotControl = 0;
                        }
                        current.Use();
                    }
                    return;

                default:
                    switch (typeForControl)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if ((this.dragging != null) && this.dragging.DragElement(item, rect, firstItem))
                            {
                                GUIUtility.hotControl = 0;
                            }
                            return;

                        case EventType.ContextClick:
                            if (rect.Contains(current.mousePosition) && (this.contextClickItemCallback != null))
                            {
                                this.contextClickItemCallback(item.id);
                            }
                            return;

                        default:
                            return;
                    }
                    break;
            }
            current.Use();
        }

        public bool HasFocus()
        {
            return ((this.m_EditorWindow != null) && (this.m_EditorWindow.m_Parent.hasFocus && (GUIUtility.keyboardControl == this.m_KeyboardControlID)));
        }

        public bool HasSelection()
        {
            return (this.state.selectedIDs.Count<int>() > 0);
        }

        public void Init(Rect rect, ITreeViewDataSource data, ITreeViewGUI gui, ITreeViewDragging dragging)
        {
            this.data = data;
            this.gui = gui;
            this.dragging = dragging;
            this.m_TotalRect = rect;
            data.OnInitialize();
            gui.OnInitialize();
            if (dragging != null)
            {
                dragging.OnInitialize();
            }
            this.expandedStateChanged = (Action) Delegate.Combine(this.expandedStateChanged, new Action(this.ExpandedStateHasChanged));
            this.m_FramingAnimFloat = new AnimFloat(this.state.scrollPos.y, new UnityAction(this.AnimatedScrollChanged));
        }

        public bool IsItemDragSelectedOrSelected(TreeViewItem item)
        {
            return ((this.m_DragSelection.Count <= 0) ? this.state.selectedIDs.Contains(item.id) : this.m_DragSelection.Contains(item.id));
        }

        public bool IsLastClickedPartOfRows()
        {
            List<TreeViewItem> rows = this.data.GetRows();
            if (rows.Count == 0)
            {
                return false;
            }
            return (GetIndexOfID(rows, this.state.lastClickedID) >= 0);
        }

        public bool IsSelected(int id)
        {
            return this.state.selectedIDs.Contains(id);
        }

        private void IterateVisibleItems(int firstRow, int numVisibleRows, float rowWidth, bool hasFocus)
        {
            this.m_StopIteratingItems = false;
            int num = 0;
            for (int i = 0; i < numVisibleRows; i++)
            {
                int row = firstRow + i;
                if (this.animatingExpansion)
                {
                    int endRow = this.m_ExpansionAnimator.endRow;
                    if (this.m_ExpansionAnimator.CullRow(row, this.gui))
                    {
                        num++;
                        row = endRow + num;
                    }
                    else
                    {
                        row += num;
                    }
                    if (row < this.data.rowCount)
                    {
                        goto Label_00AE;
                    }
                    continue;
                }
                float num5 = this.gui.GetRowRect(row, rowWidth).y - this.state.scrollPos.y;
                if (num5 > this.m_TotalRect.height)
                {
                    continue;
                }
            Label_00AE:
                this.DoItemGUI(this.data.GetItem(row), row, rowWidth, hasFocus);
                if (this.m_StopIteratingItems)
                {
                    return;
                }
            }
        }

        private void KeyboardGUI()
        {
            if ((this.m_KeyboardControlID == GUIUtility.keyboardControl) && GUI.enabled)
            {
                if (this.keyboardInputCallback != null)
                {
                    this.keyboardInputCallback();
                }
                if (Event.current.type == EventType.KeyDown)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            if ((Application.platform == RuntimePlatform.OSXEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;

                        case KeyCode.UpArrow:
                            Event.current.Use();
                            this.OffsetSelection(-1);
                            return;

                        case KeyCode.DownArrow:
                            Event.current.Use();
                            this.OffsetSelection(1);
                            return;

                        case KeyCode.RightArrow:
                            foreach (int num3 in this.state.selectedIDs)
                            {
                                int num4;
                                TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(num3, out num4);
                                if (itemAndRowIndex != null)
                                {
                                    if (this.data.IsExpandable(itemAndRowIndex) && !this.data.IsExpanded(itemAndRowIndex))
                                    {
                                        this.UserInputChangedExpandedState(itemAndRowIndex, num4, true);
                                    }
                                    else if (this.state.selectedIDs.Count == 1)
                                    {
                                        this.OffsetSelection(1);
                                    }
                                }
                            }
                            Event.current.Use();
                            return;

                        case KeyCode.LeftArrow:
                            foreach (int num in this.state.selectedIDs)
                            {
                                int num2;
                                TreeViewItem item = this.GetItemAndRowIndex(num, out num2);
                                if (item != null)
                                {
                                    if (this.data.IsExpandable(item) && this.data.IsExpanded(item))
                                    {
                                        this.UserInputChangedExpandedState(item, num2, false);
                                    }
                                    else if (this.state.selectedIDs.Count == 1)
                                    {
                                        this.OffsetSelection(-1);
                                    }
                                }
                            }
                            Event.current.Use();
                            return;

                        case KeyCode.Home:
                            Event.current.Use();
                            this.OffsetSelection(-1000000);
                            return;

                        case KeyCode.End:
                            Event.current.Use();
                            this.OffsetSelection(0xf4240);
                            return;

                        case KeyCode.PageUp:
                        {
                            Event.current.Use();
                            TreeViewItem fromItem = this.data.FindItem(this.state.lastClickedID);
                            if (fromItem != null)
                            {
                                int num5 = this.gui.GetNumRowsOnPageUpDown(fromItem, true, this.m_TotalRect.height);
                                this.OffsetSelection(-num5);
                            }
                            return;
                        }
                        case KeyCode.PageDown:
                        {
                            Event.current.Use();
                            TreeViewItem item4 = this.data.FindItem(this.state.lastClickedID);
                            if (item4 != null)
                            {
                                int offset = this.gui.GetNumRowsOnPageUpDown(item4, true, this.m_TotalRect.height);
                                this.OffsetSelection(offset);
                            }
                            return;
                        }
                        case KeyCode.F2:
                            if ((Application.platform == RuntimePlatform.WindowsEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;
                    }
                    if ((Event.current.keyCode > KeyCode.A) && (Event.current.keyCode < KeyCode.Z))
                    {
                    }
                }
            }
        }

        public void NotifyListenersThatDragEnded(int[] draggedIDs, bool draggedItemsFromOwnTreeView)
        {
            if (this.dragEndedCallback != null)
            {
                this.dragEndedCallback(draggedIDs, draggedItemsFromOwnTreeView);
            }
        }

        public void NotifyListenersThatSelectionChanged()
        {
            if (this.selectionChangedCallback != null)
            {
                this.selectionChangedCallback(this.state.selectedIDs.ToArray());
            }
        }

        public void OffsetSelection(int offset)
        {
            List<TreeViewItem> rows = this.data.GetRows();
            if (rows.Count != 0)
            {
                Event.current.Use();
                int row = Mathf.Clamp(GetIndexOfID(rows, this.state.lastClickedID) + offset, 0, rows.Count - 1);
                this.EnsureRowIsVisible(row, true);
                this.SelectionByKey(rows[row]);
            }
        }

        public void OnEvent()
        {
            this.state.renameOverlay.OnEvent();
        }

        public void OnGUI(Rect rect, int keyboardControlID)
        {
            int num;
            int num2;
            this.m_TotalRect = rect;
            this.m_KeyboardControlID = keyboardControlID;
            Event current = Event.current;
            if (this.m_GrabKeyboardFocus || ((current.type == EventType.MouseDown) && this.m_TotalRect.Contains(current.mousePosition)))
            {
                this.m_GrabKeyboardFocus = false;
                GUIUtility.keyboardControl = this.m_KeyboardControlID;
                this.m_AllowRenameOnMouseUp = true;
                this.Repaint();
            }
            bool hasFocus = this.HasFocus();
            if ((hasFocus != this.m_HadFocusLastEvent) && (current.type != EventType.Layout))
            {
                this.m_HadFocusLastEvent = hasFocus;
                if (hasFocus && (current.type == EventType.MouseDown))
                {
                    this.m_AllowRenameOnMouseUp = false;
                }
            }
            if (this.animatingExpansion)
            {
                this.m_ExpansionAnimator.OnBeforeAllRowsGUI();
            }
            this.data.InitIfNeeded();
            Vector2 totalSize = this.gui.GetTotalSize();
            Rect viewRect = new Rect(0f, 0f, totalSize.x, totalSize.y);
            if (this.m_UseScrollView)
            {
                this.state.scrollPos = GUI.BeginScrollView(this.m_TotalRect, this.state.scrollPos, viewRect);
            }
            else
            {
                GUI.BeginClip(this.m_TotalRect);
            }
            this.gui.BeginRowGUI();
            this.gui.GetFirstAndLastRowVisible(out num, out num2);
            if (num2 >= 0)
            {
                int numVisibleRows = (num2 - num) + 1;
                float rowWidth = Mathf.Max(GUIClip.visibleRect.width, viewRect.width);
                this.IterateVisibleItems(num, numVisibleRows, rowWidth, hasFocus);
            }
            if (this.animatingExpansion)
            {
                this.m_ExpansionAnimator.OnAfterAllRowsGUI();
            }
            this.gui.EndRowGUI();
            if (this.m_UseScrollView)
            {
                GUI.EndScrollView();
            }
            else
            {
                GUI.EndClip();
            }
            this.HandleUnusedEvents();
            this.KeyboardGUI();
        }

        public void ReloadData()
        {
            this.data.ReloadData();
            this.Repaint();
            this.m_StopIteratingItems = true;
        }

        public void RemoveSelection()
        {
            if (this.state.selectedIDs.Count > 0)
            {
                this.state.selectedIDs.Clear();
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void Repaint()
        {
            if (this.m_EditorWindow != null)
            {
                this.m_EditorWindow.Repaint();
            }
        }

        private void SelectionByKey(TreeViewItem itemSelected)
        {
            this.state.selectedIDs = this.GetNewSelection(itemSelected, false, true);
            this.state.lastClickedID = itemSelected.id;
            this.NotifyListenersThatSelectionChanged();
        }

        public void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
        {
            this.state.selectedIDs = this.GetNewSelection(itemClicked, keepMultiSelection, false);
            this.state.lastClickedID = (itemClicked == null) ? 0 : itemClicked.id;
            this.NotifyListenersThatSelectionChanged();
        }

        public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected)
        {
            this.SetSelection(selectedIDs, revealSelectionAndFrameLastSelected, false);
        }

        public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected, bool animatedFraming)
        {
            if (selectedIDs.Length > 0)
            {
                if (revealSelectionAndFrameLastSelected)
                {
                    foreach (int num in selectedIDs)
                    {
                        this.data.RevealItem(num);
                    }
                }
                this.state.selectedIDs = new List<int>(selectedIDs);
                bool flag = this.state.selectedIDs.IndexOf(this.state.lastClickedID) >= 0;
                if (!flag)
                {
                    int id = selectedIDs.Last<int>();
                    if (this.data.GetRow(id) != -1)
                    {
                        this.state.lastClickedID = id;
                        flag = true;
                    }
                    else
                    {
                        this.state.lastClickedID = 0;
                    }
                }
                if (revealSelectionAndFrameLastSelected && flag)
                {
                    this.Frame(this.state.lastClickedID, true, false, animatedFraming);
                }
            }
            else
            {
                this.state.selectedIDs.Clear();
                this.state.lastClickedID = 0;
            }
        }

        public void SetUseScrollView(bool useScrollView)
        {
            this.m_UseScrollView = useScrollView;
        }

        public List<int> SortIDsInVisiblityOrder(List<int> ids)
        {
            if (ids.Count <= 1)
            {
                return ids;
            }
            List<TreeViewItem> rows = this.data.GetRows();
            List<int> second = new List<int>();
            for (int i = 0; i < rows.Count; i++)
            {
                int id = rows[i].id;
                for (int j = 0; j < ids.Count; j++)
                {
                    if (ids[j] == id)
                    {
                        second.Add(id);
                        break;
                    }
                }
            }
            if (ids.Count != second.Count)
            {
                second.AddRange(ids.Except<int>(second));
                if (ids.Count != second.Count)
                {
                    Debug.LogError(string.Concat(new object[] { "SortIDsInVisiblityOrder failed: ", ids.Count, " != ", second.Count }));
                }
            }
            return second;
        }

        public void UserExpandedNode(TreeViewItem item)
        {
        }

        public void UserInputChangedExpandedState(TreeViewItem item, int row, bool expand)
        {
            if (this.useExpansionAnimation)
            {
                if (expand)
                {
                    this.ChangeExpandedState(item, true);
                }
                int startRow = row + 1;
                int lastChildRowUnder = this.GetLastChildRowUnder(row);
                float width = GUIClip.visibleRect.width;
                Rect rect = this.GetRectForRows(startRow, lastChildRowUnder, width);
                float animationDuration = this.GetAnimationDuration(rect.height);
                TreeViewAnimationInput setup = new TreeViewAnimationInput {
                    animationDuration = animationDuration,
                    startRow = startRow,
                    endRow = lastChildRowUnder,
                    startRowRect = this.gui.GetRowRect(startRow, width),
                    rowsRect = rect,
                    startTime = EditorApplication.timeSinceStartup,
                    expanding = expand,
                    animationEnded = new Action<TreeViewAnimationInput>(this.ExpansionAnimationEnded),
                    item = item,
                    treeView = this
                };
                this.expansionAnimator.BeginAnimating(setup);
            }
            else
            {
                this.ChangeExpandedState(item, expand);
            }
        }

        private bool animatingExpansion
        {
            get
            {
                return (this.m_UseExpansionAnimation && this.m_ExpansionAnimator.isAnimating);
            }
        }

        public Action<int> contextClickItemCallback { get; set; }

        public Action contextClickOutsideItemsCallback { get; set; }

        public ITreeViewDataSource data { get; set; }

        public bool deselectOnUnhandledMouseDown { get; set; }

        public Action<int[], bool> dragEndedCallback { get; set; }

        public ITreeViewDragging dragging { get; set; }

        public Action expandedStateChanged { get; set; }

        public TreeViewItemExpansionAnimator expansionAnimator
        {
            get
            {
                return this.m_ExpansionAnimator;
            }
        }

        public ITreeViewGUI gui { get; set; }

        public bool isSearching
        {
            get
            {
                return !string.IsNullOrEmpty(this.state.searchString);
            }
        }

        public Action<int> itemDoubleClickedCallback { get; set; }

        public Action keyboardInputCallback { get; set; }

        public Action<int, Rect> onGUIRowCallback { get; set; }

        public Action<Vector2> scrollChanged { get; set; }

        public Action<string> searchChanged { get; set; }

        public string searchString
        {
            get
            {
                return this.state.searchString;
            }
            set
            {
                this.state.searchString = value;
                this.data.OnSearchChanged();
                if (this.searchChanged != null)
                {
                    this.searchChanged(this.state.searchString);
                }
            }
        }

        public Action<int[]> selectionChangedCallback { get; set; }

        public TreeViewState state { get; set; }

        public bool useExpansionAnimation
        {
            get
            {
                return this.m_UseExpansionAnimation;
            }
            set
            {
                this.m_UseExpansionAnimation = value;
            }
        }

        [CompilerGenerated]
        private sealed class <BeginNameEditing>c__AnonStorey75
        {
            internal int id;

            internal bool <>m__110(TreeViewItem i)
            {
                return (i.id == this.id);
            }
        }
    }
}

