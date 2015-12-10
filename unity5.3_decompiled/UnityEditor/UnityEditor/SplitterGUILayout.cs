namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SplitterGUILayout
    {
        private static int splitterHash = "Splitter".GetHashCode();

        public static void BeginHorizontalSplit(SplitterState state, params GUILayoutOption[] options)
        {
            BeginSplit(state, GUIStyle.none, false, options);
        }

        public static void BeginHorizontalSplit(SplitterState state, GUIStyle style, params GUILayoutOption[] options)
        {
            BeginSplit(state, style, false, options);
        }

        public static void BeginSplit(SplitterState state, GUIStyle style, bool vertical, params GUILayoutOption[] options)
        {
            int num;
            GUISplitterGroup group = (GUISplitterGroup) GUILayoutUtility.BeginLayoutGroup(style, null, typeof(GUISplitterGroup));
            state.ID = GUIUtility.GetControlID(splitterHash, FocusType.Native);
            switch (Event.current.GetTypeForControl(state.ID))
            {
                case EventType.MouseDown:
                    if ((Event.current.button == 0) && (Event.current.clickCount == 1))
                    {
                        int num2 = !group.isVertical ? ((int) group.rect.x) : ((int) group.rect.y);
                        num = !group.isVertical ? ((int) Event.current.mousePosition.x) : ((int) Event.current.mousePosition.y);
                        for (int i = 0; i < (state.relativeSizes.Length - 1); i++)
                        {
                            Rect rect = !group.isVertical ? new Rect(((state.xOffset + num2) + state.realSizes[i]) - (state.splitSize / 2), group.rect.y, (float) state.splitSize, group.rect.height) : new Rect(state.xOffset + group.rect.x, (float) ((num2 + state.realSizes[i]) - (state.splitSize / 2)), group.rect.width, (float) state.splitSize);
                            if (rect.Contains(Event.current.mousePosition))
                            {
                                state.splitterInitialOffset = num;
                                state.currentActiveSplitter = i;
                                GUIUtility.hotControl = state.ID;
                                Event.current.Use();
                                break;
                            }
                            num2 += state.realSizes[i];
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == state.ID)
                    {
                        GUIUtility.hotControl = 0;
                        state.currentActiveSplitter = -1;
                        state.RealToRelativeSizes();
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == state.ID) && (state.currentActiveSplitter >= 0))
                    {
                        num = !group.isVertical ? ((int) Event.current.mousePosition.x) : ((int) Event.current.mousePosition.y);
                        int diff = num - state.splitterInitialOffset;
                        if (diff != 0)
                        {
                            state.splitterInitialOffset = num;
                            state.DoSplitter(state.currentActiveSplitter, state.currentActiveSplitter + 1, diff);
                        }
                        Event.current.Use();
                    }
                    break;

                case EventType.Repaint:
                {
                    int num5 = !group.isVertical ? ((int) group.rect.x) : ((int) group.rect.y);
                    for (int j = 0; j < (state.relativeSizes.Length - 1); j++)
                    {
                        Rect position = !group.isVertical ? new Rect(((state.xOffset + num5) + state.realSizes[j]) - (state.splitSize / 2), group.rect.y, (float) state.splitSize, group.rect.height) : new Rect(state.xOffset + group.rect.x, (float) ((num5 + state.realSizes[j]) - (state.splitSize / 2)), group.rect.width, (float) state.splitSize);
                        EditorGUIUtility.AddCursorRect(position, !group.isVertical ? MouseCursor.SplitResizeLeftRight : MouseCursor.ResizeVertical, state.ID);
                        num5 += state.realSizes[j];
                    }
                    break;
                }
                case EventType.Layout:
                    group.state = state;
                    group.resetCoords = false;
                    group.isVertical = vertical;
                    group.ApplyOptions(options);
                    break;
            }
        }

        public static void BeginVerticalSplit(SplitterState state, params GUILayoutOption[] options)
        {
            BeginSplit(state, GUIStyle.none, true, options);
        }

        public static void BeginVerticalSplit(SplitterState state, GUIStyle style, params GUILayoutOption[] options)
        {
            BeginSplit(state, style, true, options);
        }

        public static void EndHorizontalSplit()
        {
            GUILayoutUtility.EndLayoutGroup();
        }

        public static void EndVerticalSplit()
        {
            GUILayoutUtility.EndLayoutGroup();
        }

        internal class GUISplitterGroup : GUILayoutGroup
        {
            public SplitterState state;

            public override void SetHorizontal(float x, float width)
            {
                if (!base.isVertical)
                {
                    int num;
                    this.state.xOffset = x;
                    if (width != this.state.lastTotalSize)
                    {
                        this.state.RelativeToRealSizes((int) width);
                        this.state.lastTotalSize = (int) width;
                        for (num = 0; num < (this.state.realSizes.Length - 1); num++)
                        {
                            this.state.DoSplitter(num, num + 1, 0);
                        }
                    }
                    num = 0;
                    foreach (GUILayoutEntry entry in base.entries)
                    {
                        float f = this.state.realSizes[num];
                        entry.SetHorizontal(Mathf.Round(x), Mathf.Round(f));
                        x += f + base.spacing;
                        num++;
                    }
                }
                else
                {
                    base.SetHorizontal(x, width);
                }
            }

            public override void SetVertical(float y, float height)
            {
                this.rect.y = y;
                this.rect.height = height;
                RectOffset padding = base.style.padding;
                if (base.isVertical)
                {
                    int num3;
                    if (base.style != GUIStyle.none)
                    {
                        float top = padding.top;
                        float bottom = padding.bottom;
                        if (base.entries.Count != 0)
                        {
                            top = Mathf.Max(top, (float) base.entries[0].margin.top);
                            bottom = Mathf.Max(bottom, (float) base.entries[base.entries.Count - 1].margin.bottom);
                        }
                        y += top;
                        height -= bottom + top;
                    }
                    if (height != this.state.lastTotalSize)
                    {
                        this.state.RelativeToRealSizes((int) height);
                        this.state.lastTotalSize = (int) height;
                        for (num3 = 0; num3 < (this.state.realSizes.Length - 1); num3++)
                        {
                            this.state.DoSplitter(num3, num3 + 1, 0);
                        }
                    }
                    num3 = 0;
                    foreach (GUILayoutEntry entry in base.entries)
                    {
                        float f = this.state.realSizes[num3];
                        entry.SetVertical(Mathf.Round(y), Mathf.Round(f));
                        y += f + base.spacing;
                        num3++;
                    }
                }
                else if (base.style != GUIStyle.none)
                {
                    foreach (GUILayoutEntry entry2 in base.entries)
                    {
                        float num5 = Mathf.Max(entry2.margin.top, padding.top);
                        float num6 = y + num5;
                        float num7 = (height - Mathf.Max(entry2.margin.bottom, padding.bottom)) - num5;
                        if (entry2.stretchHeight != 0)
                        {
                            entry2.SetVertical(num6, num7);
                        }
                        else
                        {
                            entry2.SetVertical(num6, Mathf.Clamp(num7, entry2.minHeight, entry2.maxHeight));
                        }
                    }
                }
                else
                {
                    float num8 = y - this.margin.top;
                    float num9 = height + this.margin.vertical;
                    foreach (GUILayoutEntry entry3 in base.entries)
                    {
                        if (entry3.stretchHeight != 0)
                        {
                            entry3.SetVertical(num8 + entry3.margin.top, num9 - entry3.margin.vertical);
                        }
                        else
                        {
                            entry3.SetVertical(num8 + entry3.margin.top, Mathf.Clamp(num9 - entry3.margin.vertical, entry3.minHeight, entry3.maxHeight));
                        }
                    }
                }
            }
        }
    }
}

