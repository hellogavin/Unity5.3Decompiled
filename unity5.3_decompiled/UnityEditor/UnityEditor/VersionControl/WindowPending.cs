namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    [EditorWindowTitle(title="Versioning", icon="UnityEditor.VersionControl")]
    internal class WindowPending : EditorWindow
    {
        [CompilerGenerated]
        private static Predicate<ChangeSet> <>f__am$cacheD;
        private static Texture2D changeIcon;
        private GUIStyle header;
        [SerializeField]
        private ListControl incomingList;
        private const float k_BottomBarHeight = 17f;
        private const float k_MinIncomingAreaHeight = 50f;
        private const float k_ResizerHeight = 17f;
        private bool m_ShowIncoming;
        [SerializeField]
        private ListControl pendingList;
        private Texture2D refreshIcon;
        private float s_DeleteChangesetsButtonWidth;
        private static bool s_DidReload;
        private float s_SettingsButtonWidth;
        private static Styles s_Styles;
        private float s_ToolbarButtonsWidth;
        private static GUIContent[] sStatusWheel;
        private Texture2D syncIcon;

        public static void CloseAllWindows()
        {
            WindowPending[] pendingArray = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
            WindowPending pending = (pendingArray.Length <= 0) ? null : pendingArray[0];
            if (pending != null)
            {
                pending.Close();
            }
        }

        private void CreateResources()
        {
            if (this.refreshIcon == null)
            {
                this.refreshIcon = EditorGUIUtility.LoadIcon("Refresh");
                this.refreshIcon.hideFlags = HideFlags.HideAndDontSave;
                this.refreshIcon.name = "RefreshIcon";
            }
            if (this.header == null)
            {
                this.header = "OL Title";
            }
            this.CreateStaticResources();
            if (this.s_ToolbarButtonsWidth == 0f)
            {
                this.s_ToolbarButtonsWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Incoming (xx)")).x;
                this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent("Outgoing")).x;
                this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent(this.refreshIcon)).x;
                this.s_SettingsButtonWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Settings")).x;
                this.s_DeleteChangesetsButtonWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Delete Empty Changesets")).x;
            }
        }

        private void CreateStaticResources()
        {
            if (this.syncIcon == null)
            {
                this.syncIcon = EditorGUIUtility.LoadIcon("vcs_incoming");
                this.syncIcon.hideFlags = HideFlags.HideAndDontSave;
                this.syncIcon.name = "SyncIcon";
            }
            if (changeIcon == null)
            {
                changeIcon = EditorGUIUtility.LoadIcon("vcs_change");
                changeIcon.hideFlags = HideFlags.HideAndDontSave;
                changeIcon.name = "ChangeIcon";
            }
        }

        private void DeleteEmptyPendingChangesets()
        {
            Provider.DeleteChangeSets(this.GetEmptyChangeSetsCandidates()).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        public static void ExpandLatestChangeSet()
        {
            WindowPending[] pendingArray = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
            foreach (WindowPending pending in pendingArray)
            {
                pending.pendingList.ExpandLastItem();
            }
        }

        private ChangeSets GetEmptyChangeSetsCandidates()
        {
            <GetEmptyChangeSetsCandidates>c__AnonStoreyB8 yb = new <GetEmptyChangeSetsCandidates>c__AnonStoreyB8();
            ChangeSets emptyChangeSets = this.pendingList.EmptyChangeSets;
            yb.toDelete = new ChangeSets();
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = item => item.id != ChangeSet.defaultID;
            }
            emptyChangeSets.FindAll(<>f__am$cacheD).ForEach(new Action<ChangeSet>(yb.<>m__224));
            return yb.toDelete;
        }

        private bool HasEmptyPendingChangesets()
        {
            return Provider.DeleteChangeSetsIsValid(this.GetEmptyChangeSetsCandidates());
        }

        private void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        private void OnChangeContents(Task task)
        {
            ListItem item = this.pendingList.FindItemWithIdentifier(task.userIdentifier);
            ListItem parent = (item != null) ? item : this.incomingList.FindItemWithIdentifier(task.userIdentifier);
            if (parent != null)
            {
                ListControl control = (item != null) ? this.pendingList : this.incomingList;
                parent.RemoveAll();
                AssetList assetList = task.assetList;
                if (assetList.Count == 0)
                {
                    control.Add(parent, "Empty change list", (Asset) null).Dummy = true;
                }
                else
                {
                    foreach (Asset asset in assetList)
                    {
                        control.Add(parent, asset.prettyPath, asset);
                    }
                }
                control.Refresh(false);
                base.Repaint();
            }
        }

        private void OnChangeSets(Task task)
        {
            this.CreateStaticResources();
            this.PopulateListControl(this.pendingList, task, changeIcon);
        }

        private void OnDrop(ChangeSet targetItem)
        {
            Provider.ChangeSetMove(this.pendingList.SelectedAssets, targetItem).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            if (this.pendingList == null)
            {
                this.pendingList = new ListControl();
            }
            this.pendingList.ExpandEvent = (ListControl.ExpandDelegate) Delegate.Combine(this.pendingList.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpand));
            this.pendingList.DragEvent = (ListControl.DragDelegate) Delegate.Combine(this.pendingList.DragEvent, new ListControl.DragDelegate(this.OnDrop));
            this.pendingList.MenuDefault = "CONTEXT/Pending";
            this.pendingList.MenuFolder = "CONTEXT/Change";
            this.pendingList.DragAcceptOnly = true;
            if (this.incomingList == null)
            {
                this.incomingList = new ListControl();
            }
            this.incomingList.ExpandEvent = (ListControl.ExpandDelegate) Delegate.Combine(this.incomingList.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpandIncoming));
            this.UpdateWindow();
        }

        private void OnExpand(ChangeSet change, ListItem item)
        {
            if (Provider.isActive)
            {
                Task task = Provider.ChangeSetStatus(change);
                task.userIdentifier = item.Identifier;
                task.SetCompletionAction(CompletionAction.OnChangeContentsPendingWindow);
                if (!item.HasChildren)
                {
                    Asset asset = new Asset("Updating...");
                    this.pendingList.Add(item, asset.prettyPath, asset).Dummy = true;
                    this.pendingList.Refresh(false);
                    base.Repaint();
                }
            }
        }

        private void OnExpandIncoming(ChangeSet change, ListItem item)
        {
            if (Provider.isActive)
            {
                Task task = Provider.IncomingChangeSetAssets(change);
                task.userIdentifier = item.Identifier;
                task.SetCompletionAction(CompletionAction.OnChangeContentsPendingWindow);
                if (!item.HasChildren)
                {
                    Asset asset = new Asset("Updating...");
                    this.incomingList.Add(item, asset.prettyPath, asset).Dummy = true;
                    this.incomingList.Refresh(false);
                    base.Repaint();
                }
            }
        }

        private void OnGotLatest(Task t)
        {
            this.UpdateWindow();
        }

        private void OnGUI()
        {
            this.InitStyles();
            if (!s_DidReload)
            {
                s_DidReload = true;
                this.UpdateWindow();
            }
            this.CreateResources();
            Event current = Event.current;
            float fixedHeight = EditorStyles.toolbar.fixedHeight;
            bool flag = false;
            GUILayout.BeginArea(new Rect(0f, 0f, base.position.width, fixedHeight));
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            int num2 = (this.incomingList.Root != null) ? this.incomingList.Root.ChildCount : 0;
            this.m_ShowIncoming = !GUILayout.Toggle(!this.m_ShowIncoming, "Outgoing", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            GUIContent content = GUIContent.Temp("Incoming" + ((num2 != 0) ? (" (" + num2.ToString() + ")") : string.Empty));
            this.m_ShowIncoming = GUILayout.Toggle(this.m_ShowIncoming, content, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                flag = true;
            }
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(Provider.activeTask != null);
            foreach (CustomCommand command in Provider.customCommands)
            {
                if ((command.context == CommandContext.Global) && GUILayout.Button(command.label, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    command.StartTask();
                }
            }
            EditorGUI.EndDisabledGroup();
            if (((Mathf.FloorToInt(((base.position.width - this.s_ToolbarButtonsWidth) - this.s_SettingsButtonWidth) - this.s_DeleteChangesetsButtonWidth) > 0) && this.HasEmptyPendingChangesets()) && GUILayout.Button("Delete Empty Changesets", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.DeleteEmptyPendingChangesets();
            }
            if ((Mathf.FloorToInt((base.position.width - this.s_ToolbarButtonsWidth) - this.s_SettingsButtonWidth) > 0) && GUILayout.Button("Settings", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
                EditorWindow.FocusWindowIfItsOpen<InspectorWindow>();
                GUIUtility.ExitGUI();
            }
            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            bool flag4 = GUILayout.Button(this.refreshIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            flag = flag || flag4;
            GUI.color = color;
            if ((current.isKey && (GUIUtility.keyboardControl == 0)) && ((current.type == EventType.KeyDown) && (current.keyCode == KeyCode.F5)))
            {
                flag = true;
                current.Use();
            }
            if (flag)
            {
                if (flag4)
                {
                    Provider.InvalidateCache();
                }
                this.UpdateWindow();
            }
            GUILayout.EndArea();
            Rect screenRect = new Rect(0f, fixedHeight, base.position.width, (base.position.height - fixedHeight) - 17f);
            bool flag5 = false;
            GUILayout.EndHorizontal();
            if (!Provider.isActive)
            {
                Color color2 = GUI.color;
                GUI.color = new Color(0.8f, 0.5f, 0.5f);
                screenRect.height = fixedHeight;
                GUILayout.BeginArea(screenRect);
                GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                string text = "DISABLED";
                if (Provider.enabled)
                {
                    if (Provider.onlineState == OnlineState.Updating)
                    {
                        GUI.color = new Color(0.8f, 0.8f, 0.5f);
                        text = "CONNECTING...";
                    }
                    else
                    {
                        text = "OFFLINE";
                    }
                }
                GUILayout.Label(text, EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                screenRect.y += screenRect.height;
                if (!string.IsNullOrEmpty(Provider.offlineReason))
                {
                    GUI.Label(screenRect, Provider.offlineReason);
                }
                GUI.color = color2;
                flag5 = false;
            }
            else
            {
                if (this.m_ShowIncoming)
                {
                    flag5 |= this.incomingList.OnGUI(screenRect, base.hasFocus);
                }
                else
                {
                    flag5 |= this.pendingList.OnGUI(screenRect, base.hasFocus);
                }
                screenRect.y += screenRect.height;
                screenRect.height = 17f;
                GUI.Label(screenRect, GUIContent.none, s_Styles.bottomBarBg);
                GUIContent content2 = new GUIContent("Apply All Incoming Changes");
                Vector2 vector = EditorStyles.miniButton.CalcSize(content2);
                Rect rect = new Rect(screenRect.x, screenRect.y - 2f, (screenRect.width - vector.x) - 5f, screenRect.height);
                ProgressGUI(rect, Provider.activeTask, false);
                if (this.m_ShowIncoming)
                {
                    Rect position = screenRect;
                    position.width = vector.x;
                    position.height = vector.y;
                    position.y = screenRect.y + 2f;
                    position.x = (base.position.width - vector.x) - 5f;
                    EditorGUI.BeginDisabledGroup(this.incomingList.Size == 0);
                    if (GUI.Button(position, content2, EditorStyles.miniButton))
                    {
                        Asset asset = new Asset(string.Empty);
                        Provider.GetLatest(new AssetList { asset }).SetCompletionAction(CompletionAction.OnGotLatestPendingWindow);
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            if (flag5)
            {
                base.Repaint();
            }
        }

        private void OnIncoming(Task task)
        {
            this.CreateStaticResources();
            this.PopulateListControl(this.incomingList, task, this.syncIcon);
        }

        public void OnSelectionChange()
        {
            if (!base.hasFocus)
            {
                this.pendingList.Sync();
                base.Repaint();
            }
        }

        public static void OnStatusUpdated()
        {
            UpdateAllWindows();
        }

        private static void OnVCTaskCompletedEvent(Task task, CompletionAction completionAction)
        {
            WindowPending[] pendingArray = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
            foreach (WindowPending pending in pendingArray)
            {
                switch (completionAction)
                {
                    case CompletionAction.UpdatePendingWindow:
                    case CompletionAction.OnCheckoutCompleted:
                        pending.UpdateWindow();
                        break;

                    case CompletionAction.OnChangeContentsPendingWindow:
                        pending.OnChangeContents(task);
                        break;

                    case CompletionAction.OnIncomingPendingWindow:
                        pending.OnIncoming(task);
                        break;

                    case CompletionAction.OnChangeSetsPendingWindow:
                        pending.OnChangeSets(task);
                        break;

                    case CompletionAction.OnGotLatestPendingWindow:
                        pending.OnGotLatest(task);
                        break;
                }
            }
            switch (completionAction)
            {
                case CompletionAction.OnSubmittedChangeWindow:
                    WindowChange.OnSubmitted(task);
                    break;

                case CompletionAction.OnAddedChangeWindow:
                    WindowChange.OnAdded(task);
                    break;

                case CompletionAction.OnCheckoutCompleted:
                    if (EditorUserSettings.showFailedCheckout)
                    {
                        WindowCheckoutFailure.OpenIfCheckoutFailed(task.assetList);
                    }
                    break;
            }
            task.Dispose();
        }

        private void PopulateListControl(ListControl list, Task task, Texture2D icon)
        {
            ChangeSets changeSets = task.changeSets;
            ListItem firstChild = list.Root.FirstChild;
            while (firstChild != null)
            {
                <PopulateListControl>c__AnonStoreyB7 yb = new <PopulateListControl>c__AnonStoreyB7 {
                    cs = firstChild.Item as ChangeSet
                };
                if (changeSets.Find(new Predicate<ChangeSet>(yb.<>m__222)) == null)
                {
                    ListItem listItem = firstChild;
                    firstChild = firstChild.Next;
                    list.Root.Remove(listItem);
                }
                else
                {
                    firstChild = firstChild.Next;
                }
            }
            foreach (ChangeSet set in changeSets)
            {
                ListItem changeSetItem = list.GetChangeSetItem(set);
                if (changeSetItem != null)
                {
                    changeSetItem.Item = set;
                    changeSetItem.Name = set.description;
                }
                else
                {
                    changeSetItem = list.Add(null, set.description, set);
                }
                changeSetItem.Exclusive = true;
                changeSetItem.CanAccept = true;
                changeSetItem.Icon = icon;
            }
            list.Refresh();
            base.Repaint();
        }

        internal static bool ProgressGUI(Rect rect, Task activeTask, bool descriptionTextFirst)
        {
            if ((activeTask == null) || (((activeTask.progressPct == -1) && (activeTask.secondsSpent == -1)) && (activeTask.progressMessage.Length == 0)))
            {
                return false;
            }
            string progressMessage = activeTask.progressMessage;
            Rect position = rect;
            GUIContent statusWheel = StatusWheel;
            position.width = position.height;
            position.x += 4f;
            position.y += 4f;
            GUI.Label(position, statusWheel);
            rect.x += position.width + 4f;
            progressMessage = (progressMessage.Length != 0) ? progressMessage : activeTask.description;
            if (activeTask.progressPct == -1)
            {
                rect.width -= position.width + 4f;
                rect.y += 4f;
                GUI.Label(rect, progressMessage, EditorStyles.miniLabel);
            }
            else
            {
                rect.width = 120f;
                EditorGUI.ProgressBar(rect, (float) activeTask.progressPct, progressMessage);
            }
            return true;
        }

        public static void UpdateAllWindows()
        {
            WindowPending[] pendingArray = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
            foreach (WindowPending pending in pendingArray)
            {
                pending.UpdateWindow();
            }
        }

        private void UpdateWindow()
        {
            if (!Provider.isActive)
            {
                this.pendingList.Clear();
                Provider.UpdateSettings();
                base.Repaint();
            }
            else if (Provider.onlineState == OnlineState.Online)
            {
                Provider.ChangeSets().SetCompletionAction(CompletionAction.OnChangeSetsPendingWindow);
                Provider.Incoming().SetCompletionAction(CompletionAction.OnIncomingPendingWindow);
            }
        }

        internal static GUIContent StatusWheel
        {
            get
            {
                if (sStatusWheel == null)
                {
                    sStatusWheel = new GUIContent[12];
                    for (int i = 0; i < 12; i++)
                    {
                        GUIContent content = new GUIContent {
                            image = EditorGUIUtility.LoadIcon("WaitSpin" + i.ToString("00"))
                        };
                        content.image.hideFlags = HideFlags.HideAndDontSave;
                        content.image.name = "Spinner";
                        sStatusWheel[i] = content;
                    }
                }
                int index = (int) Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
                return sStatusWheel[index];
            }
        }

        [CompilerGenerated]
        private sealed class <GetEmptyChangeSetsCandidates>c__AnonStoreyB8
        {
            internal ChangeSets toDelete;

            internal void <>m__224(ChangeSet s)
            {
                this.toDelete.Add(s);
            }
        }

        [CompilerGenerated]
        private sealed class <PopulateListControl>c__AnonStoreyB7
        {
            internal ChangeSet cs;

            internal bool <>m__222(ChangeSet elm)
            {
                return (elm.id == this.cs.id);
            }
        }

        internal class Styles
        {
            public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
            public GUIStyle box = "CN Box";
        }
    }
}

