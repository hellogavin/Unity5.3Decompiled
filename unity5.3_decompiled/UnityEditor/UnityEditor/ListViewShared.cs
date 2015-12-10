namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ListViewShared
    {
        internal static int dragControlID = -1;
        public static bool OSX = (Application.platform == RuntimePlatform.OSXEditor);

        private static bool DoLVPageUpDown(InternalListViewState ilvState, ref int selectedRow, ref Vector2 scrollPos, bool up)
        {
            int num = ilvState.endRow - ilvState.invisibleRows;
            if (up)
            {
                if (!OSX)
                {
                    selectedRow -= num;
                    if (selectedRow < 0)
                    {
                        selectedRow = 0;
                    }
                    return true;
                }
                scrollPos.y -= ilvState.state.rowHeight * num;
                if (scrollPos.y < 0f)
                {
                    scrollPos.y = 0f;
                }
            }
            else if (OSX)
            {
                scrollPos.y += ilvState.state.rowHeight * num;
            }
            else
            {
                selectedRow += num;
                if (selectedRow >= ilvState.state.totalRows)
                {
                    selectedRow = ilvState.state.totalRows - 1;
                }
                return true;
            }
            return false;
        }

        internal static bool HasMouseDown(InternalListViewState ilvState, Rect r)
        {
            return HasMouseDown(ilvState, r, 0);
        }

        internal static bool HasMouseDown(InternalListViewState ilvState, Rect r, int button)
        {
            if (((Event.current.type == EventType.MouseDown) && (Event.current.button == button)) && r.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = ilvState.state.ID;
                GUIUtility.keyboardControl = ilvState.state.ID;
                Event.current.Use();
                return true;
            }
            return false;
        }

        internal static bool HasMouseUp(InternalListViewState ilvState, Rect r)
        {
            return HasMouseUp(ilvState, r, 0);
        }

        internal static bool HasMouseUp(InternalListViewState ilvState, Rect r, int button)
        {
            if (((Event.current.type == EventType.MouseUp) && (Event.current.button == button)) && r.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
                return true;
            }
            return false;
        }

        internal static bool ListViewKeyboard(InternalListViewState ilvState, int totalCols)
        {
            int totalRows = ilvState.state.totalRows;
            if ((Event.current.type != EventType.KeyDown) || (totalRows == 0))
            {
                return false;
            }
            return (((GUIUtility.keyboardControl == ilvState.state.ID) && (Event.current.GetTypeForControl(ilvState.state.ID) == EventType.KeyDown)) && SendKey(ilvState, Event.current.keyCode, totalCols));
        }

        internal static Vector2 ListViewScrollToRow(InternalListViewState ilvState, int row)
        {
            return ListViewScrollToRow(ilvState, ilvState.state.scrollPos, row);
        }

        internal static int ListViewScrollToRow(InternalListViewState ilvState, int currPosY, int row)
        {
            return (int) ListViewScrollToRow(ilvState, new Vector2(0f, (float) currPosY), row).y;
        }

        internal static Vector2 ListViewScrollToRow(InternalListViewState ilvState, Vector2 currPos, int row)
        {
            if ((ilvState.invisibleRows >= row) || (ilvState.endRow <= row))
            {
                if (row <= ilvState.invisibleRows)
                {
                    currPos.y = ilvState.state.rowHeight * row;
                }
                else
                {
                    currPos.y = (ilvState.state.rowHeight * (row + 1)) - ilvState.rectHeight;
                }
                if (currPos.y < 0f)
                {
                    currPos.y = 0f;
                    return currPos;
                }
                if (currPos.y > ((ilvState.state.totalRows * ilvState.state.rowHeight) - ilvState.rectHeight))
                {
                    currPos.y = (ilvState.state.totalRows * ilvState.state.rowHeight) - ilvState.rectHeight;
                }
            }
            return currPos;
        }

        internal static bool MultiSelection(InternalListViewState ilvState, int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
        {
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            bool flag3 = false;
            if ((shift || actionKey) && (initialSelected == -1))
            {
                initialSelected = prevSelected;
            }
            if (shift)
            {
                int num = Math.Min(initialSelected, currSelected);
                int num2 = Math.Max(initialSelected, currSelected);
                if (!actionKey)
                {
                    for (int j = 0; j < num; j++)
                    {
                        if (selectedItems[j])
                        {
                            flag3 = true;
                        }
                        selectedItems[j] = false;
                    }
                    for (int k = num2 + 1; k < selectedItems.Length; k++)
                    {
                        if (selectedItems[k])
                        {
                            flag3 = true;
                        }
                        selectedItems[k] = false;
                    }
                }
                if (num < 0)
                {
                    num = num2;
                }
                for (int i = num; i <= num2; i++)
                {
                    if (!selectedItems[i])
                    {
                        flag3 = true;
                    }
                    selectedItems[i] = true;
                }
            }
            else if (actionKey)
            {
                selectedItems[currSelected] = !selectedItems[currSelected];
                initialSelected = currSelected;
                flag3 = true;
            }
            else
            {
                if (!selectedItems[currSelected])
                {
                    flag3 = true;
                }
                for (int m = 0; m < selectedItems.Length; m++)
                {
                    if (selectedItems[m] && (currSelected != m))
                    {
                        flag3 = true;
                    }
                    selectedItems[m] = false;
                }
                initialSelected = -1;
                selectedItems[currSelected] = true;
            }
            if (ilvState != null)
            {
                ilvState.state.scrollPos = ListViewScrollToRow(ilvState, currSelected);
            }
            return flag3;
        }

        internal static void SendKey(ListViewState state, KeyCode keyCode)
        {
            SendKey(state.ilvState, keyCode, 1);
        }

        internal static bool SendKey(InternalListViewState ilvState, KeyCode keyCode, int totalCols)
        {
            ListViewState state = ilvState.state;
            switch (keyCode)
            {
                case KeyCode.UpArrow:
                    if (state.row > 0)
                    {
                        state.row--;
                    }
                    break;

                case KeyCode.DownArrow:
                    if (state.row < (state.totalRows - 1))
                    {
                        state.row++;
                    }
                    break;

                case KeyCode.RightArrow:
                    if (state.column < (totalCols - 1))
                    {
                        state.column++;
                    }
                    break;

                case KeyCode.LeftArrow:
                    if (state.column > 0)
                    {
                        state.column--;
                    }
                    break;

                case KeyCode.Home:
                    state.row = 0;
                    break;

                case KeyCode.End:
                    state.row = state.totalRows - 1;
                    break;

                case KeyCode.PageUp:
                    if (DoLVPageUpDown(ilvState, ref state.row, ref state.scrollPos, true))
                    {
                        break;
                    }
                    Event.current.Use();
                    return false;

                case KeyCode.PageDown:
                    if (DoLVPageUpDown(ilvState, ref state.row, ref state.scrollPos, false))
                    {
                        break;
                    }
                    Event.current.Use();
                    return false;

                default:
                    return false;
            }
            state.scrollPos = ListViewScrollToRow(ilvState, state.scrollPos, state.row);
            Event.current.Use();
            return true;
        }

        internal class Constants
        {
            public static string insertion = "PR Insertion";
        }

        internal class InternalLayoutedListViewState : ListViewShared.InternalListViewState
        {
            public ListViewGUILayout.GUILayoutedListViewGroup group;
        }

        internal class InternalListViewState
        {
            public bool beganHorizontal;
            public int dragItem;
            public int endRow;
            public int id = -1;
            public int invisibleRows;
            public Rect rect;
            public int rectHeight;
            public ListViewState state;
            public bool wantsExternalFiles;
            public bool wantsReordering;
            public bool wantsToAcceptCustomDrag;
            public bool wantsToStartCustomDrag;
        }

        internal class ListViewElementsEnumerator : IDisposable, IEnumerator, IEnumerator<ListViewElement>
        {
            private int[] colWidths;
            private string dragTitle;
            private ListViewElement element;
            private Rect firstRect;
            private ListViewShared.InternalListViewState ilvState;
            private ListViewShared.InternalLayoutedListViewState ilvStateL;
            private bool isLayouted;
            private bool quiting;
            private Rect rect;
            private int xPos = -1;
            private int xTo;
            private int yFrom;
            private int yPos = -1;
            private int yTo;

            internal ListViewElementsEnumerator(ListViewShared.InternalListViewState ilvState, int[] colWidths, int yFrom, int yTo, string dragTitle, Rect firstRect)
            {
                this.colWidths = colWidths;
                this.xTo = colWidths.Length - 1;
                this.yFrom = yFrom;
                this.yTo = yTo;
                this.firstRect = firstRect;
                this.rect = firstRect;
                this.quiting = ilvState.state.totalRows == 0;
                this.ilvState = ilvState;
                this.ilvStateL = ilvState as ListViewShared.InternalLayoutedListViewState;
                this.isLayouted = this.ilvStateL != null;
                this.dragTitle = dragTitle;
                ilvState.state.customDraggedFromID = 0;
                this.Reset();
            }

            public void Dispose()
            {
            }

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                if (this.xPos > -1)
                {
                    if (ListViewShared.HasMouseDown(this.ilvState, this.rect))
                    {
                        this.ilvState.state.selectionChanged = true;
                        this.ilvState.state.row = this.yPos;
                        this.ilvState.state.column = this.xPos;
                        this.ilvState.state.scrollPos = ListViewShared.ListViewScrollToRow(this.ilvState, this.yPos);
                        if ((this.ilvState.wantsReordering || this.ilvState.wantsToStartCustomDrag) && (GUIUtility.hotControl == this.ilvState.state.ID))
                        {
                            DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.ilvState.state.ID);
                            stateObject.mouseDownPosition = Event.current.mousePosition;
                            this.ilvState.dragItem = this.yPos;
                            ListViewShared.dragControlID = this.ilvState.state.ID;
                        }
                    }
                    if (((this.ilvState.wantsReordering || this.ilvState.wantsToStartCustomDrag) && ((GUIUtility.hotControl == this.ilvState.state.ID) && (Event.current.type == EventType.MouseDrag))) && GUIClip.visibleRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.ilvState.state.ID);
                        if (delay2.CanStartDrag())
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new Object[0];
                            DragAndDrop.paths = null;
                            if (this.ilvState.wantsReordering)
                            {
                                this.ilvState.state.dropHereRect = new Rect(this.ilvState.rect.x, 0f, this.ilvState.rect.width, (float) (this.ilvState.state.rowHeight * 2));
                                DragAndDrop.StartDrag(this.dragTitle);
                            }
                            else if (this.ilvState.wantsToStartCustomDrag)
                            {
                                DragAndDrop.SetGenericData("CustomDragID", this.ilvState.state.ID);
                                DragAndDrop.StartDrag(this.dragTitle);
                            }
                        }
                        Event.current.Use();
                    }
                }
                this.xPos++;
                if (this.xPos > this.xTo)
                {
                    this.xPos = 0;
                    this.yPos++;
                    this.rect.x = this.firstRect.x;
                    this.rect.width = this.colWidths[0];
                    if (this.yPos > this.yTo)
                    {
                        this.quiting = true;
                    }
                    else
                    {
                        this.rect.y += this.rect.height;
                    }
                }
                else
                {
                    if (this.xPos >= 1)
                    {
                        this.rect.x += this.colWidths[this.xPos - 1];
                    }
                    this.rect.width = this.colWidths[this.xPos];
                }
                this.element.row = this.yPos;
                this.element.column = this.xPos;
                this.element.position = this.rect;
                if (this.element.row >= this.ilvState.state.totalRows)
                {
                    this.quiting = true;
                }
                if ((this.isLayouted && (Event.current.type == EventType.Layout)) && ((this.yFrom + 1) == this.yPos))
                {
                    this.quiting = true;
                }
                if (this.isLayouted && (this.yPos != this.yFrom))
                {
                    GUILayout.EndHorizontal();
                }
                if (!this.quiting)
                {
                    if (this.isLayouted)
                    {
                        if (this.yPos != this.yFrom)
                        {
                            this.ilvStateL.group.ResetCursor();
                            this.ilvStateL.group.AddY();
                        }
                        else
                        {
                            this.ilvStateL.group.AddY((float) (this.ilvState.invisibleRows * this.ilvState.state.rowHeight));
                        }
                    }
                }
                else
                {
                    if (this.ilvState.state.drawDropHere && (Event.current.GetTypeForControl(this.ilvState.state.ID) == EventType.Repaint))
                    {
                        GUIStyle insertion = ListViewShared.Constants.insertion;
                        insertion.Draw(insertion.margin.Remove(this.ilvState.state.dropHereRect), false, false, false, false);
                    }
                    if (ListViewShared.ListViewKeyboard(this.ilvState, this.colWidths.Length))
                    {
                        this.ilvState.state.selectionChanged = true;
                    }
                    if (Event.current.GetTypeForControl(this.ilvState.state.ID) == EventType.MouseUp)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    if (!this.ilvState.wantsReordering || (GUIUtility.hotControl != this.ilvState.state.ID))
                    {
                        if (!this.ilvState.wantsExternalFiles)
                        {
                            if (this.ilvState.wantsToAcceptCustomDrag && (ListViewShared.dragControlID != this.ilvState.state.ID))
                            {
                                switch (Event.current.type)
                                {
                                    case EventType.DragUpdated:
                                    {
                                        object genericData = DragAndDrop.GetGenericData("CustomDragID");
                                        if (GUIClip.visibleRect.Contains(Event.current.mousePosition) && (genericData != null))
                                        {
                                            DragAndDrop.visualMode = !this.ilvState.rect.Contains(Event.current.mousePosition) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move;
                                            Event.current.Use();
                                        }
                                        break;
                                    }
                                    case EventType.DragPerform:
                                    {
                                        object obj3 = DragAndDrop.GetGenericData("CustomDragID");
                                        if (GUIClip.visibleRect.Contains(Event.current.mousePosition) && (obj3 != null))
                                        {
                                            this.ilvState.state.customDraggedFromID = (int) obj3;
                                            DragAndDrop.AcceptDrag();
                                            Event.current.Use();
                                        }
                                        GUIUtility.hotControl = 0;
                                        break;
                                    }
                                    case EventType.DragExited:
                                        GUIUtility.hotControl = 0;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch (Event.current.type)
                            {
                                case EventType.DragUpdated:
                                    if ((GUIClip.visibleRect.Contains(Event.current.mousePosition) && (DragAndDrop.paths != null)) && (DragAndDrop.paths.Length != 0))
                                    {
                                        DragAndDrop.visualMode = !this.ilvState.rect.Contains(Event.current.mousePosition) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy;
                                        Event.current.Use();
                                        if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
                                        {
                                            this.ilvState.state.dropHereRect = new Rect(this.ilvState.rect.x, (float) ((Mathf.RoundToInt(Event.current.mousePosition.y / ((float) this.ilvState.state.rowHeight)) - 1) * this.ilvState.state.rowHeight), this.ilvState.rect.width, (float) this.ilvState.state.rowHeight);
                                            if (this.ilvState.state.dropHereRect.y >= (this.ilvState.state.rowHeight * this.ilvState.state.totalRows))
                                            {
                                                this.ilvState.state.dropHereRect.y = this.ilvState.state.rowHeight * (this.ilvState.state.totalRows - 1);
                                            }
                                            this.ilvState.state.drawDropHere = true;
                                        }
                                    }
                                    break;

                                case EventType.DragPerform:
                                    if (GUIClip.visibleRect.Contains(Event.current.mousePosition))
                                    {
                                        this.ilvState.state.fileNames = DragAndDrop.paths;
                                        DragAndDrop.AcceptDrag();
                                        Event.current.Use();
                                        this.ilvState.wantsExternalFiles = false;
                                        this.ilvState.state.drawDropHere = false;
                                        this.ilvState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / ((float) this.ilvState.state.rowHeight));
                                        if (this.ilvState.state.draggedTo > this.ilvState.state.totalRows)
                                        {
                                            this.ilvState.state.draggedTo = this.ilvState.state.totalRows;
                                        }
                                        this.ilvState.state.row = this.ilvState.state.draggedTo;
                                    }
                                    GUIUtility.hotControl = 0;
                                    break;

                                case EventType.DragExited:
                                    this.ilvState.wantsExternalFiles = false;
                                    this.ilvState.state.drawDropHere = false;
                                    GUIUtility.hotControl = 0;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        ListViewState state = this.ilvState.state;
                        switch (Event.current.type)
                        {
                            case EventType.DragUpdated:
                                DragAndDrop.visualMode = !this.ilvState.rect.Contains(Event.current.mousePosition) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move;
                                Event.current.Use();
                                if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
                                {
                                    state.dropHereRect.y = (Mathf.RoundToInt(Event.current.mousePosition.y / ((float) state.rowHeight)) - 1) * state.rowHeight;
                                    if (state.dropHereRect.y >= (state.rowHeight * state.totalRows))
                                    {
                                        state.dropHereRect.y = state.rowHeight * (state.totalRows - 1);
                                    }
                                    state.drawDropHere = true;
                                }
                                break;

                            case EventType.DragPerform:
                                if (GUIClip.visibleRect.Contains(Event.current.mousePosition))
                                {
                                    this.ilvState.state.draggedFrom = this.ilvState.dragItem;
                                    this.ilvState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / ((float) state.rowHeight));
                                    if (this.ilvState.state.draggedTo > this.ilvState.state.totalRows)
                                    {
                                        this.ilvState.state.draggedTo = this.ilvState.state.totalRows;
                                    }
                                    if (this.ilvState.state.draggedTo > this.ilvState.state.draggedFrom)
                                    {
                                        this.ilvState.state.row = this.ilvState.state.draggedTo - 1;
                                    }
                                    else
                                    {
                                        this.ilvState.state.row = this.ilvState.state.draggedTo;
                                    }
                                    this.ilvState.state.selectionChanged = true;
                                    DragAndDrop.AcceptDrag();
                                    Event.current.Use();
                                    this.ilvState.wantsReordering = false;
                                    this.ilvState.state.drawDropHere = false;
                                }
                                GUIUtility.hotControl = 0;
                                break;

                            case EventType.DragExited:
                                this.ilvState.wantsReordering = false;
                                this.ilvState.state.drawDropHere = false;
                                GUIUtility.hotControl = 0;
                                break;
                        }
                    }
                    if (this.ilvState.beganHorizontal)
                    {
                        EditorGUILayout.EndScrollView();
                        GUILayout.EndHorizontal();
                        this.ilvState.beganHorizontal = false;
                    }
                    if (this.isLayouted)
                    {
                        GUILayoutUtility.EndLayoutGroup();
                        EditorGUILayout.EndScrollView();
                    }
                    this.ilvState.wantsReordering = false;
                    this.ilvState.wantsExternalFiles = false;
                }
                if (this.isLayouted)
                {
                    if (!this.quiting)
                    {
                        GUILayout.BeginHorizontal(GUIStyle.none, new GUILayoutOption[0]);
                    }
                    else
                    {
                        GUILayout.EndHorizontal();
                    }
                }
                return !this.quiting;
            }

            public void Reset()
            {
                this.xPos = -1;
                this.yPos = this.yFrom;
            }

            ListViewElement IEnumerator<ListViewElement>.Current
            {
                get
                {
                    return this.element;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.element;
                }
            }
        }
    }
}

