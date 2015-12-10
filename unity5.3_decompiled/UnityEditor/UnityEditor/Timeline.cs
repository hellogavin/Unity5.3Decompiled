namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class Timeline
    {
        private int id = -1;
        private DragStates m_DragState;
        private float m_DstDragOffset;
        private bool m_DstLoop;
        private string m_DstName = "Right";
        private List<PivotSample> m_DstPivotList = new List<PivotSample>();
        private Vector3[] m_DstPivotVectors;
        private float m_DstStartTime = 0.25f;
        private float m_DstStopTime = 1f;
        private bool m_HasExitTime;
        private float m_LeftThumbOffset;
        private Rect m_Rect = new Rect(0f, 0f, 0f, 0f);
        private float m_RightThumbOffset;
        private float m_SampleStopTime = float.PositiveInfinity;
        private bool m_SrcLoop;
        private string m_SrcName = "Left";
        private List<PivotSample> m_SrcPivotList = new List<PivotSample>();
        private Vector3[] m_SrcPivotVectors;
        private float m_SrcStartTime;
        private float m_SrcStopTime = 0.75f;
        private float m_StartTime;
        private float m_StopTime = 1f;
        private float m_Time = float.PositiveInfinity;
        private TimeArea m_TimeArea;
        private float m_TransitionStartTime = float.PositiveInfinity;
        private float m_TransitionStopTime = float.PositiveInfinity;
        private Styles styles;

        public Timeline()
        {
            this.Init();
        }

        private Vector3 CalculatePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float num = 1f - t;
            float num2 = t * t;
            float num3 = num * num;
            float num4 = num3 * num;
            float num5 = num2 * t;
            Vector3 vector = (Vector3) (num4 * p0);
            vector += (Vector3) (((3f * num3) * t) * p1);
            vector += (Vector3) (((3f * num) * num2) * p2);
            return (vector + ((Vector3) (num5 * p3)));
        }

        private void DoPivotCurves()
        {
            Color white = Color.white;
            Color toColor = Color.white;
            Color color3 = new Color(1f, 1f, 1f, 0.1f);
            Color fromColor = new Color(1f, 1f, 1f, 0.1f);
            Color loopColor = new Color(0.75f, 0.75f, 0.75f, 0.2f);
            Color color6 = new Color(0.75f, 0.75f, 0.75f, 0.2f);
            Rect position = new Rect(0f, 18f, this.m_Rect.width, 66f);
            GUI.BeginGroup(position);
            float motionStart = this.m_TimeArea.TimeToPixel(this.SrcStartTime, position);
            float motionStop = this.m_TimeArea.TimeToPixel(this.SrcStopTime, position);
            float num3 = this.m_TimeArea.TimeToPixel(this.DstStartTime, position);
            float num4 = this.m_TimeArea.TimeToPixel(this.DstStopTime, position);
            if (this.m_SrcPivotVectors == null)
            {
                this.m_SrcPivotVectors = this.GetPivotVectors(this.m_SrcPivotList.ToArray(), motionStop - motionStart, position, position.height, this.srcLoop);
            }
            if (this.m_DstPivotVectors == null)
            {
                this.m_DstPivotVectors = this.GetPivotVectors(this.m_DstPivotList.ToArray(), num4 - num3, position, position.height, this.dstLoop);
            }
            this.m_DstPivotVectors = this.OffsetPivotVectors(this.m_DstPivotVectors, (this.m_DstDragOffset + num3) - motionStart);
            Color[] colors = this.GetPivotColors(this.m_SrcPivotVectors, motionStart, motionStop, white, color3, loopColor, 0f);
            Color[] colorArray2 = this.GetPivotColors(this.m_DstPivotVectors, num3, num4, fromColor, toColor, color6, this.m_DstDragOffset);
            Handles.DrawAAPolyLine(colors, this.m_SrcPivotVectors);
            Handles.DrawAAPolyLine(colorArray2, this.m_DstPivotVectors);
            GUI.EndGroup();
        }

        public bool DoTimeline(Rect timeRect)
        {
            bool flag = false;
            this.Init();
            this.m_Rect = timeRect;
            float a = this.m_TimeArea.PixelToTime(timeRect.xMin, timeRect);
            float num2 = this.m_TimeArea.PixelToTime(timeRect.xMax, timeRect);
            if (!Mathf.Approximately(a, this.StartTime))
            {
                this.StartTime = a;
                GUI.changed = true;
            }
            if (!Mathf.Approximately(num2, this.StopTime))
            {
                this.StopTime = num2;
                GUI.changed = true;
            }
            this.Time = Mathf.Max(this.Time, 0f);
            if (Event.current.type == EventType.Repaint)
            {
                this.m_TimeArea.rect = timeRect;
            }
            this.m_TimeArea.BeginViewGUI();
            this.m_TimeArea.EndViewGUI();
            GUI.BeginGroup(timeRect);
            Event current = Event.current;
            Rect rect = new Rect(0f, 0f, timeRect.width, timeRect.height);
            Rect position = new Rect(0f, 0f, timeRect.width, 18f);
            Rect rect3 = new Rect(0f, 18f, timeRect.width, 132f);
            float x = this.m_TimeArea.TimeToPixel(this.SrcStartTime, rect);
            float pixelX = this.m_TimeArea.TimeToPixel(this.SrcStopTime, rect);
            float num5 = this.m_TimeArea.TimeToPixel(this.DstStartTime, rect) + this.m_DstDragOffset;
            float num6 = this.m_TimeArea.TimeToPixel(this.DstStopTime, rect) + this.m_DstDragOffset;
            float num7 = this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect) + this.m_LeftThumbOffset;
            float num8 = this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect) + this.m_RightThumbOffset;
            float num9 = this.m_TimeArea.TimeToPixel(this.Time, rect);
            Rect rect4 = new Rect(x, 85f, pixelX - x, 32f);
            Rect rect5 = new Rect(num5, 117f, num6 - num5, 32f);
            Rect rect6 = new Rect(num7, 0f, num8 - num7, 18f);
            Rect rect7 = new Rect(num7, 18f, num8 - num7, rect.height - 18f);
            Rect rect8 = new Rect(num7 - 9f, 5f, 9f, 15f);
            Rect rect9 = new Rect(num8, 5f, 9f, 15f);
            Rect rect10 = new Rect(num9 - 7f, 4f, 15f, 15f);
            if (current.type == EventType.KeyDown)
            {
                if ((GUIUtility.keyboardControl == this.id) && (this.m_DragState == DragStates.Destination))
                {
                    this.m_DstDragOffset = 0f;
                }
                if (this.m_DragState == DragStates.LeftSelection)
                {
                    this.m_LeftThumbOffset = 0f;
                }
                if (this.m_DragState == DragStates.RightSelection)
                {
                    this.m_RightThumbOffset = 0f;
                }
                if (this.m_DragState == DragStates.FullSelection)
                {
                    this.m_LeftThumbOffset = 0f;
                    this.m_RightThumbOffset = 0f;
                }
            }
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                GUIUtility.hotControl = this.id;
                GUIUtility.keyboardControl = this.id;
                if (rect10.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.Playhead;
                }
                else if (rect4.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.Source;
                }
                else if (rect5.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.Destination;
                }
                else if (rect8.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.LeftSelection;
                }
                else if (rect9.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.RightSelection;
                }
                else if (rect6.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.FullSelection;
                }
                else if (position.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.TimeArea;
                }
                else if (rect3.Contains(current.mousePosition))
                {
                    this.m_DragState = DragStates.TimeArea;
                }
                else
                {
                    this.m_DragState = DragStates.None;
                }
                current.Use();
            }
            if ((current.type == EventType.MouseDrag) && (GUIUtility.hotControl == this.id))
            {
                switch (this.m_DragState)
                {
                    case DragStates.LeftSelection:
                        if (((current.delta.x > 0f) && (current.mousePosition.x > x)) || ((current.delta.x < 0f) && (current.mousePosition.x < num8)))
                        {
                            this.m_LeftThumbOffset += current.delta.x;
                        }
                        this.EnforceConstraints();
                        break;

                    case DragStates.RightSelection:
                        if (((current.delta.x > 0f) && (current.mousePosition.x > num7)) || (current.delta.x < 0f))
                        {
                            this.m_RightThumbOffset += current.delta.x;
                        }
                        this.EnforceConstraints();
                        break;

                    case DragStates.FullSelection:
                        this.m_RightThumbOffset += current.delta.x;
                        this.m_LeftThumbOffset += current.delta.x;
                        this.EnforceConstraints();
                        break;

                    case DragStates.Destination:
                        this.m_DstDragOffset += current.delta.x;
                        this.EnforceConstraints();
                        break;

                    case DragStates.Source:
                        this.m_TimeArea.m_Translation.x += current.delta.x;
                        break;

                    case DragStates.Playhead:
                        if (((current.delta.x > 0f) && (current.mousePosition.x > x)) || ((current.delta.x < 0f) && (current.mousePosition.x <= this.m_TimeArea.TimeToPixel(this.SampleStopTime, rect))))
                        {
                            this.Time = this.m_TimeArea.PixelToTime(num9 + current.delta.x, rect);
                        }
                        break;

                    case DragStates.TimeArea:
                        this.m_TimeArea.m_Translation.x += current.delta.x;
                        break;
                }
                current.Use();
                GUI.changed = true;
            }
            if ((current.type == EventType.MouseUp) && (GUIUtility.hotControl == this.id))
            {
                this.SrcStartTime = this.m_TimeArea.PixelToTime(x, rect);
                this.SrcStopTime = this.m_TimeArea.PixelToTime(pixelX, rect);
                this.DstStartTime = this.m_TimeArea.PixelToTime(num5, rect);
                this.DstStopTime = this.m_TimeArea.PixelToTime(num6, rect);
                this.TransitionStartTime = this.m_TimeArea.PixelToTime(num7, rect);
                this.TransitionStopTime = this.m_TimeArea.PixelToTime(num8, rect);
                GUI.changed = true;
                this.m_DragState = DragStates.None;
                flag = this.WasDraggingData();
                this.m_LeftThumbOffset = 0f;
                this.m_RightThumbOffset = 0f;
                this.m_DstDragOffset = 0f;
                GUIUtility.hotControl = 0;
                current.Use();
            }
            GUI.Box(position, GUIContent.none, this.styles.header);
            GUI.Box(rect3, GUIContent.none, this.styles.background);
            this.m_TimeArea.DrawMajorTicks(rect3, 30f);
            GUIContent content = EditorGUIUtility.TempContent(this.SrcName);
            int num10 = !this.srcLoop ? 1 : (1 + ((int) ((num8 - rect4.xMin) / (rect4.xMax - rect4.xMin))));
            Rect rect11 = rect4;
            if (rect4.width < 10f)
            {
                rect11 = new Rect(rect4.x, rect4.y, (rect4.xMax - rect4.xMin) * num10, rect4.height);
                num10 = 1;
            }
            for (int i = 0; i < num10; i++)
            {
                GUI.BeginGroup(rect11, GUIContent.none, this.styles.leftBlock);
                float width = num7 - rect11.xMin;
                float num13 = num8 - num7;
                float num14 = (rect11.xMax - rect11.xMin) - (width + num13);
                if (width > 0f)
                {
                    GUI.Box(new Rect(0f, 0f, width, rect4.height), GUIContent.none, this.styles.onLeft);
                }
                if (num13 > 0f)
                {
                    GUI.Box(new Rect(width, 0f, num13, rect4.height), GUIContent.none, this.styles.onOff);
                }
                if (num14 > 0f)
                {
                    GUI.Box(new Rect(width + num13, 0f, num14, rect4.height), GUIContent.none, this.styles.offRight);
                }
                float b = 1f;
                float num16 = this.styles.block.CalcSize(content).x;
                float num17 = Mathf.Max(0f, width) - 20f;
                float num18 = num17 + 15f;
                if (((num17 < num16) && (num18 > 0f)) && (this.m_DragState == DragStates.LeftSelection))
                {
                    b = 0f;
                }
                GUI.EndGroup();
                float num19 = this.styles.leftBlock.normal.textColor.a;
                if (!Mathf.Approximately(num19, b) && (Event.current.type == EventType.Repaint))
                {
                    num19 = Mathf.Lerp(num19, b, 0.1f);
                    this.styles.leftBlock.normal.textColor = new Color(this.styles.leftBlock.normal.textColor.r, this.styles.leftBlock.normal.textColor.g, this.styles.leftBlock.normal.textColor.b, num19);
                    HandleUtility.Repaint();
                }
                GUI.Box(rect11, content, this.styles.leftBlock);
                rect11 = new Rect(rect11.xMax, 85f, rect11.xMax - rect11.xMin, 32f);
            }
            GUIContent content2 = EditorGUIUtility.TempContent(this.DstName);
            int num20 = !this.dstLoop ? 1 : (1 + ((int) ((num8 - rect5.xMin) / (rect5.xMax - rect5.xMin))));
            rect11 = rect5;
            if (rect5.width < 10f)
            {
                rect11 = new Rect(rect5.x, rect5.y, (rect5.xMax - rect5.xMin) * num20, rect5.height);
                num20 = 1;
            }
            for (int j = 0; j < num20; j++)
            {
                GUI.BeginGroup(rect11, GUIContent.none, this.styles.rightBlock);
                float num22 = num7 - rect11.xMin;
                float num23 = num8 - num7;
                float num24 = (rect11.xMax - rect11.xMin) - (num22 + num23);
                if (num22 > 0f)
                {
                    GUI.Box(new Rect(0f, 0f, num22, rect5.height), GUIContent.none, this.styles.offLeft);
                }
                if (num23 > 0f)
                {
                    GUI.Box(new Rect(num22, 0f, num23, rect5.height), GUIContent.none, this.styles.offOn);
                }
                if (num24 > 0f)
                {
                    GUI.Box(new Rect(num22 + num23, 0f, num24, rect5.height), GUIContent.none, this.styles.onRight);
                }
                float num25 = 1f;
                float num26 = this.styles.block.CalcSize(content2).x;
                float num27 = Mathf.Max(0f, num22) - 20f;
                float num28 = num27 + 15f;
                if (((num27 < num26) && (num28 > 0f)) && ((this.m_DragState == DragStates.LeftSelection) || (this.m_DragState == DragStates.Destination)))
                {
                    num25 = 0f;
                }
                GUI.EndGroup();
                float num29 = this.styles.rightBlock.normal.textColor.a;
                if (!Mathf.Approximately(num29, num25) && (Event.current.type == EventType.Repaint))
                {
                    num29 = Mathf.Lerp(num29, num25, 0.1f);
                    this.styles.rightBlock.normal.textColor = new Color(this.styles.rightBlock.normal.textColor.r, this.styles.rightBlock.normal.textColor.g, this.styles.rightBlock.normal.textColor.b, num29);
                    HandleUtility.Repaint();
                }
                GUI.Box(rect11, content2, this.styles.rightBlock);
                rect11 = new Rect(rect11.xMax, rect11.yMin, rect11.xMax - rect11.xMin, 32f);
            }
            GUI.Box(rect7, GUIContent.none, this.styles.select);
            GUI.Box(rect6, GUIContent.none, this.styles.selectHead);
            this.m_TimeArea.TimeRuler(position, 30f);
            GUI.Box(rect8, GUIContent.none, !this.m_HasExitTime ? this.styles.handLeftPrev : this.styles.handLeft);
            GUI.Box(rect9, GUIContent.none, this.styles.handRight);
            GUI.Box(rect10, GUIContent.none, this.styles.playhead);
            Color color = Handles.color;
            Handles.color = Color.white;
            Handles.DrawLine(new Vector3(num9, 19f, 0f), new Vector3(num9, rect.height, 0f));
            Handles.color = color;
            bool flag2 = (this.SrcStopTime - this.SrcStartTime) < 0.03333334f;
            bool flag3 = (this.DstStopTime - this.DstStartTime) < 0.03333334f;
            if ((this.m_DragState == DragStates.Destination) && !flag3)
            {
                Rect rect12 = new Rect(num7 - 50f, rect5.y, 45f, rect5.height);
                string t = string.Format("{0:0%}", (num7 - num5) / (num6 - num5));
                GUI.Box(rect12, EditorGUIUtility.TempContent(t), this.styles.timeBlockRight);
            }
            if (this.m_DragState == DragStates.LeftSelection)
            {
                if (!flag2)
                {
                    Rect rect13 = new Rect(num7 - 50f, rect4.y, 45f, rect4.height);
                    string str2 = string.Format("{0:0%}", (num7 - x) / (pixelX - x));
                    GUI.Box(rect13, EditorGUIUtility.TempContent(str2), this.styles.timeBlockRight);
                }
                if (!flag3)
                {
                    Rect rect14 = new Rect(num7 - 50f, rect5.y, 45f, rect5.height);
                    string str3 = string.Format("{0:0%}", (num7 - num5) / (num6 - num5));
                    GUI.Box(rect14, EditorGUIUtility.TempContent(str3), this.styles.timeBlockRight);
                }
            }
            if (this.m_DragState == DragStates.RightSelection)
            {
                if (!flag2)
                {
                    Rect rect15 = new Rect(num8 + 5f, rect4.y, 45f, rect4.height);
                    string str4 = string.Format("{0:0%}", (num8 - x) / (pixelX - x));
                    GUI.Box(rect15, EditorGUIUtility.TempContent(str4), this.styles.timeBlockLeft);
                }
                if (!flag3)
                {
                    Rect rect16 = new Rect(num8 + 5f, rect5.y, 45f, rect5.height);
                    string str5 = string.Format("{0:0%}", (num8 - num5) / (num6 - num5));
                    GUI.Box(rect16, EditorGUIUtility.TempContent(str5), this.styles.timeBlockLeft);
                }
            }
            this.DoPivotCurves();
            GUI.EndGroup();
            return flag;
        }

        private void EnforceConstraints()
        {
            Rect rect = new Rect(0f, 0f, this.m_Rect.width, 150f);
            if (this.m_DragState == DragStates.LeftSelection)
            {
                float min = this.m_TimeArea.TimeToPixel(this.SrcStartTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect);
                float max = this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect);
                this.m_LeftThumbOffset = Mathf.Clamp(this.m_LeftThumbOffset, min, max);
            }
            if (this.m_DragState == DragStates.RightSelection)
            {
                float num3 = this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect);
                if (this.m_RightThumbOffset < num3)
                {
                    this.m_RightThumbOffset = num3;
                }
            }
        }

        private List<Vector3> GetControls(List<Vector3> segmentPoints, float scale)
        {
            List<Vector3> list = new List<Vector3>();
            if (segmentPoints.Count >= 2)
            {
                for (int i = 0; i < segmentPoints.Count; i++)
                {
                    if (i == 0)
                    {
                        Vector3 item = segmentPoints[i];
                        Vector3 vector2 = segmentPoints[i + 1];
                        Vector3 vector3 = vector2 - item;
                        Vector3 vector4 = item + ((Vector3) (scale * vector3));
                        list.Add(item);
                        list.Add(vector4);
                    }
                    else if (i == (segmentPoints.Count - 1))
                    {
                        Vector3 vector5 = segmentPoints[i - 1];
                        Vector3 vector6 = segmentPoints[i];
                        Vector3 vector7 = vector6 - vector5;
                        Vector3 vector8 = vector6 - ((Vector3) (scale * vector7));
                        list.Add(vector8);
                        list.Add(vector6);
                    }
                    else
                    {
                        Vector3 vector9 = segmentPoints[i - 1];
                        Vector3 vector10 = segmentPoints[i];
                        Vector3 vector11 = segmentPoints[i + 1];
                        Vector3 vector15 = vector11 - vector9;
                        Vector3 normalized = vector15.normalized;
                        Vector3 vector16 = vector10 - vector9;
                        Vector3 vector13 = vector10 - ((Vector3) ((scale * normalized) * vector16.magnitude));
                        Vector3 vector17 = vector11 - vector10;
                        Vector3 vector14 = vector10 + ((Vector3) ((scale * normalized) * vector17.magnitude));
                        list.Add(vector13);
                        list.Add(vector10);
                        list.Add(vector14);
                    }
                }
            }
            return list;
        }

        private Color[] GetPivotColors(Vector3[] vectors, float motionStart, float motionStop, Color fromColor, Color toColor, Color loopColor, float offset)
        {
            Color[] colorArray = new Color[vectors.Length];
            float num = this.m_TimeArea.TimeToPixel(this.m_TransitionStartTime, this.m_Rect) + this.m_LeftThumbOffset;
            float num2 = this.m_TimeArea.TimeToPixel(this.m_TransitionStopTime, this.m_Rect) + this.m_RightThumbOffset;
            float num3 = num2 - num;
            for (int i = 0; i < colorArray.Length; i++)
            {
                if ((vectors[i].x >= num) && (vectors[i].x <= num2))
                {
                    colorArray[i] = Color.Lerp(fromColor, toColor, (vectors[i].x - num) / num3);
                }
                else if ((vectors[i].x < num) && (vectors[i].x >= (motionStart + offset)))
                {
                    colorArray[i] = fromColor;
                }
                else if ((vectors[i].x > num2) && (vectors[i].x <= (motionStop + offset)))
                {
                    colorArray[i] = toColor;
                }
                else
                {
                    colorArray[i] = loopColor;
                }
            }
            return colorArray;
        }

        private Vector3[] GetPivotVectors(PivotSample[] samples, float width, Rect rect, float height, bool loop)
        {
            if ((samples.Length == 0) || (width < 0.33f))
            {
                return new Vector3[0];
            }
            List<Vector3> segmentPoints = new List<Vector3>();
            for (int i = 0; i < samples.Length; i++)
            {
                PivotSample sample = samples[i];
                Vector3 zero = Vector3.zero;
                zero.x = this.m_TimeArea.TimeToPixel(sample.m_Time, rect);
                zero.y = (height / 16f) + (((sample.m_Weight * 12f) * height) / 16f);
                segmentPoints.Add(zero);
            }
            if (loop)
            {
                Vector3 vector7 = segmentPoints[segmentPoints.Count - 1];
                if (vector7.x <= rect.width)
                {
                    Vector3 vector8 = segmentPoints[segmentPoints.Count - 1];
                    float x = vector8.x;
                    int num3 = 0;
                    int num4 = 1;
                    List<Vector3> collection = new List<Vector3>();
                    while (x < rect.width)
                    {
                        if (num3 > (segmentPoints.Count - 1))
                        {
                            num3 = 0;
                            num4++;
                        }
                        Vector3 item = segmentPoints[num3];
                        item.x += num4 * width;
                        x = item.x;
                        collection.Add(item);
                        num3++;
                    }
                    segmentPoints.AddRange(collection);
                }
            }
            List<Vector3> controls = this.GetControls(segmentPoints, 0.5f);
            segmentPoints.Clear();
            for (int j = 0; j < (controls.Count - 3); j += 3)
            {
                Vector3 vector3 = controls[j];
                Vector3 vector4 = controls[j + 1];
                Vector3 vector5 = controls[j + 2];
                Vector3 vector6 = controls[j + 3];
                if (j == 0)
                {
                    segmentPoints.Add(this.CalculatePoint(0f, vector3, vector4, vector5, vector6));
                }
                for (int k = 1; k <= 10; k++)
                {
                    segmentPoints.Add(this.CalculatePoint(((float) k) / 10f, vector3, vector4, vector5, vector6));
                }
            }
            return segmentPoints.ToArray();
        }

        private void Init()
        {
            if (this.id == -1)
            {
                this.id = GUIUtility.GetPermanentControlID();
            }
            if (this.m_TimeArea == null)
            {
                this.m_TimeArea = new TimeArea(false);
                this.m_TimeArea.hRangeLocked = false;
                this.m_TimeArea.vRangeLocked = true;
                this.m_TimeArea.hSlider = false;
                this.m_TimeArea.vSlider = false;
                this.m_TimeArea.margin = 10f;
                this.m_TimeArea.scaleWithWindow = true;
                this.m_TimeArea.hTicks.SetTickModulosForFrameRate(30f);
            }
            if (this.styles == null)
            {
                this.styles = new Styles();
            }
        }

        private Vector3[] OffsetPivotVectors(Vector3[] vectors, float offset)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i].x += offset;
            }
            return vectors;
        }

        public void ResetRange()
        {
            this.m_TimeArea.SetShownHRangeInsideMargins(0f, this.StopTime);
        }

        private bool WasDraggingData()
        {
            return (((this.m_DstDragOffset != 0f) || (this.m_LeftThumbOffset != 0f)) || (this.m_RightThumbOffset != 0f));
        }

        public float DstDuration
        {
            get
            {
                return (this.DstStopTime - this.DstStartTime);
            }
        }

        public bool dstLoop
        {
            get
            {
                return this.m_DstLoop;
            }
            set
            {
                this.m_DstLoop = value;
            }
        }

        public string DstName
        {
            get
            {
                return this.m_DstName;
            }
            set
            {
                this.m_DstName = value;
            }
        }

        public List<PivotSample> DstPivotList
        {
            get
            {
                return this.m_DstPivotList;
            }
            set
            {
                this.m_DstPivotList = value;
                this.m_DstPivotVectors = null;
            }
        }

        public float DstStartTime
        {
            get
            {
                return this.m_DstStartTime;
            }
            set
            {
                this.m_DstStartTime = value;
            }
        }

        public float DstStopTime
        {
            get
            {
                return this.m_DstStopTime;
            }
            set
            {
                this.m_DstStopTime = value;
            }
        }

        public bool HasExitTime
        {
            get
            {
                return this.m_HasExitTime;
            }
            set
            {
                this.m_HasExitTime = value;
            }
        }

        public float SampleStopTime
        {
            get
            {
                return this.m_SampleStopTime;
            }
            set
            {
                this.m_SampleStopTime = value;
            }
        }

        public float SrcDuration
        {
            get
            {
                return (this.SrcStopTime - this.SrcStartTime);
            }
        }

        public bool srcLoop
        {
            get
            {
                return this.m_SrcLoop;
            }
            set
            {
                this.m_SrcLoop = value;
            }
        }

        public string SrcName
        {
            get
            {
                return this.m_SrcName;
            }
            set
            {
                this.m_SrcName = value;
            }
        }

        public List<PivotSample> SrcPivotList
        {
            get
            {
                return this.m_SrcPivotList;
            }
            set
            {
                this.m_SrcPivotList = value;
                this.m_SrcPivotVectors = null;
            }
        }

        public float SrcStartTime
        {
            get
            {
                return this.m_SrcStartTime;
            }
            set
            {
                this.m_SrcStartTime = value;
            }
        }

        public float SrcStopTime
        {
            get
            {
                return this.m_SrcStopTime;
            }
            set
            {
                this.m_SrcStopTime = value;
            }
        }

        public float StartTime
        {
            get
            {
                return this.m_StartTime;
            }
            set
            {
                this.m_StartTime = value;
            }
        }

        public float StopTime
        {
            get
            {
                return this.m_StopTime;
            }
            set
            {
                this.m_StopTime = value;
            }
        }

        public float Time
        {
            get
            {
                return this.m_Time;
            }
            set
            {
                this.m_Time = value;
            }
        }

        public float TransitionDuration
        {
            get
            {
                return (this.TransitionStopTime - this.TransitionStartTime);
            }
        }

        public float TransitionStartTime
        {
            get
            {
                return this.m_TransitionStartTime;
            }
            set
            {
                this.m_TransitionStartTime = value;
            }
        }

        public float TransitionStopTime
        {
            get
            {
                return this.m_TransitionStopTime;
            }
            set
            {
                this.m_TransitionStopTime = value;
            }
        }

        private enum DragStates
        {
            None,
            LeftSelection,
            RightSelection,
            FullSelection,
            Destination,
            Source,
            Playhead,
            TimeArea
        }

        internal class PivotSample
        {
            public float m_Time;
            public float m_Weight;
        }

        private class Styles
        {
            public readonly GUIStyle background = new GUIStyle("MeTransitionBack");
            public readonly GUIStyle block = new GUIStyle("MeTransitionBlock");
            public readonly GUIStyle handLeft = new GUIStyle("MeTransitionHandleLeft");
            public readonly GUIStyle handLeftPrev = new GUIStyle("MeTransitionHandleLeftPrev");
            public readonly GUIStyle handRight = new GUIStyle("MeTransitionHandleRight");
            public readonly GUIStyle header = new GUIStyle("MeTransitionHead");
            public GUIStyle leftBlock = new GUIStyle("MeTransitionBlock");
            public readonly GUIStyle offLeft = new GUIStyle("MeTransOffLeft");
            public readonly GUIStyle offOn = new GUIStyle("MeTransOff2On");
            public readonly GUIStyle offRight = new GUIStyle("MeTransOffRight");
            public readonly GUIStyle onLeft = new GUIStyle("MeTransOnLeft");
            public readonly GUIStyle onOff = new GUIStyle("MeTransOn2Off");
            public readonly GUIStyle onRight = new GUIStyle("MeTransOnRight");
            public readonly GUIStyle overlay = new GUIStyle("MeTransBGOver");
            public readonly GUIStyle playhead = new GUIStyle("MeTransPlayhead");
            public GUIStyle rightBlock = new GUIStyle("MeTransitionBlock");
            public readonly GUIStyle select = new GUIStyle("MeTransitionSelect");
            public readonly GUIStyle selectHead = new GUIStyle("MeTransitionSelectHead");
            public GUIStyle timeBlockLeft = new GUIStyle("MeTimeLabel");
            public GUIStyle timeBlockRight = new GUIStyle("MeTimeLabel");

            public Styles()
            {
                this.timeBlockRight.alignment = TextAnchor.MiddleRight;
                this.timeBlockRight.normal.background = null;
                this.timeBlockLeft.normal.background = null;
            }
        }
    }
}

