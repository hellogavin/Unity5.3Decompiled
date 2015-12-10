namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class RotationCurveInterpolation
    {
        public static char[] kPostFix = new char[] { 'x', 'y', 'z', 'w' };

        internal static EditorCurveBinding[] ConvertRotationPropertiesToDefaultInterpolation(AnimationClip clip, EditorCurveBinding[] selection)
        {
            return ConvertRotationPropertiesToInterpolationType(selection, Mode.RawEuler);
        }

        internal static EditorCurveBinding[] ConvertRotationPropertiesToInterpolationType(EditorCurveBinding[] selection, Mode newInterpolationMode)
        {
            if ((selection.Length == 4) && (GetModeFromCurveData(selection[0]) == Mode.RawQuaternions))
            {
                EditorCurveBinding[] bindingArray = new EditorCurveBinding[] { selection[0], selection[1], selection[2] };
                string prefixForInterpolation = GetPrefixForInterpolation(newInterpolationMode);
                bindingArray[0].propertyName = prefixForInterpolation + ".x";
                bindingArray[1].propertyName = prefixForInterpolation + ".y";
                bindingArray[2].propertyName = prefixForInterpolation + ".z";
                return bindingArray;
            }
            return selection;
        }

        public static char ExtractComponentCharacter(string name)
        {
            return name[name.Length - 1];
        }

        private static EditorCurveBinding[] GenerateTransformCurveBindingArray(string path, string property, Type type, int count)
        {
            EditorCurveBinding[] bindingArray = new EditorCurveBinding[count];
            for (int i = 0; i < count; i++)
            {
                bindingArray[i] = EditorCurveBinding.FloatCurve(path, type, property + kPostFix[i]);
            }
            return bindingArray;
        }

        public static int GetCurveIndexFromName(string name)
        {
            return (ExtractComponentCharacter(name) - 'x');
        }

        public static State GetCurveState(AnimationClip clip, EditorCurveBinding[] selection)
        {
            State state;
            state.allAreRaw = true;
            state.allAreNonBaked = true;
            state.allAreBaked = true;
            state.allAreRotations = true;
            foreach (EditorCurveBinding binding in selection)
            {
                Mode modeFromCurveData = GetModeFromCurveData(binding);
                state.allAreBaked &= modeFromCurveData == Mode.Baked;
                state.allAreNonBaked &= modeFromCurveData == Mode.NonBaked;
                state.allAreRaw &= modeFromCurveData == Mode.RawEuler;
                state.allAreRotations &= modeFromCurveData != Mode.Undefined;
            }
            return state;
        }

        public static Mode GetModeFromCurveData(EditorCurveBinding data)
        {
            if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("localEulerAngles"))
            {
                if (data.propertyName.StartsWith("localEulerAnglesBaked"))
                {
                    return Mode.Baked;
                }
                if (data.propertyName.StartsWith("localEulerAnglesRaw"))
                {
                    return Mode.RawEuler;
                }
                return Mode.NonBaked;
            }
            if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("m_LocalRotation"))
            {
                return Mode.RawQuaternions;
            }
            return Mode.Undefined;
        }

        public static string GetPrefixForInterpolation(Mode newInterpolationMode)
        {
            if (newInterpolationMode == Mode.Baked)
            {
                return "localEulerAnglesBaked";
            }
            if (newInterpolationMode == Mode.NonBaked)
            {
                return "localEulerAngles";
            }
            if (newInterpolationMode == Mode.RawEuler)
            {
                return "localEulerAnglesRaw";
            }
            if (newInterpolationMode == Mode.RawQuaternions)
            {
                return "m_LocalRotation";
            }
            return null;
        }

        public static EditorCurveBinding[] RemapAnimationBindingForAddKey(EditorCurveBinding binding, AnimationClip clip)
        {
            if (!AnimationWindowUtility.IsTransformType(binding.type))
            {
                return null;
            }
            if (binding.propertyName.StartsWith("m_LocalPosition."))
            {
                if (binding.type == typeof(Transform))
                {
                    return GenerateTransformCurveBindingArray(binding.path, "m_LocalPosition.", binding.type, 3);
                }
                return null;
            }
            if (binding.propertyName.StartsWith("m_LocalScale."))
            {
                return GenerateTransformCurveBindingArray(binding.path, "m_LocalScale.", binding.type, 3);
            }
            if (!binding.propertyName.StartsWith("m_LocalRotation"))
            {
                return null;
            }
            EditorCurveBinding binding2 = binding;
            binding2.propertyName = "localEulerAnglesBaked.x";
            if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
            {
                return GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesBaked.", binding.type, 3);
            }
            binding2.propertyName = "localEulerAngles.x";
            if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
            {
                return GenerateTransformCurveBindingArray(binding.path, "localEulerAngles.", binding.type, 3);
            }
            return GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesRaw.", binding.type, 3);
        }

        public static EditorCurveBinding RemapAnimationBindingForRotationCurves(EditorCurveBinding curveBinding, AnimationClip clip)
        {
            if (AnimationWindowUtility.IsTransformType(curveBinding.type))
            {
                if (!curveBinding.propertyName.StartsWith("m_LocalRotation"))
                {
                    return curveBinding;
                }
                char[] separator = new char[] { '.' };
                string str = curveBinding.propertyName.Split(separator)[1];
                EditorCurveBinding binding = curveBinding;
                binding.propertyName = "localEulerAngles." + str;
                if (AnimationUtility.GetEditorCurve(clip, binding) != null)
                {
                    return binding;
                }
                binding.propertyName = "localEulerAnglesBaked." + str;
                if (AnimationUtility.GetEditorCurve(clip, binding) != null)
                {
                    return binding;
                }
                binding.propertyName = "localEulerAnglesRaw." + str;
                if (AnimationUtility.GetEditorCurve(clip, binding) != null)
                {
                    return binding;
                }
            }
            return curveBinding;
        }

        internal static void SetInterpolation(AnimationClip clip, EditorCurveBinding[] curveBindings, Mode newInterpolationMode)
        {
            Undo.RegisterCompleteObjectUndo(clip, "Rotation Interpolation");
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            List<AnimationCurve> list2 = new List<AnimationCurve>();
            List<EditorCurveBinding> list3 = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in curveBindings)
            {
                switch (GetModeFromCurveData(binding))
                {
                    case Mode.Undefined:
                        break;

                    case Mode.RawQuaternions:
                        Debug.LogWarning("Can't convert quaternion curve: " + binding.propertyName);
                        break;

                    default:
                    {
                        AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, binding);
                        if (editorCurve != null)
                        {
                            string str = GetPrefixForInterpolation(newInterpolationMode) + '.' + ExtractComponentCharacter(binding.propertyName);
                            EditorCurveBinding item = new EditorCurveBinding {
                                propertyName = str,
                                type = binding.type,
                                path = binding.path
                            };
                            list.Add(item);
                            list2.Add(editorCurve);
                            EditorCurveBinding binding3 = new EditorCurveBinding {
                                propertyName = binding.propertyName,
                                type = binding.type,
                                path = binding.path
                            };
                            list3.Add(binding3);
                        }
                        break;
                    }
                }
            }
            Undo.RegisterCompleteObjectUndo(clip, "Rotation Interpolation");
            foreach (EditorCurveBinding binding4 in list3)
            {
                AnimationUtility.SetEditorCurve(clip, binding4, null);
            }
            foreach (EditorCurveBinding binding5 in list)
            {
                AnimationUtility.SetEditorCurve(clip, binding5, list2[list.IndexOf(binding5)]);
            }
        }

        public enum Mode
        {
            Baked,
            NonBaked,
            RawQuaternions,
            RawEuler,
            Undefined
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct State
        {
            public bool allAreNonBaked;
            public bool allAreBaked;
            public bool allAreRaw;
            public bool allAreRotations;
        }
    }
}

