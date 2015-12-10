namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AudioCurveRendering
    {
        public static readonly Color kAudioOrange = new Color(1f, 0.6588235f, 0.02745098f);
        private static Vector3[] s_PointCache;

        public static Rect BeginCurveFrame(Rect r)
        {
            DrawCurveBackground(r);
            r = DrawCurveFrame(r);
            GUI.BeginGroup(r);
            return new Rect(0f, 0f, r.width, r.height);
        }

        public static void DrawCurve(Rect r, AudioCurveEvaluator eval, Color curveColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                int width = (int) r.width;
                float num2 = r.height * 0.5f;
                float num3 = 1f / ((float) (width - 1));
                Vector3[] pointCache = GetPointCache(width);
                for (int i = 0; i < width; i++)
                {
                    pointCache[i].x = i + r.x;
                    pointCache[i].y = (num2 - (num2 * eval(i * num3))) + r.y;
                    pointCache[i].z = 0f;
                }
                GUI.BeginClip(r);
                Handles.color = curveColor;
                Handles.DrawAAPolyLine(3f, width, pointCache);
                GUI.EndClip();
            }
        }

        public static void DrawCurveBackground(Rect r)
        {
            EditorGUI.DrawRect(r, new Color(0.3f, 0.3f, 0.3f));
        }

        public static Rect DrawCurveFrame(Rect r)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.colorPickerBox.Draw(r, false, false, false, false);
                r.x++;
                r.y++;
                r.width -= 2f;
                r.height -= 2f;
            }
            return r;
        }

        public static void DrawFilledCurve(Rect r, AudioCurveAndColorEvaluator eval)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
                float num2 = 1f / pixelsPerPoint;
                float num3 = r.width * pixelsPerPoint;
                float num4 = 1f / (num3 - 1f);
                float max = r.height * 0.5f;
                float num6 = r.y + (0.5f * r.height);
                float y = r.y + r.height;
                float num8 = Mathf.Clamp(max * eval(0f, out color), -max, max);
                for (int i = 0; i < num3; i++)
                {
                    float x = Mathf.Floor(r.x) + (i * num2);
                    float num11 = Mathf.Clamp(max * eval(i * num4, out color), -max, max);
                    float num12 = ((num11 >= num8) ? num8 : num11) - (0.5f * num2);
                    float num13 = ((num11 <= num8) ? num8 : num11) + (0.5f * num2);
                    GL.Color(new Color(color.r, color.g, color.b, 0f));
                    AudioMixerDrawUtils.Vertex(x, num6 - num13);
                    GL.Color(color);
                    AudioMixerDrawUtils.Vertex(x, num6 - num12);
                    AudioMixerDrawUtils.Vertex(x, num6 - num12);
                    AudioMixerDrawUtils.Vertex(x, y);
                    num8 = num11;
                }
                GL.End();
            }
        }

        public static void DrawFilledCurve(Rect r, AudioCurveEvaluator eval, Color curveColor)
        {
            <DrawFilledCurve>c__AnonStorey58 storey = new <DrawFilledCurve>c__AnonStorey58 {
                curveColor = curveColor,
                eval = eval
            };
            DrawFilledCurve(r, new AudioCurveAndColorEvaluator(storey.<>m__9A));
        }

        public static void DrawGradientRect(Rect r, Color c1, Color c2, float blend, bool horizontal)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                GL.Begin(7);
                if (horizontal)
                {
                    GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
                    GL.Vertex3(r.x, r.y, 0f);
                    GL.Vertex3(r.x + r.width, r.y, 0f);
                    GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
                    GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
                    GL.Vertex3(r.x, r.y + r.height, 0f);
                }
                else
                {
                    GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
                    GL.Vertex3(r.x, r.y + r.height, 0f);
                    GL.Vertex3(r.x, r.y, 0f);
                    GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
                    GL.Vertex3(r.x + r.width, r.y, 0f);
                    GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
                }
                GL.End();
            }
        }

        public static void DrawSymmetricFilledCurve(Rect r, AudioCurveAndColorEvaluator eval)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
                float num2 = 1f / pixelsPerPoint;
                float num3 = r.width * pixelsPerPoint;
                float num4 = 1f / (num3 - 1f);
                float max = r.height * 0.5f;
                float num6 = r.y + (0.5f * r.height);
                float num7 = Mathf.Clamp(max * eval(0.0001f, out color), 0f, max);
                for (int i = 0; i < num3; i++)
                {
                    float x = Mathf.Floor(r.x) + (i * num2);
                    float num10 = Mathf.Clamp(max * eval(i * num4, out color), 0f, max);
                    float num11 = (num10 >= num7) ? num7 : num10;
                    float num12 = (num10 <= num7) ? num7 : num10;
                    GL.Color(new Color(color.r, color.g, color.b, 0f));
                    AudioMixerDrawUtils.Vertex(x, num6 + num12);
                    GL.Color(color);
                    AudioMixerDrawUtils.Vertex(x, num6 + num11);
                    AudioMixerDrawUtils.Vertex(x, num6 + num11);
                    AudioMixerDrawUtils.Vertex(x, num6 - num11);
                    AudioMixerDrawUtils.Vertex(x, num6 - num11);
                    GL.Color(new Color(color.r, color.g, color.b, 0f));
                    AudioMixerDrawUtils.Vertex(x, num6 - num12);
                    num7 = num10;
                }
                GL.End();
            }
        }

        public static void EndCurveFrame()
        {
            GUI.EndGroup();
        }

        private static Vector3[] GetPointCache(int numPoints)
        {
            if ((s_PointCache == null) || (s_PointCache.Length != numPoints))
            {
                s_PointCache = new Vector3[numPoints];
            }
            return s_PointCache;
        }

        [CompilerGenerated]
        private sealed class <DrawFilledCurve>c__AnonStorey58
        {
            internal Color curveColor;
            internal AudioCurveRendering.AudioCurveEvaluator eval;

            internal float <>m__9A(float x, out Color color)
            {
                color = this.curveColor;
                return this.eval(x);
            }
        }

        public delegate float AudioCurveAndColorEvaluator(float x, out Color col);

        public delegate float AudioCurveEvaluator(float x);
    }
}

