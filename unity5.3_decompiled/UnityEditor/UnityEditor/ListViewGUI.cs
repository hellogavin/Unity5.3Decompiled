namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ListViewGUI
    {
        private static int[] dummyWidths = new int[1];
        internal static ListViewShared.InternalListViewState ilvState = new ListViewShared.InternalListViewState();
        private static int listViewHash = "ListView".GetHashCode();

        public static ListViewShared.ListViewElementsEnumerator DoListView(Rect pos, ListViewState state, int[] colWidths, string dragTitle)
        {
            Rect rect;
            int controlID = GUIUtility.GetControlID(listViewHash, FocusType.Native);
            state.ID = controlID;
            state.selectionChanged = false;
            if ((GUIClip.visibleRect.x < 0f) || (GUIClip.visibleRect.y < 0f))
            {
                rect = pos;
            }
            else
            {
                rect = (pos.y >= 0f) ? new Rect(0f, state.scrollPos.y, GUIClip.visibleRect.width, GUIClip.visibleRect.height) : new Rect(0f, 0f, GUIClip.visibleRect.width, GUIClip.visibleRect.height);
            }
            if (rect.width <= 0f)
            {
                rect.width = 1f;
            }
            if (rect.height <= 0f)
            {
                rect.height = 1f;
            }
            ilvState.rect = rect;
            int yFrom = (int) ((-pos.y + rect.yMin) / ((float) state.rowHeight));
            int yTo = (yFrom + ((int) Math.Ceiling((double) ((((rect.yMin - pos.y) % ((float) state.rowHeight)) + rect.height) / ((float) state.rowHeight))))) - 1;
            if (colWidths == null)
            {
                dummyWidths[0] = (int) rect.width;
                colWidths = dummyWidths;
            }
            ilvState.invisibleRows = yFrom;
            ilvState.endRow = yTo;
            ilvState.rectHeight = (int) rect.height;
            ilvState.state = state;
            if (yFrom < 0)
            {
                yFrom = 0;
            }
            if (yTo >= state.totalRows)
            {
                yTo = state.totalRows - 1;
            }
            return new ListViewShared.ListViewElementsEnumerator(ilvState, colWidths, yFrom, yTo, dragTitle, new Rect(0f, (float) (yFrom * state.rowHeight), pos.width, (float) state.rowHeight));
        }

        public static bool HasMouseDown(Rect r)
        {
            return ListViewShared.HasMouseDown(ilvState, r, 0);
        }

        public static bool HasMouseDown(Rect r, int button)
        {
            return ListViewShared.HasMouseDown(ilvState, r, button);
        }

        public static bool HasMouseUp(Rect r)
        {
            return ListViewShared.HasMouseUp(ilvState, r, 0);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(Rect pos, ListViewState state)
        {
            return DoListView(pos, state, null, string.Empty);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, (ListViewOptions) 0, null, string.Empty, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, int[] colWidths, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, (ListViewOptions) 0, colWidths, string.Empty, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, lvOptions, null, string.Empty, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListView(state, lvOptions, null, dragTitle, style, options);
        }

        public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, int[] colWidths, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
            state.scrollPos = EditorGUILayout.BeginScrollView(state.scrollPos, options);
            ilvState.beganHorizontal = true;
            state.draggedFrom = -1;
            state.draggedTo = -1;
            state.fileNames = null;
            if ((lvOptions & ListViewOptions.wantsReordering) != ((ListViewOptions) 0))
            {
                ilvState.wantsReordering = true;
            }
            if ((lvOptions & ListViewOptions.wantsExternalFiles) != ((ListViewOptions) 0))
            {
                ilvState.wantsExternalFiles = true;
            }
            if ((lvOptions & ListViewOptions.wantsToStartCustomDrag) != ((ListViewOptions) 0))
            {
                ilvState.wantsToStartCustomDrag = true;
            }
            if ((lvOptions & ListViewOptions.wantsToAcceptCustomDrag) != ((ListViewOptions) 0))
            {
                ilvState.wantsToAcceptCustomDrag = true;
            }
            return DoListView(GUILayoutUtility.GetRect(1f, (float) ((state.totalRows * state.rowHeight) + 3)), state, colWidths, string.Empty);
        }

        public static bool MultiSelection(int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
        {
            return ListViewShared.MultiSelection(ilvState, prevSelected, currSelected, ref initialSelected, ref selectedItems);
        }
    }
}

