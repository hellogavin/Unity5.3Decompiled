namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class MuscleClipEditorUtilities
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CalculateQualityCurves(AnimationClip clip, QualityCurvesTime time, Vector2[] poseCurve, Vector2[] rotationCurve, Vector2[] heightCurve, Vector2[] positionCurve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern MuscleClipQualityInfo GetMuscleClipQualityInfo(AnimationClip clip, float startTime, float stopTime);
    }
}

