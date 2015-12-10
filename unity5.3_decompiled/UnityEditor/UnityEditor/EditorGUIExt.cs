namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class EditorGUIExt
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapE;
        private static bool adding = false;
        private static int firstScrollWait = 250;
        private static int initIndex = 0;
        private static bool[] initSelections;
        private static int kFirstScrollWait = 250;
        private static int kScrollWait = 30;
        private static Styles ms_Styles = new Styles();
        private static float nextScrollStepTime = 0f;
        private static int repeatButtonHash = "repeatButton".GetHashCode();
        private static List<bool> s_LastFrameSelections = null;
        internal static int s_MinMaxSliderHash = "MinMaxSlider".GetHashCode();
        private static MinMaxSliderState s_MinMaxSliderState;
        private static Vector2 s_MouseDownPos = Vector2.zero;
        private static DragSelectionState s_MultiSelectDragSelection = DragSelectionState.None;
        private static DateTime s_NextScrollStepTime = DateTime.Now;
        private static List<bool> s_SelectionBackup = null;
        private static Vector2 s_StartSelectPos = Vector2.zero;
        private static int scrollControlID;
        private static int scrollWait = 30;

        private static bool Any(bool[] selections)
        {
            for (int i = 0; i < selections.Length; i++)
            {
                if (selections[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal static void DoMinMaxSlider(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
        {
            float num9;
            float num10;
            Rect rect;
            Rect rect2;
            Rect rect3;
            float num13;
            float num14;
            Event current = Event.current;
            bool flag = size == 0f;
            float min = Mathf.Min(visualStart, visualEnd);
            float max = Mathf.Max(visualStart, visualEnd);
            float dragStartLimit = Mathf.Min(startLimit, endLimit);
            float dragEndLimit = Mathf.Max(startLimit, endLimit);
            MinMaxSliderState state = s_MinMaxSliderState;
            if ((GUIUtility.hotControl == id) && (state != null))
            {
                min = state.dragStartLimit;
                dragStartLimit = state.dragStartLimit;
                max = state.dragEndLimit;
                dragEndLimit = state.dragEndLimit;
            }
            float num5 = 0f;
            float num6 = Mathf.Clamp(value, min, max);
            float num7 = Mathf.Clamp(value + size, min, max) - num6;
            float num8 = (visualStart <= visualEnd) ? ((float) 1) : ((float) (-1));
            if ((slider == null) || (thumb == null))
            {
                return;
            }
            if (horiz)
            {
                float num11 = (thumb.fixedWidth == 0f) ? ((float) thumb.padding.horizontal) : thumb.fixedWidth;
                num9 = ((position.width - slider.padding.horizontal) - num11) / (max - min);
                rect = new Rect((((num6 - min) * num9) + position.x) + slider.padding.left, position.y + slider.padding.top, (num7 * num9) + num11, position.height - slider.padding.vertical);
                rect2 = new Rect(rect.x, rect.y, (float) thumb.padding.left, rect.height);
                rect3 = new Rect(rect.xMax - thumb.padding.right, rect.y, (float) thumb.padding.right, rect.height);
                num10 = current.mousePosition.x - position.x;
            }
            else
            {
                float num12 = (thumb.fixedHeight == 0f) ? ((float) thumb.padding.vertical) : thumb.fixedHeight;
                num9 = ((position.height - slider.padding.vertical) - num12) / (max - min);
                rect = new Rect(position.x + slider.padding.left, (((num6 - min) * num9) + position.y) + slider.padding.top, position.width - slider.padding.horizontal, (num7 * num9) + num12);
                rect2 = new Rect(rect.x, rect.y, rect.width, (float) thumb.padding.top);
                rect3 = new Rect(rect.x, rect.yMax - thumb.padding.bottom, rect.width, (float) thumb.padding.bottom);
                num10 = current.mousePosition.y - position.y;
            }
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && ((min - max) != 0f))
                    {
                        if (state == null)
                        {
                            state = s_MinMaxSliderState = new MinMaxSliderState();
                        }
                        state.dragStartLimit = startLimit;
                        state.dragEndLimit = endLimit;
                        if (rect.Contains(current.mousePosition))
                        {
                            state.dragStartPos = num10;
                            state.dragStartValue = value;
                            state.dragStartSize = size;
                            state.dragStartValuesPerPixel = num9;
                            if (rect2.Contains(current.mousePosition))
                            {
                                state.whereWeDrag = 1;
                            }
                            else if (rect3.Contains(current.mousePosition))
                            {
                                state.whereWeDrag = 2;
                            }
                            else
                            {
                                state.whereWeDrag = 0;
                            }
                            GUIUtility.hotControl = id;
                            current.Use();
                            return;
                        }
                        if (slider != GUIStyle.none)
                        {
                            if ((size != 0f) && flag)
                            {
                                if (horiz)
                                {
                                    if (num10 > (rect.xMax - position.x))
                                    {
                                        value += (size * num8) * 0.9f;
                                    }
                                    else
                                    {
                                        value -= (size * num8) * 0.9f;
                                    }
                                }
                                else if (num10 > (rect.yMax - position.y))
                                {
                                    value += (size * num8) * 0.9f;
                                }
                                else
                                {
                                    value -= (size * num8) * 0.9f;
                                }
                                state.whereWeDrag = 0;
                                GUI.changed = true;
                                s_NextScrollStepTime = DateTime.Now.AddMilliseconds((double) kFirstScrollWait);
                                num13 = !horiz ? current.mousePosition.y : current.mousePosition.x;
                                num14 = !horiz ? rect.y : rect.x;
                                state.whereWeDrag = (num13 <= num14) ? 3 : 4;
                            }
                            else
                            {
                                if (horiz)
                                {
                                    value = (((num10 - (rect.width * 0.5f)) / num9) + min) - (size * 0.5f);
                                }
                                else
                                {
                                    value = (((num10 - (rect.height * 0.5f)) / num9) + min) - (size * 0.5f);
                                }
                                state.dragStartPos = num10;
                                state.dragStartValue = value;
                                state.dragStartSize = size;
                                state.dragStartValuesPerPixel = num9;
                                state.whereWeDrag = 0;
                                GUI.changed = true;
                            }
                            GUIUtility.hotControl = id;
                            value = Mathf.Clamp(value, dragStartLimit, dragEndLimit - size);
                            current.Use();
                        }
                        return;
                    }
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        current.Use();
                        GUIUtility.hotControl = 0;
                    }
                    return;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        float num15 = (num10 - state.dragStartPos) / state.dragStartValuesPerPixel;
                        switch (state.whereWeDrag)
                        {
                            case 0:
                                value = Mathf.Clamp(state.dragStartValue + num15, dragStartLimit, dragEndLimit - size);
                                goto Label_069F;

                            case 1:
                                value = state.dragStartValue + num15;
                                size = state.dragStartSize - num15;
                                if (value < dragStartLimit)
                                {
                                    size -= dragStartLimit - value;
                                    value = dragStartLimit;
                                }
                                if (size < num5)
                                {
                                    value -= num5 - size;
                                    size = num5;
                                }
                                goto Label_069F;

                            case 2:
                                size = state.dragStartSize + num15;
                                if ((value + size) > dragEndLimit)
                                {
                                    size = dragEndLimit - value;
                                }
                                if (size < num5)
                                {
                                    size = num5;
                                }
                                goto Label_069F;
                        }
                        break;
                    }
                    return;

                case EventType.Repaint:
                    slider.Draw(position, GUIContent.none, id);
                    thumb.Draw(rect, GUIContent.none, id);
                    if (((GUIUtility.hotControl == id) && position.Contains(current.mousePosition)) && ((min - max) != 0f))
                    {
                        if (rect.Contains(current.mousePosition))
                        {
                            if ((state != null) && ((state.whereWeDrag == 3) || (state.whereWeDrag == 4)))
                            {
                                GUIUtility.hotControl = 0;
                            }
                            return;
                        }
                        if (DateTime.Now >= s_NextScrollStepTime)
                        {
                            num13 = !horiz ? current.mousePosition.y : current.mousePosition.x;
                            num14 = !horiz ? rect.y : rect.x;
                            int num16 = (num13 <= num14) ? 3 : 4;
                            if (num16 != state.whereWeDrag)
                            {
                                return;
                            }
                            if ((size != 0f) && flag)
                            {
                                if (horiz)
                                {
                                    if (num10 > (rect.xMax - position.x))
                                    {
                                        value += (size * num8) * 0.9f;
                                    }
                                    else
                                    {
                                        value -= (size * num8) * 0.9f;
                                    }
                                }
                                else if (num10 > (rect.yMax - position.y))
                                {
                                    value += (size * num8) * 0.9f;
                                }
                                else
                                {
                                    value -= (size * num8) * 0.9f;
                                }
                                state.whereWeDrag = -1;
                                GUI.changed = true;
                            }
                            value = Mathf.Clamp(value, dragStartLimit, dragEndLimit - size);
                            s_NextScrollStepTime = DateTime.Now.AddMilliseconds((double) kScrollWait);
                        }
                        return;
                    }
                    return;

                default:
                    return;
            }
        Label_069F:
            GUI.changed = true;
            current.Use();
        }

        private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
        {
            int controlID = GUIUtility.GetControlID(repeatButtonHash, focusType, position);
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            if (typeForControl != EventType.MouseDown)
            {
                if (typeForControl != EventType.MouseUp)
                {
                    if (typeForControl != EventType.Repaint)
                    {
                        return false;
                    }
                    style.Draw(position, content, controlID);
                    return ((controlID == GUIUtility.hotControl) && position.Contains(Event.current.mousePosition));
                }
            }
            else
            {
                if (position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    Event.current.Use();
                }
                return false;
            }
            if (GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
                return position.Contains(Event.current.mousePosition);
            }
            return false;
        }

        public static bool DragSelection(Rect[] positions, ref bool[] selections, GUIStyle style)
        {
            bool flag;
            int num5;
            int controlID = GUIUtility.GetControlID(0x20f3dc7, FocusType.Keyboard);
            Event current = Event.current;
            int index = -1;
            for (int i = positions.Length - 1; i >= 0; i--)
            {
                if (positions[i].Contains(current.mousePosition))
                {
                    index = i;
                    break;
                }
            }
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if ((current.button != 0) || (index < 0))
                    {
                        goto Label_02ED;
                    }
                    GUIUtility.keyboardControl = 0;
                    flag = false;
                    if (!selections[index])
                    {
                        goto Label_012C;
                    }
                    num5 = 0;
                    bool[] flagArray = selections;
                    for (int j = 0; j < flagArray.Length; j++)
                    {
                        if (flagArray[j])
                        {
                            num5++;
                            if (num5 > 1)
                            {
                                break;
                            }
                        }
                    }
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    goto Label_02ED;

                case EventType.MouseDrag:
                {
                    if ((GUIUtility.hotControl != controlID) || (current.button != 0))
                    {
                        goto Label_02ED;
                    }
                    if (index < 0)
                    {
                        Rect rect = new Rect(positions[0].x, positions[0].y - 200f, positions[0].width, 200f);
                        if (rect.Contains(current.mousePosition))
                        {
                            index = 0;
                        }
                        rect.y = positions[positions.Length - 1].yMax;
                        if (rect.Contains(current.mousePosition))
                        {
                            index = selections.Length - 1;
                        }
                    }
                    if (index < 0)
                    {
                        return false;
                    }
                    int num8 = Mathf.Min(initIndex, index);
                    int num9 = Mathf.Max(initIndex, index);
                    for (int k = 0; k < selections.Length; k++)
                    {
                        if ((k >= num8) && (k <= num9))
                        {
                            selections[k] = adding;
                        }
                        else
                        {
                            selections[k] = initSelections[k];
                        }
                    }
                    current.Use();
                    return true;
                }
                case EventType.Repaint:
                    for (int m = 0; m < positions.Length; m++)
                    {
                        style.Draw(positions[m], GUIContent.none, controlID, selections[m]);
                    }
                    goto Label_02ED;

                default:
                    goto Label_02ED;
            }
            if (num5 == 1)
            {
                flag = true;
            }
        Label_012C:
            if (!current.shift && !EditorGUI.actionKey)
            {
                for (int n = 0; n < positions.Length; n++)
                {
                    selections[n] = false;
                }
            }
            initIndex = index;
            initSelections = (bool[]) selections.Clone();
            adding = true;
            if ((current.shift || EditorGUI.actionKey) && selections[index])
            {
                adding = false;
            }
            selections[index] = !flag ? adding : false;
            GUIUtility.hotControl = controlID;
            current.Use();
            return true;
        Label_02ED:
            return false;
        }

        internal static Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        private static int GetIndexUnderMouse(Rect[] hitPositions, bool[] readOnly)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            for (int i = hitPositions.Length - 1; i >= 0; i--)
            {
                if (((readOnly == null) || !readOnly[i]) && hitPositions[i].Contains(mousePosition))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void MinMaxScroller(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
        {
            float num;
            Rect rect;
            Rect rect2;
            Rect rect3;
            if (horiz)
            {
                num = (size * 10f) / position.width;
            }
            else
            {
                num = (size * 10f) / position.height;
            }
            if (horiz)
            {
                rect = new Rect(position.x + leftButton.fixedWidth, position.y, (position.width - leftButton.fixedWidth) - rightButton.fixedWidth, position.height);
                rect2 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                rect3 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
            }
            else
            {
                rect = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, (position.height - leftButton.fixedHeight) - rightButton.fixedHeight);
                rect2 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                rect3 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
            }
            float num2 = Mathf.Min(visualStart, value);
            float num3 = Mathf.Max(visualEnd, value + size);
            MinMaxSlider(rect, ref value, ref size, num2, num3, num2, num3, slider, thumb, horiz);
            bool flag = false;
            if (Event.current.type == EventType.MouseUp)
            {
                flag = true;
            }
            if (ScrollerRepeatButton(id, rect2, leftButton))
            {
                value -= num * ((visualStart >= visualEnd) ? -1f : 1f);
            }
            if (ScrollerRepeatButton(id, rect3, rightButton))
            {
                value += num * ((visualStart >= visualEnd) ? -1f : 1f);
            }
            if (flag && (Event.current.type == EventType.Used))
            {
                scrollControlID = 0;
            }
            if (startLimit < endLimit)
            {
                value = Mathf.Clamp(value, startLimit, endLimit - size);
            }
            else
            {
                value = Mathf.Clamp(value, endLimit, startLimit - size);
            }
        }

        public static void MinMaxSlider(Rect position, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
        {
            DoMinMaxSlider(position, GUIUtility.GetControlID(s_MinMaxSliderHash, FocusType.Passive), ref value, ref size, visualStart, visualEnd, startLimit, endLimit, slider, thumb, horiz);
        }

        public static HighLevelEvent MultiSelection(Rect rect, Rect[] positions, GUIContent content, Rect[] hitPositions, ref bool[] selections, bool[] readOnly, out int clickedIndex, out Vector2 offset, out float startSelect, out float endSelect, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(0x27b1f9d, FocusType.Keyboard);
            Event current = Event.current;
            offset = Vector2.zero;
            clickedIndex = -1;
            startSelect = endSelect = 0f;
            if (current.type != EventType.Used)
            {
                int indexUnderMouse;
                bool flag = false;
                if (Event.current.type != EventType.Layout)
                {
                    if (GUIUtility.keyboardControl == controlID)
                    {
                        flag = true;
                    }
                    else
                    {
                        selections = new bool[selections.Length];
                    }
                }
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (current.button != 0)
                        {
                            break;
                        }
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = controlID;
                        s_StartSelectPos = current.mousePosition;
                        indexUnderMouse = GetIndexUnderMouse(hitPositions, readOnly);
                        if ((Event.current.clickCount != 2) || (indexUnderMouse < 0))
                        {
                            if (indexUnderMouse >= 0)
                            {
                                if ((!current.shift && !EditorGUI.actionKey) && !selections[indexUnderMouse])
                                {
                                    for (int j = 0; j < hitPositions.Length; j++)
                                    {
                                        selections[j] = false;
                                    }
                                }
                                if (current.shift || EditorGUI.actionKey)
                                {
                                    selections[indexUnderMouse] = !selections[indexUnderMouse];
                                }
                                else
                                {
                                    selections[indexUnderMouse] = true;
                                }
                                s_MouseDownPos = current.mousePosition;
                                s_MultiSelectDragSelection = DragSelectionState.None;
                                current.Use();
                                clickedIndex = indexUnderMouse;
                                return HighLevelEvent.SelectionChanged;
                            }
                            bool flag2 = false;
                            if (!current.shift && !EditorGUI.actionKey)
                            {
                                for (int k = 0; k < hitPositions.Length; k++)
                                {
                                    selections[k] = false;
                                }
                                flag2 = true;
                            }
                            else
                            {
                                flag2 = false;
                            }
                            s_SelectionBackup = new List<bool>(selections);
                            s_LastFrameSelections = new List<bool>(selections);
                            s_MultiSelectDragSelection = DragSelectionState.DragSelecting;
                            current.Use();
                            return (!flag2 ? HighLevelEvent.None : HighLevelEvent.SelectionChanged);
                        }
                        for (int i = 0; i < selections.Length; i++)
                        {
                            selections[i] = false;
                        }
                        selections[indexUnderMouse] = true;
                        current.Use();
                        clickedIndex = indexUnderMouse;
                        return HighLevelEvent.DoubleClick;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl != controlID)
                        {
                            break;
                        }
                        GUIUtility.hotControl = 0;
                        if (s_StartSelectPos != current.mousePosition)
                        {
                            current.Use();
                        }
                        if (s_MultiSelectDragSelection == DragSelectionState.None)
                        {
                            clickedIndex = GetIndexUnderMouse(hitPositions, readOnly);
                            if (current.clickCount == 1)
                            {
                                return HighLevelEvent.Click;
                            }
                            break;
                        }
                        s_MultiSelectDragSelection = DragSelectionState.None;
                        s_SelectionBackup = null;
                        s_LastFrameSelections = null;
                        return HighLevelEvent.EndDrag;

                    case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlID)
                        {
                            break;
                        }
                        if (s_MultiSelectDragSelection != DragSelectionState.DragSelecting)
                        {
                            offset = current.mousePosition - s_MouseDownPos;
                            current.Use();
                            if (s_MultiSelectDragSelection == DragSelectionState.None)
                            {
                                s_MultiSelectDragSelection = DragSelectionState.Dragging;
                                return HighLevelEvent.BeginDrag;
                            }
                            return HighLevelEvent.Drag;
                        }
                        float num9 = Mathf.Min(s_StartSelectPos.x, current.mousePosition.x);
                        float num10 = Mathf.Max(s_StartSelectPos.x, current.mousePosition.x);
                        s_SelectionBackup.CopyTo(selections);
                        for (int m = 0; m < hitPositions.Length; m++)
                        {
                            if (!selections[m])
                            {
                                float num12 = hitPositions[m].x + (hitPositions[m].width * 0.5f);
                                if ((num12 >= num9) && (num12 <= num10))
                                {
                                    selections[m] = true;
                                }
                            }
                        }
                        current.Use();
                        startSelect = num9;
                        endSelect = num10;
                        bool flag3 = false;
                        for (int n = 0; n < selections.Length; n++)
                        {
                            if (selections[n] != s_LastFrameSelections[n])
                            {
                                flag3 = true;
                                s_LastFrameSelections[n] = selections[n];
                            }
                        }
                        return (!flag3 ? HighLevelEvent.None : HighLevelEvent.SelectionChanged);
                    }
                    case EventType.KeyDown:
                        if (!flag || ((current.keyCode != KeyCode.Backspace) && (current.keyCode != KeyCode.Delete)))
                        {
                            break;
                        }
                        current.Use();
                        return HighLevelEvent.Delete;

                    case EventType.Repaint:
                    {
                        if ((GUIUtility.hotControl == controlID) && (s_MultiSelectDragSelection == DragSelectionState.DragSelecting))
                        {
                            float num3 = Mathf.Min(s_StartSelectPos.x, current.mousePosition.x);
                            float num4 = Mathf.Max(s_StartSelectPos.x, current.mousePosition.x);
                            Rect position = new Rect(0f, 0f, rect.width, rect.height) {
                                x = num3,
                                width = num4 - num3
                            };
                            if (position.width != 0f)
                            {
                                GUI.Box(position, string.Empty, ms_Styles.selectionRect);
                            }
                        }
                        Color color = GUI.color;
                        for (int num5 = 0; num5 < positions.Length; num5++)
                        {
                            if ((readOnly != null) && readOnly[num5])
                            {
                                GUI.color = color * new Color(0.9f, 0.9f, 0.9f, 0.5f);
                            }
                            else if (selections[num5])
                            {
                                GUI.color = color * new Color(0.3f, 0.55f, 0.95f, 1f);
                            }
                            else
                            {
                                GUI.color = color * new Color(0.9f, 0.9f, 0.9f, 1f);
                            }
                            style.Draw(positions[num5], content, controlID, selections[num5]);
                        }
                        GUI.color = color;
                        break;
                    }
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                    {
                        int num15;
                        if (!flag)
                        {
                            break;
                        }
                        bool flag4 = current.type == EventType.ExecuteCommand;
                        string commandName = current.commandName;
                        if (commandName == null)
                        {
                            break;
                        }
                        if (<>f__switch$mapE == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                            dictionary.Add("Delete", 0);
                            <>f__switch$mapE = dictionary;
                        }
                        if (!<>f__switch$mapE.TryGetValue(commandName, out num15) || (num15 != 0))
                        {
                            break;
                        }
                        current.Use();
                        if (!flag4)
                        {
                            break;
                        }
                        return HighLevelEvent.Delete;
                    }
                    case EventType.ContextClick:
                        indexUnderMouse = GetIndexUnderMouse(hitPositions, readOnly);
                        if (indexUnderMouse < 0)
                        {
                            break;
                        }
                        clickedIndex = indexUnderMouse;
                        GUIUtility.keyboardControl = controlID;
                        current.Use();
                        return HighLevelEvent.ContextClick;
                }
            }
            return HighLevelEvent.None;
        }

        private static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
        {
            bool flag = false;
            if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
            {
                bool flag2 = scrollControlID != scrollerID;
                scrollControlID = scrollerID;
                if (flag2)
                {
                    flag = true;
                    nextScrollStepTime = Time.realtimeSinceStartup + (0.001f * firstScrollWait);
                }
                else if (Time.realtimeSinceStartup >= nextScrollStepTime)
                {
                    flag = true;
                    nextScrollStepTime = Time.realtimeSinceStartup + (0.001f * scrollWait);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    HandleUtility.Repaint();
                }
            }
            return flag;
        }

        private enum DragSelectionState
        {
            None,
            DragSelecting,
            Dragging
        }

        private class MinMaxSliderState
        {
            public float dragEndLimit;
            public float dragStartLimit;
            public float dragStartPos;
            public float dragStartSize;
            public float dragStartValue;
            public float dragStartValuesPerPixel;
            public int whereWeDrag = -1;
        }

        private class Styles
        {
            public GUIStyle selectionRect = "SelectionRect";
        }
    }
}

