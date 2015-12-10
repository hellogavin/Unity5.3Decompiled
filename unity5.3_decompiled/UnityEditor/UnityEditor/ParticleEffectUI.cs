namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class ParticleEffectUI
    {
        private static readonly Color k_DarkSkinDisabledColor = new Color(0.66f, 0.66f, 0.66f, 0.95f);
        private static readonly Color k_LightSkinDisabledColor = new Color(0.84f, 0.84f, 0.84f, 0.95f);
        private static readonly Vector2 k_MinCurveAreaSize = new Vector2(100f, 100f);
        private static readonly Vector2 k_MinEmitterAreaSize = new Vector2(125f, 100f);
        private const string k_ShowSelectedId = "ShowSelected";
        private const string k_SimulationStateId = "SimulationState";
        private static PrefKey kForward = new PrefKey("ParticleSystem/Forward", "m");
        private static PrefKey kPlay = new PrefKey("ParticleSystem/Play", ",");
        private static PrefKey kReverse = new PrefKey("ParticleSystem/Reverse", "n");
        private static PrefKey kStop = new PrefKey("ParticleSystem/Stop", ".");
        private float m_CurveEditorAreaHeight = 330f;
        private Vector2 m_EmitterAreaScrollPos = Vector2.zero;
        private float m_EmitterAreaWidth = 230f;
        public ParticleSystemUI[] m_Emitters;
        private int m_IsDraggingTimeHotControlID = -1;
        public ParticleEffectUIOwner m_Owner;
        private ParticleSystemCurveEditor m_ParticleSystemCurveEditor;
        private ParticleSystem m_SelectedParticleSystem;
        private bool m_ShowOnlySelectedMode;
        public static bool m_ShowWireframe = false;
        private TimeHelper m_TimeHelper = new TimeHelper();
        public static bool m_VerticalLayout;
        private static Texts s_Texts;

        public ParticleEffectUI(ParticleEffectUIOwner owner)
        {
            this.m_Owner = owner;
        }

        private void ApplyModifiedProperties()
        {
            for (int i = 0; i < this.m_Emitters.Length; i++)
            {
                this.m_Emitters[i].ApplyProperties();
            }
        }

        private void ClampWindowContentSizes()
        {
            if (Event.current.type != EventType.Layout)
            {
                float width = GUIClip.visibleRect.width;
                float height = GUIClip.visibleRect.height;
                if (m_VerticalLayout)
                {
                    this.m_CurveEditorAreaHeight = Mathf.Clamp(this.m_CurveEditorAreaHeight, k_MinCurveAreaSize.y, height - k_MinEmitterAreaSize.y);
                }
                else
                {
                    this.m_EmitterAreaWidth = Mathf.Clamp(this.m_EmitterAreaWidth, k_MinEmitterAreaSize.x, width - k_MinCurveAreaSize.x);
                }
            }
        }

        public void Clear()
        {
            ParticleSystem root = this.GetRoot();
            if (this.ShouldManagePlaybackState(root) && (root != null))
            {
                PlayState playing;
                if (this.IsPlaying())
                {
                    playing = PlayState.Playing;
                }
                else if (this.IsPaused())
                {
                    playing = PlayState.Paused;
                }
                else
                {
                    playing = PlayState.Stopped;
                }
                int instanceID = root.GetInstanceID();
                SessionState.SetVector3("SimulationState" + instanceID, new Vector3((float) instanceID, (float) playing, ParticleSystemEditorUtils.editorPlaybackTime));
            }
            this.m_ParticleSystemCurveEditor.OnDisable();
            ParticleEffectUtils.ClearPlanes();
            Tools.s_Hidden = false;
            if (root != null)
            {
                SessionState.SetBool("ShowSelected" + root.GetInstanceID(), this.m_ShowOnlySelectedMode);
            }
            this.SetShowOnlySelectedMode(false);
            GameView.RepaintAll();
            SceneView.RepaintAll();
        }

        public GameObject CreateParticleSystem(ParticleSystem parentOfNewParticleSystem, SubModuleUI.SubEmitterType defaultType)
        {
            Type[] components = new Type[] { typeof(ParticleSystem) };
            GameObject obj2 = new GameObject(this.GetNextParticleSystemName(), components);
            if (obj2 == null)
            {
                return null;
            }
            if (parentOfNewParticleSystem != null)
            {
                obj2.transform.parent = parentOfNewParticleSystem.transform;
            }
            obj2.transform.localPosition = Vector3.zero;
            obj2.transform.localRotation = Quaternion.identity;
            ParticleSystem component = obj2.GetComponent<ParticleSystem>();
            if (defaultType != SubModuleUI.SubEmitterType.None)
            {
                component.SetupDefaultType((int) defaultType);
            }
            SessionState.SetFloat("CurrentEmitterAreaScroll", this.m_EmitterAreaScrollPos.x);
            return obj2;
        }

        private void DrawSelectionMarker(Rect rect)
        {
            rect.x++;
            rect.y++;
            rect.width -= 2f;
            rect.height -= 2f;
            ParticleSystemStyles.Get().selectionMarker.Draw(rect, GUIContent.none, false, true, true, false);
        }

        internal static bool GetAllModulesVisible()
        {
            return EditorPrefs.GetBool("ParticleSystemShowAllModules", true);
        }

        private static void GetDirectParticleSystemChildrenRecursive(Transform transform, List<ParticleSystem> particleSystems)
        {
            IEnumerator enumerator = transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    ParticleSystem component = current.gameObject.GetComponent<ParticleSystem>();
                    if (component != null)
                    {
                        particleSystems.Add(component);
                        GetDirectParticleSystemChildrenRecursive(current, particleSystems);
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
        }

        private static Color GetDisabledColor()
        {
            return (EditorGUIUtility.isProSkin ? k_DarkSkinDisabledColor : k_LightSkinDisabledColor);
        }

        public static Vector2 GetMinSize()
        {
            return (k_MinEmitterAreaSize + k_MinCurveAreaSize);
        }

        public string GetNextParticleSystemName()
        {
            string str = string.Empty;
            for (int i = 2; i < 50; i++)
            {
                str = "Particle System " + i;
                bool flag = false;
                foreach (ParticleSystemUI mui in this.m_Emitters)
                {
                    if (mui.m_ParticleSystem.name == str)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return str;
                }
            }
            return "Particle System";
        }

        internal int GetNumEnabledRenderers()
        {
            int num = 0;
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                ModuleUI particleSystemRendererModuleUI = mui.GetParticleSystemRendererModuleUI();
                if ((particleSystemRendererModuleUI != null) && particleSystemRendererModuleUI.enabled)
                {
                    num++;
                }
            }
            return num;
        }

        public ParticleSystemCurveEditor GetParticleSystemCurveEditor()
        {
            return this.m_ParticleSystemCurveEditor;
        }

        internal GameObject[] GetParticleSystemGameObjects()
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < this.m_Emitters.Length; i++)
            {
                list.Add(this.m_Emitters[i].m_ParticleSystem.gameObject);
            }
            return list.ToArray();
        }

        internal static ParticleSystem[] GetParticleSystems(ParticleSystem root)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem> {
                root
            };
            GetDirectParticleSystemChildrenRecursive(root.transform, particleSystems);
            return particleSystems.ToArray();
        }

        public ParticleSystemUI GetParticleSystemUIForParticleSystem(ParticleSystem shuriken)
        {
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                if (mui.m_ParticleSystem == shuriken)
                {
                    return mui;
                }
            }
            return null;
        }

        public List<ParticleSystemUI> GetParticleSystemUIList(List<ParticleSystem> shurikens)
        {
            List<ParticleSystemUI> list = new List<ParticleSystemUI>();
            foreach (ParticleSystem system in shurikens)
            {
                ParticleSystemUI particleSystemUIForParticleSystem = this.GetParticleSystemUIForParticleSystem(system);
                if (particleSystemUIForParticleSystem != null)
                {
                    list.Add(particleSystemUIForParticleSystem);
                }
            }
            return list;
        }

        internal ParticleSystem GetRoot()
        {
            return ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
        }

        private List<ParticleSystemUI> GetSelectedParticleSystemUIs()
        {
            List<ParticleSystemUI> list = new List<ParticleSystemUI>();
            int[] instanceIDs = Selection.instanceIDs;
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                if (instanceIDs.Contains<int>(mui.m_ParticleSystem.gameObject.GetInstanceID()))
                {
                    list.Add(mui);
                }
            }
            return list;
        }

        private void HandleKeyboardShortcuts(ParticleSystem root)
        {
            Event current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                int num = 0;
                if (current.keyCode == kPlay.keyCode)
                {
                    if (EditorApplication.isPlaying)
                    {
                        this.Stop();
                        this.Play();
                    }
                    else if (!ParticleSystemEditorUtils.editorIsPlaying)
                    {
                        this.Play();
                    }
                    else
                    {
                        this.Pause();
                    }
                    current.Use();
                }
                else if (current.keyCode == kStop.keyCode)
                {
                    this.Stop();
                    current.Use();
                }
                else if (current.keyCode == kReverse.keyCode)
                {
                    num = -1;
                }
                else if (current.keyCode == kForward.keyCode)
                {
                    num = 1;
                }
                if (num != 0)
                {
                    ParticleSystemEditorUtils.editorIsScrubbing = true;
                    float editorSimulationSpeed = ParticleSystemEditorUtils.editorSimulationSpeed;
                    float num3 = ((!current.shift ? 1f : 3f) * this.m_TimeHelper.deltaTime) * ((num <= 0) ? -3f : 3f);
                    ParticleSystemEditorUtils.editorPlaybackTime = Mathf.Max((float) 0f, (float) (ParticleSystemEditorUtils.editorPlaybackTime + (num3 * editorSimulationSpeed)));
                    if (root.isStopped)
                    {
                        root.Play();
                        root.Pause();
                    }
                    ParticleSystemEditorUtils.PerformCompleteResimulation();
                    current.Use();
                }
            }
            if ((current.type == EventType.KeyUp) && ((current.keyCode == kReverse.keyCode) || (current.keyCode == kForward.keyCode)))
            {
                ParticleSystemEditorUtils.editorIsScrubbing = false;
            }
        }

        private void InitAllEmitters(ParticleSystem[] shurikens)
        {
            int length = shurikens.Length;
            if (length != 0)
            {
                this.m_Emitters = new ParticleSystemUI[length];
                for (int i = 0; i < length; i++)
                {
                    this.m_Emitters[i] = new ParticleSystemUI();
                    this.m_Emitters[i].Init(this, shurikens[i]);
                }
                foreach (ParticleSystemUI mui in this.m_Emitters)
                {
                    foreach (ModuleUI eui in mui.m_Modules)
                    {
                        if (eui != null)
                        {
                            eui.Validate();
                        }
                    }
                }
                if (GetAllModulesVisible())
                {
                    this.SetAllModulesVisible(true);
                }
            }
        }

        public bool InitializeIfNeeded(ParticleSystem shuriken)
        {
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(shuriken);
            if (root == null)
            {
                return false;
            }
            ParticleSystem[] particleSystems = GetParticleSystems(root);
            if (((root == this.GetRoot()) && (this.m_ParticleSystemCurveEditor != null)) && ((this.m_Emitters != null) && (particleSystems.Length == this.m_Emitters.Length)))
            {
                this.m_SelectedParticleSystem = shuriken;
                if (this.IsShowOnlySelectedMode())
                {
                    this.RefreshShowOnlySelected();
                }
                return false;
            }
            if (this.m_ParticleSystemCurveEditor != null)
            {
                this.Clear();
            }
            this.m_SelectedParticleSystem = shuriken;
            ParticleSystemEditorUtils.PerformCompleteResimulation();
            this.m_ParticleSystemCurveEditor = new ParticleSystemCurveEditor();
            this.m_ParticleSystemCurveEditor.Init();
            this.m_EmitterAreaWidth = EditorPrefs.GetFloat("ParticleSystemEmitterAreaWidth", k_MinEmitterAreaSize.x);
            this.m_CurveEditorAreaHeight = EditorPrefs.GetFloat("ParticleSystemCurveEditorAreaHeight", k_MinCurveAreaSize.y);
            this.InitAllEmitters(particleSystems);
            this.m_ShowOnlySelectedMode = (this.m_Owner is ParticleSystemWindow) && SessionState.GetBool("ShowSelected" + root.GetInstanceID(), false);
            if (this.IsShowOnlySelectedMode())
            {
                this.RefreshShowOnlySelected();
            }
            this.m_EmitterAreaScrollPos.x = SessionState.GetFloat("CurrentEmitterAreaScroll", 0f);
            if (this.ShouldManagePlaybackState(root))
            {
                Vector3 vector = SessionState.GetVector3("SimulationState" + root.GetInstanceID(), Vector3.zero);
                if (root.GetInstanceID() == ((int) vector.x))
                {
                    float z = vector.z;
                    if (z > 0f)
                    {
                        ParticleSystemEditorUtils.editorPlaybackTime = z;
                    }
                }
                this.Play();
            }
            return true;
        }

        public bool IsParticleSystemUIVisible(ParticleSystemUI psUI)
        {
            OwnerType type = !(this.m_Owner is ParticleSystemInspector) ? OwnerType.ParticleSystemWindow : OwnerType.Inspector;
            if ((type != OwnerType.ParticleSystemWindow) && ((type != OwnerType.Inspector) || (psUI.m_ParticleSystem != this.m_SelectedParticleSystem)))
            {
                return false;
            }
            return true;
        }

        internal bool IsPaused()
        {
            return (!this.IsPlaying() && !IsStopped(this.GetRoot()));
        }

        internal bool IsPlaying()
        {
            return ParticleSystemEditorUtils.editorIsPlaying;
        }

        internal bool IsPlayOnAwake()
        {
            if (this.m_Emitters.Length > 0)
            {
                InitialModuleUI eui = this.m_Emitters[0].m_Modules[0] as InitialModuleUI;
                return eui.m_PlayOnAwake.boolValue;
            }
            return false;
        }

        internal bool IsShowOnlySelectedMode()
        {
            return this.m_ShowOnlySelectedMode;
        }

        internal static bool IsStopped(ParticleSystem root)
        {
            return ((!ParticleSystemEditorUtils.editorIsPlaying && !ParticleSystemEditorUtils.editorIsPaused) && !ParticleSystemEditorUtils.editorIsScrubbing);
        }

        private void MultiParticleSystemGUI(bool verticalLayout)
        {
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            GUILayout.BeginVertical(ParticleSystemStyles.Get().effectBgStyle, new GUILayoutOption[0]);
            this.m_EmitterAreaScrollPos = EditorGUILayout.BeginScrollView(this.m_EmitterAreaScrollPos, new GUILayoutOption[0]);
            Rect position = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            this.m_EmitterAreaScrollPos -= EditorGUI.MouseDeltaReader(position, Event.current.alt);
            GUILayout.Space(3f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(3f);
            Color color = GUI.color;
            bool flag = Event.current.type == EventType.Repaint;
            bool flag2 = this.IsShowOnlySelectedMode();
            List<ParticleSystemUI> selectedParticleSystemUIs = this.GetSelectedParticleSystemUIs();
            for (int i = 0; i < this.m_Emitters.Length; i++)
            {
                if (i != 0)
                {
                    GUILayout.Space(ModuleUI.k_SpaceBetweenModules);
                }
                bool flag3 = selectedParticleSystemUIs.Contains(this.m_Emitters[i]);
                ModuleUI particleSystemRendererModuleUI = this.m_Emitters[i].GetParticleSystemRendererModuleUI();
                if ((flag && (particleSystemRendererModuleUI != null)) && !particleSystemRendererModuleUI.enabled)
                {
                    GUI.color = GetDisabledColor();
                }
                if ((flag && flag2) && !flag3)
                {
                    GUI.color = GetDisabledColor();
                }
                Rect rect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                if ((flag && flag3) && (this.m_Emitters.Length > 1))
                {
                    this.DrawSelectionMarker(rect);
                }
                this.m_Emitters[i].OnGUI(root, ModuleUI.k_CompactFixedModuleWidth, true);
                EditorGUILayout.EndVertical();
                GUI.color = color;
            }
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(20f) };
            if (GUILayout.Button(s_Texts.addParticleSystem, "OL Plus", options))
            {
                this.CreateParticleSystem(root, SubModuleUI.SubEmitterType.None);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            this.m_EmitterAreaScrollPos -= EditorGUI.MouseDeltaReader(position, true);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            this.HandleKeyboardShortcuts(root);
        }

        public void OnGUI()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (this.m_Emitters != null)
            {
                this.UpdateProperties();
                OwnerType type2 = !(this.m_Owner is ParticleSystemInspector) ? OwnerType.ParticleSystemWindow : OwnerType.Inspector;
                if (type2 != OwnerType.Inspector)
                {
                    if (type2 != OwnerType.ParticleSystemWindow)
                    {
                        Debug.LogError("Unhandled enum");
                    }
                    else
                    {
                        this.ClampWindowContentSizes();
                        bool verticalLayout = m_VerticalLayout;
                        if (verticalLayout)
                        {
                            this.MultiParticleSystemGUI(verticalLayout);
                            this.WindowCurveEditorGUI(verticalLayout);
                        }
                        else
                        {
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            this.MultiParticleSystemGUI(verticalLayout);
                            this.WindowCurveEditorGUI(verticalLayout);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                else
                {
                    this.SingleParticleSystemGUI();
                }
                this.ApplyModifiedProperties();
            }
        }

        public void OnSceneGUI()
        {
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                mui.OnSceneGUI();
            }
        }

        public void OnSceneViewGUI()
        {
            ParticleSystem root = this.GetRoot();
            if ((root != null) && root.gameObject.activeInHierarchy)
            {
                SceneViewOverlay.Window(ParticleSystemInspector.playBackTitle, new SceneViewOverlay.WindowFunction(this.SceneViewGUICallback), 400, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);
            }
        }

        internal void Pause()
        {
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            if (root != null)
            {
                root.Pause();
                ParticleSystemEditorUtils.editorIsScrubbing = true;
                this.m_Owner.Repaint();
            }
        }

        internal void Play()
        {
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            if (root != null)
            {
                root.Play();
                ParticleSystemEditorUtils.editorIsScrubbing = false;
                this.m_Owner.Repaint();
            }
        }

        internal void PlayBackTimeGUI(ParticleSystem root)
        {
            if (root == null)
            {
                root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            }
            EventType type = Event.current.type;
            int hotControl = GUIUtility.hotControl;
            string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
            EditorGUI.BeginChangeCheck();
            EditorGUI.kFloatFieldFormatString = s_Texts.secondsFloatFieldFormatString;
            float a = EditorGUILayout.FloatField(s_Texts.previewTime, ParticleSystemEditorUtils.editorPlaybackTime, new GUILayoutOption[0]);
            EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
            if (EditorGUI.EndChangeCheck())
            {
                if (type == EventType.MouseDrag)
                {
                    ParticleSystemEditorUtils.editorIsScrubbing = true;
                    float editorSimulationSpeed = ParticleSystemEditorUtils.editorSimulationSpeed;
                    float editorPlaybackTime = ParticleSystemEditorUtils.editorPlaybackTime;
                    float num5 = a - editorPlaybackTime;
                    a = editorPlaybackTime + (num5 * (0.05f * editorSimulationSpeed));
                }
                ParticleSystemEditorUtils.editorPlaybackTime = Mathf.Max(a, 0f);
                if (root.isStopped)
                {
                    root.Play();
                    root.Pause();
                }
                ParticleSystemEditorUtils.PerformCompleteResimulation();
            }
            if ((type == EventType.MouseDown) && (GUIUtility.hotControl != hotControl))
            {
                this.m_IsDraggingTimeHotControlID = GUIUtility.hotControl;
                ParticleSystemEditorUtils.editorIsScrubbing = true;
            }
            if ((this.m_IsDraggingTimeHotControlID != -1) && (GUIUtility.hotControl != this.m_IsDraggingTimeHotControlID))
            {
                this.m_IsDraggingTimeHotControlID = -1;
                ParticleSystemEditorUtils.editorIsScrubbing = false;
            }
            EditorGUILayout.FloatField(s_Texts.particleCount, (float) root.particleCount, new GUILayoutOption[0]);
        }

        public void PlayOnAwakeChanged(bool newPlayOnAwake)
        {
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                InitialModuleUI eui = mui.m_Modules[0] as InitialModuleUI;
                eui.m_PlayOnAwake.boolValue = newPlayOnAwake;
                mui.ApplyProperties();
            }
        }

        internal void PlayStopGUI()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            if (Event.current.type == EventType.Layout)
            {
                this.m_TimeHelper.Update();
            }
            if (!EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                bool flag = ParticleSystemEditorUtils.editorIsPlaying && !ParticleSystemEditorUtils.editorIsPaused;
                if (GUILayout.Button(!flag ? s_Texts.play : s_Texts.pause, "ButtonLeft", new GUILayoutOption[0]))
                {
                    if (flag)
                    {
                        this.Pause();
                    }
                    else
                    {
                        this.Play();
                    }
                }
                if (GUILayout.Button(s_Texts.stop, "ButtonRight", new GUILayoutOption[0]))
                {
                    this.Stop();
                }
                GUILayout.EndHorizontal();
                string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
                EditorGUI.kFloatFieldFormatString = s_Texts.secondsFloatFieldFormatString;
                ParticleSystemEditorUtils.editorSimulationSpeed = Mathf.Clamp(EditorGUILayout.FloatField(s_Texts.previewSpeed, ParticleSystemEditorUtils.editorSimulationSpeed, new GUILayoutOption[0]), 0f, 10f);
                EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
                this.PlayBackTimeGUI(root);
            }
            else
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button(s_Texts.play, new GUILayoutOption[0]))
                {
                    this.Stop();
                    this.Play();
                }
                if (GUILayout.Button(s_Texts.stop, new GUILayoutOption[0]))
                {
                    this.Stop();
                }
                GUILayout.EndHorizontal();
            }
            this.HandleKeyboardShortcuts(root);
        }

        public void Refresh()
        {
            this.UpdateProperties();
            this.m_ParticleSystemCurveEditor.Refresh();
        }

        internal void RefreshShowOnlySelected()
        {
            if (this.IsShowOnlySelectedMode())
            {
                int[] instanceIDs = Selection.instanceIDs;
                foreach (ParticleSystemUI mui in this.m_Emitters)
                {
                    ParticleSystemRenderer particleSystemRenderer = mui.GetParticleSystemRenderer();
                    if (particleSystemRenderer != null)
                    {
                        particleSystemRenderer.editorEnabled = instanceIDs.Contains<int>(mui.m_ParticleSystem.gameObject.GetInstanceID());
                    }
                }
            }
            else
            {
                foreach (ParticleSystemUI mui2 in this.m_Emitters)
                {
                    ParticleSystemRenderer renderer2 = mui2.GetParticleSystemRenderer();
                    if (renderer2 != null)
                    {
                        renderer2.editorEnabled = true;
                    }
                }
            }
        }

        private Rect ResizeHandling(bool verticalLayout)
        {
            Rect lastRect;
            if (verticalLayout)
            {
                lastRect = GUILayoutUtility.GetLastRect();
                lastRect.y += -5f;
                lastRect.height = 5f;
                float y = EditorGUI.MouseDeltaReader(lastRect, true).y;
                if (y != 0f)
                {
                    this.m_CurveEditorAreaHeight -= y;
                    this.ClampWindowContentSizes();
                    EditorPrefs.SetFloat("ParticleSystemCurveEditorAreaHeight", this.m_CurveEditorAreaHeight);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.SplitResizeUpDown);
                }
                return lastRect;
            }
            lastRect = new Rect(this.m_EmitterAreaWidth - 5f, 0f, 5f, GUIClip.visibleRect.height);
            float x = EditorGUI.MouseDeltaReader(lastRect, true).x;
            if (x != 0f)
            {
                this.m_EmitterAreaWidth += x;
                this.ClampWindowContentSizes();
                EditorPrefs.SetFloat("ParticleSystemEmitterAreaWidth", this.m_EmitterAreaWidth);
            }
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.SplitResizeLeftRight);
            }
            return lastRect;
        }

        private void SceneViewGUICallback(Object target, SceneView sceneView)
        {
            this.PlayStopGUI();
        }

        internal void SetAllModulesVisible(bool showAll)
        {
            EditorPrefs.SetBool("ParticleSystemShowAllModules", showAll);
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                for (int i = 0; i < mui.m_Modules.Length; i++)
                {
                    ModuleUI eui = mui.m_Modules[i];
                    if (eui != null)
                    {
                        if (showAll)
                        {
                            if (!eui.visibleUI)
                            {
                                eui.visibleUI = true;
                            }
                        }
                        else
                        {
                            bool flag = true;
                            if ((eui is RendererModuleUI) && (mui.GetParticleSystemRenderer() != null))
                            {
                                flag = false;
                            }
                            if (flag && !eui.enabled)
                            {
                                eui.visibleUI = false;
                            }
                        }
                    }
                }
            }
        }

        internal void SetShowOnlySelectedMode(bool enable)
        {
            this.m_ShowOnlySelectedMode = enable;
            this.RefreshShowOnlySelected();
        }

        private bool ShouldManagePlaybackState(ParticleSystem root)
        {
            bool activeInHierarchy = false;
            if (root != null)
            {
                activeInHierarchy = root.gameObject.activeInHierarchy;
            }
            return ((!EditorApplication.isPlaying && !ParticleSystemEditorUtils.editorUpdateAll) && activeInHierarchy);
        }

        private void SingleParticleSystemGUI()
        {
            ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem);
            GUILayout.BeginVertical(ParticleSystemStyles.Get().effectBgStyle, new GUILayoutOption[0]);
            ParticleSystemUI particleSystemUIForParticleSystem = this.GetParticleSystemUIForParticleSystem(this.m_SelectedParticleSystem);
            if (particleSystemUIForParticleSystem != null)
            {
                float width = GUIClip.visibleRect.width - 18f;
                particleSystemUIForParticleSystem.OnGUI(root, width, false);
            }
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            ParticleSystemEditorUtils.editorResimulation = GUILayout.Toggle(ParticleSystemEditorUtils.editorResimulation, s_Texts.resimulation, EditorStyles.toggle, new GUILayoutOption[0]);
            m_ShowWireframe = GUILayout.Toggle(m_ShowWireframe, "Wireframe", EditorStyles.toggle, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            this.HandleKeyboardShortcuts(root);
        }

        internal void Stop()
        {
            ParticleSystemEditorUtils.editorIsScrubbing = false;
            ParticleSystemEditorUtils.editorPlaybackTime = 0f;
            ParticleSystemEditorUtils.StopEffect();
            this.m_Owner.Repaint();
        }

        internal void UndoRedoPerformed()
        {
            foreach (ParticleSystemUI mui in this.m_Emitters)
            {
                foreach (ModuleUI eui in mui.m_Modules)
                {
                    if (eui != null)
                    {
                        eui.CheckVisibilityState();
                    }
                }
            }
            this.m_Owner.Repaint();
        }

        internal void UpdateProperties()
        {
            for (int i = 0; i < this.m_Emitters.Length; i++)
            {
                this.m_Emitters[i].UpdateProperties();
            }
        }

        public bool ValidateParticleSystemProperty(SerializedProperty shurikenProperty)
        {
            if (shurikenProperty != null)
            {
                ParticleSystem objectReferenceValue = shurikenProperty.objectReferenceValue as ParticleSystem;
                if ((objectReferenceValue != null) && (this.GetParticleSystemUIForParticleSystem(objectReferenceValue) == null))
                {
                    EditorUtility.DisplayDialog("ParticleSystem Warning", "The SubEmitter module cannot reference a ParticleSystem that is not a child of the root ParticleSystem.\n\nThe ParticleSystem '" + objectReferenceValue.name + "' must be a child of the ParticleSystem '" + ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystem).name + "'.", "Ok");
                    shurikenProperty.objectReferenceValue = null;
                    return false;
                }
            }
            return true;
        }

        private void WindowCurveEditorGUI(bool verticalLayout)
        {
            Rect rect;
            if (verticalLayout)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(this.m_CurveEditorAreaHeight) };
                rect = GUILayoutUtility.GetRect((float) 13f, this.m_CurveEditorAreaHeight, options);
            }
            else
            {
                EditorWindow owner = (EditorWindow) this.m_Owner;
                rect = GUILayoutUtility.GetRect((float) (owner.position.width - this.m_EmitterAreaWidth), (float) (owner.position.height - 17f));
            }
            this.ResizeHandling(verticalLayout);
            this.m_ParticleSystemCurveEditor.OnGUI(rect);
        }

        internal static Texts texts
        {
            get
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                return s_Texts;
            }
        }

        private enum OwnerType
        {
            Inspector,
            ParticleSystemWindow
        }

        private enum PlayState
        {
            Stopped,
            Playing,
            Paused
        }

        internal class Texts
        {
            public GUIContent addParticleSystem = new GUIContent(string.Empty, "Create Particle System");
            public GUIContent particleCount = new GUIContent("Particle Count");
            public GUIContent pause = new GUIContent("Pause");
            public GUIContent play = new GUIContent("Simulate");
            public GUIContent previewSpeed = new GUIContent("Playback Speed");
            public GUIContent previewTime = new GUIContent("Playback Time");
            public GUIContent resimulation = new GUIContent("Resimulate", "If resimulate is enabled the particle system will show changes made to the system immediately (including changes made to the particle system transform)");
            public string secondsFloatFieldFormatString = "f2";
            public GUIContent stop = new GUIContent("Stop");
            public GUIContent wireframe = new GUIContent("Wireframe", "Show particles with wireframe and particle system bounds");
        }
    }
}

