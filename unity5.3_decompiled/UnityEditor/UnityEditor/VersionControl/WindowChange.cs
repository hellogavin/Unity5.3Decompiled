namespace UnityEditor.VersionControl
{
    using System;
    using UnityEditor;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class WindowChange : EditorWindow
    {
        private bool allowSubmit;
        private AssetList assetList = new AssetList();
        private const string c_defaultDescription = "";
        private ChangeSet changeSet = new ChangeSet();
        private string description = string.Empty;
        private const int kSubmitNotStartedResultCode = 0x100;
        private const int kSubmitRunningResultCode = 0;
        private int m_TextAreaControlID;
        private string submitErrorMessage;
        private ListControl submitList = new ListControl();
        private int submitResultCode = 0x100;
        private Task taskAdd;
        private Task taskDesc;
        private Task taskStat;
        private Task taskStatus;
        private Task taskSubmit;

        private void DoOpen(ChangeSet change, AssetList assets)
        {
            this.taskSubmit = null;
            this.submitResultCode = 0x100;
            this.submitErrorMessage = null;
            this.changeSet = change;
            this.description = (change != null) ? this.SanitizeDescription(change.description) : string.Empty;
            this.assetList = null;
            if (change == null)
            {
                this.taskStatus = Provider.Status(assets);
            }
            else
            {
                this.taskDesc = Provider.ChangeSetDescription(change);
                this.taskStat = Provider.ChangeSetStatus(change);
            }
        }

        internal static void OnAdded(Task task)
        {
            WindowChange[] changeArray = Resources.FindObjectsOfTypeAll(typeof(WindowChange)) as WindowChange[];
            if (changeArray.Length != 0)
            {
                WindowChange change = changeArray[0];
                change.taskSubmit = null;
                change.submitResultCode = 0x100;
                change.submitErrorMessage = null;
                change.taskAdd = null;
                change.taskStatus = Provider.Status(change.assetList, false);
                change.assetList = null;
                WindowPending.UpdateAllWindows();
            }
        }

        private void OnConflictingFilesGUI()
        {
            string text = string.Empty;
            foreach (Asset asset in this.assetList)
            {
                if (asset.IsState(Asset.States.Conflicted))
                {
                    text = text + asset.prettyPath + "\n";
                }
            }
            GUILayout.Label("Conflicting files", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("Some files need to be resolved before submitting:", new GUILayoutOption[0]);
            GUI.enabled = false;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            GUILayout.TextArea(text, options);
            GUI.enabled = true;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", new GUILayoutOption[0]))
            {
                this.ResetAndClose();
            }
            GUILayout.EndHorizontal();
        }

        public void OnDisable()
        {
            this.m_TextAreaControlID = 0;
        }

        public void OnEnable()
        {
            base.position = new Rect(100f, 100f, 700f, 395f);
            base.minSize = new Vector2(700f, 395f);
            this.submitList.ReadOnly = true;
            this.taskStatus = null;
            this.taskDesc = null;
            this.taskStat = null;
            this.taskSubmit = null;
            this.submitResultCode = 0x100;
            this.submitErrorMessage = null;
        }

        private void OnErrorGUI()
        {
            GUILayout.Label("Submit failed", EditorStyles.boldLabel, new GUILayoutOption[0]);
            string str = string.Empty;
            if (!string.IsNullOrEmpty(this.submitErrorMessage))
            {
                str = this.submitErrorMessage + "\n";
            }
            GUILayout.Label(str + "See console for details. You can get more details by increasing log level in EditorSettings.", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", new GUILayoutOption[0]))
            {
                this.ResetAndClose();
                WindowPending.UpdateAllWindows();
            }
            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            if ((this.submitResultCode & 4) != 0)
            {
                this.OnConflictingFilesGUI();
            }
            else if ((this.submitResultCode & 8) != 0)
            {
                this.OnUnaddedFilesGUI();
            }
            else if ((this.submitResultCode & 2) != 0)
            {
                this.OnErrorGUI();
            }
            else
            {
                this.OnSubmitGUI();
            }
        }

        private void OnSubmitGUI()
        {
            if (this.submitResultCode != 0x100)
            {
                GUI.enabled = false;
            }
            Event current = Event.current;
            if (current.isKey && (current.keyCode == KeyCode.Escape))
            {
                base.Close();
            }
            GUILayout.Label("Description", EditorStyles.boldLabel, new GUILayoutOption[0]);
            if ((this.taskStatus != null) && (this.taskStatus.resultCode != 0))
            {
                Asset.States[] states = new Asset.States[] { Asset.States.CheckedOutLocal, Asset.States.DeletedLocal, Asset.States.AddedLocal };
                this.assetList = this.taskStatus.assetList.Filter(true, states);
                this.RefreshList();
                this.taskStatus = null;
            }
            else if ((this.taskDesc != null) && (this.taskDesc.resultCode != 0))
            {
                this.description = (this.taskDesc.text.Length <= 0) ? string.Empty : this.taskDesc.text;
                if (this.description.Trim() == "<enter description here>")
                {
                    this.description = string.Empty;
                }
                this.taskDesc = null;
            }
            else if ((this.taskStat != null) && (this.taskStat.resultCode != 0))
            {
                this.assetList = this.taskStat.assetList;
                this.RefreshList();
                this.taskStat = null;
            }
            Task task = ((this.taskStatus == null) || (this.taskStatus.resultCode != 0)) ? (((this.taskDesc == null) || (this.taskDesc.resultCode != 0)) ? (((this.taskStat == null) || (this.taskStat.resultCode != 0)) ? this.taskSubmit : this.taskStat) : this.taskDesc) : this.taskStatus;
            GUI.enabled = ((this.taskDesc == null) || (this.taskDesc.resultCode != 0)) && (this.submitResultCode == 0x100);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(150f) };
            this.description = EditorGUILayout.TextArea(this.description, options).Trim();
            if (this.m_TextAreaControlID == 0)
            {
                this.m_TextAreaControlID = EditorGUIUtility.s_LastControlID;
            }
            if (this.m_TextAreaControlID != 0)
            {
                GUIUtility.keyboardControl = this.m_TextAreaControlID;
                EditorGUIUtility.editingTextField = true;
            }
            GUI.enabled = true;
            GUILayout.Label("Files", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            Rect screenRect = new Rect(6f, 206f, base.position.width - 12f, base.position.height - 248f);
            GUILayout.BeginArea(screenRect);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            GUILayout.Box(string.Empty, optionArray2);
            GUILayout.EndArea();
            this.submitList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (this.submitResultCode == 0x100)
            {
                if (task != null)
                {
                    GUIContent content = GUIContent.Temp("Getting info");
                    content.image = WindowPending.StatusWheel.image;
                    GUILayout.Label(content, new GUILayoutOption[0]);
                    content.image = null;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
                {
                    this.ResetAndClose();
                }
                GUI.enabled = (task == null) && !string.IsNullOrEmpty(this.description);
                bool flag3 = (current.isKey && current.shift) && (current.keyCode == KeyCode.Return);
                bool flag4 = flag3 && !this.allowSubmit;
                if (Provider.hasChangelistSupport && (GUILayout.Button("Save", new GUILayoutOption[0]) || flag4))
                {
                    this.Save(false);
                }
                if (this.allowSubmit)
                {
                    bool enabled = GUI.enabled;
                    GUI.enabled = ((this.assetList != null) && (this.assetList.Count > 0)) && !string.IsNullOrEmpty(this.description);
                    if (GUILayout.Button("Submit", new GUILayoutOption[0]) || flag3)
                    {
                        this.Save(true);
                    }
                    GUI.enabled = enabled;
                }
            }
            else
            {
                bool flag6 = (this.submitResultCode & 1) != 0;
                GUI.enabled = flag6;
                string text = string.Empty;
                if (flag6)
                {
                    text = "Finished successfully";
                }
                else if (task != null)
                {
                    GUILayout.Label(WindowPending.StatusWheel, new GUILayoutOption[0]);
                    text = task.progressMessage;
                    if (text.Length == 0)
                    {
                        text = "Running...";
                    }
                }
                GUILayout.Label(text, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", new GUILayoutOption[0]))
                {
                    this.ResetAndClose();
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
            if (task != null)
            {
                base.Repaint();
            }
        }

        internal static void OnSubmitted(Task task)
        {
            WindowChange[] changeArray = Resources.FindObjectsOfTypeAll(typeof(WindowChange)) as WindowChange[];
            if (changeArray.Length != 0)
            {
                WindowChange change = changeArray[0];
                change.assetList = task.assetList;
                change.submitResultCode = task.resultCode;
                change.submitErrorMessage = null;
                if ((task.resultCode & 2) != 0)
                {
                    string str = string.Empty;
                    foreach (Message message in task.messages)
                    {
                        if (message.severity == Message.Severity.Error)
                        {
                            change.submitErrorMessage = change.submitErrorMessage + str + message.message;
                        }
                    }
                }
                if ((task.resultCode & 3) != 0)
                {
                    WindowPending.UpdateAllWindows();
                    if (change.changeSet == null)
                    {
                        Provider.Status(string.Empty).Wait();
                        WindowPending.ExpandLatestChangeSet();
                    }
                }
                if ((task.resultCode & 1) != 0)
                {
                    change.ResetAndClose();
                }
                else
                {
                    change.RefreshList();
                }
            }
        }

        private void OnUnaddedFilesGUI()
        {
            AssetList assets = new AssetList();
            string text = string.Empty;
            foreach (Asset asset in this.assetList)
            {
                if ((!asset.IsState(Asset.States.OutOfSync) && !asset.IsState(Asset.States.Synced)) && !asset.IsState(Asset.States.AddedLocal))
                {
                    text = text + asset.prettyPath + "\n";
                    assets.Add(asset);
                }
            }
            GUILayout.Label("Files to add", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("Some additional files need to be added:", new GUILayoutOption[0]);
            GUI.enabled = false;
            GUILayout.TextArea(text, new GUILayoutOption[0]);
            GUI.enabled = true;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add files", new GUILayoutOption[0]))
            {
                this.taskAdd = Provider.Add(assets, false);
                this.taskAdd.SetCompletionAction(CompletionAction.OnAddedChangeWindow);
            }
            if (GUILayout.Button("Abort", new GUILayoutOption[0]))
            {
                this.ResetAndClose();
            }
            GUILayout.EndHorizontal();
        }

        public static void Open(AssetList list, bool submit)
        {
            Open(null, list, submit);
        }

        public static void Open(ChangeSet change, AssetList assets, bool submit)
        {
            WindowChange window = EditorWindow.GetWindow<WindowChange>(true, "Version Control Changeset");
            window.allowSubmit = submit;
            window.DoOpen(change, assets);
        }

        private void RefreshList()
        {
            this.submitList.Clear();
            foreach (Asset asset in this.assetList)
            {
                this.submitList.Add(null, asset.prettyPath, asset);
            }
            if (this.assetList.Count == 0)
            {
                ChangeSet change = new ChangeSet("Empty change list");
                this.submitList.Add(null, change.description, change).Dummy = true;
            }
            this.submitList.Refresh();
            base.Repaint();
        }

        private void ResetAndClose()
        {
            this.taskSubmit = null;
            this.submitResultCode = 0x100;
            this.submitErrorMessage = null;
            base.Close();
        }

        private string SanitizeDescription(string desc)
        {
            if ((Provider.GetActivePlugin() != null) && (Provider.GetActivePlugin().name != "Perforce"))
            {
                return desc;
            }
            int index = desc.IndexOf('\'');
            if (index == -1)
            {
                return desc;
            }
            index++;
            int num2 = desc.IndexOf('\'', index);
            if (num2 == -1)
            {
                return desc;
            }
            char[] trimChars = new char[] { ' ', '\t' };
            return desc.Substring(index, num2 - index).Trim(trimChars);
        }

        private void Save(bool submit)
        {
            if (this.description.Trim() == string.Empty)
            {
                Debug.LogError("Version control: Please enter a valid change description");
            }
            else
            {
                EditorApplication.SaveAssets();
                this.taskSubmit = Provider.Submit(this.changeSet, this.assetList, this.description, !submit);
                this.submitResultCode = 0;
                this.submitErrorMessage = null;
                this.taskSubmit.SetCompletionAction(CompletionAction.OnSubmittedChangeWindow);
            }
        }
    }
}

