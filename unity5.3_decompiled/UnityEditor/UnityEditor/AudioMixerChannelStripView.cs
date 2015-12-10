namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AudioMixerChannelStripView
    {
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, bool> <>f__am$cache30;
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, bool> <>f__am$cache31;
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, bool> <>f__am$cache32;
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, bool> <>f__am$cache33;
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, bool> <>f__am$cache34;
        [CompilerGenerated]
        private static Func<AudioMixerEffectController, bool> <>f__am$cache35;
        private GUIContent addText = new GUIContent("Add..");
        private GUIContent bypassButtonContent = new GUIContent(string.Empty, "Toggle bypass on this effect");
        [NonSerialized]
        private Vector3[] cablepoints = new Vector3[20];
        private const float channelStripBaseWidth = 90f;
        private readonly Vector2 channelStripsOffset = new Vector2(15f, 10f);
        private const int channelStripSpacing = 4;
        private const float dbHeight = 17f;
        [NonSerialized]
        private GUIStyle developerInfoStyle = AudioMixerDrawUtils.BuildGUIStyleForLabel(new Color(1f, 0f, 0f, 0.5f), 20, false, FontStyle.Bold, TextAnchor.MiddleLeft);
        private const float effectHeight = 16f;
        [NonSerialized]
        private int FrameCounter;
        private GUIContent headerGUIContent = new GUIContent();
        private const float headerHeight = 22f;
        private static Color hfaderCol1 = new Color(0.2f, 0.2f, 0.2f, 1f);
        private static Color hfaderCol2 = new Color(0.4f, 0.4f, 0.4f, 1f);
        private const float k_MinVULevel = -80f;
        private static int kEffectDraggingHashCode = "EffectDragging".GetHashCode();
        public static float kEffectScaleMouseDrag = 0.3f;
        private static readonly Color kGridColorDark = new Color(0f, 0f, 0f, 0.18f);
        private static readonly Color kGridColorLight = new Color(0f, 0f, 0f, 0.1f);
        private const float kGridTileWidth = 12f;
        private static Color kMoveColorHighlight = new Color(0.3f, 0.6f, 1f, 0.4f);
        private static Color kMoveSlotColBorderAllowed = new Color(1f, 1f, 1f, 1f);
        private static Color kMoveSlotColBorderDisallowed = new Color(1f, 0f, 0f, 1f);
        private static Color kMoveSlotColHiAllowed = new Color(1f, 1f, 1f, 0.7f);
        private static Color kMoveSlotColHiDisallowed = new Color(1f, 0f, 0f, 0.7f);
        private static Color kMoveSlotColLoAllowed = new Color(1f, 1f, 1f, 0f);
        private static Color kMoveSlotColLoDisallowed = new Color(0.8f, 0f, 0f, 0f);
        private static int kRectSelectionHashCode = "RectSelection".GetHashCode();
        private static int kVerticalFaderHash = "VerticalFader".GetHashCode();
        public static float kVolumeScaleMouseDrag = 1f;
        private int m_ChangingWetMixIndex = -1;
        private AudioMixerController m_Controller;
        public int m_EffectInteractionControlID;
        public int m_FocusIndex = -1;
        private static Texture2D m_GridTexture;
        private MixerGroupControllerCompareByName m_GroupComparer = new MixerGroupControllerCompareByName();
        public int m_IndexCounter;
        private List<int> m_LastNumChannels = new List<int>();
        public float m_MouseDragStartValue;
        public float m_MouseDragStartX;
        public float m_MouseDragStartY;
        private AudioMixerGroupController m_MovingDstGroup;
        private Rect m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);
        private bool m_MovingEffectAllowed;
        private int m_MovingEffectDstIndex = -1;
        private int m_MovingEffectSrcIndex = -1;
        private AudioMixerGroupController m_MovingSrcGroup;
        private Rect m_MovingSrcRect = new Rect(-1f, -1f, 0f, 0f);
        public int m_RectSelectionControlID;
        public Rect m_RectSelectionRect = new Rect(0f, 0f, 0f, 0f);
        public Vector2 m_RectSelectionStartPos = new Vector2(0f, 0f);
        private bool m_RequiresRepaint;
        private State m_State;
        private bool m_WaitingForDragEvent;
        public GUIStyle sharedGuiStyle = new GUIStyle();
        private const float soloMuteBypassHeight = 30f;
        private const float spaceBetween = 0f;
        private const float spaceBetweenMainGroupsAndReferenced = 50f;
        private const float vuHeight = 170f;

        public AudioMixerChannelStripView(State state)
        {
            this.m_State = state;
        }

        internal static void AddEffectItemsToMenu(AudioMixerController controller, AudioMixerGroupController[] groups, int insertIndex, string prefix, GenericMenu pm)
        {
            string[] effectList = MixerEffectDefinitions.GetEffectList();
            for (int i = 0; i < effectList.Length; i++)
            {
                if (effectList[i] != "Attenuation")
                {
                    pm.AddItem(new GUIContent(prefix + AudioMixerController.FixNameForPopupMenu(effectList[i])), false, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.InsertEffectPopupCallback), new EffectContext(controller, groups, insertIndex, effectList[i]));
                }
            }
        }

        public static void AddMenuItemsForReturns(GenericMenu pm, string prefix, int effectIndex, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, AudioMixerEffectController effect, bool showCurrent)
        {
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerEffectController controller2 in controller.effects)
                {
                    if (MixerEffectDefinitions.EffectCanBeSidechainTarget(controller2))
                    {
                        List<AudioMixerController.ConnectionNode> identifiedLoop = new List<AudioMixerController.ConnectionNode>();
                        if (!AudioMixerController.WillChangeOfEffectTargetCauseFeedback(allGroups, group, effectIndex, controller2, identifiedLoop))
                        {
                            if (showCurrent || (effect.sendTarget != controller2))
                            {
                                pm.AddItem(new GUIContent(prefix + "'" + controller2.GetDisplayString(effectMap) + "'"), effect.sendTarget == controller2, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback), new ConnectSendContext(effect, controller2));
                            }
                        }
                        else
                        {
                            string str = "A connection to '" + AudioMixerController.FixNameForPopupMenu(controller2.GetDisplayString(effectMap)) + "' would result in a feedback loop/";
                            pm.AddDisabledItem(new GUIContent(str + "Loop: "));
                            int num2 = 1;
                            foreach (AudioMixerController.ConnectionNode node in identifiedLoop)
                            {
                                object[] objArray1 = new object[] { str, num2, ": ", node.GetDisplayString(), "->" };
                                pm.AddDisabledItem(new GUIContent(string.Concat(objArray1)));
                                num2++;
                            }
                            pm.AddDisabledItem(new GUIContent(str + num2 + ": ..."));
                        }
                    }
                }
            }
        }

        private void AddSeperatorIfAnyReturns(GenericMenu pm, List<AudioMixerGroupController> allGroups)
        {
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerEffectController controller2 in controller.effects)
                {
                    if (controller2.IsReceive() || controller2.IsDuckVolume())
                    {
                        pm.AddSeparator(string.Empty);
                        break;
                    }
                }
            }
        }

        private bool CanDuplicateDraggedEffect()
        {
            return ((this.IsMovingEffect() && (this.m_MovingSrcGroup != null)) && !this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation());
        }

        private void ClearEffectDragging(ref int highlightEffectIndex)
        {
            if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
            {
                GUIUtility.hotControl = 0;
            }
            this.m_MovingEffectSrcIndex = -1;
            this.m_MovingEffectDstIndex = -1;
            this.m_MovingSrcRect = new Rect(-1f, -1f, 0f, 0f);
            this.m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);
            this.m_MovingSrcGroup = null;
            this.m_MovingDstGroup = null;
            this.m_ChangingWetMixIndex = -1;
            highlightEffectIndex = -1;
            this.ClearFocus();
            InspectorWindow.RepaintAllInspectors();
        }

        private void ClearFocus()
        {
            this.m_FocusIndex = -1;
        }

        private bool ClipRect(Rect r, Rect clipRect, ref Rect overlap)
        {
            overlap.x = Mathf.Max(r.x, clipRect.x);
            overlap.y = Mathf.Max(r.y, clipRect.y);
            overlap.width = Mathf.Min((float) (r.x + r.width), (float) (clipRect.x + clipRect.width)) - overlap.x;
            overlap.height = Mathf.Min((float) (r.y + r.height), (float) (clipRect.y + clipRect.height)) - overlap.y;
            return ((overlap.width > 0f) && (overlap.height > 0f));
        }

        public static void ConnectSendPopupCallback(object obj)
        {
            ConnectSendContext context = (ConnectSendContext) obj;
            Undo.RecordObject(context.sendEffect, "Change Send Target");
            context.sendEffect.sendTarget = context.targetEffect;
        }

        private static Texture2D CreateTilableGridTexture(int width, int height, Color backgroundColor, Color lineColor)
        {
            Color[] colors = new Color[width * height];
            for (int i = 0; i < (height * width); i++)
            {
                colors[i] = backgroundColor;
            }
            for (int j = 0; j < height; j++)
            {
                colors[(j * width) + (width - 1)] = lineColor;
            }
            for (int k = 0; k < width; k++)
            {
                colors[((height - 1) * width) + k] = lineColor;
            }
            Texture2D textured = new Texture2D(width, height, TextureFormat.ARGB32, false) {
                hideFlags = HideFlags.HideAndDontSave
            };
            textured.SetPixels(colors);
            textured.Apply();
            return textured;
        }

        private void DoAttenuationFader(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> selection, GUIStyle style)
        {
            float num = Mathf.Clamp(group.GetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot), AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume());
            float num2 = this.VerticalFader(r, num, 1, kVolumeScaleMouseDrag, true, true, this.styles.attenuationFader.tooltip, AudioMixerController.GetMaxVolume(), style);
            if (num != num2)
            {
                float num3 = num2 - num;
                Undo.RecordObject(this.m_Controller.TargetSnapshot, "Change volume fader");
                foreach (AudioMixerGroupController controller in selection)
                {
                    float num4 = Mathf.Clamp(controller.GetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot), AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume());
                    controller.SetValueForVolume(this.m_Controller, this.m_Controller.TargetSnapshot, Mathf.Clamp(num4 + num3, AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()));
                }
                InspectorWindow.RepaintAllInspectors();
            }
        }

        private bool DoBypassEffectsButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && r.Contains(current.mousePosition))
            {
                if (<>f__am$cache33 == null)
                {
                    <>f__am$cache33 = g => g.bypassEffects;
                }
                if (allGroups.Any<AudioMixerGroupController>(<>f__am$cache33))
                {
                    Undo.RecordObject(group, "Change bypass effects state");
                    foreach (AudioMixerGroupController controller in allGroups)
                    {
                        controller.bypassEffects = false;
                    }
                    current.Use();
                    return true;
                }
            }
            if (GUI.Toggle(r, group.bypassEffects, this.styles.bypassGUIContent, AudioMixerDrawUtils.styles.bypassToggle) == group.bypassEffects)
            {
                return false;
            }
            Undo.RecordObject(group, "Change bypass effects state");
            group.bypassEffects = !group.bypassEffects;
            foreach (AudioMixerGroupController controller2 in selection)
            {
                controller2.bypassEffects = group.bypassEffects;
            }
            return true;
        }

        private void DoEffectList(ChannelStripParams p, bool selected, ref int highlightEffectIndex, ref Dictionary<AudioMixerEffectController, PatchSlot> patchslots, bool showBusConnectionsOfSelection)
        {
            Event current = Event.current;
            for (int i = 0; i < p.maxEffects; i++)
            {
                Rect buttonRect = p.bgRects[p.kEffectStartIndex + i];
                if (i < p.group.effects.Length)
                {
                    AudioMixerEffectController effect = p.group.effects[i];
                    if (p.visible)
                    {
                        if ((current.type == EventType.ContextClick) && buttonRect.Contains(Event.current.mousePosition))
                        {
                            this.ClearFocus();
                            this.DoEffectSlotInsertEffectPopup(buttonRect, p.group, p.allGroups, i, ref p.effectMap);
                            current.Use();
                        }
                        this.EffectSlot(buttonRect, this.m_Controller.TargetSnapshot, effect, i, ref highlightEffectIndex, p, ref patchslots);
                    }
                }
            }
            if (p.visible)
            {
                Rect position = p.bgRects[p.bgRects.Count - 1];
                if (current.type == EventType.Repaint)
                {
                    GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, position.width, position.height - 1f), this.styles.effectBar.hover.background, new Rect(0f, 0.5f, 0.1f, 0.1f));
                    GUI.Label(position, this.addText, this.styles.effectName);
                }
                if ((current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
                {
                    this.ClearFocus();
                    int length = p.group.effects.Length;
                    this.DoEffectSlotInsertEffectPopup(position, p.group, p.allGroups, length, ref p.effectMap);
                    current.Use();
                }
            }
        }

        private void DoEffectSlotInsertEffectPopup(Rect buttonRect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, int effectSlotIndex, ref Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
        {
            GenericMenu pm = new GenericMenu();
            AudioMixerGroupController[] groups = new AudioMixerGroupController[] { group };
            if (effectSlotIndex < group.effects.Length)
            {
                <DoEffectSlotInsertEffectPopup>c__AnonStorey5E storeye = new <DoEffectSlotInsertEffectPopup>c__AnonStorey5E {
                    <>f__this = this,
                    effect = group.effects[effectSlotIndex]
                };
                if ((!storeye.effect.IsAttenuation() && !storeye.effect.IsSend()) && (!storeye.effect.IsReceive() && !storeye.effect.IsDuckVolume()))
                {
                    pm.AddItem(new GUIContent("Allow Wet Mixing (causes higher memory usage)"), storeye.effect.enableWetMix, new GenericMenu.MenuFunction(storeye.<>m__A3));
                    pm.AddItem(new GUIContent("Bypass"), storeye.effect.bypass, new GenericMenu.MenuFunction(storeye.<>m__A4));
                    pm.AddSeparator(string.Empty);
                }
                AddEffectItemsToMenu(group.controller, groups, effectSlotIndex, "Add effect before/", pm);
                AddEffectItemsToMenu(group.controller, groups, effectSlotIndex + 1, "Add effect after/", pm);
            }
            else
            {
                AddEffectItemsToMenu(group.controller, groups, effectSlotIndex, string.Empty, pm);
            }
            if (effectSlotIndex < group.effects.Length)
            {
                AudioMixerEffectController sendEffect = group.effects[effectSlotIndex];
                if (!sendEffect.IsAttenuation())
                {
                    pm.AddSeparator(string.Empty);
                    pm.AddItem(new GUIContent("Remove"), false, new GenericMenu.MenuFunction2(this.RemoveEffectPopupCallback), new EffectContext(this.m_Controller, groups, effectSlotIndex, string.Empty));
                    bool flag = false;
                    if (sendEffect.IsSend())
                    {
                        if (sendEffect.sendTarget != null)
                        {
                            if (!flag)
                            {
                                flag = true;
                                pm.AddSeparator(string.Empty);
                            }
                            pm.AddItem(new GUIContent("Disconnect from '" + sendEffect.GetSendTargetDisplayString(effectMap) + "'"), false, new GenericMenu.MenuFunction2(AudioMixerChannelStripView.ConnectSendPopupCallback), new ConnectSendContext(sendEffect, null));
                        }
                        if (!flag)
                        {
                            this.AddSeperatorIfAnyReturns(pm, allGroups);
                        }
                        AddMenuItemsForReturns(pm, "Connect to ", effectSlotIndex, group, allGroups, effectMap, sendEffect, false);
                    }
                }
            }
            pm.DropDown(buttonRect);
            Event.current.Use();
        }

        private bool DoMuteButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, bool anySoloActive, List<AudioMixerGroupController> selection)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && r.Contains(current.mousePosition))
            {
                if (<>f__am$cache31 == null)
                {
                    <>f__am$cache31 = g => g.mute;
                }
                if (allGroups.Any<AudioMixerGroupController>(<>f__am$cache31))
                {
                    Undo.RecordObject(group, "Change mute state");
                    if (<>f__am$cache32 == null)
                    {
                        <>f__am$cache32 = g => g.solo;
                    }
                    if (allGroups.Any<AudioMixerGroupController>(<>f__am$cache32))
                    {
                        return false;
                    }
                    foreach (AudioMixerGroupController controller in allGroups)
                    {
                        controller.mute = false;
                    }
                    current.Use();
                    return true;
                }
            }
            Color color = GUI.color;
            bool flag = anySoloActive && group.mute;
            if (flag)
            {
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
            }
            bool flag2 = GUI.Toggle(r, group.mute, this.styles.muteGUIContent, AudioMixerDrawUtils.styles.muteToggle);
            if (flag)
            {
                GUI.color = color;
            }
            if (flag2 == group.mute)
            {
                return false;
            }
            Undo.RecordObject(group, "Change mute state");
            group.mute = !group.mute;
            foreach (AudioMixerGroupController controller2 in selection)
            {
                controller2.mute = group.mute;
            }
            return true;
        }

        private bool DoSoloButton(Rect r, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && r.Contains(current.mousePosition))
            {
                if (<>f__am$cache30 == null)
                {
                    <>f__am$cache30 = g => g.solo;
                }
                if (allGroups.Any<AudioMixerGroupController>(<>f__am$cache30))
                {
                    Undo.RecordObject(group, "Change solo state");
                    foreach (AudioMixerGroupController controller in allGroups)
                    {
                        controller.solo = false;
                    }
                    current.Use();
                    return true;
                }
            }
            if (GUI.Toggle(r, group.solo, this.styles.soloGUIContent, AudioMixerDrawUtils.styles.soloToggle) == group.solo)
            {
                return false;
            }
            Undo.RecordObject(group, "Change solo state");
            group.solo = !group.solo;
            foreach (AudioMixerGroupController controller2 in selection)
            {
                controller2.solo = group.solo;
            }
            return true;
        }

        private void DoSoloMuteBypassButtons(Rect rect, AudioMixerGroupController group, List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> selection, bool anySoloActive)
        {
            float width = 21f;
            float num2 = 2f;
            float x = rect.x + ((rect.width - ((width * 3f) + (num2 * 2f))) * 0.5f);
            Rect r = new Rect(x, rect.y, width, width);
            bool flag = false;
            flag |= this.DoSoloButton(r, group, allGroups, selection);
            r.x += width + num2;
            if (flag | this.DoMuteButton(r, group, allGroups, anySoloActive, selection))
            {
                this.m_Controller.UpdateMuteSolo();
            }
            r.x += width + num2;
            if (this.DoBypassEffectsButton(r, group, allGroups, selection))
            {
                this.m_Controller.UpdateBypass();
            }
        }

        private void DoTotaldB(ChannelStripParams p)
        {
            float num = 50f;
            this.styles.totalVULevel.padding.right = (int) ((p.stripRect.width - num) * 0.5f);
            float num2 = Mathf.Max(p.vuinfo_level[8], -80f);
            Rect position = p.bgRects[p.kTotalVULevelIndex];
            GUI.Label(position, string.Format("{0:F1} dB", num2), this.styles.totalVULevel);
        }

        private float DoVUMeters(Rect vuRect, float attenuationMarkerHeight, ChannelStripParams p)
        {
            float num = 1f;
            int numChannels = p.numChannels;
            if (numChannels == 0)
            {
                if ((p.index >= 0) && (p.index < this.m_LastNumChannels.Count))
                {
                    numChannels = this.m_LastNumChannels[p.index];
                }
            }
            else
            {
                while (p.index >= this.m_LastNumChannels.Count)
                {
                    this.m_LastNumChannels.Add(0);
                }
                this.m_LastNumChannels[p.index] = numChannels;
            }
            if (numChannels <= 2)
            {
                vuRect.x = vuRect.xMax - 25f;
                vuRect.width = 25f;
            }
            if (numChannels == 0)
            {
                return vuRect.x;
            }
            float num4 = Mathf.Floor(attenuationMarkerHeight / 2f);
            vuRect.y += num4;
            vuRect.height -= 2f * num4;
            float width = Mathf.Round((vuRect.width - (numChannels * num)) / ((float) numChannels));
            Rect r = new Rect(vuRect.xMax - width, vuRect.y, width, vuRect.height);
            for (int i = numChannels - 1; i >= 0; i--)
            {
                if (i != (numChannels - 1))
                {
                    r.x -= r.width + num;
                }
                float level = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(p.vuinfo_level[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
                float peak = 1f - AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(p.vuinfo_peak[i], AudioMixerController.kMinVolume, AudioMixerController.GetMaxVolume()), 1f, true);
                this.VUMeter(p.group, r, level, peak);
            }
            AudioMixerDrawUtils.AddTooltipOverlay(vuRect, this.styles.vuMeterGUIContent.tooltip);
            return r.x;
        }

        private void DrawAreaBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, !EditorGUIUtility.isProSkin ? 0.2f : 0.6f);
                AudioMixerDrawUtils.styles.channelStripAreaBackground.Draw(rect, false, false, false, false);
                GUI.color = color;
            }
        }

        private void DrawBackgrounds(ChannelStripParams p, bool selected)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.styles.channelStripBg.Draw(p.stripRect, false, false, selected, false);
                float r = 0.4666667f;
                float num2 = 0.227451f;
                Color color = !EditorGUIUtility.isProSkin ? new Color(r, r, r) : new Color(num2, num2, num2);
                Rect rect = p.bgRects[p.kEffectStartIndex];
                rect.y--;
                rect.height = 1f;
                EditorGUI.DrawRect(rect, color);
            }
            Rect rect2 = p.bgRects[p.kVUMeterFaderIndex];
            rect2.height = !EditorGUIUtility.isProSkin ? ((float) 2) : ((float) 1);
            rect2.y -= rect2.height;
            int userColorIndex = p.group.userColorIndex;
            if (userColorIndex != 0)
            {
                EditorGUI.DrawRect(rect2, AudioMixerColorCodes.GetColor(userColorIndex));
            }
        }

        private void DrawChannelStrip(ChannelStripParams p, ref int highlightEffectIndex, ref Dictionary<AudioMixerEffectController, PatchSlot> patchslots, bool showBusConnectionsOfSelection)
        {
            Event current = Event.current;
            bool flag = (current.type == EventType.MouseDown) && p.stripRect.Contains(current.mousePosition);
            bool selected = this.m_Controller.CachedSelection.Contains(p.group);
            if (this.IsRectSelectionActive() && RectOverlaps(p.stripRect, this.m_RectSelectionRect))
            {
                p.rectSelectionGroups.Add(p.group);
                selected = true;
            }
            this.DrawBackgrounds(p, selected);
            string displayString = p.group.GetDisplayString();
            this.headerGUIContent.tooltip = displayString;
            this.headerGUIContent.text = displayString;
            GUI.Label(p.bgRects[p.kHeaderIndex], this.headerGUIContent, AudioMixerDrawUtils.styles.channelStripHeaderStyle);
            Rect rect = new RectOffset(-6, 0, 0, -4).Add(p.bgRects[p.kVUMeterFaderIndex]);
            float num = 1f;
            float width = 54f;
            float num3 = (rect.width - width) - num;
            Rect vuRect = new Rect(rect.x, rect.y, width, rect.height);
            Rect rect3 = new Rect(vuRect.xMax + num, rect.y, num3, rect.height);
            float num4 = 29f;
            Rect r = new Rect(rect3.x, rect3.y, num4, rect3.height);
            Rect rect5 = p.bgRects[p.kSoloMuteBypassIndex];
            GUIStyle channelStripAttenuationMarkerSquare = AudioMixerDrawUtils.styles.channelStripAttenuationMarkerSquare;
            EditorGUI.BeginDisabledGroup(!AudioMixerController.EditingTargetSnapshot());
            this.DoVUMeters(vuRect, channelStripAttenuationMarkerSquare.fixedHeight, p);
            this.DoAttenuationFader(r, p.group, this.m_Controller.CachedSelection, channelStripAttenuationMarkerSquare);
            this.DoTotaldB(p);
            this.DoEffectList(p, selected, ref highlightEffectIndex, ref patchslots, showBusConnectionsOfSelection);
            EditorGUI.EndDisabledGroup();
            this.DoSoloMuteBypassButtons(rect5, p.group, p.allGroups, this.m_Controller.CachedSelection, p.anySoloActive);
            if (flag && (current.button == 0))
            {
                this.GroupClicked(p.group, p, current.type == EventType.Used);
            }
            if ((current.type == EventType.ContextClick) && p.stripRect.Contains(current.mousePosition))
            {
                current.Use();
                if (selected)
                {
                    this.OpenGroupContextMenu(this.m_Controller.CachedSelection.ToArray());
                }
                else
                {
                    AudioMixerGroupController[] groups = new AudioMixerGroupController[] { p.group };
                    this.OpenGroupContextMenu(groups);
                }
            }
        }

        private void EffectSlot(Rect effectRect, AudioMixerSnapshotController snapshot, AudioMixerEffectController effect, int effectIndex, ref int highlightEffectIndex, ChannelStripParams p, ref Dictionary<AudioMixerEffectController, PatchSlot> patchslots)
        {
            if (effect != null)
            {
                Rect position = effectRect;
                Event current = Event.current;
                if (((current.type == EventType.Repaint) && (patchslots != null)) && (effect.IsSend() || MixerEffectDefinitions.EffectCanBeSidechainTarget(effect)))
                {
                    PatchSlot slot = new PatchSlot {
                        group = p.group,
                        x = position.xMax - ((position.yMax - position.yMin) * 0.5f),
                        y = (position.yMin + position.yMax) * 0.5f
                    };
                    patchslots[effect] = slot;
                }
                bool flag = !effect.DisallowsBypass();
                Rect rect2 = position;
                rect2.width = 10f;
                position.xMin += 10f;
                if (flag && GUI.Button(rect2, this.bypassButtonContent, GUIStyle.none))
                {
                    effect.bypass = !effect.bypass;
                    this.m_Controller.UpdateBypass();
                    InspectorWindow.RepaintAllInspectors();
                }
                this.m_IndexCounter++;
                float level = (effect == null) ? AudioMixerController.kMinVolume : Mathf.Clamp(effect.GetValueForMixLevel(this.m_Controller, snapshot), AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect);
                bool hasKeyboardFocus = (effect != null) && ((effect.IsSend() && (effect.sendTarget != null)) || effect.enableWetMix);
                if (current.type == EventType.Repaint)
                {
                    GUIStyle effectBarStyle = this.GetEffectBarStyle(effect);
                    float num3 = !hasKeyboardFocus ? 1f : ((level - AudioMixerController.kMinVolume) / (AudioMixerController.kMaxEffect - AudioMixerController.kMinVolume));
                    bool flag3 = (!p.group.bypassEffects && ((effect == null) || !effect.bypass)) || ((effect != null) && effect.DisallowsBypass());
                    Color color = (effect == null) ? new Color(0f, 0f, 0f, 0.5f) : AudioMixerDrawUtils.GetEffectColor(effect);
                    if (!flag3)
                    {
                        color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
                    }
                    if (flag3)
                    {
                        if (num3 < 1f)
                        {
                            float a = position.width * num3;
                            if (a < 4f)
                            {
                                a = Mathf.Max(a, 2f);
                                float x = 1f - (a / 4f);
                                Color color2 = GUI.color;
                                if (!GUI.enabled)
                                {
                                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                                }
                                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, a, position.height), effectBarStyle.focused.background, new Rect(x, 0f, 1f - x, 1f));
                                GUI.color = color2;
                            }
                            else
                            {
                                effectBarStyle.Draw(new Rect(position.x, position.y, a, position.height), false, false, false, true);
                            }
                            GUI.DrawTexture(new Rect(position.x + a, position.y, position.width - a, position.height), effectBarStyle.onFocused.background, ScaleMode.StretchToFill);
                        }
                        else
                        {
                            effectBarStyle.Draw(position, !hasKeyboardFocus, false, false, hasKeyboardFocus);
                        }
                    }
                    else
                    {
                        effectBarStyle.Draw(position, false, false, false, false);
                    }
                    if (flag)
                    {
                        this.styles.circularToggle.Draw(new Rect(rect2.x + 2f, rect2.y + 5f, rect2.width - 2f, rect2.width - 2f), false, false, !effect.bypass, false);
                    }
                    if (effect.IsSend() && (effect.sendTarget != null))
                    {
                        rect2.y--;
                        GUI.Label(rect2, this.styles.sendString, EditorStyles.miniLabel);
                    }
                    EditorGUI.BeginDisabledGroup(!flag3);
                    string t = this.GetEffectSlotName(effect, hasKeyboardFocus, snapshot, p);
                    string tooltip = this.GetEffectSlotTooltip(effect, position, p);
                    GUI.Label(new Rect(position.x, position.y, position.width - 10f, position.height), GUIContent.Temp(t, tooltip), this.styles.effectName);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    this.EffectSlotDragging(effectRect, snapshot, effect, hasKeyboardFocus, level, effectIndex, ref highlightEffectIndex, p);
                }
            }
        }

        private void EffectSlotDragging(Rect r, AudioMixerSnapshotController snapshot, AudioMixerEffectController effect, bool showLevel, float level, int effectIndex, ref int highlightEffectIndex, ChannelStripParams p)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(this.m_EffectInteractionControlID))
            {
                case EventType.MouseDown:
                    if ((r.Contains(current.mousePosition) && (current.button == 0)) && (GUIUtility.hotControl == 0))
                    {
                        GUIUtility.hotControl = this.m_EffectInteractionControlID;
                        this.m_MouseDragStartX = current.mousePosition.x;
                        this.m_MouseDragStartValue = level;
                        highlightEffectIndex = effectIndex;
                        this.m_MovingEffectSrcIndex = -1;
                        this.m_MovingEffectDstIndex = -1;
                        this.m_WaitingForDragEvent = true;
                        this.m_MovingSrcRect = r;
                        this.m_MovingDstRect = r;
                        this.m_MovingSrcGroup = p.group;
                        this.m_MovingDstGroup = p.group;
                        this.m_MovingEffectAllowed = true;
                        this.SetFocus();
                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        InspectorWindow.RepaintAllInspectors();
                    }
                    return;

                case EventType.MouseUp:
                    if (((GUIUtility.hotControl != this.m_EffectInteractionControlID) || (current.button != 0)) || !p.stripRect.Contains(current.mousePosition))
                    {
                        return;
                    }
                    if ((this.m_MovingEffectDstIndex != -1) && this.m_MovingEffectAllowed)
                    {
                        if (!this.IsDuplicateKeyPressed() || !this.CanDuplicateDraggedEffect())
                        {
                            if (this.m_MovingSrcGroup == this.m_MovingDstGroup)
                            {
                                List<AudioMixerEffectController> sourceEffects = this.m_MovingSrcGroup.effects.ToList<AudioMixerEffectController>();
                                if (AudioMixerController.MoveEffect(ref sourceEffects, this.m_MovingEffectSrcIndex, ref sourceEffects, this.m_MovingEffectDstIndex))
                                {
                                    this.m_MovingSrcGroup.effects = sourceEffects.ToArray();
                                }
                            }
                            else if (!this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation())
                            {
                                List<AudioMixerEffectController> list3 = this.m_MovingSrcGroup.effects.ToList<AudioMixerEffectController>();
                                List<AudioMixerEffectController> list4 = this.m_MovingDstGroup.effects.ToList<AudioMixerEffectController>();
                                if (AudioMixerController.MoveEffect(ref list3, this.m_MovingEffectSrcIndex, ref list4, this.m_MovingEffectDstIndex))
                                {
                                    this.m_MovingSrcGroup.effects = list3.ToArray();
                                    this.m_MovingDstGroup.effects = list4.ToArray();
                                }
                            }
                            break;
                        }
                        AudioMixerEffectController sourceEffect = this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex];
                        AudioMixerEffectController controller2 = this.m_MovingSrcGroup.controller.CopyEffect(sourceEffect);
                        List<AudioMixerEffectController> targetEffects = this.m_MovingDstGroup.effects.ToList<AudioMixerEffectController>();
                        if (AudioMixerController.InsertEffect(controller2, ref targetEffects, this.m_MovingEffectDstIndex))
                        {
                            this.m_MovingDstGroup.effects = targetEffects.ToArray();
                        }
                    }
                    break;

                case EventType.MouseMove:
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
                    {
                        if (this.HasFocus() && this.m_WaitingForDragEvent)
                        {
                            this.m_ChangingWetMixIndex = -1;
                            if (effectIndex < p.group.effects.Length)
                            {
                                if (Mathf.Abs(current.delta.y) <= Mathf.Abs(current.delta.x))
                                {
                                    this.m_ChangingWetMixIndex = this.m_IndexCounter;
                                }
                                else
                                {
                                    this.m_MovingEffectSrcIndex = effectIndex;
                                    this.ClearFocus();
                                }
                            }
                            this.m_WaitingForDragEvent = false;
                        }
                        if (this.IsMovingEffect() && p.stripRect.Contains(current.mousePosition))
                        {
                            float num = r.height * 0.5f;
                            float num2 = (effectIndex != 0) ? 0f : -num;
                            float num3 = (effectIndex != (p.group.effects.Length - 1)) ? r.height : (r.height + num);
                            float num4 = current.mousePosition.y - r.y;
                            if (((num4 >= num2) && (num4 <= num3)) && (effectIndex < p.group.effects.Length))
                            {
                                int targetIndex = (num4 >= num) ? (effectIndex + 1) : effectIndex;
                                if ((targetIndex != this.m_MovingEffectDstIndex) || (this.m_MovingDstGroup != p.group))
                                {
                                    this.m_MovingDstRect.x = r.x;
                                    this.m_MovingDstRect.width = r.width;
                                    this.m_MovingDstRect.y = ((num4 >= num) ? (r.y + r.height) : r.y) - 1f;
                                    this.m_MovingEffectDstIndex = targetIndex;
                                    this.m_MovingDstGroup = p.group;
                                    this.m_MovingEffectAllowed = ((!this.m_MovingSrcGroup.effects[this.m_MovingEffectSrcIndex].IsAttenuation() || (this.m_MovingSrcGroup == this.m_MovingDstGroup)) && !AudioMixerController.WillMovingEffectCauseFeedback(p.allGroups, this.m_MovingSrcGroup, this.m_MovingEffectSrcIndex, this.m_MovingDstGroup, targetIndex, null)) && (!this.IsDuplicateKeyPressed() || this.CanDuplicateDraggedEffect());
                                }
                                current.Use();
                            }
                        }
                        if ((this.IsAdjustingWetMix() && this.HasFocus()) && showLevel)
                        {
                            this.m_WaitingForDragEvent = false;
                            float num6 = (kEffectScaleMouseDrag * HandleUtility.niceMouseDelta) + level;
                            float num7 = Mathf.Clamp(num6, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect) - level;
                            if (num7 != 0f)
                            {
                                Undo.RecordObject(this.m_Controller.TargetSnapshot, "Change effect level");
                                if ((effect.IsSend() && (this.m_Controller.CachedSelection.Count > 1)) && this.m_Controller.CachedSelection.Contains(p.group))
                                {
                                    List<AudioMixerEffectController> list5 = new List<AudioMixerEffectController>();
                                    foreach (AudioMixerGroupController controller3 in this.m_Controller.CachedSelection)
                                    {
                                        foreach (AudioMixerEffectController controller4 in controller3.effects)
                                        {
                                            if ((controller4.effectName == effect.effectName) && (controller4.sendTarget == effect.sendTarget))
                                            {
                                                list5.Add(controller4);
                                            }
                                        }
                                    }
                                    foreach (AudioMixerEffectController controller5 in list5)
                                    {
                                        if (!controller5.IsSend() || (controller5.sendTarget != null))
                                        {
                                            controller5.SetValueForMixLevel(this.m_Controller, snapshot, Mathf.Clamp(controller5.GetValueForMixLevel(this.m_Controller, snapshot) + num7, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect));
                                        }
                                    }
                                }
                                else if (!effect.IsSend() || (effect.sendTarget != null))
                                {
                                    effect.SetValueForMixLevel(this.m_Controller, snapshot, Mathf.Clamp(level + num7, AudioMixerController.kMinVolume, AudioMixerController.kMaxEffect));
                                }
                                InspectorWindow.RepaintAllInspectors();
                            }
                            current.Use();
                        }
                    }
                    return;

                default:
                    return;
            }
            this.ClearEffectDragging(ref highlightEffectIndex);
            current.Use();
            EditorGUIUtility.SetWantsMouseJumping(0);
            GUIUtility.ExitGUI();
        }

        public static void GetCableVertex(float x1, float y1, float x2, float y2, float x3, float y3, float t, out float x, out float y)
        {
            x = Lerp(Lerp(x1, x2, t), Lerp(x2, x3, t), t);
            y = Lerp(Lerp(y1, y2, t), Lerp(y2, y3, t), t);
        }

        private Rect GetContentRect(List<AudioMixerGroupController> sortedGroups, bool isShowingReferencedGroups, int maxEffects)
        {
            float num = 239f;
            float height = ((((this.channelStripsOffset.y + num) + (maxEffects * 16f)) + 10f) + 16f) + 10f;
            return new Rect(0f, 0f, ((this.channelStripsOffset.x * 2f) + (94f * sortedGroups.Count)) + (!isShowingReferencedGroups ? 0f : 50f), height);
        }

        public GUIStyle GetEffectBarStyle(AudioMixerEffectController effect)
        {
            if ((effect.IsSend() || effect.IsReceive()) || effect.IsDuckVolume())
            {
                return this.styles.sendReturnBar;
            }
            if (effect.IsAttenuation())
            {
                return this.styles.attenuationBar;
            }
            return this.styles.effectBar;
        }

        private string GetEffectSlotName(AudioMixerEffectController effect, bool showLevel, AudioMixerSnapshotController snapshot, ChannelStripParams p)
        {
            if ((this.m_ChangingWetMixIndex == this.m_IndexCounter) && showLevel)
            {
                return string.Format("{0:F1} dB", effect.GetValueForMixLevel(this.m_Controller, snapshot));
            }
            if (effect.IsSend() && (effect.sendTarget != null))
            {
                return effect.GetSendTargetDisplayString(p.effectMap);
            }
            return effect.effectName;
        }

        private string GetEffectSlotTooltip(AudioMixerEffectController effect, Rect effectRect, ChannelStripParams p)
        {
            if (!effectRect.Contains(Event.current.mousePosition))
            {
                return string.Empty;
            }
            if (effect.IsSend())
            {
                if (effect.sendTarget != null)
                {
                    string sendTargetDisplayString = effect.GetSendTargetDisplayString(p.effectMap);
                    return ("Send to: " + sendTargetDisplayString);
                }
                return this.styles.emptySendSlotGUIContent.tooltip;
            }
            if (effect.IsReceive())
            {
                return this.styles.returnSlotGUIContent.tooltip;
            }
            if (effect.IsDuckVolume())
            {
                return this.styles.duckVolumeSlotGUIContent.tooltip;
            }
            if (effect.IsAttenuation())
            {
                return this.styles.attenuationSlotGUIContent.tooltip;
            }
            return this.styles.effectSlotGUIContent.tooltip;
        }

        private void GroupClicked(AudioMixerGroupController clickedGroup, ChannelStripParams p, bool clickedControlInGroup)
        {
            <GroupClicked>c__AnonStorey5D storeyd = new <GroupClicked>c__AnonStorey5D();
            List<int> allInstanceIDs = new List<int>();
            foreach (AudioMixerGroupController controller in p.shownGroups)
            {
                allInstanceIDs.Add(controller.GetInstanceID());
            }
            List<int> selectedInstanceIDs = new List<int>();
            foreach (AudioMixerGroupController controller2 in this.m_Controller.CachedSelection)
            {
                selectedInstanceIDs.Add(controller2.GetInstanceID());
            }
            int lastClickedInstanceID = this.m_State.m_LastClickedInstanceID;
            bool allowMultiSelection = true;
            bool keepMultiSelection = Event.current.shift || clickedControlInGroup;
            bool useShiftAsActionKey = false;
            storeyd.newSelection = InternalEditorUtility.GetNewSelection(clickedGroup.GetInstanceID(), allInstanceIDs, selectedInstanceIDs, lastClickedInstanceID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
            Selection.objects = p.allGroups.Where<AudioMixerGroupController>(new Func<AudioMixerGroupController, bool>(storeyd.<>m__A2)).ToList<AudioMixerGroupController>().ToArray();
            this.m_Controller.OnUnitySelectionChanged();
            InspectorWindow.RepaintAllInspectors();
        }

        private bool HasFocus()
        {
            return (this.m_FocusIndex == this.m_IndexCounter);
        }

        public float HorizontalFader(Rect r, float value, float minValue, float maxValue, int direction, float dragScale)
        {
            this.m_IndexCounter++;
            Rect rect = new Rect(r);
            float num = r.width * 0.2f;
            float num2 = rect.width - num;
            AudioMixerDrawUtils.DrawGradientRect(rect, hfaderCol1, hfaderCol2);
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.m_MouseDragStartX = current.mousePosition.x;
                this.m_MouseDragStartValue = value;
                this.SetFocus();
            }
            if (this.HasFocus())
            {
                if (current.type == EventType.MouseDrag)
                {
                    value = this.m_MouseDragStartValue + (((dragScale * (maxValue - minValue)) * (current.mousePosition.x - this.m_MouseDragStartX)) / num2);
                    Event.current.Use();
                }
                else if (current.type == EventType.MouseUp)
                {
                    this.ClearFocus();
                    Event.current.Use();
                }
            }
            value = Mathf.Clamp(value, minValue, maxValue);
            rect.x = r.x;
            rect.width = r.width;
            rect.x = r.x + (num2 * ((value - minValue) / (maxValue - minValue)));
            rect.width = num;
            AudioMixerDrawUtils.DrawGradientRect(rect, hfaderCol2, hfaderCol1);
            return value;
        }

        public static void InsertEffectPopupCallback(object obj)
        {
            EffectContext context = (EffectContext) obj;
            foreach (AudioMixerGroupController controller in context.groups)
            {
                Undo.RecordObject(controller, "Add effect");
                AudioMixerEffectController effect = new AudioMixerEffectController(context.name);
                int index = ((context.index != -1) && (context.index <= controller.effects.Length)) ? context.index : controller.effects.Length;
                controller.InsertEffect(effect, index);
                AssetDatabase.AddObjectToAsset(effect, context.controller);
                effect.PreallocateGUIDs();
            }
            AudioMixerUtility.RepaintAudioMixerAndInspectors();
        }

        private bool IsAdjustingWetMix()
        {
            return (this.m_ChangingWetMixIndex != -1);
        }

        private bool IsDuplicateKeyPressed()
        {
            return Event.current.alt;
        }

        private bool IsFocusActive()
        {
            return (this.m_FocusIndex != -1);
        }

        private bool IsMovingEffect()
        {
            return (this.m_MovingEffectSrcIndex != -1);
        }

        private bool IsRectSelectionActive()
        {
            return (GUIUtility.hotControl == this.m_RectSelectionControlID);
        }

        public static float Lerp(float x1, float x2, float t)
        {
            return (x1 + ((x2 - x1) * t));
        }

        public void OnGUI(Rect rect, bool showReferencedBuses, bool showBusConnections, bool showBusConnectionsOfSelection, List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap, bool sortGroupsAlphabetically, bool showDeveloperOverlays, AudioMixerGroupController scrollToItem)
        {
            if (this.m_Controller == null)
            {
                this.DrawAreaBackground(rect);
            }
            else if (Event.current.type != EventType.Layout)
            {
                this.m_RectSelectionControlID = GUIUtility.GetControlID(kRectSelectionHashCode, FocusType.Passive);
                this.m_EffectInteractionControlID = GUIUtility.GetControlID(kEffectDraggingHashCode, FocusType.Passive);
                this.m_IndexCounter = 0;
                Event current = Event.current;
                List<AudioMixerGroupController> sortedGroups = this.m_Controller.GetCurrentViewGroupList().ToList<AudioMixerGroupController>();
                if (sortGroupsAlphabetically)
                {
                    sortedGroups.Sort(this.m_GroupComparer);
                }
                Rect rect2 = new Rect(this.channelStripsOffset.x, this.channelStripsOffset.y, 90f, 300f);
                if (scrollToItem != null)
                {
                    int index = sortedGroups.IndexOf(scrollToItem);
                    if (index >= 0)
                    {
                        float num2 = ((rect2.width + 4f) * index) - this.m_State.m_ScrollPos.x;
                        if ((num2 < -20f) || (num2 > rect.width))
                        {
                            this.m_State.m_ScrollPos.x += num2;
                        }
                    }
                }
                List<AudioMixerGroupController> source = new List<AudioMixerGroupController>();
                foreach (AudioMixerGroupController controller in sortedGroups)
                {
                    foreach (AudioMixerEffectController controller2 in controller.effects)
                    {
                        if (controller2.sendTarget != null)
                        {
                            AudioMixerGroupController item = effectMap[controller2.sendTarget];
                            if (!source.Contains(item) && !sortedGroups.Contains(item))
                            {
                                source.Add(item);
                            }
                        }
                    }
                }
                List<AudioMixerGroupController> collection = source.ToList<AudioMixerGroupController>();
                collection.Sort(this.m_GroupComparer);
                int count = sortedGroups.Count;
                if (showReferencedBuses && (collection.Count > 0))
                {
                    sortedGroups.AddRange(collection);
                }
                int a = 1;
                foreach (AudioMixerGroupController controller4 in sortedGroups)
                {
                    a = Mathf.Max(a, controller4.effects.Length);
                }
                bool isShowingReferencedGroups = sortedGroups.Count != count;
                Rect viewRect = this.GetContentRect(sortedGroups, isShowingReferencedGroups, a);
                this.m_State.m_ScrollPos = GUI.BeginScrollView(rect, this.m_State.m_ScrollPos, viewRect);
                this.DrawAreaBackground(new Rect(0f, 0f, 10000f, 10000f));
                ChannelStripParams params2 = new ChannelStripParams {
                    effectMap = effectMap,
                    allGroups = allGroups,
                    shownGroups = sortedGroups
                };
                if (<>f__am$cache34 == null)
                {
                    <>f__am$cache34 = g => g.solo;
                }
                params2.anySoloActive = allGroups.Any<AudioMixerGroupController>(<>f__am$cache34);
                params2.visibleRect = new Rect(this.m_State.m_ScrollPos.x, this.m_State.m_ScrollPos.y, rect.width, rect.height);
                ChannelStripParams p = params2;
                Dictionary<AudioMixerEffectController, PatchSlot> patchslots = !showBusConnections ? null : new Dictionary<AudioMixerEffectController, PatchSlot>();
                for (int i = 0; i < sortedGroups.Count; i++)
                {
                    AudioMixerGroupController controller5 = sortedGroups[i];
                    p.index = i;
                    p.group = controller5;
                    p.drawingBuses = false;
                    p.visible = RectOverlaps(p.visibleRect, rect2);
                    p.Init(this.m_Controller, rect2, a);
                    this.DrawChannelStrip(p, ref this.m_Controller.m_HighlightEffectIndex, ref patchslots, showBusConnectionsOfSelection);
                    if (((current.type == EventType.MouseDown) && (current.button == 0)) && p.stripRect.Contains(current.mousePosition))
                    {
                        current.Use();
                    }
                    if ((this.IsMovingEffect() && (current.type == EventType.MouseDrag)) && (p.stripRect.Contains(current.mousePosition) && (GUIUtility.hotControl == this.m_EffectInteractionControlID)))
                    {
                        this.m_MovingEffectDstIndex = -1;
                        current.Use();
                    }
                    rect2.x += p.stripRect.width + 4f;
                    if ((showReferencedBuses && (i == (count - 1))) && (sortedGroups.Count > count))
                    {
                        rect2.x += 50f;
                        EditorGUI.BeginDisabledGroup(true);
                        GUI.Label(new Rect(rect2.x, p.stripRect.yMax, 130f, 22f), this.styles.referencedGroups, this.styles.channelStripHeaderStyle);
                        EditorGUI.EndDisabledGroup();
                    }
                }
                this.UnhandledEffectDraggingEvents(ref this.m_Controller.m_HighlightEffectIndex);
                if ((current.type == EventType.Repaint) && (patchslots != null))
                {
                    foreach (KeyValuePair<AudioMixerEffectController, PatchSlot> pair in patchslots)
                    {
                        PatchSlot slot = pair.Value;
                        bool on = !showBusConnectionsOfSelection || this.m_Controller.CachedSelection.Contains(slot.group);
                        if (on)
                        {
                            this.styles.circularToggle.Draw(new Rect(slot.x - 3f, slot.y - 3f, 6f, 6f), false, false, on, false);
                        }
                    }
                    float num7 = Mathf.Exp((-0.03f * Time.time) * Time.time) * 0.1f;
                    Color color = new Color(0f, 0f, 0f, !AudioMixerController.EditingTargetSnapshot() ? 0.05f : 0.1f);
                    Color color2 = new Color(0f, 0f, 0f, !AudioMixerController.EditingTargetSnapshot() ? 0.5f : 1f);
                    foreach (KeyValuePair<AudioMixerEffectController, PatchSlot> pair2 in patchslots)
                    {
                        AudioMixerEffectController key = pair2.Key;
                        AudioMixerEffectController sendTarget = key.sendTarget;
                        if (sendTarget != null)
                        {
                            PatchSlot slot2 = pair2.Value;
                            if (patchslots.ContainsKey(sendTarget))
                            {
                                PatchSlot slot3 = patchslots[sendTarget];
                                int num8 = key.GetInstanceID() ^ sendTarget.GetInstanceID();
                                float num9 = (num8 & 0x3f) * 0.1f;
                                float num10 = ((Mathf.Abs((float) (slot3.x - slot2.x)) * Mathf.Sin((Time.time * 5f) + num9)) * num7) + ((slot2.x + slot3.x) * 0.5f);
                                float num11 = (((Mathf.Abs((float) (slot3.y - slot2.y)) * Mathf.Cos((Time.time * 5f) + num9)) * num7) + Math.Max(slot2.y, slot3.y)) + Mathf.Max(Mathf.Min((float) (0.5f * Math.Abs((float) (slot3.y - slot2.y))), (float) 50f), 50f);
                                for (int j = 0; j < this.cablepoints.Length; j++)
                                {
                                    GetCableVertex(slot2.x, slot2.y, num10, num11, slot3.x, slot3.y, ((float) j) / ((float) (this.cablepoints.Length - 1)), out this.cablepoints[j].x, out this.cablepoints[j].y);
                                }
                                bool flag3 = (showBusConnectionsOfSelection && !this.m_Controller.CachedSelection.Contains(slot2.group)) && !this.m_Controller.CachedSelection.Contains(slot3.group);
                                Handles.color = !flag3 ? color2 : color;
                                Handles.DrawAAPolyLine(7f, this.cablepoints.Length, this.cablepoints);
                                if (!flag3)
                                {
                                    num8 ^= ((num8 >> 6) ^ (num8 >> 12)) ^ (num8 >> 0x12);
                                    Handles.color = new Color(((num8 & 3) * 0.15f) + 0.55f, (((num8 >> 2) & 3) * 0.15f) + 0.55f, (((num8 >> 4) & 3) * 0.15f) + 0.55f, !AudioMixerController.EditingTargetSnapshot() ? 0.5f : 1f);
                                    Handles.DrawAAPolyLine(4f, this.cablepoints.Length, this.cablepoints);
                                    Handles.color = new Color(1f, 1f, 1f, !AudioMixerController.EditingTargetSnapshot() ? 0.25f : 0.5f);
                                    Handles.DrawAAPolyLine(3f, this.cablepoints.Length, this.cablepoints);
                                }
                            }
                        }
                    }
                }
                this.RectSelection(p);
                GUI.EndScrollView(true);
                AudioMixerDrawUtils.DrawScrollDropShadow(rect, this.m_State.m_ScrollPos.y, viewRect.height);
                this.WarningOverlay(allGroups, rect, viewRect);
                this.ShowDeveloperOverlays(rect, current, showDeveloperOverlays);
                if (!EditorApplication.isPlaying && !this.m_Controller.isSuspended)
                {
                    this.m_RequiresRepaint = true;
                }
            }
        }

        public void OnMixerControllerChanged(AudioMixerController controller)
        {
            this.m_Controller = controller;
        }

        private void OpenGroupContextMenu(AudioMixerGroupController[] groups)
        {
            GenericMenu pm = new GenericMenu();
            AddEffectItemsToMenu(groups[0].controller, groups, 0, "Add effect at top/", pm);
            AddEffectItemsToMenu(groups[0].controller, groups, -1, "Add effect at bottom/", pm);
            pm.AddSeparator(string.Empty);
            AudioMixerColorCodes.AddColorItemsToGenericMenu(pm, groups);
            pm.AddSeparator(string.Empty);
            pm.ShowAsContext();
        }

        private static bool RectOverlaps(Rect r1, Rect r2)
        {
            Rect rect;
            rect = new Rect {
                x = Mathf.Max(r1.x, r2.x),
                y = Mathf.Max(r1.y, r2.y),
                width = Mathf.Min((float) (r1.x + r1.width), (float) (r2.x + r2.width)) - rect.x,
                height = Mathf.Min((float) (r1.y + r1.height), (float) (r2.y + r2.height)) - rect.y
            };
            return ((rect.width > 0f) && (rect.height > 0f));
        }

        private void RectSelection(ChannelStripParams channelStripParams)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseDown) && (current.button == 0)) && (GUIUtility.hotControl == 0))
            {
                if (!current.shift)
                {
                    Selection.objects = new Object[0];
                    this.m_Controller.OnUnitySelectionChanged();
                }
                GUIUtility.hotControl = this.m_RectSelectionControlID;
                this.m_RectSelectionStartPos = current.mousePosition;
                this.m_RectSelectionRect = new Rect(this.m_RectSelectionStartPos.x, this.m_RectSelectionStartPos.y, 0f, 0f);
                Event.current.Use();
                InspectorWindow.RepaintAllInspectors();
            }
            if (current.type == EventType.MouseDrag)
            {
                if (this.IsRectSelectionActive())
                {
                    this.m_RectSelectionRect.x = Mathf.Min(this.m_RectSelectionStartPos.x, current.mousePosition.x);
                    this.m_RectSelectionRect.y = Mathf.Min(this.m_RectSelectionStartPos.y, current.mousePosition.y);
                    this.m_RectSelectionRect.width = Mathf.Max(this.m_RectSelectionStartPos.x, current.mousePosition.x) - this.m_RectSelectionRect.x;
                    this.m_RectSelectionRect.height = Mathf.Max(this.m_RectSelectionStartPos.y, current.mousePosition.y) - this.m_RectSelectionRect.y;
                    Event.current.Use();
                }
                if (this.m_MovingSrcRect.y >= 0f)
                {
                    Event.current.Use();
                }
            }
            if (this.IsRectSelectionActive() && (current.GetTypeForControl(this.m_RectSelectionControlID) == EventType.MouseUp))
            {
                List<AudioMixerGroupController> list = !current.shift ? new List<AudioMixerGroupController>() : this.m_Controller.CachedSelection;
                foreach (AudioMixerGroupController controller in channelStripParams.rectSelectionGroups)
                {
                    if (!list.Contains(controller))
                    {
                        list.Add(controller);
                    }
                }
                Selection.objects = list.ToArray();
                this.m_Controller.OnUnitySelectionChanged();
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }
            if ((current.type == EventType.Repaint) && this.IsRectSelectionActive())
            {
                Color color = new Color(1f, 1f, 1f, 0.3f);
                AudioMixerDrawUtils.DrawGradientRectHorizontal(this.m_RectSelectionRect, color, color);
            }
        }

        public void RemoveEffectPopupCallback(object obj)
        {
            EffectContext context = (EffectContext) obj;
            foreach (AudioMixerGroupController controller in context.groups)
            {
                if (context.index < controller.effects.Length)
                {
                    AudioMixerEffectController sendTarget = controller.effects[context.index];
                    context.controller.ClearSendConnectionsTo(sendTarget);
                    context.controller.RemoveEffect(sendTarget, controller);
                }
            }
            AudioMixerUtility.RepaintAudioMixerAndInspectors();
        }

        private void SetFocus()
        {
            this.m_FocusIndex = this.m_IndexCounter;
        }

        public void ShowDeveloperOverlays(Rect rect, Event evt, bool show)
        {
            if ((show && Unsupported.IsDeveloperBuild()) && (evt.type == EventType.Repaint))
            {
                AudioMixerDrawUtils.ReadOnlyLabel(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 20f), "Current snapshot: " + this.m_Controller.TargetSnapshot.name, this.developerInfoStyle);
                AudioMixerDrawUtils.ReadOnlyLabel(new Rect(rect.x + 5f, rect.y + 25f, rect.width - 10f, 20f), "Frame count: " + this.FrameCounter++, this.developerInfoStyle);
            }
        }

        private void UnhandledEffectDraggingEvents(ref int highlightEffectIndex)
        {
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(this.m_EffectInteractionControlID);
            switch (typeForControl)
            {
                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == this.m_EffectInteractionControlID) && (current.button == 0))
                    {
                        this.ClearEffectDragging(ref highlightEffectIndex);
                        current.Use();
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == this.m_EffectInteractionControlID)
                    {
                        this.m_MovingEffectDstIndex = -1;
                        this.m_MovingDstRect = new Rect(-1f, -1f, 0f, 0f);
                        this.m_MovingDstGroup = null;
                        current.Use();
                    }
                    return;
            }
            if (typeForControl == EventType.Repaint)
            {
                if (this.IsMovingEffect() && (current.type == EventType.Repaint))
                {
                    EditorGUI.DrawRect(this.m_MovingSrcRect, kMoveColorHighlight);
                    MouseCursor mouse = (!this.IsDuplicateKeyPressed() || !this.m_MovingEffectAllowed) ? MouseCursor.ResizeVertical : MouseCursor.ArrowPlus;
                    EditorGUIUtility.AddCursorRect(new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f), mouse, this.m_EffectInteractionControlID);
                }
                if ((this.m_MovingEffectDstIndex != -1) && (this.m_MovingDstRect.y >= 0f))
                {
                    float height = 2f;
                    Color color = !this.m_MovingEffectAllowed ? kMoveSlotColLoDisallowed : kMoveSlotColLoAllowed;
                    Color color2 = !this.m_MovingEffectAllowed ? kMoveSlotColHiDisallowed : kMoveSlotColHiAllowed;
                    Color color3 = !this.m_MovingEffectAllowed ? kMoveSlotColBorderDisallowed : kMoveSlotColBorderAllowed;
                    AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y - height, this.m_MovingDstRect.width, height), color, color2);
                    AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y, this.m_MovingDstRect.width, height), color2, color);
                    AudioMixerDrawUtils.DrawGradientRect(new Rect(this.m_MovingDstRect.x, this.m_MovingDstRect.y - 1f, this.m_MovingDstRect.width, 1f), color3, color3);
                }
            }
        }

        public float VerticalFader(Rect r, float value, int direction, float dragScale, bool drawScaleValues, bool drawMarkerValue, string tooltip, float maxValue, GUIStyle style)
        {
            Event current = Event.current;
            int fixedHeight = (int) style.fixedHeight;
            int num2 = ((int) r.height) - fixedHeight;
            float num3 = AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(value, AudioMixerController.kMinVolume, maxValue), (float) num2, true);
            Rect position = new Rect(r.x, r.y + ((int) num3), r.width, (float) fixedHeight);
            int controlID = GUIUtility.GetControlID(kVerticalFaderHash, FocusType.Passive);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (r.Contains(current.mousePosition) && (GUIUtility.hotControl == 0))
                    {
                        this.m_MouseDragStartY = current.mousePosition.y;
                        this.m_MouseDragStartValue = num3;
                        GUIUtility.hotControl = controlID;
                        current.Use();
                    }
                    return value;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    return value;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return value;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        value = Mathf.Clamp(AudioMixerController.VolumeToScreenMapping(Mathf.Clamp(this.m_MouseDragStartValue + (dragScale * (current.mousePosition.y - this.m_MouseDragStartY)), 0f, (float) num2), (float) num2, false), AudioMixerController.kMinVolume, maxValue);
                        current.Use();
                    }
                    return value;

                case EventType.Repaint:
                    if (drawScaleValues)
                    {
                        float num5 = r.y + (((float) fixedHeight) / 2f);
                        float num6 = maxValue;
                        EditorGUI.BeginDisabledGroup(true);
                        while (num6 >= AudioMixerController.kMinVolume)
                        {
                            float num7 = AudioMixerController.VolumeToScreenMapping(num6, (float) num2, true);
                            if (((num6 / 10f) % 2f) == 0f)
                            {
                                GUI.Label(new Rect(r.x, (num5 + num7) - 7f, r.width, 14f), GUIContent.Temp(Mathf.RoundToInt(num6).ToString()), this.styles.vuValue);
                            }
                            EditorGUI.DrawRect(new Rect(r.x, (num5 + num7) - 1f, 5f, 1f), new Color(0f, 0f, 0f, 0.5f));
                            num6 -= 10f;
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    break;

                default:
                    return value;
            }
            if (drawMarkerValue)
            {
                style.Draw(position, GUIContent.Temp(Mathf.RoundToInt(value).ToString()), false, false, false, false);
            }
            else
            {
                style.Draw(position, false, false, false, false);
            }
            AudioMixerDrawUtils.AddTooltipOverlay(position, tooltip);
            return value;
        }

        public void VUMeter(AudioMixerGroupController group, Rect r, float level, float peak)
        {
            UnityEditor.EditorGUI.VUMeter.VerticalMeter(r, level, peak, UnityEditor.EditorGUI.VUMeter.verticalVUTexture, Color.grey);
        }

        private void WarningOverlay(List<AudioMixerGroupController> allGroups, Rect rect, Rect contentRect)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (AudioMixerGroupController controller in allGroups)
            {
                if (controller.solo)
                {
                    num++;
                }
                if (controller.mute)
                {
                    num2++;
                }
                if (controller.bypassEffects)
                {
                    num3 += controller.effects.Length - 1;
                }
                else
                {
                    if (<>f__am$cache35 == null)
                    {
                        <>f__am$cache35 = e => e.bypass;
                    }
                    num3 += controller.effects.Count<AudioMixerEffectController>(<>f__am$cache35);
                }
            }
            if (((Event.current.type == EventType.Repaint) && (num > 0)) || ((num2 > 0) || (num3 > 0)))
            {
                string t = "Warning: " + ((num <= 0) ? ((num2 <= 0) ? (num3 + ((num3 <= 1) ? " effect" : " effects") + " currently bypassed") : (num2 + ((num2 <= 1) ? " group" : " groups") + " currently muted")) : (num + ((num <= 1) ? " group" : " groups") + " currently soloed"));
                bool flag = contentRect.width > rect.width;
                float x = this.styles.warningOverlay.CalcSize(GUIContent.Temp(t)).x;
                Rect position = new Rect((rect.x + 5f) + Mathf.Max((float) (((rect.width - 10f) - x) * 0.5f), (float) 0f), ((rect.yMax - this.styles.warningOverlay.fixedHeight) - 5f) - (!flag ? 0f : 17f), x, this.styles.warningOverlay.fixedHeight);
                GUI.Label(position, GUIContent.Temp(t), this.styles.warningOverlay);
            }
        }

        private static Color gridColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return kGridColorDark;
                }
                return kGridColorLight;
            }
        }

        private Texture2D gridTextureTilable
        {
            get
            {
                if (m_GridTexture == null)
                {
                    m_GridTexture = CreateTilableGridTexture(12, 12, new Color(0f, 0f, 0f, 0f), gridColor);
                }
                return m_GridTexture;
            }
        }

        public bool requiresRepaint
        {
            get
            {
                if (this.m_RequiresRepaint)
                {
                    this.m_RequiresRepaint = false;
                    return true;
                }
                return false;
            }
        }

        private AudioMixerDrawUtils.Styles styles
        {
            get
            {
                return AudioMixerDrawUtils.styles;
            }
        }

        [CompilerGenerated]
        private sealed class <DoEffectSlotInsertEffectPopup>c__AnonStorey5E
        {
            internal AudioMixerChannelStripView <>f__this;
            internal AudioMixerEffectController effect;

            internal void <>m__A3()
            {
                this.effect.enableWetMix = !this.effect.enableWetMix;
            }

            internal void <>m__A4()
            {
                this.effect.bypass = !this.effect.bypass;
                this.<>f__this.m_Controller.UpdateBypass();
                InspectorWindow.RepaintAllInspectors();
            }
        }

        [CompilerGenerated]
        private sealed class <GroupClicked>c__AnonStorey5D
        {
            internal List<int> newSelection;

            internal bool <>m__A2(AudioMixerGroupController x)
            {
                return this.newSelection.Contains(x.GetInstanceID());
            }
        }

        private class BusConnection
        {
            public Color color;
            public bool isSelected;
            public bool isSend;
            public float mixLevel;
            public float srcX;
            public float srcY;
            public AudioMixerEffectController targetEffect;

            public BusConnection(float srcX, float srcY, AudioMixerEffectController targetEffect, float mixLevel, Color col, bool isSend, bool isSelected)
            {
                this.srcX = srcX;
                this.srcY = srcY;
                this.targetEffect = targetEffect;
                this.mixLevel = mixLevel;
                this.color = col;
                this.isSend = isSend;
                this.isSelected = isSelected;
            }
        }

        private class ChannelStripParams
        {
            public List<AudioMixerGroupController> allGroups;
            public bool anySoloActive;
            public List<Rect> bgRects;
            public List<AudioMixerChannelStripView.BusConnection> busConnections = new List<AudioMixerChannelStripView.BusConnection>();
            public bool drawingBuses;
            public Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap;
            public AudioMixerGroupController group;
            public int index;
            private const float kAddEffectButtonHeight = 16f;
            public readonly int kEffectStartIndex = 4;
            public readonly int kHeaderIndex;
            public readonly int kSoloMuteBypassIndex = 3;
            public readonly int kTotalVULevelIndex = 2;
            public readonly int kVUMeterFaderIndex = 1;
            public int maxEffects;
            public int numChannels;
            public List<AudioMixerGroupController> rectSelectionGroups = new List<AudioMixerGroupController>();
            public List<AudioMixerGroupController> shownGroups;
            public Rect stripRect;
            public bool visible;
            public Rect visibleRect;
            public float[] vuinfo_level = new float[9];
            public float[] vuinfo_peak = new float[9];

            private List<Rect> GetBackgroundRects(Rect r, AudioMixerGroupController group, int maxNumGroups)
            {
                List<float> list = new List<float>();
                list.AddRange(Enumerable.Repeat<float>(0f, this.kEffectStartIndex));
                list[this.kHeaderIndex] = 22f;
                list[this.kVUMeterFaderIndex] = 170f;
                list[this.kTotalVULevelIndex] = 17f;
                list[this.kSoloMuteBypassIndex] = 30f;
                int num = maxNumGroups;
                for (int i = 0; i < num; i++)
                {
                    list.Add(16f);
                }
                list.Add(10f);
                List<Rect> list2 = new List<Rect>();
                float y = r.y;
                foreach (int num4 in list)
                {
                    if (list2.Count > 0)
                    {
                        y = y;
                    }
                    list2.Add(new Rect(r.x, y, r.width, (float) num4));
                    y += num4;
                }
                y += 10f;
                list2.Add(new Rect(r.x, y, r.width, 16f));
                return list2;
            }

            public void Init(AudioMixerController controller, Rect channelStripRect, int maxNumEffects)
            {
                this.numChannels = controller.GetGroupVUInfo(this.group.groupID, false, ref this.vuinfo_level, ref this.vuinfo_peak);
                this.maxEffects = maxNumEffects;
                this.bgRects = this.GetBackgroundRects(channelStripRect, this.group, this.maxEffects);
                this.stripRect = channelStripRect;
                Rect rect = this.bgRects[this.bgRects.Count - 1];
                this.stripRect.yMax = rect.yMax;
            }
        }

        public class ConnectSendContext
        {
            public AudioMixerEffectController sendEffect;
            public AudioMixerEffectController targetEffect;

            public ConnectSendContext(AudioMixerEffectController sendEffect, AudioMixerEffectController targetEffect)
            {
                this.sendEffect = sendEffect;
                this.targetEffect = targetEffect;
            }
        }

        public class EffectContext
        {
            public AudioMixerController controller;
            public AudioMixerGroupController[] groups;
            public int index;
            public string name;

            public EffectContext(AudioMixerController controller, AudioMixerGroupController[] groups, int index, string name)
            {
                this.controller = controller;
                this.groups = groups;
                this.index = index;
                this.name = name;
            }
        }

        private class PatchSlot
        {
            public AudioMixerGroupController group;
            public float x;
            public float y;
        }

        [Serializable]
        public class State
        {
            public int m_LastClickedInstanceID;
            public Vector2 m_ScrollPos = new Vector2(0f, 0f);
        }
    }
}

