namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    internal class ObjectSelector : EditorWindow
    {
        private const float kMinTopSize = 250f;
        private const float kMinWidth = 200f;
        private const float kPreviewExpandedAreaHeight = 75f;
        private const float kPreviewMargin = 5f;
        private List<int> m_AllowedIDs;
        private bool m_AllowSceneObjects;
        private GUIView m_DelegateView;
        private EditorCache m_EditorCache;
        private bool m_FocusSearchFilter;
        private bool m_IsShowingAssets;
        private ObjectListArea m_ListArea;
        private ObjectListAreaState m_ListAreaState;
        private int m_ModalUndoGroup = -1;
        private ObjectTreeForSelector m_ObjectTreeWithSearch = new ObjectTreeForSelector();
        private Object m_OriginalSelection;
        private PreviewResizer m_PreviewResizer = new PreviewResizer();
        private float m_PreviewSize;
        private string m_RequiredType;
        private string m_SearchFilter;
        private AnimBool m_ShowOverlapPreview = new AnimBool();
        private AnimBool m_ShowWidePreview = new AnimBool();
        private SavedInt m_StartGridSize = new SavedInt("ObjectSelector.GridSize", 0x40);
        private Styles m_Styles;
        private float m_ToolbarHeight = 44f;
        private float m_TopSize;
        internal int objectSelectorID;
        private static ObjectSelector s_SharedObjectSelector;

        private ObjectSelector()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        private void Cancel()
        {
            Undo.RevertAllDownToGroup(this.m_ModalUndoGroup);
            base.Close();
            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        private void CreateAndSetTreeView(ObjectTreeForSelector.TreeSelectorData data)
        {
            TreeViewForAudioMixerGroup.CreateAndSetTreeView(data);
        }

        private void DrawObjectIcon(Rect position, Texture icon)
        {
            if (icon != null)
            {
                int num = Mathf.Min((int) position.width, (int) position.height);
                if (num >= (icon.width * 2))
                {
                    num = icon.width * 2;
                }
                FilterMode filterMode = icon.filterMode;
                icon.filterMode = FilterMode.Point;
                GUI.DrawTexture(new Rect(position.x + ((((int) position.width) - num) / 2), position.y + ((((int) position.height) - num) / 2), (float) num, (float) num), icon, ScaleMode.ScaleToFit);
                icon.filterMode = filterMode;
            }
        }

        private void FilterSettingsChanged()
        {
            SearchFilter searchFilter = new SearchFilter();
            searchFilter.SearchFieldStringToFilter(this.m_SearchFilter);
            if (!string.IsNullOrEmpty(this.m_RequiredType))
            {
                searchFilter.classNames = new string[] { this.m_RequiredType };
            }
            this.m_ListArea.Init(this.listPosition, !this.m_IsShowingAssets ? HierarchyType.GameObjects : HierarchyType.Assets, searchFilter, true);
        }

        public static Object GetCurrentObject()
        {
            return EditorUtility.InstanceIDToObject(get.GetSelectedInstanceID());
        }

        public static Object GetInitialObject()
        {
            return get.m_OriginalSelection;
        }

        private int GetSelectedInstanceID()
        {
            int[] numArray = !this.IsUsingTreeView() ? this.m_ListArea.GetSelection() : this.m_ObjectTreeWithSearch.GetSelection();
            if (numArray.Length >= 1)
            {
                return numArray[0];
            }
            return 0;
        }

        private void GridListArea()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_ListArea.OnGUI(this.listPosition, controlID);
        }

        private static bool GuessIfUserIsLookingForAnAsset(string requiredClassName, bool checkGameObject)
        {
            string[] strArray = new string[] { 
                "AnimationClip", "AnimatorController", "AnimatorOverrideController", "AudioClip", "Avatar", "Flare", "Font", "Material", "ProceduralMaterial", "Mesh", "PhysicMaterial", "GUISkin", "Shader", "TerrainData", "Texture", "Cubemap", 
                "MovieTexture", "RenderTexture", "Texture2D", "ProceduralTexture", "Sprite", "AudioMixerGroup", "AudioMixerSnapshot"
             };
            if (checkGameObject && (requiredClassName == "GameObject"))
            {
                return true;
            }
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == requiredClassName)
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        base.Close();
                        GUI.changed = true;
                        GUIUtility.ExitGUI();
                        Event.current.Use();
                        GUI.changed = true;
                        break;
                }
            }
        }

        private void InitIfNeeded()
        {
            if (this.m_ListAreaState == null)
            {
                this.m_ListAreaState = new ObjectListAreaState();
            }
            if (this.m_ListArea == null)
            {
                this.m_ListArea = new ObjectListArea(this.m_ListAreaState, this, true);
                this.m_ListArea.allowDeselection = false;
                this.m_ListArea.allowDragging = false;
                this.m_ListArea.allowFocusRendering = false;
                this.m_ListArea.allowMultiSelect = false;
                this.m_ListArea.allowRenaming = false;
                this.m_ListArea.allowBuiltinResources = true;
                this.m_ListArea.repaintCallback = (Action) Delegate.Combine(this.m_ListArea.repaintCallback, new Action(this.Repaint));
                this.m_ListArea.itemSelectedCallback = (Action<bool>) Delegate.Combine(this.m_ListArea.itemSelectedCallback, new Action<bool>(this.ListAreaItemSelectedCallback));
                this.m_ListArea.gridSize = this.m_StartGridSize.value;
                SearchFilter searchFilter = new SearchFilter {
                    nameFilter = this.m_SearchFilter
                };
                if (!string.IsNullOrEmpty(this.m_RequiredType))
                {
                    searchFilter.classNames = new string[] { this.m_RequiredType };
                }
                this.m_ListArea.Init(this.listPosition, !this.m_IsShowingAssets ? HierarchyType.GameObjects : HierarchyType.Assets, searchFilter, true);
            }
        }

        private bool IsUsingTreeView()
        {
            return this.m_ObjectTreeWithSearch.IsInitialized();
        }

        private void ItemWasDoubleClicked()
        {
            base.Close();
            GUIUtility.ExitGUI();
        }

        private void LinePreview(string s, Object o, EditorWrapper p)
        {
            if (this.m_ListArea.m_SelectedObjectIcon != null)
            {
                GUI.DrawTexture(new Rect(2f, (float) ((int) (this.m_TopSize + 2f)), 16f, 16f), this.m_ListArea.m_SelectedObjectIcon, ScaleMode.StretchToFill);
            }
            Rect position = new Rect(20f, this.m_TopSize + 1f, base.position.width - 22f, 18f);
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DropShadowLabel(position, s, this.m_Styles.smallStatus);
            }
            else
            {
                GUI.Label(position, s, this.m_Styles.smallStatus);
            }
        }

        private void ListAreaItemSelectedCallback(bool doubleClicked)
        {
            if (doubleClicked)
            {
                this.ItemWasDoubleClicked();
            }
            else
            {
                this.m_FocusSearchFilter = false;
                this.SendEvent("ObjectSelectorUpdated", true);
            }
        }

        private void OnDestroy()
        {
            if (this.m_ListArea != null)
            {
                this.m_ListArea.OnDestroy();
            }
            this.m_ObjectTreeWithSearch.Clear();
        }

        private void OnDisable()
        {
            this.SendEvent("ObjectSelectorClosed", false);
            if (this.m_ListArea != null)
            {
                this.m_StartGridSize.value = this.m_ListArea.gridSize;
            }
            Undo.CollapseUndoOperations(this.m_ModalUndoGroup);
            if (s_SharedObjectSelector == this)
            {
                s_SharedObjectSelector = null;
            }
            if (this.m_EditorCache != null)
            {
                this.m_EditorCache.Dispose();
            }
            AssetPreview.ClearTemporaryAssetPreviews();
        }

        private void OnEnable()
        {
            this.m_ShowOverlapPreview.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowOverlapPreview.speed = 1.5f;
            this.m_ShowWidePreview.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowWidePreview.speed = 1.5f;
            this.m_PreviewResizer.Init("ObjectPickerPreview");
            this.m_PreviewSize = this.m_PreviewResizer.GetPreviewSize();
            AssetPreview.ClearTemporaryAssetPreviews();
            this.SetupPreview();
        }

        private void OnGUI()
        {
            this.HandleKeyboard();
            if (this.m_ObjectTreeWithSearch.IsInitialized())
            {
                this.OnObjectTreeGUI();
            }
            else
            {
                this.OnObjectGridGUI();
            }
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                this.Cancel();
            }
        }

        private void OnInspectorUpdate()
        {
            if ((this.m_ListArea != null) && AssetPreview.HasAnyNewPreviewTexturesAvailable(this.m_ListArea.GetAssetPreviewManagerID()))
            {
                base.Repaint();
            }
        }

        private void OnObjectGridGUI()
        {
            this.InitIfNeeded();
            if (this.m_Styles == null)
            {
                this.m_Styles = new Styles();
            }
            if (this.m_EditorCache == null)
            {
                this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
            }
            this.ResizeBottomPartOfWindow();
            Rect position = base.position;
            EditorPrefs.SetFloat("ObjectSelectorWidth", position.width);
            EditorPrefs.SetFloat("ObjectSelectorHeight", position.height);
            GUI.BeginGroup(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none);
            this.m_ListArea.HandleKeyboard(false);
            this.SearchArea();
            this.GridListArea();
            this.PreviewArea();
            GUI.EndGroup();
            GUI.Label(new Rect((base.position.width * 0.5f) - 16f, (base.position.height - this.m_PreviewSize) + 2f, 32f, this.m_Styles.bottomResize.fixedHeight), GUIContent.none, this.m_Styles.bottomResize);
        }

        private void OnObjectTreeGUI()
        {
            this.m_ObjectTreeWithSearch.OnGUI(new Rect(0f, 0f, base.position.width, base.position.height));
        }

        private void OverlapPreview(float actualSize, string s, Object o, EditorWrapper p)
        {
            float x = 5f;
            Rect position = new Rect(x, this.m_TopSize + x, base.position.width - (x * 2f), actualSize - (x * 2f));
            if ((p != null) && p.HasPreviewGUI())
            {
                p.OnPreviewGUI(position, this.m_Styles.previewTextureBackground);
            }
            else if (o != null)
            {
                this.DrawObjectIcon(position, this.m_ListArea.m_SelectedObjectIcon);
            }
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DropShadowLabel(position, s, this.m_Styles.largeStatus);
            }
            else
            {
                EditorGUI.DoDropShadowLabel(position, EditorGUIUtility.TempContent(s), this.m_Styles.largeStatus, 0.3f);
            }
        }

        private void PreviewArea()
        {
            GUI.Box(new Rect(0f, this.m_TopSize, base.position.width, this.m_PreviewSize), string.Empty, this.m_Styles.previewBackground);
            if (this.m_ListArea.GetSelection().Length != 0)
            {
                EditorWrapper p = null;
                Object currentObject = GetCurrentObject();
                if (this.m_PreviewSize < 75f)
                {
                    string str;
                    if (currentObject != null)
                    {
                        p = this.m_EditorCache[currentObject];
                        string str2 = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
                        if (p != null)
                        {
                            str = p.name + " (" + str2 + ")";
                        }
                        else
                        {
                            str = currentObject.name + " (" + str2 + ")";
                        }
                        str = str + "      " + AssetDatabase.GetAssetPath(currentObject);
                    }
                    else
                    {
                        str = "None";
                    }
                    this.LinePreview(str, currentObject, p);
                }
                else
                {
                    string infoString;
                    if (this.m_EditorCache == null)
                    {
                        this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
                    }
                    if (currentObject != null)
                    {
                        p = this.m_EditorCache[currentObject];
                        string str4 = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
                        if (p != null)
                        {
                            infoString = p.GetInfoString();
                            if (infoString != string.Empty)
                            {
                                string[] textArray1 = new string[] { p.name, "\n", str4, "\n", infoString };
                                infoString = string.Concat(textArray1);
                            }
                            else
                            {
                                infoString = p.name + "\n" + str4;
                            }
                        }
                        else
                        {
                            infoString = currentObject.name + "\n" + str4;
                        }
                        infoString = infoString + "\n" + AssetDatabase.GetAssetPath(currentObject);
                    }
                    else
                    {
                        infoString = "None";
                    }
                    if (this.m_ShowWidePreview.faded != 0f)
                    {
                        GUI.color = new Color(1f, 1f, 1f, this.m_ShowWidePreview.faded);
                        this.WidePreview(this.m_PreviewSize, infoString, currentObject, p);
                    }
                    if (this.m_ShowOverlapPreview.faded != 0f)
                    {
                        GUI.color = new Color(1f, 1f, 1f, this.m_ShowOverlapPreview.faded);
                        this.OverlapPreview(this.m_PreviewSize, infoString, currentObject, p);
                    }
                    GUI.color = Color.white;
                    this.m_EditorCache.CleanupUntouchedEditors();
                }
            }
        }

        private bool PreviewIsOpen()
        {
            return (this.m_PreviewSize >= 37f);
        }

        private bool PreviewIsWide()
        {
            return (((base.position.width - this.m_PreviewSize) - 5f) > Mathf.Min((float) ((this.m_PreviewSize * 2f) - 20f), (float) 256f));
        }

        private void ResizeBottomPartOfWindow()
        {
            GUI.changed = false;
            this.m_PreviewSize = this.m_PreviewResizer.ResizeHandle(base.position, 65f, 270f, 20f) + 20f;
            this.m_TopSize = base.position.height - this.m_PreviewSize;
            bool flag = this.PreviewIsOpen();
            bool flag2 = this.PreviewIsWide();
            this.m_ShowOverlapPreview.target = flag && !flag2;
            this.m_ShowWidePreview.target = flag && flag2;
        }

        private void SearchArea()
        {
            GUI.Label(new Rect(0f, 0f, base.position.width, this.m_ToolbarHeight), GUIContent.none, this.m_Styles.toolbarBack);
            bool flag = (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape);
            GUI.SetNextControlName("SearchFilter");
            string str = EditorGUI.SearchField(new Rect(5f, 5f, base.position.width - 10f, 15f), this.m_SearchFilter);
            if (flag && (Event.current.type == EventType.Used))
            {
                if (this.m_SearchFilter == string.Empty)
                {
                    this.Cancel();
                }
                this.m_FocusSearchFilter = true;
            }
            if ((str != this.m_SearchFilter) || this.m_FocusSearchFilter)
            {
                this.m_SearchFilter = str;
                this.FilterSettingsChanged();
                base.Repaint();
            }
            if (this.m_FocusSearchFilter)
            {
                EditorGUI.FocusTextInControl("SearchFilter");
                this.m_FocusSearchFilter = false;
            }
            GUI.changed = false;
            GUILayout.BeginArea(new Rect(0f, 26f, base.position.width, this.m_ToolbarHeight - 26f));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool flag2 = GUILayout.Toggle(this.m_IsShowingAssets, "Assets", this.m_Styles.tab, new GUILayoutOption[0]);
            if (!this.m_IsShowingAssets && flag2)
            {
                this.m_IsShowingAssets = true;
            }
            if (!this.m_AllowSceneObjects)
            {
                GUI.enabled = false;
                GUI.color = new Color(1f, 1f, 1f, 0f);
            }
            bool flag3 = !this.m_IsShowingAssets;
            flag3 = GUILayout.Toggle(flag3, "Scene", this.m_Styles.tab, new GUILayoutOption[0]);
            if (this.m_IsShowingAssets && flag3)
            {
                this.m_IsShowingAssets = false;
            }
            if (!this.m_AllowSceneObjects)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            if (GUI.changed)
            {
                this.FilterSettingsChanged();
            }
            if (this.m_ListArea.CanShowThumbnails())
            {
                this.m_ListArea.gridSize = (int) GUI.HorizontalSlider(new Rect(base.position.width - 60f, 26f, 55f, this.m_ToolbarHeight - 28f), (float) this.m_ListArea.gridSize, (float) this.m_ListArea.minGridSize, (float) this.m_ListArea.maxGridSize);
            }
        }

        private void SendEvent(string eventName, bool exitGUI)
        {
            if (this.m_DelegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent(eventName);
                try
                {
                    this.m_DelegateView.SendEvent(e);
                }
                finally
                {
                }
                if (exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        public void SetupPreview()
        {
            bool flag = this.PreviewIsOpen();
            bool flag2 = this.PreviewIsWide();
            bool flag3 = flag && !flag2;
            this.m_ShowOverlapPreview.value = flag3;
            this.m_ShowOverlapPreview.target = flag3;
            flag3 = flag && flag2;
            this.m_ShowWidePreview.value = flag3;
            this.m_ShowWidePreview.target = flag3;
        }

        private static bool ShouldTreeViewBeUsed(string className)
        {
            return (className == "AudioMixerGroup");
        }

        public void Show(Object obj, Type requiredType, SerializedProperty property, bool allowSceneObjects)
        {
            this.Show(obj, requiredType, property, allowSceneObjects, null);
        }

        internal void Show(Object obj, Type requiredType, SerializedProperty property, bool allowSceneObjects, List<int> allowedInstanceIDs)
        {
            this.m_AllowSceneObjects = allowSceneObjects;
            this.m_IsShowingAssets = true;
            this.m_AllowedIDs = allowedInstanceIDs;
            string requiredClassName = string.Empty;
            if (property != null)
            {
                requiredClassName = property.objectReferenceTypeString;
                obj = property.objectReferenceValue;
                Object targetObject = property.serializedObject.targetObject;
                if ((targetObject != null) && EditorUtility.IsPersistent(targetObject))
                {
                    this.m_AllowSceneObjects = false;
                }
            }
            else if (requiredType != null)
            {
                requiredClassName = requiredType.Name;
            }
            if (this.m_AllowSceneObjects)
            {
                if (obj != null)
                {
                    if (typeof(Component).IsAssignableFrom(obj.GetType()))
                    {
                        obj = ((Component) obj).gameObject;
                    }
                    this.m_IsShowingAssets = EditorUtility.IsPersistent(obj) || GuessIfUserIsLookingForAnAsset(requiredClassName, false);
                }
                else
                {
                    this.m_IsShowingAssets = GuessIfUserIsLookingForAnAsset(requiredClassName, true);
                }
            }
            else
            {
                this.m_IsShowingAssets = true;
            }
            this.m_DelegateView = GUIView.current;
            this.m_RequiredType = requiredClassName;
            this.m_SearchFilter = string.Empty;
            this.m_OriginalSelection = obj;
            this.m_ModalUndoGroup = Undo.GetCurrentGroup();
            ContainerWindow.SetFreezeDisplay(true);
            base.ShowWithMode(ShowMode.AuxWindow);
            base.titleContent = new GUIContent("Select " + requiredClassName);
            Rect position = base.m_Parent.window.position;
            position.width = EditorPrefs.GetFloat("ObjectSelectorWidth", 200f);
            position.height = EditorPrefs.GetFloat("ObjectSelectorHeight", 390f);
            base.position = position;
            base.minSize = new Vector2(200f, 335f);
            base.maxSize = new Vector2(10000f, 10000f);
            this.SetupPreview();
            base.Focus();
            ContainerWindow.SetFreezeDisplay(false);
            this.m_FocusSearchFilter = true;
            base.m_Parent.AddToAuxWindowList();
            int initialSelectedTreeViewItemID = (obj == null) ? 0 : obj.GetInstanceID();
            if ((property != null) && property.hasMultipleDifferentValues)
            {
                initialSelectedTreeViewItemID = 0;
            }
            if (ShouldTreeViewBeUsed(requiredClassName))
            {
                this.m_ObjectTreeWithSearch.Init(base.position, this, new UnityAction<ObjectTreeForSelector.TreeSelectorData>(this.CreateAndSetTreeView), new UnityAction<TreeViewItem>(this.TreeViewSelection), new UnityAction(this.ItemWasDoubleClicked), initialSelectedTreeViewItemID, 0);
            }
            else
            {
                this.InitIfNeeded();
                int[] selectedInstanceIDs = new int[] { initialSelectedTreeViewItemID };
                this.m_ListArea.InitSelection(selectedInstanceIDs);
                if (initialSelectedTreeViewItemID != 0)
                {
                    this.m_ListArea.Frame(initialSelectedTreeViewItemID, true, false);
                }
            }
        }

        private void TreeViewSelection(TreeViewItem item)
        {
            this.SendEvent("ObjectSelectorUpdated", true);
        }

        private void WidePreview(float actualSize, string s, Object o, EditorWrapper p)
        {
            float x = 5f;
            Rect position = new Rect(x, this.m_TopSize + x, actualSize - (x * 2f), actualSize - (x * 2f));
            Rect rect2 = new Rect(this.m_PreviewSize + 3f, this.m_TopSize + ((this.m_PreviewSize - 75f) * 0.5f), ((base.m_Parent.window.position.width - this.m_PreviewSize) - 3f) - x, 75f);
            if ((p != null) && p.HasPreviewGUI())
            {
                p.OnPreviewGUI(position, this.m_Styles.previewTextureBackground);
            }
            else if (o != null)
            {
                this.DrawObjectIcon(position, this.m_ListArea.m_SelectedObjectIcon);
            }
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DropShadowLabel(rect2, s, this.m_Styles.smallStatus);
            }
            else
            {
                GUI.Label(rect2, s, this.m_Styles.smallStatus);
            }
        }

        public List<int> allowedInstanceIDs
        {
            get
            {
                return this.m_AllowedIDs;
            }
        }

        public static ObjectSelector get
        {
            get
            {
                if (s_SharedObjectSelector == null)
                {
                    Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(ObjectSelector));
                    if ((objArray != null) && (objArray.Length > 0))
                    {
                        s_SharedObjectSelector = (ObjectSelector) objArray[0];
                    }
                    if (s_SharedObjectSelector == null)
                    {
                        s_SharedObjectSelector = ScriptableObject.CreateInstance<ObjectSelector>();
                    }
                }
                return s_SharedObjectSelector;
            }
        }

        public static bool isVisible
        {
            get
            {
                return (s_SharedObjectSelector != null);
            }
        }

        private Rect listPosition
        {
            get
            {
                return new Rect(0f, this.m_ToolbarHeight, base.position.width, Mathf.Max((float) 0f, (float) (this.m_TopSize - this.m_ToolbarHeight)));
            }
        }

        internal string searchFilter
        {
            get
            {
                return this.m_SearchFilter;
            }
            set
            {
                this.m_SearchFilter = value;
                this.FilterSettingsChanged();
            }
        }

        private class Styles
        {
            public GUIStyle background = "ObjectPickerBackground";
            public GUIStyle bottomResize = "WindowBottomResize";
            public GUIStyle largeStatus = "ObjectPickerLargeStatus";
            public GUIStyle previewBackground = "PopupCurveSwatchBackground";
            public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";
            public GUIStyle smallStatus = "ObjectPickerSmallStatus";
            public GUIStyle tab = "ObjectPickerTab";
            public GUIStyle toolbarBack = "ObjectPickerToolbar";
        }
    }
}

