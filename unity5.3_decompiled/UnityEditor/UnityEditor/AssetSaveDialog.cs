namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class AssetSaveDialog : EditorWindow
    {
        private List<string> m_Assets;
        private List<string> m_AssetsToSave;
        private List<GUIContent> m_Content;
        private int m_InitialSelectedItem = -1;
        private ListViewState m_LV = new ListViewState();
        private bool[] m_SelectedItems;
        private static Styles s_Styles;

        private void Cancel()
        {
            base.Close();
            GUIUtility.ExitGUI();
        }

        private void CloseWindow()
        {
            base.Close();
            GUIUtility.ExitGUI();
        }

        public static GUIContent GetContentForAsset(string path)
        {
            Texture cachedIcon = AssetDatabase.GetCachedIcon(path);
            if (path.StartsWith("Library/"))
            {
                path = ObjectNames.NicifyVariableName(AssetDatabase.LoadMainAssetAtPath(path).name);
            }
            if (path.StartsWith("Assets/"))
            {
                path = path.Substring(7);
            }
            return new GUIContent(path, cachedIcon);
        }

        private void HandleKeyboard()
        {
        }

        private void IgnoreSelectedAssets()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.m_SelectedItems.Length; i++)
            {
                if (!this.m_SelectedItems[i])
                {
                    list.Add(this.m_Assets[i]);
                }
            }
            this.m_Assets = list;
            this.RebuildLists(false);
            if (this.m_Assets.Count == 0)
            {
                this.CloseWindow();
            }
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
                base.minSize = new Vector2(500f, 300f);
                base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
            }
            this.HandleKeyboard();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label("Unity is about to save the following modified files. Unsaved changes will be lost!", new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            int row = this.m_LV.row;
            int num2 = 0;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.m_LV, s_Styles.box, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if (this.m_SelectedItems[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        Rect position = current.position;
                        position.x++;
                        position.y++;
                        position.width--;
                        position.height--;
                        s_Styles.selected.Draw(position, false, false, false, false);
                    }
                    GUILayout.Label(this.m_Content[current.row], new GUILayoutOption[0]);
                    if (ListViewGUILayout.HasMouseUp(current.position))
                    {
                        Event.current.command = true;
                        Event.current.control = true;
                        ListViewGUILayout.MultiSelection(row, current.row, ref this.m_InitialSelectedItem, ref this.m_SelectedItems);
                    }
                    if (this.m_SelectedItems[current.row])
                    {
                        num2++;
                    }
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
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(s_Styles.buttonWidth) };
            if (GUILayout.Button(s_Styles.close, s_Styles.button, options))
            {
                this.CloseWindow();
            }
            GUILayout.FlexibleSpace();
            GUI.enabled = num2 > 0;
            bool flag = num2 == this.m_Assets.Count;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(s_Styles.buttonWidth) };
            if (GUILayout.Button(s_Styles.dontSave, s_Styles.button, optionArray2))
            {
                this.IgnoreSelectedAssets();
            }
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(s_Styles.buttonWidth) };
            if (GUILayout.Button(!flag ? s_Styles.saveSelected : s_Styles.saveAll, s_Styles.button, optionArray3))
            {
                this.SaveSelectedAssets();
            }
            if (this.m_Assets.Count == 0)
            {
                this.CloseWindow();
            }
            GUI.enabled = true;
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }

        private void RebuildLists(bool selected)
        {
            this.m_LV.totalRows = this.m_Assets.Count;
            this.m_SelectedItems = new bool[this.m_Assets.Count];
            this.m_Content = new List<GUIContent>(this.m_Assets.Count);
            for (int i = 0; i < this.m_Assets.Count; i++)
            {
                this.m_SelectedItems[i] = selected;
                this.m_Content.Add(GetContentForAsset(this.m_Assets[i]));
            }
        }

        private void SaveSelectedAssets()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.m_SelectedItems.Length; i++)
            {
                if (this.m_SelectedItems[i])
                {
                    this.m_AssetsToSave.Add(this.m_Assets[i]);
                }
                else
                {
                    list.Add(this.m_Assets[i]);
                }
            }
            this.m_Assets = list;
            this.RebuildLists(false);
        }

        private void SetAssets(string[] assets)
        {
            this.m_Assets = new List<string>(assets);
            this.RebuildLists(true);
            this.m_AssetsToSave = new List<string>();
        }

        public static void ShowWindow(string[] inAssets, out string[] assetsThatShouldBeSaved)
        {
            int num = 0;
            foreach (string str in inAssets)
            {
                if (str.EndsWith("meta"))
                {
                    num++;
                }
            }
            int num3 = inAssets.Length - num;
            if (num3 == 0)
            {
                assetsThatShouldBeSaved = inAssets;
            }
            else
            {
                string[] assets = new string[num3];
                string[] strArray3 = new string[num];
                num3 = 0;
                num = 0;
                foreach (string str2 in inAssets)
                {
                    if (str2.EndsWith("meta"))
                    {
                        strArray3[num++] = str2;
                    }
                    else
                    {
                        assets[num3++] = str2;
                    }
                }
                AssetSaveDialog windowDontShow = EditorWindow.GetWindowDontShow<AssetSaveDialog>();
                windowDontShow.titleContent = EditorGUIUtility.TextContent("Save Assets");
                windowDontShow.SetAssets(assets);
                windowDontShow.ShowUtility();
                windowDontShow.ShowModal();
                assetsThatShouldBeSaved = new string[windowDontShow.m_AssetsToSave.Count + num];
                windowDontShow.m_AssetsToSave.CopyTo(assetsThatShouldBeSaved, 0);
                strArray3.CopyTo(assetsThatShouldBeSaved, windowDontShow.m_AssetsToSave.Count);
            }
        }

        private class Styles
        {
            public GUIStyle box = "OL Box";
            public GUIStyle button = "LargeButton";
            public float buttonWidth;
            public GUIContent close = EditorGUIUtility.TextContent("Close");
            public GUIContent dontSave = EditorGUIUtility.TextContent("Don't Save");
            public GUIContent saveAll = EditorGUIUtility.TextContent("Save All");
            public GUIContent saveSelected = EditorGUIUtility.TextContent("Save Selected");
            public GUIStyle selected = "ServerUpdateChangesetOn";

            public Styles()
            {
                this.buttonWidth = Mathf.Max(Mathf.Max(this.button.CalcSize(this.saveSelected).x, this.button.CalcSize(this.saveAll).x), this.button.CalcSize(this.dontSave).x);
            }
        }
    }
}

