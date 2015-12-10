namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class GameObjectTreeViewGUI : TreeViewGUI
    {
        private float m_PrevScollPos;
        private float m_PrevTotalHeight;
        public Action mouseDownInTreeViewRect;
        protected static GameObjectStyles s_GOStyles;
        public Action scrollHeightChanged;
        public Action scrollPositionChanged;

        public GameObjectTreeViewGUI(TreeView treeView, bool useHorizontalScroll) : base(treeView, useHorizontalScroll)
        {
            base.k_TopRowMargin = 4f;
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
            if (item2 == null)
            {
                return false;
            }
            if (item2.isSceneHeader)
            {
                return false;
            }
            if ((item2.objectPPTR.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                Debug.LogWarning("Unable to rename a GameObject with HideFlags.NotEditable.");
                return false;
            }
            return base.BeginRename(item, delay);
        }

        public override void BeginRowGUI()
        {
            this.DetectScrollChange();
            this.DetectTotalRectChange();
            this.DetectMouseDownInTreeViewRect();
            base.BeginRowGUI();
            if (this.showingStickyHeaders && (Event.current.type != EventType.Repaint))
            {
                this.DoStickySceneHeaders();
            }
        }

        private void DetectMouseDownInTreeViewRect()
        {
            Event current = Event.current;
            if (((this.mouseDownInTreeViewRect != null) && (current.type == EventType.MouseDown)) && base.m_TreeView.GetTotalRect().Contains(current.mousePosition))
            {
                this.mouseDownInTreeViewRect();
            }
        }

        private void DetectScrollChange()
        {
            float y = base.m_TreeView.state.scrollPos.y;
            if ((this.scrollPositionChanged != null) && !Mathf.Approximately(y, this.m_PrevScollPos))
            {
                this.scrollPositionChanged();
            }
            this.m_PrevScollPos = y;
        }

        private void DetectTotalRectChange()
        {
            float height = base.m_TreeView.GetTotalRect().height;
            if ((this.scrollHeightChanged != null) && !Mathf.Approximately(height, this.m_PrevTotalHeight))
            {
                this.scrollHeightChanged();
            }
            this.m_PrevTotalHeight = height;
        }

        protected void DoAdditionalSceneHeaderGUI(GameObjectTreeViewItem goItem, Rect rect)
        {
            Rect position = new Rect((rect.width - 16f) - 4f, rect.y + ((rect.height - 6f) * 0.5f), 16f, rect.height);
            if (Event.current.type == EventType.Repaint)
            {
                s_GOStyles.optionsButtonStyle.Draw(position, false, false, false, false);
            }
            position.y = rect.y;
            position.height = rect.height;
            position.width = 24f;
            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
            {
                base.m_TreeView.SelectionClick(goItem, true);
                base.m_TreeView.contextClickItemCallback(goItem.id);
            }
        }

        protected override void DoNodeGUI(Rect rect, int row, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            GameObjectTreeViewItem goItem = item as GameObjectTreeViewItem;
            if (goItem != null)
            {
                if (goItem.isSceneHeader)
                {
                    Color color = GUI.color;
                    GUI.color *= new Color(1f, 1f, 1f, 0.9f);
                    GUI.Label(rect, GUIContent.none, s_GOStyles.sceneHeaderBg);
                    GUI.color = color;
                }
                base.DoNodeGUI(rect, row, item, selected, focused, useBoldFont);
                if (goItem.isSceneHeader)
                {
                    this.DoAdditionalSceneHeaderGUI(goItem, rect);
                }
                if (SceneHierarchyWindow.s_Debug)
                {
                    GUI.Label(new Rect(rect.xMax - 70f, rect.y, 70f, rect.height), string.Empty + row, EditorStyles.boldLabel);
                }
            }
        }

        private void DoStickySceneHeaders()
        {
            int num;
            int num2;
            this.GetFirstAndLastRowVisible(out num, out num2);
            if ((num >= 0) && (num2 >= 0))
            {
                <DoStickySceneHeaders>c__AnonStorey74 storey = new <DoStickySceneHeaders>c__AnonStorey74();
                float y = base.m_TreeView.state.scrollPos.y;
                if ((num != 0) || (y >= this.topRowMargin))
                {
                    storey.firstItem = (GameObjectTreeViewItem) base.m_TreeView.data.GetItem(num);
                    GameObjectTreeViewItem item = (GameObjectTreeViewItem) base.m_TreeView.data.GetItem(num + 1);
                    bool flag = storey.firstItem.scene != item.scene;
                    float width = GUIClip.visibleRect.width;
                    Rect rowRect = this.GetRowRect(num, width);
                    if (!storey.firstItem.isSceneHeader || !Mathf.Approximately(y, rowRect.y))
                    {
                        if (!flag)
                        {
                            rowRect.y = y;
                        }
                        GameObjectTreeViewItem item2 = ((GameObjectTreeViewDataSource) base.m_TreeView.data).sceneHeaderItems.FirstOrDefault<GameObjectTreeViewItem>(new Func<GameObjectTreeViewItem, bool>(storey.<>m__10D));
                        if (item2 != null)
                        {
                            bool selected = base.m_TreeView.IsItemDragSelectedOrSelected(item2);
                            bool focused = base.m_TreeView.HasFocus();
                            bool useBoldFont = item2.scene == SceneManager.GetActiveScene();
                            this.DoNodeGUI(rowRect, num, item2, selected, focused, useBoldFont);
                            if (GUI.Button(new Rect(rowRect.x, rowRect.y, rowRect.height, rowRect.height), GUIContent.none, GUIStyle.none))
                            {
                                base.m_TreeView.Frame(item2.id, true, false);
                            }
                            base.m_TreeView.HandleUnusedMouseEventsForNode(rowRect, item2, false);
                            this.HandleStickyHeaderContextClick(rowRect, item2);
                        }
                    }
                }
            }
        }

        protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
            if (item2 != null)
            {
                if (item2.isSceneHeader)
                {
                    if (item2.scene.isDirty)
                    {
                        label = label + "*";
                    }
                    if (!item2.scene.isLoaded)
                    {
                        label = label + " (not loaded)";
                    }
                    bool flag = item2.scene == SceneManager.GetActiveScene();
                    EditorGUI.BeginDisabledGroup(!item2.scene.isLoaded);
                    base.DrawIconAndLabel(rect, item, label, selected, focused, flag, isPinging);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    if (!isPinging)
                    {
                        float contentIndent = this.GetContentIndent(item);
                        rect.x += contentIndent;
                        rect.width -= contentIndent;
                    }
                    int colorCode = item2.colorCode;
                    if (string.IsNullOrEmpty(item.displayName))
                    {
                        if (item2.objectPPTR != null)
                        {
                            item2.displayName = item2.objectPPTR.name;
                        }
                        else
                        {
                            item2.displayName = "deleted gameobject";
                        }
                        label = item2.displayName;
                    }
                    GUIStyle lineStyle = TreeViewGUI.s_Styles.lineStyle;
                    if (!item2.shouldDisplay)
                    {
                        lineStyle = s_GOStyles.disabledLabel;
                    }
                    else if ((colorCode & 3) == 0)
                    {
                        lineStyle = (colorCode >= 4) ? s_GOStyles.disabledLabel : TreeViewGUI.s_Styles.lineStyle;
                    }
                    else if ((colorCode & 3) == 1)
                    {
                        lineStyle = (colorCode >= 4) ? s_GOStyles.disabledPrefabLabel : s_GOStyles.prefabLabel;
                    }
                    else if ((colorCode & 3) == 2)
                    {
                        lineStyle = (colorCode >= 4) ? s_GOStyles.disabledBrokenPrefabLabel : s_GOStyles.brokenPrefabLabel;
                    }
                    lineStyle.padding.left = (int) base.k_SpaceBetweenIconAndText;
                    lineStyle.Draw(rect, label, false, false, selected, focused);
                }
            }
        }

        public override void EndRowGUI()
        {
            base.EndRowGUI();
            if (this.showingStickyHeaders && (Event.current.type == EventType.Repaint))
            {
                this.DoStickySceneHeaders();
            }
        }

        public override Rect GetRectForFraming(int row)
        {
            Rect rectForFraming = base.GetRectForFraming(row);
            if (this.showingStickyHeaders && (row < base.m_TreeView.data.rowCount))
            {
                GameObjectTreeViewItem item = base.m_TreeView.data.GetItem(row) as GameObjectTreeViewItem;
                if ((item != null) && !item.isSceneHeader)
                {
                    rectForFraming.y -= base.k_LineHeight;
                    rectForFraming.height = 2f * base.k_LineHeight;
                }
            }
            return rectForFraming;
        }

        private void HandleStickyHeaderContextClick(Rect rect, GameObjectTreeViewItem sceneHeaderItem)
        {
            Event current = Event.current;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if ((((current.type == EventType.MouseDown) && (current.button == 1)) || (current.type == EventType.ContextClick)) && rect.Contains(Event.current.mousePosition))
                {
                    current.Use();
                    base.m_TreeView.contextClickItemCallback(sceneHeaderItem.id);
                }
            }
            else if (((Application.platform == RuntimePlatform.WindowsEditor) && (current.type == EventType.MouseDown)) && ((current.button == 1) && rect.Contains(Event.current.mousePosition)))
            {
                current.Use();
            }
        }

        protected override void InitStyles()
        {
            base.InitStyles();
            if (s_GOStyles == null)
            {
                s_GOStyles = new GameObjectStyles();
            }
        }

        public override void OnInitialize()
        {
            this.m_PrevScollPos = base.m_TreeView.state.scrollPos.y;
            this.m_PrevTotalHeight = base.m_TreeView.GetTotalRect().height;
        }

        protected override void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
            int userData = base.GetRenameOverlay().userData;
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                ObjectNames.SetNameSmartWithInstanceID(userData, name);
                TreeViewItem item = base.m_TreeView.data.FindItem(userData);
                if (item != null)
                {
                    item.displayName = name;
                }
                EditorApplication.RepaintAnimationWindow();
            }
        }

        private bool showingStickyHeaders
        {
            get
            {
                return (SceneManager.sceneCount > 1);
            }
        }

        [CompilerGenerated]
        private sealed class <DoStickySceneHeaders>c__AnonStorey74
        {
            internal GameObjectTreeViewItem firstItem;

            internal bool <>m__10D(GameObjectTreeViewItem p)
            {
                return (p.scene == this.firstItem.scene);
            }
        }

        private enum GameObjectColorType
        {
            Normal,
            Prefab,
            BrokenPrefab,
            Count
        }

        internal class GameObjectStyles
        {
            public GUIStyle brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
            public GUIStyle disabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel");
            public GUIStyle disabledLabel = new GUIStyle("PR DisabledLabel");
            public GUIStyle disabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel");
            public readonly int kSceneHeaderIconsInterval = 2;
            public GUIContent loadSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneLoadIn"), "Load scene");
            public GUIStyle optionsButtonStyle = "PaneOptions";
            public GUIStyle prefabLabel = new GUIStyle("PR PrefabLabel");
            public GUIContent saveSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneSave"), "Save scene");
            public GUIStyle sceneHeaderBg = "ProjectBrowserTopBarBg";
            public GUIContent unloadSceneGUIContent = new GUIContent(EditorGUIUtility.FindTexture("SceneLoadOut"), "Unload scene");

            public GameObjectStyles()
            {
                this.disabledLabel.alignment = TextAnchor.MiddleLeft;
                this.prefabLabel.alignment = TextAnchor.MiddleLeft;
                this.disabledPrefabLabel.alignment = TextAnchor.MiddleLeft;
                this.brokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
                this.disabledBrokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
            }
        }
    }
}

