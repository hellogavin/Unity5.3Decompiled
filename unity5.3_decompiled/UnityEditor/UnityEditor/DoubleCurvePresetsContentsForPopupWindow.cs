namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class DoubleCurvePresetsContentsForPopupWindow : PopupWindowContent
    {
        private PresetLibraryEditor<DoubleCurvePresetLibrary> m_CurveLibraryEditor;
        private PresetLibraryEditorState m_CurveLibraryEditorState;
        private DoubleCurve m_DoubleCurve;
        private Action<DoubleCurve> m_PresetSelectedCallback;
        private bool m_WantsToClose;

        public DoubleCurvePresetsContentsForPopupWindow(DoubleCurve doubleCurveToSave, Action<DoubleCurve> presetSelectedCallback)
        {
            this.m_DoubleCurve = doubleCurveToSave;
            this.m_PresetSelectedCallback = presetSelectedCallback;
        }

        private void AddDefaultPresetsToLibrary(PresetLibrary presetLibrary)
        {
            DoubleCurvePresetLibrary library = presetLibrary as DoubleCurvePresetLibrary;
            if (library == null)
            {
                Debug.Log("Incorrect preset library, should be a DoubleCurvePresetLibrary but was a " + presetLibrary.GetType());
            }
            else
            {
                bool signedRange = this.m_DoubleCurve.signedRange;
                List<DoubleCurve> unsignedSingleCurveDefaults = new List<DoubleCurve>();
                if (this.IsSingleCurve(this.m_DoubleCurve))
                {
                    unsignedSingleCurveDefaults = GetUnsignedSingleCurveDefaults(signedRange);
                }
                else if (signedRange)
                {
                    unsignedSingleCurveDefaults = GetSignedDoubleCurveDefaults();
                }
                else
                {
                    unsignedSingleCurveDefaults = GetUnsignedDoubleCurveDefaults();
                }
                foreach (DoubleCurve curve in unsignedSingleCurveDefaults)
                {
                    library.Add(curve, string.Empty);
                }
            }
        }

        private string GetEditorPrefBaseName()
        {
            return PresetLibraryLocations.GetParticleCurveLibraryExtension(this.m_DoubleCurve.IsSingleCurve(), this.m_DoubleCurve.signedRange);
        }

        public PresetLibraryEditor<DoubleCurvePresetLibrary> GetPresetLibraryEditor()
        {
            return this.m_CurveLibraryEditor;
        }

        private static List<DoubleCurve> GetSignedDoubleCurveDefaults()
        {
            return new List<DoubleCurve> { new DoubleCurve(new AnimationCurve(CurveEditorWindow.GetConstantKeys(-1f)), new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), 1) };
        }

        private static List<DoubleCurve> GetUnsignedDoubleCurveDefaults()
        {
            return new List<DoubleCurve> { new DoubleCurve(new AnimationCurve(CurveEditorWindow.GetConstantKeys(0f)), new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), 0) };
        }

        private static List<DoubleCurve> GetUnsignedSingleCurveDefaults(bool signedRange)
        {
            return new List<DoubleCurve> { new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetLinearKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetLinearMirrorKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInMirrorKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseOutKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseOutMirrorKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInOutKeys()), signedRange), new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInOutMirrorKeys()), signedRange) };
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(240f, 330f);
        }

        public void InitIfNeeded()
        {
            if (this.m_CurveLibraryEditorState == null)
            {
                this.m_CurveLibraryEditorState = new PresetLibraryEditorState(this.GetEditorPrefBaseName());
                this.m_CurveLibraryEditorState.TransferEditorPrefsState(true);
            }
            if (this.m_CurveLibraryEditor == null)
            {
                ScriptableObjectSaveLoadHelper<DoubleCurvePresetLibrary> helper = new ScriptableObjectSaveLoadHelper<DoubleCurvePresetLibrary>(PresetLibraryLocations.GetParticleCurveLibraryExtension(this.m_DoubleCurve.IsSingleCurve(), this.m_DoubleCurve.signedRange), SaveType.Text);
                this.m_CurveLibraryEditor = new PresetLibraryEditor<DoubleCurvePresetLibrary>(helper, this.m_CurveLibraryEditorState, new Action<int, object>(this.ItemClickedCallback));
                this.m_CurveLibraryEditor.addDefaultPresets = (Action<PresetLibrary>) Delegate.Combine(this.m_CurveLibraryEditor.addDefaultPresets, new Action<PresetLibrary>(this.AddDefaultPresetsToLibrary));
                this.m_CurveLibraryEditor.presetsWasReordered = new Action(this.PresetsWasReordered);
                this.m_CurveLibraryEditor.previewAspect = 4f;
                this.m_CurveLibraryEditor.minMaxPreviewHeight = new Vector2(24f, 24f);
                this.m_CurveLibraryEditor.showHeader = true;
            }
        }

        private bool IsSingleCurve(DoubleCurve doubleCurve)
        {
            return ((doubleCurve.minCurve == null) || (doubleCurve.minCurve.length == 0));
        }

        private void ItemClickedCallback(int clickCount, object presetObject)
        {
            DoubleCurve curve = presetObject as DoubleCurve;
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
            this.m_CurveLibraryEditor.OnGUI(rect, this.m_DoubleCurve);
            if (this.m_WantsToClose)
            {
                base.editorWindow.Close();
            }
        }

        private void PresetsWasReordered()
        {
            InspectorWindow.RepaintAllInspectors();
        }

        public DoubleCurve doubleCurveToSave
        {
            get
            {
                return this.m_DoubleCurve;
            }
            set
            {
                this.m_DoubleCurve = value;
            }
        }
    }
}

