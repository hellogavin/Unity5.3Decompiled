namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class TimeArea : ZoomableArea
    {
        internal const int kTickRulerDistFull = 80;
        internal const int kTickRulerDistLabel = 40;
        internal const int kTickRulerDistMin = 3;
        internal const float kTickRulerFatThreshold = 0.5f;
        internal const float kTickRulerHeightMax = 0.7f;
        [SerializeField]
        private TickHandler m_HTicks;
        private CurveEditorSettings m_Settings;
        [SerializeField]
        private TickHandler m_VTicks;
        private static float s_OriginalTime;
        private static float s_PickOffset;
        private static Styles2 styles;

        public TimeArea(bool minimalGUI) : base(minimalGUI)
        {
            this.m_Settings = new CurveEditorSettings();
            float[] tickModulos = new float[] { 
                1E-07f, 5E-07f, 1E-06f, 5E-06f, 1E-05f, 5E-05f, 0.0001f, 0.0005f, 0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1f, 5f, 
                10f, 50f, 100f, 500f, 1000f, 5000f, 10000f, 50000f, 100000f, 500000f, 1000000f, 5000000f, 1E+07f
             };
            this.hTicks = new TickHandler();
            this.hTicks.SetTickModulos(tickModulos);
            this.vTicks = new TickHandler();
            this.vTicks.SetTickModulos(tickModulos);
        }

        protected virtual void ApplySettings()
        {
            base.hRangeLocked = this.settings.hRangeLocked;
            base.vRangeLocked = this.settings.vRangeLocked;
            base.hRangeMin = this.settings.hRangeMin;
            base.hRangeMax = this.settings.hRangeMax;
            base.vRangeMin = this.settings.vRangeMin;
            base.vRangeMax = this.settings.vRangeMax;
            base.scaleWithWindow = this.settings.scaleWithWindow;
            base.hSlider = this.settings.hSlider;
            base.vSlider = this.settings.vSlider;
        }

        public TimeRulerDragMode BrowseRuler(Rect position, ref float time, float frameRate, bool pickAnywhere, GUIStyle thumbStyle)
        {
            int controlID = GUIUtility.GetControlID(0x2fb605, FocusType.Passive);
            return this.BrowseRuler(position, controlID, ref time, frameRate, pickAnywhere, thumbStyle);
        }

        public TimeRulerDragMode BrowseRuler(Rect position, int id, ref float time, float frameRate, bool pickAnywhere, GUIStyle thumbStyle)
        {
            Event current = Event.current;
            Rect rect = position;
            if (time != -1f)
            {
                rect.x = Mathf.Round(base.TimeToPixel(time, position)) - thumbStyle.overflow.left;
                rect.width = thumbStyle.fixedWidth + thumbStyle.overflow.horizontal;
            }
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (!rect.Contains(current.mousePosition))
                    {
                        if (pickAnywhere && position.Contains(current.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            float num = SnapTimeToWholeFPS(base.PixelToTime(current.mousePosition.x, position), frameRate);
                            s_OriginalTime = time;
                            if (num != time)
                            {
                                GUI.changed = true;
                            }
                            time = num;
                            s_PickOffset = 0f;
                            current.Use();
                            return TimeRulerDragMode.Start;
                        }
                        break;
                    }
                    GUIUtility.hotControl = id;
                    s_PickOffset = current.mousePosition.x - base.TimeToPixel(time, position);
                    current.Use();
                    return TimeRulerDragMode.Start;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    GUIUtility.hotControl = 0;
                    current.Use();
                    return TimeRulerDragMode.End;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    float num2 = SnapTimeToWholeFPS(base.PixelToTime(current.mousePosition.x - s_PickOffset, position), frameRate);
                    if (num2 != time)
                    {
                        GUI.changed = true;
                    }
                    time = num2;
                    current.Use();
                    return TimeRulerDragMode.Dragging;
                }
                case EventType.KeyDown:
                    if ((GUIUtility.hotControl != id) || (current.keyCode != KeyCode.Escape))
                    {
                        break;
                    }
                    if (time != s_OriginalTime)
                    {
                        GUI.changed = true;
                    }
                    time = s_OriginalTime;
                    GUIUtility.hotControl = 0;
                    current.Use();
                    return TimeRulerDragMode.Cancel;

                case EventType.Repaint:
                    if (time != -1f)
                    {
                        bool flag = position.Contains(current.mousePosition);
                        rect.x += thumbStyle.overflow.left;
                        thumbStyle.Draw(rect, id == GUIUtility.hotControl, flag || (id == GUIUtility.hotControl), false, false);
                    }
                    break;
            }
            return TimeRulerDragMode.None;
        }

        private void DrawLine(Vector2 lhs, Vector2 rhs)
        {
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
        }

        public void DrawMajorTicks(Rect position, float frameRate)
        {
            Color color = Handles.color;
            GUI.BeginGroup(position);
            if (Event.current.type != EventType.Repaint)
            {
                GUI.EndGroup();
            }
            else
            {
                InitStyles();
                this.SetTickMarkerRanges();
                this.hTicks.SetTickStrengths(3f, 80f, true);
                Color textColor = styles.TimelineTick.normal.textColor;
                textColor.a = 0.1f;
                Handles.color = textColor;
                for (int i = 0; i < this.hTicks.tickLevels; i++)
                {
                    float num2 = this.hTicks.GetStrengthOfLevel(i) * 0.9f;
                    if (num2 > 0.5f)
                    {
                        float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                        for (int j = 0; j < ticksAtLevel.Length; j++)
                        {
                            if (ticksAtLevel[j] >= 0f)
                            {
                                int num4 = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                                float x = this.FrameToPixel((float) num4, frameRate, position);
                                Handles.DrawLine(new Vector3(x, 0f, 0f), new Vector3(x, position.height, 0f));
                            }
                        }
                    }
                }
                GUI.EndGroup();
                Handles.color = color;
            }
        }

        public static void DrawVerticalLine(float x, float minY, float maxY, Color color)
        {
            HandleUtility.ApplyWireMaterial();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GL.Begin(7);
            }
            else
            {
                GL.Begin(1);
            }
            DrawVerticalLineFast(x, minY, maxY, color);
            GL.End();
        }

        public static void DrawVerticalLineFast(float x, float minY, float maxY, Color color)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x - 0.5f, minY, 0f));
                GL.Vertex(new Vector3(x + 0.5f, minY, 0f));
                GL.Vertex(new Vector3(x + 0.5f, maxY, 0f));
                GL.Vertex(new Vector3(x - 0.5f, maxY, 0f));
            }
            else
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x, minY, 0f));
                GL.Vertex(new Vector3(x, maxY, 0f));
            }
        }

        public string FormatFrame(int frame, float frameRate)
        {
            int num2 = (int) frameRate;
            int length = num2.ToString().Length;
            string str = string.Empty;
            if (frame < 0)
            {
                str = "-";
                frame = -frame;
            }
            int num3 = frame / ((int) frameRate);
            float num4 = ((float) frame) % frameRate;
            return (str + num3.ToString() + ":" + num4.ToString().PadLeft(length, '0'));
        }

        public float FrameToPixel(float i, float frameRate, Rect rect)
        {
            return (((i - (base.shownArea.xMin * frameRate)) * rect.width) / (base.shownArea.width * frameRate));
        }

        private static void InitStyles()
        {
            if (styles == null)
            {
                styles = new Styles2();
            }
        }

        public void SetTickMarkerRanges()
        {
            this.hTicks.SetRanges(base.shownArea.xMin, base.shownArea.xMax, base.drawRect.xMin, base.drawRect.xMax);
            this.vTicks.SetRanges(base.shownArea.yMin, base.shownArea.yMax, base.drawRect.yMin, base.drawRect.yMax);
        }

        public void SetTransform(Vector2 newTranslation, Vector2 newScale)
        {
            base.m_Scale = newScale;
            base.m_Translation = newTranslation;
            base.EnforceScaleAndRange();
        }

        public static float SnapTimeToWholeFPS(float time, float frameRate)
        {
            if (frameRate == 0f)
            {
                return time;
            }
            return (Mathf.Round(time * frameRate) / frameRate);
        }

        public void TimeRuler(Rect position, float frameRate)
        {
            this.TimeRuler(position, frameRate, true, false, 1f);
        }

        public void TimeRuler(Rect position, float frameRate, bool labels, bool useEntireHeight, float alpha)
        {
            Color color = GUI.color;
            GUI.BeginGroup(position);
            if (Event.current.type != EventType.Repaint)
            {
                GUI.EndGroup();
            }
            else
            {
                InitStyles();
                HandleUtility.ApplyWireMaterial();
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GL.Begin(7);
                }
                else
                {
                    GL.Begin(1);
                }
                Color backgroundColor = GUI.backgroundColor;
                this.SetTickMarkerRanges();
                this.hTicks.SetTickStrengths(3f, 80f, true);
                Color textColor = styles.TimelineTick.normal.textColor;
                textColor.a = 0.75f * alpha;
                for (int i = 0; i < this.hTicks.tickLevels; i++)
                {
                    float b = this.hTicks.GetStrengthOfLevel(i) * 0.9f;
                    float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                    for (int j = 0; j < ticksAtLevel.Length; j++)
                    {
                        if ((ticksAtLevel[j] >= base.hRangeMin) && (ticksAtLevel[j] <= base.hRangeMax))
                        {
                            int num4 = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                            float num5 = !useEntireHeight ? ((position.height * Mathf.Min(1f, b)) * 0.7f) : position.height;
                            DrawVerticalLineFast(this.FrameToPixel((float) num4, frameRate, position), (position.height - num5) + 0.5f, position.height - 0.5f, new Color(1f, 1f, 1f, b / 0.5f) * textColor);
                        }
                    }
                }
                GL.End();
                if (labels)
                {
                    int levelWithMinSeparation = this.hTicks.GetLevelWithMinSeparation(40f);
                    float[] numArray2 = this.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
                    for (int k = 0; k < numArray2.Length; k++)
                    {
                        if ((numArray2[k] >= base.hRangeMin) && (numArray2[k] <= base.hRangeMax))
                        {
                            int frame = Mathf.RoundToInt(numArray2[k] * frameRate);
                            float num10 = Mathf.Floor(this.FrameToPixel((float) frame, frameRate, position));
                            string text = this.FormatFrame(frame, frameRate);
                            GUI.Label(new Rect(num10 + 3f, -3f, 40f, 20f), text, styles.TimelineTick);
                        }
                    }
                }
                GUI.EndGroup();
                GUI.backgroundColor = backgroundColor;
                GUI.color = color;
            }
        }

        public TickHandler hTicks
        {
            get
            {
                return this.m_HTicks;
            }
            set
            {
                this.m_HTicks = value;
            }
        }

        public CurveEditorSettings settings
        {
            get
            {
                return this.m_Settings;
            }
            set
            {
                if (value != null)
                {
                    this.m_Settings = value;
                    this.ApplySettings();
                }
            }
        }

        public TickHandler vTicks
        {
            get
            {
                return this.m_VTicks;
            }
            set
            {
                this.m_VTicks = value;
            }
        }

        private class Styles2
        {
            public GUIStyle labelTickMarks = "CurveEditorLabelTickMarks";
            public GUIStyle TimelineTick = "AnimationTimelineTick";
        }

        public enum TimeRulerDragMode
        {
            None,
            Start,
            End,
            Dragging,
            Cancel
        }
    }
}

