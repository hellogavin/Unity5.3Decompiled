namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class NormalCurveRenderer : CurveRenderer
    {
        private const int kMaximumSampleCount = 50;
        private const float kSegmentWindowResolution = 1000f;
        private AnimationCurve m_Curve;
        private float m_CustomRangeEnd;
        private float m_CustomRangeStart;
        private WrapMode postWrapMode = WrapMode.Once;
        private WrapMode preWrapMode = WrapMode.Once;

        public NormalCurveRenderer(AnimationCurve curve)
        {
            this.m_Curve = curve;
            if (this.m_Curve == null)
            {
                this.m_Curve = new AnimationCurve();
            }
        }

        private void AddPoints(ref List<Vector3> points, float minTime, float maxTime, float visibleMinTime, float visibleMaxTime)
        {
            Keyframe keyframe3 = this.m_Curve[0];
            if (keyframe3.time >= minTime)
            {
                Keyframe keyframe4 = this.m_Curve[0];
                points.Add(new Vector3(this.rangeStart, keyframe4.value));
                Keyframe keyframe5 = this.m_Curve[0];
                Keyframe keyframe6 = this.m_Curve[0];
                points.Add(new Vector3(keyframe5.time, keyframe6.value));
            }
            for (int i = 0; i < (this.m_Curve.length - 1); i++)
            {
                Keyframe keyframe = this.m_Curve[i];
                Keyframe keyframe2 = this.m_Curve[i + 1];
                if ((keyframe2.time >= minTime) && (keyframe.time <= maxTime))
                {
                    points.Add(new Vector3(keyframe.time, keyframe.value));
                    int num2 = GetSegmentResolution(visibleMinTime, visibleMaxTime, keyframe.time, keyframe2.time);
                    float x = Mathf.Lerp(keyframe.time, keyframe2.time, 0.001f / ((float) num2));
                    points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    for (float j = 1f; j < num2; j++)
                    {
                        x = Mathf.Lerp(keyframe.time, keyframe2.time, j / ((float) num2));
                        points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    }
                    x = Mathf.Lerp(keyframe.time, keyframe2.time, 1f - (0.001f / ((float) num2)));
                    points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    x = keyframe2.time;
                    points.Add(new Vector3(x, keyframe2.value));
                }
            }
            Keyframe keyframe7 = this.m_Curve[this.m_Curve.length - 1];
            if (keyframe7.time <= maxTime)
            {
                Keyframe keyframe8 = this.m_Curve[this.m_Curve.length - 1];
                Keyframe keyframe9 = this.m_Curve[this.m_Curve.length - 1];
                points.Add(new Vector3(keyframe8.time, keyframe9.value));
                Keyframe keyframe10 = this.m_Curve[this.m_Curve.length - 1];
                points.Add(new Vector3(this.rangeEnd, keyframe10.value));
            }
        }

        public static float[,] CalculateRanges(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrapMode, WrapMode postWrapMode)
        {
            WrapMode mode = preWrapMode;
            if (postWrapMode != mode)
            {
                return new float[,] { { rangeStart, rangeEnd } };
            }
            if (mode == WrapMode.Loop)
            {
                if ((maxTime - minTime) > (rangeEnd - rangeStart))
                {
                    return new float[,] { { rangeStart, rangeEnd } };
                }
                minTime = Mathf.Repeat(minTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
                maxTime = Mathf.Repeat(maxTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
                if (minTime < maxTime)
                {
                    return new float[,] { { minTime, maxTime } };
                }
                return new float[,] { { rangeStart, maxTime }, { minTime, rangeEnd } };
            }
            if (mode == WrapMode.PingPong)
            {
                return new float[,] { { rangeStart, rangeEnd } };
            }
            return new float[,] { { minTime, maxTime } };
        }

        public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor)
        {
            Vector3[] points = this.GetPoints(minTime, maxTime);
            DrawCurveWrapped(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode, color, transform, points, wrapColor);
        }

        public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrap, WrapMode postWrap, Color color, Matrix4x4 transform, Vector3[] points, Color wrapColor)
        {
            if (points.Length != 0)
            {
                int num;
                int num2;
                if ((rangeEnd - rangeStart) != 0f)
                {
                    num = Mathf.FloorToInt((minTime - rangeStart) / (rangeEnd - rangeStart));
                    num2 = Mathf.CeilToInt((maxTime - rangeEnd) / (rangeEnd - rangeStart));
                }
                else
                {
                    preWrap = WrapMode.Once;
                    postWrap = WrapMode.Once;
                    num = (minTime >= rangeStart) ? 0 : -1;
                    num2 = (maxTime <= rangeEnd) ? 0 : 1;
                }
                int index = points.Length - 1;
                Handles.color = color;
                List<Vector3> list = new List<Vector3>();
                if ((num <= 0) && (num2 >= 0))
                {
                    DrawPolyLine(transform, 2f, points);
                }
                else
                {
                    Handles.DrawPolyLine(points);
                }
                Handles.color = new Color(wrapColor.r, wrapColor.g, wrapColor.b, wrapColor.a * color.a);
                if (preWrap == WrapMode.Loop)
                {
                    list = new List<Vector3>();
                    for (int i = num; i < 0; i++)
                    {
                        for (int j = 0; j < points.Length; j++)
                        {
                            Vector3 v = points[j];
                            v.x += i * (rangeEnd - rangeStart);
                            v = transform.MultiplyPoint(v);
                            list.Add(v);
                        }
                    }
                    list.Add(transform.MultiplyPoint(points[0]));
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (preWrap == WrapMode.PingPong)
                {
                    list = new List<Vector3>();
                    for (int k = num; k < 0; k++)
                    {
                        for (int m = 0; m < points.Length; m++)
                        {
                            if ((k / 2) == (((float) k) / 2f))
                            {
                                Vector3 vector2 = points[m];
                                vector2.x += k * (rangeEnd - rangeStart);
                                vector2 = transform.MultiplyPoint(vector2);
                                list.Add(vector2);
                            }
                            else
                            {
                                Vector3 vector3 = points[index - m];
                                vector3.x = (-vector3.x + ((k + 1) * (rangeEnd - rangeStart))) + (rangeStart * 2f);
                                vector3 = transform.MultiplyPoint(vector3);
                                list.Add(vector3);
                            }
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (num < 0)
                {
                    Vector3[] vectorArray1 = new Vector3[] { transform.MultiplyPoint(new Vector3(minTime, points[0].y, 0f)), transform.MultiplyPoint(new Vector3(Mathf.Min(maxTime, points[0].x), points[0].y, 0f)) };
                    Handles.DrawPolyLine(vectorArray1);
                }
                if (postWrap == WrapMode.Loop)
                {
                    list = new List<Vector3> {
                        transform.MultiplyPoint(points[index])
                    };
                    for (int n = 1; n <= num2; n++)
                    {
                        for (int num9 = 0; num9 < points.Length; num9++)
                        {
                            Vector3 vector4 = points[num9];
                            vector4.x += n * (rangeEnd - rangeStart);
                            vector4 = transform.MultiplyPoint(vector4);
                            list.Add(vector4);
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (postWrap == WrapMode.PingPong)
                {
                    list = new List<Vector3>();
                    for (int num10 = 1; num10 <= num2; num10++)
                    {
                        for (int num11 = 0; num11 < points.Length; num11++)
                        {
                            if ((num10 / 2) == (((float) num10) / 2f))
                            {
                                Vector3 vector5 = points[num11];
                                vector5.x += num10 * (rangeEnd - rangeStart);
                                vector5 = transform.MultiplyPoint(vector5);
                                list.Add(vector5);
                            }
                            else
                            {
                                Vector3 vector6 = points[index - num11];
                                vector6.x = (-vector6.x + ((num10 + 1) * (rangeEnd - rangeStart))) + (rangeStart * 2f);
                                vector6 = transform.MultiplyPoint(vector6);
                                list.Add(vector6);
                            }
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (num2 > 0)
                {
                    Vector3[] vectorArray2 = new Vector3[] { transform.MultiplyPoint(new Vector3(Mathf.Max(minTime, points[index].x), points[index].y, 0f)), transform.MultiplyPoint(new Vector3(maxTime, points[index].y, 0f)) };
                    Handles.DrawPolyLine(vectorArray2);
                }
            }
        }

        public static void DrawPolyLine(Matrix4x4 transform, float minDistance, params Vector3[] points)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color c = Handles.color * new Color(1f, 1f, 1f, 0.75f);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(Handles.matrix);
                GL.Begin(1);
                GL.Color(c);
                Vector3 v = transform.MultiplyPoint(points[0]);
                for (int i = 1; i < points.Length; i++)
                {
                    Vector3 vector2 = transform.MultiplyPoint(points[i]);
                    Vector3 vector3 = v - vector2;
                    if (vector3.magnitude > minDistance)
                    {
                        GL.Vertex(v);
                        GL.Vertex(vector2);
                        v = vector2;
                    }
                }
                GL.End();
                GL.PopMatrix();
            }
        }

        public float EvaluateCurveDeltaSlow(float time)
        {
            float num = 0.0001f;
            return ((this.m_Curve.Evaluate(time + num) - this.m_Curve.Evaluate(time - num)) / (num * 2f));
        }

        public float EvaluateCurveSlow(float time)
        {
            return this.m_Curve.Evaluate(time);
        }

        public Bounds GetBounds()
        {
            return this.GetBounds(this.rangeStart, this.rangeEnd);
        }

        public Bounds GetBounds(float minTime, float maxTime)
        {
            Vector3[] points = this.GetPoints(minTime, maxTime);
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            foreach (Vector3 vector in points)
            {
                if (vector.y > negativeInfinity)
                {
                    negativeInfinity = vector.y;
                }
                if (vector.y < positiveInfinity)
                {
                    positiveInfinity = vector.y;
                }
            }
            if (positiveInfinity == float.PositiveInfinity)
            {
                positiveInfinity = 0f;
                negativeInfinity = 0f;
            }
            return new Bounds(new Vector3((maxTime + minTime) * 0.5f, (negativeInfinity + positiveInfinity) * 0.5f, 0f), new Vector3(maxTime - minTime, negativeInfinity - positiveInfinity, 0f));
        }

        public AnimationCurve GetCurve()
        {
            return this.m_Curve;
        }

        private Vector3[] GetPoints(float minTime, float maxTime)
        {
            List<Vector3> points = new List<Vector3>();
            if (this.m_Curve.length != 0)
            {
                points.Capacity = 0x3e8 + this.m_Curve.length;
                float[,] numArray = CalculateRanges(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode);
                for (int i = 0; i < numArray.GetLength(0); i++)
                {
                    this.AddPoints(ref points, numArray[i, 0], numArray[i, 1], minTime, maxTime);
                }
                if (points.Count > 0)
                {
                    for (int j = 1; j < points.Count; j++)
                    {
                        Vector3 vector = points[j];
                        Vector3 vector2 = points[j - 1];
                        if (vector.x < vector2.x)
                        {
                            points.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            return points.ToArray();
        }

        private static int GetSegmentResolution(float minTime, float maxTime, float keyTime, float nextKeyTime)
        {
            float num = maxTime - minTime;
            float num2 = nextKeyTime - keyTime;
            return Mathf.Clamp(Mathf.RoundToInt(1000f * (num2 / num)), 1, 50);
        }

        public float RangeEnd()
        {
            return this.rangeEnd;
        }

        public float RangeStart()
        {
            return this.rangeStart;
        }

        public void SetCustomRange(float start, float end)
        {
            this.m_CustomRangeStart = start;
            this.m_CustomRangeEnd = end;
        }

        public void SetWrap(WrapMode wrap)
        {
            this.preWrapMode = wrap;
            this.postWrapMode = wrap;
        }

        public void SetWrap(WrapMode preWrap, WrapMode postWrap)
        {
            this.preWrapMode = preWrap;
            this.postWrapMode = postWrap;
        }

        private float rangeEnd
        {
            get
            {
                return ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.m_Curve.length <= 0)) ? this.m_CustomRangeEnd : this.m_Curve.keys[this.m_Curve.length - 1].time);
            }
        }

        private float rangeStart
        {
            get
            {
                return ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.m_Curve.length <= 0)) ? this.m_CustomRangeStart : this.m_Curve.keys[0].time);
            }
        }
    }
}

