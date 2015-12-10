namespace UnityEditor.VersionControl
{
    using System;
    using UnityEditor;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class WindowRevert : EditorWindow
    {
        private AssetList assetList = new AssetList();
        private ListControl revertList = new ListControl();

        private void DoOpen(AssetList revert)
        {
            this.assetList = revert;
            this.RefreshList();
        }

        private static WindowRevert GetWindow()
        {
            return EditorWindow.GetWindow<WindowRevert>(true, "Version Control Revert");
        }

        public void OnEnable()
        {
            base.position = new Rect(100f, 100f, 700f, 230f);
            base.minSize = new Vector2(700f, 230f);
            this.revertList.ReadOnly = true;
        }

        private void OnGUI()
        {
            GUILayout.Label("Revert Files", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            Rect screenRect = new Rect(6f, 40f, base.position.width - 12f, base.position.height - 82f);
            GUILayout.BeginArea(screenRect);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            GUILayout.Box(string.Empty, options);
            GUILayout.EndArea();
            this.revertList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
            {
                base.Close();
            }
            if ((this.assetList.Count > 0) && GUILayout.Button("Revert", new GUILayoutOption[0]))
            {
                string str = string.Empty;
                foreach (Asset asset in this.assetList)
                {
                    Scene sceneByPath = SceneManager.GetSceneByPath(asset.path);
                    if (sceneByPath.IsValid() && sceneByPath.isLoaded)
                    {
                        str = str + sceneByPath.path + "\n";
                    }
                }
                if ((str.Length > 0) && !EditorUtility.DisplayDialog("Revert open scene(s)?", "You are about to revert your currently open scene(s):\n\n" + str + "\nContinuing will remove all unsaved changes.", "Continue", "Cancel"))
                {
                    base.Close();
                    return;
                }
                Provider.Revert(this.assetList, RevertMode.Normal).Wait();
                WindowPending.UpdateAllWindows();
                AssetDatabase.Refresh();
                base.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }

        public static void Open(AssetList assets)
        {
            Task task = Provider.Status(assets);
            task.Wait();
            Asset.States[] states = new Asset.States[] { Asset.States.CheckedOutLocal, Asset.States.DeletedLocal, Asset.States.AddedLocal, Asset.States.Missing };
            AssetList revert = task.assetList.Filter(true, states);
            GetWindow().DoOpen(revert);
        }

        public static void Open(ChangeSet change)
        {
            Task task = Provider.ChangeSetStatus(change);
            task.Wait();
            GetWindow().DoOpen(task.assetList);
        }

        private void RefreshList()
        {
            this.revertList.Clear();
            foreach (Asset asset in this.assetList)
            {
                this.revertList.Add(null, asset.prettyPath, asset);
            }
            if (this.assetList.Count == 0)
            {
                ChangeSet change = new ChangeSet("no files to revert");
                this.revertList.Add(null, change.description, change).Dummy = true;
            }
            this.revertList.Refresh();
            base.Repaint();
        }
    }
}

