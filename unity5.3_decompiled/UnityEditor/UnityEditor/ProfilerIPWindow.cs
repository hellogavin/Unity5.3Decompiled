namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ProfilerIPWindow : EditorWindow
    {
        internal bool didFocus;
        private const string kLastIP = "ProfilerLastIP";
        private const string kTextFieldId = "IPWindow";
        internal string m_IPString = GetLastIPString();

        public static string GetLastIPString()
        {
            return EditorPrefs.GetString("ProfilerLastIP", string.Empty);
        }

        private void OnGUI()
        {
            Event current = Event.current;
            bool flag = (current.type == EventType.KeyDown) && ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter));
            GUI.SetNextControlName("IPWindow");
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            this.m_IPString = EditorGUILayout.TextField(this.m_IPString, new GUILayoutOption[0]);
            if (!this.didFocus)
            {
                this.didFocus = true;
                EditorGUI.FocusTextInControl("IPWindow");
            }
            GUI.enabled = this.m_IPString.Length != 0;
            if (GUILayout.Button("Connect", new GUILayoutOption[0]) || flag)
            {
                base.Close();
                EditorPrefs.SetString("ProfilerLastIP", this.m_IPString);
                AttachProfilerUI.DirectIPConnect(this.m_IPString);
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndVertical();
        }

        public static void Show(Rect buttonScreenRect)
        {
            Rect rect = new Rect(buttonScreenRect.x, buttonScreenRect.yMax, 300f, 50f);
            ProfilerIPWindow window = EditorWindow.GetWindowWithRect<ProfilerIPWindow>(rect, true, "Enter Player IP");
            window.position = rect;
            window.m_Parent.window.m_DontSaveToLayout = true;
        }
    }
}

