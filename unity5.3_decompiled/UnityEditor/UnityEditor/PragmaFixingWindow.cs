namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEditor.Scripting;
    using UnityEngine;

    internal class PragmaFixingWindow : EditorWindow
    {
        private ListViewState m_LV = new ListViewState();
        private string[] m_Paths;
        private static Styles s_Styles;

        public PragmaFixingWindow()
        {
            base.titleContent = new GUIContent("Unity - #pragma fixing");
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
                base.minSize = new Vector2(450f, 300f);
                base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
            }
            GUILayout.Space(10f);
            GUILayout.Label("#pragma implicit and #pragma downcast need to be added to following files\nfor backwards compatibility", new GUILayoutOption[0]);
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
                base.Close();
                PragmaFixing30.FixFiles(this.m_Paths);
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Ignore", s_Styles.button, new GUILayoutOption[0]))
            {
                base.Close();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Quit", s_Styles.button, new GUILayoutOption[0]))
            {
                EditorApplication.Exit(0);
                GUIUtility.ExitGUI();
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
            PragmaFixingWindow window = EditorWindow.GetWindow<PragmaFixingWindow>(true);
            window.SetPaths(paths);
            window.ShowModal();
        }

        private class Styles
        {
            public GUIStyle box = "OL Box";
            public GUIStyle button = "LargeButton";
            public GUIStyle selected = "ServerUpdateChangesetOn";
        }
    }
}

