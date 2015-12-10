namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AnimEditor : ScriptableObject
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapB;
        internal static PrefKey kAnimationNextFrame = new PrefKey("Animation/Next Frame", ".");
        internal static PrefKey kAnimationNextKeyframe = new PrefKey("Animation/Next Keyframe", "&.");
        internal static PrefKey kAnimationPrevFrame = new PrefKey("Animation/Previous Frame", ",");
        internal static PrefKey kAnimationPrevKeyframe = new PrefKey("Animation/Previous Keyframe", "&,");
        internal static PrefKey kAnimationRecordKeyframe = new PrefKey("Animation/Record Keyframe", "k");
        internal static PrefKey kAnimationShowCurvesToggle = new PrefKey("Animation/Show curves", "c");
        internal const float kDisabledRulerAlpha = 0.12f;
        internal static PrefColor kEulerXColor = new PrefColor("Testing/EulerX", 1f, 0f, 1f, 1f);
        internal static PrefColor kEulerYColor = new PrefColor("Testing/EulerY", 1f, 1f, 0f, 1f);
        internal static PrefColor kEulerZColor = new PrefColor("Testing/EulerZ", 0f, 1f, 1f, 1f);
        internal const int kHierarchyMinWidth = 300;
        internal const int kIntFieldWidth = 0x23;
        internal const int kLayoutRowHeight = 0x12;
        internal const int kSliderThickness = 15;
        [SerializeField]
        private AnimationWindowClipPopup m_ClipPopup;
        [SerializeField]
        private CurveEditor m_CurveEditor;
        [SerializeField]
        private DopeSheetEditor m_DopeSheet;
        [SerializeField]
        private AnimationEventTimeLine m_Events;
        [SerializeField]
        private AnimationWindowHierarchy m_Hierarchy;
        [SerializeField]
        private SplitterState m_HorizontalSplitter;
        [NonSerialized]
        private bool m_Initialized;
        [SerializeField]
        private EditorWindow m_OwnerWindow;
        [NonSerialized]
        private Rect m_Position;
        [NonSerialized]
        private float m_PreviousUpdateTime;
        [SerializeField]
        private AnimationWindowState m_State;
        [NonSerialized]
        private bool m_StylesInitialized;
        [NonSerialized]
        private bool m_TriggerFraming;
        private static List<AnimEditor> s_AnimationWindows = new List<AnimEditor>();

        private void AddEventButtonOnGUI()
        {
            if (GUILayout.Button(AnimationWindowStyles.addEventContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                AnimationEventPopup.Create(this.m_State.activeRootGameObject, this.m_State.activeAnimationClip, this.m_State.currentTime, this.m_OwnerWindow);
            }
        }

        private void AddKeyframeButtonOnGUI()
        {
            if (GUILayout.Button(AnimationWindowStyles.addKeyframeContent, EditorStyles.toolbarButton, new GUILayoutOption[0]) || kAnimationRecordKeyframe.activated)
            {
                AnimationWindowUtility.AddSelectedKeyframes(this.m_State, this.m_State.time);
            }
        }

        private void ClampSplitterSize()
        {
            this.m_HorizontalSplitter.realSizes[1] = (int) Mathf.Min(this.m_Position.width - this.hierarchyWidth, (float) this.m_HorizontalSplitter.realSizes[1]);
        }

        private void ClipSelectionDropDownOnGUI()
        {
            this.m_ClipPopup.OnGUI();
        }

        private void CurveEditorOnGUI(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.m_CurveEditor.rect = position;
                this.m_CurveEditor.SetTickMarkerRanges();
                this.m_CurveEditor.RecalculateBounds();
            }
            if (this.m_State.showCurveEditor)
            {
                Rect rect = new Rect(position.xMin, position.yMin, position.width - 15f, position.height - 15f);
                this.m_CurveEditor.vSlider = this.m_State.showCurveEditor;
                this.m_CurveEditor.hSlider = this.m_State.showCurveEditor;
                this.UpdateCurveEditorData();
                if (this.m_State.m_FrameCurveEditor)
                {
                    this.m_CurveEditor.FrameSelected(false, true);
                    this.m_State.m_FrameCurveEditor = false;
                }
                this.m_CurveEditor.BeginViewGUI();
                if (!this.m_State.disabled)
                {
                    GUI.Box(rect, GUIContent.none, AnimationWindowStyles.curveEditorBackground);
                    this.m_CurveEditor.GridGUI();
                    this.DrawPlayHead(rect.yMin, rect.yMax);
                }
                EditorGUI.BeginDisabledGroup(this.m_State.animationIsReadOnly);
                EditorGUI.BeginChangeCheck();
                this.m_CurveEditor.CurveGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    this.SaveChangedCurvesFromCurveEditor();
                    this.UpdateSelectedKeysFromCurveEditor();
                }
                EditorGUI.EndDisabledGroup();
                this.m_CurveEditor.EndViewGUI();
            }
        }

        private void DopeSheetOnGUI(Rect position)
        {
            Rect rect = new Rect(position.xMin, position.yMin, position.width - 15f, position.height);
            if (Event.current.type == EventType.Repaint)
            {
                this.m_DopeSheet.rect = rect;
                this.m_DopeSheet.SetTickMarkerRanges();
                this.m_DopeSheet.RecalculateBounds();
            }
            if (!this.m_State.showCurveEditor)
            {
                if (this.m_TriggerFraming && (Event.current.type == EventType.Repaint))
                {
                    this.m_DopeSheet.FrameClip();
                    this.m_TriggerFraming = false;
                }
                Rect rect2 = new Rect(position.xMin, position.yMin, position.width - 15f, position.height - 15f);
                Rect rect3 = new Rect(rect2.xMin, rect2.yMin, rect2.width, 16f);
                this.m_DopeSheet.BeginViewGUI();
                if (!this.m_State.disabled)
                {
                    this.m_DopeSheet.TimeRuler(rect2, this.m_State.frameRate, false, true, 0.12f);
                    this.m_DopeSheet.DrawMasterDopelineBackground(rect3);
                    this.DrawPlayHead(rect2.yMin - 1f, rect2.yMax);
                }
                this.m_DopeSheet.OnGUI(rect2, (Vector2) (this.m_State.hierarchyState.scrollPos * -1f));
                this.m_DopeSheet.EndViewGUI();
                Rect rect4 = new Rect(rect.xMax, rect.yMin, 15f, rect2.height);
                float bottomValue = Mathf.Max(this.m_DopeSheet.contentHeight, position.height);
                this.m_State.hierarchyState.scrollPos.y = GUI.VerticalScrollbar(rect4, this.m_State.hierarchyState.scrollPos.y, position.height, 0f, bottomValue);
            }
        }

        private void DrawPlayHead(float minY, float maxY)
        {
            Rect screenRect = new Rect(this.hierarchyWidth - 1f, 0f, (this.m_Position.width - this.hierarchyWidth) - 15f, this.m_Position.height);
            GUIClip.Push(screenRect, Vector2.zero, Vector2.zero, false);
            AnimationWindowUtility.DrawPlayHead(this.m_State.TimeToPixel(this.m_State.currentTime), minY, maxY, 1f);
            GUIClip.Pop();
        }

        private void EventLineOnGUI()
        {
            Rect position = GUILayoutUtility.GetRect((float) (this.m_Position.width - this.hierarchyWidth), (float) 18f);
            position.width -= 15f;
            GUI.Label(position, GUIContent.none, AnimationWindowStyles.eventBackground);
            if (!this.m_State.disabled)
            {
                this.DrawPlayHead(position.yMin - 1f, position.yMax);
            }
            this.m_Events.EventLineGUI(position, this.m_State);
        }

        private void FrameNavigationOnGUI()
        {
            if (GUILayout.Button(AnimationWindowStyles.prevKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToPreviousKeyframe();
            }
            if (GUILayout.Button(AnimationWindowStyles.nextKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToNextKeyframe();
            }
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(35f) };
            int num = EditorGUILayout.IntField(this.m_State.frame, EditorStyles.toolbarTextField, options);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_State.frame = num;
            }
        }

        private void FrameRateInputFieldOnGUI()
        {
            GUILayout.Label(AnimationWindowStyles.samples, AnimationWindowStyles.toolbarLabel, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(35f) };
            int num = EditorGUILayout.IntField((int) this.m_State.frameRate, EditorStyles.toolbarTextField, options);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_State.frameRate = num;
            }
        }

        public static List<AnimEditor> GetAllAnimationWindows()
        {
            return s_AnimationWindows;
        }

        private Rect GetContentLayoutRect()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            return GUILayoutUtility.GetRect(this.contentWidth, this.contentWidth, 0f, float.MaxValue, options);
        }

        private void HandleCopyPaste()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                string commandName = Event.current.commandName;
                if (commandName != null)
                {
                    int num;
                    if (<>f__switch$mapB == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                        dictionary.Add("Copy", 0);
                        dictionary.Add("Paste", 1);
                        <>f__switch$mapB = dictionary;
                    }
                    if (<>f__switch$mapB.TryGetValue(commandName, out num))
                    {
                        if (num == 0)
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.m_State.CopyKeys();
                            }
                            Event.current.Use();
                        }
                        else if (num == 1)
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.m_State.PasteKeys();
                                this.UpdateCurveEditorData();
                                this.UpdateSelectedKeysToCurveEditor();
                            }
                            Event.current.Use();
                        }
                    }
                }
            }
        }

        private void HandleFrameNavigationHotKeys()
        {
            if (kAnimationPrevKeyframe.activated)
            {
                this.MoveToPreviousKeyframe();
            }
            if (kAnimationNextKeyframe.activated)
            {
                this.MoveToNextKeyframe();
            }
            if (kAnimationNextFrame.activated)
            {
                this.m_State.frame++;
            }
            if (kAnimationPrevFrame.activated)
            {
                this.m_State.frame--;
            }
            if ((kAnimationPrevKeyframe.activated || kAnimationNextKeyframe.activated) || (kAnimationNextFrame.activated || kAnimationPrevFrame.activated))
            {
                this.Repaint();
            }
        }

        private void HierarchyOnGUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            Rect position = GUILayoutUtility.GetRect(this.hierarchyWidth, this.hierarchyWidth, 0f, float.MaxValue, options);
            if (!this.m_State.disabled)
            {
                this.m_Hierarchy.OnGUI(position);
            }
        }

        private void Initialize()
        {
            AnimationWindowStyles.Initialize();
            this.InitializeHierarchy();
            this.m_CurveEditor.m_PlayHead = this.m_State;
            this.m_Initialized = true;
        }

        private void InitializeClipSelection()
        {
            this.m_ClipPopup = new AnimationWindowClipPopup();
        }

        private void InitializeCurveEditor()
        {
            this.m_CurveEditor = new CurveEditor(new Rect(0f, 0f, this.contentWidth, 100f), new CurveWrapper[0], false);
            CurveEditorSettings settings = new CurveEditorSettings {
                hTickStyle = { distMin = 30, distFull = 80, distLabel = 0 }
            };
            if (EditorGUIUtility.isProSkin)
            {
                settings.vTickStyle.color = new Color(1f, 1f, 1f, settings.vTickStyle.color.a);
                settings.vTickStyle.labelColor = new Color(1f, 1f, 1f, settings.vTickStyle.labelColor.a);
            }
            settings.vTickStyle.distMin = 15;
            settings.vTickStyle.distFull = 40;
            settings.vTickStyle.distLabel = 30;
            settings.vTickStyle.stubs = true;
            settings.hRangeMin = 0f;
            settings.hRangeLocked = false;
            settings.vRangeLocked = false;
            settings.hSlider = true;
            settings.vSlider = true;
            this.m_CurveEditor.shownArea = new Rect(1f, 1f, 1f, 1f);
            this.m_CurveEditor.settings = settings;
            this.m_CurveEditor.m_PlayHead = this.m_State;
        }

        private void InitializeDopeSheet()
        {
            this.m_DopeSheet = new DopeSheetEditor(this.m_OwnerWindow);
            this.m_DopeSheet.SetTickMarkerRanges();
            this.m_DopeSheet.hSlider = true;
            this.m_DopeSheet.shownArea = new Rect(1f, 1f, 1f, 1f);
            this.m_DopeSheet.rect = new Rect(0f, 0f, this.contentWidth, 100f);
            this.m_DopeSheet.hTicks.SetTickModulosForFrameRate(this.m_State.frameRate);
        }

        private void InitializeEvents()
        {
            this.m_Events = new AnimationEventTimeLine(this.m_OwnerWindow);
        }

        private void InitializeHierarchy()
        {
            this.m_Hierarchy = new AnimationWindowHierarchy(this.m_State, this.m_OwnerWindow, new Rect(0f, 0f, this.hierarchyWidth, 100f));
        }

        private void InitializeHorizontalSplitter()
        {
            float[] relativeSizes = new float[] { 300f, 900f };
            int[] minSizes = new int[] { 300, 300 };
            this.m_HorizontalSplitter = new SplitterState(relativeSizes, minSizes, null);
            this.m_HorizontalSplitter.realSizes[0] = 300;
            this.m_HorizontalSplitter.realSizes[1] = ((int) this.m_Position.width) - 300;
            this.ClampSplitterSize();
        }

        private void InitializeNonserializedValues()
        {
            this.m_State.onFrameRateChange = (Action<float>) Delegate.Combine(this.m_State.onFrameRateChange, delegate (float newFrameRate) {
                this.m_CurveEditor.invSnap = newFrameRate;
                this.m_CurveEditor.hTicks.SetTickModulosForFrameRate(newFrameRate);
            });
        }

        private void MainContentOnGUI()
        {
            if (this.m_State.animatorIsOptimized)
            {
                GUILayout.Label("Editing optimized game object hierarchy is not supported.\nPlease select a game object that does not have 'Optimize Game Objects' applied.", new GUILayoutOption[0]);
            }
            else
            {
                Rect contentLayoutRect = this.GetContentLayoutRect();
                if (this.m_State.disabled)
                {
                    this.SetupWizardOnGUI(contentLayoutRect);
                }
                else if (this.m_State.showCurveEditor)
                {
                    this.CurveEditorOnGUI(contentLayoutRect);
                }
                else
                {
                    this.DopeSheetOnGUI(contentLayoutRect);
                }
                this.HandleCopyPaste();
                AnimationWindowUtility.DrawVerticalSplitLine(new Vector2(contentLayoutRect.xMin + 1f, contentLayoutRect.yMin), new Vector2(contentLayoutRect.xMin + 1f, contentLayoutRect.yMax));
            }
        }

        private void MoveToNextKeyframe()
        {
            this.m_State.currentTime = AnimationWindowUtility.GetNextKeyframeTime((!this.m_State.showCurveEditor ? this.m_State.allCurves : this.m_State.activeCurves).ToArray(), this.m_State.FrameToTime((float) this.m_State.frame), this.m_State.frameRate);
            this.m_State.frame = this.m_State.TimeToFrameFloor(this.m_State.currentTime);
        }

        private void MoveToPreviousKeyframe()
        {
            this.m_State.currentTime = AnimationWindowUtility.GetPreviousKeyframeTime((!this.m_State.showCurveEditor ? this.m_State.allCurves : this.m_State.activeCurves).ToArray(), this.m_State.FrameToTime((float) this.m_State.frame), this.m_State.frameRate);
            this.m_State.frame = this.m_State.TimeToFrameFloor(this.m_State.currentTime);
        }

        public void OnBreadcrumbGUI(EditorWindow parent, Rect position)
        {
            this.m_DopeSheet.m_Owner = parent;
            this.m_OwnerWindow = parent;
            this.m_Position = position;
            if (!this.m_Initialized)
            {
                this.Initialize();
            }
            this.m_State.OnGUI();
            this.ClampSplitterSize();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            SplitterGUILayout.BeginHorizontalSplit(this.m_HorizontalSplitter, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(this.m_State.disabled);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.RecordButtonOnGUI();
            this.PlayButtonOnGUI();
            this.FrameNavigationOnGUI();
            this.AddKeyframeButtonOnGUI();
            this.AddEventButtonOnGUI();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.ClipSelectionDropDownOnGUI();
            this.FrameRateInputFieldOnGUI();
            GUILayout.EndHorizontal();
            this.HierarchyOnGUI();
            GUILayout.BeginHorizontal(AnimationWindowStyles.miniToolbar, new GUILayoutOption[0]);
            this.TabSelectionButtonsOnGUI();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.TimeRulerOnGUI();
            this.EventLineOnGUI();
            this.MainContentOnGUI();
            GUILayout.EndVertical();
            SplitterGUILayout.EndHorizontalSplit();
            GUILayout.EndHorizontal();
            this.RenderEventTooltip();
            EditorGUI.EndDisabledGroup();
            this.HandleFrameNavigationHotKeys();
        }

        public void OnClipSelectionChange()
        {
            this.m_TriggerFraming = true;
        }

        public void OnControllerChange()
        {
            this.m_State.OnControllerChange();
            this.Repaint();
        }

        public void OnDisable()
        {
            s_AnimationWindows.Remove(this);
            if (this.m_CurveEditor != null)
            {
                this.m_CurveEditor.curvesUpdated = (CurveEditor.CallbackFunction) Delegate.Remove(this.m_CurveEditor.curvesUpdated, new CurveEditor.CallbackFunction(this.SaveChangedCurvesFromCurveEditor));
            }
            this.m_State.onClipSelectionChanged = (Action) Delegate.Remove(this.m_State.onClipSelectionChanged, new Action(this.OnClipSelectionChange));
            this.m_State.OnDisable();
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            s_AnimationWindows.Add(this);
            if (this.m_State == null)
            {
                this.m_State = ScriptableObject.CreateInstance(typeof(AnimationWindowState)) as AnimationWindowState;
                this.m_State.hideFlags = HideFlags.HideAndDontSave;
                this.m_State.animEditor = this;
                this.InitializeHorizontalSplitter();
                this.InitializeClipSelection();
                this.InitializeDopeSheet();
                this.InitializeEvents();
                this.InitializeCurveEditor();
            }
            this.InitializeNonserializedValues();
            this.m_State.timeArea = !this.m_State.showCurveEditor ? ((TimeArea) this.m_DopeSheet) : ((TimeArea) this.m_CurveEditor);
            this.m_DopeSheet.state = this.m_State;
            this.m_ClipPopup.state = this.m_State;
            this.m_State.onClipSelectionChanged = (Action) Delegate.Combine(this.m_State.onClipSelectionChanged, new Action(this.OnClipSelectionChange));
            this.m_State.OnSelectionChange();
            this.m_CurveEditor.curvesUpdated = (CurveEditor.CallbackFunction) Delegate.Combine(this.m_CurveEditor.curvesUpdated, new CurveEditor.CallbackFunction(this.SaveChangedCurvesFromCurveEditor));
        }

        public void OnSelectionChange()
        {
            this.m_State.OnSelectionChange();
            this.Repaint();
        }

        private void PlaybackUpdate()
        {
            if (this.m_State.playing)
            {
                float num = Time.realtimeSinceStartup - this.m_PreviousUpdateTime;
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                this.m_State.currentTime += num;
                if (this.m_State.currentTime > this.m_State.maxTime)
                {
                    this.m_State.currentTime = this.m_State.minTime;
                }
                this.m_State.currentTime = Mathf.Clamp(this.m_State.currentTime, this.m_State.minTime, this.m_State.maxTime);
                this.m_State.ResampleAnimation();
                this.Repaint();
            }
        }

        private void PlayButtonOnGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_State.playing = GUILayout.Toggle(this.m_State.playing, AnimationWindowStyles.playContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
            }
        }

        private void RecordButtonOnGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_State.recording = GUILayout.Toggle(this.m_State.recording, AnimationWindowStyles.recordContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && this.m_State.recording)
            {
                this.m_State.ResampleAnimation();
            }
        }

        private void RenderEndOfClipOverlay(float minY, float maxY)
        {
            Rect rect = new Rect(this.hierarchyWidth - 1f, 0f, (this.m_Position.width - this.hierarchyWidth) - 15f, this.m_Position.height);
            Rect rect2 = new Rect(rect.xMin, minY, rect.width, maxY - minY);
            AnimationWindowUtility.DrawEndOfClip(rect2, this.m_State.TimeToPixel(this.m_State.maxTime) + rect2.xMin);
        }

        private void RenderEventTooltip()
        {
            this.m_Events.DrawInstantTooltip(this.m_Position);
        }

        public void Repaint()
        {
            if (this.m_OwnerWindow != null)
            {
                this.m_OwnerWindow.Repaint();
            }
        }

        private void SaveChangedCurvesFromCurveEditor()
        {
            Undo.RegisterCompleteObjectUndo(this.m_State.activeAnimationClip, "Edit Curve");
            foreach (CurveWrapper wrapper in this.m_CurveEditor.animationCurves)
            {
                if (wrapper.changed)
                {
                    AnimationUtility.SetEditorCurve(this.m_State.activeAnimationClip, wrapper.binding, wrapper.curve);
                    wrapper.changed = false;
                }
            }
            this.m_State.ResampleAnimation();
        }

        private void SetupWizardOnGUI(Rect position)
        {
            Rect rect = new Rect(position.x, position.y, position.width - 15f, position.height - 15f);
            GUI.BeginClip(rect);
            GUI.enabled = true;
            this.m_State.showCurveEditor = false;
            this.m_State.timeArea = this.m_DopeSheet;
            this.m_State.timeArea.SetShownHRangeInsideMargins(0f, 1f);
            if ((Selection.activeGameObject != null) && !EditorUtility.IsPersistent(Selection.activeGameObject))
            {
                string str = ((this.m_State.activeRootGameObject != null) || (this.m_State.activeAnimationClip != null)) ? AnimationWindowStyles.animationClip.text : AnimationWindowStyles.animatorAndAnimationClip.text;
                GUIContent content = GUIContent.Temp(string.Format(AnimationWindowStyles.formatIsMissing.text, Selection.activeGameObject.name, str));
                Vector2 vector = GUI.skin.label.CalcSize(content);
                Rect rect2 = new Rect((rect.width * 0.5f) - (vector.x * 0.5f), (rect.height * 0.5f) - (vector.y * 0.5f), vector.x, vector.y);
                GUI.Label(rect2, content);
                Rect rect3 = new Rect((rect.width * 0.5f) - 35f, rect2.yMax + 3f, 70f, 20f);
                if (GUI.Button(rect3, AnimationWindowStyles.create) && AnimationWindowUtility.InitializeGameobjectForAnimation(Selection.activeGameObject))
                {
                    Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(Selection.activeGameObject.transform);
                    this.m_State.activeAnimationClip = AnimationUtility.GetAnimationClips(closestAnimationPlayerComponentInParents.gameObject)[0];
                    this.m_State.recording = true;
                    this.m_State.currentTime = 0f;
                    this.m_State.ResampleAnimation();
                }
            }
            else
            {
                Color color = GUI.color;
                GUI.color = Color.gray;
                Vector2 vector2 = GUI.skin.label.CalcSize(AnimationWindowStyles.noAnimatableObjectSelectedText);
                Rect rect4 = new Rect((rect.width * 0.5f) - (vector2.x * 0.5f), (rect.height * 0.5f) - (vector2.y * 0.5f), vector2.x, vector2.y);
                GUI.Label(rect4, AnimationWindowStyles.noAnimatableObjectSelectedText);
                GUI.color = color;
            }
            GUI.EndClip();
            GUI.enabled = false;
        }

        private void SwitchBetweenCurvesAndDopesheet()
        {
            this.m_State.showCurveEditor = !this.m_State.showCurveEditor;
            if (this.m_State.showCurveEditor)
            {
                this.UpdateCurveEditorData();
                this.UpdateSelectedKeysToCurveEditor();
                AnimationWindowUtility.SyncTimeArea(this.m_DopeSheet, this.m_CurveEditor);
                this.m_State.timeArea = this.m_CurveEditor;
                this.m_CurveEditor.RecalculateBounds();
                this.m_CurveEditor.FrameSelected(false, true);
            }
            else
            {
                this.UpdateSelectedKeysFromCurveEditor();
                AnimationWindowUtility.SyncTimeArea(this.m_CurveEditor, this.m_DopeSheet);
                this.m_State.timeArea = this.m_DopeSheet;
            }
        }

        private void TabSelectionButtonsOnGUI()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.Toggle(!this.m_State.showCurveEditor, AnimationWindowStyles.dopesheet, AnimationWindowStyles.miniToolbarButton, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.Toggle(this.m_State.showCurveEditor, AnimationWindowStyles.curves, AnimationWindowStyles.miniToolbarButton, optionArray2);
            if (EditorGUI.EndChangeCheck() || kAnimationShowCurvesToggle.activated)
            {
                this.SwitchBetweenCurvesAndDopesheet();
            }
        }

        private void TimeRulerOnGUI()
        {
            Rect position = GUILayoutUtility.GetRect((float) (this.m_Position.width - this.hierarchyWidth), (float) 18f);
            Rect rect2 = new Rect(position.xMin, position.yMin, position.width - 15f, position.height);
            GUI.Box(position, GUIContent.none, EditorStyles.toolbarButton);
            this.m_State.timeArea.TimeRuler(rect2, this.m_State.frameRate, true, false, 1f);
            if (!this.m_State.disabled)
            {
                this.RenderEndOfClipOverlay(rect2.yMin, rect2.yMax);
                this.DrawPlayHead(rect2.yMin, rect2.yMax);
            }
            EditorGUI.BeginChangeCheck();
            int num = Mathf.Max(Mathf.RoundToInt(GUI.HorizontalSlider(rect2, (float) this.m_State.frame, this.m_State.minVisibleFrame, this.m_State.maxVisibleFrame, GUIStyle.none, GUIStyle.none)), 0);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_State.frame = num;
                this.m_State.recording = true;
                this.m_State.ResampleAnimation();
            }
        }

        public void Update()
        {
            if (this.m_State != null)
            {
                this.PlaybackUpdate();
            }
        }

        private void UpdateCurveEditorData()
        {
            this.m_CurveEditor.animationCurves = this.m_State.activeCurveWrappers.ToArray();
        }

        private void UpdateSelectedKeysFromCurveEditor()
        {
            this.m_State.ClearKeySelections();
            foreach (CurveSelection selection in this.m_CurveEditor.selectedCurves)
            {
                AnimationWindowKeyframe keyframe = AnimationWindowUtility.CurveSelectionToAnimationWindowKeyframe(selection, this.m_State.allCurves);
                if (keyframe != null)
                {
                    this.m_State.SelectKey(keyframe);
                }
            }
        }

        private void UpdateSelectedKeysToCurveEditor()
        {
            this.m_CurveEditor.ClearSelection();
            foreach (AnimationWindowKeyframe keyframe in this.m_State.selectedKeys)
            {
                CurveSelection curveSelection = AnimationWindowUtility.AnimationWindowKeyframeToCurveSelection(keyframe, this.m_CurveEditor);
                if (curveSelection != null)
                {
                    this.m_CurveEditor.AddSelection(curveSelection);
                }
            }
        }

        private float contentWidth
        {
            get
            {
                return (float) this.m_HorizontalSplitter.realSizes[1];
            }
        }

        private float hierarchyWidth
        {
            get
            {
                return (float) this.m_HorizontalSplitter.realSizes[0];
            }
        }

        public bool locked
        {
            get
            {
                return this.m_State.locked;
            }
            set
            {
                this.m_State.locked = value;
            }
        }

        public bool stateDisabled
        {
            get
            {
                return this.m_State.disabled;
            }
        }
    }
}

