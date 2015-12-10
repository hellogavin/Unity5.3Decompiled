namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class ProfilerTimelineGUI
    {
        private float animationTime = 1f;
        private List<GroupInfo> groups;
        private const float kGroupHeight = 20f;
        private const float kLineHeight = 16f;
        private const float kSmallWidth = 7f;
        private const float kTextFadeOutWidth = 20f;
        private const float kTextFadeStartWidth = 50f;
        private const float kTextLongWidth = 200f;
        private double lastScrollUpdate;
        private float m_SelectedDur;
        private int m_SelectedID = -1;
        private int m_SelectedThread;
        private float m_SelectedTime;
        [NonSerialized]
        private ZoomableArea m_TimeArea;
        private IProfilerWindowController m_Window;
        private static Styles ms_Styles;

        public ProfilerTimelineGUI(IProfilerWindowController window)
        {
            this.m_Window = window;
            this.groups = new List<GroupInfo>();
        }

        private void CalculateBars(Rect r, int frameIndex, float time)
        {
            ProfilerFrameDataIterator iterator = new ProfilerFrameDataIterator();
            int groupCount = iterator.GetGroupCount(frameIndex);
            float num2 = 0f;
            iterator.SetRoot(frameIndex, 0);
            int threadCount = iterator.GetThreadCount(frameIndex);
            <CalculateBars>c__AnonStoreyA5 ya = new <CalculateBars>c__AnonStoreyA5 {
                i = 0
            };
            while (ya.i < threadCount)
            {
                <CalculateBars>c__AnonStoreyA4 ya2 = new <CalculateBars>c__AnonStoreyA4 {
                    <>f__ref$165 = ya
                };
                iterator.SetRoot(frameIndex, ya.i);
                ya2.groupname = iterator.GetGroupName();
                GroupInfo item = this.groups.Find(new Predicate<GroupInfo>(ya2.<>m__1EA));
                if (item == null)
                {
                    item = new GroupInfo {
                        name = ya2.groupname,
                        height = 20f,
                        expanded = false,
                        threads = new List<ThreadInfo>()
                    };
                    this.groups.Add(item);
                    if ((ya2.groupname == string.Empty) || (ya2.groupname == "Unity Job System"))
                    {
                        item.expanded = true;
                    }
                }
                ThreadInfo info2 = item.threads.Find(new Predicate<ThreadInfo>(ya2.<>m__1EB));
                if (info2 == null)
                {
                    info2 = new ThreadInfo {
                        name = iterator.GetThreadName(),
                        height = 0f,
                        weight = info2.desiredWeight = !item.expanded ? ((float) 0) : ((float) 1),
                        threadIndex = ya.i
                    };
                    item.threads.Add(info2);
                }
                if (info2.weight != info2.desiredWeight)
                {
                    info2.weight = (info2.desiredWeight * time) + ((1f - info2.desiredWeight) * (1f - time));
                }
                num2 += info2.weight;
                ya.i++;
            }
            float num4 = 16f * groupCount;
            float num5 = r.height - num4;
            float num6 = num5 / (num2 + 2f);
            foreach (GroupInfo info3 in this.groups)
            {
                foreach (ThreadInfo info4 in info3.threads)
                {
                    info4.height = num6 * info4.weight;
                }
            }
            this.groups[0].expanded = true;
            this.groups[0].height = 0f;
            this.groups[0].threads[0].height = 3f * num6;
        }

        public void DoGUI(int frameIndex, float width, float ypos, float height)
        {
            Rect position = new Rect(0f, ypos - 1f, width, height + 1f);
            float num = 169f;
            if (Event.current.type == EventType.Repaint)
            {
                styles.profilerGraphBackground.Draw(position, false, false, false, false);
                EditorStyles.toolbar.Draw(new Rect(0f, (ypos + height) - 15f, num, 15f), false, false, false, false);
            }
            bool flag = false;
            if (this.m_TimeArea == null)
            {
                flag = true;
                this.m_TimeArea = new ZoomableArea();
                this.m_TimeArea.hRangeLocked = false;
                this.m_TimeArea.vRangeLocked = true;
                this.m_TimeArea.hSlider = true;
                this.m_TimeArea.vSlider = false;
                this.m_TimeArea.scaleWithWindow = true;
                this.m_TimeArea.rect = new Rect((position.x + num) - 1f, position.y, position.width - num, position.height);
                this.m_TimeArea.margin = 10f;
            }
            ProfilerFrameDataIterator iterator = new ProfilerFrameDataIterator();
            iterator.SetRoot(frameIndex, 0);
            this.m_TimeArea.hBaseRangeMin = 0f;
            this.m_TimeArea.hBaseRangeMax = iterator.frameTimeMS;
            if (flag)
            {
                this.PerformFrameSelected(iterator.frameTimeMS);
            }
            this.m_TimeArea.rect = new Rect(position.x + num, position.y, position.width - num, position.height);
            this.m_TimeArea.BeginViewGUI();
            this.m_TimeArea.EndViewGUI();
            position = this.m_TimeArea.drawRect;
            this.CalculateBars(position, frameIndex, this.animationTime);
            this.DrawBars(position, frameIndex);
            GUI.BeginClip(this.m_TimeArea.drawRect);
            position.x = 0f;
            position.y = 0f;
            int threadCount = 0;
            this.DoProfilerFrame(frameIndex, position, false, ref threadCount, 0f);
            bool enabled = GUI.enabled;
            GUI.enabled = false;
            int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(frameIndex);
            if (previousFrameIndex != -1)
            {
                ProfilerFrameDataIterator iterator2 = new ProfilerFrameDataIterator();
                iterator2.SetRoot(previousFrameIndex, 0);
                this.DoProfilerFrame(previousFrameIndex, position, true, ref threadCount, -iterator2.frameTimeMS);
            }
            int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(frameIndex);
            if (nextFrameIndex != -1)
            {
                ProfilerFrameDataIterator iterator3 = new ProfilerFrameDataIterator();
                iterator3.SetRoot(frameIndex, 0);
                this.DoProfilerFrame(nextFrameIndex, position, true, ref threadCount, iterator3.frameTimeMS);
            }
            GUI.enabled = enabled;
            GUI.EndClip();
        }

        private void DoProfilerFrame(int frameIndex, Rect fullRect, bool ghost, ref int threadCount, float offset)
        {
            ProfilerFrameDataIterator iter = new ProfilerFrameDataIterator();
            int num = iter.GetThreadCount(frameIndex);
            if (!ghost || (num == threadCount))
            {
                iter.SetRoot(frameIndex, 0);
                if (!ghost)
                {
                    threadCount = num;
                    this.DrawGrid(fullRect, threadCount, iter.frameTimeMS);
                    this.HandleFrameSelected(iter.frameTimeMS);
                }
                float y = fullRect.y;
                foreach (GroupInfo info in this.groups)
                {
                    Rect r = fullRect;
                    bool expanded = info.expanded;
                    if (expanded)
                    {
                        y += info.height;
                    }
                    float num3 = y;
                    int count = info.threads.Count;
                    foreach (ThreadInfo info2 in info.threads)
                    {
                        iter.SetRoot(frameIndex, info2.threadIndex);
                        r.y = y;
                        r.height = !expanded ? Math.Max((float) ((info.height / ((float) count)) - 1f), (float) 2f) : info2.height;
                        this.DrawProfilingData(iter, r, info2.threadIndex, offset, ghost, expanded);
                        y += r.height;
                    }
                    if (!expanded)
                    {
                        y = num3 + info.height;
                    }
                }
            }
        }

        private bool DrawBar(Rect r, float y, float height, string name, bool group, bool expanded, bool indent)
        {
            Rect position = new Rect(r.x - 170f, y, 170f, height);
            Rect rect2 = new Rect(r.x, y, r.width, height);
            if (Event.current.type == EventType.Repaint)
            {
                styles.rightPane.Draw(rect2, false, false, false, false);
                bool flag = height < 10f;
                bool flag2 = height < 25f;
                GUIContent content = (!group && !flag) ? GUIContent.Temp(name) : GUIContent.none;
                if (flag2)
                {
                    RectOffset padding = styles.leftPane.padding;
                    padding.top -= ((int) (25f - height)) / 2;
                }
                if (indent)
                {
                    RectOffset offset2 = styles.leftPane.padding;
                    offset2.left += 10;
                }
                styles.leftPane.Draw(position, content, false, false, false, false);
                if (indent)
                {
                    RectOffset offset3 = styles.leftPane.padding;
                    offset3.left -= 10;
                }
                if (flag2)
                {
                    RectOffset offset4 = styles.leftPane.padding;
                    offset4.top += ((int) (25f - height)) / 2;
                }
            }
            if (group)
            {
                position.width--;
                position.xMin++;
                return GUI.Toggle(position, expanded, GUIContent.Temp(name), styles.foldout);
            }
            return false;
        }

        private void DrawBars(Rect r, int frameIndex)
        {
            float y = r.y;
            foreach (GroupInfo info in this.groups)
            {
                bool flag = info.name == string.Empty;
                if (!flag)
                {
                    float height = info.height;
                    bool expanded = info.expanded;
                    info.expanded = this.DrawBar(r, y, height, info.name, true, expanded, false);
                    if (info.expanded != expanded)
                    {
                        this.animationTime = 0f;
                        this.lastScrollUpdate = EditorApplication.timeSinceStartup;
                        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
                        foreach (ThreadInfo info2 in info.threads)
                        {
                            info2.desiredWeight = !info.expanded ? 0f : 1f;
                        }
                    }
                    y += height;
                }
                foreach (ThreadInfo info3 in info.threads)
                {
                    float num3 = info3.height;
                    if (num3 != 0f)
                    {
                        this.DrawBar(r, y, num3, info3.name, false, true, !flag);
                    }
                    y += num3;
                }
            }
        }

        private void DrawGrid(Rect r, int threadCount, float frameTime)
        {
            float num;
            float num3;
            float num2 = 16.66667f;
            HandleUtility.ApplyWireMaterial();
            GL.Begin(1);
            GL.Color(new Color(1f, 1f, 1f, 0.2f));
            for (num3 = num2; num3 <= frameTime; num3 += num2)
            {
                num = this.m_TimeArea.TimeToPixel(num3, r);
                GL.Vertex3(num, r.y, 0f);
                GL.Vertex3(num, r.y + r.height, 0f);
            }
            GL.Color(new Color(1f, 1f, 1f, 0.8f));
            num = this.m_TimeArea.TimeToPixel(0f, r);
            GL.Vertex3(num, r.y, 0f);
            GL.Vertex3(num, r.y + r.height, 0f);
            num = this.m_TimeArea.TimeToPixel(frameTime, r);
            GL.Vertex3(num, r.y, 0f);
            GL.Vertex3(num, r.y + r.height, 0f);
            GL.End();
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            for (num3 = 0f; num3 <= frameTime; num3 += num2)
            {
                Chart.DoLabel(this.m_TimeArea.TimeToPixel(num3, r) + 2f, r.yMax - 12f, string.Format("{0:f1}ms", num3), 0f);
            }
            GUI.color = new Color(1f, 1f, 1f, 1f);
            num3 = frameTime;
            Chart.DoLabel(this.m_TimeArea.TimeToPixel(num3, r) + 2f, r.yMax - 12f, string.Format("{0:f1}ms ({1:f0}FPS)", num3, 1000f / num3), 0f);
        }

        private void DrawProfilingData(ProfilerFrameDataIterator iter, Rect r, int threadIdx, float timeOffset, bool ghost, bool includeSubSamples)
        {
            float num = !ghost ? 7f : 21f;
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            Color[] colors = ProfilerColors.colors;
            bool flag = false;
            float num2 = -1f;
            float num3 = -1f;
            float y = -1f;
            int size = 0;
            float num6 = -1f;
            string str2 = null;
            float num7 = !includeSubSamples ? r.height : 16f;
            float num8 = !includeSubSamples ? ((float) 0) : ((float) 1);
            float height = num7 - (2f * num8);
            r.height -= num8;
            GUI.BeginGroup(r);
            float num23 = 0f;
            r.y = num23;
            r.x = num23;
            bool flag2 = (Event.current.clickCount == 1) && (Event.current.type == EventType.MouseDown);
            bool flag3 = (Event.current.clickCount == 2) && (Event.current.type == EventType.MouseDown);
            Rect shownArea = this.m_TimeArea.shownArea;
            float rectWidthDivShownWidth = r.width / shownArea.width;
            float x = r.x;
            float shownX = shownArea.x;
            bool enterChildren = true;
            while (iter.Next(enterChildren))
            {
                enterChildren = includeSubSamples;
                float time = iter.startTimeMS + timeOffset;
                float durationMS = iter.durationMS;
                float num15 = Mathf.Max(durationMS, 0.0003f);
                float num16 = TimeToPixelCached(time, rectWidthDivShownWidth, shownX, x);
                float num17 = TimeToPixelCached(time + num15, rectWidthDivShownWidth, shownX, x) - 1f;
                float width = num17 - num16;
                if ((num16 > (r.x + r.width)) || (num17 < r.x))
                {
                    enterChildren = false;
                }
                else
                {
                    float num19 = iter.depth - 1;
                    float num20 = r.y + (num19 * num7);
                    if (flag)
                    {
                        bool flag5 = false;
                        if (width >= num)
                        {
                            flag5 = true;
                        }
                        if (y != num20)
                        {
                            flag5 = true;
                        }
                        if ((num16 - num3) > 6f)
                        {
                            flag5 = true;
                        }
                        if (flag5)
                        {
                            this.DrawSmallGroup(num2, num3, y, height, size);
                            flag = false;
                        }
                    }
                    if (width < num)
                    {
                        enterChildren = false;
                        if (!flag)
                        {
                            flag = true;
                            y = num20;
                            num2 = num16;
                            size = 0;
                        }
                        num3 = num17;
                        size++;
                        continue;
                    }
                    int id = iter.id;
                    string path = iter.path;
                    bool flag6 = (path == selectedPropertyPath) && !ghost;
                    if (this.m_SelectedID >= 0)
                    {
                        flag6 &= id == this.m_SelectedID;
                    }
                    flag6 &= threadIdx == this.m_SelectedThread;
                    Color white = Color.white;
                    Color color4 = colors[iter.group % colors.Length];
                    color4.a = !flag6 ? 0.75f : 1f;
                    if (ghost)
                    {
                        color4.a = 0.4f;
                        white.a = 0.5f;
                    }
                    string name = iter.name;
                    if (flag6)
                    {
                        str2 = name;
                        this.m_SelectedTime = time;
                        this.m_SelectedDur = durationMS;
                        num6 = num20 + num7;
                    }
                    if ((width < 20f) || !includeSubSamples)
                    {
                        name = string.Empty;
                    }
                    else
                    {
                        if ((width < 50f) && !flag6)
                        {
                            white.a *= (width - 20f) / 30f;
                        }
                        if (width > 200f)
                        {
                            name = name + string.Format(" ({0:f2}ms)", durationMS);
                        }
                    }
                    GUI.color = color4;
                    GUI.contentColor = white;
                    Rect position = new Rect(num16, num20, width, height);
                    GUI.Label(position, name, styles.bar);
                    if ((flag2 || flag3) && position.Contains(Event.current.mousePosition))
                    {
                        this.m_Window.SetSelectedPropertyPath(path);
                        this.m_SelectedThread = threadIdx;
                        this.m_SelectedID = id;
                        Object gameObject = EditorUtility.InstanceIDToObject(iter.instanceId);
                        if (gameObject is Component)
                        {
                            gameObject = ((Component) gameObject).gameObject;
                        }
                        if (gameObject != null)
                        {
                            if (flag2)
                            {
                                EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                            }
                            else if (flag3)
                            {
                                Selection.objects = new List<Object> { gameObject }.ToArray();
                            }
                        }
                        Event.current.Use();
                    }
                    flag = false;
                }
            }
            if (flag)
            {
                this.DrawSmallGroup(num2, num3, y, height, size);
            }
            GUI.color = color;
            GUI.contentColor = contentColor;
            if (((str2 != null) && (threadIdx == this.m_SelectedThread)) && includeSubSamples)
            {
                Rect rect3;
                GUIContent content = new GUIContent(string.Format((this.m_SelectedDur < 1.0) ? "{0}\n{1:f3}ms" : "{0}\n{1:f2}ms", str2, this.m_SelectedDur));
                GUIStyle tooltip = styles.tooltip;
                Vector2 vector = tooltip.CalcSize(content);
                float num22 = this.m_TimeArea.TimeToPixel(this.m_SelectedTime + (this.m_SelectedDur * 0.5f), r);
                if (num22 < r.x)
                {
                    num22 = r.x + 20f;
                }
                if (num22 > r.xMax)
                {
                    num22 = r.xMax - 20f;
                }
                if (((num6 + 6f) + vector.y) < r.yMax)
                {
                    rect3 = new Rect(num22 - 32f, num6, 50f, 7f);
                    GUI.Label(rect3, GUIContent.none, styles.tooltipArrow);
                }
                rect3 = new Rect(num22, num6 + 6f, vector.x, vector.y);
                if (rect3.xMax > (r.xMax + 20f))
                {
                    rect3.x = (r.xMax - rect3.width) + 20f;
                }
                if (rect3.yMax > r.yMax)
                {
                    rect3.y = r.yMax - rect3.height;
                }
                if (rect3.y < r.y)
                {
                    rect3.y = r.y;
                }
                GUI.Label(rect3, content, tooltip);
            }
            if ((Event.current.type == EventType.MouseDown) && r.Contains(Event.current.mousePosition))
            {
                this.m_Window.ClearSelectedPropertyPath();
                this.m_SelectedID = -1;
                this.m_SelectedThread = threadIdx;
                Event.current.Use();
            }
            GUI.EndGroup();
        }

        private void DrawSmallGroup(float x1, float x2, float y, float height, int size)
        {
            if ((x2 - x1) >= 1f)
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                GUI.contentColor = Color.white;
                GUIContent none = GUIContent.none;
                if ((x2 - x1) > 20f)
                {
                    none = new GUIContent(size + " items");
                }
                GUI.Label(new Rect(x1, y, x2 - x1, height), none, styles.bar);
            }
        }

        private void HandleFrameSelected(float frameMS)
        {
            Event current = Event.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && (current.commandName == "FrameSelected"))
            {
                if (current.type == EventType.ExecuteCommand)
                {
                    this.PerformFrameSelected(frameMS);
                }
                current.Use();
            }
        }

        private void PerformFrameSelected(float frameMS)
        {
            float selectedTime = this.m_SelectedTime;
            float selectedDur = this.m_SelectedDur;
            if ((this.m_SelectedID < 0) || (selectedDur <= 0f))
            {
                selectedTime = 0f;
                selectedDur = frameMS;
            }
            this.m_TimeArea.SetShownHRangeInsideMargins(selectedTime - (selectedDur * 0.2f), selectedTime + (selectedDur * 1.2f));
        }

        private static float TimeToPixelCached(float time, float rectWidthDivShownWidth, float shownX, float rectX)
        {
            return (((time - shownX) * rectWidthDivShownWidth) + rectX);
        }

        private void UpdateAnimatedFoldout()
        {
            double num = EditorApplication.timeSinceStartup - this.lastScrollUpdate;
            this.animationTime = Math.Min((float) 1f, (float) (this.animationTime + ((float) num)));
            this.m_Window.Repaint();
            if (this.animationTime == 1f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
            }
        }

        private static Styles styles
        {
            get
            {
                if (ms_Styles == null)
                {
                }
                return (ms_Styles = new Styles());
            }
        }

        [CompilerGenerated]
        private sealed class <CalculateBars>c__AnonStoreyA4
        {
            internal ProfilerTimelineGUI.<CalculateBars>c__AnonStoreyA5 <>f__ref$165;
            internal string groupname;

            internal bool <>m__1EA(ProfilerTimelineGUI.GroupInfo g)
            {
                return (g.name == this.groupname);
            }

            internal bool <>m__1EB(ProfilerTimelineGUI.ThreadInfo t)
            {
                return (t.threadIndex == this.<>f__ref$165.i);
            }
        }

        [CompilerGenerated]
        private sealed class <CalculateBars>c__AnonStoreyA5
        {
            internal int i;
        }

        internal class GroupInfo
        {
            public bool expanded;
            public float height;
            public string name;
            public List<ProfilerTimelineGUI.ThreadInfo> threads;
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public GUIStyle bar = "ProfilerTimelineBar";
            public GUIStyle foldout = "ProfilerTimelineFoldout";
            public GUIStyle leftPane = "ProfilerTimelineLeftPane";
            public GUIStyle profilerGraphBackground = new GUIStyle("ProfilerScrollviewBackground");
            public GUIStyle rightPane = "ProfilerRightPane";
            public GUIStyle tooltip = "AnimationEventTooltip";
            public GUIStyle tooltipArrow = "AnimationEventTooltipArrow";

            internal Styles()
            {
                Texture2D whiteTexture = EditorGUIUtility.whiteTexture;
                this.bar.active.background = whiteTexture;
                this.bar.hover.background = whiteTexture;
                this.bar.normal.background = whiteTexture;
                Color black = Color.black;
                this.bar.active.textColor = black;
                this.bar.hover.textColor = black;
                this.bar.normal.textColor = black;
                this.profilerGraphBackground.overflow.left = -169;
                this.leftPane.padding.left = 15;
            }
        }

        internal class ThreadInfo
        {
            public float desiredWeight;
            public float height;
            public string name;
            public int threadIndex;
            public float weight;
        }
    }
}

