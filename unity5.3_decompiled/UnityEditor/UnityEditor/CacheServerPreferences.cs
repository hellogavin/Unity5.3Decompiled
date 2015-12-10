namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class CacheServerPreferences
    {
        private static bool s_CacheServerEnabled;
        private static string s_CacheServerIPAddress;
        private static ConnectionState s_ConnectionState;
        private static bool s_PrefsLoaded;

        [PreferenceItem("Cache Server")]
        public static void OnGUI()
        {
            GUILayout.Space(10f);
            if (!InternalEditorUtility.HasTeamLicense())
            {
                GUILayout.Label(EditorGUIUtility.TempContent("You need to have a Pro or Team license to use the cache server.", EditorGUIUtility.GetHelpIcon(MessageType.Warning)), EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasTeamLicense());
            if (!s_PrefsLoaded)
            {
                ReadPreferences();
                s_PrefsLoaded = true;
            }
            if (s_CacheServerEnabled && (s_ConnectionState == ConnectionState.Unknown))
            {
                if (InternalEditorUtility.CanConnectToCacheServer())
                {
                    s_ConnectionState = ConnectionState.Success;
                }
                else
                {
                    s_ConnectionState = ConnectionState.Failure;
                }
            }
            EditorGUI.BeginChangeCheck();
            s_CacheServerEnabled = EditorGUILayout.Toggle("Use Cache Server", s_CacheServerEnabled, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(!s_CacheServerEnabled);
            s_CacheServerIPAddress = EditorGUILayout.TextField("IP Address", s_CacheServerIPAddress, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                s_ConnectionState = ConnectionState.Unknown;
            }
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(150f) };
            if (GUILayout.Button("Check Connection", options))
            {
                if (InternalEditorUtility.CanConnectToCacheServer())
                {
                    s_ConnectionState = ConnectionState.Success;
                }
                else
                {
                    s_ConnectionState = ConnectionState.Failure;
                }
            }
            GUILayout.Space(-25f);
            switch (s_ConnectionState)
            {
                case ConnectionState.Unknown:
                    GUILayout.Space(44f);
                    break;

                case ConnectionState.Success:
                    EditorGUILayout.HelpBox("Connection successful.", MessageType.Info, false);
                    break;

                case ConnectionState.Failure:
                    EditorGUILayout.HelpBox("Connection failed.", MessageType.Warning, false);
                    break;
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                WritePreferences();
                ReadPreferences();
            }
            EditorGUI.EndDisabledGroup();
        }

        public static void ReadPreferences()
        {
            s_CacheServerIPAddress = EditorPrefs.GetString("CacheServerIPAddress", s_CacheServerIPAddress);
            s_CacheServerEnabled = EditorPrefs.GetBool("CacheServerEnabled");
        }

        public static void WritePreferences()
        {
            EditorPrefs.SetString("CacheServerIPAddress", s_CacheServerIPAddress);
            EditorPrefs.SetBool("CacheServerEnabled", s_CacheServerEnabled);
        }

        private enum ConnectionState
        {
            Unknown,
            Success,
            Failure
        }
    }
}

