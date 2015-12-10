namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(ColorPresetLibrary))]
    internal class ColorPresetLibraryEditor : Editor
    {
        private GenericPresetLibraryInspector<ColorPresetLibrary> m_GenericPresetLibraryInspector;

        public void OnDestroy()
        {
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnDestroy();
            }
        }

        private void OnEditButtonClicked(string libraryPath)
        {
            ColorPicker.Show(GUIView.current, Color.white);
            ColorPicker.get.currentPresetLibrary = libraryPath;
        }

        public void OnEnable()
        {
            this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<ColorPresetLibrary>(this.target, "Color Preset Library", new Action<string>(this.OnEditButtonClicked));
            this.m_GenericPresetLibraryInspector.useOnePixelOverlappedGrid = true;
            this.m_GenericPresetLibraryInspector.maxShowNumPresets = 0x7d0;
        }

        public override void OnInspectorGUI()
        {
            this.m_GenericPresetLibraryInspector.itemViewMode = PresetLibraryEditorState.GetItemViewMode(ColorPicker.presetsEditorPrefID);
            if (this.m_GenericPresetLibraryInspector != null)
            {
                this.m_GenericPresetLibraryInspector.OnInspectorGUI();
            }
        }
    }
}

