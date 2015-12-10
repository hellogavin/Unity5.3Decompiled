namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal interface CurveRenderer
    {
        void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor);
        float EvaluateCurveDeltaSlow(float time);
        float EvaluateCurveSlow(float time);
        Bounds GetBounds();
        Bounds GetBounds(float minTime, float maxTime);
        AnimationCurve GetCurve();
        float RangeEnd();
        float RangeStart();
        void SetCustomRange(float start, float end);
        void SetWrap(WrapMode wrap);
        void SetWrap(WrapMode preWrap, WrapMode postWrap);
    }
}

