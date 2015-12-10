namespace UnityEditor
{
    using System;
    using System.Linq;
    using UnityEngine;

    internal static class ShadowCascadeSplitGUI
    {
        private static readonly Color[] kCascadeColors = new Color[] { new Color(0.5f, 0.5f, 0.6f, 1f), new Color(0.5f, 0.6f, 0.5f, 1f), new Color(0.6f, 0.6f, 0.5f, 1f), new Color(0.6f, 0.5f, 0.5f, 1f) };
        private const int kPartitionHandleExtraHitAreaWidth = 2;
        private const int kPartitionHandleWidth = 2;
        private const int kSliderbarBottomMargin = 2;
        private const int kSliderbarHeight = 0x18;
        private const int kSliderbarTopMargin = 2;
        private static readonly GUIStyle s_CascadeSliderBG = "LODSliderRange";
        private static readonly int s_CascadeSliderId;
        private static DragCache s_DragCache;
        private static DrawCameraMode s_OldSceneDrawMode;
        private static bool s_OldSceneLightingMode;
        private static SceneView s_RestoreSceneView;
        private static readonly GUIStyle s_TextCenteredStyle;

        static ShadowCascadeSplitGUI()
        {
            GUIStyle style = new GUIStyle(EditorStyles.whiteMiniLabel) {
                alignment = TextAnchor.MiddleCenter
            };
            s_TextCenteredStyle = style;
            s_CascadeSliderId = "s_CascadeSliderId".GetHashCode();
            s_OldSceneDrawMode = DrawCameraMode.Textured;
        }

        public static void HandleCascadeSliderGUI(ref float[] normalizedCascadePartitions)
        {
            GUILayout.Label("Cascade splits", new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(28f), GUILayout.ExpandWidth(true) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, s_CascadeSliderBG, options);
            GUI.Box(position, GUIContent.none);
            float x = position.x;
            float y = position.y + 2f;
            float num3 = position.width - (normalizedCascadePartitions.Length * 2);
            Color color = GUI.color;
            Color backgroundColor = GUI.backgroundColor;
            int index = -1;
            float[] destinationArray = new float[normalizedCascadePartitions.Length + 1];
            Array.Copy(normalizedCascadePartitions, destinationArray, normalizedCascadePartitions.Length);
            destinationArray[destinationArray.Length - 1] = 1f - normalizedCascadePartitions.Sum();
            int controlID = GUIUtility.GetControlID(s_CascadeSliderId, FocusType.Passive);
            Event current = Event.current;
            int activePartition = -1;
            for (int i = 0; i < destinationArray.Length; i++)
            {
                float num8 = destinationArray[i];
                index = (index + 1) % kCascadeColors.Length;
                GUI.backgroundColor = kCascadeColors[index];
                float width = num3 * num8;
                Rect rect2 = new Rect(x, y, width, 24f);
                GUI.Box(rect2, GUIContent.none, s_CascadeSliderBG);
                x += width;
                GUI.color = Color.white;
                Rect rect3 = rect2;
                string t = string.Format("{0}\n{1:F1}%", i, num8 * 100f);
                GUI.Label(rect3, GUIContent.Temp(t, t), s_TextCenteredStyle);
                if (i == (destinationArray.Length - 1))
                {
                    break;
                }
                GUI.backgroundColor = Color.black;
                Rect rect4 = rect2;
                rect4.x = x;
                rect4.width = 2f;
                GUI.Box(rect4, GUIContent.none, s_CascadeSliderBG);
                Rect rect5 = rect4;
                rect5.xMin -= 2f;
                rect5.xMax += 2f;
                if (rect5.Contains(current.mousePosition))
                {
                    activePartition = i;
                }
                if (s_DragCache == null)
                {
                    EditorGUIUtility.AddCursorRect(rect5, MouseCursor.ResizeHorizontal, controlID);
                }
                x += 2f;
            }
            GUI.color = color;
            GUI.backgroundColor = backgroundColor;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (activePartition >= 0)
                    {
                        s_DragCache = new DragCache(activePartition, normalizedCascadePartitions[activePartition], current.mousePosition);
                        if (GUIUtility.hotControl == 0)
                        {
                            GUIUtility.hotControl = controlID;
                        }
                        current.Use();
                        if (s_RestoreSceneView == null)
                        {
                            s_RestoreSceneView = SceneView.lastActiveSceneView;
                            if (s_RestoreSceneView != null)
                            {
                                s_OldSceneDrawMode = s_RestoreSceneView.renderMode;
                                s_OldSceneLightingMode = s_RestoreSceneView.m_SceneLighting;
                                s_RestoreSceneView.renderMode = DrawCameraMode.ShadowCascades;
                            }
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    s_DragCache = null;
                    if (s_RestoreSceneView != null)
                    {
                        s_RestoreSceneView.renderMode = s_OldSceneDrawMode;
                        s_RestoreSceneView.m_SceneLighting = s_OldSceneLightingMode;
                        s_RestoreSceneView = null;
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector = current.mousePosition - s_DragCache.m_LastCachedMousePosition;
                        float num10 = vector.x / num3;
                        bool flag = (destinationArray[s_DragCache.m_ActivePartition] + num10) > 0f;
                        bool flag2 = (destinationArray[s_DragCache.m_ActivePartition + 1] - num10) > 0f;
                        if (flag && flag2)
                        {
                            s_DragCache.m_NormalizedPartitionSize += num10;
                            normalizedCascadePartitions[s_DragCache.m_ActivePartition] = s_DragCache.m_NormalizedPartitionSize;
                            if (s_DragCache.m_ActivePartition < (normalizedCascadePartitions.Length - 1))
                            {
                                normalizedCascadePartitions[s_DragCache.m_ActivePartition + 1] -= num10;
                            }
                        }
                        s_DragCache.m_LastCachedMousePosition = current.mousePosition;
                        current.Use();
                        break;
                    }
                    break;
            }
        }

        private class DragCache
        {
            public int m_ActivePartition;
            public Vector2 m_LastCachedMousePosition;
            public float m_NormalizedPartitionSize;

            public DragCache(int activePartition, float normalizedPartitionSize, Vector2 currentMousePos)
            {
                this.m_ActivePartition = activePartition;
                this.m_NormalizedPartitionSize = normalizedPartitionSize;
                this.m_LastCachedMousePosition = currentMousePos;
            }
        }
    }
}

