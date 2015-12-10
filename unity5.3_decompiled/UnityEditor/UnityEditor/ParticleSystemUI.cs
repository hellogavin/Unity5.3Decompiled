namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ParticleSystemUI
    {
        public ModuleUI[] m_Modules;
        public ParticleEffectUI m_ParticleEffectUI;
        public ParticleSystem m_ParticleSystem;
        public SerializedObject m_ParticleSystemSerializedObject;
        public SerializedObject m_RendererSerializedObject;
        private static string m_SupportsCullingText;
        private static string[] s_ModuleNames;
        private static Texts s_Texts;

        private void AddModuleCallback(object obj)
        {
            int index = (int) obj;
            if ((index >= 0) && (index < this.m_Modules.Length))
            {
                if (index == (this.m_Modules.Length - 1))
                {
                    this.InitRendererUI();
                }
                else
                {
                    this.m_Modules[index].enabled = true;
                    this.m_Modules[index].foldout = true;
                }
            }
            else
            {
                this.m_ParticleEffectUI.SetAllModulesVisible(!ParticleEffectUI.GetAllModulesVisible());
            }
            this.ApplyProperties();
        }

        public void ApplyProperties()
        {
            bool hasModifiedProperties = this.m_ParticleSystemSerializedObject.hasModifiedProperties;
            this.m_ParticleSystemSerializedObject.ApplyModifiedProperties();
            if (hasModifiedProperties)
            {
                if (!ParticleEffectUI.IsStopped(ParticleSystemEditorUtils.GetRoot(this.m_ParticleSystem)) && ParticleSystemEditorUtils.editorResimulation)
                {
                    ParticleSystemEditorUtils.PerformCompleteResimulation();
                }
                this.UpdateParticleSystemInfoString();
            }
            if (this.m_RendererSerializedObject != null)
            {
                this.m_RendererSerializedObject.ApplyModifiedProperties();
            }
        }

        private void ClearRenderer()
        {
            this.m_RendererSerializedObject = null;
            ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
            if (particleSystemRenderer != null)
            {
                Undo.DestroyObjectImmediate(particleSystemRenderer);
            }
            this.m_Modules[this.m_Modules.Length - 1] = null;
        }

        private static ModuleUI[] CreateUIModules(ParticleSystemUI e, SerializedObject so)
        {
            int num = 0;
            ModuleUI[] euiArray1 = new ModuleUI[0x12];
            euiArray1[0] = new InitialModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[1] = new EmissionModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[2] = new ShapeModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[3] = new VelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[4] = new ClampVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[5] = new InheritVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[6] = new ForceModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[7] = new ColorModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[8] = new ColorByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[9] = new SizeModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[10] = new SizeByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[11] = new RotationModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[12] = new RotationByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[13] = new ExternalForcesModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[14] = new CollisionModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[15] = new SubModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x10] = new UVModuleUI(e, so, s_ModuleNames[num++]);
            return euiArray1;
        }

        private void EmitterMenuCallback(object obj)
        {
            switch (((int) obj))
            {
                case 0:
                    this.m_ParticleEffectUI.CreateParticleSystem(this.m_ParticleSystem, SubModuleUI.SubEmitterType.None);
                    break;

                case 1:
                    this.ResetModules();
                    break;

                case 2:
                    EditorGUIUtility.PingObject(this.m_ParticleSystem);
                    break;
            }
        }

        public float GetEmitterDuration()
        {
            InitialModuleUI eui = this.m_Modules[0] as InitialModuleUI;
            if (eui != null)
            {
                return eui.m_LengthInSec.floatValue;
            }
            return -1f;
        }

        public string GetName()
        {
            return this.m_ParticleSystem.gameObject.name;
        }

        internal ParticleSystemRenderer GetParticleSystemRenderer()
        {
            if (this.m_ParticleSystem != null)
            {
                return this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
            }
            return null;
        }

        internal ModuleUI GetParticleSystemRendererModuleUI()
        {
            return this.m_Modules[this.m_Modules.Length - 1];
        }

        private ParticleSystem GetSelectedParticleSystem()
        {
            return Selection.activeGameObject.GetComponent<ParticleSystem>();
        }

        public static string[] GetUIModuleNames()
        {
            return new string[] { 
                string.Empty, "Emission", "Shape", "Velocity over Lifetime", "Limit Velocity over Lifetime", "Inherit Velocity", "Force over Lifetime", "Color over Lifetime", "Color by Speed", "Size over Lifetime", "Size by Speed", "Rotation over Lifetime", "Rotation by Speed", "External Forces", "Collision", "Sub Emitters", 
                "Texture Sheet Animation", "Renderer"
             };
        }

        public void Init(ParticleEffectUI owner, ParticleSystem ps)
        {
            if (s_ModuleNames == null)
            {
                s_ModuleNames = GetUIModuleNames();
            }
            this.m_ParticleEffectUI = owner;
            this.m_ParticleSystem = ps;
            this.m_ParticleSystemSerializedObject = new SerializedObject(this.m_ParticleSystem);
            this.m_RendererSerializedObject = null;
            m_SupportsCullingText = null;
            this.m_Modules = CreateUIModules(this, this.m_ParticleSystemSerializedObject);
            if (this.GetParticleSystemRenderer() != null)
            {
                this.InitRendererUI();
            }
            this.UpdateParticleSystemInfoString();
        }

        private void InitRendererUI()
        {
            if (this.GetParticleSystemRenderer() == null)
            {
                this.m_ParticleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
            }
            ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
            if (particleSystemRenderer != null)
            {
                this.m_RendererSerializedObject = new SerializedObject(particleSystemRenderer);
                this.m_Modules[this.m_Modules.Length - 1] = new RendererModuleUI(this, this.m_RendererSerializedObject, s_ModuleNames[s_ModuleNames.Length - 1]);
                EditorUtility.SetSelectedWireframeHidden(particleSystemRenderer, !ParticleEffectUI.m_ShowWireframe);
            }
        }

        private void ModuleMenuCallback(object obj)
        {
            int index = (int) obj;
            if (index == (this.m_Modules.Length - 1))
            {
                this.ClearRenderer();
            }
            else
            {
                if (!ParticleEffectUI.GetAllModulesVisible())
                {
                    this.m_Modules[index].visibleUI = false;
                }
                this.m_Modules[index].enabled = false;
            }
        }

        public void OnGUI(ParticleSystem root, float width, bool fixedWidth)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            bool flag = Event.current.type == EventType.Repaint;
            string str = (this.m_ParticleSystem == null) ? null : this.m_ParticleSystem.gameObject.name;
            if (fixedWidth)
            {
                EditorGUIUtility.labelWidth = width * 0.55f;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
                EditorGUILayout.BeginVertical(options);
            }
            else
            {
                EditorGUIUtility.labelWidth = 0f;
                EditorGUIUtility.labelWidth -= 4f;
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            }
            for (int i = 0; i < this.m_Modules.Length; i++)
            {
                GUIStyle emitterHeaderStyle;
                Rect rect;
                bool flag5;
                ModuleUI eui = this.m_Modules[i];
                if (eui == null)
                {
                    continue;
                }
                bool flag2 = eui == this.m_Modules[0];
                if (!eui.visibleUI && !flag2)
                {
                    continue;
                }
                GUIContent content = new GUIContent();
                if (flag2)
                {
                    rect = GUILayoutUtility.GetRect(width, 25f);
                    emitterHeaderStyle = ParticleSystemStyles.Get().emitterHeaderStyle;
                }
                else
                {
                    rect = GUILayoutUtility.GetRect(width, 15f);
                    emitterHeaderStyle = ParticleSystemStyles.Get().moduleHeaderStyle;
                }
                if (eui.foldout)
                {
                    EditorGUI.BeginDisabledGroup(!eui.enabled);
                    Rect rect2 = EditorGUILayout.BeginVertical(ParticleSystemStyles.Get().modulePadding, new GUILayoutOption[0]);
                    rect2.y -= 4f;
                    rect2.height += 4f;
                    GUI.Label(rect2, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
                    eui.OnInspectorGUI(this.m_ParticleSystem);
                    EditorGUILayout.EndVertical();
                    EditorGUI.EndDisabledGroup();
                }
                if (flag2)
                {
                    ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
                    float num2 = 21f;
                    Rect rect3 = new Rect(rect.x + 4f, rect.y + 2f, num2, num2);
                    if (flag && (particleSystemRenderer != null))
                    {
                        bool flag3 = false;
                        int instanceID = 0;
                        RendererModuleUI eui2 = this.m_Modules[this.m_Modules.Length - 1] as RendererModuleUI;
                        if (eui2 != null)
                        {
                            if (eui2.IsMeshEmitter())
                            {
                                if (particleSystemRenderer.mesh != null)
                                {
                                    instanceID = particleSystemRenderer.mesh.GetInstanceID();
                                }
                            }
                            else if (particleSystemRenderer.sharedMaterial != null)
                            {
                                instanceID = particleSystemRenderer.sharedMaterial.GetInstanceID();
                            }
                            if (EditorUtility.IsDirty(instanceID))
                            {
                                AssetPreview.ClearTemporaryAssetPreviews();
                            }
                        }
                        if (instanceID != 0)
                        {
                            Texture2D assetPreview = AssetPreview.GetAssetPreview(instanceID);
                            if (assetPreview != null)
                            {
                                GUI.DrawTexture(rect3, assetPreview, ScaleMode.StretchToFill, true);
                                flag3 = true;
                            }
                        }
                        if (!flag3)
                        {
                            GUI.Label(rect3, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
                        }
                    }
                    if (EditorGUI.ButtonMouseDown(rect3, GUIContent.none, FocusType.Passive, GUIStyle.none))
                    {
                        if (EditorGUI.actionKey)
                        {
                            List<int> list = new List<int>();
                            int item = this.m_ParticleSystem.gameObject.GetInstanceID();
                            list.AddRange(Selection.instanceIDs);
                            if (!list.Contains(item) || (list.Count != 1))
                            {
                                if (list.Contains(item))
                                {
                                    list.Remove(item);
                                }
                                else
                                {
                                    list.Add(item);
                                }
                            }
                            Selection.instanceIDs = list.ToArray();
                        }
                        else
                        {
                            Selection.instanceIDs = new int[0];
                            Selection.activeInstanceID = this.m_ParticleSystem.gameObject.GetInstanceID();
                        }
                    }
                }
                Rect position = new Rect(rect.x + 2f, rect.y + 1f, 13f, 13f);
                if (!flag2 && GUI.Button(position, GUIContent.none, GUIStyle.none))
                {
                    eui.enabled = !eui.enabled;
                }
                Rect rect5 = new Rect((rect.x + rect.width) - 10f, (rect.y + rect.height) - 10f, 10f, 10f);
                Rect rect6 = new Rect(rect5.x - 4f, rect5.y - 4f, rect5.width + 4f, rect5.height + 4f);
                Rect rect7 = new Rect(rect5.x - 23f, rect5.y - 3f, 16f, 16f);
                if (flag2 && EditorGUI.ButtonMouseDown(rect6, s_Texts.addModules, FocusType.Passive, GUIStyle.none))
                {
                    this.ShowAddModuleMenu();
                }
                if (!string.IsNullOrEmpty(str))
                {
                    content.text = !flag2 ? eui.displayName : str;
                }
                else
                {
                    content.text = eui.displayName;
                }
                content.tooltip = eui.toolTip;
                if (GUI.Toggle(rect, eui.foldout, content, emitterHeaderStyle) != eui.foldout)
                {
                    switch (Event.current.button)
                    {
                        case 0:
                            flag5 = !eui.foldout;
                            if (!Event.current.control)
                            {
                                goto Label_057E;
                            }
                            foreach (ModuleUI eui3 in this.m_Modules)
                            {
                                if ((eui3 != null) && eui3.visibleUI)
                                {
                                    eui3.foldout = flag5;
                                }
                            }
                            break;

                        case 1:
                            if (flag2)
                            {
                                this.ShowEmitterMenu();
                            }
                            else
                            {
                                this.ShowModuleMenu(i);
                            }
                            break;
                    }
                }
                goto Label_05A9;
            Label_057E:
                eui.foldout = flag5;
            Label_05A9:
                if (!flag2)
                {
                    GUI.Toggle(position, eui.enabled, GUIContent.none, ParticleSystemStyles.Get().checkmark);
                }
                if (flag && flag2)
                {
                    GUI.Label(rect5, GUIContent.none, ParticleSystemStyles.Get().plus);
                }
                s_Texts.supportsCullingText.tooltip = m_SupportsCullingText;
                if (flag2 && (s_Texts.supportsCullingText.tooltip != null))
                {
                    GUI.Label(rect7, s_Texts.supportsCullingText);
                }
                GUILayout.Space(1f);
            }
            GUILayout.Space(-1f);
            EditorGUILayout.EndVertical();
            this.ApplyProperties();
        }

        public void OnSceneGUI()
        {
            if ((this.m_Modules != null) && (this.m_ParticleSystem != null))
            {
                if (this.m_ParticleSystem.particleCount > 0)
                {
                    ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
                    EditorUtility.SetSelectedWireframeHidden(particleSystemRenderer, !ParticleEffectUI.m_ShowWireframe);
                    if (ParticleEffectUI.m_ShowWireframe)
                    {
                        ModuleUI particleSystemRendererModuleUI = this.GetParticleSystemRendererModuleUI();
                        ParticleSystemRenderer renderer2 = this.GetParticleSystemRenderer();
                        if (((particleSystemRendererModuleUI != null) && particleSystemRendererModuleUI.enabled) && renderer2.editorEnabled)
                        {
                            Vector3 extents = particleSystemRenderer.bounds.extents;
                            Transform transform = Camera.current.transform;
                            Vector2 size = new Vector2(0f, 0f);
                            Vector3[] vectorArray = new Vector3[] { new Vector3(-1f, -1f, -1f), new Vector3(-1f, -1f, 1f), new Vector3(-1f, 1f, -1f), new Vector3(-1f, 1f, 1f), new Vector3(1f, -1f, -1f), new Vector3(1f, -1f, 1f), new Vector3(1f, 1f, -1f), new Vector3(1f, 1f, 1f) };
                            for (int i = 0; i < 8; i++)
                            {
                                size.x = Mathf.Max(size.x, Vector3.Dot(Vector3.Scale(vectorArray[i], extents), transform.right));
                                size.y = Mathf.Max(size.y, Vector3.Dot(Vector3.Scale(vectorArray[i], extents), transform.up));
                            }
                            Handles.RectangleCap(0, particleSystemRenderer.bounds.center, Camera.current.transform.rotation, size);
                        }
                    }
                }
                this.UpdateProperties();
                InitialModuleUI initial = (InitialModuleUI) this.m_Modules[0];
                foreach (ModuleUI eui3 in this.m_Modules)
                {
                    if ((((eui3 != null) && eui3.visibleUI) && eui3.enabled) && eui3.foldout)
                    {
                        eui3.OnSceneGUI(this.m_ParticleSystem, initial);
                    }
                }
                this.ApplyProperties();
            }
        }

        private void ResetModules()
        {
            foreach (ModuleUI eui in this.m_Modules)
            {
                if (eui != null)
                {
                    eui.enabled = false;
                    if (!ParticleEffectUI.GetAllModulesVisible())
                    {
                        eui.visibleUI = false;
                    }
                }
            }
            if (this.m_Modules[this.m_Modules.Length - 1] == null)
            {
                this.InitRendererUI();
            }
            int[] numArray1 = new int[] { 1, 2, this.m_Modules.Length - 1 };
            foreach (int num3 in numArray1)
            {
                if (this.m_Modules[num3] != null)
                {
                    this.m_Modules[num3].enabled = true;
                    this.m_Modules[num3].visibleUI = true;
                }
            }
        }

        private void ShowAddModuleMenu()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < s_ModuleNames.Length; i++)
            {
                if ((this.m_Modules[i] == null) || !this.m_Modules[i].visibleUI)
                {
                    menu.AddItem(new GUIContent(s_ModuleNames[i]), false, new GenericMenu.MenuFunction2(this.AddModuleCallback), i);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(s_ModuleNames[i]));
                }
            }
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Show All Modules"), ParticleEffectUI.GetAllModulesVisible(), new GenericMenu.MenuFunction2(this.AddModuleCallback), 0x2710);
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void ShowEmitterMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Show Location"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 2);
            menu.AddSeparator(string.Empty);
            if (this.m_ParticleSystem.gameObject.activeInHierarchy)
            {
                menu.AddItem(new GUIContent("Create Particle System"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 0);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Create new Particle System"));
            }
            menu.AddItem(new GUIContent("Reset"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 1);
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void ShowModuleMenu(int moduleIndex)
        {
            GenericMenu menu = new GenericMenu();
            if (!ParticleEffectUI.GetAllModulesVisible())
            {
                menu.AddItem(new GUIContent("Remove"), false, new GenericMenu.MenuFunction2(this.ModuleMenuCallback), moduleIndex);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Remove"));
            }
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void UpdateParticleSystemInfoString()
        {
            string text = string.Empty;
            foreach (ModuleUI eui in this.m_Modules)
            {
                if (((eui != null) && eui.visibleUI) && eui.enabled)
                {
                    eui.UpdateCullingSupportedString(ref text);
                }
            }
            if (text != string.Empty)
            {
                m_SupportsCullingText = "Automatic culling is disabled because: " + text;
            }
            else
            {
                m_SupportsCullingText = null;
            }
        }

        public void UpdateProperties()
        {
            this.m_ParticleSystemSerializedObject.UpdateIfDirtyOrScript();
            if (this.m_RendererSerializedObject != null)
            {
                this.m_RendererSerializedObject.UpdateIfDirtyOrScript();
            }
        }

        public enum DefaultTypes
        {
            Root,
            SubBirth,
            SubCollision,
            SubDeath
        }

        protected class Texts
        {
            public GUIContent addModules = new GUIContent(string.Empty, "Show/Hide Modules");
            public GUIContent supportsCullingText = new GUIContent(string.Empty, ParticleSystemStyles.Get().warningIcon);
        }
    }
}

