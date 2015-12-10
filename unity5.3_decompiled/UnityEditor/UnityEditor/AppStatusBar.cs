namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal class AppStatusBar : GUIView
    {
        private static GUIStyle background;
        private string m_LastMiniMemoryOverview = string.Empty;
        private static GUIStyle resize;
        private static AppStatusBar s_AppStatusBar;
        private static GUIContent[] s_StatusWheel;

        private void OnEnable()
        {
            s_AppStatusBar = this;
            s_StatusWheel = new GUIContent[12];
            for (int i = 0; i < 12; i++)
            {
                s_StatusWheel[i] = EditorGUIUtility.IconContent("WaitSpin" + i.ToString("00"));
            }
        }

        private void OnGUI()
        {
            ConsoleWindow.LoadIcons();
            if (background == null)
            {
                background = "AppToolbar";
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUI.color = (Color) HostView.kPlayModeDarken;
            }
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
            }
            bool isCompiling = EditorApplication.isCompiling;
            GUILayout.Space(2f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(2f);
            string statusText = LogEntries.GetStatusText();
            if (statusText != null)
            {
                int statusMask = LogEntries.GetStatusMask();
                GUIStyle statusStyleForErrorMode = ConsoleWindow.GetStatusStyleForErrorMode(statusMask);
                GUILayout.Label(ConsoleWindow.GetIconForErrorMode(statusMask, false), statusStyleForErrorMode, new GUILayoutOption[0]);
                GUILayout.Space(2f);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Space(2f);
                if (isCompiling)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(GUIView.current.position.width - 52f) };
                    GUILayout.Label(statusText, statusStyleForErrorMode, options);
                }
                else
                {
                    GUILayout.Label(statusText, statusStyleForErrorMode, new GUILayoutOption[0]);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                if (Event.current.type == EventType.MouseDown)
                {
                    Event.current.Use();
                    LogEntries.ClickStatusBar(Event.current.clickCount);
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                float x = base.position.width - 24f;
                if (AsyncProgressBar.isShowing)
                {
                    x -= 188f;
                    EditorGUI.ProgressBar(new Rect(x, 0f, 185f, 19f), AsyncProgressBar.progress, AsyncProgressBar.progressInfo);
                }
                if (isCompiling)
                {
                    int index = (int) Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
                    GUI.Label(new Rect(base.position.width - 24f, 0f, (float) s_StatusWheel[index].image.width, (float) s_StatusWheel[index].image.height), s_StatusWheel[index], GUIStyle.none);
                }
                if (Unsupported.IsBleedingEdgeBuild())
                {
                    Color color = GUI.color;
                    GUI.color = Color.yellow;
                    GUI.Label(new Rect(x - 310f, 0f, 310f, 19f), "THIS IS AN UNTESTED BLEEDINGEDGE UNITY BUILD");
                    GUI.color = color;
                }
                else if (Unsupported.IsDeveloperBuild())
                {
                    GUI.Label(new Rect(x - 200f, 0f, 200f, 19f), this.m_LastMiniMemoryOverview, EditorStyles.progressBarText);
                    EditorGUIUtility.CleanCache(this.m_LastMiniMemoryOverview);
                }
            }
            base.DoWindowDecorationEnd();
            EditorGUI.ShowRepaints();
        }

        private void OnInspectorUpdate()
        {
            string miniMemoryOverview = ProfilerDriver.miniMemoryOverview;
            if (Unsupported.IsDeveloperBuild() && (this.m_LastMiniMemoryOverview != miniMemoryOverview))
            {
                this.m_LastMiniMemoryOverview = miniMemoryOverview;
                base.Repaint();
            }
        }

        [RequiredByNativeCode]
        public static void StatusChanged()
        {
            if (s_AppStatusBar != null)
            {
                s_AppStatusBar.Repaint();
            }
        }
    }
}

