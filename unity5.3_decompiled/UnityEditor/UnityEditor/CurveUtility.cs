namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal static class CurveUtility
    {
        private static Texture2D iconCurve;
        private static Texture2D iconKey;
        private static Texture2D iconNone;
        private const int kBrokenMask = 1;
        private const int kLeftTangentMask = 6;
        private const int kRightTangentMask = 0x18;

        private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
        {
            Keyframe keyframe = curve[index];
            Keyframe keyframe2 = curve[toIndex];
            Keyframe keyframe3 = curve[index];
            Keyframe keyframe4 = curve[toIndex];
            return ((keyframe.value - keyframe2.value) / (keyframe3.time - keyframe4.time));
        }

        public static float CalculateSmoothTangent(Keyframe key)
        {
            if (key.inTangent == float.PositiveInfinity)
            {
                key.inTangent = 0f;
            }
            if (key.outTangent == float.PositiveInfinity)
            {
                key.outTangent = 0f;
            }
            return ((key.outTangent + key.inTangent) * 0.5f);
        }

        public static Color GetBalancedColor(Color c)
        {
            return new Color(0.15f + (0.75f * c.r), 0.2f + (0.6f * c.g), 0.1f + (0.9f * c.b));
        }

        public static string GetClipName(AnimationClip clip)
        {
            if (clip == null)
            {
                return "[No Clip]";
            }
            string name = clip.name;
            if ((clip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                name = name + " (Read-Only)";
            }
            return name;
        }

        public static int GetCurveGroupID(AnimationClip clip, EditorCurveBinding curveData)
        {
            if (curveData.type != typeof(Transform))
            {
                return -1;
            }
            int num = (clip != null) ? clip.GetInstanceID() : 0;
            string str = curveData.propertyName.Substring(0, curveData.propertyName.Length - 1);
            return ((((num * 0x4c93) ^ (curveData.path.GetHashCode() * 0x2d9)) ^ (curveData.type.GetHashCode() * 0x1b)) ^ str.GetHashCode());
        }

        public static int GetCurveID(AnimationClip clip, EditorCurveBinding curveData)
        {
            int num = (clip != null) ? clip.GetInstanceID() : 0;
            return ((((num * 0x4c93) ^ (curveData.path.GetHashCode() * 0x2d9)) ^ (curveData.type.GetHashCode() * 0x1b)) ^ curveData.propertyName.GetHashCode());
        }

        public static Texture2D GetIconCurve()
        {
            if (iconCurve == null)
            {
                iconCurve = EditorGUIUtility.LoadIcon("animationanimated");
            }
            return iconCurve;
        }

        public static Texture2D GetIconKey()
        {
            if (iconKey == null)
            {
                iconKey = EditorGUIUtility.LoadIcon("animationkeyframe");
            }
            return iconKey;
        }

        public static bool GetKeyBroken(Keyframe key)
        {
            return ((key.tangentMode & 1) != 0);
        }

        public static TangentMode GetKeyTangentMode(Keyframe key, int leftRight)
        {
            if (leftRight == 0)
            {
                return (TangentMode) ((key.tangentMode & 6) >> 1);
            }
            return (TangentMode) ((key.tangentMode & 0x18) >> 3);
        }

        public static int GetPathAndTypeID(string path, Type type)
        {
            return ((path.GetHashCode() * 0x1b) ^ type.GetHashCode());
        }

        public static Color GetPropertyColor(string name)
        {
            Color white = Color.white;
            int num = 0;
            if (name.StartsWith("m_LocalPosition"))
            {
                num = 1;
            }
            if (name.StartsWith("localEulerAngles"))
            {
                num = 2;
            }
            if (name.StartsWith("m_LocalScale"))
            {
                num = 3;
            }
            if (num == 1)
            {
                if (name.EndsWith(".x"))
                {
                    white = Handles.xAxisColor;
                }
                else if (name.EndsWith(".y"))
                {
                    white = Handles.yAxisColor;
                }
                else if (name.EndsWith(".z"))
                {
                    white = Handles.zAxisColor;
                }
            }
            else if (num == 2)
            {
                if (name.EndsWith(".x"))
                {
                    white = (Color) AnimEditor.kEulerXColor;
                }
                else if (name.EndsWith(".y"))
                {
                    white = (Color) AnimEditor.kEulerYColor;
                }
                else if (name.EndsWith(".z"))
                {
                    white = (Color) AnimEditor.kEulerZColor;
                }
            }
            else if (num == 3)
            {
                if (name.EndsWith(".x"))
                {
                    white = GetBalancedColor(new Color(0.7f, 0.4f, 0.4f));
                }
                else if (name.EndsWith(".y"))
                {
                    white = GetBalancedColor(new Color(0.4f, 0.7f, 0.4f));
                }
                else if (name.EndsWith(".z"))
                {
                    white = GetBalancedColor(new Color(0.4f, 0.4f, 0.7f));
                }
            }
            else if (name.EndsWith(".x"))
            {
                white = Handles.xAxisColor;
            }
            else if (name.EndsWith(".y"))
            {
                white = Handles.yAxisColor;
            }
            else if (name.EndsWith(".z"))
            {
                white = Handles.zAxisColor;
            }
            else if (name.EndsWith(".w"))
            {
                white = new Color(1f, 0.5f, 0f);
            }
            else if (name.EndsWith(".r"))
            {
                white = GetBalancedColor(Color.red);
            }
            else if (name.EndsWith(".g"))
            {
                white = GetBalancedColor(Color.green);
            }
            else if (name.EndsWith(".b"))
            {
                white = GetBalancedColor(Color.blue);
            }
            else if (name.EndsWith(".a"))
            {
                white = GetBalancedColor(Color.yellow);
            }
            else if (name.EndsWith(".width"))
            {
                white = GetBalancedColor(Color.blue);
            }
            else if (name.EndsWith(".height"))
            {
                white = GetBalancedColor(Color.yellow);
            }
            else
            {
                float f = 6.283185f * (name.GetHashCode() % 0x3e8);
                f -= Mathf.Floor(f);
                white = GetBalancedColor(Color.HSVToRGB(f, 1f, 1f));
            }
            white.a = 1f;
            return white;
        }

        public static bool HaveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
        {
            for (int i = curve.length - 1; i >= 0; i--)
            {
                Keyframe keyframe = curve[i];
                if (keyframe.time >= beginTime)
                {
                    Keyframe keyframe2 = curve[i];
                    if (keyframe2.time < endTime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void RemoveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
        {
            for (int i = curve.length - 1; i >= 0; i--)
            {
                Keyframe keyframe = curve[i];
                if (keyframe.time >= beginTime)
                {
                    Keyframe keyframe2 = curve[i];
                    if (keyframe2.time < endTime)
                    {
                        curve.RemoveKey(i);
                    }
                }
            }
        }

        public static void SetKeyBroken(ref Keyframe key, bool broken)
        {
            if (broken)
            {
                key.tangentMode |= 1;
            }
            else
            {
                key.tangentMode &= -2;
            }
        }

        public static void SetKeyModeFromContext(AnimationCurve curve, int keyIndex)
        {
            Keyframe key = curve[keyIndex];
            bool broken = false;
            if ((keyIndex > 0) && GetKeyBroken(curve[keyIndex - 1]))
            {
                broken = true;
            }
            if ((keyIndex < (curve.length - 1)) && GetKeyBroken(curve[keyIndex + 1]))
            {
                broken = true;
            }
            SetKeyBroken(ref key, broken);
            if (broken)
            {
                if (keyIndex > 0)
                {
                    SetKeyTangentMode(ref key, 0, GetKeyTangentMode(curve[keyIndex - 1], 1));
                }
                if (keyIndex < (curve.length - 1))
                {
                    SetKeyTangentMode(ref key, 1, GetKeyTangentMode(curve[keyIndex + 1], 0));
                }
            }
            else
            {
                TangentMode smooth = TangentMode.Smooth;
                if ((keyIndex > 0) && (GetKeyTangentMode(curve[keyIndex - 1], 1) != TangentMode.Smooth))
                {
                    smooth = TangentMode.Editable;
                }
                if ((keyIndex < (curve.length - 1)) && (GetKeyTangentMode(curve[keyIndex + 1], 0) != TangentMode.Smooth))
                {
                    smooth = TangentMode.Editable;
                }
                SetKeyTangentMode(ref key, 0, smooth);
                SetKeyTangentMode(ref key, 1, smooth);
            }
            curve.MoveKey(keyIndex, key);
        }

        public static void SetKeyTangentMode(ref Keyframe key, int leftRight, TangentMode mode)
        {
            if (leftRight == 0)
            {
                key.tangentMode &= -7;
                key.tangentMode |= ((int) mode) << 1;
            }
            else
            {
                key.tangentMode &= -25;
                key.tangentMode |= ((int) mode) << 3;
            }
            if (GetKeyTangentMode(key, leftRight) != mode)
            {
                Debug.Log("bug");
            }
        }

        public static void UpdateTangentsFromMode(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                UpdateTangentsFromMode(curve, i);
            }
        }

        private static void UpdateTangentsFromMode(AnimationCurve curve, int index)
        {
            if ((index >= 0) && (index < curve.length))
            {
                Keyframe key = curve[index];
                if ((GetKeyTangentMode(key, 0) == TangentMode.Linear) && (index >= 1))
                {
                    key.inTangent = CalculateLinearTangent(curve, index, index - 1);
                    curve.MoveKey(index, key);
                }
                if ((GetKeyTangentMode(key, 1) == TangentMode.Linear) && ((index + 1) < curve.length))
                {
                    key.outTangent = CalculateLinearTangent(curve, index, index + 1);
                    curve.MoveKey(index, key);
                }
                if ((GetKeyTangentMode(key, 0) == TangentMode.Smooth) || (GetKeyTangentMode(key, 1) == TangentMode.Smooth))
                {
                    curve.SmoothTangents(index, 0f);
                }
            }
        }

        public static void UpdateTangentsFromModeSurrounding(AnimationCurve curve, int index)
        {
            UpdateTangentsFromMode(curve, index - 2);
            UpdateTangentsFromMode(curve, index - 1);
            UpdateTangentsFromMode(curve, index);
            UpdateTangentsFromMode(curve, index + 1);
            UpdateTangentsFromMode(curve, index + 2);
        }
    }
}

