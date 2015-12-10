namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class UndoWindow : EditorWindow
    {
        private List<string> newRedos = new List<string>();
        private List<string> newUndos = new List<string>();
        private List<string> redos = new List<string>();
        private Vector2 redosScroll = Vector2.zero;
        private List<string> undos = new List<string>();
        private Vector2 undosScroll = Vector2.zero;

        internal static void Init()
        {
            EditorWindow.GetWindow(typeof(UndoWindow)).titleContent = new GUIContent("Undo");
        }

        private void OnGUI()
        {
            GUILayout.Label("(Available only in Developer builds)", EditorStyles.boldLabel, new GUILayoutOption[0]);
            float minHeight = base.position.height - 60f;
            float minWidth = (base.position.width * 0.5f) - 5f;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Undos", new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth) };
            this.undosScroll = GUILayout.BeginScrollView(this.undosScroll, EditorStyles.helpBox, options);
            int num3 = 0;
            foreach (string str in this.undos)
            {
                GUILayout.Label(string.Format("[{0}] - {1}", num3++, str), new GUILayoutOption[0]);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Redos", new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth) };
            this.redosScroll = GUILayout.BeginScrollView(this.redosScroll, EditorStyles.helpBox, optionArray2);
            num3 = 0;
            foreach (string str2 in this.redos)
            {
                GUILayout.Label(string.Format("[{0}] - {1}", num3++, str2), new GUILayoutOption[0]);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            Undo.GetRecords(this.newUndos, this.newRedos);
            if (!this.undos.SequenceEqual<string>(this.newUndos) || !this.redos.SequenceEqual<string>(this.newRedos))
            {
                this.undos = new List<string>(this.newUndos);
                this.redos = new List<string>(this.newRedos);
                base.Repaint();
            }
        }
    }
}

