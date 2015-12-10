namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEditorInternal;
    using UnityEngine;

    internal class BumpMapSettingsFixingWindow : EditorWindow
    {
        private ListViewState m_LV = new ListViewState();
        private string[] m_Paths;
        private static Styles s_Styles;

        public BumpMapSettingsFixingWindow()
        {
            base.titleContent = new GUIContent("NormalMap settings");
        }

        private void OnDestroy()
        {
            InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(0);
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
                base.minSize = new Vector2(400f, 300f);
                base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
            }
            GUILayout.Space(5f);
            GUILayout.Label(s_Styles.overviewText, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            IEnumerator enumerator = ListViewGUILayout.ListView(this.m_LV, s_Styles.box, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if ((current.row == this.m_LV.row) && (Event.current.type == EventType.Repaint))
                    {
                        s_Styles.selected.Draw(current.position, false, false, false, false);
                    }
                    GUILayout.Label(this.m_Paths[current.row], new GUILayoutOption[0]);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fix now", s_Styles.button, new GUILayoutOption[0]))
            {
                InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(1);
                base.Close();
            }
            if (GUILayout.Button("Ignore", s_Styles.button, new GUILayoutOption[0]))
            {
                InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(0);
                base.Close();
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }

        public void SetPaths(string[] paths)
        {
            this.m_Paths = paths;
            this.m_LV.totalRows = paths.Length;
        }

        public static void ShowWindow(string[] paths)
        {
            BumpMapSettingsFixingWindow window = EditorWindow.GetWindow<BumpMapSettingsFixingWindow>(true);
            window.SetPaths(paths);
            window.ShowUtility();
        }

        private class Styles
        {
            public GUIStyle box = "OL Box";
            public GUIStyle button = "LargeButton";
            public GUIContent overviewText = EditorGUIUtility.TextContent("A Material is using the texture as a normal map.\nThe texture must be marked as a normal map in the import settings.");
            public GUIStyle selected = "ServerUpdateChangesetOn";
        }
    }
}

