namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class Chart
    {
        private const int kCloseButtonSize = 13;
        private const int kDistFromTopToFirstLabel = 20;
        private const int kLabelHeight = 11;
        private const float kLabelXOffset = 40f;
        public const float kSideWidth = 170f;
        private const float kWarningLabelHeightOffset = 43f;
        private Vector3[] m_CachedLineData;
        private int[] m_ChartOrderBackup;
        private string m_ChartSettingsName;
        private Vector2 m_DragDownPos;
        private int m_DragItemIndex = -1;
        private int m_MouseDownIndex = -1;
        public string m_NotSupportedWarning;
        private static Styles ms_Styles = null;
        private static int s_ChartHash = "Charts".GetHashCode();

        private static void CorrectLabelPositions(float[] ypositions, float[] heights, float maxHeight)
        {
            int num = 5;
            for (int i = 0; i < num; i++)
            {
                bool flag = false;
                for (int j = 0; j < ypositions.Length; j++)
                {
                    if (heights[j] > 0f)
                    {
                        float num4 = heights[j] / 2f;
                        for (int k = j + 2; k < ypositions.Length; k += 2)
                        {
                            if (heights[k] > 0f)
                            {
                                float f = ypositions[j] - ypositions[k];
                                float num7 = (heights[j] + heights[k]) / 2f;
                                if (Mathf.Abs(f) < num7)
                                {
                                    f = ((num7 - Mathf.Abs(f)) / 2f) * Mathf.Sign(f);
                                    ypositions[j] += f;
                                    ypositions[k] -= f;
                                    flag = true;
                                }
                            }
                        }
                        if ((ypositions[j] + num4) > maxHeight)
                        {
                            ypositions[j] = maxHeight - num4;
                        }
                        if ((ypositions[j] - num4) < 0f)
                        {
                            ypositions[j] = num4;
                        }
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
        }

        private int DoFrameSelectionDrag(float x, Rect r, ChartData cdata, int len)
        {
            int num = Mathf.RoundToInt((((x - r.x) / r.width) * len) - 0.5f);
            GUI.changed = true;
            return Mathf.Clamp(num + cdata.firstFrame, cdata.firstSelectableFrame, cdata.firstFrame + len);
        }

        public int DoGUI(ChartType type, int selectedFrame, ChartData cdata, ProfilerArea area, bool active, GUIContent icon, out ChartAction action)
        {
            action = ChartAction.None;
            if (cdata != null)
            {
                int numberOfFrames = cdata.NumberOfFrames;
                if (ms_Styles == null)
                {
                    ms_Styles = new Styles();
                }
                int controlID = GUIUtility.GetControlID(s_ChartHash, FocusType.Keyboard);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(120f) };
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, ms_Styles.background, options);
                Rect chartFrame = rect;
                chartFrame.x += 170f;
                chartFrame.width -= 170f;
                Event current = Event.current;
                if ((current.GetTypeForControl(controlID) == EventType.MouseDown) && rect.Contains(current.mousePosition))
                {
                    action = ChartAction.Activated;
                }
                if (this.m_DragItemIndex == -1)
                {
                    selectedFrame = this.HandleFrameSelectionEvents(selectedFrame, controlID, chartFrame, cdata, numberOfFrames);
                }
                Rect position = chartFrame;
                position.x -= 170f;
                position.width = 170f;
                GUI.Label(new Rect(position.x, position.y, position.width, 20f), GUIContent.Temp(string.Empty, icon.tooltip));
                if (current.type == EventType.Repaint)
                {
                    ms_Styles.rightPane.Draw(chartFrame, false, false, active, false);
                    ms_Styles.leftPane.Draw(position, EditorGUIUtility.TempContent(icon.text), false, false, active, false);
                    if (this.m_NotSupportedWarning == null)
                    {
                        chartFrame.height--;
                        if (type == ChartType.StackedFill)
                        {
                            this.DrawChartStacked(selectedFrame, cdata, chartFrame);
                        }
                        else
                        {
                            this.DrawChartLine(selectedFrame, cdata, chartFrame);
                        }
                    }
                    else
                    {
                        Rect rect4 = chartFrame;
                        rect4.x += 56.1f;
                        rect4.y += 43f;
                        GUI.Label(rect4, this.m_NotSupportedWarning, EditorStyles.boldLabel);
                    }
                    position.x += 10f;
                    position.y += 10f;
                    GUIStyle.none.Draw(position, EditorGUIUtility.TempContent(icon.image), false, false, false, false);
                    position.x += 40f;
                    this.DrawLabelDragger(type, position, cdata);
                }
                else
                {
                    position.y += 10f;
                    this.LabelDraggerDrag(controlID, type, cdata, position, active);
                }
                if (area == ProfilerArea.GPU)
                {
                    GUI.Label(new Rect((rect.x + 170f) - ms_Styles.performanceWarning.image.width, (rect.yMax - ms_Styles.performanceWarning.image.height) - 2f, (float) ms_Styles.performanceWarning.image.width, (float) ms_Styles.performanceWarning.image.height), ms_Styles.performanceWarning);
                }
                if (GUI.Button(new Rect(((rect.x + 170f) - 13f) - 2f, rect.y + 2f, 13f, 13f), GUIContent.none, ms_Styles.closeButton))
                {
                    action = ChartAction.Closed;
                }
            }
            return selectedFrame;
        }

        internal static void DoLabel(float x, float y, string text, float alignment)
        {
            if (!string.IsNullOrEmpty(text))
            {
                GUIContent content = new GUIContent(text);
                Vector2 vector = ms_Styles.whiteLabel.CalcSize(content);
                Rect position = new Rect(x + (vector.x * alignment), y, vector.x, vector.y);
                EditorGUI.DoDropShadowLabel(position, content, ms_Styles.whiteLabel, 0.3f);
            }
        }

        private void DrawChartItemLine(Rect r, ChartData cdata, int index)
        {
            if (cdata.charts[index].enabled)
            {
                Color color = cdata.charts[index].color;
                int numberOfFrames = cdata.NumberOfFrames;
                int num2 = -cdata.firstFrame;
                num2 = Mathf.Clamp(num2, 0, numberOfFrames);
                int actualNumberOfPoints = numberOfFrames - num2;
                if (actualNumberOfPoints > 0)
                {
                    if ((this.m_CachedLineData == null) || (numberOfFrames > this.m_CachedLineData.Length))
                    {
                        this.m_CachedLineData = new Vector3[numberOfFrames];
                    }
                    float num4 = r.width / ((float) numberOfFrames);
                    float num5 = (r.x + (num4 * 0.5f)) + (num2 * num4);
                    float height = r.height;
                    float y = r.y;
                    int num8 = num2;
                    while (num8 < numberOfFrames)
                    {
                        float num9 = 0f;
                        num9 = y + height;
                        if (cdata.charts[index].data[num8] != -1f)
                        {
                            float num10 = (cdata.charts[index].data[num8] * cdata.scale[index]) * height;
                            num9 -= num10;
                        }
                        this.m_CachedLineData[num8 - num2].Set(num5, num9, 0f);
                        num8++;
                        num5 += num4;
                    }
                    Handles.color = color;
                    Handles.DrawAAPolyLine(2f, actualNumberOfPoints, this.m_CachedLineData);
                }
            }
        }

        private void DrawChartItemStacked(Rect r, int index, ChartData cdata, float[] sumbuf)
        {
            int numberOfFrames = cdata.NumberOfFrames;
            float num2 = r.width / ((float) numberOfFrames);
            index = cdata.chartOrder[index];
            if (cdata.charts[index].enabled)
            {
                Color c = cdata.charts[index].color;
                if (cdata.hasOverlay)
                {
                    c.r *= 0.9f;
                    c.g *= 0.9f;
                    c.b *= 0.9f;
                    c.a *= 0.4f;
                }
                Color color2 = c;
                color2.r *= 0.8f;
                color2.g *= 0.8f;
                color2.b *= 0.8f;
                color2.a *= 0.8f;
                GL.Begin(5);
                float x = r.x + (num2 * 0.5f);
                float height = r.height;
                float y = r.y;
                int num6 = 0;
                while (num6 < numberOfFrames)
                {
                    float num7 = (y + height) - sumbuf[num6];
                    float num8 = cdata.charts[index].data[num6];
                    if (num8 != -1f)
                    {
                        float num9 = (num8 * cdata.scale[0]) * height;
                        if ((num7 - num9) < r.yMin)
                        {
                            num9 = num7 - r.yMin;
                        }
                        GL.Color(c);
                        GL.Vertex3(x, num7 - num9, 0f);
                        GL.Color(color2);
                        GL.Vertex3(x, num7, 0f);
                        sumbuf[num6] += num9;
                    }
                    num6++;
                    x += num2;
                }
                GL.End();
            }
        }

        private void DrawChartItemStackedOverlay(Rect r, int index, ChartData cdata, float[] sumbuf)
        {
            int numberOfFrames = cdata.NumberOfFrames;
            float num2 = r.width / ((float) numberOfFrames);
            index = cdata.chartOrder[index];
            if (cdata.charts[index].enabled)
            {
                Color c = cdata.charts[index].color;
                Color color2 = c;
                color2.r *= 0.8f;
                color2.g *= 0.8f;
                color2.b *= 0.8f;
                color2.a *= 0.8f;
                GL.Begin(5);
                float x = r.x + (num2 * 0.5f);
                float height = r.height;
                float y = r.y;
                int num6 = 0;
                while (num6 < numberOfFrames)
                {
                    float num7 = (y + height) - sumbuf[num6];
                    float num8 = cdata.charts[index].overlayData[num6];
                    if (num8 != -1f)
                    {
                        float num9 = (num8 * cdata.scale[0]) * height;
                        GL.Color(c);
                        GL.Vertex3(x, num7 - num9, 0f);
                        GL.Color(color2);
                        GL.Vertex3(x, num7, 0f);
                    }
                    num6++;
                    x += num2;
                }
                GL.End();
            }
        }

        private void DrawChartLine(int selectedFrame, ChartData cdata, Rect r)
        {
            for (int i = 0; i < cdata.charts.Length; i++)
            {
                this.DrawChartItemLine(r, cdata, i);
            }
            if (cdata.maxValue > 0f)
            {
                this.DrawMaxValueScale(cdata, r);
            }
            this.DrawSelectedFrame(selectedFrame, cdata, r);
            this.DrawLabelsLine(selectedFrame, cdata, r);
        }

        private void DrawChartStacked(int selectedFrame, ChartData cdata, Rect r)
        {
            HandleUtility.ApplyWireMaterial();
            float[] sumbuf = new float[cdata.NumberOfFrames];
            for (int i = 0; i < cdata.charts.Length; i++)
            {
                if (cdata.hasOverlay)
                {
                    this.DrawChartItemStackedOverlay(r, i, cdata, sumbuf);
                }
                this.DrawChartItemStacked(r, i, cdata, sumbuf);
            }
            this.DrawSelectedFrame(selectedFrame, cdata, r);
            this.DrawGridStacked(r, cdata);
            this.DrawLabelsStacked(selectedFrame, cdata, r);
            if (cdata.hasOverlay)
            {
                string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
                if (selectedPropertyPath.Length > 0)
                {
                    int num2 = selectedPropertyPath.LastIndexOf('/');
                    if (num2 != -1)
                    {
                        selectedPropertyPath = selectedPropertyPath.Substring(num2 + 1);
                    }
                    GUIContent content = EditorGUIUtility.TempContent("Selected: " + selectedPropertyPath);
                    Vector2 vector = EditorStyles.whiteBoldLabel.CalcSize(content);
                    EditorGUI.DropShadowLabel(new Rect(((r.x + r.width) - vector.x) - 3f, r.y + 3f, vector.x, vector.y), content, ms_Styles.selectedLabel);
                }
            }
        }

        private void DrawGridStacked(Rect r, ChartData cdata)
        {
            if ((cdata.grid != null) && (cdata.gridLabels != null))
            {
                GL.Begin(1);
                GL.Color(new Color(1f, 1f, 1f, 0.2f));
                for (int i = 0; i < cdata.grid.Length; i++)
                {
                    float y = (r.y + r.height) - ((cdata.grid[i] * cdata.scale[0]) * r.height);
                    if (y > r.y)
                    {
                        GL.Vertex3(r.x + 80f, y, 0f);
                        GL.Vertex3(r.x + r.width, y, 0f);
                    }
                }
                GL.End();
                for (int j = 0; j < cdata.grid.Length; j++)
                {
                    float num4 = (r.y + r.height) - ((cdata.grid[j] * cdata.scale[0]) * r.height);
                    if (num4 > r.y)
                    {
                        DoLabel(r.x + 5f, num4 - 8f, cdata.gridLabels[j], 0f);
                    }
                }
            }
        }

        private void DrawLabelDragger(ChartType type, Rect r, ChartData cdata)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            if (type == ChartType.StackedFill)
            {
                int num = 0;
                int index = cdata.charts.Length - 1;
                while (index >= 0)
                {
                    Rect position = (this.m_DragItemIndex != index) ? new Rect(r.x, (r.y + 20f) + (num * 11), 170f, 11f) : new Rect(r.x, mousePosition.y - this.m_DragDownPos.y, 170f, 11f);
                    if (cdata.charts[cdata.chartOrder[index]].enabled)
                    {
                        GUI.backgroundColor = cdata.charts[cdata.chartOrder[index]].color;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.black;
                    }
                    GUI.Label(position, cdata.charts[cdata.chartOrder[index]].name, ms_Styles.paneSubLabel);
                    index--;
                    num++;
                }
            }
            else
            {
                for (int i = 0; i < cdata.charts.Length; i++)
                {
                    Rect rect2 = new Rect(r.x, (r.y + 20f) + (i * 11), 170f, 11f);
                    GUI.backgroundColor = cdata.charts[i].color;
                    GUI.Label(rect2, cdata.charts[i].name, ms_Styles.paneSubLabel);
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawLabelsLine(int selectedFrame, ChartData cdata, Rect r)
        {
            if (cdata.selectedLabels != null)
            {
                int numberOfFrames = cdata.NumberOfFrames;
                if ((selectedFrame >= cdata.firstSelectableFrame) && (selectedFrame < (cdata.firstFrame + numberOfFrames)))
                {
                    selectedFrame -= cdata.firstFrame;
                    float[] ypositions = new float[cdata.charts.Length];
                    float[] heights = new float[ypositions.Length];
                    for (int i = 0; i < cdata.charts.Length; i++)
                    {
                        ypositions[i] = -1f;
                        heights[i] = 0f;
                        float num3 = cdata.charts[i].data[selectedFrame];
                        if (num3 != -1f)
                        {
                            ypositions[i] = (num3 * cdata.scale[i]) * r.height;
                            heights[i] = GetLabelHeight(cdata.selectedLabels[i]);
                        }
                    }
                    CorrectLabelPositions(ypositions, heights, r.height);
                    float num4 = r.width / ((float) numberOfFrames);
                    float num5 = r.x + (num4 * selectedFrame);
                    for (int j = 0; j < cdata.charts.Length; j++)
                    {
                        if (heights[j] > 0f)
                        {
                            GUI.contentColor = (Color) ((cdata.charts[j].color + Color.white) * 0.5f);
                            float alignment = ((j & 1) != 0) ? ((float) 0) : ((float) (-1));
                            float num8 = ((j & 1) != 0) ? (num4 + 1f) : -1f;
                            DoLabel(num5 + num8, ((r.y + r.height) - ypositions[j]) - 8f, cdata.selectedLabels[j], alignment);
                        }
                    }
                    GUI.contentColor = Color.white;
                }
            }
        }

        private void DrawLabelsStacked(int selectedFrame, ChartData cdata, Rect r)
        {
            if (cdata.selectedLabels != null)
            {
                int numberOfFrames = cdata.NumberOfFrames;
                if ((selectedFrame >= cdata.firstSelectableFrame) && (selectedFrame < (cdata.firstFrame + numberOfFrames)))
                {
                    selectedFrame -= cdata.firstFrame;
                    float num2 = r.width / ((float) numberOfFrames);
                    float num3 = r.x + (num2 * selectedFrame);
                    float num4 = cdata.scale[0] * r.height;
                    float[] ypositions = new float[cdata.charts.Length];
                    float[] heights = new float[ypositions.Length];
                    float num5 = 0f;
                    for (int i = 0; i < cdata.charts.Length; i++)
                    {
                        ypositions[i] = -1f;
                        heights[i] = 0f;
                        int index = cdata.chartOrder[i];
                        if (cdata.charts[index].enabled)
                        {
                            float num8 = cdata.charts[index].data[selectedFrame];
                            if (num8 != -1f)
                            {
                                float num9 = !cdata.hasOverlay ? num8 : cdata.charts[index].overlayData[selectedFrame];
                                if ((num9 * num4) > 5f)
                                {
                                    ypositions[i] = (num5 + (num9 * 0.5f)) * num4;
                                    heights[i] = GetLabelHeight(cdata.selectedLabels[index]);
                                }
                                num5 += num8;
                            }
                        }
                    }
                    CorrectLabelPositions(ypositions, heights, r.height);
                    for (int j = 0; j < cdata.charts.Length; j++)
                    {
                        if (heights[j] > 0f)
                        {
                            int num11 = cdata.chartOrder[j];
                            GUI.contentColor = (Color) ((cdata.charts[num11].color * 0.8f) + (Color.white * 0.2f));
                            float alignment = ((num11 & 1) != 0) ? ((float) 0) : ((float) (-1));
                            float num13 = ((num11 & 1) != 0) ? (num2 + 1f) : -1f;
                            DoLabel(num3 + num13, ((r.y + r.height) - ypositions[j]) - 8f, cdata.selectedLabels[num11], alignment);
                        }
                    }
                    GUI.contentColor = Color.white;
                }
            }
        }

        private void DrawMaxValueScale(ChartData cdata, Rect r)
        {
            Handles.Label(new Vector3((r.x + (r.width / 2f)) - 20f, r.yMin + 2f, 0f), "Scale: " + cdata.maxValue);
        }

        private void DrawSelectedFrame(int selectedFrame, ChartData cdata, Rect r)
        {
            if ((cdata.firstSelectableFrame != -1) && ((selectedFrame - cdata.firstSelectableFrame) >= 0))
            {
                float numberOfFrames = cdata.NumberOfFrames;
                selectedFrame -= cdata.firstFrame;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(7);
                GL.Color(new Color(1f, 1f, 1f, 0.6f));
                GL.Vertex3(r.x + ((r.width / numberOfFrames) * selectedFrame), r.y + 1f, 0f);
                GL.Vertex3((r.x + ((r.width / numberOfFrames) * selectedFrame)) + (r.width / numberOfFrames), r.y + 1f, 0f);
                GL.Color(new Color(1f, 1f, 1f, 0.7f));
                GL.Vertex3((r.x + ((r.width / numberOfFrames) * selectedFrame)) + (r.width / numberOfFrames), r.yMax, 0f);
                GL.Vertex3(r.x + ((r.width / numberOfFrames) * selectedFrame), r.yMax, 0f);
                GL.End();
            }
        }

        private static float GetLabelHeight(string text)
        {
            GUIContent content = new GUIContent(text);
            return ms_Styles.whiteLabel.CalcSize(content).y;
        }

        private int HandleFrameSelectionEvents(int selectedFrame, int chartControlID, Rect chartFrame, ChartData cdata, int len)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (chartFrame.Contains(current.mousePosition))
                    {
                        GUIUtility.keyboardControl = chartControlID;
                        GUIUtility.hotControl = chartControlID;
                        selectedFrame = this.DoFrameSelectionDrag(current.mousePosition.x, chartFrame, cdata, len);
                        current.Use();
                    }
                    return selectedFrame;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == chartControlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    return selectedFrame;

                case EventType.MouseMove:
                    return selectedFrame;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == chartControlID)
                    {
                        selectedFrame = this.DoFrameSelectionDrag(current.mousePosition.x, chartFrame, cdata, len);
                        current.Use();
                    }
                    return selectedFrame;

                case EventType.KeyDown:
                    if ((GUIUtility.keyboardControl == chartControlID) && (selectedFrame >= 0))
                    {
                        if (current.keyCode == KeyCode.LeftArrow)
                        {
                            selectedFrame = this.MoveSelectedFrame(selectedFrame, cdata, -1);
                            current.Use();
                            return selectedFrame;
                        }
                        if (current.keyCode == KeyCode.RightArrow)
                        {
                            selectedFrame = this.MoveSelectedFrame(selectedFrame, cdata, 1);
                            current.Use();
                        }
                        return selectedFrame;
                    }
                    return selectedFrame;
            }
            return selectedFrame;
        }

        private void LabelDraggerDrag(int chartControlID, ChartType chartType, ChartData cdata, Rect r, bool active)
        {
            if ((chartType != ChartType.Line) && active)
            {
                Event current = Event.current;
                EventType typeForControl = current.GetTypeForControl(chartControlID);
                switch (typeForControl)
                {
                    case EventType.MouseDown:
                    case EventType.MouseUp:
                    case EventType.KeyDown:
                    case EventType.MouseDrag:
                    {
                        if (((typeForControl == EventType.KeyDown) && (current.keyCode == KeyCode.Escape)) && (this.m_DragItemIndex != -1))
                        {
                            GUIUtility.hotControl = 0;
                            Array.Copy(this.m_ChartOrderBackup, cdata.chartOrder, this.m_ChartOrderBackup.Length);
                            this.m_DragItemIndex = -1;
                            current.Use();
                        }
                        int num = 0;
                        int index = cdata.charts.Length - 1;
                        while (index >= 0)
                        {
                            if (((current.type == EventType.MouseUp) && (this.m_MouseDownIndex != -1)) || (current.type == EventType.MouseDown))
                            {
                                Rect rect = new Rect((r.x + 10f) + 40f, (r.y + 20f) + (num * 11), 9f, 9f);
                                if (rect.Contains(current.mousePosition))
                                {
                                    this.m_DragItemIndex = -1;
                                    if ((current.type == EventType.MouseUp) && (this.m_MouseDownIndex == index))
                                    {
                                        this.m_MouseDownIndex = -1;
                                        cdata.charts[cdata.chartOrder[index]].enabled = !cdata.charts[cdata.chartOrder[index]].enabled;
                                        if (chartType == ChartType.StackedFill)
                                        {
                                            this.SaveChartsSettingsEnabled(cdata);
                                        }
                                    }
                                    else
                                    {
                                        this.m_MouseDownIndex = index;
                                    }
                                    current.Use();
                                }
                            }
                            if (current.type == EventType.MouseDown)
                            {
                                Rect rect2 = new Rect(r.x, (r.y + 20f) + (num * 11), 170f, 11f);
                                if (rect2.Contains(current.mousePosition))
                                {
                                    this.m_MouseDownIndex = -1;
                                    this.m_DragItemIndex = index;
                                    this.m_DragDownPos = current.mousePosition;
                                    this.m_DragDownPos.x -= rect2.x;
                                    this.m_DragDownPos.y -= rect2.y;
                                    this.m_ChartOrderBackup = new int[cdata.chartOrder.Length];
                                    Array.Copy(cdata.chartOrder, this.m_ChartOrderBackup, this.m_ChartOrderBackup.Length);
                                    GUIUtility.hotControl = chartControlID;
                                    Event.current.Use();
                                }
                            }
                            else if (((this.m_DragItemIndex != -1) && (typeForControl == EventType.MouseDrag)) && (index != this.m_DragItemIndex))
                            {
                                float y = current.mousePosition.y;
                                float num4 = (r.y + 20f) + (num * 11);
                                if ((y >= num4) && (y < (num4 + 11f)))
                                {
                                    int num5 = cdata.chartOrder[index];
                                    cdata.chartOrder[index] = cdata.chartOrder[this.m_DragItemIndex];
                                    cdata.chartOrder[this.m_DragItemIndex] = num5;
                                    this.m_DragItemIndex = index;
                                    this.SaveChartsSettingsOrder(cdata);
                                }
                            }
                            index--;
                            num++;
                        }
                        if ((typeForControl == EventType.MouseDrag) && (this.m_DragItemIndex != -1))
                        {
                            current.Use();
                        }
                        if ((typeForControl == EventType.MouseUp) && (GUIUtility.hotControl == chartControlID))
                        {
                            GUIUtility.hotControl = 0;
                            this.m_DragItemIndex = -1;
                            current.Use();
                        }
                        break;
                    }
                }
            }
        }

        public void LoadAndBindSettings(string chartSettingsName, ChartData cdata)
        {
            this.m_ChartSettingsName = chartSettingsName;
            this.LoadChartsSettings(cdata);
        }

        private void LoadChartsSettings(ChartData cdata)
        {
            if (!string.IsNullOrEmpty(this.m_ChartSettingsName))
            {
                string str = EditorPrefs.GetString(this.m_ChartSettingsName + "Order");
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        char[] separator = new char[] { ',' };
                        string[] strArray = str.Split(separator);
                        if (strArray.Length == cdata.charts.Length)
                        {
                            for (int j = 0; j < cdata.charts.Length; j++)
                            {
                                cdata.chartOrder[j] = int.Parse(strArray[j]);
                            }
                        }
                    }
                    catch (FormatException)
                    {
                    }
                }
                str = EditorPrefs.GetString(this.m_ChartSettingsName + "Visible");
                for (int i = 0; i < cdata.charts.Length; i++)
                {
                    if ((i < str.Length) && (str[i] == '0'))
                    {
                        cdata.charts[i].enabled = false;
                    }
                }
            }
        }

        private int MoveSelectedFrame(int selectedFrame, ChartData cdata, int direction)
        {
            int numberOfFrames = cdata.NumberOfFrames;
            int num2 = selectedFrame + direction;
            if ((num2 >= cdata.firstSelectableFrame) && (num2 <= (cdata.firstFrame + numberOfFrames)))
            {
                return num2;
            }
            return selectedFrame;
        }

        private void SaveChartsSettingsEnabled(ChartData cdata)
        {
            string str = string.Empty;
            for (int i = 0; i < cdata.charts.Length; i++)
            {
                str = str + (!cdata.charts[i].enabled ? '0' : '1');
            }
            EditorPrefs.SetString(this.m_ChartSettingsName + "Visible", str);
        }

        private void SaveChartsSettingsOrder(ChartData cdata)
        {
            if (!string.IsNullOrEmpty(this.m_ChartSettingsName))
            {
                string str = string.Empty;
                for (int i = 0; i < cdata.charts.Length; i++)
                {
                    if (str.Length != 0)
                    {
                        str = str + ",";
                    }
                    str = str + cdata.chartOrder[i];
                }
                EditorPrefs.SetString(this.m_ChartSettingsName + "Order", str);
            }
        }

        internal enum ChartAction
        {
            None,
            Activated,
            Closed
        }

        internal enum ChartType
        {
            StackedFill,
            Line
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public GUIStyle closeButton = "WinBtnClose";
            public GUIStyle leftPane = "ProfilerLeftPane";
            public GUIStyle paneSubLabel = "ProfilerPaneSubLabel";
            public GUIContent performanceWarning = new GUIContent(string.Empty, EditorGUIUtility.LoadIcon("console.warnicon.sml"), "Collecting GPU Profiler data might have overhead. Close graph if you don't need its data");
            public GUIStyle rightPane = "ProfilerRightPane";
            public GUIStyle selectedLabel = "ProfilerSelectedLabel";
            public GUIStyle whiteLabel = "ProfilerBadge";
        }
    }
}

