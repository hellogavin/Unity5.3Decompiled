namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    public sealed class AnimationUtility
    {
        public static OnCurveWasModified onCurveWasModified;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool AmbiguousBinding(string path, int classID, Transform root);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CalculateTransformPath(Transform targetTransform, Transform root);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ConstrainToPolynomialCurve(AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CurveSupportsProcedural(AnimationCurve curve);
        [ExcludeFromDocs, Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead.")]
        public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip)
        {
            bool includeCurveData = true;
            return GetAllCurves(clip, includeCurveData);
        }

        [Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead.")]
        public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip, [DefaultValue("true")] bool includeCurveData)
        {
            EditorCurveBinding[] curveBindings = GetCurveBindings(clip);
            AnimationClipCurveData[] dataArray = new AnimationClipCurveData[curveBindings.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = new AnimationClipCurveData(curveBindings[i]);
                if (includeCurveData)
                {
                    dataArray[i].curve = GetEditorCurve(clip, curveBindings[i]);
                }
            }
            return dataArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern EditorCurveBinding[] GetAnimatableBindings(GameObject targetObject, GameObject root);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetAnimatedObject(GameObject root, EditorCurveBinding binding);
        [Obsolete("GetAnimationClips(Animation) is deprecated. Use GetAnimationClips(GameObject) instead.")]
        public static AnimationClip[] GetAnimationClips(Animation component)
        {
            return GetAnimationClips(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AnimationClip[] GetAnimationClips(GameObject gameObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AnimationClipSettings GetAnimationClipSettings(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern AnimationClipStats GetAnimationClipStats(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AnimationEvent[] GetAnimationEvents(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern EditorCurveBinding[] GetCurveBindings(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AnimationCurve GetEditorCurve(AnimationClip clip, EditorCurveBinding binding);
        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static AnimationCurve GetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName)
        {
            return GetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Type GetEditorCurveValueType(GameObject root, EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetFloatValue(GameObject root, EditorCurveBinding binding, out float data);
        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static bool GetFloatValue(GameObject root, string relativePath, Type type, string propertyName, out float data)
        {
            return GetFloatValue(root, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), out data);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetGenerateMotionCurves(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ObjectReferenceKeyframe[] GetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern EditorCurveBinding[] GetObjectReferenceCurveBindings(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetObjectReferenceValue(GameObject root, EditorCurveBinding binding, out Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasGenericRootTransform(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasMotionCurves(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasMotionFloatCurves(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasRootCurves(AnimationClip clip);
        [Obsolete("Use AnimationMode.InAnimationMode instead")]
        public static bool InAnimationMode()
        {
            return AnimationMode.InAnimationMode();
        }

        [RequiredByNativeCode]
        private static void Internal_CallAnimationClipAwake(AnimationClip clip)
        {
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, new EditorCurveBinding(), CurveModifiedType.ClipModified);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsValidPolynomialCurve(AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Type PropertyModificationToEditorCurveBinding(PropertyModification modification, GameObject gameObject, out EditorCurveBinding binding);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAdditiveReferencePose(AnimationClip clip, AnimationClip referenceClip, float time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAnimationClips(Animation animation, AnimationClip[] clips);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetAnimationClipSettingsNoDirty(AnimationClip clip, AnimationClipSettings srcClipInfo);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetAnimationEvents(AnimationClip clip, AnimationEvent[] events);
        [Obsolete("SetAnimationType is no longer supported", true)]
        public static void SetAnimationType(AnimationClip clip, ModelImporterAnimationType type)
        {
        }

        public static void SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve)
        {
            Internal_SetEditorCurve(clip, binding, curve);
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, binding, (curve == null) ? CurveModifiedType.CurveDeleted : CurveModifiedType.CurveModified);
            }
        }

        [Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
        public static void SetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName, AnimationCurve curve)
        {
            SetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), curve);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetGenerateMotionCurves(AnimationClip clip, bool value);
        public static void SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
        {
            Internal_SetObjectReferenceCurve(clip, binding, keyframes);
            if (onCurveWasModified != null)
            {
                onCurveWasModified(clip, binding, (keyframes == null) ? CurveModifiedType.CurveDeleted : CurveModifiedType.CurveModified);
            }
        }

        [Obsolete("Use AnimationMode.StartAnimationmode instead")]
        public static void StartAnimationMode(Object[] objects)
        {
            Debug.LogWarning("AnimationUtility.StartAnimationMode is deprecated. Use AnimationMode.StartAnimationMode with the new APIs. The objects passed to this function will no longer be reverted automatically. See AnimationMode.AddPropertyModification");
            AnimationMode.StartAnimationMode();
        }

        [Obsolete("Use AnimationMode.StopAnimationMode instead")]
        public static void StopAnimationMode()
        {
            AnimationMode.StopAnimationMode();
        }

        public enum CurveModifiedType
        {
            CurveDeleted,
            CurveModified,
            ClipModified
        }

        public delegate void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted);
    }
}

