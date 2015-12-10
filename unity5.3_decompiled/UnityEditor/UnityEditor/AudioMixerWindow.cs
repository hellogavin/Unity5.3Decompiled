namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Audio Mixer", icon="Audio Mixer")]
    internal class AudioMixerWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache1E;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache1F;
        private static string kAudioMixerUseRMSMetering = "AudioMixerUseRMSMetering";
        private const float kToolbarHeight = 17f;
        private List<AudioMixerController> m_AllControllers;
        private TreeViewState m_AudioGroupTreeState;
        private AudioMixerChannelStripView m_ChannelStripView;
        private AudioMixerChannelStripView.State m_ChannelStripViewState;
        private AudioMixerController m_Controller;
        private bool m_GroupsRenderedAboveSections;
        private AudioMixerGroupTreeView m_GroupTree;
        private AudioMixerGroupViewList m_GroupViews;
        [NonSerialized]
        private bool m_Initialized;
        private Vector2 m_LastSize;
        [SerializeField]
        private LayoutMode m_LayoutMode;
        [SerializeField]
        private Layout m_LayoutStripsOnRight;
        [SerializeField]
        private Layout m_LayoutStripsOnTop;
        private AudioMixersTreeView m_MixersTree;
        [SerializeField]
        private TreeViewState m_MixersTreeState;
        private int m_RepaintCounter;
        [SerializeField]
        private SectionType[] m_SectionOrder;
        private Vector2 m_SectionsScrollPosition;
        [SerializeField]
        private bool m_ShowBusConnections;
        [SerializeField]
        private bool m_ShowBusConnectionsOfSelection;
        [NonSerialized]
        private bool m_ShowDeveloperOverlays;
        [SerializeField]
        private bool m_ShowReferencedBuses;
        private AudioMixerSnapshotListView m_SnapshotListView;
        private ReorderableListWithRenameAndScrollView.State m_SnapshotState;
        [SerializeField]
        private bool m_SortGroupsAlphabetically;
        private readonly TickTimerHelper m_Ticker;
        private ReorderableListWithRenameAndScrollView.State m_ViewsState;
        private static GUIContents s_GuiContents;
        private static AudioMixerWindow s_Instance;

        public AudioMixerWindow()
        {
            SectionType[] typeArray1 = new SectionType[4];
            typeArray1[1] = SectionType.SnapshotList;
            typeArray1[2] = SectionType.GroupTree;
            typeArray1[3] = SectionType.ViewList;
            this.m_SectionOrder = typeArray1;
            this.m_LayoutMode = LayoutMode.Vertical;
            this.m_ShowReferencedBuses = true;
            this.m_SectionsScrollPosition = Vector2.zero;
            this.m_RepaintCounter = 2;
            this.m_GroupsRenderedAboveSections = true;
            this.m_Ticker = new TickTimerHelper(0.05);
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Sort groups alphabetically"), this.m_SortGroupsAlphabetically, (GenericMenu.MenuFunction) (() => (this.m_SortGroupsAlphabetically = !this.m_SortGroupsAlphabetically)));
            menu.AddItem(new GUIContent("Show referenced groups"), this.m_ShowReferencedBuses, (GenericMenu.MenuFunction) (() => (this.m_ShowReferencedBuses = !this.m_ShowReferencedBuses)));
            menu.AddItem(new GUIContent("Show group connections"), this.m_ShowBusConnections, (GenericMenu.MenuFunction) (() => (this.m_ShowBusConnections = !this.m_ShowBusConnections)));
            if (this.m_ShowBusConnections)
            {
                menu.AddItem(new GUIContent("Only highlight selected group connections"), this.m_ShowBusConnectionsOfSelection, (GenericMenu.MenuFunction) (() => (this.m_ShowBusConnectionsOfSelection = !this.m_ShowBusConnectionsOfSelection)));
            }
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Vertical layout"), this.layoutMode == LayoutMode.Vertical, (GenericMenu.MenuFunction) (() => (this.layoutMode = LayoutMode.Vertical)));
            menu.AddItem(new GUIContent("Horizontal layout"), this.layoutMode == LayoutMode.Horizontal, (GenericMenu.MenuFunction) (() => (this.layoutMode = LayoutMode.Horizontal)));
            menu.AddSeparator(string.Empty);
            if (<>f__am$cache1E == null)
            {
                <>f__am$cache1E = () => EditorPrefs.SetBool(kAudioMixerUseRMSMetering, true);
            }
            menu.AddItem(new GUIContent("Use RMS metering for display"), EditorPrefs.GetBool(kAudioMixerUseRMSMetering, true), <>f__am$cache1E);
            if (<>f__am$cache1F == null)
            {
                <>f__am$cache1F = () => EditorPrefs.SetBool(kAudioMixerUseRMSMetering, false);
            }
            menu.AddItem(new GUIContent("Use peak metering for display"), !EditorPrefs.GetBool(kAudioMixerUseRMSMetering, true), <>f__am$cache1F);
            if (Unsupported.IsDeveloperBuild())
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("DEVELOPER/Groups Rendered Above"), this.m_GroupsRenderedAboveSections, (GenericMenu.MenuFunction) (() => (this.m_GroupsRenderedAboveSections = !this.m_GroupsRenderedAboveSections)));
                menu.AddItem(new GUIContent("DEVELOPER/Build 10 groups"), false, () => this.m_Controller.BuildTestSetup(0, 7, 10));
                menu.AddItem(new GUIContent("DEVELOPER/Build 20 groups"), false, () => this.m_Controller.BuildTestSetup(0, 7, 20));
                menu.AddItem(new GUIContent("DEVELOPER/Build 40 groups"), false, () => this.m_Controller.BuildTestSetup(0, 7, 40));
                menu.AddItem(new GUIContent("DEVELOPER/Build 80 groups"), false, () => this.m_Controller.BuildTestSetup(0, 7, 80));
                menu.AddItem(new GUIContent("DEVELOPER/Build 160 groups"), false, () => this.m_Controller.BuildTestSetup(0, 7, 160));
                menu.AddItem(new GUIContent("DEVELOPER/Build chain of 10 groups"), false, () => this.m_Controller.BuildTestSetup(1, 1, 10));
                menu.AddItem(new GUIContent("DEVELOPER/Build chain of 20 groups "), false, () => this.m_Controller.BuildTestSetup(1, 1, 20));
                menu.AddItem(new GUIContent("DEVELOPER/Build chain of 40 groups"), false, () => this.m_Controller.BuildTestSetup(1, 1, 40));
                menu.AddItem(new GUIContent("DEVELOPER/Build chain of 80 groups"), false, () => this.m_Controller.BuildTestSetup(1, 1, 80));
                menu.AddItem(new GUIContent("DEVELOPER/Show overlays"), this.m_ShowDeveloperOverlays, (GenericMenu.MenuFunction) (() => (this.m_ShowDeveloperOverlays = !this.m_ShowDeveloperOverlays)));
            }
        }

        public void Awake()
        {
            this.m_AllControllers = FindAllAudioMixerControllers();
            if (this.m_MixersTreeState != null)
            {
                this.m_MixersTreeState.OnAwake();
                this.m_MixersTreeState.selectedIDs = new List<int>();
            }
        }

        private void ChangeSectionOrder(object userData)
        {
            Vector2 vector = (Vector2) userData;
            int x = (int) vector.x;
            int y = (int) vector.y;
            int index = Mathf.Clamp(x + y, 0, this.m_SectionOrder.Length - 1);
            if (index != x)
            {
                SectionType type = this.m_SectionOrder[x];
                this.m_SectionOrder[x] = this.m_SectionOrder[index];
                this.m_SectionOrder[index] = type;
            }
        }

        public static void Create()
        {
            Type[] desiredDockNextTo = new Type[] { typeof(ProjectBrowser) };
            AudioMixerWindow window = EditorWindow.GetWindow<AudioMixerWindow>(desiredDockNextTo);
            if (window.m_Pos.width < 400f)
            {
                window.m_Pos = new Rect(window.m_Pos.x, window.m_Pos.y, 800f, 450f);
            }
        }

        private void DetectControllerChange()
        {
            AudioMixerController controller = this.m_Controller;
            if (Selection.activeObject is AudioMixerController)
            {
                this.m_Controller = Selection.activeObject as AudioMixerController;
            }
            if (this.m_Controller != controller)
            {
                this.OnMixerControllerChanged();
            }
        }

        private void DoSections(Rect totalRectOfSections, Rect[] sectionRects, SectionType[] sectionOrder)
        {
            Event current = Event.current;
            bool flag = (this.m_Controller == null) || AudioMixerController.EditingTargetSnapshot();
            for (int i = 0; i < sectionOrder.Length; i++)
            {
                Rect rect = sectionRects[i];
                if (rect.height > 0f)
                {
                    switch (sectionOrder[i])
                    {
                        case SectionType.MixerTree:
                            this.m_MixersTree.OnGUI(rect);
                            break;

                        case SectionType.GroupTree:
                            this.m_GroupTree.OnGUI(rect);
                            break;

                        case SectionType.ViewList:
                            this.m_GroupViews.OnGUI(rect);
                            break;

                        case SectionType.SnapshotList:
                            EditorGUI.BeginDisabledGroup(!flag);
                            this.m_SnapshotListView.OnGUI(rect);
                            EditorGUI.EndDisabledGroup();
                            break;

                        default:
                            Debug.LogError("Unhandled enum value");
                            break;
                    }
                    if (current.type == EventType.ContextClick)
                    {
                        Rect rect2 = new Rect(rect.x, rect.y, rect.width - 15f, 22f);
                        if (rect2.Contains(current.mousePosition))
                        {
                            this.ReorderContextMenu(rect2, i);
                            current.Use();
                        }
                    }
                }
            }
        }

        private void DoToolbar()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, options);
            GUILayout.FlexibleSpace();
            if (this.m_Controller != null)
            {
                if (Application.isPlaying)
                {
                    Color backgroundColor = GUI.backgroundColor;
                    if (AudioSettings.editingInPlaymode)
                    {
                        GUI.backgroundColor = AnimationMode.animatedPropertyColor;
                    }
                    EditorGUI.BeginChangeCheck();
                    AudioSettings.editingInPlaymode = GUILayout.Toggle(AudioSettings.editingInPlaymode, s_GuiContents.editSnapShots, EditorStyles.toolbarButton, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        InspectorWindow.RepaintAllInspectors();
                    }
                    GUI.backgroundColor = backgroundColor;
                }
                GUILayout.FlexibleSpace();
                AudioMixerExposedParametersPopup.Popup(this.m_Controller, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void EndRenaming()
        {
            if (this.m_GroupTree != null)
            {
                this.m_GroupTree.EndRenaming();
            }
            if (this.m_MixersTree != null)
            {
                this.m_MixersTree.EndRenaming();
            }
        }

        private static List<AudioMixerController> FindAllAudioMixerControllers()
        {
            List<AudioMixerController> list = new List<AudioMixerController>();
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            SearchFilter filter = new SearchFilter();
            filter.classNames = new string[] { "AudioMixerController" };
            property.SetSearchFilter(filter);
            while (property.Next(null))
            {
                AudioMixerController pptrValue = property.pptrValue as AudioMixerController;
                if (pptrValue != null)
                {
                    list.Add(pptrValue);
                }
            }
            return list;
        }

        private List<AudioMixerController> GetAllControllers()
        {
            return this.m_AllControllers;
        }

        private Dictionary<AudioMixerEffectController, AudioMixerGroupController> GetEffectMap(List<AudioMixerGroupController> allGroups)
        {
            Dictionary<AudioMixerEffectController, AudioMixerGroupController> dictionary = new Dictionary<AudioMixerEffectController, AudioMixerGroupController>();
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerEffectController controller2 in controller.effects)
                {
                    dictionary[controller2] = controller;
                }
            }
            return dictionary;
        }

        private float GetHeightOfSection(SectionType sectionType)
        {
            switch (sectionType)
            {
                case SectionType.MixerTree:
                    return this.m_MixersTree.GetTotalHeight();

                case SectionType.GroupTree:
                    return this.m_GroupTree.GetTotalHeight();

                case SectionType.ViewList:
                    return this.m_GroupViews.GetTotalHeight();

                case SectionType.SnapshotList:
                    return this.m_SnapshotListView.GetTotalHeight();
            }
            Debug.LogError("Unhandled enum value");
            return 0f;
        }

        private void Init()
        {
            if (!this.m_Initialized)
            {
                if (this.m_LayoutStripsOnTop == null)
                {
                    this.m_LayoutStripsOnTop = new Layout();
                }
                if ((this.m_LayoutStripsOnTop.m_VerticalSplitter == null) || (this.m_LayoutStripsOnTop.m_VerticalSplitter.realSizes.Length != 2))
                {
                    int[] realSizes = new int[] { 0x41, 0x23 };
                    int[] minSizes = new int[] { 0x55, 0x69 };
                    this.m_LayoutStripsOnTop.m_VerticalSplitter = new SplitterState(realSizes, minSizes, null);
                }
                if ((this.m_LayoutStripsOnTop.m_HorizontalSplitter == null) || (this.m_LayoutStripsOnTop.m_HorizontalSplitter.realSizes.Length != 4))
                {
                    this.m_LayoutStripsOnTop.m_HorizontalSplitter = new SplitterState(new int[] { 60, 60, 60, 60 }, new int[] { 0x55, 0x55, 0x55, 0x55 }, null);
                }
                if (this.m_LayoutStripsOnRight == null)
                {
                    this.m_LayoutStripsOnRight = new Layout();
                }
                if ((this.m_LayoutStripsOnRight.m_HorizontalSplitter == null) || (this.m_LayoutStripsOnRight.m_HorizontalSplitter.realSizes.Length != 2))
                {
                    int[] numArray3 = new int[] { 30, 70 };
                    int[] numArray4 = new int[] { 160, 160 };
                    this.m_LayoutStripsOnRight.m_HorizontalSplitter = new SplitterState(numArray3, numArray4, null);
                }
                if ((this.m_LayoutStripsOnRight.m_VerticalSplitter == null) || (this.m_LayoutStripsOnRight.m_VerticalSplitter.realSizes.Length != 4))
                {
                    this.m_LayoutStripsOnRight.m_VerticalSplitter = new SplitterState(new int[] { 60, 60, 60, 60 }, new int[] { 100, 0x55, 0x55, 0x55 }, null);
                }
                if (this.m_AudioGroupTreeState == null)
                {
                    this.m_AudioGroupTreeState = new TreeViewState();
                }
                this.m_GroupTree = new AudioMixerGroupTreeView(this, this.m_AudioGroupTreeState);
                if (this.m_MixersTreeState == null)
                {
                    this.m_MixersTreeState = new TreeViewState();
                }
                this.m_MixersTree = new AudioMixersTreeView(this, this.m_MixersTreeState, new Func<List<AudioMixerController>>(this.GetAllControllers));
                if (this.m_ViewsState == null)
                {
                    this.m_ViewsState = new ReorderableListWithRenameAndScrollView.State();
                }
                this.m_GroupViews = new AudioMixerGroupViewList(this.m_ViewsState);
                if (this.m_SnapshotState == null)
                {
                    this.m_SnapshotState = new ReorderableListWithRenameAndScrollView.State();
                }
                this.m_SnapshotListView = new AudioMixerSnapshotListView(this.m_SnapshotState);
                if (this.m_ChannelStripViewState == null)
                {
                    this.m_ChannelStripViewState = new AudioMixerChannelStripView.State();
                }
                this.m_ChannelStripView = new AudioMixerChannelStripView(this.m_ChannelStripViewState);
                this.OnMixerControllerChanged();
                this.m_Initialized = true;
            }
        }

        private void LayoutWithStripsOnRightSideOneScrollBar(List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
        {
            SplitterState horizontalSplitter = this.m_LayoutStripsOnRight.m_HorizontalSplitter;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            SplitterGUILayout.BeginHorizontalSplit(horizontalSplitter, options);
            SplitterGUILayout.EndHorizontalSplit();
            float width = horizontalSplitter.realSizes[0];
            float num2 = base.position.width - width;
            Rect rect = new Rect(0f, 17f, width, base.position.height - 17f);
            Rect rect2 = new Rect(width, 17f, num2, rect.height);
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f, 0f) : new Color(0.19f, 0.19f, 0.19f));
            }
            float num3 = 10f;
            Rect[] source = new Rect[this.m_SectionOrder.Length];
            float y = 0f;
            for (int i = 0; i < this.m_SectionOrder.Length; i++)
            {
                y += num3;
                if (i > 0)
                {
                    y += source[i - 1].height;
                }
                source[i] = new Rect(0f, y, rect.width, this.GetHeightOfSection(this.m_SectionOrder[i]));
                source[i].x += 4f;
                source[i].width -= 8f;
            }
            Rect viewRect = new Rect(0f, 0f, 1f, source.Last<Rect>().yMax);
            if (viewRect.height > rect.height)
            {
                for (int j = 0; j < source.Length; j++)
                {
                    source[j].width -= 14f;
                }
            }
            this.m_SectionsScrollPosition = GUI.BeginScrollView(rect, this.m_SectionsScrollPosition, viewRect);
            this.DoSections(rect, source, this.m_SectionOrder);
            GUI.EndScrollView();
            this.m_ChannelStripView.OnGUI(rect2, this.m_ShowReferencedBuses, this.m_ShowBusConnections, this.m_ShowBusConnectionsOfSelection, allGroups, effectMap, this.m_SortGroupsAlphabetically, this.m_ShowDeveloperOverlays, this.m_GroupTree.ScrollToItem);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1f, 17f, 1f, base.position.height - 17f), !EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.15f, 0.15f, 0.15f));
        }

        private void LayoutWithStripsOnTop(List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
        {
            SplitterState horizontalSplitter = this.m_LayoutStripsOnTop.m_HorizontalSplitter;
            SplitterState verticalSplitter = this.m_LayoutStripsOnTop.m_VerticalSplitter;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            SplitterGUILayout.BeginVerticalSplit(verticalSplitter, options);
            if (this.m_GroupsRenderedAboveSections)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            SplitterGUILayout.BeginHorizontalSplit(horizontalSplitter, optionArray2);
            if (!this.m_GroupsRenderedAboveSections)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            SplitterGUILayout.EndHorizontalSplit();
            SplitterGUILayout.EndVerticalSplit();
            float y = !this.m_GroupsRenderedAboveSections ? (17f + verticalSplitter.realSizes[0]) : 17f;
            float height = !this.m_GroupsRenderedAboveSections ? ((float) verticalSplitter.realSizes[1]) : ((float) verticalSplitter.realSizes[0]);
            float num3 = this.m_GroupsRenderedAboveSections ? (17f + verticalSplitter.realSizes[0]) : 17f;
            float num4 = this.m_GroupsRenderedAboveSections ? ((float) verticalSplitter.realSizes[1]) : ((float) verticalSplitter.realSizes[0]);
            Rect rect = new Rect(0f, y, base.position.width, height);
            Rect totalRectOfSections = new Rect(0f, rect.yMax, base.position.width, base.position.height - rect.height);
            Rect[] sectionRects = new Rect[this.m_SectionOrder.Length];
            for (int i = 0; i < sectionRects.Length; i++)
            {
                float x = (i <= 0) ? 0f : sectionRects[i - 1].xMax;
                sectionRects[i] = new Rect(x, num3, (float) horizontalSplitter.realSizes[i], num4 - 12f);
            }
            sectionRects[0].x += 8f;
            sectionRects[0].width -= 12f;
            sectionRects[sectionRects.Length - 1].x += 4f;
            sectionRects[sectionRects.Length - 1].width -= 12f;
            for (int j = 1; j < (sectionRects.Length - 1); j++)
            {
                sectionRects[j].x += 4f;
                sectionRects[j].width -= 8f;
            }
            this.DoSections(totalRectOfSections, sectionRects, this.m_SectionOrder);
            this.m_ChannelStripView.OnGUI(rect, this.m_ShowReferencedBuses, this.m_ShowBusConnections, this.m_ShowBusConnectionsOfSelection, allGroups, effectMap, this.m_SortGroupsAlphabetically, this.m_ShowDeveloperOverlays, this.m_GroupTree.ScrollToItem);
            EditorGUI.DrawRect(new Rect(0f, (17f + verticalSplitter.realSizes[0]) - 1f, base.position.width, 1f), new Color(0f, 0f, 0f, 0.4f));
        }

        public void OnDisable()
        {
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.PlaymodeChanged));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
        }

        public void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            s_Instance = this;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.PlaymodeChanged));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
        }

        public void OnGUI()
        {
            List<AudioMixerGroupController> allAudioGroupsSlow;
            this.Init();
            if (s_GuiContents == null)
            {
                s_GuiContents = new GUIContents();
            }
            AudioMixerDrawUtils.InitStyles();
            this.DetectControllerChange();
            this.m_GroupViews.OnEvent();
            this.m_SnapshotListView.OnEvent();
            this.DoToolbar();
            if (this.m_Controller != null)
            {
                allAudioGroupsSlow = this.m_Controller.GetAllAudioGroupsSlow();
            }
            else
            {
                allAudioGroupsSlow = new List<AudioMixerGroupController>();
            }
            Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap = this.GetEffectMap(allAudioGroupsSlow);
            this.m_GroupTree.UseScrollView(this.m_LayoutMode == LayoutMode.Horizontal);
            if (this.m_LayoutMode == LayoutMode.Horizontal)
            {
                this.LayoutWithStripsOnTop(allAudioGroupsSlow, effectMap);
            }
            else
            {
                this.LayoutWithStripsOnRightSideOneScrollBar(allAudioGroupsSlow, effectMap);
            }
            if ((this.m_LastSize.x != base.position.width) || (this.m_LastSize.y != base.position.height))
            {
                this.m_RepaintCounter = 2;
                this.m_LastSize = new Vector2(base.position.width, base.position.height);
            }
            this.RepaintIfNeeded();
        }

        private void OnLostFocus()
        {
            this.EndRenaming();
        }

        private void OnMixerControllerChanged()
        {
            if (this.m_Controller != null)
            {
                this.m_Controller.ClearEventHandlers();
            }
            this.m_MixersTree.OnMixerControllerChanged(this.m_Controller);
            this.m_GroupTree.OnMixerControllerChanged(this.m_Controller);
            this.m_GroupViews.OnMixerControllerChanged(this.m_Controller);
            this.m_ChannelStripView.OnMixerControllerChanged(this.m_Controller);
            this.m_SnapshotListView.OnMixerControllerChanged(this.m_Controller);
            if (this.m_Controller != null)
            {
                this.m_Controller.ForceSetView(this.m_Controller.currentViewIndex);
            }
        }

        private void OnProjectChanged()
        {
            if (this.m_MixersTree == null)
            {
                this.Init();
            }
            this.m_AllControllers = FindAllAudioMixerControllers();
            this.m_MixersTree.ReloadTree();
        }

        private void OnSelectionChange()
        {
            if (this.m_Controller != null)
            {
                this.m_Controller.OnUnitySelectionChanged();
            }
            if (this.m_GroupTree != null)
            {
                this.m_GroupTree.InitSelection(true);
            }
            base.Repaint();
        }

        public MixerParameterDefinition ParamDef(string name, string desc, string units, float displayScale, float minRange, float maxRange, float defaultValue)
        {
            return new MixerParameterDefinition { name = name, description = desc, units = units, displayScale = displayScale, minRange = minRange, maxRange = maxRange, defaultValue = defaultValue };
        }

        private void PlaymodeChanged()
        {
            this.m_Ticker.Reset();
            if (this.m_Controller != null)
            {
                base.Repaint();
            }
            this.EndRenaming();
        }

        private void ReorderContextMenu(Rect rect, int sectionIndex)
        {
            Event current = Event.current;
            if ((Event.current.type == EventType.ContextClick) && rect.Contains(current.mousePosition))
            {
                GUIContent content = new GUIContent((this.m_LayoutMode != LayoutMode.Horizontal) ? "Move Up" : "Move Left");
                GUIContent content2 = new GUIContent((this.m_LayoutMode != LayoutMode.Horizontal) ? "Move Down" : "Move Right");
                GenericMenu menu = new GenericMenu();
                if (sectionIndex > 1)
                {
                    menu.AddItem(content, false, new GenericMenu.MenuFunction2(this.ChangeSectionOrder), new Vector2((float) sectionIndex, -1f));
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
                if ((sectionIndex > 0) && (sectionIndex < (this.m_SectionOrder.Length - 1)))
                {
                    menu.AddItem(content2, false, new GenericMenu.MenuFunction2(this.ChangeSectionOrder), new Vector2((float) sectionIndex, 1f));
                }
                else
                {
                    menu.AddDisabledItem(content2);
                }
                menu.ShowAsContext();
            }
        }

        public static void RepaintAudioMixerWindow()
        {
            if (s_Instance != null)
            {
                s_Instance.Repaint();
            }
        }

        private void RepaintIfNeeded()
        {
            if (this.m_RepaintCounter > 0)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_RepaintCounter--;
                }
                base.Repaint();
            }
        }

        public void UndoRedoPerformed()
        {
            if (this.m_Controller != null)
            {
                this.m_Controller.SanitizeGroupViews();
                this.m_Controller.OnUnitySelectionChanged();
                this.m_Controller.OnSubAssetChanged();
                if (this.m_GroupTree != null)
                {
                    this.m_GroupTree.OnUndoRedoPerformed();
                }
                if (this.m_GroupViews != null)
                {
                    this.m_GroupViews.OnUndoRedoPerformed();
                }
                if (this.m_SnapshotListView != null)
                {
                    this.m_SnapshotListView.OnUndoRedoPerformed();
                }
                if (this.m_MixersTree != null)
                {
                    this.m_MixersTree.OnUndoRedoPerformed();
                }
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }
        }

        public void Update()
        {
            if (this.m_Ticker.DoTick() && (EditorApplication.isPlaying || ((this.m_ChannelStripView != null) && this.m_ChannelStripView.requiresRepaint)))
            {
                base.Repaint();
            }
        }

        public AudioMixerController controller
        {
            get
            {
                return this.m_Controller;
            }
        }

        private LayoutMode layoutMode
        {
            get
            {
                return this.m_LayoutMode;
            }
            set
            {
                this.m_LayoutMode = value;
                this.m_RepaintCounter = 2;
            }
        }

        private class GUIContents
        {
            public GUIContent editSnapShots = new GUIContent("Edit in Play Mode", EditorGUIUtility.IconContent("Animation.Record", "|Are scene and inspector changes recorded into the animation curves?").image, "Edit in playmode and your changes are automatically saved. Note when editting is disabled then live values are shown.");
            public GUIContent infoText = new GUIContent("Create an AudioMixer asset from the Project Browser to get started");
            public GUIStyle mixerHeader = new GUIStyle(EditorStyles.largeLabel);
            public GUIContent output = new GUIContent("Output", "Select an Audio Mixer Group from another Audio Mixer to output to. If 'None' is selected then output is routed directly to the Audio Listener.");
            public GUIContent rms = new GUIContent("RMS", "Switches between RMS (Root Mean Square) metering and peak metering. RMS is closer to the energy level and perceived loudness of the sound (hence lower than the peak meter), while peak-metering is useful for monitoring spikes in the signal that can cause clipping.");
            public GUIContent selectAudioMixer = new GUIContent(string.Empty, "Select an Audio Mixer");
            public GUIStyle toolbarLabel = new GUIStyle(EditorStyles.miniLabel);
            public GUIStyle toolbarObjectField = new GUIStyle("ShurikenObjectField");

            public GUIContents()
            {
                this.toolbarLabel.alignment = TextAnchor.MiddleLeft;
                this.toolbarObjectField.normal.textColor = this.toolbarLabel.normal.textColor;
                this.mixerHeader.fontStyle = FontStyle.Bold;
                this.mixerHeader.fontSize = 0x11;
                this.mixerHeader.margin = new RectOffset();
                this.mixerHeader.padding = new RectOffset();
                this.mixerHeader.alignment = TextAnchor.MiddleLeft;
                if (!EditorGUIUtility.isProSkin)
                {
                    this.mixerHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
                else
                {
                    this.mixerHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
            }
        }

        [Serializable]
        private class Layout
        {
            [SerializeField]
            public SplitterState m_HorizontalSplitter;
            [SerializeField]
            public SplitterState m_VerticalSplitter;
        }

        public enum LayoutMode
        {
            Horizontal,
            Vertical
        }

        private enum SectionType
        {
            MixerTree,
            GroupTree,
            ViewList,
            SnapshotList
        }
    }
}

