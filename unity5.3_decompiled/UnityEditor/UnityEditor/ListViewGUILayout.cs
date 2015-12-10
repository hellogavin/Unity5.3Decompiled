namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ListViewGUILayout
    {
        private static Rect dummyRect = new Rect(0f, 0f, 1f, 1f);
        private static int[] dummyWidths = new int[1];
        private static int layoutedListViewHash = "layoutedListView".GetHashCode();
        private static int listViewHash = "ListView".GetHashCode();
        private static ListViewState lvState = null;

        private static void BeginLayoutedListview(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayoutedListViewGroup group = (GUILayoutedListViewGroup) GUILayoutUtility.BeginLayoutGroup(style, null, typeof(GUILayoutedListViewGroup));
            group.state = state;
            state.ilvState.group = group;
            GUIUtility.GetControlID(layoutedListViewHash, FocusType.Native);
            if (Event.current.type == EventType.Layout)
            {
                group.resetCoords = false;
                group.isVertical = true;
                group.ApplyOptions(options);
            }
        }

        private static ListViewShared.ListViewElementsEnumerator DoListView(ListViewState state, int[] colWidths, string dragTitle)
        {
            Rect dummyRect = ListViewGUILayout.dummyRect;
            int yFrom = 0;
            int yTo = 0;
            ListViewShared.InternalLayoutedListViewState ilvState = state.ilvState;
            int controlID = GUIUtility.GetControlID(listViewHash, FocusType.Native);
            state.ID = controlID;
            state.selectionChanged = false;
            ilvState.state = state;
            if (Event.current.type != EventType.Layout)
            {
                dummyRect = new Rect(0f, state.scrollPos.y, GUIClip.visibleRect.width, GUIClip.visibleRect.height);
                if (dummyRect.width <= 0f)
                {
                    dummyRect.width = 1f;
                }
                if (dummyRect.height <= 0f)
                {
                    dummyRect.height = 1f;
                }
                state.ilvState.rect = dummyRect;
                yFrom = ((int) dummyRect.yMin) / state.rowHeight;
                yTo = (yFrom + ((int) Math.Ceiling((double) (((dummyRect.yMin % ((float) state.rowHeight)) + dummyRect.height) / ((float) state.rowHeight))))) - 1;
                ilvState.invisibleRows = yFrom;
                ilvState.endRow = yTo;
                ilvState.rectHeight = (int) dummyRect.height;
                if (yFrom < 0)
                {
                    yFrom = 0;
                }
                if (yTo >= state.totalRows)
                {
                    yTo = state.totalRows - 1;
                }
            }
            if (colWidths == null)
            {
                dummyWidths[0] = (int) dummyRect.width;
                colWidths = dummyWidths;
            }
            return new ListViewShared.ListViewElementsEnumerator(ilvState, colWidths, yFrom, yTo, dragTitle, new Rect(0f, (float) (yFrom * state.rowHeight), dummyRect.width, (float) state.rowHeight));
        }

        public static bool HasMouseDown(Rect r)
        {
            return ListViewShared.HasMouseDown(lvState.ilvState, r, 0);
        }

        public static bool HasMouseDown(Rect r, int button)
        {
            return ListViewShared.HasMouseDown(lvState.ilvState, r, button);
        }

        public static bool HasMouseUp(Rect r)
        {
            return ListViewShared.HasMouseUp(lvState.ilvState, r, 0);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, (ListViewOptions) 0, string.Empty, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, (ListViewOptions) 0, dragTitle, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, lvOptions, string.Empty, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            lvState = state;
            GUILayout.BeginHorizontal(style, options);
            state.scrollPos = EditorGUILayout.BeginScrollView(state.scrollPos, options);
            BeginLayoutedListview(state, GUIStyle.none, new GUILayoutOption[0]);
            state.draggedFrom = -1;
            state.draggedTo = -1;
            state.fileNames = null;
            if ((lvOptions & ListViewOptions.wantsReordering) != ((ListViewOptions) 0))
            {
                state.ilvState.wantsReordering = true;
            }
            if ((lvOptions & ListViewOptions.wantsExternalFiles) != ((ListViewOptions) 0))
            {
                state.ilvState.wantsExternalFiles = true;
            }
            if ((lvOptions & ListViewOptions.wantsToStartCustomDrag) != ((ListViewOptions) 0))
            {
                state.ilvState.wantsToStartCustomDrag = true;
            }
            if ((lvOptions & ListViewOptions.wantsToAcceptCustomDrag) != ((ListViewOptions) 0))
            {
                state.ilvState.wantsToAcceptCustomDrag = true;
            }
            return DoListView(state, null, dragTitle);
        }

        public static bool MultiSelection(int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
        {
            return ListViewShared.MultiSelection(lvState.ilvState, prevSelected, currSelected, ref initialSelected, ref selectedItems);
        }

        internal class GUILayoutedListViewGroup : GUILayoutGroup
        {
            internal ListViewState state;

            public void AddY()
            {
                if (base.entries.Count > 0)
                {
                    this.AddYRecursive(base.entries[0], base.entries[0].minHeight);
                }
            }

            public void AddY(float val)
            {
                if (base.entries.Count > 0)
                {
                    this.AddYRecursive(base.entries[0], val);
                }
            }

            private void AddYRecursive(GUILayoutEntry e, float y)
            {
                e.rect.y += y;
                GUILayoutGroup group = e as GUILayoutGroup;
                if (group != null)
                {
                    for (int i = 0; i < group.entries.Count; i++)
                    {
                        this.AddYRecursive(group.entries[i], y);
                    }
                }
            }

            public override void CalcHeight()
            {
                base.minHeight = 0f;
                base.maxHeight = 0f;
                base.CalcHeight();
                this.margin.top = 0;
                this.margin.bottom = 0;
                if (base.minHeight == 0f)
                {
                    base.minHeight = 1f;
                    base.maxHeight = 1f;
                    this.state.rowHeight = 1;
                }
                else
                {
                    this.state.rowHeight = (int) base.minHeight;
                    base.minHeight *= this.state.totalRows;
                    base.maxHeight *= this.state.totalRows;
                }
            }

            public override void CalcWidth()
            {
                base.CalcWidth();
                base.minWidth = 0f;
                base.maxWidth = 0f;
                base.stretchWidth = 0x2710;
            }
        }
    }
}

