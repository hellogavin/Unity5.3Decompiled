namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class CurvePresetsContentsForPopupWindow : PopupWindowContent
    {
        private AnimationCurve m_Curve;
        private PresetLibraryEditor<CurvePresetLibrary> m_CurveLibraryEditor;
        private PresetLibraryEditorState m_CurveLibraryEditorState;
        private CurveLibraryType m_CurveLibraryType;
        private Action<AnimationCurve> m_PresetSelectedCallback;
        private bool m_WantsToClose;

        public CurvePresetsContentsForPopupWindow(AnimationCurve animCurve, CurveLibraryType curveLibraryType, Action<AnimationCurve> presetSelectedCallback)
        {
            this.m_CurveLibraryType = curveLibraryType;
            this.m_Curve = animCurve;
            this.m_PresetSelectedCallback = presetSelectedCallback;
        }

        private void AddDefaultPresetsToLibrary(PresetLibrary presetLibrary)
        {
            CurvePresetLibrary library = presetLibrary as CurvePresetLibrary;
            if (library == null)
            {
                Debug.Log("Incorrect preset library, should be a CurvePresetLibrary but was a " + presetLibrary.GetType());
            }
            else
            {
                List<AnimationCurve> list = new List<AnimationCurve> {
                    new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)),
                    new AnimationCurve(CurveEditorWindow.GetLinearKeys()),
                    new AnimationCurve(CurveEditorWindow.GetEaseInKeys()),
                    new AnimationCurve(CurveEditorWindow.GetEaseOutKeys()),
                    new AnimationCurve(CurveEditorWindow.GetEaseInOutKeys())
                };
                foreach (AnimationCurve curve in list)
                {
                    library.Add(curve, string.Empty);
                }
            }
        }

        public static string GetBasePrefText(CurveLibraryType curveLibraryType)
        {
            return GetExtension(curveLibraryType);
        }

        private static string GetExtension(CurveLibraryType curveLibraryType)
        {
            switch (curveLibraryType)
            {
                case CurveLibraryType.Unbounded:
                    return PresetLibraryLocations.GetCurveLibraryExtension(false);

                case CurveLibraryType.NormalizedZeroToOne:
                    return PresetLibraryLocations.GetCurveLibraryExtension(true);
            }
            Debug.LogError("Enum not handled!");
            return "curves";
        }

        public PresetLibraryEditor<CurvePresetLibrary> GetPresetLibraryEditor()
        {
            return this.m_CurveLibraryEditor;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(240f, 330f);
        }

        public void InitIfNeeded()
        {
            if (this.m_CurveLibraryEditorState == null)
            {
                this.m_CurveLibraryEditorState = new PresetLibraryEditorState(GetBasePrefText(this.m_CurveLibraryType));
                this.m_CurveLibraryEditorState.TransferEditorPrefsState(true);
            }
            if (this.m_CurveLibraryEditor == null)
            {
                ScriptableObjectSaveLoadHelper<CurvePresetLibrary> helper = new ScriptableObjectSaveLoadHelper<CurvePresetLibrary>(GetExtension(this.m_CurveLibraryType), SaveType.Text);
                this.m_CurveLibraryEditor = new PresetLibraryEditor<CurvePresetLibrary>(helper, this.m_CurveLibraryEditorState, new Action<int, object>(this.ItemClickedCallback));
                this.m_CurveLibraryEditor.addDefaultPresets = (Action<PresetLibrary>) Delegate.Combine(this.m_CurveLibraryEditor.addDefaultPresets, new Action<PresetLibrary>(this.AddDefaultPresetsToLibrary));
                this.m_CurveLibraryEditor.presetsWasReordered = (Action) Delegate.Combine(this.m_CurveLibraryEditor.presetsWasReordered, new Action(this.OnPresetsWasReordered));
                this.m_CurveLibraryEditor.previewAspect = 4f;
                this.m_CurveLibraryEditor.minMaxPreviewHeight = new Vector2(24f, 24f);
                this.m_CurveLibraryEditor.showHeader = true;
            }
        }

        private void ItemClickedCallback(int clickCount, object presetObject)
        {
            AnimationCurve curve = presetObject as AnimationCurve;
            if (curve == null)
            {
                Debug.LogError("Incorrect object passed " + presetObject);
            }
            this.m_PresetSelectedCallback(curve);
        }

        public override void OnClose()
        {
            this.m_CurveLibraryEditorState.TransferEditorPrefsState(false);
        }

        public override void OnGUI(Rect rect)
        {
            this.InitIfNeeded();
            this.m_CurveLibraryEditor.OnGUI(rect, this.m_Curve);
            if (this.m_WantsToClose)
            {
                base.editorWindow.Close();
            }
        }

        private void OnPresetsWasReordered()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        public string currentPresetLibrary
        {
            get
            {
                this.InitIfNeeded();
                return this.m_CurveLibraryEditor.currentLibraryWithoutExtension;
            }
            set
            {
                this.InitIfNeeded();
                this.m_CurveLibraryEditor.currentLibraryWithoutExtension = value;
            }
        }

        public AnimationCurve curveToSaveAsPreset
        {
            get
            {
                return this.m_Curve;
            }
            set
            {
                this.m_Curve = value;
            }
        }
    }
}

