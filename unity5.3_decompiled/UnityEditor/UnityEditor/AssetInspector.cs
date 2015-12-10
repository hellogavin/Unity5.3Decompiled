namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetInspector
    {
        private GUIContent[] m_Menu = new GUIContent[] { EditorGUIUtility.TextContent("Show Diff"), EditorGUIUtility.TextContent("Show History"), EditorGUIUtility.TextContent("Discard") };
        private GUIContent[] m_UnmodifiedMenu = new GUIContent[] { EditorGUIUtility.TextContent("Show History") };
        private static AssetInspector s_Instance;

        private AssetInspector()
        {
        }

        private string AddChangesetFlag(string str, string strToAdd)
        {
            if (str != string.Empty)
            {
                str = str + ", ";
                str = str + strToAdd;
                return str;
            }
            str = strToAdd;
            return str;
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (((bool) userData) && (selected == 0))
            {
                selected = 1;
            }
            switch (selected)
            {
                case 0:
                    this.DoShowDiff(this.GetGUID());
                    break;

                case 1:
                    ASEditorBackend.DoAS();
                    ASEditorBackend.ASWin.ShowHistory();
                    break;

                case 2:
                    if (!ASEditorBackend.SettingsIfNeeded())
                    {
                        Debug.Log("Asset Server connection for current project is not set up");
                    }
                    if (EditorUtility.DisplayDialog("Discard changes", "Are you sure you want to discard local changes of selected asset?", "Discard", "Cancel"))
                    {
                        string[] guids = new string[] { this.GetGUID() };
                        AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(guids);
                    }
                    break;
            }
        }

        private void DoShowDiff(string guid)
        {
            List<string> list = new List<string>();
            List<CompareInfo> list2 = new List<CompareInfo>();
            int workingItemChangeset = AssetServer.GetWorkingItemChangeset(guid);
            list.Add(guid);
            list2.Add(new CompareInfo(workingItemChangeset, -1, 0, 1));
            Debug.Log("Comparing asset revisions " + workingItemChangeset.ToString() + " and Local");
            AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
        }

        public static AssetInspector Get()
        {
            if (s_Instance == null)
            {
                s_Instance = new AssetInspector();
            }
            return s_Instance;
        }

        private ChangeFlags GetChangeFlags()
        {
            string gUID = this.GetGUID();
            if (gUID == string.Empty)
            {
                return ChangeFlags.None;
            }
            return AssetServer.GetChangeFlags(gUID);
        }

        private string GetGUID()
        {
            if (Selection.objects.Length == 0)
            {
                return string.Empty;
            }
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.objects[0]));
        }

        private string GetModificationString(ChangeFlags flags)
        {
            string str = string.Empty;
            if (this.HasFlag(flags, ChangeFlags.Undeleted))
            {
                str = this.AddChangesetFlag(str, "undeleted");
            }
            if (this.HasFlag(flags, ChangeFlags.Modified))
            {
                str = this.AddChangesetFlag(str, "modified");
            }
            if (this.HasFlag(flags, ChangeFlags.Renamed))
            {
                str = this.AddChangesetFlag(str, "renamed");
            }
            if (this.HasFlag(flags, ChangeFlags.Moved))
            {
                str = this.AddChangesetFlag(str, "moved");
            }
            if (this.HasFlag(flags, ChangeFlags.Created))
            {
                str = this.AddChangesetFlag(str, "created");
            }
            return str;
        }

        private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
        {
            return ((flagToCheck & flags) != ChangeFlags.None);
        }

        internal static bool IsAssetServerSetUp()
        {
            return (InternalEditorUtility.HasTeamLicense() && ASEditorBackend.SettingsAreValid());
        }

        public void OnAssetStatusGUI(Rect r, int id, Object target, GUIStyle style)
        {
            if (target != null)
            {
                GUIContent content;
                ChangeFlags changeFlags = this.GetChangeFlags();
                string modificationString = this.GetModificationString(changeFlags);
                if (modificationString == string.Empty)
                {
                    content = EditorGUIUtility.TextContent("Asset is unchanged");
                }
                else
                {
                    content = new GUIContent("Locally " + modificationString);
                }
                if (EditorGUI.DoToggle(r, id, false, content, style))
                {
                    GUIUtility.hotControl = 0;
                    r = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                    EditorUtility.DisplayCustomMenu(r, !(modificationString == string.Empty) ? this.m_Menu : this.m_UnmodifiedMenu, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), modificationString == string.Empty);
                }
            }
        }
    }
}

