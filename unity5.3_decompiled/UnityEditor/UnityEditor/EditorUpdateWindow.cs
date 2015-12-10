namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class EditorUpdateWindow : EditorWindow
    {
        private Vector2 m_ScrollPos;
        private static GUIContent s_CheckForNewUpdatesText;
        private static string s_ErrorString;
        private static bool s_HasConnectionError;
        private static bool s_HasUpdate;
        private static string s_LatestVersionMessage;
        private static string s_LatestVersionString;
        private static bool s_ShowAtStartup;
        private static GUIContent s_TextHasUpdate;
        private static GUIContent s_TextUpToDate;
        private static GUIContent s_Title;
        private static GUIContent s_UnityLogo;
        private static string s_UpdateURL;

        private static void LoadResources()
        {
            if (s_UnityLogo == null)
            {
                s_ShowAtStartup = EditorPrefs.GetBool("EditorUpdateShowAtStartup", true);
                s_Title = EditorGUIUtility.TextContent("Unity Editor Update Check");
                s_UnityLogo = EditorGUIUtility.IconContent("UnityLogo");
                s_TextHasUpdate = EditorGUIUtility.TextContent("There is a new version of the Unity Editor available for download.\n\nCurrently installed version is {0}\nNew version is {1}");
                s_TextUpToDate = EditorGUIUtility.TextContent("The Unity Editor is up to date. Currently installed version is {0}");
                s_CheckForNewUpdatesText = EditorGUIUtility.TextContent("Check for Updates");
            }
        }

        public void OnGUI()
        {
            LoadResources();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUI.Box(new Rect(13f, 8f, (float) s_UnityLogo.image.width, (float) s_UnityLogo.image.height), s_UnityLogo, GUIStyle.none);
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(120f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (s_HasConnectionError)
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(405f) };
                GUILayout.Label(s_ErrorString, "WordWrappedLabel", optionArray1);
            }
            else if (s_HasUpdate)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(300f) };
                GUILayout.Label(string.Format(s_TextHasUpdate.text, InternalEditorUtility.GetFullUnityVersion(), s_LatestVersionString), "WordWrappedLabel", optionArray2);
                GUILayout.Space(20f);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(405f), GUILayout.Height(200f) };
                this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, optionArray3);
                GUILayout.Label(s_LatestVersionMessage, "WordWrappedLabel", new GUILayoutOption[0]);
                EditorGUILayout.EndScrollView();
                GUILayout.Space(20f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(200f) };
                if (GUILayout.Button("Download new version", optionArray4))
                {
                    Help.BrowseURL(s_UpdateURL);
                }
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(200f) };
                if (GUILayout.Button("Skip new version", optionArray5))
                {
                    EditorPrefs.SetString("EditorUpdateSkipVersionString", s_LatestVersionString);
                    base.Close();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Width(405f) };
                GUILayout.Label(string.Format(s_TextUpToDate.text, Application.unityVersion), "WordWrappedLabel", optionArray6);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);
            GUILayout.FlexibleSpace();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(20f) };
            GUILayout.BeginHorizontal(options);
            GUILayout.FlexibleSpace();
            GUI.changed = false;
            s_ShowAtStartup = GUILayout.Toggle(s_ShowAtStartup, s_CheckForNewUpdatesText, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                EditorPrefs.SetBool("EditorUpdateShowAtStartup", s_ShowAtStartup);
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void ShowEditorErrorWindow(string errorString)
        {
            LoadResources();
            s_ErrorString = errorString;
            s_HasConnectionError = true;
            s_HasUpdate = false;
            ShowWindow();
        }

        private static void ShowEditorUpdateWindow(string latestVersionString, string latestVersionMessage, string updateURL)
        {
            LoadResources();
            s_LatestVersionString = latestVersionString;
            s_LatestVersionMessage = latestVersionMessage;
            s_UpdateURL = updateURL;
            s_HasConnectionError = false;
            s_HasUpdate = updateURL.Length > 0;
            ShowWindow();
        }

        private static void ShowWindow()
        {
            EditorWindow.GetWindowWithRect(typeof(EditorUpdateWindow), new Rect(100f, 100f, 570f, 400f), true, s_Title.text);
        }
    }
}

