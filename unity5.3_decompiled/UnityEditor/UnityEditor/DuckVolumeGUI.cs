namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class DuckVolumeGUI : IAudioEffectPluginGUI
    {
        private static DragType dragtype = DragType.None;
        public static string kAttackTimeName = "Attack Time";
        public static string kKneeName = "Knee";
        public static string kMakeupGainName = "Make-up Gain";
        public static string kRatioName = "Ratio";
        public static string kReleaseTimeName = "Release Time";
        public static string kThresholdName = "Threshold";
        public static GUIStyle textStyle10 = BuildGUIStyleForLabel(Color.grey, 10, false, FontStyle.Normal, TextAnchor.MiddleLeft);

        public static GUIStyle BuildGUIStyleForLabel(Color color, int fontSize, bool wrapText, FontStyle fontstyle, TextAnchor anchor)
        {
            GUIStyle style;
            style = new GUIStyle {
                focused = { background = style.onNormal.background, textColor = color },
                alignment = anchor,
                fontSize = fontSize,
                fontStyle = fontstyle,
                wordWrap = wrapText,
                clipping = TextClipping.Overflow
            };
            style.normal.textColor = color;
            return style;
        }

        private static bool CurveDisplay(IAudioEffectPlugin plugin, Rect r0, ref float threshold, ref float ratio, ref float makeupGain, ref float attackTime, ref float releaseTime, ref float knee, float sidechainLevel, float outputLevel, float blend)
        {
            float num4;
            float num5;
            float num6;
            float num7;
            float num8;
            float num9;
            float num10;
            float num11;
            float num12;
            float num13;
            float num14;
            float num15;
            <CurveDisplay>c__AnonStorey59 storey = new <CurveDisplay>c__AnonStorey59 {
                blend = blend
            };
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Rect r = AudioCurveRendering.BeginCurveFrame(r0);
            float num3 = 10f;
            plugin.GetFloatParameterInfo(kThresholdName, out num4, out num5, out num6);
            plugin.GetFloatParameterInfo(kRatioName, out num7, out num8, out num9);
            plugin.GetFloatParameterInfo(kMakeupGainName, out num10, out num11, out num12);
            plugin.GetFloatParameterInfo(kKneeName, out num13, out num14, out num15);
            storey.dbRange = 100f;
            storey.dbMin = -80f;
            float num16 = (r.width * (threshold - storey.dbMin)) / storey.dbRange;
            bool flag = false;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (r.Contains(Event.current.mousePosition) && (current.button == 0))
                    {
                        dragtype = DragType.None;
                        GUIUtility.hotControl = controlID;
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        current.Use();
                        if (Mathf.Abs((float) ((r.x + num16) - current.mousePosition.x)) < 10f)
                        {
                            dragtype = DragType.ThresholdAndKnee;
                        }
                        else
                        {
                            dragtype = (current.mousePosition.x >= (r.x + num16)) ? DragType.Ratio : DragType.MakeupGain;
                        }
                    }
                    goto Label_02F7;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        dragtype = DragType.None;
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        current.Use();
                    }
                    goto Label_02F7;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        goto Label_02F7;
                    }
                    float num17 = !current.alt ? 1f : 0.25f;
                    if (dragtype != DragType.ThresholdAndKnee)
                    {
                        if (dragtype == DragType.Ratio)
                        {
                            ratio = Mathf.Clamp(ratio + ((current.delta.y * ((ratio <= 1f) ? 0.003f : 0.05f)) * num17), num7, num8);
                        }
                        else if (dragtype == DragType.MakeupGain)
                        {
                            makeupGain = Mathf.Clamp(makeupGain - ((current.delta.y * 0.5f) * num17), num10, num11);
                        }
                        else
                        {
                            Debug.LogError("Drag: Unhandled enum");
                        }
                        break;
                    }
                    if (Mathf.Abs(current.delta.x) >= Mathf.Abs(current.delta.y))
                    {
                        threshold = Mathf.Clamp(threshold + ((current.delta.x * 0.1f) * num17), num4, num5);
                        break;
                    }
                    knee = Mathf.Clamp(knee + ((current.delta.y * 0.5f) * num17), num13, num14);
                    break;
                }
                default:
                    goto Label_02F7;
            }
            flag = true;
            current.Use();
        Label_02F7:
            if (current.type == EventType.Repaint)
            {
                <CurveDisplay>c__AnonStorey5A storeya = new <CurveDisplay>c__AnonStorey5A {
                    <>f__ref$89 = storey
                };
                HandleUtility.ApplyWireMaterial();
                float num18 = r.height * (1f - (((threshold - storey.dbMin) + makeupGain) / storey.dbRange));
                Color col = new Color(0.7f, 0.7f, 0.7f);
                Color black = Color.black;
                storeya.duckGradient = 1f / ratio;
                storeya.duckThreshold = threshold;
                storeya.duckSidechainLevel = sidechainLevel;
                storeya.duckMakeupGain = makeupGain;
                storeya.duckKnee = knee;
                storeya.duckKneeC1 = (knee <= 0f) ? 0f : ((storeya.duckGradient - 1f) / (4f * knee));
                storeya.duckKneeC2 = storeya.duckThreshold - knee;
                AudioCurveRendering.DrawFilledCurve(r, new AudioCurveRendering.AudioCurveAndColorEvaluator(storeya.<>m__9B));
                if (dragtype == DragType.MakeupGain)
                {
                    AudioCurveRendering.DrawCurve(r, new AudioCurveRendering.AudioCurveEvaluator(storeya.<>m__9C), Color.white);
                }
                textStyle10.normal.textColor = ScaleAlpha(col, storey.blend);
                EditorGUI.DrawRect(new Rect(r.x + num16, r.y, 1f, r.height), textStyle10.normal.textColor);
                DrawText((r.x + num16) + 4f, r.y + 6f, string.Format("Threshold: {0:F1} dB", (float) threshold));
                textStyle10.normal.textColor = ScaleAlpha(black, storey.blend);
                DrawText(r.x + 4f, (r.y + r.height) - 10f, (sidechainLevel >= -80f) ? string.Format("Input: {0:F1} dB", sidechainLevel) : "Input: None");
                if (dragtype == DragType.Ratio)
                {
                    float num19 = r.height / r.width;
                    Color[] colors = new Color[] { Color.black, Color.black };
                    Vector3[] points = new Vector3[] { new Vector3((r.x + num16) + r.width, (r.y + num18) - (num19 * r.width), 0f), new Vector3((r.x + num16) - r.width, (r.y + num18) + (num19 * r.width), 0f) };
                    Handles.DrawAAPolyLine(2f, colors, points);
                    Color[] colorArray2 = new Color[] { Color.white, Color.white };
                    Vector3[] vectorArray2 = new Vector3[] { new Vector3((r.x + num16) + r.width, (r.y + num18) - ((num19 * storeya.duckGradient) * r.width), 0f), new Vector3((r.x + num16) - r.width, (r.y + num18) + ((num19 * storeya.duckGradient) * r.width), 0f) };
                    Handles.DrawAAPolyLine(3f, colorArray2, vectorArray2);
                }
                else if (dragtype == DragType.ThresholdAndKnee)
                {
                    float x = ((threshold - knee) - storey.dbMin) / storey.dbRange;
                    float num21 = ((threshold + knee) - storey.dbMin) / storey.dbRange;
                    float num22 = EvaluateDuckingVolume(x, ratio, threshold, makeupGain, knee, storey.dbRange, storey.dbMin);
                    float num23 = EvaluateDuckingVolume(num21, ratio, threshold, makeupGain, knee, storey.dbRange, storey.dbMin);
                    float y = r.yMax - (((num22 + 1f) * 0.5f) * r.height);
                    float num25 = r.yMax - (((num23 + 1f) * 0.5f) * r.height);
                    EditorGUI.DrawRect(new Rect(r.x + (x * r.width), y, 1f, r.height - y), new Color(0f, 0f, 0f, 0.5f));
                    EditorGUI.DrawRect(new Rect((r.x + (num21 * r.width)) - 1f, num25, 1f, r.height - num25), new Color(0f, 0f, 0f, 0.5f));
                    EditorGUI.DrawRect(new Rect((r.x + num16) - 1f, r.y, 3f, r.height), Color.white);
                }
                outputLevel = (Mathf.Clamp(outputLevel - makeupGain, storey.dbMin, storey.dbMin + storey.dbRange) - storey.dbMin) / storey.dbRange;
                if (EditorApplication.isPlaying)
                {
                    Rect rect2 = new Rect(((r.x + r.width) - num3) + 2f, r.y + 2f, num3 - 4f, r.height - 4f);
                    DrawVU(rect2, outputLevel, storey.blend, true);
                }
            }
            AudioCurveRendering.EndCurveFrame();
            return flag;
        }

        public static void DrawLine(float x1, float y1, float x2, float y2, Color col)
        {
            Handles.color = col;
            Handles.DrawLine(new Vector3(x1, y1, 0f), new Vector3(x2, y2, 0f));
        }

        public static void DrawText(float x, float y, string text)
        {
            GUI.Label(new Rect(x, y - 5f, 200f, 10f), new GUIContent(text, string.Empty), textStyle10);
        }

        protected static void DrawVU(Rect r, float level, float blend, bool topdown)
        {
            level = 1f - level;
            Rect rect = new Rect(r.x + 1f, (r.y + 1f) + (!topdown ? (level * r.height) : 0f), r.width - 2f, (r.y - 2f) + (!topdown ? (r.height - (level * r.height)) : (level * r.height)));
            AudioMixerDrawUtils.DrawRect(r, new Color(0.1f, 0.1f, 0.1f));
            AudioMixerDrawUtils.DrawRect(rect, new Color(0.6f, 0.2f, 0.2f));
        }

        private static float EvaluateDuckingVolume(float x, float ratio, float threshold, float makeupGain, float knee, float dbRange, float dbMin)
        {
            float num = 1f / ratio;
            float num2 = threshold;
            float num3 = makeupGain;
            float num4 = knee;
            float num5 = (knee <= 0f) ? 0f : ((num - 1f) / (4f * knee));
            float num6 = num2 - knee;
            float num7 = (x * dbRange) + dbMin;
            float num8 = num7;
            float num9 = num7 - num2;
            if ((num9 > -num4) && (num9 < num4))
            {
                num9 += num4;
                num8 = (num9 * ((num5 * num9) + 1f)) + num6;
            }
            else if (num9 > 0f)
            {
                num8 = num2 + (num * num9);
            }
            return (((2f * ((num8 + num3) - dbMin)) / dbRange) - 1f);
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            float num2;
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            float[] numArray;
            float blend = !plugin.IsPluginEditableAndEnabled() ? 0.5f : 1f;
            plugin.GetFloatParameter(kThresholdName, out num2);
            plugin.GetFloatParameter(kRatioName, out num3);
            plugin.GetFloatParameter(kMakeupGainName, out num4);
            plugin.GetFloatParameter(kAttackTimeName, out num5);
            plugin.GetFloatParameter(kReleaseTimeName, out num6);
            plugin.GetFloatParameter(kKneeName, out num7);
            plugin.GetFloatBuffer("Metering", out numArray, 2);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect rect = GUILayoutUtility.GetRect((float) 200f, (float) 160f, options);
            if (CurveDisplay(plugin, rect, ref num2, ref num3, ref num4, ref num5, ref num6, ref num7, numArray[0], numArray[1], blend))
            {
                plugin.SetFloatParameter(kThresholdName, num2);
                plugin.SetFloatParameter(kRatioName, num3);
                plugin.SetFloatParameter(kMakeupGainName, num4);
                plugin.SetFloatParameter(kAttackTimeName, num5);
                plugin.SetFloatParameter(kReleaseTimeName, num6);
                plugin.SetFloatParameter(kKneeName, num7);
            }
            return true;
        }

        protected static Color ScaleAlpha(Color col, float blend)
        {
            return new Color(col.r, col.g, col.b, col.a * blend);
        }

        public override string Description
        {
            get
            {
                return "Volume Ducking";
            }
        }

        public override string Name
        {
            get
            {
                return "Duck Volume";
            }
        }

        public override string Vendor
        {
            get
            {
                return "Unity Technologies";
            }
        }

        [CompilerGenerated]
        private sealed class <CurveDisplay>c__AnonStorey59
        {
            internal float blend;
            internal float dbMin;
            internal float dbRange;
        }

        [CompilerGenerated]
        private sealed class <CurveDisplay>c__AnonStorey5A
        {
            internal DuckVolumeGUI.<CurveDisplay>c__AnonStorey59 <>f__ref$89;
            internal float duckGradient;
            internal float duckKnee;
            internal float duckKneeC1;
            internal float duckKneeC2;
            internal float duckMakeupGain;
            internal float duckSidechainLevel;
            internal float duckThreshold;

            internal float <>m__9B(float x, out Color col)
            {
                float num = (x * this.<>f__ref$89.dbRange) + this.<>f__ref$89.dbMin;
                float num2 = num;
                float num3 = num - this.duckThreshold;
                col = DuckVolumeGUI.ScaleAlpha((this.duckSidechainLevel <= num) ? Color.grey : AudioCurveRendering.kAudioOrange, this.<>f__ref$89.blend);
                if ((num3 > -this.duckKnee) && (num3 < this.duckKnee))
                {
                    num3 += this.duckKnee;
                    num2 = (num3 * ((this.duckKneeC1 * num3) + 1f)) + this.duckKneeC2;
                    if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.ThresholdAndKnee)
                    {
                        col = new Color(col.r * 1.2f, col.g * 1.2f, col.b * 1.2f);
                    }
                }
                else if (num3 > 0f)
                {
                    num2 = this.duckThreshold + (this.duckGradient * num3);
                }
                return (((2f * ((num2 + this.duckMakeupGain) - this.<>f__ref$89.dbMin)) / this.<>f__ref$89.dbRange) - 1f);
            }

            internal float <>m__9C(float x)
            {
                float num = (x * this.<>f__ref$89.dbRange) + this.<>f__ref$89.dbMin;
                float num2 = num;
                float num3 = num - this.duckThreshold;
                if ((num3 > -this.duckKnee) && (num3 < this.duckKnee))
                {
                    num3 += this.duckKnee;
                    num2 = (num3 * ((this.duckKneeC1 * num3) + 1f)) + this.duckKneeC2;
                }
                else if (num3 > 0f)
                {
                    num2 = this.duckThreshold + (this.duckGradient * num3);
                }
                return (((2f * ((num2 + this.duckMakeupGain) - this.<>f__ref$89.dbMin)) / this.<>f__ref$89.dbRange) - 1f);
            }
        }

        public enum DragType
        {
            None,
            ThresholdAndKnee,
            Ratio,
            MakeupGain
        }
    }
}

