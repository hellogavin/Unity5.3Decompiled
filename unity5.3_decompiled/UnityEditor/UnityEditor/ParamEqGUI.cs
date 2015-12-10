namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ParamEqGUI : IAudioEffectPluginGUI
    {
        public static string kCenterFreqName = "Center freq";
        public static string kFrequencyGainName = "Frequency gain";
        public static string kOctaveRangeName = "Octave range";
        public static GUIStyle textStyle10 = BuildGUIStyleForLabel(Color.grey, 10, false, FontStyle.Normal, TextAnchor.MiddleCenter);
        private const bool useLogScale = true;

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

        private static void DrawFrequencyTickMarks(Rect r, float samplerate, bool logScale, Color col)
        {
            textStyle10.normal.textColor = col;
            float x = r.x;
            float num2 = 60f;
            for (float i = 0f; i < 1f; i += 0.01f)
            {
                float num4 = (float) MapNormalizedFrequency((double) i, (double) samplerate, logScale, true);
                float num5 = r.x + (i * r.width);
                if ((num5 - x) > num2)
                {
                    EditorGUI.DrawRect(new Rect(num5, r.yMax - 5f, 1f, 5f), col);
                    GUI.Label(new Rect(num5, r.yMax - 22f, 1f, 15f), (num4 >= 1000f) ? string.Format("{0:F0} kHz", num4 * 0.001f) : string.Format("{0:F0} Hz", num4), textStyle10);
                    x = num5;
                }
            }
        }

        private static double MapNormalizedFrequency(double f, double sr, bool useLogScale, bool forward)
        {
            double num = 0.5 * sr;
            if (!useLogScale)
            {
                return (!forward ? (f / num) : (f * num));
            }
            if (forward)
            {
                return (10.0 * Math.Pow(num / 10.0, f));
            }
            return (Math.Log(f / 10.0) / Math.Log(num / 10.0));
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            float num2;
            float num3;
            float num4;
            float blend = !plugin.IsPluginEditableAndEnabled() ? 0.5f : 1f;
            plugin.GetFloatParameter(kCenterFreqName, out num2);
            plugin.GetFloatParameter(kOctaveRangeName, out num3);
            plugin.GetFloatParameter(kFrequencyGainName, out num4);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect r = GUILayoutUtility.GetRect((float) 200f, (float) 100f, options);
            if (ParamEqualizerCurveEditor(plugin, r, ref num2, ref num3, ref num4, blend))
            {
                plugin.SetFloatParameter(kCenterFreqName, num2);
                plugin.SetFloatParameter(kOctaveRangeName, num3);
                plugin.SetFloatParameter(kFrequencyGainName, num4);
            }
            return true;
        }

        private static bool ParamEqualizerCurveEditor(IAudioEffectPlugin plugin, Rect r, ref float centerFreq, ref float bandwidth, ref float gain, float blend)
        {
            float num2;
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            float num8;
            float num9;
            float num10;
            <ParamEqualizerCurveEditor>c__AnonStorey5B storeyb = new <ParamEqualizerCurveEditor>c__AnonStorey5B {
                plugin = plugin
            };
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            r = AudioCurveRendering.BeginCurveFrame(r);
            storeyb.plugin.GetFloatParameterInfo(kCenterFreqName, out num2, out num3, out num4);
            storeyb.plugin.GetFloatParameterInfo(kOctaveRangeName, out num5, out num6, out num7);
            storeyb.plugin.GetFloatParameterInfo(kFrequencyGainName, out num8, out num9, out num10);
            bool flag = false;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (r.Contains(Event.current.mousePosition) && (current.button == 0))
                    {
                        GUIUtility.hotControl = controlID;
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        current.Use();
                    }
                    goto Label_01E2;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        current.Use();
                    }
                    goto Label_01E2;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        goto Label_01E2;
                    }
                    float num11 = !Event.current.alt ? 1f : 0.25f;
                    centerFreq = Mathf.Clamp((float) MapNormalizedFrequency(MapNormalizedFrequency((double) centerFreq, (double) storeyb.plugin.GetSampleRate(), true, false) + (current.delta.x / r.width), (double) storeyb.plugin.GetSampleRate(), true, true), num2, num3);
                    if (!Event.current.shift)
                    {
                        gain = Mathf.Clamp(gain - ((current.delta.y * 0.01f) * num11), num8, num9);
                        break;
                    }
                    bandwidth = Mathf.Clamp(bandwidth - ((current.delta.y * 0.02f) * num11), num5, num6);
                    break;
                }
                default:
                    goto Label_01E2;
            }
            flag = true;
            current.Use();
        Label_01E2:
            if (Event.current.type == EventType.Repaint)
            {
                <ParamEqualizerCurveEditor>c__AnonStorey5C storeyc = new <ParamEqualizerCurveEditor>c__AnonStorey5C {
                    <>f__ref$91 = storeyb
                };
                float num12 = (float) MapNormalizedFrequency((double) centerFreq, (double) storeyb.plugin.GetSampleRate(), true, false);
                EditorGUI.DrawRect(new Rect((num12 * r.width) + r.x, r.y, 1f, r.height), (GUIUtility.hotControl != controlID) ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f));
                HandleUtility.ApplyWireMaterial();
                double num13 = 3.1415926;
                storeyc.wm = (-2.0 * num13) / ((double) storeyb.plugin.GetSampleRate());
                double a = ((2.0 * num13) * ((double) centerFreq)) / ((double) storeyb.plugin.GetSampleRate());
                double num15 = 1.0 / ((double) bandwidth);
                double num16 = (double) gain;
                double num17 = Math.Sin(a) / (2.0 * num15);
                storeyc.b0 = 1.0 + (num17 * num16);
                storeyc.b1 = -2.0 * Math.Cos(a);
                storeyc.b2 = 1.0 - (num17 * num16);
                storeyc.a0 = 1.0 + (num17 / num16);
                storeyc.a1 = -2.0 * Math.Cos(a);
                storeyc.a2 = 1.0 - (num17 / num16);
                AudioCurveRendering.DrawCurve(r, new AudioCurveRendering.AudioCurveEvaluator(storeyc.<>m__9D), ScaleAlpha(AudioCurveRendering.kAudioOrange, blend));
            }
            DrawFrequencyTickMarks(r, (float) storeyb.plugin.GetSampleRate(), true, new Color(1f, 1f, 1f, 0.3f * blend));
            AudioCurveRendering.EndCurveFrame();
            return flag;
        }

        protected static Color ScaleAlpha(Color col, float blend)
        {
            return new Color(col.r, col.g, col.b, col.a * blend);
        }

        public override string Description
        {
            get
            {
                return "Parametric equalizer";
            }
        }

        public override string Name
        {
            get
            {
                return "ParamEQ";
            }
        }

        public override string Vendor
        {
            get
            {
                return "Firelight Technologies";
            }
        }

        [CompilerGenerated]
        private sealed class <ParamEqualizerCurveEditor>c__AnonStorey5B
        {
            internal IAudioEffectPlugin plugin;
        }

        [CompilerGenerated]
        private sealed class <ParamEqualizerCurveEditor>c__AnonStorey5C
        {
            internal ParamEqGUI.<ParamEqualizerCurveEditor>c__AnonStorey5B <>f__ref$91;
            internal double a0;
            internal double a1;
            internal double a2;
            internal double b0;
            internal double b1;
            internal double b2;
            internal double wm;

            internal float <>m__9D(float x)
            {
                double num = ParamEqGUI.MapNormalizedFrequency((double) x, (double) this.<>f__ref$91.plugin.GetSampleRate(), true, true);
                ComplexD xd = ComplexD.Exp(this.wm * num);
                ComplexD xd2 = (ComplexD) ((xd * ((xd * this.b2) + this.b1)) + this.b0);
                ComplexD xd3 = (ComplexD) ((xd * ((xd * this.a2) + this.a1)) + this.a0);
                double num2 = Math.Log10((xd2 / xd3).Mag2());
                return (float) (0.5 * num2);
            }
        }
    }
}

