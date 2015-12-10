namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerEffectView
    {
        private const int kLabelWidth = 170;
        private const float kMaxPitch = 10f;
        private const float kMinPitch = 0.01f;
        private const int kTextboxWidth = 70;
        private Dictionary<string, IAudioEffectPluginGUI> m_CustomEffectGUIs;
        private readonly EffectDragging m_EffectDragging;
        private int m_LastNumChannels;
        private AudioMixerGroupController m_PrevGroup;
        private AudioMixerEffectPlugin m_SharedPlugin;

        public AudioMixerEffectView()
        {
            <AudioMixerEffectView>c__AnonStorey5F storeyf = new <AudioMixerEffectView>c__AnonStorey5F();
            this.m_SharedPlugin = new AudioMixerEffectPlugin();
            this.m_CustomEffectGUIs = new Dictionary<string, IAudioEffectPluginGUI>();
            this.m_EffectDragging = new EffectDragging();
            storeyf.pluginType = typeof(IAudioEffectPluginGUI);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    IEnumerator<Type> enumerator = assembly.GetTypes().Where<Type>(new Func<Type, bool>(storeyf.<>m__A7)).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Type current = enumerator.Current;
                            this.RegisterCustomGUI(Activator.CreateInstance(current) as IAudioEffectPluginGUI);
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void DoEffectGUI(int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, ref int highlightEffectIndex)
        {
            Event current = Event.current;
            AudioMixerController controller = group.controller;
            AudioMixerEffectController effect = group.effects[effectIndex];
            MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(effect.effectName);
            Rect effectRect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            bool flag = effectRect.Contains(current.mousePosition);
            EventType typeForControl = current.GetTypeForControl(this.m_EffectDragging.dragControlID);
            if (((typeForControl == EventType.MouseMove) && flag) && (highlightEffectIndex != effectIndex))
            {
                highlightEffectIndex = effectIndex;
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }
            Rect rect = GUILayoutUtility.GetRect((float) 1f, (float) 17f);
            Rect rect3 = new Rect(rect.x + 6f, rect.y + 5f, 6f, 6f);
            Rect position = new Rect((rect.x + 8f) + 6f, rect.y, (((rect.width - 8f) - 6f) - 14f) - 5f, rect.height);
            Rect rect5 = new Rect(position.xMax, rect.y, 14f, 14f);
            Rect rect6 = new Rect(rect.x, rect.y, (rect.width - 14f) - 5f, rect.height);
            bool flag2 = EditorPrefs.GetBool(AudioMixerGroupEditor.kPrefKeyForShowCpuUsage, false) && EditorUtility.audioProfilingEnabled;
            float r = !EditorGUIUtility.isProSkin ? 1f : 0.1f;
            Color color = new Color(r, r, r, 0.2f);
            Color color2 = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            GUI.color = color2;
            Color effectColor = AudioMixerDrawUtils.GetEffectColor(effect);
            EditorGUI.DrawRect(rect3, effectColor);
            GUI.Label(position, !flag2 ? effect.effectName : (effect.effectName + string.Format(Texts.cpuFormatString, effect.GetCPUUsage(controller))), EditorStyles.boldLabel);
            if (EditorGUI.ButtonMouseDown(rect5, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.inspectorTitlebarText))
            {
                ShowEffectContextMenu(group, effect, effectIndex, controller, rect5);
            }
            if ((current.type == EventType.ContextClick) && rect.Contains(current.mousePosition))
            {
                ShowEffectContextMenu(group, effect, effectIndex, controller, new Rect(current.mousePosition.x, rect.y, 1f, rect.height));
                current.Use();
            }
            if (typeForControl == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(rect6, MouseCursor.ResizeVertical, this.m_EffectDragging.dragControlID);
            }
            EditorGUI.BeginDisabledGroup(effect.bypass || group.bypassEffects);
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            if (effect.IsAttenuation())
            {
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                float valueForVolume = group.GetValueForVolume(controller, controller.TargetSnapshot);
                if (AudioMixerEffectGUI.Slider(Texts.volume, ref valueForVolume, 1f, 1f, Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume(), controller, new AudioGroupParameterPath(group, group.GetGUIDForVolume()), new GUILayoutOption[0]))
                {
                    Undo.RecordObject(controller.TargetSnapshot, "Change Volume Fader");
                    group.SetValueForVolume(controller, controller.TargetSnapshot, valueForVolume);
                    AudioMixerUtility.RepaintAudioMixerAndInspectors();
                }
                int lastNumChannels = 0;
                float[] vuLevel = new float[9];
                float[] vuPeak = new float[9];
                lastNumChannels = group.controller.GetGroupVUInfo(group.groupID, true, ref vuLevel, ref vuPeak);
                if (current.type == EventType.Layout)
                {
                    this.m_LastNumChannels = lastNumChannels;
                }
                else
                {
                    if (lastNumChannels != this.m_LastNumChannels)
                    {
                        HandleUtility.Repaint();
                    }
                    lastNumChannels = this.m_LastNumChannels;
                }
                GUILayout.Space(4f);
                for (int i = 0; i < lastNumChannels; i++)
                {
                    float num7 = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(vuLevel[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
                    float peak = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(vuPeak[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(10f) };
                    EditorGUILayout.VUMeterHorizontal(num7, peak, options);
                    if (!EditorApplication.isPlaying && (peak > 0f))
                    {
                        AudioMixerUtility.RepaintAudioMixerAndInspectors();
                    }
                }
                GUILayout.Space(4f);
                EditorGUILayout.EndVertical();
            }
            if (effect.IsSend())
            {
                Rect rect7;
                GUIContent buttonContent = (effect.sendTarget != null) ? GUIContent.Temp(effect.GetSendTargetDisplayString(effectMap)) : Texts.none;
                if (AudioMixerEffectGUI.PopupButton(Texts.bus, buttonContent, EditorStyles.popup, out rect7, new GUILayoutOption[0]))
                {
                    ShowBusPopupMenu(effectIndex, group, allGroups, effectMap, effect, rect7);
                }
                if (effect.sendTarget != null)
                {
                    float valueForMixLevel = effect.GetValueForMixLevel(controller, controller.TargetSnapshot);
                    if (AudioMixerEffectGUI.Slider(Texts.sendLevel, ref valueForMixLevel, 1f, 1f, Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect, controller, new AudioEffectParameterPath(group, effect, effect.GetGUIDForMixLevel()), new GUILayoutOption[0]))
                    {
                        Undo.RecordObject(controller.TargetSnapshot, "Change Send Level");
                        effect.SetValueForMixLevel(controller, controller.TargetSnapshot, valueForMixLevel);
                        AudioMixerUtility.RepaintAudioMixerAndInspectors();
                    }
                }
            }
            if (MixerEffectDefinitions.EffectCanBeSidechainTarget(effect))
            {
                bool flag3 = false;
                foreach (AudioMixerGroupController controller3 in allGroups)
                {
                    foreach (AudioMixerEffectController controller4 in controller3.effects)
                    {
                        if (controller4.IsSend() && (controller4.sendTarget == effect))
                        {
                            flag3 = true;
                            break;
                        }
                        if (flag3)
                        {
                            break;
                        }
                    }
                    if (flag3)
                    {
                        break;
                    }
                }
                if (!flag3)
                {
                    GUILayout.Label(new GUIContent("No Send sources connected.", EditorGUIUtility.warningIcon), new GUILayoutOption[0]);
                }
            }
            if (((effect.enableWetMix && !effect.IsReceive()) && (!effect.IsDuckVolume() && !effect.IsAttenuation())) && !effect.IsSend())
            {
                float num11 = effect.GetValueForMixLevel(controller, controller.TargetSnapshot);
                if (AudioMixerEffectGUI.Slider(Texts.wet, ref num11, 1f, 1f, Texts.dB, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect, controller, new AudioEffectParameterPath(group, effect, effect.GetGUIDForMixLevel()), new GUILayoutOption[0]))
                {
                    Undo.RecordObject(controller.TargetSnapshot, "Change Mix Level");
                    effect.SetValueForMixLevel(controller, controller.TargetSnapshot, num11);
                    AudioMixerUtility.RepaintAudioMixerAndInspectors();
                }
            }
            bool flag4 = true;
            if (this.m_CustomEffectGUIs.ContainsKey(effect.effectName))
            {
                IAudioEffectPluginGUI ngui = this.m_CustomEffectGUIs[effect.effectName];
                this.m_SharedPlugin.m_Controller = controller;
                this.m_SharedPlugin.m_Effect = effect;
                this.m_SharedPlugin.m_ParamDefs = effectParameters;
                flag4 = ngui.OnGUI(this.m_SharedPlugin);
            }
            if (flag4)
            {
                foreach (MixerParameterDefinition definition in effectParameters)
                {
                    float num13 = effect.GetValueForParameter(controller, controller.TargetSnapshot, definition.name);
                    if (AudioMixerEffectGUI.Slider(GUIContent.Temp(definition.name, definition.description), ref num13, definition.displayScale, definition.displayExponent, definition.units, definition.minRange, definition.maxRange, controller, new AudioEffectParameterPath(group, effect, effect.GetGUIDForParameter(definition.name)), new GUILayoutOption[0]))
                    {
                        Undo.RecordObject(controller.TargetSnapshot, "Change " + definition.name);
                        effect.SetValueForParameter(controller, controller.TargetSnapshot, definition.name, num13);
                    }
                }
                if (effectParameters.Length > 0)
                {
                    GUILayout.Space(6f);
                }
            }
            EditorGUI.EndDisabledGroup();
            this.m_EffectDragging.HandleDragElement(effectIndex, effectRect, rect6, group, allGroups);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            AudioMixerDrawUtils.DrawSplitter();
        }

        public static float DoInitialModule(AudioMixerGroupController group, AudioMixerController controller, List<AudioMixerGroupController> allGroups)
        {
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            float valueForPitch = group.GetValueForPitch(controller, controller.TargetSnapshot);
            if (AudioMixerEffectGUI.Slider(Texts.pitch, ref valueForPitch, 100f, 1f, Texts.percentage, 0.01f, 10f, controller, new AudioGroupParameterPath(group, group.GetGUIDForPitch()), new GUILayoutOption[0]))
            {
                Undo.RecordObject(controller.TargetSnapshot, "Change Pitch");
                group.SetValueForPitch(controller, controller.TargetSnapshot, valueForPitch);
            }
            GUILayout.Space(5f);
            EditorGUILayout.EndVertical();
            AudioMixerDrawUtils.DrawSplitter();
            return rect.height;
        }

        public void OnGUI(AudioMixerGroupController group)
        {
            if (group != null)
            {
                AudioMixerController controller = group.controller;
                List<AudioMixerGroupController> allAudioGroupsSlow = controller.GetAllAudioGroupsSlow();
                Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap = new Dictionary<AudioMixerEffectController, AudioMixerGroupController>();
                foreach (AudioMixerGroupController controller2 in allAudioGroupsSlow)
                {
                    foreach (AudioMixerEffectController controller3 in controller2.effects)
                    {
                        effectMap[controller3] = controller2;
                    }
                }
                Rect totalRect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                if (EditorApplication.isPlaying)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
                    GUILayout.Toggle(AudioSettings.editingInPlaymode, Texts.editInPlaymode, EditorStyles.miniButton, options);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AudioSettings.editingInPlaymode = !AudioSettings.editingInPlaymode;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                EditorGUI.BeginDisabledGroup(!AudioMixerController.EditingTargetSnapshot());
                if (group != this.m_PrevGroup)
                {
                    this.m_PrevGroup = group;
                    controller.m_HighlightEffectIndex = -1;
                    AudioMixerUtility.RepaintAudioMixerAndInspectors();
                }
                DoInitialModule(group, controller, allAudioGroupsSlow);
                for (int i = 0; i < group.effects.Length; i++)
                {
                    this.DoEffectGUI(i, group, allAudioGroupsSlow, effectMap, ref controller.m_HighlightEffectIndex);
                }
                this.m_EffectDragging.HandleDragging(totalRect, group, controller);
                GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (EditorGUILayout.ButtonMouseDown(Texts.addEffect, FocusType.Passive, GUISkin.current.button, new GUILayoutOption[0]))
                {
                    GenericMenu pm = new GenericMenu();
                    Rect last = GUILayoutUtility.topLevel.GetLast();
                    AudioMixerGroupController[] groups = new AudioMixerGroupController[] { group };
                    AudioMixerChannelStripView.AddEffectItemsToMenu(controller, groups, group.effects.Length, string.Empty, pm);
                    pm.DropDown(last);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
        }

        public bool RegisterCustomGUI(IAudioEffectPluginGUI gui)
        {
            string name = gui.Name;
            if (this.m_CustomEffectGUIs.ContainsKey(name))
            {
                IAudioEffectPluginGUI ngui = this.m_CustomEffectGUIs[name];
                Debug.LogError("Attempt to register custom GUI for plugin " + name + " failed as another plugin is already registered under this name.");
                Debug.LogError("Plugin trying to register itself: " + gui.Description + " (Vendor: " + gui.Vendor + ")");
                Debug.LogError("Plugin already registered: " + ngui.Description + " (Vendor: " + ngui.Vendor + ")");
                return false;
            }
            this.m_CustomEffectGUIs[name] = gui;
            return true;
        }

        private static void ShowBusPopupMenu(int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, AudioMixerEffectController effect, Rect buttonRect)
        {
            GenericMenu pm = new GenericMenu();
            pm.AddItem(new GUIContent("None"), false, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback), new AudioMixerChannelStripView.ConnectSendContext(effect, null));
            pm.AddSeparator(string.Empty);
            AudioMixerChannelStripView.AddMenuItemsForReturns(pm, string.Empty, effectIndex, group, allGroups, effectMap, effect, true);
            if (pm.GetItemCount() == 2)
            {
                pm.AddDisabledItem(new GUIContent("No valid Receive targets found"));
            }
            pm.DropDown(buttonRect);
        }

        private static void ShowEffectContextMenu(AudioMixerGroupController group, AudioMixerEffectController effect, int effectIndex, AudioMixerController controller, Rect buttonRect)
        {
            <ShowEffectContextMenu>c__AnonStorey60 storey = new <ShowEffectContextMenu>c__AnonStorey60 {
                effect = effect,
                controller = controller,
                group = group,
                effectIndex = effectIndex
            };
            GenericMenu pm = new GenericMenu();
            if (!storey.effect.IsReceive())
            {
                if ((!storey.effect.IsAttenuation() && !storey.effect.IsSend()) && !storey.effect.IsDuckVolume())
                {
                    pm.AddItem(new GUIContent("Allow Wet Mixing (causes higher memory usage)"), storey.effect.enableWetMix, new GenericMenu.MenuFunction(storey.<>m__A8));
                    pm.AddItem(new GUIContent("Bypass"), storey.effect.bypass, new GenericMenu.MenuFunction(storey.<>m__A9));
                    pm.AddSeparator(string.Empty);
                }
                pm.AddItem(new GUIContent("Copy effect settings to all snapshots"), false, new GenericMenu.MenuFunction(storey.<>m__AA));
                if ((!storey.effect.IsAttenuation() && !storey.effect.IsSend()) && (!storey.effect.IsDuckVolume() && storey.effect.enableWetMix))
                {
                    pm.AddItem(new GUIContent("Copy effect settings to all snapshots, including wet level"), false, new GenericMenu.MenuFunction(storey.<>m__AB));
                }
                pm.AddSeparator(string.Empty);
            }
            AudioMixerGroupController[] groups = new AudioMixerGroupController[] { storey.group };
            AudioMixerChannelStripView.AddEffectItemsToMenu(storey.controller, groups, storey.effectIndex, "Add effect before/", pm);
            AudioMixerChannelStripView.AddEffectItemsToMenu(storey.controller, groups, storey.effectIndex + 1, "Add effect after/", pm);
            if (!storey.effect.IsAttenuation())
            {
                pm.AddSeparator(string.Empty);
                pm.AddItem(new GUIContent("Remove this effect"), false, new GenericMenu.MenuFunction(storey.<>m__AC));
            }
            pm.DropDown(buttonRect);
        }

        [CompilerGenerated]
        private sealed class <AudioMixerEffectView>c__AnonStorey5F
        {
            internal Type pluginType;

            internal bool <>m__A7(Type t)
            {
                return (!t.IsAbstract && this.pluginType.IsAssignableFrom(t));
            }
        }

        [CompilerGenerated]
        private sealed class <ShowEffectContextMenu>c__AnonStorey60
        {
            internal AudioMixerController controller;
            internal AudioMixerEffectController effect;
            internal int effectIndex;
            internal AudioMixerGroupController group;

            internal void <>m__A8()
            {
                this.effect.enableWetMix = !this.effect.enableWetMix;
            }

            internal void <>m__A9()
            {
                this.effect.bypass = !this.effect.bypass;
                this.controller.UpdateBypass();
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }

            internal void <>m__AA()
            {
                Undo.RecordObject(this.controller, "Copy effect settings to all snapshots");
                if (this.effect.IsAttenuation())
                {
                    this.controller.CopyAttenuationToAllSnapshots(this.group, this.controller.TargetSnapshot);
                }
                else
                {
                    this.controller.CopyEffectSettingsToAllSnapshots(this.group, this.effectIndex, this.controller.TargetSnapshot, this.effect.IsSend());
                }
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }

            internal void <>m__AB()
            {
                Undo.RecordObject(this.controller, "Copy effect settings to all snapshots, including wet level");
                this.controller.CopyEffectSettingsToAllSnapshots(this.group, this.effectIndex, this.controller.TargetSnapshot, true);
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }

            internal void <>m__AC()
            {
                this.controller.ClearSendConnectionsTo(this.effect);
                this.controller.RemoveEffect(this.effect, this.group);
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }
        }

        private class EffectDragging
        {
            private readonly Color kMoveColorBorderAllowed = new Color(1f, 1f, 1f, 1f);
            private readonly Color kMoveColorBorderDisallowed = new Color(0.8f, 0f, 0f, 1f);
            private readonly Color kMoveColorHiAllowed = new Color(1f, 1f, 1f, 0.3f);
            private readonly Color kMoveColorHiDisallowed = new Color(1f, 0f, 0f, 0.3f);
            private readonly Color kMoveColorLoAllowed = new Color(1f, 1f, 1f, 0f);
            private readonly Color kMoveColorLoDisallowed = new Color(1f, 0f, 0f, 0f);
            private readonly int m_DragControlID = GUIUtility.GetPermanentControlID();
            private float m_DragHighlightHeight = 2f;
            private float m_DragHighlightPos = -1f;
            private int m_MovingDstIndex = -1;
            private bool m_MovingEffectAllowed;
            private float m_MovingPos;
            private Rect m_MovingRect = new Rect(0f, 0f, 0f, 0f);
            private int m_MovingSrcIndex = -1;

            public void HandleDragElement(int effectIndex, Rect effectRect, Rect dragRect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups)
            {
                Event current = Event.current;
                switch (current.GetTypeForControl(this.m_DragControlID))
                {
                    case EventType.MouseDown:
                        if (((current.button == 0) && dragRect.Contains(current.mousePosition)) && (GUIUtility.hotControl == 0))
                        {
                            this.m_MovingSrcIndex = effectIndex;
                            this.m_MovingPos = current.mousePosition.y;
                            this.m_MovingRect = new Rect(effectRect.x, effectRect.y - this.m_MovingPos, effectRect.width, effectRect.height);
                            GUIUtility.hotControl = this.m_DragControlID;
                            EditorGUIUtility.SetWantsMouseJumping(1);
                            current.Use();
                        }
                        break;

                    case EventType.Repaint:
                        if (effectIndex == this.m_MovingSrcIndex)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            AudioMixerDrawUtils.styles.channelStripAreaBackground.Draw(effectRect, false, false, false, false);
                            EditorGUI.EndDisabledGroup();
                        }
                        break;
                }
                if (this.isDragging)
                {
                    float num = effectRect.height * 0.5f;
                    float f = (current.mousePosition.y - effectRect.y) - num;
                    if (Mathf.Abs(f) <= num)
                    {
                        int targetIndex = (f >= 0f) ? (effectIndex + 1) : effectIndex;
                        if (targetIndex != this.m_MovingDstIndex)
                        {
                            this.m_DragHighlightPos = (f >= 0f) ? (effectRect.y + effectRect.height) : effectRect.y;
                            this.m_MovingDstIndex = targetIndex;
                            this.m_MovingEffectAllowed = !AudioMixerController.WillMovingEffectCauseFeedback(allGroups, group, this.m_MovingSrcIndex, group, targetIndex, null);
                        }
                    }
                    if ((this.m_MovingDstIndex == this.m_MovingSrcIndex) || (this.m_MovingDstIndex == (this.m_MovingSrcIndex + 1)))
                    {
                        this.m_DragHighlightPos = 0f;
                    }
                }
            }

            public void HandleDragging(Rect totalRect, AudioMixerGroupController group, AudioMixerController controller)
            {
                if (this.isDragging)
                {
                    Event current = Event.current;
                    EventType typeForControl = current.GetTypeForControl(this.m_DragControlID);
                    switch (typeForControl)
                    {
                        case EventType.MouseUp:
                            current.Use();
                            if (this.m_MovingSrcIndex != -1)
                            {
                                if ((this.m_MovingDstIndex != -1) && this.m_MovingEffectAllowed)
                                {
                                    List<AudioMixerEffectController> sourceEffects = group.effects.ToList<AudioMixerEffectController>();
                                    if (AudioMixerController.MoveEffect(ref sourceEffects, this.m_MovingSrcIndex, ref sourceEffects, this.m_MovingDstIndex))
                                    {
                                        group.effects = sourceEffects.ToArray();
                                    }
                                }
                                this.m_MovingSrcIndex = -1;
                                this.m_MovingDstIndex = -1;
                                controller.m_HighlightEffectIndex = -1;
                                if (GUIUtility.hotControl == this.m_DragControlID)
                                {
                                    GUIUtility.hotControl = 0;
                                }
                                EditorGUIUtility.SetWantsMouseJumping(0);
                                AudioMixerUtility.RepaintAudioMixerAndInspectors();
                                GUIUtility.ExitGUI();
                            }
                            return;

                        case EventType.MouseDrag:
                            this.m_MovingPos = current.mousePosition.y;
                            current.Use();
                            return;
                    }
                    if ((typeForControl == EventType.Repaint) && (this.m_DragHighlightPos > 0f))
                    {
                        float width = totalRect.width;
                        Color color = !this.m_MovingEffectAllowed ? this.kMoveColorLoDisallowed : this.kMoveColorLoAllowed;
                        Color color2 = !this.m_MovingEffectAllowed ? this.kMoveColorHiDisallowed : this.kMoveColorHiAllowed;
                        Color color3 = !this.m_MovingEffectAllowed ? this.kMoveColorBorderDisallowed : this.kMoveColorBorderAllowed;
                        AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos - 15f, width, 15f), color, color2);
                        AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos, width, 15f), color2, color);
                        AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingRect.x, this.m_DragHighlightPos - (this.m_DragHighlightHeight / 2f), width, this.m_DragHighlightHeight), color3, color3);
                    }
                }
            }

            public bool IsDraggingIndex(int effectIndex)
            {
                return ((this.m_MovingSrcIndex == effectIndex) && (GUIUtility.hotControl == this.m_DragControlID));
            }

            public int dragControlID
            {
                get
                {
                    return this.m_DragControlID;
                }
            }

            private bool isDragging
            {
                get
                {
                    return ((this.m_MovingSrcIndex != -1) && (GUIUtility.hotControl == this.m_DragControlID));
                }
            }
        }

        private static class Texts
        {
            public static GUIContent addEffect = new GUIContent("Add Effect");
            public static GUIContent bus = new GUIContent("Receive");
            public static string cpuFormatString = " - CPU: {0:#0.00}%";
            public static string dB = "dB";
            public static GUIContent editInPlaymode = new GUIContent("Edit in Playmode");
            public static GUIContent none = new GUIContent("None");
            public static string percentage = "%";
            public static GUIContent pitch = new GUIContent("Pitch");
            public static GUIContent sendLevel = new GUIContent("Send level");
            public static GUIContent volume = new GUIContent("Volume");
            public static GUIContent wet = new GUIContent("Wet", "Enables/disables wet/dry ratio on this effect. Note that this makes the DSP graph more complex and requires additional CPU and memory, so use it only when necessary.");
        }
    }
}

