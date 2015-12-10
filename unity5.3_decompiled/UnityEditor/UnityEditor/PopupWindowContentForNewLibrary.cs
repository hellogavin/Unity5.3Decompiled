namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PopupWindowContentForNewLibrary : PopupWindowContent
    {
        private Func<string, PresetFileLocation, string> m_CreateLibraryCallback;
        private string m_ErrorString;
        private string m_NewLibraryName = string.Empty;
        private int m_SelectedIndexInPopup;
        private Rect m_WantedSize;
        private static Texts s_Texts;

        public PopupWindowContentForNewLibrary(Func<string, PresetFileLocation, string> createLibraryCallback)
        {
            this.m_CreateLibraryCallback = createLibraryCallback;
        }

        private void CreateLibraryAndCloseWindow(EditorWindow editorWindow)
        {
            PresetFileLocation location = s_Texts.fileLocationOrder[this.m_SelectedIndexInPopup];
            this.m_ErrorString = this.m_CreateLibraryCallback(this.m_NewLibraryName, location);
            if (string.IsNullOrEmpty(this.m_ErrorString))
            {
                editorWindow.Close();
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(350f, (this.m_WantedSize.height <= 0f) ? 90f : this.m_WantedSize.height);
        }

        private void KeyboardHandling(EditorWindow editorWindow)
        {
            Event current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                KeyCode keyCode = current.keyCode;
                if (keyCode != KeyCode.Return)
                {
                    if (keyCode == KeyCode.Escape)
                    {
                        editorWindow.Close();
                        return;
                    }
                    if (keyCode != KeyCode.KeypadEnter)
                    {
                        return;
                    }
                }
                this.CreateLibraryAndCloseWindow(editorWindow);
            }
        }

        public override void OnGUI(Rect rect)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            this.KeyboardHandling(base.editorWindow);
            float width = 80f;
            Rect rect2 = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            if (Event.current.type != EventType.Layout)
            {
                this.m_WantedSize = rect2;
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(s_Texts.header, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            GUILayout.Label(s_Texts.name, options);
            EditorGUI.FocusTextInControl("NewLibraryName");
            GUI.SetNextControlName("NewLibraryName");
            this.m_NewLibraryName = GUILayout.TextField(this.m_NewLibraryName, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(width) };
            GUILayout.Label(s_Texts.location, optionArray2);
            this.m_SelectedIndexInPopup = EditorGUILayout.Popup(this.m_SelectedIndexInPopup, s_Texts.fileLocations, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                this.m_ErrorString = null;
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (!string.IsNullOrEmpty(this.m_ErrorString))
            {
                Color color = GUI.color;
                GUI.color = new Color(1f, 0.8f, 0.8f);
                GUILayout.Label(GUIContent.Temp(this.m_ErrorString), EditorStyles.helpBox, new GUILayoutOption[0]);
                GUI.color = color;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.Temp("Create"), new GUILayoutOption[0]))
            {
                this.CreateLibraryAndCloseWindow(base.editorWindow);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
            EditorGUILayout.EndVertical();
        }

        private class Texts
        {
            public PresetFileLocation[] fileLocationOrder;
            public GUIContent[] fileLocations = new GUIContent[] { new GUIContent("Preferences Folder"), new GUIContent("Project Folder") };
            public GUIContent header = new GUIContent("Create New Library");
            public GUIContent location = new GUIContent("Location");
            public GUIContent name = new GUIContent("Name");

            public Texts()
            {
                PresetFileLocation[] locationArray1 = new PresetFileLocation[2];
                locationArray1[1] = PresetFileLocation.ProjectFolder;
                this.fileLocationOrder = locationArray1;
            }
        }
    }
}

