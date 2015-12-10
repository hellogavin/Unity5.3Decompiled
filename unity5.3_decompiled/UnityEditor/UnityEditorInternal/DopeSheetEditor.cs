namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class DopeSheetEditor : TimeArea, CurveUpdater
    {
        [CompilerGenerated]
        private static Comparison<Object> <>f__am$cache11;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapD;
        private List<DrawElement> dragdropKeysDrawBuffer;
        private const float k_KeyframeOffset = -5.5f;
        private const float k_PptrKeyframeOffset = -1f;
        public Bounds m_Bounds;
        private Texture m_DefaultDopeKeyIcon;
        private float m_DragStartTime;
        private bool m_Initialized;
        private bool m_IsDragging;
        private bool m_IsDraggingPlayhead;
        private bool m_IsDraggingPlayheadStarted;
        private bool m_MousedownOnKeyframe;
        [SerializeField]
        public EditorWindow m_Owner;
        private DopeSheetSelectionRect m_SelectionRect;
        public int m_SpritePreviewCacheSize;
        public bool m_SpritePreviewLoading;
        private List<DrawElement> selectedKeysDrawBuffer;
        public AnimationWindowState state;
        private List<DrawElement> unselectedKeysDrawBuffer;

        public DopeSheetEditor(EditorWindow owner) : base(false)
        {
            this.m_Bounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_Owner = owner;
        }

        private void AddKeyToDopeline(object obj)
        {
            this.AddKeyToDopeline((DopeLine) obj);
        }

        private void AddKeyToDopeline(DopeLine dopeLine)
        {
            this.state.ClearSelections();
            foreach (AnimationWindowCurve curve in dopeLine.m_Curves)
            {
                AnimationWindowKeyframe keyframe = AnimationWindowUtility.AddKeyframeToCurve(this.state, curve, this.state.time);
                this.state.SelectKey(keyframe);
            }
        }

        private bool AnyKeyIsSelectedAtTime(DopeLine dopeLine, int keyIndex)
        {
            int timeHash = dopeLine.keys[keyIndex].m_TimeHash;
            int count = dopeLine.keys.Count;
            for (int i = keyIndex; i < count; i++)
            {
                AnimationWindowKeyframe keyframe = dopeLine.keys[i];
                if (keyframe.m_TimeHash != timeHash)
                {
                    return false;
                }
                if (this.state.KeyIsSelected(keyframe))
                {
                    return true;
                }
            }
            return false;
        }

        private EditorCurveBinding? CreateNewPptrDopeline(Type valueType)
        {
            List<EditorCurveBinding> userData = AnimationWindowUtility.GetAnimatableProperties(this.state.activeRootGameObject, this.state.activeRootGameObject, valueType);
            if ((userData.Count == 0) && (valueType == typeof(Sprite)))
            {
                return this.CreateNewSpriteRendererDopeline();
            }
            if (userData.Count == 1)
            {
                return new EditorCurveBinding?(userData[0]);
            }
            List<string> list2 = new List<string>();
            foreach (EditorCurveBinding binding in userData)
            {
                list2.Add(binding.type.Name);
            }
            Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
            EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(list2.ToArray()), -1, new EditorUtility.SelectMenuItemFunction(this.SelectTypeForCreatingNewPptrDopeline), userData);
            return null;
        }

        private void CreateNewPPtrKeyframe(float time, Object value, AnimationWindowCurve targetCurve)
        {
            ObjectReferenceKeyframe key = new ObjectReferenceKeyframe {
                time = time,
                value = value
            };
            AnimationWindowKeyframe keyframe2 = new AnimationWindowKeyframe(targetCurve, key);
            AnimationKeyTime keyTime = AnimationKeyTime.Time(keyframe2.time, this.state.frameRate);
            targetCurve.AddKeyframe(keyframe2, keyTime);
            this.state.SelectKey(keyframe2);
        }

        private EditorCurveBinding? CreateNewSpriteRendererDopeline()
        {
            if (this.state.activeGameObject.GetComponent<SpriteRenderer>() == null)
            {
                this.state.activeGameObject.AddComponent<SpriteRenderer>();
            }
            List<EditorCurveBinding> list = AnimationWindowUtility.GetAnimatableProperties(this.state.activeRootGameObject, this.state.activeRootGameObject, typeof(SpriteRenderer), typeof(Sprite));
            if (list.Count == 1)
            {
                return new EditorCurveBinding?(list[0]);
            }
            Debug.LogError("Unable to create animatable SpriteRenderer component");
            return null;
        }

        private void DeleteSelectedKeys()
        {
            this.state.DeleteSelectedKeys();
        }

        private bool DoDragAndDrop(DopeLine dopeLine, bool perform)
        {
            return this.DoDragAndDrop(dopeLine, new Rect(), true, perform);
        }

        private bool DoDragAndDrop(DopeLine dopeLine, Rect position, bool perform)
        {
            return this.DoDragAndDrop(dopeLine, position, false, perform);
        }

        private bool DoDragAndDrop(DopeLine dopeLine, Rect position, bool ignoreMousePosition, bool perform)
        {
            if (ignoreMousePosition || position.Contains(Event.current.mousePosition))
            {
                if (!ValidateDragAndDropObjects())
                {
                    return false;
                }
                Type type = DragAndDrop.objectReferences[0].GetType();
                AnimationWindowCurve curve = null;
                if (dopeLine.valueType == type)
                {
                    curve = dopeLine.m_Curves[0];
                }
                else
                {
                    foreach (AnimationWindowCurve curve2 in dopeLine.m_Curves)
                    {
                        if (curve2.isPPtrCurve)
                        {
                            if (curve2.m_ValueType == type)
                            {
                                curve = curve2;
                            }
                            Sprite[] spriteFromDraggedPathsOrObjects = SpriteUtility.GetSpriteFromDraggedPathsOrObjects();
                            if (((curve2.m_ValueType == typeof(Sprite)) && (spriteFromDraggedPathsOrObjects != null)) && (spriteFromDraggedPathsOrObjects.Length > 0))
                            {
                                curve = curve2;
                                type = typeof(Sprite);
                            }
                        }
                    }
                }
                bool flag = true;
                if (curve != null)
                {
                    if (perform)
                    {
                        if (DragAndDrop.objectReferences.Length == 1)
                        {
                            Analytics.Event("Sprite Drag and Drop", "Drop single sprite into existing dopeline", "null", 1);
                        }
                        else
                        {
                            Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites into existing dopeline", "null", 1);
                        }
                        Rect dragAndDropRect = this.GetDragAndDropRect(dopeLine, Event.current.mousePosition.x);
                        float time = Mathf.Max(this.state.PixelToTime(dragAndDropRect.xMin, true), 0f);
                        AnimationWindowCurve curveOfType = this.GetCurveOfType(dopeLine, type);
                        this.PeformDragAndDrop(curveOfType, time);
                    }
                    return flag;
                }
            }
            return false;
        }

        private bool DopelineForValueTypeExists(Type valueType)
        {
            string str = AnimationUtility.CalculateTransformPath(this.state.activeGameObject.transform, this.state.activeRootGameObject.transform);
            foreach (DopeLine line in this.state.dopelines)
            {
                if (line.valueType == valueType)
                {
                    AnimationWindowHierarchyNode node = (AnimationWindowHierarchyNode) this.state.hierarchyData.FindItem(line.m_HierarchyNodeID);
                    if ((node != null) && node.path.Equals(str))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return true;
                    }
                }
            }
            return false;
        }

        private void DopeLineRepaint(DopeLine dopeline)
        {
            Color color = GUI.color;
            AnimationWindowHierarchyNode node = (AnimationWindowHierarchyNode) this.state.hierarchyData.FindItem(dopeline.m_HierarchyNodeID);
            Color color2 = ((node == null) || (node.depth <= 0)) ? Color.gray.AlphaMultiplied(0.16f) : Color.gray.AlphaMultiplied(0.05f);
            if (!dopeline.isMasterDopeline)
            {
                DrawBox(dopeline.position, color2);
            }
            int? nullable = null;
            int count = dopeline.keys.Count;
            for (int i = 0; i < count; i++)
            {
                AnimationWindowKeyframe keyframe = dopeline.keys[i];
                if ((nullable.GetValueOrDefault() != keyframe.m_TimeHash) || !nullable.HasValue)
                {
                    nullable = new int?(keyframe.m_TimeHash);
                    Rect keyframeRect = this.GetKeyframeRect(dopeline, keyframe);
                    color2 = !dopeline.isMasterDopeline ? Color.gray.RGBMultiplied((float) 1.2f) : Color.gray.RGBMultiplied((float) 0.85f);
                    Texture2D texture = null;
                    if (keyframe.isPPtrCurve && dopeline.tallMode)
                    {
                        texture = (keyframe.value != null) ? AssetPreview.GetAssetPreview(((Object) keyframe.value).GetInstanceID(), this.assetPreviewManagerID) : null;
                    }
                    if (texture != null)
                    {
                        keyframeRect = this.GetPreviewRectFromKeyFrameRect(keyframeRect);
                        color2 = Color.white.AlphaMultiplied(0.5f);
                    }
                    else if (((keyframe.value != null) && keyframe.isPPtrCurve) && dopeline.tallMode)
                    {
                        this.m_SpritePreviewLoading = true;
                    }
                    if (Mathf.Approximately(keyframe.time, 0f))
                    {
                        keyframeRect.xMin -= 0.01f;
                    }
                    if (this.AnyKeyIsSelectedAtTime(dopeline, i))
                    {
                        color2 = (!dopeline.tallMode || !dopeline.isPptrDopeline) ? new Color(0.34f, 0.52f, 0.85f, 1f) : Color.white;
                        if (dopeline.isMasterDopeline)
                        {
                            color2 = color2.RGBMultiplied((float) 0.85f);
                        }
                        this.selectedKeysDrawBuffer.Add(new DrawElement(keyframeRect, color2, texture));
                    }
                    else
                    {
                        this.unselectedKeysDrawBuffer.Add(new DrawElement(keyframeRect, color2, texture));
                    }
                }
            }
            if (this.state.clipIsEditable && this.DoDragAndDrop(dopeline, dopeline.position, false))
            {
                float time = Mathf.Max(this.state.PixelToTime(Event.current.mousePosition.x), 0f);
                Color color3 = Color.gray.RGBMultiplied((float) 1.2f);
                Texture2D assetPreview = null;
                foreach (Object obj2 in this.GetSortedDragAndDropObjectReferences())
                {
                    Rect dragAndDropRect = this.GetDragAndDropRect(dopeline, this.state.TimeToPixel(time));
                    if (dopeline.isPptrDopeline && dopeline.tallMode)
                    {
                        assetPreview = AssetPreview.GetAssetPreview(obj2.GetInstanceID(), this.assetPreviewManagerID);
                    }
                    if (assetPreview != null)
                    {
                        dragAndDropRect = this.GetPreviewRectFromKeyFrameRect(dragAndDropRect);
                        color3 = Color.white.AlphaMultiplied(0.5f);
                    }
                    this.dragdropKeysDrawBuffer.Add(new DrawElement(dragAndDropRect, color3, assetPreview));
                    time += 1f / this.state.frameRate;
                }
            }
            GUI.color = color;
        }

        private Rect DopelinesGUI(Rect position, Vector2 scrollPosition)
        {
            Color color = GUI.color;
            Rect rect = position;
            this.selectedKeysDrawBuffer = new List<DrawElement>();
            this.unselectedKeysDrawBuffer = new List<DrawElement>();
            this.dragdropKeysDrawBuffer = new List<DrawElement>();
            if (Event.current.type == EventType.Repaint)
            {
                this.m_SpritePreviewLoading = false;
            }
            if (Event.current.type == EventType.MouseDown)
            {
                this.m_IsDragging = false;
            }
            this.UpdateSpritePreviewCacheSize();
            foreach (DopeLine line in this.state.dopelines)
            {
                line.position = rect;
                line.position.height = !line.tallMode ? 16f : 32f;
                if ((((line.position.yMin + scrollPosition.y) >= position.yMin) && ((line.position.yMin + scrollPosition.y) <= position.yMax)) || (((line.position.yMax + scrollPosition.y) >= position.yMin) && ((line.position.yMax + scrollPosition.y) <= position.yMax)))
                {
                    Event current = Event.current;
                    EventType type = current.type;
                    switch (type)
                    {
                        case EventType.Repaint:
                            this.DopeLineRepaint(line);
                            goto Label_01B7;

                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if (this.state.clipIsEditable)
                            {
                                this.HandleDragAndDrop(line);
                            }
                            goto Label_01B7;
                    }
                    if (type != EventType.MouseDown)
                    {
                        if ((type == EventType.ContextClick) && !this.m_IsDraggingPlayhead)
                        {
                            this.HandleContextMenu(line);
                        }
                    }
                    else if (current.button == 0)
                    {
                        this.HandleMouseDown(line);
                    }
                }
            Label_01B7:
                rect.y += line.position.height;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                this.m_IsDraggingPlayheadStarted = false;
                this.m_IsDraggingPlayhead = false;
            }
            Rect rect2 = new Rect(position.xMin, position.yMin, position.width, rect.yMax - position.yMin);
            this.DrawElements(this.unselectedKeysDrawBuffer);
            this.DrawElements(this.selectedKeysDrawBuffer);
            this.DrawElements(this.dragdropKeysDrawBuffer);
            GUI.color = color;
            return rect2;
        }

        private void DoSpriteDropAfterGeneratingNewDopeline(EditorCurveBinding? spriteBinding)
        {
            if (DragAndDrop.objectReferences.Length == 1)
            {
                Analytics.Event("Sprite Drag and Drop", "Drop single sprite into empty dopesheet", "null", 1);
            }
            else
            {
                Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites into empty dopesheet", "null", 1);
            }
            AnimationWindowCurve curve = new AnimationWindowCurve(this.state.activeAnimationClip, spriteBinding.Value, typeof(Sprite));
            this.state.SaveCurve(curve);
            this.PeformDragAndDrop(curve, 0f);
        }

        private static void DrawBox(Rect position, Color color)
        {
            Color color2 = GUI.color;
            GUI.color = color;
            DopeLine.dopekeyStyle.Draw(position, GUIContent.none, 0, false);
            GUI.color = color2;
        }

        private void DrawElements(List<DrawElement> elements)
        {
            Color color = GUI.color;
            Color white = Color.white;
            GUI.color = white;
            Texture defaultDopeKeyIcon = this.m_DefaultDopeKeyIcon;
            foreach (DrawElement element in elements)
            {
                if (element.color != white)
                {
                    white = !GUI.enabled ? ((Color) (element.color * 0.8f)) : element.color;
                    GUI.color = white;
                }
                if (element.texture != null)
                {
                    GUI.DrawTexture(element.position, element.texture);
                }
                else
                {
                    Rect position = new Rect(element.position.center.x - (defaultDopeKeyIcon.width / 2), element.position.center.y - (defaultDopeKeyIcon.height / 2), (float) defaultDopeKeyIcon.width, (float) defaultDopeKeyIcon.height);
                    GUI.DrawTexture(position, defaultDopeKeyIcon, ScaleMode.ScaleToFit, true, 1f);
                }
            }
            GUI.color = color;
        }

        private void DrawGrid(Rect position)
        {
            base.TimeRuler(position, this.state.frameRate, false, true, 0.2f);
        }

        public void DrawMasterDopelineBackground(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                AnimationWindowStyles.eventBackground.Draw(position, false, false, false, false);
            }
        }

        public void FrameClip()
        {
            if (this.state.activeAnimationClip != null)
            {
                float max = Mathf.Max(this.state.activeAnimationClip.length, 1f);
                base.SetShownHRangeInsideMargins(0f, max);
            }
        }

        public void FrameSelected()
        {
            float maxValue = float.MaxValue;
            float minValue = float.MinValue;
            bool flag = this.state.selectedKeys.Count > 0;
            if (flag)
            {
                foreach (AnimationWindowKeyframe keyframe in this.state.selectedKeys)
                {
                    maxValue = Mathf.Min(keyframe.time, maxValue);
                    minValue = Mathf.Max(keyframe.time, minValue);
                }
            }
            bool flag2 = !flag;
            if (!flag && (this.state.hierarchyState.selectedIDs.Count > 0))
            {
                foreach (AnimationWindowCurve curve in this.state.activeCurves)
                {
                    int count = curve.m_Keyframes.Count;
                    if (count > 1)
                    {
                        maxValue = Mathf.Min(curve.m_Keyframes[0].time, maxValue);
                        minValue = Mathf.Max(curve.m_Keyframes[count - 1].time, minValue);
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                this.FrameClip();
            }
            else
            {
                float num4 = Mathf.Min(this.state.FrameToTime(Mathf.Min(4f, this.state.frameRate)), Mathf.Max(this.state.activeAnimationClip.length, this.state.FrameToTime(4f)));
                minValue = Mathf.Max(minValue, maxValue + num4);
                base.SetShownHRangeInsideMargins(maxValue, minValue);
            }
        }

        private GenericMenu GenerateMenu(DopeLine dopeline, bool clickedEmpty)
        {
            GenericMenu menu = new GenericMenu();
            this.state.recording = true;
            this.state.ResampleAnimation();
            string text = "Add Key";
            if (clickedEmpty)
            {
                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeyToDopeline), dopeline);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(text));
            }
            text = (this.state.selectedKeys.Count <= 1) ? "Delete Key" : "Delete Keys";
            if (this.state.selectedKeys.Count > 0)
            {
                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction(this.DeleteSelectedKeys));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(text));
            }
            if (AnimationWindowUtility.ContainsFloatKeyframes(this.state.selectedKeys))
            {
                menu.AddSeparator(string.Empty);
                List<KeyIdentifier> keyList = new List<KeyIdentifier>();
                foreach (AnimationWindowKeyframe keyframe in this.state.selectedKeys)
                {
                    if (!keyframe.isPPtrCurve)
                    {
                        int keyframeIndex = keyframe.curve.GetKeyframeIndex(AnimationKeyTime.Time(keyframe.time, this.state.frameRate));
                        if (keyframeIndex != -1)
                        {
                            CurveRenderer curveRenderer = CurveRendererCache.GetCurveRenderer(this.state.activeAnimationClip, keyframe.curve.binding);
                            int curveID = CurveUtility.GetCurveID(this.state.activeAnimationClip, keyframe.curve.binding);
                            keyList.Add(new KeyIdentifier(curveRenderer, curveID, keyframeIndex, keyframe.curve.binding));
                        }
                    }
                }
                new CurveMenuManager(this).AddTangentMenuItems(menu, keyList);
            }
            return menu;
        }

        private AnimationWindowCurve GetCurveOfType(DopeLine dopeLine, Type type)
        {
            foreach (AnimationWindowCurve curve in dopeLine.m_Curves)
            {
                if (curve.m_ValueType == type)
                {
                    return curve;
                }
            }
            return null;
        }

        private Rect GetDragAndDropRect(DopeLine dopeline, float screenX)
        {
            Rect keyframeRect = this.GetKeyframeRect(dopeline, null);
            float keyframeOffset = this.GetKeyframeOffset(dopeline, null);
            float time = Mathf.Max(this.state.PixelToTime(screenX - (keyframeRect.width * 0.5f), true), 0f);
            keyframeRect.center = new Vector2((this.state.TimeToPixel(time) + (keyframeRect.width * 0.5f)) + keyframeOffset, keyframeRect.center.y);
            return keyframeRect;
        }

        private float GetKeyframeOffset(DopeLine dopeline, AnimationWindowKeyframe keyframe)
        {
            if ((!dopeline.isPptrDopeline || !dopeline.tallMode) || ((keyframe != null) && (keyframe.value == null)))
            {
                return -5.5f;
            }
            return -1f;
        }

        private Rect GetKeyframeRect(DopeLine dopeline, AnimationWindowKeyframe keyframe)
        {
            float time = (keyframe == null) ? 0f : keyframe.time;
            float width = 10f;
            if ((dopeline.isPptrDopeline && dopeline.tallMode) && ((keyframe == null) || (keyframe.value != null)))
            {
                width = dopeline.position.height;
            }
            if (dopeline.isPptrDopeline && dopeline.tallMode)
            {
                return new Rect(this.state.TimeToPixel(this.state.SnapToFrame(time)) + this.GetKeyframeOffset(dopeline, keyframe), dopeline.position.yMin, width, dopeline.position.height);
            }
            return new Rect(this.state.TimeToPixel(this.state.SnapToFrame(time)) + this.GetKeyframeOffset(dopeline, keyframe), dopeline.position.yMin, width, dopeline.position.height);
        }

        private Rect GetPreviewRectFromKeyFrameRect(Rect keyframeRect)
        {
            keyframeRect.width -= 2f;
            keyframeRect.height -= 2f;
            keyframeRect.xMin += 2f;
            keyframeRect.yMin += 2f;
            return keyframeRect;
        }

        private Object[] GetSortedDragAndDropObjectReferences()
        {
            Object[] objectReferences = DragAndDrop.objectReferences;
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = (a, b) => EditorUtility.NaturalCompare(a.name, b.name);
            }
            Array.Sort<Object>(objectReferences, <>f__am$cache11);
            return objectReferences;
        }

        private void HandleContextMenu(DopeLine dopeline)
        {
            if (dopeline.position.Contains(Event.current.mousePosition))
            {
                bool clickedEmpty = true;
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    if (this.GetKeyframeRect(dopeline, keyframe).Contains(Event.current.mousePosition))
                    {
                        clickedEmpty = false;
                        break;
                    }
                }
                this.GenerateMenu(dopeline, clickedEmpty).ShowAsContext();
            }
        }

        private void HandleDelete()
        {
            switch (Event.current.type)
            {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    if ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete"))
                    {
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.state.DeleteSelectedKeys();
                        }
                        Event.current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if ((Event.current.keyCode == KeyCode.Backspace) || (Event.current.keyCode == KeyCode.Delete))
                    {
                        this.state.DeleteSelectedKeys();
                        Event.current.Use();
                    }
                    break;
            }
        }

        private void HandleDopelineDoubleclick(DopeLine dopeline)
        {
            this.state.ClearSelections();
            AnimationKeyTime time = AnimationKeyTime.Time(this.state.PixelToTime(Event.current.mousePosition.x, true), this.state.frameRate);
            foreach (AnimationWindowCurve curve in dopeline.m_Curves)
            {
                AnimationWindowKeyframe keyframe = AnimationWindowUtility.AddKeyframeToCurve(this.state, curve, time);
                this.state.SelectKey(keyframe);
            }
            if (!this.state.playing)
            {
                this.state.frame = time.frame;
            }
            Event.current.Use();
        }

        private void HandleDragAndDrop(DopeLine dopeline)
        {
            Event current = Event.current;
            if ((current.type == EventType.DragPerform) || (current.type == EventType.DragUpdated))
            {
                if (this.DoDragAndDrop(dopeline, dopeline.position, current.type == EventType.DragPerform))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    current.Use();
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
        }

        private void HandleDragAndDropToEmptyArea()
        {
            Event current = Event.current;
            if (((current.type == EventType.DragPerform) || (current.type == EventType.DragUpdated)) && (ValidateDragAndDropObjects() && (this.state.activeGameObject != null)))
            {
                if ((DragAndDrop.objectReferences[0].GetType() == typeof(Sprite)) || (DragAndDrop.objectReferences[0].GetType() == typeof(Texture2D)))
                {
                    if (!this.DopelineForValueTypeExists(typeof(Sprite)))
                    {
                        if (current.type == EventType.DragPerform)
                        {
                            EditorCurveBinding? spriteBinding = this.CreateNewPptrDopeline(typeof(Sprite));
                            if (spriteBinding.HasValue)
                            {
                                this.DoSpriteDropAfterGeneratingNewDopeline(spriteBinding);
                            }
                        }
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        current.Use();
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
        }

        private void HandleDragging()
        {
            int num = GUIUtility.GetControlID("dopesheetdrag".GetHashCode(), FocusType.Passive, new Rect());
            if (((Event.current.type == EventType.MouseDrag) || (Event.current.type == EventType.MouseUp)) && this.m_MousedownOnKeyframe)
            {
                if ((((Event.current.type == EventType.MouseDrag) && !EditorGUI.actionKey) && (!Event.current.shift && !this.m_IsDragging)) && (this.state.selectedKeys.Count > 0))
                {
                    this.m_IsDragging = true;
                    this.m_IsDraggingPlayheadStarted = true;
                    GUIUtility.hotControl = num;
                    this.m_DragStartTime = this.state.PixelToTime(Event.current.mousePosition.x);
                    Event.current.Use();
                }
                float maxValue = float.MaxValue;
                foreach (AnimationWindowKeyframe keyframe in this.state.selectedKeys)
                {
                    maxValue = Mathf.Min(keyframe.time, maxValue);
                }
                float a = this.state.SnapToFrame(this.state.PixelToTime(Event.current.mousePosition.x));
                float deltaTime = Mathf.Max((float) (a - this.m_DragStartTime), (float) (maxValue * -1f));
                if (this.m_IsDragging && !Mathf.Approximately(a, this.m_DragStartTime))
                {
                    this.m_DragStartTime = a;
                    this.state.MoveSelectedKeys(deltaTime, true, false);
                    if ((this.state.activeKeyframe != null) && !this.state.playing)
                    {
                        this.state.frame = this.state.TimeToFrameFloor(this.state.activeKeyframe.time);
                    }
                    Event.current.Use();
                }
                if (Event.current.type == EventType.MouseUp)
                {
                    if (this.m_IsDragging && (GUIUtility.hotControl == num))
                    {
                        this.state.MoveSelectedKeys(deltaTime, true, true);
                        Event.current.Use();
                        this.m_IsDragging = false;
                    }
                    this.m_MousedownOnKeyframe = false;
                    GUIUtility.hotControl = 0;
                }
            }
            if ((this.m_IsDraggingPlayheadStarted && (Event.current.type == EventType.MouseDrag)) && (Event.current.button == 1))
            {
                this.m_IsDraggingPlayhead = true;
                Event.current.Use();
            }
        }

        private void HandleKeyboard()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                string commandName = Event.current.commandName;
                if (commandName != null)
                {
                    int num;
                    if (<>f__switch$mapD == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                        dictionary.Add("SelectAll", 0);
                        dictionary.Add("FrameSelected", 1);
                        <>f__switch$mapD = dictionary;
                    }
                    if (<>f__switch$mapD.TryGetValue(commandName, out num))
                    {
                        if (num == 0)
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.HandleSelectAll();
                            }
                            Event.current.Use();
                        }
                        else if (num == 1)
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.FrameSelected();
                            }
                            Event.current.Use();
                        }
                    }
                }
            }
        }

        private void HandleMouseDown(DopeLine dopeline)
        {
            Event current = Event.current;
            if (!EditorGUI.actionKey && !current.shift)
            {
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    if (this.GetKeyframeRect(dopeline, keyframe).Contains(current.mousePosition) && !this.state.KeyIsSelected(keyframe))
                    {
                        this.state.ClearSelections();
                        break;
                    }
                }
            }
            float num = this.state.PixelToTime(Event.current.mousePosition.x);
            float num2 = num;
            if (Event.current.shift)
            {
                foreach (AnimationWindowKeyframe keyframe2 in dopeline.keys)
                {
                    if (this.state.KeyIsSelected(keyframe2))
                    {
                        if (keyframe2.time < num)
                        {
                            num = keyframe2.time;
                        }
                        if (keyframe2.time > num2)
                        {
                            num2 = keyframe2.time;
                        }
                    }
                }
            }
            bool flag = false;
            foreach (AnimationWindowKeyframe keyframe3 in dopeline.keys)
            {
                if (this.GetKeyframeRect(dopeline, keyframe3).Contains(current.mousePosition))
                {
                    flag = true;
                    if (!this.state.KeyIsSelected(keyframe3))
                    {
                        if (Event.current.shift)
                        {
                            foreach (AnimationWindowKeyframe keyframe4 in dopeline.keys)
                            {
                                if ((keyframe4 == keyframe3) || ((keyframe4.time > num) && (keyframe4.time < num2)))
                                {
                                    this.state.SelectKey(keyframe4);
                                }
                            }
                        }
                        else
                        {
                            this.state.SelectKey(keyframe3);
                        }
                        if (!dopeline.isMasterDopeline)
                        {
                            this.state.SelectHierarchyItem(dopeline, EditorGUI.actionKey || current.shift);
                        }
                    }
                    else if (EditorGUI.actionKey)
                    {
                        this.state.UnselectKey(keyframe3);
                        if (!this.state.AnyKeyIsSelected(dopeline))
                        {
                            this.state.UnSelectHierarchyItem(dopeline);
                        }
                    }
                    this.state.activeKeyframe = keyframe3;
                    this.m_MousedownOnKeyframe = true;
                    if (!this.state.playing)
                    {
                        this.state.frame = this.state.TimeToFrameFloor(this.state.activeKeyframe.time);
                    }
                    current.Use();
                }
            }
            if (dopeline.isMasterDopeline)
            {
                this.state.ClearHierarchySelection();
                foreach (int num3 in this.state.GetAffectedHierarchyIDs(this.state.selectedKeys))
                {
                    this.state.SelectHierarchyItem(num3, true, true);
                }
            }
            if (dopeline.position.Contains(Event.current.mousePosition))
            {
                if (((current.clickCount == 2) && (current.button == 0)) && (!Event.current.shift && !EditorGUI.actionKey))
                {
                    this.HandleDopelineDoubleclick(dopeline);
                }
                if ((current.button == 1) && !this.state.playing)
                {
                    AnimationKeyTime time = AnimationKeyTime.Time(this.state.PixelToTime(Event.current.mousePosition.x, true), this.state.frameRate);
                    this.state.frame = time.frame;
                    if (!flag)
                    {
                        this.state.ClearSelections();
                        this.m_IsDraggingPlayheadStarted = true;
                        HandleUtility.Repaint();
                        current.Use();
                    }
                }
            }
        }

        private void HandleSelectAll()
        {
            foreach (DopeLine line in this.state.dopelines)
            {
                foreach (AnimationWindowKeyframe keyframe in line.keys)
                {
                    this.state.SelectKey(keyframe);
                }
                this.state.SelectHierarchyItem(line, true, false);
            }
        }

        private void HandleSelectionRect(Rect rect)
        {
            if (this.m_SelectionRect == null)
            {
                this.m_SelectionRect = new DopeSheetSelectionRect(this);
            }
            if (!this.m_MousedownOnKeyframe)
            {
                this.m_SelectionRect.OnGUI(rect);
            }
        }

        public void Init()
        {
            if (!this.m_Initialized)
            {
                if (this.m_DefaultDopeKeyIcon == null)
                {
                    this.m_DefaultDopeKeyIcon = EditorGUIUtility.LoadIcon("blendKey");
                }
                base.hSlider = true;
                base.vSlider = false;
                base.hRangeLocked = false;
                base.vRangeLocked = true;
                base.hRangeMin = 0f;
                base.margin = 40f;
                base.scaleWithWindow = true;
                base.ignoreScrollWheelUntilClicked = false;
            }
            this.m_Initialized = true;
        }

        internal void OnDestroy()
        {
            AssetPreview.DeletePreviewTextureManagerByID(this.assetPreviewManagerID);
        }

        public void OnGUI(Rect position, Vector2 scrollPosition)
        {
            this.Init();
            EditorGUI.BeginDisabledGroup(!this.state.clipIsEditable);
            this.HandleDragAndDropToEmptyArea();
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(this.state.animationIsReadOnly);
            GUIClip.Push(position, scrollPosition, Vector2.zero, false);
            Rect rect = new Rect(0f, 0f, position.width, position.height);
            Rect rect2 = this.DopelinesGUI(rect, scrollPosition);
            if (GUI.enabled)
            {
                this.HandleKeyboard();
                this.HandleDragging();
                this.HandleSelectionRect(rect2);
                this.HandleDelete();
            }
            GUIClip.Pop();
            EditorGUI.EndDisabledGroup();
        }

        private void PeformDragAndDrop(AnimationWindowCurve targetCurve, float time)
        {
            if ((DragAndDrop.objectReferences.Length != 0) && (targetCurve != null))
            {
                this.state.ClearSelections();
                foreach (Object obj2 in this.GetSortedDragAndDropObjectReferences())
                {
                    Object obj3 = obj2;
                    if (obj3 is Texture2D)
                    {
                        obj3 = SpriteUtility.TextureToSprite(obj2 as Texture2D);
                    }
                    this.CreateNewPPtrKeyframe(time, obj3, targetCurve);
                    time += 1f / this.state.activeAnimationClip.frameRate;
                }
                this.state.SaveCurve(targetCurve);
                DragAndDrop.AcceptDrag();
            }
        }

        public void RecalculateBounds()
        {
            if (this.state.activeAnimationClip != null)
            {
                this.m_Bounds.SetMinMax(new Vector3(this.state.activeAnimationClip.startTime, 0f, 0f), new Vector3(this.state.activeAnimationClip.stopTime, 0f, 0f));
            }
        }

        private void SelectTypeForCreatingNewPptrDopeline(object userData, string[] options, int selected)
        {
            EditorCurveBinding[] bindingArray = userData as EditorCurveBinding[];
            if (bindingArray.Length > selected)
            {
                this.DoSpriteDropAfterGeneratingNewDopeline(new EditorCurveBinding?(bindingArray[selected]));
            }
        }

        public void UpdateCurves(List<int> curveIds, string undoText)
        {
        }

        public void UpdateCurves(List<ChangedCurve> changedCurves, string undoText)
        {
            Undo.RegisterCompleteObjectUndo(this.state.activeAnimationClip, undoText);
            foreach (ChangedCurve curve in changedCurves)
            {
                AnimationUtility.SetEditorCurve(this.state.activeAnimationClip, curve.binding, curve.curve);
            }
        }

        private void UpdateSpritePreviewCacheSize()
        {
            int size = 1;
            foreach (DopeLine line in this.state.dopelines)
            {
                if (line.tallMode && line.isPptrDopeline)
                {
                    size += line.keys.Count;
                }
            }
            size += DragAndDrop.objectReferences.Length;
            if (size > this.m_SpritePreviewCacheSize)
            {
                AssetPreview.SetPreviewTextureCacheSize(size, this.assetPreviewManagerID);
                this.m_SpritePreviewCacheSize = size;
            }
        }

        private static bool ValidateDragAndDropObjects()
        {
            if (DragAndDrop.objectReferences.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                Object obj2 = DragAndDrop.objectReferences[i];
                if (obj2 == null)
                {
                    return false;
                }
                if (i < (DragAndDrop.objectReferences.Length - 1))
                {
                    Object obj3 = DragAndDrop.objectReferences[i + 1];
                    bool flag = ((obj2 is Texture2D) || (obj2 is Sprite)) && ((obj3 is Texture2D) || (obj3 is Sprite));
                    if ((obj2.GetType() != obj3.GetType()) && !flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal int assetPreviewManagerID
        {
            get
            {
                return ((this.m_Owner == null) ? 0 : this.m_Owner.GetInstanceID());
            }
        }

        public float contentHeight
        {
            get
            {
                float num = 0f;
                foreach (DopeLine line in this.state.dopelines)
                {
                    num += !line.tallMode ? 16f : 32f;
                }
                return (num + 40f);
            }
        }

        public override Bounds drawingBounds
        {
            get
            {
                return this.m_Bounds;
            }
        }

        internal class DopeSheetPopup
        {
            private Rect backgroundRect;
            private Rect position;
            private static int s_height = 0x70;
            private static int s_width = 0x60;

            public DopeSheetPopup(Rect position)
            {
                this.position = position;
            }

            public void OnGUI(AnimationWindowState state, AnimationWindowKeyframe keyframe)
            {
                if (!keyframe.isPPtrCurve)
                {
                    this.backgroundRect = this.position;
                    this.backgroundRect.x = (state.TimeToPixel(keyframe.time) + this.position.x) - (s_width / 2);
                    this.backgroundRect.y += 16f;
                    this.backgroundRect.width = s_width;
                    this.backgroundRect.height = s_height;
                    Rect backgroundRect = this.backgroundRect;
                    backgroundRect.height = 16f;
                    Rect position = this.backgroundRect;
                    position.y += 16f;
                    position.height = s_width;
                    GUI.Box(this.backgroundRect, string.Empty);
                    GUI.Box(position, AssetPreview.GetAssetPreview((Object) keyframe.value));
                    EditorGUI.BeginChangeCheck();
                    Object obj2 = EditorGUI.ObjectField(backgroundRect, (Object) keyframe.value, keyframe.curve.m_ValueType, false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        keyframe.value = obj2;
                        state.SaveCurve(keyframe.curve);
                    }
                }
            }
        }

        internal class DopeSheetSelectionRect
        {
            public readonly GUIStyle createRect = "U2D.createRect";
            private Vector2 m_SelectMousePoint;
            private Vector2 m_SelectStartPoint;
            private bool m_ValidRect;
            private DopeSheetEditor owner;
            private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

            public DopeSheetSelectionRect(DopeSheetEditor owner)
            {
                this.owner = owner;
            }

            public Rect GetCurrentPixelRect()
            {
                float num = 16f;
                Rect rect = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
                rect.xMin = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(rect.xMin, true), true);
                rect.xMax = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(rect.xMax, true), true);
                rect.yMin = Mathf.Floor(rect.yMin / num) * num;
                rect.yMax = (Mathf.Floor(rect.yMax / num) + 1f) * num;
                return rect;
            }

            public Rect GetCurrentTimeRect()
            {
                float num = 16f;
                Rect rect = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
                rect.xMin = this.owner.state.PixelToTime(rect.xMin, true);
                rect.xMax = this.owner.state.PixelToTime(rect.xMax, true);
                rect.yMin = Mathf.Floor(rect.yMin / num) * num;
                rect.yMax = (Mathf.Floor(rect.yMax / num) + 1f) * num;
                return rect;
            }

            public void OnGUI(Rect position)
            {
                Event current = Event.current;
                Vector2 mousePosition = current.mousePosition;
                int controlID = s_RectSelectionID;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if ((current.button == 0) && position.Contains(mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            this.m_SelectStartPoint = mousePosition;
                            this.m_ValidRect = false;
                            current.Use();
                        }
                        return;

                    case EventType.MouseUp:
                    {
                        if ((GUIUtility.hotControl != controlID) || (current.button != 0))
                        {
                            return;
                        }
                        if (!this.m_ValidRect)
                        {
                            this.owner.state.ClearSelections();
                            break;
                        }
                        if (!EditorGUI.actionKey)
                        {
                            this.owner.state.ClearSelections();
                        }
                        float frameRate = this.owner.state.frameRate;
                        Rect currentTimeRect = this.GetCurrentTimeRect();
                        AnimationKeyTime time = AnimationKeyTime.Time(currentTimeRect.xMin, frameRate);
                        AnimationKeyTime time2 = AnimationKeyTime.Time(currentTimeRect.xMax, frameRate);
                        GUI.changed = true;
                        this.owner.state.ClearHierarchySelection();
                        List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
                        List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
                        foreach (DopeLine line in this.owner.state.dopelines)
                        {
                            if ((line.position.yMin >= currentTimeRect.yMin) && (line.position.yMax <= currentTimeRect.yMax))
                            {
                                foreach (AnimationWindowKeyframe keyframe in line.keys)
                                {
                                    AnimationKeyTime time3 = AnimationKeyTime.Time(keyframe.time, frameRate);
                                    if ((((!line.tallMode && (time3.frame >= time.frame)) && (time3.frame <= time2.frame)) || ((line.tallMode && (time3.frame >= time.frame)) && (time3.frame < time2.frame))) && (!list2.Contains(keyframe) && !list.Contains(keyframe)))
                                    {
                                        if (!this.owner.state.KeyIsSelected(keyframe))
                                        {
                                            list2.Add(keyframe);
                                        }
                                        else if (this.owner.state.KeyIsSelected(keyframe))
                                        {
                                            list.Add(keyframe);
                                        }
                                    }
                                }
                            }
                        }
                        if (list2.Count == 0)
                        {
                            foreach (AnimationWindowKeyframe keyframe2 in list)
                            {
                                this.owner.state.UnselectKey(keyframe2);
                            }
                        }
                        foreach (AnimationWindowKeyframe keyframe3 in list2)
                        {
                            this.owner.state.SelectKey(keyframe3);
                        }
                        foreach (DopeLine line2 in this.owner.state.dopelines)
                        {
                            if (this.owner.state.AnyKeyIsSelected(line2))
                            {
                                this.owner.state.SelectHierarchyItem(line2, true, false);
                            }
                        }
                        break;
                    }
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                        return;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            Vector2 vector2 = mousePosition - this.m_SelectStartPoint;
                            this.m_ValidRect = Mathf.Abs(vector2.x) > 1f;
                            if (this.m_ValidRect)
                            {
                                this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
                            }
                            current.Use();
                        }
                        return;

                    case EventType.Repaint:
                        if ((GUIUtility.hotControl == controlID) && this.m_ValidRect)
                        {
                            EditorStyles.selectionRect.Draw(this.GetCurrentPixelRect(), GUIContent.none, false, false, false, false);
                        }
                        return;

                    default:
                        return;
                }
                current.Use();
                GUIUtility.hotControl = 0;
            }

            private enum SelectionType
            {
                Normal,
                Additive,
                Subtractive
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DrawElement
        {
            public Rect position;
            public Color color;
            public Texture2D texture;
            public DrawElement(Rect position, Color color, Texture2D texture)
            {
                this.position = position;
                this.color = color;
                this.texture = texture;
            }
        }
    }
}

