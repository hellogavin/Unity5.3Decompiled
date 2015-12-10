namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class EulerCurveCombinedRenderer
    {
        private float cachedEvaluationTime = float.PositiveInfinity;
        private Vector3 cachedEvaluationValue;
        private float cachedRangeEnd = float.NegativeInfinity;
        private float cachedRangeStart = float.PositiveInfinity;
        private const float epsilon = 0.001f;
        private AnimationCurve eulerX;
        private AnimationCurve eulerY;
        private AnimationCurve eulerZ;
        private const int kSegmentResolution = 40;
        private float m_CustomRangeEnd;
        private float m_CustomRangeStart;
        private SortedDictionary<float, Vector3> points;
        private WrapMode postWrapMode = WrapMode.Once;
        private WrapMode preWrapMode = WrapMode.Once;
        private AnimationCurve quaternionW;
        private AnimationCurve quaternionX;
        private AnimationCurve quaternionY;
        private AnimationCurve quaternionZ;
        private Vector3 refEuler;

        public EulerCurveCombinedRenderer(AnimationCurve quaternionX, AnimationCurve quaternionY, AnimationCurve quaternionZ, AnimationCurve quaternionW, AnimationCurve eulerX, AnimationCurve eulerY, AnimationCurve eulerZ)
        {
            this.quaternionX = (quaternionX != null) ? quaternionX : new AnimationCurve();
            this.quaternionY = (quaternionY != null) ? quaternionY : new AnimationCurve();
            this.quaternionZ = (quaternionZ != null) ? quaternionZ : new AnimationCurve();
            this.quaternionW = (quaternionW != null) ? quaternionW : new AnimationCurve();
            this.eulerX = (eulerX != null) ? eulerX : new AnimationCurve();
            this.eulerY = (eulerY != null) ? eulerY : new AnimationCurve();
            this.eulerZ = (eulerZ != null) ? eulerZ : new AnimationCurve();
        }

        private void AddPoints(float minTime, float maxTime)
        {
            AnimationCurve quaternionX = this.quaternionX;
            if (quaternionX.length == 0)
            {
                quaternionX = this.eulerX;
            }
            if (quaternionX.length != 0)
            {
                Keyframe keyframe = quaternionX[0];
                if (keyframe.time >= minTime)
                {
                    Keyframe keyframe2 = quaternionX[0];
                    Vector3 values = this.GetValues(keyframe2.time, true);
                    this.points[this.rangeStart] = values;
                    Keyframe keyframe3 = quaternionX[0];
                    this.points[keyframe3.time] = values;
                }
                Keyframe keyframe4 = quaternionX[quaternionX.length - 1];
                if (keyframe4.time <= maxTime)
                {
                    Keyframe keyframe5 = quaternionX[quaternionX.length - 1];
                    Vector3 vector2 = this.GetValues(keyframe5.time, true);
                    Keyframe keyframe6 = quaternionX[quaternionX.length - 1];
                    this.points[keyframe6.time] = vector2;
                    this.points[this.rangeEnd] = vector2;
                }
                for (int i = 0; i < (quaternionX.length - 1); i++)
                {
                    Keyframe keyframe7 = quaternionX[i + 1];
                    if (keyframe7.time >= minTime)
                    {
                        Keyframe keyframe8 = quaternionX[i];
                        if (keyframe8.time <= maxTime)
                        {
                            Keyframe keyframe9 = quaternionX[i];
                            float time = keyframe9.time;
                            this.points[time] = this.GetValues(time, true);
                            for (float j = 1f; j <= 20f; j++)
                            {
                                Keyframe keyframe10 = quaternionX[i];
                                Keyframe keyframe11 = quaternionX[i + 1];
                                time = Mathf.Lerp(keyframe10.time, keyframe11.time, (j - 0.001f) / 40f);
                                this.points[time] = this.GetValues(time, false);
                            }
                            Keyframe keyframe12 = quaternionX[i + 1];
                            time = keyframe12.time;
                            this.points[time] = this.GetValues(time, true);
                            for (float k = 1f; k <= 20f; k++)
                            {
                                Keyframe keyframe13 = quaternionX[i];
                                Keyframe keyframe14 = quaternionX[i + 1];
                                time = Mathf.Lerp(keyframe13.time, keyframe14.time, 1f - ((k - 0.001f) / 40f));
                                this.points[time] = this.GetValues(time, false);
                            }
                        }
                    }
                }
            }
        }

        private void CalculateCurves(float minTime, float maxTime)
        {
            this.points = new SortedDictionary<float, Vector3>();
            float[,] numArray = NormalCurveRenderer.CalculateRanges(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode);
            for (int i = 0; i < numArray.GetLength(0); i++)
            {
                this.AddPoints(numArray[i, 0], numArray[i, 1]);
            }
        }

        public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, int component, Color wrapColor)
        {
            if ((minTime < this.cachedRangeStart) || (maxTime > this.cachedRangeEnd))
            {
                this.CalculateCurves(minTime, maxTime);
                if ((minTime <= this.rangeStart) && (maxTime >= this.rangeEnd))
                {
                    this.cachedRangeStart = float.NegativeInfinity;
                    this.cachedRangeEnd = float.PositiveInfinity;
                }
                else
                {
                    this.cachedRangeStart = minTime;
                    this.cachedRangeEnd = maxTime;
                }
            }
            List<Vector3> list = new List<Vector3>();
            foreach (KeyValuePair<float, Vector3> pair in this.points)
            {
                list.Add(new Vector3(pair.Key, pair.Value[component]));
            }
            NormalCurveRenderer.DrawCurveWrapped(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode, color, transform, list.ToArray(), wrapColor);
        }

        public float EvaluateCurveDeltaSlow(float time, int component)
        {
            if (this.quaternionX == null)
            {
                return 0f;
            }
            return ((this.EvaluateCurveSlow(time + 0.001f, component) - this.EvaluateCurveSlow(time - 0.001f, component)) / 0.002f);
        }

        public float EvaluateCurveSlow(float time, int component)
        {
            if (this.GetCurveOfComponent(component).length == 1)
            {
                Keyframe keyframe = this.GetCurveOfComponent(component)[0];
                return keyframe.value;
            }
            if (time == this.cachedEvaluationTime)
            {
                return this.cachedEvaluationValue[component];
            }
            if ((time < this.cachedRangeStart) || (time > this.cachedRangeEnd))
            {
                this.CalculateCurves(this.rangeStart, this.rangeEnd);
                this.cachedRangeStart = float.NegativeInfinity;
                this.cachedRangeEnd = float.PositiveInfinity;
            }
            float[] numArray = new float[this.points.Count];
            Vector3[] vectorArray = new Vector3[this.points.Count];
            int index = 0;
            foreach (KeyValuePair<float, Vector3> pair in this.points)
            {
                numArray[index] = pair.Key;
                vectorArray[index] = pair.Value;
                index++;
            }
            for (int i = 0; i < (numArray.Length - 1); i++)
            {
                if (time < numArray[i + 1])
                {
                    float t = Mathf.InverseLerp(numArray[i], numArray[i + 1], time);
                    this.cachedEvaluationValue = Vector3.Lerp(vectorArray[i], vectorArray[i + 1], t);
                    this.cachedEvaluationTime = time;
                    return this.cachedEvaluationValue[component];
                }
            }
            if (vectorArray.Length > 0)
            {
                return vectorArray[vectorArray.Length - 1][component];
            }
            Debug.LogError("List of euler curve points is empty, probably caused by lack of euler curve key synching");
            return 0f;
        }

        private Vector3 EvaluateEulerCurvesDirectly(float time)
        {
            return new Vector3(this.eulerX.Evaluate(time), this.eulerY.Evaluate(time), this.eulerZ.Evaluate(time));
        }

        private Quaternion EvaluateQuaternionCurvesDirectly(float time)
        {
            return new Quaternion(this.quaternionX.Evaluate(time), this.quaternionY.Evaluate(time), this.quaternionZ.Evaluate(time), this.quaternionW.Evaluate(time));
        }

        public Bounds GetBounds(float minTime, float maxTime, int component)
        {
            this.CalculateCurves(minTime, maxTime);
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            foreach (KeyValuePair<float, Vector3> pair in this.points)
            {
                if (pair.Value[component] > negativeInfinity)
                {
                    negativeInfinity = pair.Value[component];
                }
                if (pair.Value[component] < positiveInfinity)
                {
                    positiveInfinity = pair.Value[component];
                }
            }
            if (positiveInfinity == float.PositiveInfinity)
            {
                positiveInfinity = 0f;
                negativeInfinity = 0f;
            }
            return new Bounds(new Vector3((maxTime + minTime) * 0.5f, (negativeInfinity + positiveInfinity) * 0.5f, 0f), new Vector3(maxTime - minTime, negativeInfinity - positiveInfinity, 0f));
        }

        public AnimationCurve GetCurveOfComponent(int component)
        {
            switch (component)
            {
                case 0:
                    return this.eulerX;

                case 1:
                    return this.eulerY;

                case 2:
                    return this.eulerZ;
            }
            return null;
        }

        private Vector3 GetValues(float time, bool keyReference)
        {
            if (this.quaternionX == null)
            {
                Debug.LogError("X curve is null!");
            }
            if (this.quaternionY == null)
            {
                Debug.LogError("Y curve is null!");
            }
            if (this.quaternionZ == null)
            {
                Debug.LogError("Z curve is null!");
            }
            if (this.quaternionW == null)
            {
                Debug.LogError("W curve is null!");
            }
            if (((this.quaternionX.length != 0) && (this.quaternionY.length != 0)) && ((this.quaternionZ.length != 0) && (this.quaternionW.length != 0)))
            {
                Quaternion q = this.EvaluateQuaternionCurvesDirectly(time);
                if (keyReference)
                {
                    this.refEuler = this.EvaluateEulerCurvesDirectly(time);
                }
                this.refEuler = QuaternionCurveTangentCalculation.GetEulerFromQuaternion(q, this.refEuler);
            }
            else
            {
                this.refEuler = this.EvaluateEulerCurvesDirectly(time);
            }
            return this.refEuler;
        }

        public WrapMode PostWrapMode()
        {
            return this.postWrapMode;
        }

        public WrapMode PreWrapMode()
        {
            return this.preWrapMode;
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
                return ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.eulerX.length <= 0)) ? this.m_CustomRangeEnd : this.eulerX.keys[this.eulerX.length - 1].time);
            }
        }

        private float rangeStart
        {
            get
            {
                return ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.eulerX.length <= 0)) ? this.m_CustomRangeStart : this.eulerX.keys[0].time);
            }
        }
    }
}

