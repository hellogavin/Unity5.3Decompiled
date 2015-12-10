namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.IO;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Delete Layout")]
    internal class DeleteWindowLayout : EditorWindow
    {
        private const int kMaxLayoutNameLength = 15;
        internal string[] m_Paths;
        private Vector2 m_ScrollPos;

        private void InitializePaths()
        {
            string[] files = Directory.GetFiles(WindowLayout.layoutsPreferencesPath);
            ArrayList list = new ArrayList();
            foreach (string str in files)
            {
                if (Path.GetExtension(Path.GetFileName(str)) == ".wlt")
                {
                    list.Add(str);
                }
            }
            this.m_Paths = list.ToArray(typeof(string)) as string[];
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
        }

        private void OnGUI()
        {
            if (this.m_Paths == null)
            {
                this.InitializePaths();
            }
            this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
            foreach (string str in this.m_Paths)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
                if (fileNameWithoutExtension.Length > 15)
                {
                    fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, 15) + "...";
                }
                if (GUILayout.Button(fileNameWithoutExtension, new GUILayoutOption[0]))
                {
                    if (Toolbar.lastLoadedLayoutName == fileNameWithoutExtension)
                    {
                        Toolbar.lastLoadedLayoutName = null;
                    }
                    File.Delete(str);
                    InternalEditorUtility.ReloadWindowLayoutMenu();
                    this.InitializePaths();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}

