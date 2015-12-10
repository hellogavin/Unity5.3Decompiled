namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationRecording
    {
        private static void AddKey(AnimationWindowState state, EditorCurveBinding binding, Type type, PropertyModification modification)
        {
            GameObject activeRootGameObject = state.activeRootGameObject;
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            AnimationWindowCurve curve = new AnimationWindowCurve(activeAnimationClip, binding, type);
            object currentValue = CurveBindingUtility.GetCurrentValue(activeRootGameObject, binding);
            if (curve.length == 0)
            {
                object outObject = null;
                if (!ValueFromPropertyModification(modification, binding, out outObject))
                {
                    outObject = currentValue;
                }
                if (state.frame != 0)
                {
                    AnimationWindowUtility.AddKeyframeToCurve(curve, outObject, type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
                }
            }
            AnimationWindowUtility.AddKeyframeToCurve(curve, currentValue, type, AnimationKeyTime.Frame(state.frame, activeAnimationClip.frameRate));
            state.SaveCurve(curve);
        }

        private static PropertyModification FindPropertyModification(GameObject root, UndoPropertyModification[] modifications, EditorCurveBinding binding)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                if ((modifications[i].currentValue == null) || !(modifications[i].currentValue.target is Animator))
                {
                    EditorCurveBinding binding2;
                    AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out binding2);
                    if (binding2 == binding)
                    {
                        return modifications[i].previousValue;
                    }
                }
            }
            return null;
        }

        private static bool HasAnyRecordableModifications(GameObject root, UndoPropertyModification[] modifications)
        {
            for (int i = 0; i < modifications.Length; i++)
            {
                EditorCurveBinding binding;
                if (((modifications[i].currentValue == null) || !(modifications[i].currentValue.target is Animator)) && (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out binding) != null))
                {
                    return true;
                }
            }
            return false;
        }

        public static UndoPropertyModification[] Process(AnimationWindowState state, UndoPropertyModification[] modifications)
        {
            GameObject activeRootGameObject = state.activeRootGameObject;
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            Animator component = activeRootGameObject.GetComponent<Animator>();
            if (!HasAnyRecordableModifications(activeRootGameObject, modifications))
            {
                return modifications;
            }
            List<UndoPropertyModification> list = new List<UndoPropertyModification>();
            for (int i = 0; i < modifications.Length; i++)
            {
                EditorCurveBinding binding = new EditorCurveBinding();
                PropertyModification previousValue = modifications[i].previousValue;
                Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, activeRootGameObject, out binding);
                if ((type != null) && (type != typeof(Animator)))
                {
                    if (((component != null) && component.isHuman) && ((binding.type == typeof(Transform)) && component.IsBoneTransform(previousValue.target as Transform)))
                    {
                        Debug.LogWarning("Keyframing for humanoid rig is not supported!", previousValue.target as Transform);
                    }
                    else
                    {
                        AnimationMode.AddPropertyModification(binding, previousValue, modifications[i].keepPrefabOverride);
                        EditorCurveBinding[] bindingArray = RotationCurveInterpolation.RemapAnimationBindingForAddKey(binding, activeAnimationClip);
                        if (bindingArray != null)
                        {
                            for (int j = 0; j < bindingArray.Length; j++)
                            {
                                AddKey(state, bindingArray[j], type, FindPropertyModification(activeRootGameObject, modifications, bindingArray[j]));
                            }
                        }
                        else
                        {
                            AddKey(state, binding, type, previousValue);
                        }
                    }
                }
                else
                {
                    list.Add(modifications[i]);
                }
            }
            return list.ToArray();
        }

        private static bool ValueFromPropertyModification(PropertyModification modification, EditorCurveBinding binding, out object outObject)
        {
            float num;
            if (modification == null)
            {
                outObject = null;
                return false;
            }
            if (binding.isPPtrCurve)
            {
                outObject = modification.objectReference;
                return true;
            }
            if (float.TryParse(modification.value, out num))
            {
                outObject = num;
                return true;
            }
            outObject = null;
            return false;
        }
    }
}

