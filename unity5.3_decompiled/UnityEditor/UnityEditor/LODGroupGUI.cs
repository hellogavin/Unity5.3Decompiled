namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class LODGroupGUI
    {
        public const int kButtonPadding = 2;
        public static readonly Color kCulledLODColor = new Color(0.4f, 0f, 0f, 1f);
        public const int kDeleteButtonSize = 20;
        public static readonly Color[] kLODColors = new Color[] { new Color(0.4831376f, 0.6211768f, 0.0219608f, 1f), new Color(0.279216f, 0.4078432f, 0.5835296f, 1f), new Color(0.2070592f, 0.5333336f, 0.6556864f, 1f), new Color(0.5333336f, 0.16f, 0.0282352f, 1f), new Color(0.3827448f, 0.2886272f, 0.5239216f, 1f), new Color(0.8f, 0.4423528f, 0f, 1f), new Color(0.4486272f, 0.4078432f, 0.050196f, 1f), new Color(0.7749016f, 0.6368624f, 0.0250984f, 1f) };
        public const int kRenderAreaForegroundPadding = 3;
        public const int kRenderersButtonHeight = 60;
        public const int kSceneHeaderOffset = 40;
        public const int kSceneLabelHalfWidth = 100;
        public const int kSceneLabelHeight = 0x2d;
        public const int kSelectedLODRangePadding = 3;
        public const int kSliderBarBottomMargin = 0x10;
        public const int kSliderBarHeight = 30;
        public const int kSliderBarTopMargin = 0x12;
        private static GUIStyles s_Styles;

        public static Rect CalcLODButton(Rect totalRect, float percentage)
        {
            return new Rect((totalRect.x + Mathf.Round(totalRect.width * (1f - percentage))) - 5f, totalRect.y, 10f, totalRect.height);
        }

        private static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent)
        {
            float num = Mathf.Round(totalRect.width * (1f - startPercent));
            float num2 = Mathf.Round(totalRect.width * (1f - endPercent));
            return new Rect(totalRect.x + num, totalRect.y, num2 - num, totalRect.height);
        }

        public static List<LODInfo> CreateLODInfos(int numLODs, Rect area, Func<int, string> nameGen, Func<int, float> heightGen)
        {
            List<LODInfo> list = new List<LODInfo>();
            for (int i = 0; i < numLODs; i++)
            {
                LODInfo info;
                info = new LODInfo(i, nameGen(i), heightGen(i)) {
                    m_ButtonPosition = CalcLODButton(area, info.ScreenPercent)
                };
                float startPercent = (i != 0) ? list[i - 1].ScreenPercent : 1f;
                info.m_RangePosition = CalcLODRange(area, startPercent, info.ScreenPercent);
                list.Add(info);
            }
            return list;
        }

        public static float DelinearizeScreenPercentage(float percentage)
        {
            if (Mathf.Approximately(0f, percentage))
            {
                return 0f;
            }
            return Mathf.Sqrt(percentage);
        }

        private static void DrawCulledRange(Rect totalRect, float previousLODPercentage)
        {
            if (!Mathf.Approximately(previousLODPercentage, 0f))
            {
                Rect culledBox = GetCulledBox(totalRect, DelinearizeScreenPercentage(previousLODPercentage));
                Color color = GUI.color;
                GUI.color = kCulledLODColor;
                Styles.m_LODSliderRange.Draw(culledBox, GUIContent.none, false, false, false, false);
                GUI.color = color;
                string text = string.Format("Culled\n{0:0}%", previousLODPercentage * 100f);
                Styles.m_LODSliderText.Draw(culledBox, text, false, false, false, false);
            }
        }

        private static void DrawLODButton(LODInfo currentLOD)
        {
            EditorGUIUtility.AddCursorRect(currentLOD.m_ButtonPosition, MouseCursor.ResizeHorizontal);
        }

        private static void DrawLODRange(LODInfo currentLOD, float previousLODPercentage, bool isSelected)
        {
            Color backgroundColor = GUI.backgroundColor;
            string text = string.Format("{0}\n{1:0}%", currentLOD.LODName, previousLODPercentage * 100f);
            if (isSelected)
            {
                Rect rangePosition = currentLOD.m_RangePosition;
                rangePosition.width -= 6f;
                rangePosition.height -= 6f;
                rangePosition.center += new Vector2(3f, 3f);
                Styles.m_LODSliderRangeSelected.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
                GUI.backgroundColor = kLODColors[currentLOD.LODLevel];
                if (rangePosition.width > 0f)
                {
                    Styles.m_LODSliderRange.Draw(rangePosition, GUIContent.none, false, false, false, false);
                }
                Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
            }
            else
            {
                GUI.backgroundColor = kLODColors[currentLOD.LODLevel];
                GUI.backgroundColor = (Color) (GUI.backgroundColor * 0.6f);
                Styles.m_LODSliderRange.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
                Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
            }
            GUI.backgroundColor = backgroundColor;
        }

        public static void DrawLODSlider(Rect area, IList<LODInfo> lods, int selectedLevel)
        {
            Styles.m_LODSliderBG.Draw(area, GUIContent.none, false, false, false, false);
            for (int i = 0; i < lods.Count; i++)
            {
                LODInfo currentLOD = lods[i];
                DrawLODRange(currentLOD, (i != 0) ? lods[i - 1].RawScreenPercent : 1f, i == selectedLevel);
                DrawLODButton(currentLOD);
            }
            DrawCulledRange(area, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].RawScreenPercent);
        }

        public static void DrawMixedValueLODSlider(Rect area)
        {
            Styles.m_LODSliderBG.Draw(area, GUIContent.none, false, false, false, false);
            Rect culledBox = GetCulledBox(area, 1f);
            Color color = GUI.color;
            GUI.color = (Color) (kLODColors[1] * 0.6f);
            Styles.m_LODSliderRange.Draw(culledBox, GUIContent.none, false, false, false, false);
            GUI.color = color;
            GUIStyle style = new GUIStyle(EditorStyles.whiteLargeLabel) {
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(area, "---", style);
        }

        public static Rect GetCulledBox(Rect totalRect, float previousLODPercentage)
        {
            Rect rect = CalcLODRange(totalRect, previousLODPercentage, 0f);
            rect.height -= 2f;
            rect.width--;
            rect.center += new Vector2(0f, 1f);
            return rect;
        }

        public static float LinearizeScreenPercentage(float percentage)
        {
            return (percentage * percentage);
        }

        public static void SetSelectedLODLevelPercentage(float newScreenPercentage, int lod, List<LODInfo> lods)
        {
            <SetSelectedLODLevelPercentage>c__AnonStorey8E storeye = new <SetSelectedLODLevelPercentage>c__AnonStorey8E {
                lods = lods,
                lod = lod
            };
            float screenPercent = 0f;
            LODInfo info = storeye.lods.FirstOrDefault<LODInfo>(new Func<LODInfo, bool>(storeye.<>m__173));
            if (info != null)
            {
                screenPercent = info.ScreenPercent;
            }
            float num2 = 1f;
            LODInfo info2 = storeye.lods.FirstOrDefault<LODInfo>(new Func<LODInfo, bool>(storeye.<>m__174));
            if (info2 != null)
            {
                num2 = info2.ScreenPercent;
            }
            num2 = Mathf.Clamp01(num2);
            screenPercent = Mathf.Clamp01(screenPercent);
            storeye.lods[storeye.lod].ScreenPercent = Mathf.Clamp(newScreenPercentage, screenPercent, num2);
        }

        public static GUIStyles Styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new GUIStyles();
                }
                return s_Styles;
            }
        }

        [CompilerGenerated]
        private sealed class <SetSelectedLODLevelPercentage>c__AnonStorey8E
        {
            internal int lod;
            internal List<LODGroupGUI.LODInfo> lods;

            internal bool <>m__173(LODGroupGUI.LODInfo x)
            {
                return (x.LODLevel == (this.lods[this.lod].LODLevel + 1));
            }

            internal bool <>m__174(LODGroupGUI.LODInfo x)
            {
                return (x.LODLevel == (this.lods[this.lod].LODLevel - 1));
            }
        }

        public class GUIStyles
        {
            public readonly GUIContent m_AnimateBetweenPreviousLOD = EditorGUIUtility.TextContent("Animate Between Previous LOD|Cross-fade animation plays when transits between this LOD and the previous (lower) LOD.");
            public readonly GUIContent m_AnimatedCrossFadeInconsistentText = EditorGUIUtility.TextContent("Animated cross-fading is currently disabled. \"Animate Between Next LOD\" is enabled but the next LOD is not in Animated Cross Fade mode.");
            public readonly GUIContent m_AnimatedCrossFadeInvalidText = EditorGUIUtility.TextContent("Animated cross-fading is currently disabled. Please enable \"Animate Between Next LOD\" on either the current or the previous LOD.");
            public readonly GUIContent m_CameraIcon = EditorGUIUtility.IconContent("Camera Icon");
            public readonly GUIContent m_IconRendererMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove Renderer");
            public readonly GUIContent m_IconRendererPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add New Renderers");
            public readonly GUIContent m_LightmapScale = EditorGUIUtility.TextContent("Lightmap Scale|Set the lightmap scale to match the LOD percentages.");
            public readonly GUIStyle m_LODBlackBox = "LODBlackBox";
            public readonly GUIStyle m_LODCameraLine = "LODCameraLine";
            public readonly GUIStyle m_LODLevelNotifyText = "LODLevelNotifyText";
            public readonly GUIStyle m_LODRendererAddButton = "LODRendererAddButton";
            public readonly GUIStyle m_LODRendererButton = "LODRendererButton";
            public readonly GUIStyle m_LODRendererRemove = "LODRendererRemove";
            public readonly GUIStyle m_LODRenderersText = "LODRenderersText";
            public readonly GUIStyle m_LODSceneText = "LODSceneText";
            public readonly GUIStyle m_LODSliderBG = "LODSliderBG";
            public readonly GUIStyle m_LODSliderRange = "LODSliderRange";
            public readonly GUIStyle m_LODSliderRangeSelected = "LODSliderRangeSelected";
            public readonly GUIStyle m_LODSliderText = "LODSliderText";
            public readonly GUIStyle m_LODSliderTextSelected = "LODSliderTextSelected";
            public readonly GUIStyle m_LODStandardButton = "Button";
            public readonly GUIContent m_RecalculateBounds = EditorGUIUtility.TextContent("Bounds|Recalculate bounds for the current LOD group.");
            public readonly GUIContent m_RendersTitle = EditorGUIUtility.TextContent("Renderers:");
            public readonly GUIContent m_UploadToImporter = EditorGUIUtility.TextContent("Upload to Importer|Upload the modified screen percentages to the model importer.");
            public readonly GUIContent m_UploadToImporterDisabled = EditorGUIUtility.TextContent("Upload to Importer|Number of LOD's in the scene instance differ from the number of LOD's in the imported model.");
        }

        public class LODInfo
        {
            public Rect m_ButtonPosition;
            public Rect m_RangePosition;

            public LODInfo(int lodLevel, string name, float screenPercentage)
            {
                this.LODLevel = lodLevel;
                this.LODName = name;
                this.RawScreenPercent = screenPercentage;
            }

            public int LODLevel { get; private set; }

            public string LODName { get; private set; }

            public float RawScreenPercent { get; set; }

            public float ScreenPercent
            {
                get
                {
                    return LODGroupGUI.DelinearizeScreenPercentage(this.RawScreenPercent);
                }
                set
                {
                    this.RawScreenPercent = LODGroupGUI.LinearizeScreenPercentage(value);
                }
            }
        }
    }
}

