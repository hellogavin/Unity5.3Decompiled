namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEngine;

    [CustomEditor(typeof(CurvePresetLibrary))]
    internal class CurvePresetLibraryEditor : Editor
    {
        private CurveLibraryType m_CurveLibraryType;
        private GenericPresetLibraryInspector<CurvePresetLibrary> m_GenericPresetLibraryInspector;

        private CurveLibraryType GetCurveLibraryTypeFromExtension(string extension)
        {
            string curveLibraryExtension = PresetLibraryLocations.GetCurveLibraryExtension(true);
            string str2 = PresetLibraryLocations.GetCurveLibraryExtension(false);
            if (!extension.Equals(curveLibraryExtension, StringComparison.OrdinalIgnoreCase))
            {
                if (extension.Equals(str2, StringComparison.OrdinalIgnoreCase))
                {
                    return CurveLibraryType.Unbounded;
                }
                Debug.LogError("Extension not recognized!");
            }
            return CurveLibraryType.NormalizedZeroToOne;
        }

        private Rect GetCurveRanges()
        {
            CurveLibraryType curveLibraryType = this.m_CurveLibraryType;
            if (curveLibraryType == CurveLibraryType.Unbounded)
            {
                return new Rect();
            }
            if (curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
            {
                return new Rect();
            }
            return new Rect(0f, 0f, 1f, 1f);
        }

        private string GetHeader()
        {
            CurveLibraryType curveLibraryType = this.m_CurveLibraryType;
            if (curveLibraryType == CurveLibraryType.Unbounded)
            {
                return "Curve Preset Library";
            }
            if (curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
            {
                return "Curve Preset Library ?";
            }
            return "Curve Preset Library (Normalized 0 - 1)";
        }

        public void OnDestroy()
        {
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnDestroy();
            }
        }

        private void OnEditButtonClicked(string libraryPath)
        {
            Rect curveRanges = this.GetCurveRanges();
            CurveEditorSettings settings = new CurveEditorSettings();
            if (((curveRanges.width > 0f) && (curveRanges.height > 0f)) && ((curveRanges.width != float.PositiveInfinity) && (curveRanges.height != float.PositiveInfinity)))
            {
                settings.hRangeMin = curveRanges.xMin;
                settings.hRangeMax = curveRanges.xMax;
                settings.vRangeMin = curveRanges.yMin;
                settings.vRangeMax = curveRanges.yMax;
            }
            CurveEditorWindow.curve = new AnimationCurve();
            CurveEditorWindow.color = new Color(0f, 0.8f, 0f);
            CurveEditorWindow.instance.Show(GUIView.current, settings);
            CurveEditorWindow.instance.currentPresetLibrary = libraryPath;
        }

        public void OnEnable()
        {
            string assetPath = AssetDatabase.GetAssetPath(this.target.GetInstanceID());
            char[] trimChars = new char[] { '.' };
            this.m_CurveLibraryType = this.GetCurveLibraryTypeFromExtension(Path.GetExtension(assetPath).TrimStart(trimChars));
            this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<CurvePresetLibrary>(this.target, this.GetHeader(), new Action<string>(this.OnEditButtonClicked));
            this.m_GenericPresetLibraryInspector.presetSize = new Vector2(72f, 20f);
            this.m_GenericPresetLibraryInspector.lineSpacing = 5f;
        }

        public override void OnInspectorGUI()
        {
            string basePrefText = CurvePresetsContentsForPopupWindow.GetBasePrefText(this.m_CurveLibraryType);
            this.m_GenericPresetLibraryInspector.itemViewMode = PresetLibraryEditorState.GetItemViewMode(basePrefText);
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnInspectorGUI();
            }
        }
    }
}

