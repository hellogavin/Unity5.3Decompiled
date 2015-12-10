namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class EditorGUI
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__am$cache5D;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache5E;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache5F;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache60;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache61;
        [CompilerGenerated]
        private static EditorUtility.SelectMenuItemFunction <>f__am$cache64;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map3;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map4;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map5;
        private static RecycledTextEditor activeEditor;
        private static bool bKeyEventActive = false;
        internal const int kControlVerticalSpacing = 2;
        internal static Color kCurveBGColor = new Color(0.337f, 0.337f, 0.337f, 1f);
        internal static Color kCurveColor = Color.green;
        internal static string kDoubleFieldFormatString = "g15";
        private static float kDragDeadzone = 16f;
        private const float kDragSensitivity = 0.03f;
        private const string kEmptyDropDownElement = "--empty--";
        internal static string kFloatFieldFormatString = "g7";
        private const double kFoldoutExpandTimeout = 0.7;
        private const float kIndentPerLevel = 15f;
        internal const int kInspTitlebarIconWidth = 0x10;
        private const int kInspTitlebarSpacing = 2;
        private const int kInspTitlebarToggleWidth = 0x10;
        internal static string kIntFieldFormatString = "#######0";
        internal const float kLabelW = 80f;
        internal const float kMiniLabelW = 13f;
        internal const float kNearFarLabelsWidth = 35f;
        internal const float kObjectFieldMiniThumbnailHeight = 18f;
        internal const float kObjectFieldMiniThumbnailWidth = 32f;
        internal const float kObjectFieldThumbnailHeight = 64f;
        internal const float kSingleLineHeight = 16f;
        internal const float kSliderMaxW = 100f;
        internal const float kSliderMinW = 60f;
        internal const float kSpacing = 5f;
        internal const float kSpacingSubLabel = 2f;
        internal const float kStructHeaderLineHeight = 16f;
        internal static int ms_IndentLevel = 0;
        internal static readonly string s_AllowedCharactersForFloat = "inftynaeINFTYNAE0123456789.,-*/+%^()";
        internal static readonly string s_AllowedCharactersForInt = "0123456789-*/+%^()";
        private static int s_ArraySizeFieldHash = "ArraySizeField".GetHashCode();
        private static int s_ButtonMouseDownHash = "ButtonMouseDown".GetHashCode();
        private static Stack<bool> s_ChangedStack = new Stack<bool>();
        internal static readonly GUIContent s_ClipingPlanesLabel = EditorGUIUtility.TextContent("Clipping Planes");
        internal static bool s_CollectingToolTips;
        private static int s_ColorHash = "s_ColorHash".GetHashCode();
        private static int s_ColorPickID;
        private static int s_CurveHash = "s_CurveHash".GetHashCode();
        private static int s_CurveID;
        internal static DelayedTextEditor s_DelayedTextEditor = new DelayedTextEditor();
        private static int s_DelayedTextFieldHash = "DelayedEditorTextField".GetHashCode();
        private static int s_DragCandidateState = 0;
        internal static bool s_Dragged = false;
        private static double s_DragSensitivity = 0.0;
        private static long s_DragStartIntValue = 0L;
        private static Vector2 s_DragStartPos;
        private static double s_DragStartValue = 0.0;
        internal static bool s_DragToPosition = true;
        private static int s_DragUpdatedOverID = 0;
        private static Stack<bool> s_EnabledStack = new Stack<bool>();
        private static int s_FloatFieldHash = "EditorTextField".GetHashCode();
        private static double s_FoldoutDestTime;
        private static int s_FoldoutHash = "Foldout".GetHashCode();
        private static int s_GenericField = "s_GenericField".GetHashCode();
        private static readonly int s_GradientHash = "s_GradientHash".GetHashCode();
        private static int s_GradientID;
        private static readonly GUIContent s_HDRWarning = new GUIContent(string.Empty, EditorGUIUtility.warningIcon, LocalizationDatabase.GetLocalizedString("For HDR colors the normalized LDR hex color value is shown"));
        private static GUIContent s_IconDropDown;
        private static Material s_IconTextureInactive;
        private static int s_KeyEventFieldHash = "KeyEventField".GetHashCode();
        private static int s_LayerMaskField = "s_LayerMaskField".GetHashCode();
        private static int s_MaskField = "s_MaskField".GetHashCode();
        private static int s_MinMaxSliderHash = "EditorMinMaxSlider".GetHashCode();
        private static GUIContent s_MixedValueContent = EditorGUIUtility.TextContent("—|Mixed Values");
        private static Color s_MixedValueContentColor = new Color(1f, 1f, 1f, 0.5f);
        private static Color s_MixedValueContentColorTemp = Color.white;
        private static int s_MouseDeltaReaderHash = "MouseDeltaReader".GetHashCode();
        private static Vector2 s_MouseDeltaReaderLastPos;
        internal static readonly GUIContent[] s_NearAndFarLabels = new GUIContent[] { EditorGUIUtility.TextContent("Near"), EditorGUIUtility.TextContent("Far") };
        private static int s_ObjectFieldHash = "s_ObjectFieldHash".GetHashCode();
        internal static string s_OriginalText = string.Empty;
        private static int s_PasswordFieldHash = "PasswordField".GetHashCode();
        private static SerializedProperty s_PendingPropertyDelete = null;
        private static SerializedProperty s_PendingPropertyKeyboardHandling = null;
        private static int s_PopupHash = "EditorPopup".GetHashCode();
        internal static bool s_PostPoneMove = false;
        private static int s_PPtrHash = "s_PPtrHash".GetHashCode();
        internal static GUIContent s_PrefixLabel = new GUIContent(null);
        internal static Rect s_PrefixRect;
        internal static GUIStyle s_PrefixStyle;
        internal static Rect s_PrefixTotalRect;
        private static int s_ProgressBarHash = "s_ProgressBarHash".GetHashCode();
        private static GUIContent s_PropertyFieldTempContent = new GUIContent();
        private static Stack<PropertyGUIData> s_PropertyStack = new Stack<PropertyGUIData>();
        internal static double s_RecycledCurrentEditingFloat;
        internal static long s_RecycledCurrentEditingInt;
        internal static string s_RecycledCurrentEditingString;
        internal static RecycledTextEditor s_RecycledEditor = new RecycledTextEditor();
        private static int s_SearchFieldHash = "EditorSearchField".GetHashCode();
        private static int s_SelectableLabelHash = "s_SelectableLabel".GetHashCode();
        internal static bool s_SelectAllOnMouseUp = true;
        private static bool s_ShowMixedValue;
        private static int s_SliderHash = "EditorSlider".GetHashCode();
        private static int s_SliderKnobHash = "EditorSliderKnob".GetHashCode();
        private static int s_SortingLayerFieldHash = "s_SortingLayerFieldHash".GetHashCode();
        private static int s_TagFieldHash = "s_TagFieldHash".GetHashCode();
        private static int s_TextAreaHash = "EditorTextField".GetHashCode();
        private static int s_TextFieldDropDownHash = "s_TextFieldDropDown".GetHashCode();
        private static int s_TextFieldHash = "EditorTextField".GetHashCode();
        private static int s_TitlebarHash = "GenericTitlebar".GetHashCode();
        private static int s_ToggleHash = "s_ToggleHash".GetHashCode();
        internal static string s_UnitString = string.Empty;
        private static float[] s_Vector2Floats = new float[2];
        private static float[] s_Vector3Floats = new float[3];
        private static float[] s_Vector4Floats = new float[4];
        private static bool s_WasBoldDefaultFont;
        private static GUIContent[] s_WHLabels = new GUIContent[] { EditorGUIUtility.TextContent("W"), EditorGUIUtility.TextContent("H") };
        private static GUIContent[] s_XYLabels = new GUIContent[] { EditorGUIUtility.TextContent("X"), EditorGUIUtility.TextContent("Y") };
        private static GUIContent[] s_XYZLabels = new GUIContent[] { EditorGUIUtility.TextContent("X"), EditorGUIUtility.TextContent("Y"), EditorGUIUtility.TextContent("Z") };
        private static GUIContent[] s_XYZWLabels = new GUIContent[] { EditorGUIUtility.TextContent("X"), EditorGUIUtility.TextContent("Y"), EditorGUIUtility.TextContent("Z"), EditorGUIUtility.TextContent("W") };

        internal static int ArraySizeField(Rect position, GUIContent label, int value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_ArraySizeFieldHash, FocusType.Keyboard, position);
            BeginChangeCheck();
            string s = DelayedTextFieldInternal(position, id, label, value.ToString(kIntFieldFormatString), "0123456789-", style);
            if (EndChangeCheck())
            {
                try
                {
                    value = int.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                }
                catch (FormatException)
                {
                }
            }
            return value;
        }

        public static void BeginChangeCheck()
        {
            s_ChangedStack.Push(GUI.changed);
            GUI.changed = false;
        }

        internal static void BeginCollectTooltips()
        {
            isCollectingTooltips = true;
        }

        public static void BeginDisabledGroup(bool disabled)
        {
            s_EnabledStack.Push(GUI.enabled);
            GUI.enabled &= !disabled;
        }

        internal static void BeginHandleMixedValueContentColor()
        {
            s_MixedValueContentColorTemp = GUI.contentColor;
            GUI.contentColor = !showMixedValue ? GUI.contentColor : (GUI.contentColor * s_MixedValueContentColor);
        }

        public static GUIContent BeginProperty(Rect totalPosition, GUIContent label, SerializedProperty property)
        {
            Highlighter.HighlightIdentifier(totalPosition, property.propertyPath);
            if (s_PendingPropertyKeyboardHandling != null)
            {
                DoPropertyFieldKeyboardHandling(s_PendingPropertyKeyboardHandling);
            }
            s_PendingPropertyKeyboardHandling = property;
            if (property == null)
            {
                string message = ((label != null) ? (label.text + ": ") : string.Empty) + "SerializedProperty is null";
                HelpBox(totalPosition, "null", MessageType.Error);
                throw new NullReferenceException(message);
            }
            s_PropertyFieldTempContent.text = LocalizationDatabase.GetLocalizedString((label != null) ? label.text : property.displayName);
            s_PropertyFieldTempContent.tooltip = !isCollectingTooltips ? null : ((label != null) ? label.tooltip : property.tooltip);
            string tooltip = ScriptAttributeUtility.GetHandler(property).tooltip;
            if (tooltip != null)
            {
                s_PropertyFieldTempContent.tooltip = tooltip;
            }
            s_PropertyFieldTempContent.image = (label != null) ? label.image : null;
            if (Event.current.alt && (property.serializedObject.inspectorMode != InspectorMode.Normal))
            {
                string propertyPath = property.propertyPath;
                s_PropertyFieldTempContent.text = propertyPath;
                s_PropertyFieldTempContent.tooltip = propertyPath;
            }
            bool boldDefaultFont = EditorGUIUtility.GetBoldDefaultFont();
            if ((property.serializedObject.targetObjects.Length == 1) && property.isInstantiatedPrefab)
            {
                EditorGUIUtility.SetBoldDefaultFont(property.prefabOverride);
            }
            s_PropertyStack.Push(new PropertyGUIData(property, totalPosition, boldDefaultFont, GUI.enabled, GUI.color));
            showMixedValue = property.hasMultipleDifferentValues;
            if (property.isAnimated)
            {
                Color animatedPropertyColor = AnimationMode.animatedPropertyColor;
                animatedPropertyColor.a *= GUI.color.a;
                GUI.color = animatedPropertyColor;
            }
            GUI.enabled &= property.editable;
            return s_PropertyFieldTempContent;
        }

        public static Bounds BoundsField(Rect position, Bounds value)
        {
            return BoundsField(position, GUIContent.none, value);
        }

        private static void BoundsField(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 3);
            if (EditorGUIUtility.wideMode && LabelHasContent(label))
            {
                position.y += 16f;
            }
            position.height = 16f;
            position = DrawBoundsFieldLabelsAndAdjustPositionForValues(position, EditorGUIUtility.wideMode && LabelHasContent(label));
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            valuesIterator.NextVisible(true);
            MultiPropertyField(position, s_XYZLabels, valuesIterator);
            valuesIterator.NextVisible(true);
            position.y += 16f;
            MultiPropertyField(position, s_XYZLabels, valuesIterator);
        }

        public static Bounds BoundsField(Rect position, GUIContent label, Bounds value)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 3);
            if (EditorGUIUtility.wideMode && LabelHasContent(label))
            {
                position.y += 16f;
            }
            position.height = 16f;
            position = DrawBoundsFieldLabelsAndAdjustPositionForValues(position, EditorGUIUtility.wideMode && LabelHasContent(label));
            value.center = Vector3Field(position, value.center);
            position.y += 16f;
            value.extents = Vector3Field(position, value.extents);
            return value;
        }

        internal static bool ButtonMouseDown(int id, Rect position, GUIContent content, GUIStyle style)
        {
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.KeyDown:
                    if ((GUIUtility.keyboardControl != id) || (current.character != ' '))
                    {
                        break;
                    }
                    Event.current.Use();
                    return true;

                case EventType.Repaint:
                    if (!showMixedValue)
                    {
                        style.Draw(position, content, id, false);
                    }
                    else
                    {
                        BeginHandleMixedValueContentColor();
                        style.Draw(position, s_MixedValueContent, id, false);
                        EndHandleMixedValueContentColor();
                    }
                    break;

                default:
                    if ((type == EventType.MouseDown) && (position.Contains(current.mousePosition) && (current.button == 0)))
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }

        internal static bool ButtonMouseDown(Rect position, GUIContent content, FocusType focusType, GUIStyle style)
        {
            return ButtonMouseDown(GUIUtility.GetControlID(s_ButtonMouseDownHash, focusType, position), position, content, style);
        }

        internal static bool ButtonWithDropdownList(string buttonName, string[] buttonNames, GenericMenu.MenuFunction2 callback, params GUILayoutOption[] options)
        {
            return ButtonWithDropdownList(EditorGUIUtility.TempContent(buttonName), buttonNames, callback, options);
        }

        internal static bool ButtonWithDropdownList(GUIContent content, string[] buttonNames, GenericMenu.MenuFunction2 callback, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(content, EditorStyles.dropDownList, options);
            Rect rect2 = position;
            rect2.xMin = rect2.xMax - 20f;
            if ((Event.current.type != EventType.MouseDown) || !rect2.Contains(Event.current.mousePosition))
            {
                return GUI.Button(position, content, EditorStyles.dropDownList);
            }
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i != buttonNames.Length; i++)
            {
                menu.AddItem(new GUIContent(buttonNames[i]), false, callback, i);
            }
            menu.DropDown(position);
            Event.current.Use();
            return false;
        }

        private static double CalculateFloatDragSensitivity(double value)
        {
            if (!double.IsInfinity(value) && !double.IsNaN(value))
            {
                return (Math.Max(1.0, Math.Pow(Math.Abs(value), 0.5)) * 0.029999999329447746);
            }
            return 0.0;
        }

        private static long CalculateIntDragSensitivity(long value)
        {
            return (long) Math.Max((double) 1.0, (double) (Math.Pow(Math.Abs((double) value), 0.5) * 0.029999999329447746));
        }

        internal static void ClearStacks()
        {
            s_EnabledStack.Clear();
            s_ChangedStack.Clear();
            s_PropertyStack.Clear();
            ScriptAttributeUtility.s_DrawerStack.Clear();
        }

        internal static Color ColorBrightnessField(Rect r, GUIContent label, Color value, float minBrightness, float maxBrightness)
        {
            return ColorBrightnessFieldInternal(r, label, value, minBrightness, maxBrightness, EditorStyles.numberField);
        }

        internal static Color ColorBrightnessFieldInternal(Rect position, GUIContent label, Color value, float minBrightness, float maxBrightness, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            return DoColorBrightnessField(rect, position, value, minBrightness, maxBrightness, style);
        }

        public static Color ColorField(Rect position, Color value)
        {
            int id = GUIUtility.GetControlID(s_ColorHash, EditorGUIUtility.native, position);
            return DoColorField(IndentedRect(position), id, value, true, true, false, null);
        }

        public static Color ColorField(Rect position, string label, Color value)
        {
            return ColorField(position, EditorGUIUtility.TempContent(label), value);
        }

        public static Color ColorField(Rect position, GUIContent label, Color value)
        {
            int id = GUIUtility.GetControlID(s_ColorHash, EditorGUIUtility.native, position);
            return DoColorField(PrefixLabel(position, id, label), id, value, true, true, false, null);
        }

        internal static Color ColorField(Rect position, Color value, bool showEyedropper, bool showAlpha)
        {
            int id = GUIUtility.GetControlID(s_ColorHash, EditorGUIUtility.native, position);
            return DoColorField(position, id, value, showEyedropper, showAlpha, false, null);
        }

        internal static Color ColorField(Rect position, GUIContent label, Color value, bool showEyedropper, bool showAlpha)
        {
            int id = GUIUtility.GetControlID(s_ColorHash, EditorGUIUtility.native, position);
            return DoColorField(PrefixLabel(position, id, label), id, value, showEyedropper, showAlpha, false, null);
        }

        public static Color ColorField(Rect position, GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
        {
            int id = GUIUtility.GetControlID(s_ColorHash, EditorGUIUtility.native, position);
            return DoColorField(PrefixLabel(position, id, label), id, value, showEyedropper, showAlpha, hdr, hdrConfig);
        }

        internal static Color ColorSelector(Rect activatorRect, Rect renderRect, int id, Color value)
        {
            Event current = Event.current;
            Color color = value;
            value = !showMixedValue ? value : Color.white;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(id))
                    {
                        current.Use();
                        showMixedValue = false;
                        ColorPicker.Show(GUIView.current, value, false, false, null);
                        GUIUtility.ExitGUI();
                    }
                    return color;

                case EventType.Repaint:
                    if ((renderRect.height > 0f) && (renderRect.width > 0f))
                    {
                        DrawRect(renderRect, value);
                    }
                    return color;

                case EventType.ValidateCommand:
                    if (((current.commandName == "UndoRedoPerformed") && (GUIUtility.keyboardControl == id)) && ColorPicker.visible)
                    {
                        ColorPicker.color = value;
                    }
                    return color;

                case EventType.ExecuteCommand:
                {
                    int num;
                    if (GUIUtility.keyboardControl != id)
                    {
                        return color;
                    }
                    string commandName = current.commandName;
                    if (commandName == null)
                    {
                        return color;
                    }
                    if (<>f__switch$map4 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                        dictionary.Add("ColorPickerChanged", 0);
                        <>f__switch$map4 = dictionary;
                    }
                    if (!<>f__switch$map4.TryGetValue(commandName, out num))
                    {
                        return color;
                    }
                    if (num != 0)
                    {
                        return color;
                    }
                    current.Use();
                    GUI.changed = true;
                    HandleUtility.Repaint();
                    return ColorPicker.color;
                }
            }
            if ((typeForControl == EventType.MouseDown) && activatorRect.Contains(current.mousePosition))
            {
                current.Use();
                GUIUtility.keyboardControl = id;
                showMixedValue = false;
                ColorPicker.Show(GUIView.current, value, false, false, null);
                GUIUtility.ExitGUI();
            }
            return color;
        }

        public static AnimationCurve CurveField(Rect position, AnimationCurve value)
        {
            int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
            return DoCurveField(IndentedRect(position), id, value, kCurveColor, new Rect(), null);
        }

        public static AnimationCurve CurveField(Rect position, string label, AnimationCurve value)
        {
            return CurveField(position, EditorGUIUtility.TempContent(label), value);
        }

        public static AnimationCurve CurveField(Rect position, GUIContent label, AnimationCurve value)
        {
            int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
            return DoCurveField(PrefixLabel(position, id, label), id, value, kCurveColor, new Rect(), null);
        }

        public static void CurveField(Rect position, SerializedProperty value, Color color, Rect ranges)
        {
            int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
            DoCurveField(position, id, null, color, ranges, value);
        }

        public static AnimationCurve CurveField(Rect position, AnimationCurve value, Color color, Rect ranges)
        {
            int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
            return DoCurveField(IndentedRect(position), id, value, color, ranges, null);
        }

        public static AnimationCurve CurveField(Rect position, string label, AnimationCurve value, Color color, Rect ranges)
        {
            return CurveField(position, EditorGUIUtility.TempContent(label), value, color, ranges);
        }

        public static AnimationCurve CurveField(Rect position, GUIContent label, AnimationCurve value, Color color, Rect ranges)
        {
            int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
            return DoCurveField(PrefixLabel(position, id, label), id, value, color, ranges, null);
        }

        internal static int CycleButton(Rect position, int selected, GUIContent[] options, GUIStyle style)
        {
            if ((selected >= options.Length) || (selected < 0))
            {
                selected = 0;
                GUI.changed = true;
            }
            if (GUI.Button(position, options[selected], style))
            {
                selected++;
                GUI.changed = true;
                if (selected >= options.Length)
                {
                    selected = 0;
                }
            }
            return selected;
        }

        internal static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            SerializedPropertyType propertyType = property.propertyType;
            bool foldout = false;
            if (HasVisibleChildFields(property))
            {
                Event event2 = new Event(Event.current);
                foldout = property.isExpanded;
                BeginDisabledGroup(!property.editable);
                GUIStyle style = (DragAndDrop.activeControlID != -10) ? EditorStyles.foldout : EditorStyles.foldoutPreDrop;
                bool expanded = Foldout(position, foldout, s_PropertyFieldTempContent, true, style);
                EndDisabledGroup();
                if (expanded != foldout)
                {
                    if (Event.current.alt)
                    {
                        SetExpandedRecurse(property, expanded);
                    }
                    else
                    {
                        property.isExpanded = expanded;
                    }
                }
                foldout = expanded;
                int num7 = EditorGUIUtility.s_LastControlID;
                EventType type = event2.type;
                switch (type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (position.Contains(event2.mousePosition) && GUI.enabled)
                        {
                            Object[] objectReferences = DragAndDrop.objectReferences;
                            Object[] references = new Object[1];
                            bool flag6 = false;
                            foreach (Object obj2 in objectReferences)
                            {
                                references[0] = obj2;
                                Object obj3 = ValidateObjectFieldAssignment(references, null, property);
                                if (obj3 != null)
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (event2.type == EventType.DragPerform)
                                    {
                                        property.AppendFoldoutPPtrValue(obj3);
                                        flag6 = true;
                                        DragAndDrop.activeControlID = 0;
                                    }
                                    else
                                    {
                                        DragAndDrop.activeControlID = num7;
                                    }
                                }
                            }
                            if (flag6)
                            {
                                GUI.changed = true;
                                DragAndDrop.AcceptDrag();
                            }
                        }
                        goto Label_0484;
                }
                if ((type == EventType.DragExited) && GUI.enabled)
                {
                    HandleUtility.Repaint();
                }
            }
            else
            {
                bool changed;
                switch (propertyType)
                {
                    case SerializedPropertyType.Integer:
                    {
                        BeginChangeCheck();
                        long num = LongField(position, label, property.longValue);
                        if (EndChangeCheck())
                        {
                            property.longValue = num;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.Boolean:
                    {
                        BeginChangeCheck();
                        bool flag3 = Toggle(position, label, property.boolValue);
                        if (EndChangeCheck())
                        {
                            property.boolValue = flag3;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.Float:
                    {
                        BeginChangeCheck();
                        double num2 = !(property.type == "float") ? DoubleField(position, label, property.doubleValue) : ((double) FloatField(position, label, property.floatValue));
                        if (EndChangeCheck())
                        {
                            property.doubleValue = num2;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.String:
                    {
                        BeginChangeCheck();
                        string str = TextField(position, label, property.stringValue);
                        if (EndChangeCheck())
                        {
                            property.stringValue = str;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.Color:
                    {
                        BeginChangeCheck();
                        Color color = ColorField(position, label, property.colorValue);
                        if (EndChangeCheck())
                        {
                            property.colorValue = color;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.ObjectReference:
                        ObjectFieldInternal(position, property, null, label, EditorStyles.objectField);
                        goto Label_0484;

                    case SerializedPropertyType.LayerMask:
                        LayerMaskField(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Enum:
                        Popup(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Vector2:
                        Vector2Field(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Vector3:
                        Vector3Field(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Vector4:
                        Vector4Field(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Rect:
                        RectField(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.ArraySize:
                    {
                        BeginChangeCheck();
                        int num3 = ArraySizeField(position, label, property.intValue, EditorStyles.numberField);
                        if (EndChangeCheck())
                        {
                            property.intValue = num3;
                        }
                        goto Label_0484;
                    }
                    case SerializedPropertyType.Character:
                    {
                        char[] chArray = new char[] { (char) property.intValue };
                        changed = GUI.changed;
                        GUI.changed = false;
                        string str2 = TextField(position, label, new string(chArray));
                        if (GUI.changed)
                        {
                            if (str2.Length != 1)
                            {
                                GUI.changed = false;
                                break;
                            }
                            property.intValue = str2[0];
                        }
                        break;
                    }
                    case SerializedPropertyType.AnimationCurve:
                    {
                        int id = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
                        DoCurveField(PrefixLabel(position, id, label), id, null, kCurveColor, new Rect(), property);
                        goto Label_0484;
                    }
                    case SerializedPropertyType.Bounds:
                        BoundsField(position, property, label);
                        goto Label_0484;

                    case SerializedPropertyType.Gradient:
                    {
                        int num5 = GUIUtility.GetControlID(s_CurveHash, EditorGUIUtility.native, position);
                        DoGradientField(PrefixLabel(position, num5, label), num5, null, property);
                        goto Label_0484;
                    }
                    default:
                    {
                        int num6 = GUIUtility.GetControlID(s_GenericField, FocusType.Keyboard, position);
                        PrefixLabel(position, num6, label);
                        goto Label_0484;
                    }
                }
                GUI.changed |= changed;
            }
        Label_0484:
            EndProperty();
            return foldout;
        }

        [ExcludeFromDocs]
        public static float DelayedFloatField(Rect position, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedFloatField(position, value, numberField);
        }

        [ExcludeFromDocs]
        public static void DelayedFloatField(Rect position, SerializedProperty property)
        {
            GUIContent label = null;
            DelayedFloatField(position, property, label);
        }

        public static float DelayedFloatField(Rect position, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedFloatField(position, GUIContent.none, value, style);
        }

        [ExcludeFromDocs]
        public static float DelayedFloatField(Rect position, string label, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedFloatField(position, label, value, numberField);
        }

        public static void DelayedFloatField(Rect position, SerializedProperty property, [DefaultValue("null")] GUIContent label)
        {
            DelayedFloatFieldInternal(position, property, label);
        }

        [ExcludeFromDocs]
        public static float DelayedFloatField(Rect position, GUIContent label, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedFloatField(position, label, value, numberField);
        }

        public static float DelayedFloatField(Rect position, string label, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedFloatField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static float DelayedFloatField(Rect position, GUIContent label, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedFloatFieldInternal(position, label, value, style);
        }

        internal static void DelayedFloatFieldInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            float num = DelayedFloatFieldInternal(position, label, property.floatValue, EditorStyles.numberField);
            if (EndChangeCheck())
            {
                property.floatValue = num;
            }
            EndProperty();
        }

        internal static float DelayedFloatFieldInternal(Rect position, GUIContent label, float value, GUIStyle style)
        {
            float num = value;
            float result = num;
            BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_DelayedTextFieldHash, FocusType.Keyboard, position);
            string s = DelayedTextFieldInternal(position, id, label, num.ToString(), s_AllowedCharactersForFloat, style);
            if ((EndChangeCheck() && float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result)) && (result != num))
            {
                value = result;
                GUI.changed = true;
            }
            return result;
        }

        [ExcludeFromDocs]
        public static int DelayedIntField(Rect position, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedIntField(position, value, numberField);
        }

        [ExcludeFromDocs]
        public static void DelayedIntField(Rect position, SerializedProperty property)
        {
            GUIContent label = null;
            DelayedIntField(position, property, label);
        }

        public static int DelayedIntField(Rect position, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedIntField(position, GUIContent.none, value, style);
        }

        [ExcludeFromDocs]
        public static int DelayedIntField(Rect position, string label, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedIntField(position, label, value, numberField);
        }

        public static void DelayedIntField(Rect position, SerializedProperty property, [DefaultValue("null")] GUIContent label)
        {
            DelayedIntFieldInternal(position, property, label);
        }

        [ExcludeFromDocs]
        public static int DelayedIntField(Rect position, GUIContent label, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DelayedIntField(position, label, value, numberField);
        }

        public static int DelayedIntField(Rect position, string label, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedIntField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static int DelayedIntField(Rect position, GUIContent label, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DelayedIntFieldInternal(position, label, value, style);
        }

        internal static void DelayedIntFieldInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            int num = DelayedIntFieldInternal(position, label, property.intValue, EditorStyles.numberField);
            if (EndChangeCheck())
            {
                property.intValue = num;
            }
            EndProperty();
        }

        internal static int DelayedIntFieldInternal(Rect position, GUIContent label, int value, GUIStyle style)
        {
            int num = value;
            int result = num;
            BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_DelayedTextFieldHash, FocusType.Keyboard, position);
            string s = DelayedTextFieldInternal(position, id, label, num.ToString(), s_AllowedCharactersForInt, style);
            if ((EndChangeCheck() && int.TryParse(s, out result)) && (result != num))
            {
                value = result;
                GUI.changed = true;
            }
            return result;
        }

        [ExcludeFromDocs]
        public static string DelayedTextField(Rect position, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return DelayedTextField(position, text, textField);
        }

        [ExcludeFromDocs]
        public static void DelayedTextField(Rect position, SerializedProperty property)
        {
            GUIContent label = null;
            DelayedTextField(position, property, label);
        }

        [ExcludeFromDocs]
        public static string DelayedTextField(Rect position, string label, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return DelayedTextField(position, label, text, textField);
        }

        public static string DelayedTextField(Rect position, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return DelayedTextField(position, GUIContent.none, text, style);
        }

        public static void DelayedTextField(Rect position, SerializedProperty property, [DefaultValue("null")] GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, position);
            DelayedTextFieldInternal(position, id, property, null, label);
        }

        [ExcludeFromDocs]
        public static string DelayedTextField(Rect position, GUIContent label, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return DelayedTextField(position, label, text, textField);
        }

        public static string DelayedTextField(Rect position, string label, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return DelayedTextField(position, EditorGUIUtility.TempContent(label), text, style);
        }

        public static string DelayedTextField(Rect position, GUIContent label, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, position);
            return DelayedTextFieldInternal(position, id, label, text, null, style);
        }

        internal static string DelayedTextFieldDropDown(Rect position, string text, string[] dropDownElement)
        {
            return DelayedTextFieldDropDown(position, GUIContent.none, text, dropDownElement);
        }

        internal static string DelayedTextFieldDropDown(Rect position, GUIContent label, string text, string[] dropDownElement)
        {
            int id = GUIUtility.GetControlID(s_TextFieldDropDownHash, FocusType.Keyboard, position);
            return DoTextFieldDropDown(PrefixLabel(position, id, label), id, text, dropDownElement, true);
        }

        internal static string DelayedTextFieldInternal(Rect position, string value, string allowedLetters, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_DelayedTextFieldHash, FocusType.Keyboard, position);
            return DelayedTextFieldInternal(position, id, GUIContent.none, value, allowedLetters, style);
        }

        internal static void DelayedTextFieldInternal(Rect position, int id, SerializedProperty property, string allowedLetters, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            string str = DelayedTextFieldInternal(position, id, label, property.stringValue, allowedLetters, EditorStyles.textField);
            if (EndChangeCheck())
            {
                property.stringValue = str;
            }
            EndProperty();
        }

        internal static string DelayedTextFieldInternal(Rect position, int id, GUIContent label, string value, string allowedLetters, GUIStyle style)
        {
            string str;
            bool flag;
            if (HasKeyboardFocus(id))
            {
                if (!s_DelayedTextEditor.IsEditingControl(id))
                {
                    str = s_RecycledCurrentEditingString = value;
                }
                else
                {
                    str = s_RecycledCurrentEditingString;
                }
                Event current = Event.current;
                if ((current.type == EventType.ValidateCommand) && (current.commandName == "UndoRedoPerformed"))
                {
                    str = value;
                }
            }
            else
            {
                str = value;
            }
            bool changed = GUI.changed;
            str = s_DelayedTextEditor.OnGUI(id, str, out flag);
            GUI.changed = false;
            if (!flag)
            {
                str = DoTextField(s_DelayedTextEditor, id, PrefixLabel(position, id, label), str, style, allowedLetters, out flag, false, false, false);
                GUI.changed = false;
                if (GUIUtility.keyboardControl == id)
                {
                    if (!s_DelayedTextEditor.IsEditingControl(id))
                    {
                        if (value != str)
                        {
                            GUI.changed = true;
                            value = str;
                        }
                    }
                    else
                    {
                        s_RecycledCurrentEditingString = str;
                    }
                }
            }
            else
            {
                GUI.changed = true;
                value = str;
            }
            GUI.changed |= changed;
            return value;
        }

        internal static Color DoColorBrightnessField(Rect rect, Rect dragRect, Color col, float minBrightness, float maxBrightness, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(0x1218b72, FocusType.Keyboard);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (((current.button == 0) && dragRect.Contains(Event.current.mousePosition)) && (GUIUtility.hotControl == 0))
                    {
                        ColorBrightnessFieldStateObject stateObject = GUIUtility.GetStateObject(typeof(ColorBrightnessFieldStateObject), controlID) as ColorBrightnessFieldStateObject;
                        if (stateObject != null)
                        {
                            Color.RGBToHSV(col, out stateObject.m_Hue, out stateObject.m_Saturation, out stateObject.m_Brightness);
                        }
                        GUIUtility.keyboardControl = 0;
                        GUIUtility.hotControl = controlID;
                        GUI.changed = true;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        ColorBrightnessFieldStateObject obj3 = GUIUtility.QueryStateObject(typeof(ColorBrightnessFieldStateObject), controlID) as ColorBrightnessFieldStateObject;
                        float maxColorComponent = col.maxColorComponent;
                        float num4 = Mathf.Clamp01(Mathf.Max(1f, Mathf.Pow(Mathf.Abs(maxColorComponent), 0.5f)) * 0.004f);
                        float num5 = HandleUtility.niceMouseDelta * num4;
                        float num6 = Mathf.Clamp(obj3.m_Brightness + num5, minBrightness, maxBrightness);
                        obj3.m_Brightness = (float) Math.Round((double) num6, 3);
                        col = Color.HSVToRGB(obj3.m_Hue, obj3.m_Saturation, obj3.m_Brightness, maxBrightness > 1f);
                        GUIUtility.keyboardControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    if (GUIUtility.hotControl == 0)
                    {
                        EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SlideArrow);
                    }
                    break;
            }
            BeginChangeCheck();
            float num7 = DelayedFloatField(rect, col.maxColorComponent, style);
            if (EndChangeCheck())
            {
                float num8;
                float num9;
                float num10;
                Color.RGBToHSV(col, out num8, out num9, out num10);
                float v = Mathf.Clamp(num7, minBrightness, maxBrightness);
                col = Color.HSVToRGB(num8, num9, v, maxBrightness > 1f);
            }
            return col;
        }

        private static Color DoColorField(Rect position, int id, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
        {
            Event current = Event.current;
            GUIStyle colorField = EditorStyles.colorField;
            Color color = value;
            value = !showMixedValue ? value : Color.white;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(id))
                    {
                        Event.current.Use();
                        showMixedValue = false;
                        ColorPicker.Show(GUIView.current, value, showAlpha, hdr, hdrConfig);
                        GUIUtility.ExitGUI();
                    }
                    return color;

                case EventType.Repaint:
                    Rect rect;
                    if (!showEyedropper)
                    {
                        rect = position;
                    }
                    else
                    {
                        rect = colorField.padding.Remove(position);
                    }
                    if (showEyedropper && (s_ColorPickID == id))
                    {
                        Color pickedColor = EyeDropper.GetPickedColor();
                        pickedColor.a = value.a;
                        EditorGUIUtility.DrawColorSwatch(rect, pickedColor, showAlpha, hdr);
                    }
                    else
                    {
                        EditorGUIUtility.DrawColorSwatch(rect, value, showAlpha, hdr);
                    }
                    if (showEyedropper)
                    {
                        colorField.Draw(position, GUIContent.none, id);
                        return color;
                    }
                    EditorStyles.colorPickerBox.Draw(position, GUIContent.none, id);
                    return color;

                default:
                    int num;
                    string commandName;
                    Dictionary<string, int> dictionary;
                    switch (typeForControl)
                    {
                        case EventType.ValidateCommand:
                            commandName = current.commandName;
                            if (commandName != null)
                            {
                                if (<>f__switch$map2 == null)
                                {
                                    dictionary = new Dictionary<string, int>(3);
                                    dictionary.Add("UndoRedoPerformed", 0);
                                    dictionary.Add("Copy", 1);
                                    dictionary.Add("Paste", 1);
                                    <>f__switch$map2 = dictionary;
                                }
                                if (!<>f__switch$map2.TryGetValue(commandName, out num))
                                {
                                    return color;
                                }
                                if (num == 0)
                                {
                                    if ((GUIUtility.keyboardControl == id) && ColorPicker.visible)
                                    {
                                        ColorPicker.color = value;
                                    }
                                    return color;
                                }
                                if (num == 1)
                                {
                                    current.Use();
                                    return color;
                                }
                            }
                            return color;

                        case EventType.ExecuteCommand:
                            if (GUIUtility.keyboardControl == id)
                            {
                                commandName = current.commandName;
                                if (commandName == null)
                                {
                                    return color;
                                }
                                if (<>f__switch$map3 == null)
                                {
                                    dictionary = new Dictionary<string, int>(6);
                                    dictionary.Add("EyeDropperUpdate", 0);
                                    dictionary.Add("EyeDropperClicked", 1);
                                    dictionary.Add("EyeDropperCancelled", 2);
                                    dictionary.Add("ColorPickerChanged", 3);
                                    dictionary.Add("Copy", 4);
                                    dictionary.Add("Paste", 5);
                                    <>f__switch$map3 = dictionary;
                                }
                                if (!<>f__switch$map3.TryGetValue(commandName, out num))
                                {
                                    return color;
                                }
                                switch (num)
                                {
                                    case 0:
                                        HandleUtility.Repaint();
                                        return color;

                                    case 1:
                                    {
                                        GUI.changed = true;
                                        HandleUtility.Repaint();
                                        Color lastPickedColor = EyeDropper.GetLastPickedColor();
                                        lastPickedColor.a = value.a;
                                        s_ColorPickID = 0;
                                        return lastPickedColor;
                                    }
                                    case 2:
                                        HandleUtility.Repaint();
                                        s_ColorPickID = 0;
                                        return color;

                                    case 3:
                                        GUI.changed = true;
                                        HandleUtility.Repaint();
                                        return ColorPicker.color;

                                    case 4:
                                        ColorClipboard.SetColor(value);
                                        current.Use();
                                        return color;

                                    case 5:
                                        Color color4;
                                        if (ColorClipboard.TryGetColor(hdr, out color4))
                                        {
                                            if (!showAlpha)
                                            {
                                                color4.a = color.a;
                                            }
                                            color = color4;
                                            GUI.changed = true;
                                            current.Use();
                                        }
                                        return color;
                                }
                            }
                            return color;

                        default:
                            if (typeForControl != EventType.MouseDown)
                            {
                                return color;
                            }
                            if (showEyedropper)
                            {
                                position.width -= 20f;
                            }
                            if (position.Contains(current.mousePosition))
                            {
                                switch (current.button)
                                {
                                    case 0:
                                        GUIUtility.keyboardControl = id;
                                        showMixedValue = false;
                                        ColorPicker.Show(GUIView.current, value, showAlpha, hdr, hdrConfig);
                                        GUIUtility.ExitGUI();
                                        goto Label_012E;

                                    case 1:
                                    {
                                        GUIUtility.keyboardControl = id;
                                        string[] options = new string[] { "Copy", "Paste" };
                                        bool[] enabled = new bool[] { true, ColorClipboard.HasColor() };
                                        if (<>f__am$cache64 == null)
                                        {
                                            <>f__am$cache64 = delegate (object data, string[] options, int selected) {
                                                if (selected == 0)
                                                {
                                                    Event e = EditorGUIUtility.CommandEvent("Copy");
                                                    GUIView.current.SendEvent(e);
                                                }
                                                else if (selected == 1)
                                                {
                                                    Event event3 = EditorGUIUtility.CommandEvent("Paste");
                                                    GUIView.current.SendEvent(event3);
                                                }
                                            };
                                        }
                                        EditorUtility.DisplayCustomMenu(position, options, enabled, null, <>f__am$cache64, null);
                                        return color;
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        Label_012E:
            if (showEyedropper)
            {
                position.width += 20f;
                if (position.Contains(current.mousePosition))
                {
                    GUIUtility.keyboardControl = id;
                    EyeDropper.Start(GUIView.current);
                    s_ColorPickID = id;
                    GUIUtility.ExitGUI();
                }
            }
            return color;
        }

        private static AnimationCurve DoCurveField(Rect position, int id, AnimationCurve value, Color color, Rect ranges, SerializedProperty property)
        {
            Event current = Event.current;
            position.width = Mathf.Max(position.width, 2f);
            position.height = Mathf.Max(position.height, 2f);
            if ((GUIUtility.keyboardControl == id) && (Event.current.type != EventType.Layout))
            {
                if (s_CurveID != id)
                {
                    s_CurveID = id;
                    if (CurveEditorWindow.visible)
                    {
                        SetCurveEditorWindowCurve(value, property, color);
                        ShowCurvePopup(GUIView.current, ranges);
                    }
                }
                else if (CurveEditorWindow.visible && (Event.current.type == EventType.Repaint))
                {
                    SetCurveEditorWindowCurve(value, property, color);
                    CurveEditorWindow.instance.Repaint();
                }
            }
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(id))
                    {
                        s_CurveID = id;
                        SetCurveEditorWindowCurve(value, property, color);
                        ShowCurvePopup(GUIView.current, ranges);
                        current.Use();
                        GUIUtility.ExitGUI();
                    }
                    return value;

                case EventType.Repaint:
                {
                    Rect rect = position;
                    rect.y++;
                    rect.height--;
                    if (!(ranges != new Rect()))
                    {
                        EditorGUIUtility.DrawCurveSwatch(rect, value, property, color, kCurveBGColor);
                    }
                    else
                    {
                        EditorGUIUtility.DrawCurveSwatch(rect, value, property, color, kCurveBGColor, ranges);
                    }
                    EditorStyles.colorPickerBox.Draw(rect, GUIContent.none, id, false);
                    return value;
                }
            }
            if (typeForControl != EventType.MouseDown)
            {
                int num;
                if (typeForControl != EventType.ExecuteCommand)
                {
                    return value;
                }
                if (s_CurveID != id)
                {
                    return value;
                }
                string commandName = current.commandName;
                if (commandName == null)
                {
                    return value;
                }
                if (<>f__switch$map5 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                    dictionary.Add("CurveChanged", 0);
                    <>f__switch$map5 = dictionary;
                }
                if (!<>f__switch$map5.TryGetValue(commandName, out num))
                {
                    return value;
                }
                if (num != 0)
                {
                    return value;
                }
                GUI.changed = true;
                AnimationCurvePreviewCache.ClearCache();
                HandleUtility.Repaint();
                if (property != null)
                {
                    property.animationCurveValue = CurveEditorWindow.curve;
                    if (property.hasMultipleDifferentValues)
                    {
                        Debug.LogError("AnimationCurve SerializedProperty hasMultipleDifferentValues is true after writing.");
                    }
                }
                return CurveEditorWindow.curve;
            }
            if (position.Contains(current.mousePosition))
            {
                s_CurveID = id;
                GUIUtility.keyboardControl = id;
                SetCurveEditorWindowCurve(value, property, color);
                ShowCurvePopup(GUIView.current, ranges);
                current.Use();
                GUIUtility.ExitGUI();
            }
            return value;
        }

        internal static double DoDoubleField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, double value, string formatString, GUIStyle style, bool draggable)
        {
            return DoDoubleField(editor, position, dragHotZone, id, value, formatString, style, draggable, (Event.current.GetTypeForControl(id) != EventType.MouseDown) ? 0.0 : CalculateFloatDragSensitivity(s_DragStartValue));
        }

        internal static double DoDoubleField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, double value, string formatString, GUIStyle style, bool draggable, double dragSensitivity)
        {
            long longVal = 0L;
            DoNumberField(editor, position, dragHotZone, id, true, ref value, ref longVal, formatString, style, draggable, dragSensitivity);
            return value;
        }

        internal static Object DoDropField(Rect position, int id, Type objType, ObjectFieldValidator validator, bool allowSceneObjects, GUIStyle style)
        {
            if (validator == null)
            {
                validator = new ObjectFieldValidator(EditorGUI.ValidateObjectFieldAssignment);
            }
            EventType rawType = Event.current.type;
            if ((!GUI.enabled && GUIClip.enabled) && (Event.current.rawType == EventType.MouseDown))
            {
                rawType = Event.current.rawType;
            }
            EventType type2 = rawType;
            switch (type2)
            {
                case EventType.Repaint:
                    style.Draw(position, GUIContent.none, id, DragAndDrop.activeControlID == id);
                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (position.Contains(Event.current.mousePosition) && GUI.enabled)
                    {
                        Object[] objectReferences = DragAndDrop.objectReferences;
                        Object target = validator(objectReferences, objType, null);
                        if (((target != null) && !allowSceneObjects) && !EditorUtility.IsPersistent(target))
                        {
                            target = null;
                        }
                        if (target != null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                            if (rawType == EventType.DragPerform)
                            {
                                GUI.changed = true;
                                DragAndDrop.AcceptDrag();
                                DragAndDrop.activeControlID = 0;
                                Event.current.Use();
                                return target;
                            }
                            DragAndDrop.activeControlID = id;
                            Event.current.Use();
                        }
                    }
                    break;

                default:
                    if ((type2 == EventType.DragExited) && GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }
                    break;
            }
            return null;
        }

        internal static void DoDropShadowLabel(Rect position, GUIContent content, GUIStyle style, float shadowOpa)
        {
            if (Event.current.type == EventType.Repaint)
            {
                DrawLabelShadow(position, content, style, shadowOpa);
                style.Draw(position, content, false, false, false, false);
            }
        }

        internal static float DoFloatField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, float value, string formatString, GUIStyle style, bool draggable)
        {
            return DoFloatField(editor, position, dragHotZone, id, value, formatString, style, draggable, (Event.current.GetTypeForControl(id) != EventType.MouseDown) ? 0f : ((float) CalculateFloatDragSensitivity(s_DragStartValue)));
        }

        internal static float DoFloatField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, float value, string formatString, GUIStyle style, bool draggable, float dragSensitivity)
        {
            long longVal = 0L;
            double doubleVal = value;
            DoNumberField(editor, position, dragHotZone, id, true, ref doubleVal, ref longVal, formatString, style, draggable, (double) dragSensitivity);
            return MathUtils.ClampToFloat(doubleVal);
        }

        internal static Gradient DoGradientField(Rect position, int id, Gradient value, SerializedProperty property)
        {
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.KeyDown:
                    if ((GUIUtility.keyboardControl == id) && (((current.keyCode == KeyCode.Space) || (current.keyCode == KeyCode.Return)) || (current.keyCode == KeyCode.KeypadEnter)))
                    {
                        Event.current.Use();
                        Gradient newGradient = (property == null) ? value : property.gradientValue;
                        GradientPicker.Show(newGradient);
                        GUIUtility.ExitGUI();
                    }
                    return value;

                case EventType.Repaint:
                {
                    Rect rect = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
                    if (property == null)
                    {
                        GradientEditor.DrawGradientSwatch(rect, value, Color.white);
                    }
                    else
                    {
                        GradientEditor.DrawGradientSwatch(rect, property, Color.white);
                    }
                    break;
                }
                default:
                    switch (typeForControl)
                    {
                        case EventType.ValidateCommand:
                            if ((s_GradientID == id) && (current.commandName == "UndoRedoPerformed"))
                            {
                                if (property != null)
                                {
                                    GradientPicker.SetCurrentGradient(property.gradientValue);
                                }
                                GradientPreviewCache.ClearCache();
                            }
                            return value;

                        case EventType.ExecuteCommand:
                            if ((s_GradientID != id) || !(current.commandName == "GradientPickerChanged"))
                            {
                                return value;
                            }
                            GUI.changed = true;
                            GradientPreviewCache.ClearCache();
                            HandleUtility.Repaint();
                            if (property != null)
                            {
                                property.gradientValue = GradientPicker.gradient;
                            }
                            return GradientPicker.gradient;

                        default:
                            if (typeForControl == EventType.MouseDown)
                            {
                                if (!position.Contains(current.mousePosition))
                                {
                                    return value;
                                }
                                if (current.button == 0)
                                {
                                    s_GradientID = id;
                                    GUIUtility.keyboardControl = id;
                                    Gradient gradient = (property == null) ? value : property.gradientValue;
                                    GradientPicker.Show(gradient);
                                    GUIUtility.ExitGUI();
                                    return value;
                                }
                                if ((current.button != 1) || (property == null))
                                {
                                    return value;
                                }
                                GradientContextMenu.Show(property);
                            }
                            return value;
                    }
                    break;
            }
            EditorStyles.colorPickerBox.Draw(position, GUIContent.none, id);
            return value;
        }

        internal static Color DoHexColorTextField(Rect rect, Color color, bool showAlpha, GUIStyle style)
        {
            bool flag2;
            bool flag = false;
            if (color.maxColorComponent > 1f)
            {
                color = color.RGBMultiplied((float) (1f / color.maxColorComponent));
                flag = true;
            }
            Rect position = new Rect(rect.x, rect.y, 14f, rect.height);
            rect.xMin += 14f;
            GUI.Label(position, GUIContent.Temp("#"));
            string text = !showAlpha ? ColorUtility.ToHtmlStringRGB(color) : ColorUtility.ToHtmlStringRGBA(color);
            BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, rect);
            string str3 = DoTextField(s_RecycledEditor, id, rect, text, style, "0123456789ABCDEFabcdef", out flag2, false, false, false);
            if (EndChangeCheck())
            {
                Color color2;
                s_RecycledEditor.text = s_RecycledEditor.text.ToUpper();
                if (ColorUtility.TryParseHtmlString("#" + str3, out color2))
                {
                    color = new Color(color2.r, color2.g, color2.b, !showAlpha ? color.a : color2.a);
                }
            }
            if (flag)
            {
                EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
                GUI.Label(new Rect(position.x - 20f, rect.y, 20f, 20f), s_HDRWarning);
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
            return color;
        }

        internal static void DoInspectorTitlebar(Rect position, int id, bool foldout, Object[] targetObjs, GUIStyle baseStyle)
        {
            GUIStyle inspectorTitlebarText = EditorStyles.inspectorTitlebarText;
            Rect rect = new Rect(position.x + baseStyle.padding.left, position.y + baseStyle.padding.top, 16f, 16f);
            Rect rect2 = new Rect(((position.xMax - baseStyle.padding.right) - 2f) - 16f, rect.y, 16f, 16f);
            Rect rect3 = new Rect(((rect.xMax + 2f) + 2f) + 16f, rect.y, 100f, rect.height) {
                xMax = rect2.xMin - 2f
            };
            int num = -1;
            foreach (Object obj2 in targetObjs)
            {
                int objectEnabled = EditorUtility.GetObjectEnabled(obj2);
                if (num == -1)
                {
                    num = objectEnabled;
                }
                else if (num != objectEnabled)
                {
                    num = -2;
                }
            }
            if (num != -1)
            {
                bool flag = AnimationMode.IsPropertyAnimated(targetObjs[0], "m_Enabled");
                bool flag2 = num != 0;
                showMixedValue = num == -2;
                Rect rect4 = rect;
                rect4.x = rect.xMax + 2f;
                BeginChangeCheck();
                Color color = GUI.color;
                if (flag)
                {
                    GUI.color = AnimationMode.animatedPropertyColor;
                }
                int num4 = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
                flag2 = EditorGUIInternal.DoToggleForward(rect4, num4, flag2, GUIContent.none, EditorStyles.toggle);
                if (flag)
                {
                    GUI.color = color;
                }
                if (EndChangeCheck())
                {
                    Undo.RecordObjects(targetObjs, (!flag2 ? "Disable" : "Enable") + " Component" + ((targetObjs.Length <= 1) ? string.Empty : "s"));
                    foreach (Object obj3 in targetObjs)
                    {
                        EditorUtility.SetObjectEnabled(obj3, flag2);
                    }
                }
                showMixedValue = false;
            }
            Rect rect5 = rect2;
            rect5.x -= 18f;
            if (HelpIconButton(rect5, targetObjs[0]))
            {
                rect3.xMax = rect5.xMin - 2f;
            }
            Event current = Event.current;
            Texture2D i = null;
            if (current.type == EventType.Repaint)
            {
                i = AssetPreview.GetMiniThumbnail(targetObjs[0]);
            }
            if ((ButtonMouseDown(rect, EditorGUIUtility.TempContent(i), FocusType.Passive, GUIStyle.none) && (targetObjs[0] is MonoScript)) && IconSelector.ShowAtPosition(targetObjs[0], rect, true))
            {
                GUIUtility.ExitGUI();
            }
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (rect2.Contains(current.mousePosition))
                    {
                        EditorUtility.DisplayObjectContextMenu(rect2, targetObjs, 0);
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    baseStyle.Draw(position, GUIContent.none, id, foldout);
                    position = baseStyle.padding.Remove(position);
                    inspectorTitlebarText.Draw(rect3, EditorGUIUtility.TempContent(ObjectNames.GetInspectorTitle(targetObjs[0])), id, foldout);
                    inspectorTitlebarText.Draw(rect2, GUIContents.titleSettingsIcon, id, foldout);
                    break;
            }
        }

        internal static int DoIntField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, int value, string formatString, GUIStyle style, bool draggable, float dragSensitivity)
        {
            double doubleVal = 0.0;
            long longVal = value;
            DoNumberField(editor, position, dragHotZone, id, false, ref doubleVal, ref longVal, formatString, style, draggable, (double) dragSensitivity);
            return MathUtils.ClampToInt(longVal);
        }

        internal static Event DoKeyEventField(Rect position, Event _event, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(s_KeyEventFieldHash, FocusType.Native, position);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        if (!bKeyEventActive)
                        {
                            bKeyEventActive = true;
                            return _event;
                        }
                        bKeyEventActive = false;
                    }
                    return _event;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                    }
                    return _event;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return _event;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        current.Use();
                    }
                    return _event;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && bKeyEventActive)
                    {
                        if (current.character != '\0')
                        {
                            break;
                        }
                        if (current.alt && (((current.keyCode == KeyCode.AltGr) || (current.keyCode == KeyCode.LeftAlt)) || (current.keyCode == KeyCode.RightAlt)))
                        {
                            return _event;
                        }
                        if (current.control && ((current.keyCode == KeyCode.LeftControl) || (current.keyCode == KeyCode.RightControl)))
                        {
                            return _event;
                        }
                        if (current.command && (((current.keyCode == KeyCode.LeftCommand) || (current.keyCode == KeyCode.RightCommand)) || ((current.keyCode == KeyCode.LeftWindows) || (current.keyCode == KeyCode.RightWindows))))
                        {
                            return _event;
                        }
                        if (!current.shift || (((current.keyCode != KeyCode.LeftShift) && (current.keyCode != KeyCode.RightShift)) && (current.keyCode != KeyCode.None)))
                        {
                            break;
                        }
                    }
                    return _event;

                case EventType.Repaint:
                {
                    if (!bKeyEventActive)
                    {
                        string t = InternalEditorUtility.TextifyEvent(_event);
                        style.Draw(position, EditorGUIUtility.TempContent(t), controlID);
                        return _event;
                    }
                    GUIContent content = EditorGUIUtility.TempContent("[Please press a key]");
                    style.Draw(position, content, controlID);
                    return _event;
                }
                default:
                    return _event;
            }
            bKeyEventActive = false;
            GUI.changed = true;
            GUIUtility.hotControl = 0;
            Event event3 = new Event(current);
            current.Use();
            return event3;
        }

        internal static long DoLongField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, long value, string formatString, GUIStyle style, bool draggable, double dragSensitivity)
        {
            double doubleVal = 0.0;
            DoNumberField(editor, position, dragHotZone, id, false, ref doubleVal, ref value, formatString, style, draggable, dragSensitivity);
            return value;
        }

        private static void DoMinMaxSlider(Rect position, int id, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            float size = maxValue - minValue;
            BeginChangeCheck();
            EditorGUIExt.DoMinMaxSlider(position, id, ref minValue, ref size, minLimit, maxLimit, minLimit, maxLimit, GUI.skin.horizontalSlider, EditorStyles.minMaxHorizontalSliderThumb, true);
            if (EndChangeCheck())
            {
                maxValue = minValue + size;
            }
        }

        internal static void DoNumberField(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, bool isDouble, ref double doubleVal, ref long longVal, string formatString, GUIStyle style, bool draggable, double dragSensitivity)
        {
            bool flag;
            string str2;
            string allowedletters = !isDouble ? s_AllowedCharactersForInt : s_AllowedCharactersForFloat;
            if (draggable)
            {
                DragNumberValue(editor, position, dragHotZone, id, isDouble, ref doubleVal, ref longVal, formatString, style, dragSensitivity);
            }
            Event current = Event.current;
            if (HasKeyboardFocus(id) || (((current.type == EventType.MouseDown) && (current.button == 0)) && position.Contains(current.mousePosition)))
            {
                if (!editor.IsEditingControl(id))
                {
                    str2 = s_RecycledCurrentEditingString = !isDouble ? ((long) longVal).ToString(formatString) : ((double) doubleVal).ToString(formatString);
                }
                else
                {
                    str2 = s_RecycledCurrentEditingString;
                    if ((current.type == EventType.ValidateCommand) && (current.commandName == "UndoRedoPerformed"))
                    {
                        str2 = !isDouble ? ((long) longVal).ToString(formatString) : ((double) doubleVal).ToString(formatString);
                    }
                }
            }
            else
            {
                str2 = !isDouble ? ((long) longVal).ToString(formatString) : ((double) doubleVal).ToString(formatString);
            }
            if (GUIUtility.keyboardControl == id)
            {
                str2 = DoTextField(editor, id, position, str2, style, allowedletters, out flag, false, false, false);
                if (flag)
                {
                    GUI.changed = true;
                    s_RecycledCurrentEditingString = str2;
                    if (isDouble)
                    {
                        switch (str2.ToLower())
                        {
                            case "inf":
                            case "infinity":
                                doubleVal = double.PositiveInfinity;
                                return;

                            case "-inf":
                            case "-infinity":
                                doubleVal = double.NegativeInfinity;
                                return;
                        }
                        str2 = str2.Replace(',', '.');
                        if (!double.TryParse(str2, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out doubleVal))
                        {
                            doubleVal = s_RecycledCurrentEditingFloat = ExpressionEvaluator.Evaluate<double>(str2);
                        }
                        else
                        {
                            if (double.IsNaN(doubleVal))
                            {
                                doubleVal = 0.0;
                            }
                            s_RecycledCurrentEditingFloat = doubleVal;
                        }
                    }
                    else if (!long.TryParse(str2, out longVal))
                    {
                        longVal = s_RecycledCurrentEditingInt = ExpressionEvaluator.Evaluate<long>(str2);
                    }
                    else
                    {
                        s_RecycledCurrentEditingInt = longVal;
                    }
                }
            }
            else
            {
                str2 = DoTextField(editor, id, position, str2, style, allowedletters, out flag, false, false, false);
            }
        }

        internal static Object DoObjectField(Rect position, Rect dropRect, int id, Object obj, Type objType, SerializedProperty property, ObjectFieldValidator validator, bool allowSceneObjects)
        {
            return DoObjectField(position, dropRect, id, obj, objType, property, validator, allowSceneObjects, EditorStyles.objectField);
        }

        internal static Object DoObjectField(Rect position, Rect dropRect, int id, Object obj, Type objType, SerializedProperty property, ObjectFieldValidator validator, bool allowSceneObjects, GUIStyle style)
        {
            Rect rect;
            if (validator == null)
            {
                validator = new ObjectFieldValidator(EditorGUI.ValidateObjectFieldAssignment);
            }
            Event current = Event.current;
            EventType rawType = current.type;
            if ((!GUI.enabled && GUIClip.enabled) && (Event.current.rawType == EventType.MouseDown))
            {
                rawType = Event.current.rawType;
            }
            bool flag = EditorGUIUtility.HasObjectThumbnail(objType);
            ObjectFieldVisualType iconAndText = ObjectFieldVisualType.IconAndText;
            if ((flag && (position.height <= 18f)) && (position.width <= 32f))
            {
                iconAndText = ObjectFieldVisualType.MiniPreivew;
            }
            else if (flag && (position.height > 16f))
            {
                iconAndText = ObjectFieldVisualType.LargePreview;
            }
            Vector2 iconSize = EditorGUIUtility.GetIconSize();
            switch (iconAndText)
            {
                case ObjectFieldVisualType.IconAndText:
                    EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
                    break;

                case ObjectFieldVisualType.LargePreview:
                    EditorGUIUtility.SetIconSize(new Vector2(64f, 64f));
                    break;
            }
            EventType type3 = rawType;
            switch (type3)
            {
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id)
                    {
                        if ((current.keyCode == KeyCode.Backspace) || (current.keyCode == KeyCode.Delete))
                        {
                            if (property != null)
                            {
                                property.objectReferenceValue = null;
                            }
                            else
                            {
                                obj = null;
                            }
                            GUI.changed = true;
                            current.Use();
                        }
                        if (current.MainActionKeyForControl(id))
                        {
                            ObjectSelector.get.Show(obj, objType, property, allowSceneObjects);
                            ObjectSelector.get.objectSelectorID = id;
                            current.Use();
                            GUIUtility.ExitGUI();
                        }
                    }
                    goto Label_060B;

                case EventType.Repaint:
                    GUIContent content;
                    if (!showMixedValue)
                    {
                        if (property != null)
                        {
                            content = EditorGUIUtility.TempContent(property.objectReferenceStringValue, AssetPreview.GetMiniThumbnail(property.objectReferenceValue));
                            obj = property.objectReferenceValue;
                            if (obj != null)
                            {
                                Object[] references = new Object[] { obj };
                                if (validator(references, objType, property) == null)
                                {
                                    content = EditorGUIUtility.TempContent("Type mismatch");
                                }
                            }
                        }
                        else
                        {
                            content = EditorGUIUtility.ObjectContent(obj, objType);
                        }
                    }
                    else
                    {
                        content = s_MixedValueContent;
                    }
                    switch (iconAndText)
                    {
                        case ObjectFieldVisualType.IconAndText:
                            BeginHandleMixedValueContentColor();
                            style.Draw(position, content, id, DragAndDrop.activeControlID == id);
                            EndHandleMixedValueContentColor();
                            break;

                        case ObjectFieldVisualType.LargePreview:
                            DrawObjectFieldLargeThumb(position, id, obj, content);
                            break;

                        case ObjectFieldVisualType.MiniPreivew:
                            DrawObjectFieldMiniThumb(position, id, obj, content);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    goto Label_060B;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropRect.Contains(Event.current.mousePosition) && GUI.enabled)
                    {
                        Object[] objectReferences = DragAndDrop.objectReferences;
                        Object target = validator(objectReferences, objType, property);
                        if (((target != null) && !allowSceneObjects) && !EditorUtility.IsPersistent(target))
                        {
                            target = null;
                        }
                        if (target != null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                            if (rawType != EventType.DragPerform)
                            {
                                DragAndDrop.activeControlID = id;
                            }
                            else
                            {
                                if (property != null)
                                {
                                    property.objectReferenceValue = target;
                                }
                                else
                                {
                                    obj = target;
                                }
                                GUI.changed = true;
                                DragAndDrop.AcceptDrag();
                                DragAndDrop.activeControlID = 0;
                            }
                            Event.current.Use();
                        }
                    }
                    goto Label_060B;

                case EventType.ExecuteCommand:
                {
                    if ((!(current.commandName == "ObjectSelectorUpdated") || (ObjectSelector.get.objectSelectorID != id)) || (GUIUtility.keyboardControl != id))
                    {
                        goto Label_060B;
                    }
                    Object[] objArray2 = new Object[] { ObjectSelector.GetCurrentObject() };
                    Object obj4 = validator(objArray2, objType, property);
                    if (property != null)
                    {
                        property.objectReferenceValue = obj4;
                    }
                    GUI.changed = true;
                    current.Use();
                    return obj4;
                }
                case EventType.DragExited:
                    if (GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }
                    goto Label_060B;
            }
            if ((type3 == EventType.MouseDown) && ((Event.current.button == 0) && position.Contains(Event.current.mousePosition)))
            {
                switch (iconAndText)
                {
                    case ObjectFieldVisualType.IconAndText:
                        rect = new Rect(position.xMax - 15f, position.y, 15f, position.height);
                        goto Label_02CA;

                    case ObjectFieldVisualType.LargePreview:
                        rect = new Rect(position.xMax - 36f, position.yMax - 14f, 36f, 14f);
                        goto Label_02CA;

                    case ObjectFieldVisualType.MiniPreivew:
                        rect = new Rect(position.xMax - 15f, position.y, 15f, position.height);
                        goto Label_02CA;
                }
                throw new ArgumentOutOfRangeException();
            }
            goto Label_060B;
        Label_02CA:
            EditorGUIUtility.editingTextField = false;
            if (rect.Contains(Event.current.mousePosition))
            {
                if (GUI.enabled)
                {
                    GUIUtility.keyboardControl = id;
                    ObjectSelector.get.Show(obj, objType, property, allowSceneObjects);
                    ObjectSelector.get.objectSelectorID = id;
                    current.Use();
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                Object gameObject = (property == null) ? obj : property.objectReferenceValue;
                Component component = gameObject as Component;
                if (component != null)
                {
                    gameObject = component.gameObject;
                }
                if (showMixedValue)
                {
                    gameObject = null;
                }
                if (Event.current.clickCount == 1)
                {
                    GUIUtility.keyboardControl = id;
                    if (gameObject != null)
                    {
                        bool flag2 = current.shift || current.control;
                        if (!flag2)
                        {
                            EditorGUIUtility.PingObject(gameObject);
                        }
                        if (flag2 && (gameObject is Texture))
                        {
                            PopupLocationHelper.PopupLocation[] locationPriorityOrder = new PopupLocationHelper.PopupLocation[3];
                            locationPriorityOrder[0] = PopupLocationHelper.PopupLocation.Left;
                            locationPriorityOrder[2] = PopupLocationHelper.PopupLocation.Right;
                            PopupWindowWithoutFocus.Show(new RectOffset(6, 3, 0, 3).Add(position), new ObjectPreviewPopup(gameObject), locationPriorityOrder);
                        }
                    }
                    current.Use();
                }
                else if (Event.current.clickCount == 2)
                {
                    if (gameObject != null)
                    {
                        AssetDatabase.OpenAsset(gameObject);
                        GUIUtility.ExitGUI();
                    }
                    current.Use();
                }
            }
        Label_060B:
            EditorGUIUtility.SetIconSize(iconSize);
            return obj;
        }

        internal static bool DoObjectFoldout(bool foldout, Rect interactionRect, Rect renderRect, Object[] targetObjs, int id)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (interactionRect.Contains(current.mousePosition))
                    {
                        if ((current.button != 1) || (targetObjs[0] == null))
                        {
                            if ((current.button == 0) && ((Application.platform != RuntimePlatform.OSXEditor) || !current.control))
                            {
                                GUIUtility.hotControl = id;
                                GUIUtility.keyboardControl = id;
                                DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                                stateObject.mouseDownPosition = current.mousePosition;
                                current.Use();
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayObjectContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), targetObjs, 0);
                            current.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        if (interactionRect.Contains(current.mousePosition))
                        {
                            GUI.changed = true;
                            foldout = !foldout;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                        if (delay2.CanStartDrag())
                        {
                            GUIUtility.hotControl = 0;
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = targetObjs;
                            if (targetObjs.Length <= 1)
                            {
                                DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(targetObjs[0]));
                            }
                            else
                            {
                                DragAndDrop.StartDrag("<Multiple>");
                            }
                        }
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id)
                    {
                        if (current.keyCode == KeyCode.LeftArrow)
                        {
                            foldout = false;
                            current.Use();
                        }
                        if (current.keyCode == KeyCode.RightArrow)
                        {
                            foldout = true;
                            current.Use();
                        }
                    }
                    break;

                case EventType.Repaint:
                {
                    bool isHover = GUIUtility.hotControl == id;
                    EditorStyles.foldout.Draw(renderRect, isHover, isHover, foldout, false);
                    break;
                }
                case EventType.DragUpdated:
                    if (s_DragUpdatedOverID != id)
                    {
                        if (interactionRect.Contains(current.mousePosition))
                        {
                            s_DragUpdatedOverID = id;
                            s_FoldoutDestTime = Time.realtimeSinceStartup + 0.7;
                        }
                    }
                    else if (!interactionRect.Contains(current.mousePosition))
                    {
                        s_DragUpdatedOverID = 0;
                    }
                    else if (Time.realtimeSinceStartup > s_FoldoutDestTime)
                    {
                        foldout = true;
                        HandleUtility.Repaint();
                    }
                    if (interactionRect.Contains(current.mousePosition))
                    {
                        DragAndDrop.visualMode = InternalEditorUtility.InspectorWindowDrag(targetObjs, false);
                        Event.current.Use();
                    }
                    break;

                case EventType.DragPerform:
                    if (interactionRect.Contains(current.mousePosition))
                    {
                        DragAndDrop.visualMode = InternalEditorUtility.InspectorWindowDrag(targetObjs, true);
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                    break;

                default:
                    if ((typeForControl == EventType.ContextClick) && (interactionRect.Contains(current.mousePosition) && (targetObjs[0] != null)))
                    {
                        EditorUtility.DisplayObjectContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), targetObjs, 0);
                        current.Use();
                    }
                    break;
            }
            GUI.enabled = enabled;
            return foldout;
        }

        public static string DoPasswordField(int id, Rect position, string password, GUIStyle style)
        {
            bool flag;
            return DoTextField(s_RecycledEditor, id, position, password, style, null, out flag, false, false, true);
        }

        public static string DoPasswordField(int id, Rect position, GUIContent label, string password, GUIStyle style)
        {
            bool flag;
            return DoTextField(s_RecycledEditor, id, PrefixLabel(position, id, label), password, style, null, out flag, false, false, true);
        }

        internal static int DoPopup(Rect position, int controlID, int selected, GUIContent[] popupValues, GUIStyle style)
        {
            selected = PopupCallbackInfo.GetSelectedValueForControl(controlID, selected);
            GUIContent none = null;
            if (none == null)
            {
                if (showMixedValue)
                {
                    none = s_MixedValueContent;
                }
                else if ((selected < 0) || (selected >= popupValues.Length))
                {
                    none = GUIContent.none;
                }
                else
                {
                    none = popupValues[selected];
                }
            }
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(controlID))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (position.y - (selected * 0x10)) - 19f;
                        }
                        PopupCallbackInfo.instance = new PopupCallbackInfo(controlID);
                        EditorUtility.DisplayCustomMenu(position, popupValues, !showMixedValue ? selected : -1, new EditorUtility.SelectMenuItemFunction(PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                        current.Use();
                    }
                    return selected;

                case EventType.Repaint:
                {
                    Font font = style.font;
                    if (((font != null) && EditorGUIUtility.GetBoldDefaultFont()) && (font == EditorStyles.miniFont))
                    {
                        style.font = EditorStyles.miniBoldFont;
                    }
                    BeginHandleMixedValueContentColor();
                    style.Draw(position, none, controlID, false);
                    EndHandleMixedValueContentColor();
                    style.font = font;
                    return selected;
                }
            }
            if ((type == EventType.MouseDown) && ((current.button == 0) && position.Contains(current.mousePosition)))
            {
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    position.y = (position.y - (selected * 0x10)) - 19f;
                }
                PopupCallbackInfo.instance = new PopupCallbackInfo(controlID);
                EditorUtility.DisplayCustomMenu(position, popupValues, !showMixedValue ? selected : -1, new EditorUtility.SelectMenuItemFunction(PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                GUIUtility.keyboardControl = controlID;
                current.Use();
            }
            return selected;
        }

        private static void DoPropertyContextMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();
            SerializedProperty property2 = property.serializedObject.FindProperty(property.propertyPath);
            ScriptAttributeUtility.GetHandler(property).AddMenuItems(property, menu);
            if (property.hasMultipleDifferentValues && !property.hasVisibleChildren)
            {
                TargetChoiceHandler.AddSetToValueOfTargetMenuItems(menu, property2, new TargetChoiceHandler.TargetChoiceMenuFunction(TargetChoiceHandler.SetToValueOfTarget));
            }
            if ((property.serializedObject.targetObjects.Length == 1) && property.isInstantiatedPrefab)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Revert Value to Prefab"), false, new GenericMenu.MenuFunction2(TargetChoiceHandler.SetPrefabOverride), property2);
            }
            if (property.propertyPath.LastIndexOf(']') == (property.propertyPath.Length - 1))
            {
                if (menu.GetItemCount() > 0)
                {
                    menu.AddSeparator(string.Empty);
                }
                menu.AddItem(EditorGUIUtility.TextContent("Duplicate Array Element"), false, new GenericMenu.MenuFunction2(TargetChoiceHandler.DuplicateArrayElement), property2);
                menu.AddItem(EditorGUIUtility.TextContent("Delete Array Element"), false, new GenericMenu.MenuFunction2(TargetChoiceHandler.DeleteArrayElement), property2);
            }
            if (Event.current.shift)
            {
                if (menu.GetItemCount() > 0)
                {
                    menu.AddSeparator(string.Empty);
                }
                if (<>f__am$cache5D == null)
                {
                    <>f__am$cache5D = e => Debug.Log(((SerializedProperty) e).propertyPath);
                }
                menu.AddItem(EditorGUIUtility.TextContent("Print Property Path"), false, <>f__am$cache5D, property2);
            }
            Event.current.Use();
            if (menu.GetItemCount() != 0)
            {
                menu.ShowAsContext();
            }
        }

        private static void DoPropertyFieldKeyboardHandling(SerializedProperty property)
        {
            if ((Event.current.type == EventType.ExecuteCommand) || (Event.current.type == EventType.ValidateCommand))
            {
                if ((GUIUtility.keyboardControl == EditorGUIUtility.s_LastControlID) && ((Event.current.commandName == "Delete") || (Event.current.commandName == "SoftDelete")))
                {
                    if (Event.current.type == EventType.ExecuteCommand)
                    {
                        s_PendingPropertyDelete = property.Copy();
                    }
                    Event.current.Use();
                }
                if ((GUIUtility.keyboardControl == EditorGUIUtility.s_LastControlID) && (Event.current.commandName == "Duplicate"))
                {
                    if (Event.current.type == EventType.ExecuteCommand)
                    {
                        property.DuplicateCommand();
                    }
                    Event.current.Use();
                }
            }
            s_PendingPropertyKeyboardHandling = null;
        }

        private static float DoSlider(Rect position, Rect dragZonePosition, int id, float value, float left, float right, string formatString)
        {
            return DoSlider(position, dragZonePosition, id, value, left, right, formatString, 1f);
        }

        private static float DoSlider(Rect position, Rect dragZonePosition, int id, float value, float left, float right, string formatString, float power)
        {
            float width = position.width;
            if (width >= (65f + EditorGUIUtility.fieldWidth))
            {
                float num2 = (width - 5f) - EditorGUIUtility.fieldWidth;
                BeginChangeCheck();
                int num3 = GUIUtility.GetControlID(s_SliderKnobHash, FocusType.Native, position);
                if ((GUIUtility.keyboardControl == id) && !s_RecycledEditor.IsEditingControl(id))
                {
                    GUIUtility.keyboardControl = num3;
                }
                float start = left;
                float end = right;
                if (power != 1f)
                {
                    start = PowPreserveSign(left, 1f / power);
                    end = PowPreserveSign(right, 1f / power);
                    value = PowPreserveSign(value, 1f / power);
                }
                Rect rect = new Rect(position.x, position.y, num2, position.height);
                value = GUI.Slider(rect, value, 0f, start, end, GUI.skin.horizontalSlider, !showMixedValue ? GUI.skin.horizontalSliderThumb : "SliderMixed", true, num3);
                if (power != 1f)
                {
                    value = PowPreserveSign(value, power);
                }
                if (EditorGUIUtility.sliderLabels.HasLabels())
                {
                    Color color = GUI.color;
                    GUI.color *= new Color(1f, 1f, 1f, 0.5f);
                    Rect rect2 = new Rect(rect.x, rect.y + 10f, rect.width, rect.height);
                    DoTwoLabels(rect2, EditorGUIUtility.sliderLabels.leftLabel, EditorGUIUtility.sliderLabels.rightLabel, EditorStyles.miniLabel);
                    GUI.color = color;
                    EditorGUIUtility.sliderLabels.SetLabels(null, null);
                }
                if ((GUIUtility.keyboardControl == num3) || (GUIUtility.hotControl == num3))
                {
                    GUIUtility.keyboardControl = id;
                }
                if ((((GUIUtility.keyboardControl == id) && (Event.current.type == EventType.KeyDown)) && !s_RecycledEditor.IsEditingControl(id)) && ((Event.current.keyCode == KeyCode.LeftArrow) || (Event.current.keyCode == KeyCode.RightArrow)))
                {
                    float closestPowerOfTen = MathUtils.GetClosestPowerOfTen(Mathf.Abs((float) ((right - left) * 0.01f)));
                    if ((formatString == kIntFieldFormatString) && (closestPowerOfTen < 1f))
                    {
                        closestPowerOfTen = 1f;
                    }
                    if (Event.current.shift)
                    {
                        closestPowerOfTen *= 10f;
                    }
                    if (Event.current.keyCode == KeyCode.LeftArrow)
                    {
                        value -= closestPowerOfTen * 0.5001f;
                    }
                    else
                    {
                        value += closestPowerOfTen * 0.5001f;
                    }
                    value = MathUtils.RoundToMultipleOf(value, closestPowerOfTen);
                    GUI.changed = true;
                    Event.current.Use();
                }
                if (EndChangeCheck())
                {
                    float f = (right - left) / ((num2 - GUI.skin.horizontalSlider.padding.horizontal) - GUI.skin.horizontalSliderThumb.fixedWidth);
                    value = MathUtils.RoundBasedOnMinimumDifference(value, Mathf.Abs(f));
                    if (s_RecycledEditor.IsEditingControl(id))
                    {
                        s_RecycledEditor.EndEditing();
                    }
                }
                value = DoFloatField(s_RecycledEditor, new Rect((position.x + num2) + 5f, position.y, EditorGUIUtility.fieldWidth, position.height), dragZonePosition, id, value, formatString, EditorStyles.numberField, true);
            }
            else
            {
                width = Mathf.Min(EditorGUIUtility.fieldWidth, width);
                position.x = position.xMax - width;
                position.width = width;
                value = DoFloatField(s_RecycledEditor, position, dragZonePosition, id, value, formatString, EditorStyles.numberField, true);
            }
            value = Mathf.Clamp(value, Mathf.Min(left, right), Mathf.Max(left, right));
            return value;
        }

        internal static string DoTextField(RecycledTextEditor editor, int id, Rect position, string text, GUIStyle style, string allowedletters, out bool changed, bool reset, bool multiline, bool passwordField)
        {
            Event current = Event.current;
            string str = text;
            if (text == null)
            {
                text = string.Empty;
            }
            if (showMixedValue)
            {
                text = string.Empty;
            }
            if (HasKeyboardFocus(id) && (Event.current.type != EventType.Layout))
            {
                if (editor.IsEditingControl(id))
                {
                    editor.position = position;
                    editor.style = style;
                    editor.controlID = id;
                    editor.multiline = multiline;
                    editor.isPasswordField = passwordField;
                    editor.DetectFocusChange();
                }
                else if (EditorGUIUtility.editingTextField)
                {
                    editor.BeginEditing(id, text, position, style, multiline, passwordField);
                    if (GUI.skin.settings.cursorColor.a > 0f)
                    {
                        editor.SelectAll();
                    }
                }
            }
            if ((editor.controlID == id) && (GUIUtility.keyboardControl != id))
            {
                editor.controlID = 0;
            }
            bool flag = false;
            string str2 = editor.text;
            EventType typeForControl = current.GetTypeForControl(id);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && (current.button == 0))
                    {
                        if (!editor.IsEditingControl(id))
                        {
                            GUIUtility.keyboardControl = id;
                            editor.BeginEditing(id, text, position, style, multiline, passwordField);
                            editor.MoveCursorToPosition(Event.current.mousePosition);
                            if (GUI.skin.settings.cursorColor.a > 0f)
                            {
                                s_SelectAllOnMouseUp = true;
                            }
                        }
                        else if ((Event.current.clickCount != 2) || !GUI.skin.settings.doubleClickSelectsWord)
                        {
                            if ((Event.current.clickCount == 3) && GUI.skin.settings.tripleClickSelectsLine)
                            {
                                editor.MoveCursorToPosition(Event.current.mousePosition);
                                editor.SelectCurrentParagraph();
                                editor.MouseDragSelectsWholeWords(true);
                                editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                                s_DragToPosition = false;
                            }
                            else
                            {
                                editor.MoveCursorToPosition(Event.current.mousePosition);
                                s_SelectAllOnMouseUp = false;
                            }
                        }
                        else
                        {
                            editor.MoveCursorToPosition(Event.current.mousePosition);
                            editor.SelectCurrentWord();
                            editor.MouseDragSelectsWholeWords(true);
                            editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                            s_DragToPosition = false;
                        }
                        GUIUtility.hotControl = id;
                        current.Use();
                    }
                    goto Label_09EF;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        if (!s_Dragged || !s_DragToPosition)
                        {
                            if (s_PostPoneMove)
                            {
                                editor.MoveCursorToPosition(Event.current.mousePosition);
                            }
                            else if (s_SelectAllOnMouseUp)
                            {
                                if (GUI.skin.settings.cursorColor.a > 0f)
                                {
                                    editor.SelectAll();
                                }
                                s_SelectAllOnMouseUp = false;
                            }
                        }
                        else
                        {
                            editor.MoveSelectionToAltCursor();
                            flag = true;
                        }
                        editor.MouseDragSelectsWholeWords(false);
                        s_DragToPosition = true;
                        s_Dragged = false;
                        s_PostPoneMove = false;
                        if (current.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                    }
                    goto Label_09EF;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        if ((current.shift || !editor.hasSelection) || !s_DragToPosition)
                        {
                            if (current.shift)
                            {
                                editor.MoveCursorToPosition(Event.current.mousePosition);
                            }
                            else
                            {
                                editor.SelectToPosition(Event.current.mousePosition);
                            }
                            s_DragToPosition = false;
                            s_SelectAllOnMouseUp = !editor.hasSelection;
                        }
                        else
                        {
                            editor.MoveAltCursorToPosition(Event.current.mousePosition);
                        }
                        s_Dragged = true;
                        current.Use();
                    }
                    goto Label_09EF;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id)
                    {
                        char character = current.character;
                        if (!editor.IsEditingControl(id) || !editor.HandleKeyEvent(current))
                        {
                            if (current.keyCode == KeyCode.Escape)
                            {
                                if (editor.IsEditingControl(id))
                                {
                                    if ((style == EditorStyles.toolbarSearchField) || (style == EditorStyles.searchField))
                                    {
                                        s_OriginalText = string.Empty;
                                    }
                                    editor.text = s_OriginalText;
                                    editor.EndEditing();
                                    flag = true;
                                }
                            }
                            else
                            {
                                switch (character)
                                {
                                    case '\n':
                                    case '\x0003':
                                        if (!editor.IsEditingControl(id))
                                        {
                                            editor.BeginEditing(id, text, position, style, multiline, passwordField);
                                            editor.SelectAll();
                                        }
                                        else if ((!multiline || current.alt) || (current.shift || current.control))
                                        {
                                            editor.EndEditing();
                                        }
                                        else
                                        {
                                            editor.Insert(character);
                                            flag = true;
                                            goto Label_09EF;
                                        }
                                        current.Use();
                                        goto Label_09EF;
                                }
                                if ((character == '\t') || (current.keyCode == KeyCode.Tab))
                                {
                                    if (multiline && editor.IsEditingControl(id))
                                    {
                                        bool flag2 = (allowedletters == null) || (allowedletters.IndexOf(character) != -1);
                                        if ((((!current.alt && !current.shift) && !current.control) && (character == '\t')) && flag2)
                                        {
                                            editor.Insert(character);
                                            flag = true;
                                        }
                                    }
                                }
                                else
                                {
                                    switch (character)
                                    {
                                        case '\x0019':
                                        case '\x001b':
                                            goto Label_09EF;
                                    }
                                    if (editor.IsEditingControl(id))
                                    {
                                        if (((allowedletters == null) || (allowedletters.IndexOf(character) != -1)) && (character != '\0'))
                                        {
                                            editor.Insert(character);
                                            flag = true;
                                        }
                                        else
                                        {
                                            if (Input.compositionString != string.Empty)
                                            {
                                                editor.ReplaceSelection(string.Empty);
                                                flag = true;
                                            }
                                            current.Use();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            current.Use();
                            flag = true;
                        }
                    }
                    goto Label_09EF;

                case EventType.Repaint:
                    string str3;
                    if (!editor.IsEditingControl(id))
                    {
                        if (showMixedValue)
                        {
                            str3 = s_MixedValueContent.text;
                        }
                        else
                        {
                            str3 = !passwordField ? text : string.Empty.PadRight(text.Length, '*');
                        }
                    }
                    else
                    {
                        str3 = !passwordField ? editor.text : string.Empty.PadRight(editor.text.Length, '*');
                    }
                    if (!string.IsNullOrEmpty(s_UnitString) && !passwordField)
                    {
                        str3 = str3 + " " + s_UnitString;
                    }
                    if (GUIUtility.hotControl == 0)
                    {
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.Text);
                    }
                    if (!editor.IsEditingControl(id))
                    {
                        BeginHandleMixedValueContentColor();
                        style.Draw(position, EditorGUIUtility.TempContent(str3), id, false);
                        EndHandleMixedValueContentColor();
                    }
                    else
                    {
                        editor.DrawCursor(str3);
                    }
                    goto Label_09EF;

                default:
                    string commandName;
                    Dictionary<string, int> dictionary;
                    int num;
                    switch (typeForControl)
                    {
                        case EventType.ValidateCommand:
                            if (GUIUtility.keyboardControl == id)
                            {
                                commandName = current.commandName;
                                if (commandName != null)
                                {
                                    if (<>f__switch$map0 == null)
                                    {
                                        dictionary = new Dictionary<string, int>(6);
                                        dictionary.Add("Cut", 0);
                                        dictionary.Add("Copy", 0);
                                        dictionary.Add("Paste", 1);
                                        dictionary.Add("SelectAll", 2);
                                        dictionary.Add("UndoRedoPerformed", 3);
                                        dictionary.Add("Delete", 4);
                                        <>f__switch$map0 = dictionary;
                                    }
                                    if (<>f__switch$map0.TryGetValue(commandName, out num))
                                    {
                                        switch (num)
                                        {
                                            case 0:
                                                if (editor.hasSelection)
                                                {
                                                    current.Use();
                                                }
                                                break;

                                            case 1:
                                                if (editor.CanPaste())
                                                {
                                                    current.Use();
                                                }
                                                break;

                                            case 2:
                                                current.Use();
                                                break;

                                            case 3:
                                                editor.text = text;
                                                current.Use();
                                                break;

                                            case 4:
                                                current.Use();
                                                break;
                                        }
                                    }
                                }
                            }
                            goto Label_09EF;

                        case EventType.ExecuteCommand:
                            if (GUIUtility.keyboardControl == id)
                            {
                                commandName = current.commandName;
                                if (commandName != null)
                                {
                                    if (<>f__switch$map1 == null)
                                    {
                                        dictionary = new Dictionary<string, int>(6);
                                        dictionary.Add("OnLostFocus", 0);
                                        dictionary.Add("Cut", 1);
                                        dictionary.Add("Copy", 2);
                                        dictionary.Add("Paste", 3);
                                        dictionary.Add("SelectAll", 4);
                                        dictionary.Add("Delete", 5);
                                        <>f__switch$map1 = dictionary;
                                    }
                                    if (<>f__switch$map1.TryGetValue(commandName, out num))
                                    {
                                        switch (num)
                                        {
                                            case 0:
                                                if (activeEditor != null)
                                                {
                                                    activeEditor.EndEditing();
                                                }
                                                current.Use();
                                                break;

                                            case 1:
                                                editor.BeginEditing(id, text, position, style, multiline, passwordField);
                                                editor.Cut();
                                                flag = true;
                                                break;

                                            case 2:
                                                editor.Copy();
                                                current.Use();
                                                break;

                                            case 3:
                                                editor.BeginEditing(id, text, position, style, multiline, passwordField);
                                                editor.Paste();
                                                flag = true;
                                                break;

                                            case 4:
                                                editor.SelectAll();
                                                current.Use();
                                                break;

                                            case 5:
                                                editor.BeginEditing(id, text, position, style, multiline, passwordField);
                                                if (((Application.platform != RuntimePlatform.OSXPlayer) && (Application.platform != RuntimePlatform.OSXWebPlayer)) && (((Application.platform != RuntimePlatform.OSXDashboardPlayer) && (Application.platform != RuntimePlatform.OSXEditor)) && ((Application.platform != RuntimePlatform.WebGLPlayer) || !SystemInfo.operatingSystem.StartsWith("Mac"))))
                                                {
                                                    editor.Cut();
                                                }
                                                else
                                                {
                                                    editor.Delete();
                                                }
                                                goto Label_03E2;
                                        }
                                    }
                                }
                            }
                            goto Label_09EF;

                        case EventType.DragExited:
                            goto Label_09EF;

                        case EventType.ContextClick:
                            if (position.Contains(current.mousePosition))
                            {
                                if (!editor.IsEditingControl(id))
                                {
                                    GUIUtility.keyboardControl = id;
                                    editor.BeginEditing(id, text, position, style, multiline, passwordField);
                                    editor.MoveCursorToPosition(Event.current.mousePosition);
                                }
                                ShowTextEditorPopupMenu();
                                Event.current.Use();
                            }
                            goto Label_09EF;

                        default:
                            goto Label_09EF;
                    }
                    break;
            }
        Label_03E2:
            flag = true;
            current.Use();
        Label_09EF:
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.textFieldInput = true;
            }
            editor.UpdateScrollOffsetIfNeeded();
            changed = false;
            if (flag)
            {
                changed = str2 != editor.text;
                current.Use();
            }
            if (changed)
            {
                GUI.changed = true;
                return editor.text;
            }
            RecycledTextEditor.s_AllowContextCutOrPaste = true;
            return str;
        }

        internal static string DoTextFieldDropDown(Rect rect, int id, string text, string[] dropDownElements, bool delayed)
        {
            Rect position = new Rect(rect.x, rect.y, rect.width - EditorStyles.textFieldDropDown.fixedWidth, rect.height);
            Rect rect3 = new Rect(position.xMax, position.y, EditorStyles.textFieldDropDown.fixedWidth, rect.height);
            if (delayed)
            {
                text = DelayedTextField(position, text, EditorStyles.textFieldDropDownText);
            }
            else
            {
                bool flag;
                text = DoTextField(s_RecycledEditor, id, position, text, EditorStyles.textFieldDropDownText, null, out flag, false, false, false);
            }
            BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            int index = Popup(rect3, string.Empty, -1, (dropDownElements.Length <= 0) ? new string[] { "--empty--" } : dropDownElements, EditorStyles.textFieldDropDown);
            if (EndChangeCheck() && (dropDownElements.Length > 0))
            {
                text = dropDownElements[index];
            }
            EditorGUI.indentLevel = indentLevel;
            return text;
        }

        internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
        {
            return EditorGUIInternal.DoToggleForward(position, id, value, content, style);
        }

        internal static void DoTwoLabels(Rect rect, GUIContent leftLabel, GUIContent rightLabel, GUIStyle labelStyle)
        {
            if (Event.current.type == EventType.Repaint)
            {
                TextAnchor alignment = labelStyle.alignment;
                labelStyle.alignment = TextAnchor.UpperLeft;
                GUI.Label(rect, leftLabel, labelStyle);
                labelStyle.alignment = TextAnchor.UpperRight;
                GUI.Label(rect, rightLabel, labelStyle);
                labelStyle.alignment = alignment;
            }
        }

        [ExcludeFromDocs]
        public static double DoubleField(Rect position, double value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DoubleField(position, value, numberField);
        }

        public static double DoubleField(Rect position, double value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DoubleFieldInternal(position, value, style);
        }

        [ExcludeFromDocs]
        public static double DoubleField(Rect position, string label, double value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DoubleField(position, label, value, numberField);
        }

        [ExcludeFromDocs]
        public static double DoubleField(Rect position, GUIContent label, double value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return DoubleField(position, label, value, numberField);
        }

        public static double DoubleField(Rect position, string label, double value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DoubleField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static double DoubleField(Rect position, GUIContent label, double value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return DoubleFieldInternal(position, label, value, style);
        }

        internal static double DoubleFieldInternal(Rect position, double value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            return DoDoubleField(s_RecycledEditor, IndentedRect(position), new Rect(0f, 0f, 0f, 0f), id, value, kDoubleFieldFormatString, style, false);
        }

        internal static double DoubleFieldInternal(Rect position, GUIContent label, double value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            return DoDoubleField(s_RecycledEditor, rect, position, id, value, kDoubleFieldFormatString, style, true);
        }

        private static void DragNumberValue(RecycledTextEditor editor, Rect position, Rect dragHotZone, int id, bool isDouble, ref double doubleVal, ref long longVal, string formatString, GUIStyle style, double dragSensitivity)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (dragHotZone.Contains(current.mousePosition) && (current.button == 0))
                    {
                        EditorGUIUtility.editingTextField = false;
                        GUIUtility.hotControl = id;
                        if (activeEditor != null)
                        {
                            activeEditor.EndEditing();
                        }
                        current.Use();
                        GUIUtility.keyboardControl = id;
                        s_DragCandidateState = 1;
                        s_DragStartValue = doubleVal;
                        s_DragStartIntValue = longVal;
                        s_DragStartPos = current.mousePosition;
                        s_DragSensitivity = dragSensitivity;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && (s_DragCandidateState != 0))
                    {
                        GUIUtility.hotControl = 0;
                        s_DragCandidateState = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        switch (s_DragCandidateState)
                        {
                            case 1:
                            {
                                Vector2 vector = Event.current.mousePosition - s_DragStartPos;
                                if (vector.sqrMagnitude > kDragDeadzone)
                                {
                                    s_DragCandidateState = 2;
                                    GUIUtility.keyboardControl = id;
                                }
                                current.Use();
                                return;
                            }
                            case 2:
                                if (isDouble)
                                {
                                    doubleVal += HandleUtility.niceMouseDelta * s_DragSensitivity;
                                    doubleVal = MathUtils.RoundBasedOnMinimumDifference(doubleVal, s_DragSensitivity);
                                }
                                else
                                {
                                    longVal += (long) Math.Round((double) (HandleUtility.niceMouseDelta * s_DragSensitivity));
                                }
                                GUI.changed = true;
                                current.Use();
                                return;
                        }
                    }
                    break;

                case EventType.KeyDown:
                    if (((GUIUtility.hotControl == id) && (current.keyCode == KeyCode.Escape)) && (s_DragCandidateState != 0))
                    {
                        doubleVal = s_DragStartValue;
                        longVal = s_DragStartIntValue;
                        GUI.changed = true;
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(dragHotZone, MouseCursor.SlideArrow);
                    break;
            }
        }

        private static void Draw4(Rect position, GUIContent content, float offset, float alpha, GUIStyle style)
        {
            GUI.color = new Color(0f, 0f, 0f, alpha);
            position.y -= offset;
            style.Draw(position, content, false, false, false, false);
            position.y += offset * 2f;
            style.Draw(position, content, false, false, false, false);
            position.y -= offset;
            position.x -= offset;
            style.Draw(position, content, false, false, false, false);
            position.x += offset * 2f;
            style.Draw(position, content, false, false, false, false);
        }

        private static Rect DrawBoundsFieldLabelsAndAdjustPositionForValues(Rect position, bool drawOutside)
        {
            if (drawOutside)
            {
                position.xMin -= 53f;
            }
            GUI.Label(IndentedRect(position), "Center:", EditorStyles.label);
            position.y += 16f;
            GUI.Label(IndentedRect(position), "Extents:", EditorStyles.label);
            position.y -= 16f;
            position.xMin += 53f;
            return position;
        }

        internal static void DrawLabelShadow(Rect position, GUIContent content, GUIStyle style, float shadowOpa)
        {
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            Color backgroundColor = GUI.backgroundColor;
            GUI.contentColor = new Color(0f, 0f, 0f, 0f);
            style.Draw(position, content, false, false, false, false);
            position.y++;
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0f);
            GUI.contentColor = contentColor;
            Draw4(position, content, 1f, GUI.color.a * shadowOpa, style);
            Draw4(position, content, 2f, (GUI.color.a * shadowOpa) * 0.42f, style);
            GUI.color = color;
            GUI.backgroundColor = backgroundColor;
        }

        internal static void DrawLegend(Rect position, Color color, string label, bool enabled)
        {
            position = new Rect(position.x + 2f, position.y + 2f, position.width - 2f, position.height - 2f);
            Color backgroundColor = GUI.backgroundColor;
            if (enabled)
            {
                GUI.backgroundColor = color;
            }
            else
            {
                GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.45f);
            }
            GUI.Label(position, label, "ProfilerPaneSubLabel");
            GUI.backgroundColor = backgroundColor;
        }

        private static void DrawObjectFieldLargeThumb(Rect position, int id, Object obj, GUIContent content)
        {
            GUIStyle objectFieldThumb = EditorStyles.objectFieldThumb;
            objectFieldThumb.Draw(position, GUIContent.none, id, DragAndDrop.activeControlID == id);
            if ((obj != null) && !showMixedValue)
            {
                bool flag = obj is Cubemap;
                bool flag2 = obj is Sprite;
                Rect rect = objectFieldThumb.padding.Remove(position);
                if (flag || flag2)
                {
                    Texture2D assetPreview = AssetPreview.GetAssetPreview(obj);
                    if (assetPreview != null)
                    {
                        if (flag2 || assetPreview.alphaIsTransparency)
                        {
                            DrawTextureTransparent(rect, assetPreview);
                        }
                        else
                        {
                            DrawPreviewTexture(rect, assetPreview);
                        }
                    }
                    else
                    {
                        rect.x += (rect.width - content.image.width) / 2f;
                        rect.y += (rect.height - content.image.width) / 2f;
                        GUIStyle.none.Draw(rect, content.image, false, false, false, false);
                        HandleUtility.Repaint();
                    }
                }
                else
                {
                    Texture2D image = content.image as Texture2D;
                    if ((image != null) && image.alphaIsTransparency)
                    {
                        DrawTextureTransparent(rect, image);
                    }
                    else
                    {
                        DrawPreviewTexture(rect, content.image);
                    }
                }
            }
            else
            {
                GUIStyle style2 = objectFieldThumb.name + "Overlay";
                BeginHandleMixedValueContentColor();
                style2.Draw(position, content, id);
                EndHandleMixedValueContentColor();
            }
            (objectFieldThumb.name + "Overlay2").Draw(position, EditorGUIUtility.TempContent("Select"), id);
        }

        private static void DrawObjectFieldMiniThumb(Rect position, int id, Object obj, GUIContent content)
        {
            GUIStyle objectFieldMiniThumb = EditorStyles.objectFieldMiniThumb;
            position.width = 32f;
            BeginHandleMixedValueContentColor();
            bool isHover = obj != null;
            bool on = DragAndDrop.activeControlID == id;
            bool hasKeyboardFocus = GUIUtility.keyboardControl == id;
            objectFieldMiniThumb.Draw(position, isHover, false, on, hasKeyboardFocus);
            EndHandleMixedValueContentColor();
            if ((obj != null) && !showMixedValue)
            {
                Rect rect = new Rect(position.x + 1f, position.y + 1f, position.height - 2f, position.height - 2f);
                Texture2D image = content.image as Texture2D;
                if ((image != null) && image.alphaIsTransparency)
                {
                    DrawTextureTransparent(rect, image);
                }
                else
                {
                    DrawPreviewTexture(rect, content.image);
                }
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUI.Label(rect, GUIContent.Temp(string.Empty, "Ctrl + Click to show preview"));
                }
            }
        }

        [ExcludeFromDocs]
        public static void DrawPreviewTexture(Rect position, Texture image)
        {
            float imageAspect = 0f;
            ScaleMode stretchToFill = ScaleMode.StretchToFill;
            Material mat = null;
            DrawPreviewTexture(position, image, mat, stretchToFill, imageAspect);
        }

        [ExcludeFromDocs]
        public static void DrawPreviewTexture(Rect position, Texture image, Material mat)
        {
            float imageAspect = 0f;
            ScaleMode stretchToFill = ScaleMode.StretchToFill;
            DrawPreviewTexture(position, image, mat, stretchToFill, imageAspect);
        }

        [ExcludeFromDocs]
        public static void DrawPreviewTexture(Rect position, Texture image, Material mat, ScaleMode scaleMode)
        {
            float imageAspect = 0f;
            DrawPreviewTexture(position, image, mat, scaleMode, imageAspect);
        }

        public static void DrawPreviewTexture(Rect position, Texture image, [DefaultValue("null")] Material mat, [DefaultValue("ScaleMode.StretchToFill")] ScaleMode scaleMode, [DefaultValue("0")] float imageAspect)
        {
            DrawPreviewTextureInternal(position, image, mat, scaleMode, imageAspect);
        }

        internal static void DrawPreviewTextureInternal(Rect position, Texture image, Material mat, ScaleMode scaleMode, float imageAspect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (imageAspect == 0f)
                {
                    imageAspect = ((float) image.width) / ((float) image.height);
                }
                if (mat == null)
                {
                    mat = GetMaterialForSpecialTexture(image);
                }
                GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear) && !TextureUtil.GetLinearSampled(image);
                if (mat == null)
                {
                    GUI.DrawTexture(position, image, scaleMode, false, imageAspect);
                    GL.sRGBWrite = false;
                }
                else
                {
                    Rect outScreenRect = new Rect();
                    Rect outSourceRect = new Rect();
                    GUI.CalculateScaledTextureRects(position, scaleMode, imageAspect, ref outScreenRect, ref outSourceRect);
                    Texture2D t = image as Texture2D;
                    if ((t != null) && (TextureUtil.GetUsageMode(image) == TextureUsageMode.AlwaysPadded))
                    {
                        outSourceRect.width *= ((float) t.width) / ((float) TextureUtil.GetGLWidth(t));
                        outSourceRect.height *= ((float) t.height) / ((float) TextureUtil.GetGLHeight(t));
                    }
                    Graphics.DrawTexture(outScreenRect, image, outSourceRect, 0, 0, 0, 0, GUI.color, mat);
                    GL.sRGBWrite = false;
                }
            }
        }

        public static void DrawRect(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color2 = GUI.color;
                GUI.color *= color;
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                GUI.color = color2;
            }
        }

        private static void DrawTextDebugHelpers(Rect labelPosition)
        {
            Color color = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(labelPosition.x, labelPosition.y, labelPosition.width, 4f), EditorGUIUtility.whiteTexture);
            GUI.color = Color.cyan;
            GUI.DrawTexture(new Rect(labelPosition.x, labelPosition.yMax - 4f, labelPosition.width, 4f), EditorGUIUtility.whiteTexture);
            GUI.color = color;
        }

        [ExcludeFromDocs]
        public static void DrawTextureAlpha(Rect position, Texture image)
        {
            float imageAspect = 0f;
            ScaleMode stretchToFill = ScaleMode.StretchToFill;
            DrawTextureAlpha(position, image, stretchToFill, imageAspect);
        }

        [ExcludeFromDocs]
        public static void DrawTextureAlpha(Rect position, Texture image, ScaleMode scaleMode)
        {
            float imageAspect = 0f;
            DrawTextureAlpha(position, image, scaleMode, imageAspect);
        }

        public static void DrawTextureAlpha(Rect position, Texture image, [DefaultValue("ScaleMode.StretchToFill")] ScaleMode scaleMode, [DefaultValue("0")] float imageAspect)
        {
            DrawTextureAlphaInternal(position, image, scaleMode, imageAspect);
        }

        internal static void DrawTextureAlphaInternal(Rect position, Texture image, ScaleMode scaleMode, float imageAspect)
        {
            DrawPreviewTextureInternal(position, image, alphaMaterial, scaleMode, imageAspect);
        }

        [ExcludeFromDocs]
        public static void DrawTextureTransparent(Rect position, Texture image)
        {
            float imageAspect = 0f;
            ScaleMode stretchToFill = ScaleMode.StretchToFill;
            DrawTextureTransparent(position, image, stretchToFill, imageAspect);
        }

        [ExcludeFromDocs]
        public static void DrawTextureTransparent(Rect position, Texture image, ScaleMode scaleMode)
        {
            float imageAspect = 0f;
            DrawTextureTransparent(position, image, scaleMode, imageAspect);
        }

        public static void DrawTextureTransparent(Rect position, Texture image, [DefaultValue("ScaleMode.StretchToFill")] ScaleMode scaleMode, [DefaultValue("0")] float imageAspect)
        {
            DrawTextureTransparentInternal(position, image, scaleMode, imageAspect);
        }

        internal static void DrawTextureTransparentInternal(Rect position, Texture image, ScaleMode scaleMode, float imageAspect)
        {
            if ((imageAspect == 0f) && (image == null))
            {
                Debug.LogError("Please specify an image or a imageAspect");
            }
            else
            {
                if (imageAspect == 0f)
                {
                    imageAspect = ((float) image.width) / ((float) image.height);
                }
                DrawTransparencyCheckerTexture(position, scaleMode, imageAspect);
                if (image != null)
                {
                    DrawPreviewTexture(position, image, transparentMaterial, scaleMode, imageAspect);
                }
            }
        }

        internal static void DrawTransparencyCheckerTexture(Rect position, ScaleMode scaleMode, float imageAspect)
        {
            Rect outScreenRect = new Rect();
            Rect outSourceRect = new Rect();
            GUI.CalculateScaledTextureRects(position, scaleMode, imageAspect, ref outScreenRect, ref outSourceRect);
            GUI.DrawTextureWithTexCoords(outScreenRect, transparentCheckerTexture, new Rect((outScreenRect.width * -0.5f) / ((float) transparentCheckerTexture.width), (outScreenRect.height * -0.5f) / ((float) transparentCheckerTexture.height), outScreenRect.width / ((float) transparentCheckerTexture.width), outScreenRect.height / ((float) transparentCheckerTexture.height)), false);
        }

        public static void DropShadowLabel(Rect position, string text)
        {
            DoDropShadowLabel(position, EditorGUIUtility.TempContent(text), "PreOverlayLabel", 0.6f);
        }

        public static void DropShadowLabel(Rect position, GUIContent content)
        {
            DoDropShadowLabel(position, content, "PreOverlayLabel", 0.6f);
        }

        public static void DropShadowLabel(Rect position, string text, GUIStyle style)
        {
            DoDropShadowLabel(position, EditorGUIUtility.TempContent(text), style, 0.6f);
        }

        public static void DropShadowLabel(Rect position, GUIContent content, GUIStyle style)
        {
            DoDropShadowLabel(position, content, style, 0.6f);
        }

        public static bool EndChangeCheck()
        {
            bool changed = GUI.changed;
            GUI.changed |= s_ChangedStack.Pop();
            return changed;
        }

        internal static void EndCollectTooltips()
        {
            isCollectingTooltips = false;
        }

        public static void EndDisabledGroup()
        {
            GUI.enabled = s_EnabledStack.Pop();
        }

        internal static void EndEditingActiveTextField()
        {
            if (activeEditor != null)
            {
                activeEditor.EndEditing();
            }
        }

        internal static void EndHandleMixedValueContentColor()
        {
            GUI.contentColor = s_MixedValueContentColorTemp;
        }

        public static void EndProperty()
        {
            showMixedValue = false;
            PropertyGUIData data = s_PropertyStack.Pop();
            if ((Event.current.type == EventType.ContextClick) && data.totalPosition.Contains(Event.current.mousePosition))
            {
                DoPropertyContextMenu(data.property);
            }
            EditorGUIUtility.SetBoldDefaultFont(data.wasBoldDefaultFont);
            GUI.enabled = data.wasEnabled;
            GUI.color = data.color;
            if (s_PendingPropertyKeyboardHandling != null)
            {
                DoPropertyFieldKeyboardHandling(s_PendingPropertyKeyboardHandling);
            }
            if ((s_PendingPropertyDelete != null) && (s_PropertyStack.Count == 0))
            {
                if (s_PendingPropertyDelete.propertyPath == data.property.propertyPath)
                {
                    data.property.DeleteCommand();
                }
                else
                {
                    s_PendingPropertyDelete.DeleteCommand();
                }
                s_PendingPropertyDelete = null;
            }
        }

        private static Enum EnumFlagsToInt(Type type, int value)
        {
            return (Enum.Parse(type, value.ToString()) as Enum);
        }

        [ExcludeFromDocs]
        public static Enum EnumMaskField(Rect position, Enum enumValue)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumMaskField(position, enumValue, popup);
        }

        public static Enum EnumMaskField(Rect position, Enum enumValue, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumMaskFieldInternal(position, enumValue, style);
        }

        [ExcludeFromDocs]
        public static Enum EnumMaskField(Rect position, string label, Enum enumValue)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumMaskField(position, label, enumValue, popup);
        }

        [ExcludeFromDocs]
        public static Enum EnumMaskField(Rect position, GUIContent label, Enum enumValue)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumMaskField(position, label, enumValue, popup);
        }

        public static Enum EnumMaskField(Rect position, string label, Enum enumValue, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumMaskFieldInternal(position, EditorGUIUtility.TempContent(label), enumValue, style);
        }

        public static Enum EnumMaskField(Rect position, GUIContent label, Enum enumValue, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumMaskFieldInternal(position, label, enumValue, style);
        }

        internal static Enum EnumMaskField(Rect position, Enum enumValue, GUIStyle style, out int changedFlags, out bool changedToValue)
        {
            Type type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            if (<>f__am$cache61 == null)
            {
                <>f__am$cache61 = x => ObjectNames.NicifyVariableName(x);
            }
            string[] flagNames = Enum.GetNames(enumValue.GetType()).Select<string, string>(<>f__am$cache61).ToArray<string>();
            int num = MaskFieldGUI.DoMaskField(IndentedRect(position), GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position), Convert.ToInt32(enumValue), flagNames, style, out changedFlags, out changedToValue);
            return EnumFlagsToInt(type, num);
        }

        internal static Enum EnumMaskFieldInternal(Rect position, Enum enumValue, GUIStyle style)
        {
            Type type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            if (<>f__am$cache60 == null)
            {
                <>f__am$cache60 = x => ObjectNames.NicifyVariableName(x);
            }
            string[] flagNames = Enum.GetNames(enumValue.GetType()).Select<string, string>(<>f__am$cache60).ToArray<string>();
            int num = MaskFieldGUI.DoMaskField(IndentedRect(position), GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position), Convert.ToInt32(enumValue), flagNames, style);
            return EnumFlagsToInt(type, num);
        }

        internal static Enum EnumMaskFieldInternal(Rect position, GUIContent label, Enum enumValue, GUIStyle style)
        {
            Type type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            int id = GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            if (<>f__am$cache5F == null)
            {
                <>f__am$cache5F = x => ObjectNames.NicifyVariableName(x);
            }
            string[] flagNames = Enum.GetNames(enumValue.GetType()).Select<string, string>(<>f__am$cache5F).ToArray<string>();
            int num2 = MaskFieldGUI.DoMaskField(rect, id, Convert.ToInt32(enumValue), flagNames, style);
            return EnumFlagsToInt(type, num2);
        }

        [ExcludeFromDocs]
        public static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumMaskPopup(position, label, selected, popup);
        }

        public static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            int num;
            bool flag;
            return EnumMaskPopupInternal(position, label, selected, out num, out flag, style);
        }

        [ExcludeFromDocs]
        internal static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected, out int changedFlags, out bool changedToValue)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumMaskPopup(position, label, selected, out changedFlags, out changedToValue, popup);
        }

        internal static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumMaskPopupInternal(position, label, selected, out changedFlags, out changedToValue, style);
        }

        private static Enum EnumMaskPopupInternal(Rect position, GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position);
            return EnumMaskField(PrefixLabel(position, id, label), selected, style, out changedFlags, out changedToValue);
        }

        [ExcludeFromDocs]
        public static Enum EnumPopup(Rect position, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumPopup(position, selected, popup);
        }

        public static Enum EnumPopup(Rect position, Enum selected, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumPopup(position, GUIContent.none, selected, style);
        }

        [ExcludeFromDocs]
        public static Enum EnumPopup(Rect position, string label, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumPopup(position, label, selected, popup);
        }

        [ExcludeFromDocs]
        public static Enum EnumPopup(Rect position, GUIContent label, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return EnumPopup(position, label, selected, popup);
        }

        public static Enum EnumPopup(Rect position, string label, Enum selected, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumPopup(position, EditorGUIUtility.TempContent(label), selected, style);
        }

        public static Enum EnumPopup(Rect position, GUIContent label, Enum selected, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return EnumPopupInternal(position, label, selected, style);
        }

        private static Enum EnumPopupInternal(Rect position, GUIContent label, Enum selected, GUIStyle style)
        {
            Type enumType = selected.GetType();
            if (!enumType.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            Enum[] array = Enum.GetValues(enumType).Cast<Enum>().ToArray<Enum>();
            string[] names = Enum.GetNames(enumType);
            int index = Array.IndexOf<Enum>(array, selected);
            if (<>f__am$cache5E == null)
            {
                <>f__am$cache5E = x => ObjectNames.NicifyVariableName(x);
            }
            index = Popup(position, label, index, EditorGUIUtility.TempContent(names.Select<string, string>(<>f__am$cache5E).ToArray<string>()), style);
            if ((index >= 0) && (index < names.Length))
            {
                return array[index];
            }
            return selected;
        }

        [ExcludeFromDocs]
        public static float FloatField(Rect position, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return FloatField(position, value, numberField);
        }

        public static float FloatField(Rect position, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return FloatFieldInternal(position, value, style);
        }

        [ExcludeFromDocs]
        public static float FloatField(Rect position, string label, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return FloatField(position, label, value, numberField);
        }

        [ExcludeFromDocs]
        public static float FloatField(Rect position, GUIContent label, float value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return FloatField(position, label, value, numberField);
        }

        public static float FloatField(Rect position, string label, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return FloatField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static float FloatField(Rect position, GUIContent label, float value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return FloatFieldInternal(position, label, value, style);
        }

        internal static float FloatFieldInternal(Rect position, float value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            return DoFloatField(s_RecycledEditor, IndentedRect(position), new Rect(0f, 0f, 0f, 0f), id, value, kFloatFieldFormatString, style, false);
        }

        internal static float FloatFieldInternal(Rect position, GUIContent label, float value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            return DoFloatField(s_RecycledEditor, rect, position, id, value, kFloatFieldFormatString, style, true);
        }

        public static void FocusTextInControl(string name)
        {
            GUI.FocusControl(name);
            EditorGUIUtility.editingTextField = true;
        }

        [ExcludeFromDocs]
        public static bool Foldout(Rect position, bool foldout, string content)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(position, foldout, content, style);
        }

        [ExcludeFromDocs]
        public static bool Foldout(Rect position, bool foldout, GUIContent content)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(position, foldout, content, style);
        }

        [ExcludeFromDocs]
        public static bool Foldout(Rect position, bool foldout, string content, bool toggleOnLabelClick)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(position, foldout, content, toggleOnLabelClick, style);
        }

        public static bool Foldout(Rect position, bool foldout, string content, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
        {
            return FoldoutInternal(position, foldout, EditorGUIUtility.TempContent(content), false, style);
        }

        [ExcludeFromDocs]
        public static bool Foldout(Rect position, bool foldout, GUIContent content, bool toggleOnLabelClick)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(position, foldout, content, toggleOnLabelClick, style);
        }

        public static bool Foldout(Rect position, bool foldout, GUIContent content, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
        {
            return FoldoutInternal(position, foldout, content, false, style);
        }

        public static bool Foldout(Rect position, bool foldout, string content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
        {
            return FoldoutInternal(position, foldout, EditorGUIUtility.TempContent(content), toggleOnLabelClick, style);
        }

        public static bool Foldout(Rect position, bool foldout, GUIContent content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
        {
            return FoldoutInternal(position, foldout, content, toggleOnLabelClick, style);
        }

        internal static bool FoldoutInternal(Rect position, bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
        {
            Rect rect = position;
            if (EditorGUIUtility.hierarchyMode)
            {
                int num = EditorStyles.foldout.padding.left - EditorStyles.label.padding.left;
                position.xMin -= num;
            }
            int controlID = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            EventType rawType = Event.current.type;
            if ((!GUI.enabled && GUIClip.enabled) && (((Event.current.rawType == EventType.MouseDown) || (Event.current.rawType == EventType.MouseDrag)) || (Event.current.rawType == EventType.MouseUp)))
            {
                rawType = Event.current.rawType;
            }
            switch (rawType)
            {
                case EventType.MouseDown:
                    if (position.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                    {
                        int num3 = controlID;
                        GUIUtility.hotControl = num3;
                        GUIUtility.keyboardControl = num3;
                        Event.current.Use();
                    }
                    return foldout;

                case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        return foldout;
                    }
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    Rect rect2 = position;
                    if (!toggleOnLabelClick)
                    {
                        rect2.width = style.padding.left;
                        rect2.x += indent;
                    }
                    if (!rect2.Contains(Event.current.mousePosition))
                    {
                        return foldout;
                    }
                    GUI.changed = true;
                    return !foldout;
                }
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Event.current.Use();
                    }
                    return foldout;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == controlID)
                    {
                        KeyCode keyCode = Event.current.keyCode;
                        if (((keyCode != KeyCode.LeftArrow) || !foldout) && ((keyCode != KeyCode.RightArrow) || foldout))
                        {
                            return foldout;
                        }
                        foldout = !foldout;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    return foldout;

                case EventType.Repaint:
                {
                    EditorStyles.foldoutSelected.Draw(position, GUIContent.none, controlID, s_DragUpdatedOverID == controlID);
                    Rect rect3 = new Rect(position.x + indent, position.y, EditorGUIUtility.labelWidth - indent, position.height);
                    if (!showMixedValue || foldout)
                    {
                        style.Draw(rect3, content, controlID, foldout);
                        return foldout;
                    }
                    style.Draw(rect3, content, controlID, foldout);
                    BeginHandleMixedValueContentColor();
                    Rect rect4 = rect;
                    rect4.xMin += EditorGUIUtility.labelWidth;
                    EditorStyles.label.Draw(rect4, s_MixedValueContent, controlID, false);
                    EndHandleMixedValueContentColor();
                    return foldout;
                }
                case EventType.DragUpdated:
                    if (s_DragUpdatedOverID != controlID)
                    {
                        if (position.Contains(Event.current.mousePosition))
                        {
                            s_DragUpdatedOverID = controlID;
                            s_FoldoutDestTime = Time.realtimeSinceStartup + 0.7;
                            Event.current.Use();
                        }
                        return foldout;
                    }
                    if (!position.Contains(Event.current.mousePosition))
                    {
                        s_DragUpdatedOverID = 0;
                        return foldout;
                    }
                    if (Time.realtimeSinceStartup > s_FoldoutDestTime)
                    {
                        foldout = true;
                        Event.current.Use();
                    }
                    return foldout;

                case EventType.DragExited:
                    if (s_DragUpdatedOverID == controlID)
                    {
                        s_DragUpdatedOverID = 0;
                        Event.current.Use();
                    }
                    return foldout;
            }
            return foldout;
        }

        internal static bool FoldoutTitlebar(Rect position, GUIContent label, bool foldout)
        {
            int controlID = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle inspectorTitlebar = EditorStyles.inspectorTitlebar;
                GUIStyle inspectorTitlebarText = EditorStyles.inspectorTitlebarText;
                Rect rect = new Rect(((position.x + inspectorTitlebar.padding.left) + 2f) + 16f, position.y + inspectorTitlebar.padding.top, 200f, 16f);
                inspectorTitlebar.Draw(position, GUIContent.none, controlID, foldout);
                position = inspectorTitlebar.padding.Remove(position);
                inspectorTitlebarText.Draw(rect, EditorGUIUtility.TempContent(label.text), controlID, foldout);
            }
            return EditorGUIInternal.DoToggleForward(IndentedRect(position), controlID, foldout, GUIContent.none, GUIStyle.none);
        }

        internal static void GameViewSizePopup(Rect buttonRect, GameViewSizeGroupType groupType, int selectedIndex, Action<int, object> itemClickedCallback, GUIStyle guiStyle)
        {
            GameViewSizeGroup group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType);
            string t = null;
            if ((selectedIndex >= 0) && (selectedIndex < group.GetTotalCount()))
            {
                t = group.GetGameViewSize(selectedIndex).displayText;
            }
            else
            {
                t = string.Empty;
            }
            if (ButtonMouseDown(buttonRect, GUIContent.Temp(t), FocusType.Passive, guiStyle))
            {
                IFlexibleMenuItemProvider itemProvider = new GameViewSizesMenuItemProvider(groupType);
                FlexibleMenu windowContent = new FlexibleMenu(itemProvider, selectedIndex, new GameViewSizesMenuModifyItemUI(), itemClickedCallback);
                PopupWindow.Show(buttonRect, windowContent);
            }
        }

        internal static Rect GetInspectorTitleBarObjectFoldoutRenderRect(Rect rect)
        {
            return new Rect(rect.x + 3f, rect.y + 3f, 16f, 16f);
        }

        internal static Material GetMaterialForSpecialTexture(Texture t)
        {
            if (t != null)
            {
                switch (TextureUtil.GetUsageMode(t))
                {
                    case TextureUsageMode.LightmapRGBM:
                    case TextureUsageMode.RGBMEncoded:
                        return lightmapRGBMMaterial;

                    case TextureUsageMode.LightmapDoubleLDR:
                        return lightmapDoubleLDRMaterial;

                    case TextureUsageMode.NormalmapPlain:
                    case TextureUsageMode.NormalmapDXT5nm:
                        return normalmapMaterial;
                }
                if (TextureUtil.IsAlphaOnlyTextureFormat(TextureUtil.GetTextureFormat(t)))
                {
                    return alphaMaterial;
                }
            }
            return null;
        }

        [ExcludeFromDocs]
        public static float GetPropertyHeight(SerializedProperty property)
        {
            bool includeChildren = true;
            GUIContent label = null;
            return GetPropertyHeight(property, label, includeChildren);
        }

        [ExcludeFromDocs]
        public static float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool includeChildren = true;
            return GetPropertyHeight(property, label, includeChildren);
        }

        public static float GetPropertyHeight(SerializedProperty property, [DefaultValue("null")] GUIContent label, [DefaultValue("true")] bool includeChildren)
        {
            return GetPropertyHeightInternal(property, label, includeChildren);
        }

        internal static float GetPropertyHeightInternal(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return ScriptAttributeUtility.GetHandler(property).GetHeight(property, label, includeChildren);
        }

        internal static void GetRectsForMiniThumbnailField(Rect position, out Rect thumbRect, out Rect labelRect)
        {
            thumbRect = IndentedRect(position);
            thumbRect.y--;
            thumbRect.height = 18f;
            thumbRect.width = 32f;
            float x = thumbRect.x + 30f;
            labelRect = new Rect(x, position.y, (thumbRect.x + EditorGUIUtility.labelWidth) - x, position.height);
        }

        internal static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property != null)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3:
                    case SerializedPropertyType.Vector2:
                        return (((LabelHasContent(label) && !EditorGUIUtility.wideMode) ? 16f : 0f) + 16f);

                    case SerializedPropertyType.Rect:
                        return (((LabelHasContent(label) && !EditorGUIUtility.wideMode) ? 16f : 0f) + 32f);

                    case SerializedPropertyType.Bounds:
                        return ((LabelHasContent(label) ? 16f : 0f) + 32f);
                }
            }
            return 16f;
        }

        internal static Gradient GradientField(Rect position, SerializedProperty gradient)
        {
            int id = GUIUtility.GetControlID(s_GradientHash, EditorGUIUtility.native, position);
            return DoGradientField(position, id, null, gradient);
        }

        internal static Gradient GradientField(Rect position, Gradient gradient)
        {
            int id = GUIUtility.GetControlID(s_GradientHash, EditorGUIUtility.native, position);
            return DoGradientField(position, id, gradient, null);
        }

        internal static Gradient GradientField(string label, Rect position, SerializedProperty property)
        {
            return GradientField(EditorGUIUtility.TempContent(label), position, property);
        }

        internal static Gradient GradientField(string label, Rect position, Gradient gradient)
        {
            return GradientField(EditorGUIUtility.TempContent(label), position, gradient);
        }

        internal static Gradient GradientField(GUIContent label, Rect position, SerializedProperty property)
        {
            int id = GUIUtility.GetControlID(s_GradientHash, EditorGUIUtility.native, position);
            return DoGradientField(PrefixLabel(position, id, label), id, null, property);
        }

        internal static Gradient GradientField(GUIContent label, Rect position, Gradient gradient)
        {
            int id = GUIUtility.GetControlID(s_GradientHash, EditorGUIUtility.native, position);
            return DoGradientField(PrefixLabel(position, id, label), id, gradient, null);
        }

        [ExcludeFromDocs]
        public static void HandlePrefixLabel(Rect totalPosition, Rect labelPosition, GUIContent label)
        {
            GUIStyle style = EditorStyles.label;
            int id = 0;
            HandlePrefixLabel(totalPosition, labelPosition, label, id, style);
        }

        [ExcludeFromDocs]
        public static void HandlePrefixLabel(Rect totalPosition, Rect labelPosition, GUIContent label, int id)
        {
            GUIStyle style = EditorStyles.label;
            HandlePrefixLabel(totalPosition, labelPosition, label, id, style);
        }

        public static void HandlePrefixLabel(Rect totalPosition, Rect labelPosition, GUIContent label, [DefaultValue("0")] int id, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            HandlePrefixLabelInternal(totalPosition, labelPosition, label, id, style);
        }

        internal static void HandlePrefixLabelInternal(Rect totalPosition, Rect labelPosition, GUIContent label, int id, GUIStyle style)
        {
            if ((id == 0) && (label != null))
            {
                s_PrefixLabel.text = label.text;
                s_PrefixLabel.image = label.image;
                s_PrefixLabel.tooltip = label.tooltip;
                s_PrefixTotalRect = totalPosition;
                s_PrefixRect = labelPosition;
                s_PrefixStyle = style;
            }
            else
            {
                if ((Highlighter.searchMode == HighlightSearchMode.PrefixLabel) || (Highlighter.searchMode == HighlightSearchMode.Auto))
                {
                    Highlighter.Handle(totalPosition, label.text);
                }
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if ((Event.current.button == 0) && labelPosition.Contains(Event.current.mousePosition))
                        {
                            if (EditorGUIUtility.CanHaveKeyboardFocus(id))
                            {
                                GUIUtility.keyboardControl = id;
                            }
                            EditorGUIUtility.editingTextField = false;
                            HandleUtility.Repaint();
                        }
                        break;

                    case EventType.Repaint:
                        labelPosition.width++;
                        style.DrawPrefixLabel(labelPosition, label, id);
                        break;
                }
                s_PrefixLabel.text = null;
            }
        }

        private static Object HandleTextureToSprite(Texture2D tex)
        {
            Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(Sprite))
                {
                    return objArray[i];
                }
            }
            return tex;
        }

        private static bool HasKeyboardFocus(int controlID)
        {
            return ((GUIUtility.keyboardControl == controlID) && GUIView.current.hasFocus);
        }

        internal static bool HasVisibleChildFields(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.Bounds:
                    return false;
            }
            return property.hasVisibleChildren;
        }

        public static void HelpBox(Rect position, string message, MessageType type)
        {
            GUI.Label(position, EditorGUIUtility.TempContent(message, EditorGUIUtility.GetHelpIcon(type)), EditorStyles.helpBox);
        }

        internal static bool HelpIconButton(Rect position, Object obj)
        {
            bool flag = Unsupported.IsDeveloperBuild();
            bool defaultToMonoBehaviour = !flag || obj.GetType().Assembly.ToString().StartsWith("Assembly-");
            bool flag3 = Help.HasHelpForObject(obj, defaultToMonoBehaviour);
            if (!flag3 && !flag)
            {
                return false;
            }
            Color color = GUI.color;
            GUIContent content = new GUIContent(GUIContents.helpIcon);
            string niceHelpNameForObject = Help.GetNiceHelpNameForObject(obj, defaultToMonoBehaviour);
            if (flag && !flag3)
            {
                GUI.color = Color.yellow;
                string str2 = (!(obj is MonoBehaviour) ? "sealed partial class-" : "script-") + niceHelpNameForObject;
                content.tooltip = string.Format("Could not find Reference page for {0} ({1}).\nDocs for this object is missing or all docs are missing.\nThis warning only shows up in development builds.", niceHelpNameForObject, str2);
            }
            else
            {
                content.tooltip = string.Format("Open Reference for {0}.", niceHelpNameForObject);
            }
            GUIStyle inspectorTitlebarText = EditorStyles.inspectorTitlebarText;
            if (GUI.Button(position, content, inspectorTitlebarText))
            {
                Help.ShowHelpForObject(obj);
            }
            GUI.color = color;
            return true;
        }

        internal static Color HexColorTextField(Rect rect, GUIContent label, Color color, bool showAlpha)
        {
            return HexColorTextField(rect, label, color, showAlpha, EditorStyles.textField);
        }

        internal static Color HexColorTextField(Rect rect, GUIContent label, Color color, bool showAlpha, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, rect);
            return DoHexColorTextField(PrefixLabel(rect, id, label), color, showAlpha, style);
        }

        internal static bool IconButton(int id, Rect position, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            switch (Event.current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (!position.Contains(Event.current.mousePosition))
                    {
                        return false;
                    }
                    GUIUtility.hotControl = id;
                    Event.current.Use();
                    return true;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl != id)
                    {
                        return false;
                    }
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    return position.Contains(Event.current.mousePosition);

                case EventType.MouseDrag:
                    if (!position.Contains(Event.current.mousePosition))
                    {
                        break;
                    }
                    GUIUtility.hotControl = id;
                    Event.current.Use();
                    return true;

                case EventType.Repaint:
                    style.Draw(position, content, id);
                    break;
            }
            return false;
        }

        public static Rect IndentedRect(Rect source)
        {
            float indent = EditorGUI.indent;
            return new Rect(source.x + indent, source.y, source.width - indent, source.height);
        }

        public static void InspectorTitlebar(Rect position, Object[] targetObjs)
        {
            GUIStyle none = GUIStyle.none;
            int id = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
            DoInspectorTitlebar(position, id, true, targetObjs, none);
        }

        public static bool InspectorTitlebar(Rect position, bool foldout, Object targetObj, bool expandable)
        {
            Object[] targetObjs = new Object[] { targetObj };
            return InspectorTitlebar(position, foldout, targetObjs, expandable);
        }

        public static bool InspectorTitlebar(Rect position, bool foldout, Object[] targetObjs, bool expandable)
        {
            GUIStyle inspectorTitlebar = EditorStyles.inspectorTitlebar;
            int id = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
            DoInspectorTitlebar(position, id, foldout, targetObjs, inspectorTitlebar);
            if (expandable)
            {
                Rect inspectorTitleBarObjectFoldoutRenderRect = GetInspectorTitleBarObjectFoldoutRenderRect(position);
                return DoObjectFoldout(foldout, position, inspectorTitleBarObjectFoldoutRenderRect, targetObjs, id);
            }
            return foldout;
        }

        [ExcludeFromDocs]
        public static int IntField(Rect position, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return IntField(position, value, numberField);
        }

        public static int IntField(Rect position, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return IntFieldInternal(position, value, style);
        }

        [ExcludeFromDocs]
        public static int IntField(Rect position, string label, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return IntField(position, label, value, numberField);
        }

        [ExcludeFromDocs]
        public static int IntField(Rect position, GUIContent label, int value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return IntField(position, label, value, numberField);
        }

        public static int IntField(Rect position, string label, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return IntField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static int IntField(Rect position, GUIContent label, int value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return IntFieldInternal(position, label, value, style);
        }

        internal static int IntFieldInternal(Rect position, int value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            return DoIntField(s_RecycledEditor, IndentedRect(position), new Rect(0f, 0f, 0f, 0f), id, value, kIntFieldFormatString, style, false, (float) CalculateIntDragSensitivity((long) value));
        }

        internal static int IntFieldInternal(Rect position, GUIContent label, int value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            return DoIntField(s_RecycledEditor, rect, position, id, value, kIntFieldFormatString, style, true, (float) CalculateIntDragSensitivity((long) value));
        }

        [ExcludeFromDocs]
        public static int IntPopup(Rect position, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            GUIStyle popup = EditorStyles.popup;
            return IntPopup(position, selectedValue, displayedOptions, optionValues, popup);
        }

        [ExcludeFromDocs]
        public static int IntPopup(Rect position, int selectedValue, GUIContent[] displayedOptions, int[] optionValues)
        {
            GUIStyle popup = EditorStyles.popup;
            return IntPopup(position, selectedValue, displayedOptions, optionValues, popup);
        }

        [ExcludeFromDocs]
        public static void IntPopup(Rect position, SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues)
        {
            GUIContent label = null;
            IntPopup(position, property, displayedOptions, optionValues, label);
        }

        public static int IntPopup(Rect position, int selectedValue, string[] displayedOptions, int[] optionValues, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return IntPopup(position, GUIContent.none, selectedValue, EditorGUIUtility.TempContent(displayedOptions), optionValues, style);
        }

        public static int IntPopup(Rect position, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return IntPopup(position, GUIContent.none, selectedValue, displayedOptions, optionValues, style);
        }

        [ExcludeFromDocs]
        public static int IntPopup(Rect position, string label, int selectedValue, string[] displayedOptions, int[] optionValues)
        {
            GUIStyle popup = EditorStyles.popup;
            return IntPopup(position, label, selectedValue, displayedOptions, optionValues, popup);
        }

        public static void IntPopup(Rect position, SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, [DefaultValue("null")] GUIContent label)
        {
            IntPopupInternal(position, property, displayedOptions, optionValues, label);
        }

        [ExcludeFromDocs]
        public static int IntPopup(Rect position, GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues)
        {
            GUIStyle popup = EditorStyles.popup;
            return IntPopup(position, label, selectedValue, displayedOptions, optionValues, popup);
        }

        public static int IntPopup(Rect position, string label, int selectedValue, string[] displayedOptions, int[] optionValues, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return IntPopupInternal(position, EditorGUIUtility.TempContent(label), selectedValue, EditorGUIUtility.TempContent(displayedOptions), optionValues, style);
        }

        public static int IntPopup(Rect position, GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return IntPopupInternal(position, label, selectedValue, displayedOptions, optionValues, style);
        }

        internal static void IntPopupInternal(Rect position, SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            int num = IntPopupInternal(position, label, property.intValue, displayedOptions, optionValues, EditorStyles.popup);
            if (EndChangeCheck())
            {
                property.intValue = num;
            }
            EndProperty();
        }

        private static int IntPopupInternal(Rect position, GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style)
        {
            int num;
            if (optionValues != null)
            {
                for (num = 0; (num < optionValues.Length) && (selectedValue != optionValues[num]); num++)
                {
                }
            }
            else
            {
                num = selectedValue;
            }
            num = PopupInternal(position, label, num, displayedOptions, style);
            if (optionValues == null)
            {
                return num;
            }
            if ((num >= 0) && (num < optionValues.Length))
            {
                return optionValues[num];
            }
            return selectedValue;
        }

        public static int IntSlider(Rect position, int value, int leftValue, int rightValue)
        {
            int id = GUIUtility.GetControlID(s_SliderHash, EditorGUIUtility.native, position);
            return Mathf.RoundToInt(DoSlider(IndentedRect(position), EditorGUIUtility.DragZoneRect(position), id, (float) value, (float) leftValue, (float) rightValue, kIntFieldFormatString));
        }

        public static void IntSlider(Rect position, SerializedProperty property, int leftValue, int rightValue)
        {
            IntSlider(position, property, leftValue, rightValue, property.displayName);
        }

        public static int IntSlider(Rect position, string label, int value, int leftValue, int rightValue)
        {
            return IntSlider(position, EditorGUIUtility.TempContent(label), value, leftValue, rightValue);
        }

        public static void IntSlider(Rect position, SerializedProperty property, int leftValue, int rightValue, string label)
        {
            IntSlider(position, property, leftValue, rightValue, EditorGUIUtility.TempContent(label));
        }

        public static void IntSlider(Rect position, SerializedProperty property, int leftValue, int rightValue, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            int num = IntSlider(position, label, property.intValue, leftValue, rightValue);
            if (EndChangeCheck())
            {
                property.intValue = num;
            }
            EndProperty();
        }

        public static int IntSlider(Rect position, GUIContent label, int value, int leftValue, int rightValue)
        {
            int id = GUIUtility.GetControlID(s_SliderHash, EditorGUIUtility.native, position);
            return Mathf.RoundToInt(DoSlider(PrefixLabel(position, id, label), EditorGUIUtility.DragZoneRect(position), id, (float) value, (float) leftValue, (float) rightValue, kIntFieldFormatString));
        }

        internal static bool IsEditingTextField()
        {
            return RecycledTextEditor.s_ActuallyEditing;
        }

        internal static Event KeyEventField(Rect position, Event evt)
        {
            return DoKeyEventField(position, evt, GUI.skin.textField);
        }

        internal static float Knob(Rect position, Vector2 knobSize, float currentValue, float start, float end, string unit, Color backgroundColor, Color activeColor, bool showValue, int id)
        {
            KnobContext context = new KnobContext(position, knobSize, currentValue, start, end, unit, backgroundColor, activeColor, showValue, id);
            return context.Handle();
        }

        [ExcludeFromDocs]
        public static void LabelField(Rect position, string label)
        {
            GUIStyle style = EditorStyles.label;
            LabelField(position, label, style);
        }

        [ExcludeFromDocs]
        public static void LabelField(Rect position, GUIContent label)
        {
            GUIStyle style = EditorStyles.label;
            LabelField(position, label, style);
        }

        [ExcludeFromDocs]
        public static void LabelField(Rect position, string label, string label2)
        {
            GUIStyle style = EditorStyles.label;
            LabelField(position, label, label2, style);
        }

        public static void LabelField(Rect position, string label, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            LabelField(position, GUIContent.none, EditorGUIUtility.TempContent(label), style);
        }

        [ExcludeFromDocs]
        public static void LabelField(Rect position, GUIContent label, GUIContent label2)
        {
            GUIStyle style = EditorStyles.label;
            LabelField(position, label, label2, style);
        }

        public static void LabelField(Rect position, GUIContent label, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            LabelField(position, GUIContent.none, label, style);
        }

        public static void LabelField(Rect position, string label, string label2, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            LabelField(position, new GUIContent(label), EditorGUIUtility.TempContent(label2), style);
        }

        public static void LabelField(Rect position, GUIContent label, GUIContent label2, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            LabelFieldInternal(position, label, label2, style);
        }

        internal static void LabelFieldInternal(Rect position, GUIContent label, GUIContent label2, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Passive, position);
            position = PrefixLabel(position, id, label);
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(position, label2, id);
            }
        }

        internal static bool LabelHasContent(GUIContent label)
        {
            return ((label == null) || ((label.text != string.Empty) || (label.image != null)));
        }

        [ExcludeFromDocs]
        public static int LayerField(Rect position, int layer)
        {
            GUIStyle popup = EditorStyles.popup;
            return LayerField(position, layer, popup);
        }

        public static int LayerField(Rect position, int layer, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return LayerFieldInternal(position, GUIContent.none, layer, style);
        }

        [ExcludeFromDocs]
        public static int LayerField(Rect position, string label, int layer)
        {
            GUIStyle popup = EditorStyles.popup;
            return LayerField(position, label, layer, popup);
        }

        [ExcludeFromDocs]
        public static int LayerField(Rect position, GUIContent label, int layer)
        {
            GUIStyle popup = EditorStyles.popup;
            return LayerField(position, label, layer, popup);
        }

        public static int LayerField(Rect position, string label, int layer, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return LayerFieldInternal(position, EditorGUIUtility.TempContent(label), layer, style);
        }

        public static int LayerField(Rect position, GUIContent label, int layer, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return LayerFieldInternal(position, label, layer, style);
        }

        internal static int LayerFieldInternal(Rect position, GUIContent label, int layer, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_TagFieldHash, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            Event current = Event.current;
            bool changed = GUI.changed;
            int selectedValueForControl = PopupCallbackInfo.GetSelectedValueForControl(id, -1);
            if (selectedValueForControl != -1)
            {
                if (selectedValueForControl >= InternalEditorUtility.layers.Length)
                {
                    ((TagManager) EditorApplication.tagManager).m_DefaultExpandedFoldout = "Layers";
                    Selection.activeObject = EditorApplication.tagManager;
                    GUI.changed = changed;
                }
                else
                {
                    int num3 = 0;
                    for (int j = 0; j < 0x20; j++)
                    {
                        if (InternalEditorUtility.GetLayerName(j).Length != 0)
                        {
                            if (num3 == selectedValueForControl)
                            {
                                layer = j;
                                break;
                            }
                            num3++;
                        }
                    }
                }
            }
            if (((current.type != EventType.MouseDown) || !position.Contains(current.mousePosition)) && !current.MainActionKeyForControl(id))
            {
                if (current.type == EventType.Repaint)
                {
                    style.Draw(position, EditorGUIUtility.TempContent(InternalEditorUtility.GetLayerName(layer)), id, false);
                }
                return layer;
            }
            int selected = 0;
            for (int i = 0; i < 0x20; i++)
            {
                if (InternalEditorUtility.GetLayerName(i).Length != 0)
                {
                    if (i == layer)
                    {
                        break;
                    }
                    selected++;
                }
            }
            string[] layers = InternalEditorUtility.layers;
            ArrayUtility.Add<string>(ref layers, string.Empty);
            ArrayUtility.Add<string>(ref layers, "Add Layer...");
            DoPopup(position, id, selected, EditorGUIUtility.TempContent(layers), style);
            Event.current.Use();
            return layer;
        }

        internal static void LayerMaskField(Rect position, SerializedProperty property, GUIContent label)
        {
            LayerMaskField(position, property, label, EditorStyles.layerMaskField);
        }

        internal static void LayerMaskField(Rect position, SerializedProperty property, GUIContent label, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_LayerMaskField, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            Event current = Event.current;
            if (current.type == EventType.Repaint)
            {
                if (showMixedValue)
                {
                    BeginHandleMixedValueContentColor();
                    style.Draw(position, s_MixedValueContent, id, false);
                    EndHandleMixedValueContentColor();
                }
                else
                {
                    style.Draw(position, EditorGUIUtility.TempContent(property.layerMaskStringValue), id, false);
                }
            }
            else if (((current.type == EventType.MouseDown) && position.Contains(current.mousePosition)) || current.MainActionKeyForControl(id))
            {
                SerializedProperty userData = property.serializedObject.FindProperty(property.propertyPath);
                EditorUtility.DisplayCustomMenu(position, property.GetLayerMaskNames(), !property.hasMultipleDifferentValues ? property.GetLayerMaskSelectedIndex() : new int[0], new EditorUtility.SelectMenuItemFunction(EditorGUI.SetLayerMaskValueDelegate), userData);
                Event.current.Use();
            }
        }

        [ExcludeFromDocs]
        public static long LongField(Rect position, long value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return LongField(position, value, numberField);
        }

        public static long LongField(Rect position, long value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return LongFieldInternal(position, value, style);
        }

        [ExcludeFromDocs]
        public static long LongField(Rect position, string label, long value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return LongField(position, label, value, numberField);
        }

        [ExcludeFromDocs]
        public static long LongField(Rect position, GUIContent label, long value)
        {
            GUIStyle numberField = EditorStyles.numberField;
            return LongField(position, label, value, numberField);
        }

        public static long LongField(Rect position, string label, long value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return LongField(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static long LongField(Rect position, GUIContent label, long value, [DefaultValue("EditorStyles.numberField")] GUIStyle style)
        {
            return LongFieldInternal(position, label, value, style);
        }

        internal static long LongFieldInternal(Rect position, long value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            return DoLongField(s_RecycledEditor, IndentedRect(position), new Rect(0f, 0f, 0f, 0f), id, value, kIntFieldFormatString, style, false, (double) CalculateIntDragSensitivity(value));
        }

        internal static long LongFieldInternal(Rect position, GUIContent label, long value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect rect = PrefixLabel(position, id, label);
            position.xMax = rect.x;
            return DoLongField(s_RecycledEditor, rect, position, id, value, kIntFieldFormatString, style, true, (double) CalculateIntDragSensitivity(value));
        }

        [ExcludeFromDocs]
        public static int MaskField(Rect position, int mask, string[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return MaskField(position, mask, displayedOptions, popup);
        }

        public static int MaskField(Rect position, int mask, string[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return MaskFieldInternal(position, mask, displayedOptions, style);
        }

        [ExcludeFromDocs]
        public static int MaskField(Rect position, string label, int mask, string[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return MaskField(position, label, mask, displayedOptions, popup);
        }

        [ExcludeFromDocs]
        public static int MaskField(Rect position, GUIContent label, int mask, string[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return MaskField(position, label, mask, displayedOptions, popup);
        }

        public static int MaskField(Rect position, string label, int mask, string[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return MaskFieldInternal(position, GUIContent.Temp(label), mask, displayedOptions, style);
        }

        public static int MaskField(Rect position, GUIContent label, int mask, string[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return MaskFieldInternal(position, label, mask, displayedOptions, style);
        }

        internal static int MaskFieldInternal(Rect position, int mask, string[] displayedOptions, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position);
            return MaskFieldGUI.DoMaskField(IndentedRect(position), controlID, mask, displayedOptions, style);
        }

        internal static int MaskFieldInternal(Rect position, GUIContent label, int mask, string[] displayedOptions, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_MaskField, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            return MaskFieldGUI.DoMaskField(position, id, mask, displayedOptions, style);
        }

        internal static Object MiniThumbnailObjectField(Rect position, GUIContent label, Object obj, Type objType, ObjectFieldValidator validator)
        {
            Rect rect;
            Rect rect2;
            int id = GUIUtility.GetControlID(s_ObjectFieldHash, EditorGUIUtility.native, position);
            GetRectsForMiniThumbnailField(position, out rect, out rect2);
            HandlePrefixLabel(position, rect2, label, id, EditorStyles.label);
            return DoObjectField(rect, rect, id, obj, objType, null, validator, false);
        }

        public static void MinMaxSlider(Rect position, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            DoMinMaxSlider(IndentedRect(position), GUIUtility.GetControlID(s_MinMaxSliderHash, FocusType.Native), ref minValue, ref maxValue, minLimit, maxLimit);
        }

        public static void MinMaxSlider(GUIContent label, Rect position, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            int controlID = GUIUtility.GetControlID(s_MinMaxSliderHash, FocusType.Native);
            DoMinMaxSlider(PrefixLabel(position, controlID, label), controlID, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        internal static Vector2 MouseDeltaReader(Rect position, bool activated)
        {
            int controlID = GUIUtility.GetControlID(s_MouseDeltaReaderHash, FocusType.Passive, position);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((activated && (GUIUtility.hotControl == 0)) && (position.Contains(current.mousePosition) && (current.button == 0)))
                    {
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        s_MouseDeltaReaderLastPos = GUIClip.Unclip(current.mousePosition);
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        break;
                    }
                    Vector2 vector = GUIClip.Unclip(current.mousePosition);
                    Vector2 vector2 = vector - s_MouseDeltaReaderLastPos;
                    s_MouseDeltaReaderLastPos = vector;
                    current.Use();
                    return vector2;
                }
            }
            return Vector2.zero;
        }

        internal static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
        {
            if (!LabelHasContent(label))
            {
                return IndentedRect(totalPosition);
            }
            if (EditorGUIUtility.wideMode)
            {
                Rect rect = new Rect(totalPosition.x + indent, totalPosition.y, EditorGUIUtility.labelWidth - indent, 16f);
                Rect rect2 = totalPosition;
                rect2.xMin += EditorGUIUtility.labelWidth;
                if (columns > 1)
                {
                    rect.width--;
                    rect2.xMin--;
                }
                if (columns == 2)
                {
                    float num = (rect2.width - 4f) / 3f;
                    rect2.xMax -= num + 2f;
                }
                HandlePrefixLabel(totalPosition, rect, label, id);
                return rect2;
            }
            Rect labelPosition = new Rect(totalPosition.x + indent, totalPosition.y, totalPosition.width - indent, 16f);
            Rect rect4 = totalPosition;
            rect4.xMin += indent + 15f;
            rect4.yMin += 16f;
            HandlePrefixLabel(totalPosition, labelPosition, label, id);
            return rect4;
        }

        public static void MultiFloatField(Rect position, GUIContent[] subLabels, float[] values)
        {
            MultiFloatField(position, subLabels, values, 13f);
        }

        internal static void MultiFloatField(Rect position, GUIContent[] subLabels, float[] values, float labelWidth)
        {
            int length = values.Length;
            float num2 = (position.width - ((length - 1) * 2f)) / ((float) length);
            Rect rect = new Rect(position) {
                width = num2
            };
            float num3 = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = FloatField(rect, subLabels[i], values[i]);
                rect.x += num2 + 2f;
            }
            EditorGUIUtility.labelWidth = num3;
            EditorGUI.indentLevel = indentLevel;
        }

        public static void MultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, subLabels.Length);
            position.height = 16f;
            MultiFloatField(position, subLabels, values);
        }

        public static void MultiPropertyField(Rect position, GUIContent[] subLabels, SerializedProperty valuesIterator)
        {
            MultiPropertyField(position, subLabels, valuesIterator, 13f, null);
        }

        public static void MultiPropertyField(Rect position, GUIContent[] subLabels, SerializedProperty valuesIterator, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, subLabels.Length);
            position.height = 16f;
            MultiPropertyField(position, subLabels, valuesIterator);
        }

        internal static void MultiPropertyField(Rect position, GUIContent[] subLabels, SerializedProperty valuesIterator, float labelWidth, bool[] disabledMask)
        {
            int length = subLabels.Length;
            float num2 = (position.width - ((length - 1) * 2f)) / ((float) length);
            Rect rect = new Rect(position) {
                width = num2
            };
            float num3 = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < subLabels.Length; i++)
            {
                if (disabledMask != null)
                {
                    BeginDisabledGroup(disabledMask[i]);
                }
                PropertyField(rect, valuesIterator, subLabels[i]);
                if (disabledMask != null)
                {
                    EndDisabledGroup();
                }
                rect.x += num2 + 2f;
                valuesIterator.NextVisible(false);
            }
            EditorGUIUtility.labelWidth = num3;
            EditorGUI.indentLevel = indentLevel;
        }

        public static void ObjectField(Rect position, SerializedProperty property, GUIContent label)
        {
            ObjectField(position, property, null, label, EditorStyles.objectField);
        }

        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(Rect position, Object obj, Type objType)
        {
            int id = GUIUtility.GetControlID(s_ObjectFieldHash, EditorGUIUtility.native, position);
            return DoObjectField(position, position, id, obj, objType, null, null, true);
        }

        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(Rect position, string label, Object obj, Type objType)
        {
            return ObjectField(position, EditorGUIUtility.TempContent(label), obj, objType, true);
        }

        public static void ObjectField(Rect position, SerializedProperty property, Type objType, GUIContent label)
        {
            ObjectField(position, property, objType, label, EditorStyles.objectField);
        }

        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(Rect position, GUIContent label, Object obj, Type objType)
        {
            return ObjectField(position, label, obj, objType, true);
        }

        public static Object ObjectField(Rect position, Object obj, Type objType, bool allowSceneObjects)
        {
            int id = GUIUtility.GetControlID(s_ObjectFieldHash, EditorGUIUtility.native, position);
            return DoObjectField(IndentedRect(position), IndentedRect(position), id, obj, objType, null, null, allowSceneObjects);
        }

        public static Object ObjectField(Rect position, string label, Object obj, Type objType, bool allowSceneObjects)
        {
            return ObjectField(position, EditorGUIUtility.TempContent(label), obj, objType, allowSceneObjects);
        }

        internal static void ObjectField(Rect position, SerializedProperty property, Type objType, GUIContent label, GUIStyle style)
        {
            label = BeginProperty(position, label, property);
            ObjectFieldInternal(position, property, objType, label, style);
            EndProperty();
        }

        public static Object ObjectField(Rect position, GUIContent label, Object obj, Type objType, bool allowSceneObjects)
        {
            int id = GUIUtility.GetControlID(s_ObjectFieldHash, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            if (EditorGUIUtility.HasObjectThumbnail(objType) && (position.height > 16f))
            {
                float num2 = Mathf.Min(position.width, position.height);
                position.height = num2;
                position.xMin = position.xMax - num2;
            }
            return DoObjectField(position, position, id, obj, objType, null, null, allowSceneObjects);
        }

        private static void ObjectFieldInternal(Rect position, SerializedProperty property, Type objType, GUIContent label, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_PPtrHash, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            bool allowSceneObjects = false;
            if (property != null)
            {
                Object targetObject = property.serializedObject.targetObject;
                if ((targetObject != null) && !EditorUtility.IsPersistent(targetObject))
                {
                    allowSceneObjects = true;
                }
            }
            DoObjectField(position, position, id, null, null, property, null, allowSceneObjects, style);
        }

        internal static void ObjectIconDropDown(Rect position, Object[] targets, bool showLabelIcons, Texture2D nullIcon, SerializedProperty iconProperty)
        {
            if (s_IconTextureInactive == null)
            {
                s_IconTextureInactive = (Material) EditorGUIUtility.LoadRequired("Inspectors/InactiveGUI.mat");
            }
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D texture = null;
                if (!iconProperty.hasMultipleDifferentValues)
                {
                    texture = AssetPreview.GetMiniThumbnail(targets[0]);
                }
                if (texture == null)
                {
                    texture = nullIcon;
                }
                Vector2 vector = new Vector2(position.width, position.height);
                if (texture != null)
                {
                    vector.x = Mathf.Min((float) texture.width, vector.x);
                    vector.y = Mathf.Min((float) texture.height, vector.y);
                }
                Rect screenRect = new Rect((position.x + (position.width / 2f)) - (vector.x / 2f), (position.y + (position.height / 2f)) - (vector.y / 2f), vector.x, vector.y);
                GameObject obj2 = targets[0] as GameObject;
                if ((obj2 != null) && (!EditorUtility.IsPersistent(targets[0]) && (!obj2.activeSelf || !obj2.activeInHierarchy)))
                {
                    Graphics.DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, new Color(0.5f, 0.5f, 0.5f, 1f), s_IconTextureInactive);
                }
                else
                {
                    GUI.DrawTexture(screenRect, texture);
                }
                if (ValidTargetForIconSelection(targets))
                {
                    if (s_IconDropDown == null)
                    {
                        s_IconDropDown = EditorGUIUtility.IconContent("Icon Dropdown");
                    }
                    float x = Mathf.Max((float) (position.x + 2f), (float) (screenRect.x - 6f));
                    GUIStyle.none.Draw(new Rect(x, screenRect.yMax - (screenRect.height * 0.2f), 13f, 8f), s_IconDropDown, false, false, false, false);
                }
            }
            if ((ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none) && ValidTargetForIconSelection(targets)) && IconSelector.ShowAtPosition(targets[0], position, showLabelIcons))
            {
                GUIUtility.ExitGUI();
            }
        }

        internal static float OffsetKnob(Rect position, float currentValue, float start, float end, float median, string unit, Color backgroundColor, Color activeColor, GUIStyle knob, int id)
        {
            return 0f;
        }

        [ExcludeFromDocs]
        public static string PasswordField(Rect position, string password)
        {
            GUIStyle textField = EditorStyles.textField;
            return PasswordField(position, password, textField);
        }

        [ExcludeFromDocs]
        public static string PasswordField(Rect position, string label, string password)
        {
            GUIStyle textField = EditorStyles.textField;
            return PasswordField(position, label, password, textField);
        }

        public static string PasswordField(Rect position, string password, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return PasswordFieldInternal(position, password, style);
        }

        [ExcludeFromDocs]
        public static string PasswordField(Rect position, GUIContent label, string password)
        {
            GUIStyle textField = EditorStyles.textField;
            return PasswordField(position, label, password, textField);
        }

        public static string PasswordField(Rect position, string label, string password, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return PasswordField(position, EditorGUIUtility.TempContent(label), password, style);
        }

        public static string PasswordField(Rect position, GUIContent label, string password, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return PasswordFieldInternal(position, label, password, style);
        }

        internal static string PasswordFieldInternal(Rect position, string password, GUIStyle style)
        {
            return DoPasswordField(GUIUtility.GetControlID(s_PasswordFieldHash, FocusType.Keyboard, position), IndentedRect(position), password, style);
        }

        internal static string PasswordFieldInternal(Rect position, GUIContent label, string password, GUIStyle style)
        {
            return DoPasswordField(GUIUtility.GetControlID(s_PasswordFieldHash, FocusType.Keyboard, position), position, label, password, style);
        }

        [ExcludeFromDocs]
        public static int Popup(Rect position, int selectedIndex, string[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return Popup(position, selectedIndex, displayedOptions, popup);
        }

        [ExcludeFromDocs]
        public static int Popup(Rect position, int selectedIndex, GUIContent[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return Popup(position, selectedIndex, displayedOptions, popup);
        }

        private static void Popup(Rect position, SerializedProperty property, GUIContent label)
        {
            BeginChangeCheck();
            int num = Popup(position, label, !property.hasMultipleDifferentValues ? property.enumValueIndex : -1, EditorGUIUtility.TempContent(property.enumDisplayNames));
            if (EndChangeCheck())
            {
                property.enumValueIndex = num;
            }
        }

        public static int Popup(Rect position, int selectedIndex, string[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return DoPopup(IndentedRect(position), GUIUtility.GetControlID(s_PopupHash, EditorGUIUtility.native, position), selectedIndex, EditorGUIUtility.TempContent(displayedOptions), style);
        }

        public static int Popup(Rect position, int selectedIndex, GUIContent[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return DoPopup(IndentedRect(position), GUIUtility.GetControlID(s_PopupHash, EditorGUIUtility.native, position), selectedIndex, displayedOptions, style);
        }

        [ExcludeFromDocs]
        public static int Popup(Rect position, string label, int selectedIndex, string[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return Popup(position, label, selectedIndex, displayedOptions, popup);
        }

        internal static void Popup(Rect position, SerializedProperty property, GUIContent[] displayedOptions, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            int num = Popup(position, label, !property.hasMultipleDifferentValues ? property.intValue : -1, displayedOptions);
            if (EndChangeCheck())
            {
                property.intValue = num;
            }
            EndProperty();
        }

        [ExcludeFromDocs]
        public static int Popup(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions)
        {
            GUIStyle popup = EditorStyles.popup;
            return Popup(position, label, selectedIndex, displayedOptions, popup);
        }

        public static int Popup(Rect position, string label, int selectedIndex, string[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return PopupInternal(position, EditorGUIUtility.TempContent(label), selectedIndex, EditorGUIUtility.TempContent(displayedOptions), style);
        }

        public static int Popup(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return PopupInternal(position, label, selectedIndex, displayedOptions, style);
        }

        private static int PopupInternal(Rect position, string label, int selectedIndex, string[] displayedOptions, GUIStyle style)
        {
            return PopupInternal(position, EditorGUIUtility.TempContent(label), selectedIndex, EditorGUIUtility.TempContent(displayedOptions), style);
        }

        private static int PopupInternal(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(s_PopupHash, EditorGUIUtility.native, position);
            return DoPopup(PrefixLabel(position, controlID, label), controlID, selectedIndex, displayedOptions, style);
        }

        internal static float PowerSlider(Rect position, string label, float sliderValue, float leftValue, float rightValue, float power)
        {
            return PowerSlider(position, EditorGUIUtility.TempContent(label), sliderValue, leftValue, rightValue, power);
        }

        internal static float PowerSlider(Rect position, GUIContent label, float sliderValue, float leftValue, float rightValue, float power)
        {
            int id = GUIUtility.GetControlID(s_SliderHash, EditorGUIUtility.native, position);
            Rect rect = PrefixLabel(position, id, label);
            Rect dragZonePosition = !LabelHasContent(label) ? new Rect() : EditorGUIUtility.DragZoneRect(position);
            return DoSlider(rect, dragZonePosition, id, sliderValue, leftValue, rightValue, kFloatFieldFormatString, power);
        }

        private static float PowPreserveSign(float f, float p)
        {
            float num = Mathf.Pow(Mathf.Abs(f), p);
            return ((f >= 0f) ? num : -num);
        }

        public static Rect PrefixLabel(Rect totalPosition, GUIContent label)
        {
            return PrefixLabel(totalPosition, 0, label, EditorStyles.label);
        }

        public static Rect PrefixLabel(Rect totalPosition, int id, GUIContent label)
        {
            return PrefixLabel(totalPosition, id, label, EditorStyles.label);
        }

        public static Rect PrefixLabel(Rect totalPosition, GUIContent label, GUIStyle style)
        {
            return PrefixLabel(totalPosition, 0, label, style);
        }

        public static Rect PrefixLabel(Rect totalPosition, int id, GUIContent label, GUIStyle style)
        {
            if (!LabelHasContent(label))
            {
                return IndentedRect(totalPosition);
            }
            Rect labelPosition = new Rect(totalPosition.x + indent, totalPosition.y, EditorGUIUtility.labelWidth - indent, 16f);
            Rect rect2 = new Rect(totalPosition.x + EditorGUIUtility.labelWidth, totalPosition.y, totalPosition.width - EditorGUIUtility.labelWidth, totalPosition.height);
            HandlePrefixLabel(totalPosition, labelPosition, label, id, style);
            return rect2;
        }

        public static void ProgressBar(Rect position, float value, string text)
        {
            int controlID = GUIUtility.GetControlID(s_ProgressBarHash, EditorGUIUtility.native, position);
            if (Event.current.GetTypeForControl(controlID) == EventType.Repaint)
            {
                EditorStyles.progressBarBack.Draw(position, false, false, false, false);
                Rect rect = new Rect(position);
                value = Mathf.Clamp01(value);
                rect.width *= value;
                EditorStyles.progressBarBar.Draw(rect, false, false, false, false);
                EditorStyles.progressBarText.Draw(position, text, false, false, false, false);
            }
        }

        internal static void PropertiesField(Rect position, GUIContent label, SerializedProperty[] properties, GUIContent[] propertyLabels, float propertyLabelsWidth)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            Rect rect = PrefixLabel(position, id, label);
            rect.height = 16f;
            float labelWidth = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = propertyLabelsWidth;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyField(rect, properties[i], propertyLabels[i]);
                rect.y += 16f;
            }
            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        [ExcludeFromDocs]
        public static bool PropertyField(Rect position, SerializedProperty property)
        {
            bool includeChildren = false;
            return PropertyField(position, property, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, [DefaultValue("false")] bool includeChildren)
        {
            return PropertyFieldInternal(position, property, null, includeChildren);
        }

        [ExcludeFromDocs]
        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            bool includeChildren = false;
            return PropertyField(position, property, label, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label, [DefaultValue("false")] bool includeChildren)
        {
            return PropertyFieldInternal(position, property, label, includeChildren);
        }

        internal static bool PropertyFieldInternal(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return ScriptAttributeUtility.GetHandler(property).OnGUI(position, property, label, includeChildren);
        }

        public static Rect RectField(Rect position, Rect value)
        {
            position.height = 16f;
            s_Vector2Floats[0] = value.x;
            s_Vector2Floats[1] = value.y;
            BeginChangeCheck();
            MultiFloatField(position, s_XYLabels, s_Vector2Floats);
            if (EndChangeCheck())
            {
                value.x = s_Vector2Floats[0];
                value.y = s_Vector2Floats[1];
            }
            position.y += 16f;
            s_Vector2Floats[0] = value.width;
            s_Vector2Floats[1] = value.height;
            BeginChangeCheck();
            MultiFloatField(position, s_WHLabels, s_Vector2Floats);
            if (EndChangeCheck())
            {
                value.width = s_Vector2Floats[0];
                value.height = s_Vector2Floats[1];
            }
            return value;
        }

        public static Rect RectField(Rect position, string label, Rect value)
        {
            return RectField(position, EditorGUIUtility.TempContent(label), value);
        }

        private static void RectField(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 2);
            position.height = 16f;
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            MultiPropertyField(position, s_XYLabels, valuesIterator);
            position.y += 16f;
            MultiPropertyField(position, s_WHLabels, valuesIterator);
        }

        public static Rect RectField(Rect position, GUIContent label, Rect value)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 2);
            position.height = 16f;
            return RectField(position, value);
        }

        internal static string ScrollableTextAreaInternal(Rect position, string text, ref Vector2 scrollPosition, GUIStyle style)
        {
            bool flag;
            if (Event.current.type == EventType.Layout)
            {
                return text;
            }
            int id = GUIUtility.GetControlID(s_TextAreaHash, FocusType.Keyboard, position);
            float height = style.CalcHeight(GUIContent.Temp(text), position.width);
            Rect rect = new Rect(0f, 0f, position.width, height);
            Vector2 contentOffset = style.contentOffset;
            if (position.height < rect.height)
            {
                Rect rect2 = position;
                rect2.width = GUI.skin.verticalScrollbar.fixedWidth;
                rect2.height -= 2f;
                rect2.y++;
                rect2.x = (position.x + position.width) - rect2.width;
                position.width -= rect2.width;
                height = style.CalcHeight(GUIContent.Temp(text), position.width);
                rect = new Rect(0f, 0f, position.width, height);
                if (position.Contains(Event.current.mousePosition) && (Event.current.type == EventType.ScrollWheel))
                {
                    float num4 = scrollPosition.y + (Event.current.delta.y * 10f);
                    scrollPosition.y = Mathf.Clamp(num4, 0f, rect.height);
                    Event.current.Use();
                }
                scrollPosition.y = GUI.VerticalScrollbar(rect2, scrollPosition.y, position.height, 0f, rect.height);
                if (!s_RecycledEditor.IsEditingControl(id))
                {
                    style.contentOffset -= scrollPosition;
                    style.Internal_clipOffset = scrollPosition;
                }
                else
                {
                    s_RecycledEditor.scrollOffset.y = scrollPosition.y;
                }
            }
            EventType type = Event.current.type;
            string str = DoTextField(s_RecycledEditor, id, IndentedRect(position), text, style, null, out flag, false, true, false);
            if (type != Event.current.type)
            {
                scrollPosition = s_RecycledEditor.scrollOffset;
            }
            style.contentOffset = contentOffset;
            style.Internal_clipOffset = Vector2.zero;
            return str;
        }

        internal static string SearchField(Rect position, string text)
        {
            bool flag;
            int id = GUIUtility.GetControlID(s_SearchFieldHash, FocusType.Keyboard, position);
            Rect rect = position;
            rect.width -= 15f;
            text = DoTextField(s_RecycledEditor, id, rect, text, EditorStyles.searchField, null, out flag, false, false, false);
            Rect rect2 = position;
            rect2.x += position.width - 15f;
            rect2.width = 15f;
            if (GUI.Button(rect2, GUIContent.none, !(text != string.Empty) ? EditorStyles.searchFieldCancelButtonEmpty : EditorStyles.searchFieldCancelButton) && (text != string.Empty))
            {
                s_RecycledEditor.text = text = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            return text;
        }

        [ExcludeFromDocs]
        public static void SelectableLabel(Rect position, string text)
        {
            GUIStyle label = EditorStyles.label;
            SelectableLabel(position, text, label);
        }

        public static void SelectableLabel(Rect position, string text, [DefaultValue("EditorStyles.label")] GUIStyle style)
        {
            SelectableLabelInternal(position, text, style);
        }

        internal static void SelectableLabelInternal(Rect position, string text, GUIStyle style)
        {
            bool flag;
            int controlID = GUIUtility.GetControlID(s_SelectableLabelHash, FocusType.Keyboard, position);
            Event current = Event.current;
            if ((GUIUtility.keyboardControl == controlID) && (current.GetTypeForControl(controlID) == EventType.KeyDown))
            {
                KeyCode keyCode = current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                    case KeyCode.RightArrow:
                    case KeyCode.LeftArrow:
                    case KeyCode.Home:
                    case KeyCode.End:
                    case KeyCode.PageUp:
                    case KeyCode.PageDown:
                        goto Label_00A0;
                }
                if (keyCode == KeyCode.Space)
                {
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
                else if (current.character != '\t')
                {
                    current.Use();
                }
            }
        Label_00A0:
            if (((current.type == EventType.ExecuteCommand) && ((current.commandName == "Paste") || (current.commandName == "Cut"))) && (GUIUtility.keyboardControl == controlID))
            {
                current.Use();
            }
            Color cursorColor = GUI.skin.settings.cursorColor;
            GUI.skin.settings.cursorColor = new Color(0f, 0f, 0f, 0f);
            RecycledTextEditor.s_AllowContextCutOrPaste = false;
            text = DoTextField(s_RecycledEditor, controlID, IndentedRect(position), text, style, string.Empty, out flag, false, true, false);
            GUI.skin.settings.cursorColor = cursorColor;
        }

        private static void SetCurveEditorWindowCurve(AnimationCurve value, SerializedProperty property, Color color)
        {
            if (property != null)
            {
                CurveEditorWindow.curve = !property.hasMultipleDifferentValues ? property.animationCurveValue : new AnimationCurve();
            }
            else
            {
                CurveEditorWindow.curve = value;
            }
            CurveEditorWindow.color = color;
        }

        private static void SetExpandedRecurse(SerializedProperty property, bool expanded)
        {
            SerializedProperty property2 = property.Copy();
            property2.isExpanded = expanded;
            int depth = property2.depth;
            while (property2.NextVisible(true) && (property2.depth > depth))
            {
                if (property2.hasVisibleChildren)
                {
                    property2.isExpanded = expanded;
                }
            }
        }

        internal static void SetLayerMaskValueDelegate(object userData, string[] options, int selected)
        {
            SerializedProperty property = (SerializedProperty) userData;
            property.ToggleLayerMaskAtIndex(selected);
            property.serializedObject.ApplyModifiedProperties();
        }

        private static void ShowCurvePopup(GUIView viewToUpdate, Rect ranges)
        {
            CurveEditorSettings settings = new CurveEditorSettings();
            if (((ranges.width > 0f) && (ranges.height > 0f)) && ((ranges.width != float.PositiveInfinity) && (ranges.height != float.PositiveInfinity)))
            {
                settings.hRangeMin = ranges.xMin;
                settings.hRangeMax = ranges.xMax;
                settings.vRangeMin = ranges.yMin;
                settings.vRangeMax = ranges.yMax;
            }
            CurveEditorWindow.instance.Show(GUIView.current, settings);
        }

        internal static void ShowRepaints()
        {
            if (Unsupported.IsDeveloperBuild())
            {
                Color backgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(Random.value, Random.value, Random.value, Random.value);
                GUI.Label(new Rect(0f, 0f, 10f, 10f), string.Empty, GUI.skin.button);
                GUI.backgroundColor = backgroundColor;
            }
        }

        private static void ShowTextEditorPopupMenu()
        {
            GenericMenu menu = new GenericMenu();
            if (s_RecycledEditor.hasSelection && !s_RecycledEditor.isPasswordField)
            {
                if (RecycledTextEditor.s_AllowContextCutOrPaste)
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Cut"), false, new GenericMenu.MenuFunction(new PopupMenuEvent("Cut", GUIView.current).SendEvent));
                }
                menu.AddItem(EditorGUIUtility.TextContent("Copy"), false, new GenericMenu.MenuFunction(new PopupMenuEvent("Copy", GUIView.current).SendEvent));
            }
            else
            {
                if (RecycledTextEditor.s_AllowContextCutOrPaste)
                {
                    menu.AddDisabledItem(EditorGUIUtility.TextContent("Cut"));
                }
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Copy"));
            }
            if (s_RecycledEditor.CanPaste() && RecycledTextEditor.s_AllowContextCutOrPaste)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Paste"), false, new GenericMenu.MenuFunction(new PopupMenuEvent("Paste", GUIView.current).SendEvent));
            }
            menu.ShowAsContext();
        }

        public static float Slider(Rect position, float value, float leftValue, float rightValue)
        {
            int id = GUIUtility.GetControlID(s_SliderHash, EditorGUIUtility.native, position);
            return DoSlider(IndentedRect(position), EditorGUIUtility.DragZoneRect(position), id, value, leftValue, rightValue, kFloatFieldFormatString);
        }

        public static void Slider(Rect position, SerializedProperty property, float leftValue, float rightValue)
        {
            Slider(position, property, leftValue, rightValue, property.displayName);
        }

        public static float Slider(Rect position, string label, float value, float leftValue, float rightValue)
        {
            return Slider(position, EditorGUIUtility.TempContent(label), value, leftValue, rightValue);
        }

        public static void Slider(Rect position, SerializedProperty property, float leftValue, float rightValue, string label)
        {
            Slider(position, property, leftValue, rightValue, EditorGUIUtility.TempContent(label));
        }

        public static void Slider(Rect position, SerializedProperty property, float leftValue, float rightValue, GUIContent label)
        {
            label = BeginProperty(position, label, property);
            BeginChangeCheck();
            float num = Slider(position, label, property.floatValue, leftValue, rightValue);
            if (EndChangeCheck())
            {
                property.floatValue = num;
            }
            EndProperty();
        }

        public static float Slider(Rect position, GUIContent label, float value, float leftValue, float rightValue)
        {
            return PowerSlider(position, label, value, leftValue, rightValue, 1f);
        }

        internal static void SortingLayerField(Rect position, GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
        {
            int id = GUIUtility.GetControlID(s_SortingLayerFieldHash, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label, labelStyle);
            Event current = Event.current;
            int selectedValueForControl = PopupCallbackInfo.GetSelectedValueForControl(id, -1);
            if (selectedValueForControl != -1)
            {
                int[] sortingLayerUniqueIDs = InternalEditorUtility.sortingLayerUniqueIDs;
                if (selectedValueForControl >= sortingLayerUniqueIDs.Length)
                {
                    ((TagManager) EditorApplication.tagManager).m_DefaultExpandedFoldout = "SortingLayers";
                    Selection.activeObject = EditorApplication.tagManager;
                }
                else
                {
                    layerID.intValue = sortingLayerUniqueIDs[selectedValueForControl];
                }
            }
            if (((current.type != EventType.MouseDown) || !position.Contains(current.mousePosition)) && !current.MainActionKeyForControl(id))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIContent mixedValueContent;
                    if (layerID.hasMultipleDifferentValues)
                    {
                        mixedValueContent = EditorGUI.mixedValueContent;
                    }
                    else
                    {
                        mixedValueContent = EditorGUIUtility.TempContent(InternalEditorUtility.GetSortingLayerNameFromUniqueID(layerID.intValue));
                    }
                    showMixedValue = layerID.hasMultipleDifferentValues;
                    BeginHandleMixedValueContentColor();
                    style.Draw(position, mixedValueContent, id, false);
                    EndHandleMixedValueContentColor();
                    showMixedValue = false;
                }
            }
            else
            {
                int index = 0;
                int[] numArray2 = InternalEditorUtility.sortingLayerUniqueIDs;
                string[] sortingLayerNames = InternalEditorUtility.sortingLayerNames;
                index = 0;
                while (index < numArray2.Length)
                {
                    if (numArray2[index] == layerID.intValue)
                    {
                        break;
                    }
                    index++;
                }
                ArrayUtility.Add<string>(ref sortingLayerNames, string.Empty);
                ArrayUtility.Add<string>(ref sortingLayerNames, "Add Sorting Layer...");
                DoPopup(position, id, index, EditorGUIUtility.TempContent(sortingLayerNames), style);
            }
        }

        [ExcludeFromDocs]
        public static string TagField(Rect position, string tag)
        {
            GUIStyle popup = EditorStyles.popup;
            return TagField(position, tag, popup);
        }

        [ExcludeFromDocs]
        public static string TagField(Rect position, string label, string tag)
        {
            GUIStyle popup = EditorStyles.popup;
            return TagField(position, label, tag, popup);
        }

        public static string TagField(Rect position, string tag, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return TagFieldInternal(position, EditorGUIUtility.TempContent(string.Empty), tag, style);
        }

        [ExcludeFromDocs]
        public static string TagField(Rect position, GUIContent label, string tag)
        {
            GUIStyle popup = EditorStyles.popup;
            return TagField(position, label, tag, popup);
        }

        public static string TagField(Rect position, string label, string tag, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return TagFieldInternal(position, EditorGUIUtility.TempContent(label), tag, style);
        }

        public static string TagField(Rect position, GUIContent label, string tag, [DefaultValue("EditorStyles.popup")] GUIStyle style)
        {
            return TagFieldInternal(position, label, tag, style);
        }

        internal static string TagFieldInternal(Rect position, string tag, GUIStyle style)
        {
            position = IndentedRect(position);
            int controlID = GUIUtility.GetControlID(s_TagFieldHash, EditorGUIUtility.native, position);
            Event current = Event.current;
            int selectedValueForControl = PopupCallbackInfo.GetSelectedValueForControl(controlID, -1);
            if (selectedValueForControl != -1)
            {
                string[] strArray = InternalEditorUtility.tags;
                if (selectedValueForControl >= strArray.Length)
                {
                    ((TagManager) EditorApplication.tagManager).m_DefaultExpandedFoldout = "Tags";
                    Selection.activeObject = EditorApplication.tagManager;
                }
                else
                {
                    tag = strArray[selectedValueForControl];
                }
            }
            if (((current.type != EventType.MouseDown) || !position.Contains(current.mousePosition)) && !current.MainActionKeyForControl(controlID))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    BeginHandleMixedValueContentColor();
                    style.Draw(position, !showMixedValue ? EditorGUIUtility.TempContent(tag) : s_MixedValueContent, controlID, false);
                    EndHandleMixedValueContentColor();
                }
                return tag;
            }
            int index = 0;
            string[] tags = InternalEditorUtility.tags;
            index = 0;
            while (index < tags.Length)
            {
                if (tags[index] == tag)
                {
                    break;
                }
                index++;
            }
            ArrayUtility.Add<string>(ref tags, string.Empty);
            ArrayUtility.Add<string>(ref tags, "Add Tag...");
            DoPopup(position, controlID, index, EditorGUIUtility.TempContent(tags), style);
            return tag;
        }

        internal static string TagFieldInternal(Rect position, GUIContent label, string tag, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_TagFieldHash, EditorGUIUtility.native, position);
            position = PrefixLabel(position, id, label);
            Event current = Event.current;
            int selectedValueForControl = PopupCallbackInfo.GetSelectedValueForControl(id, -1);
            if (selectedValueForControl != -1)
            {
                string[] strArray = InternalEditorUtility.tags;
                if (selectedValueForControl >= strArray.Length)
                {
                    ((TagManager) EditorApplication.tagManager).m_DefaultExpandedFoldout = "Tags";
                    Selection.activeObject = EditorApplication.tagManager;
                }
                else
                {
                    tag = strArray[selectedValueForControl];
                }
            }
            if (((current.type != EventType.MouseDown) || !position.Contains(current.mousePosition)) && !current.MainActionKeyForControl(id))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    style.Draw(position, EditorGUIUtility.TempContent(tag), id, false);
                }
                return tag;
            }
            int index = 0;
            string[] tags = InternalEditorUtility.tags;
            index = 0;
            while (index < tags.Length)
            {
                if (tags[index] == tag)
                {
                    break;
                }
                index++;
            }
            ArrayUtility.Add<string>(ref tags, string.Empty);
            ArrayUtility.Add<string>(ref tags, "Add Tag...");
            DoPopup(position, id, index, EditorGUIUtility.TempContent(tags), style);
            return tag;
        }

        internal static void TargetChoiceField(Rect position, SerializedProperty property, GUIContent label)
        {
            TargetChoiceField(position, property, label, new TargetChoiceHandler.TargetChoiceMenuFunction(TargetChoiceHandler.SetToValueOfTarget));
        }

        internal static void TargetChoiceField(Rect position, SerializedProperty property, GUIContent label, TargetChoiceHandler.TargetChoiceMenuFunction func)
        {
            BeginProperty(position, label, property);
            position = PrefixLabel(position, 0, label);
            BeginHandleMixedValueContentColor();
            if (GUI.Button(position, mixedValueContent, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                TargetChoiceHandler.AddSetToValueOfTargetMenuItems(menu, property, func);
                menu.DropDown(position);
            }
            EndHandleMixedValueContentColor();
            EndProperty();
        }

        [ExcludeFromDocs]
        public static string TextArea(Rect position, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return TextArea(position, text, textField);
        }

        public static string TextArea(Rect position, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return TextAreaInternal(position, text, style);
        }

        internal static string TextAreaInternal(Rect position, string text, GUIStyle style)
        {
            bool flag;
            int id = GUIUtility.GetControlID(s_TextAreaHash, FocusType.Keyboard, position);
            text = DoTextField(s_RecycledEditor, id, IndentedRect(position), text, style, null, out flag, false, true, false);
            return text;
        }

        [ExcludeFromDocs]
        public static string TextField(Rect position, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return TextField(position, text, textField);
        }

        [ExcludeFromDocs]
        public static string TextField(Rect position, string label, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return TextField(position, label, text, textField);
        }

        public static string TextField(Rect position, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return TextFieldInternal(position, text, style);
        }

        [ExcludeFromDocs]
        public static string TextField(Rect position, GUIContent label, string text)
        {
            GUIStyle textField = EditorStyles.textField;
            return TextField(position, label, text, textField);
        }

        public static string TextField(Rect position, string label, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return TextField(position, EditorGUIUtility.TempContent(label), text, style);
        }

        public static string TextField(Rect position, GUIContent label, string text, [DefaultValue("EditorStyles.textField")] GUIStyle style)
        {
            return TextFieldInternal(position, label, text, style);
        }

        internal static string TextFieldDropDown(Rect position, string text, string[] dropDownElement)
        {
            return TextFieldDropDown(position, GUIContent.none, text, dropDownElement);
        }

        internal static string TextFieldDropDown(Rect position, GUIContent label, string text, string[] dropDownElement)
        {
            int id = GUIUtility.GetControlID(s_TextFieldDropDownHash, FocusType.Keyboard, position);
            return DoTextFieldDropDown(PrefixLabel(position, id, label), id, text, dropDownElement, false);
        }

        internal static string TextFieldInternal(Rect position, string text, GUIStyle style)
        {
            bool flag;
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, position);
            text = DoTextField(s_RecycledEditor, id, IndentedRect(position), text, style, null, out flag, false, false, false);
            return text;
        }

        internal static string TextFieldInternal(Rect position, GUIContent label, string text, GUIStyle style)
        {
            bool flag;
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, position);
            text = DoTextField(s_RecycledEditor, id, PrefixLabel(position, id, label), text, style, null, out flag, false, false, false);
            return text;
        }

        public static bool Toggle(Rect position, bool value)
        {
            int id = GUIUtility.GetControlID(s_ToggleHash, EditorGUIUtility.native, position);
            return EditorGUIInternal.DoToggleForward(IndentedRect(position), id, value, GUIContent.none, EditorStyles.toggle);
        }

        public static bool Toggle(Rect position, bool value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_ToggleHash, EditorGUIUtility.native, position);
            return EditorGUIInternal.DoToggleForward(position, id, value, GUIContent.none, style);
        }

        public static bool Toggle(Rect position, string label, bool value)
        {
            return Toggle(position, EditorGUIUtility.TempContent(label), value);
        }

        public static bool Toggle(Rect position, GUIContent label, bool value)
        {
            int id = GUIUtility.GetControlID(s_ToggleHash, EditorGUIUtility.native, position);
            return EditorGUIInternal.DoToggleForward(PrefixLabel(position, id, label), id, value, GUIContent.none, EditorStyles.toggle);
        }

        public static bool Toggle(Rect position, string label, bool value, GUIStyle style)
        {
            return Toggle(position, EditorGUIUtility.TempContent(label), value, style);
        }

        public static bool Toggle(Rect position, GUIContent label, bool value, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(s_ToggleHash, EditorGUIUtility.native, position);
            return EditorGUIInternal.DoToggleForward(PrefixLabel(position, id, label), id, value, GUIContent.none, style);
        }

        [ExcludeFromDocs]
        public static bool ToggleLeft(Rect position, string label, bool value)
        {
            GUIStyle labelStyle = EditorStyles.label;
            return ToggleLeft(position, label, value, labelStyle);
        }

        [ExcludeFromDocs]
        public static bool ToggleLeft(Rect position, GUIContent label, bool value)
        {
            GUIStyle labelStyle = EditorStyles.label;
            return ToggleLeft(position, label, value, labelStyle);
        }

        public static bool ToggleLeft(Rect position, string label, bool value, [DefaultValue("EditorStyles.label")] GUIStyle labelStyle)
        {
            return ToggleLeft(position, EditorGUIUtility.TempContent(label), value, labelStyle);
        }

        public static bool ToggleLeft(Rect position, GUIContent label, bool value, [DefaultValue("EditorStyles.label")] GUIStyle labelStyle)
        {
            return ToggleLeftInternal(position, label, value, labelStyle);
        }

        internal static bool ToggleLeftInternal(Rect position, GUIContent label, bool value, GUIStyle labelStyle)
        {
            int id = GUIUtility.GetControlID(s_ToggleHash, EditorGUIUtility.native, position);
            Rect rect = IndentedRect(position);
            Rect labelPosition = IndentedRect(position);
            labelPosition.xMin += 13f;
            HandlePrefixLabel(position, labelPosition, label, id, labelStyle);
            return EditorGUIInternal.DoToggleForward(rect, id, value, GUIContent.none, EditorStyles.toggle);
        }

        internal static bool ToggleTitlebar(Rect position, GUIContent label, bool foldout, ref bool toggleValue)
        {
            int controlID = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
            GUIStyle inspectorTitlebar = EditorStyles.inspectorTitlebar;
            GUIStyle inspectorTitlebarText = EditorStyles.inspectorTitlebarText;
            Rect rect = new Rect(position.x + inspectorTitlebar.padding.left, position.y + inspectorTitlebar.padding.top, 16f, 16f);
            Rect rect2 = new Rect(rect.xMax + 2f, rect.y, 200f, 16f);
            int id = GUIUtility.GetControlID(s_TitlebarHash, EditorGUIUtility.native, position);
            toggleValue = EditorGUIInternal.DoToggleForward(rect, id, toggleValue, GUIContent.none, EditorStyles.toggle);
            if (Event.current.type == EventType.Repaint)
            {
                inspectorTitlebar.Draw(position, GUIContent.none, controlID, foldout);
                position = inspectorTitlebar.padding.Remove(position);
                inspectorTitlebarText.Draw(rect2, label, controlID, foldout);
            }
            return EditorGUIInternal.DoToggleForward(IndentedRect(position), controlID, foldout, GUIContent.none, GUIStyle.none);
        }

        internal static string ToolbarSearchField(int id, Rect position, string text, bool showWithPopupArrow)
        {
            bool flag;
            Rect rect = position;
            rect.width -= 14f;
            text = DoTextField(s_RecycledEditor, id, rect, text, !showWithPopupArrow ? EditorStyles.toolbarSearchField : EditorStyles.toolbarSearchFieldPopup, null, out flag, false, false, false);
            Rect rect2 = position;
            rect2.x += position.width - 14f;
            rect2.width = 14f;
            if (GUI.Button(rect2, GUIContent.none, !(text != string.Empty) ? EditorStyles.toolbarSearchFieldCancelButtonEmpty : EditorStyles.toolbarSearchFieldCancelButton) && (text != string.Empty))
            {
                s_RecycledEditor.text = text = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            return text;
        }

        internal static string ToolbarSearchField(Rect position, string[] searchModes, ref int searchMode, string text)
        {
            return ToolbarSearchField(GUIUtility.GetControlID(s_SearchFieldHash, FocusType.Keyboard, position), position, searchModes, ref searchMode, text);
        }

        internal static string ToolbarSearchField(int id, Rect position, string[] searchModes, ref int searchMode, string text)
        {
            bool showWithPopupArrow = searchModes != null;
            if (showWithPopupArrow)
            {
                searchMode = PopupCallbackInfo.GetSelectedValueForControl(id, searchMode);
                Rect rect = position;
                rect.width = 20f;
                if ((Event.current.type == EventType.MouseDown) && rect.Contains(Event.current.mousePosition))
                {
                    PopupCallbackInfo.instance = new PopupCallbackInfo(id);
                    EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(searchModes), searchMode, new EditorUtility.SelectMenuItemFunction(PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                    if (s_RecycledEditor.IsEditingControl(id))
                    {
                        Event.current.Use();
                    }
                }
            }
            text = ToolbarSearchField(id, position, text, showWithPopupArrow);
            if ((showWithPopupArrow && (text == string.Empty)) && (!s_RecycledEditor.IsEditingControl(id) && (Event.current.type == EventType.Repaint)))
            {
                position.width -= 14f;
                BeginDisabledGroup(true);
                EditorStyles.toolbarSearchFieldPopup.Draw(position, EditorGUIUtility.TempContent(searchModes[searchMode]), id, false);
                EndDisabledGroup();
            }
            return text;
        }

        internal static Object ValidateObjectFieldAssignment(Object[] references, Type objType, SerializedProperty property)
        {
            if (references.Length > 0)
            {
                bool flag = DragAndDrop.objectReferences.Length > 0;
                bool flag2 = (references[0] != null) && (references[0].GetType() == typeof(Texture2D));
                if (((objType == typeof(Sprite)) && flag2) && flag)
                {
                    return SpriteUtility.TextureToSprite(references[0] as Texture2D);
                }
                if (property != null)
                {
                    if ((references[0] != null) && property.ValidateObjectReferenceValue(references[0]))
                    {
                        return references[0];
                    }
                    if ((((property.type == "PPtr<Sprite>") || (property.type == "PPtr<$Sprite>")) || (property.type == "vector")) && (flag2 && flag))
                    {
                        return SpriteUtility.TextureToSprite(references[0] as Texture2D);
                    }
                }
                else
                {
                    if (((references[0] != null) && (references[0].GetType() == typeof(GameObject))) && typeof(Component).IsAssignableFrom(objType))
                    {
                        references = ((GameObject) references[0]).GetComponents(typeof(Component));
                    }
                    foreach (Object obj3 in references)
                    {
                        if ((obj3 != null) && objType.IsAssignableFrom(obj3.GetType()))
                        {
                            return obj3;
                        }
                    }
                }
            }
            return null;
        }

        private static bool ValidTargetForIconSelection(Object[] targets)
        {
            return (((targets[0] is MonoScript) || (targets[0] is GameObject)) && (targets.Length == 1));
        }

        private static Vector2 Vector2Field(Rect position, Vector2 value)
        {
            s_Vector2Floats[0] = value.x;
            s_Vector2Floats[1] = value.y;
            position.height = 16f;
            BeginChangeCheck();
            MultiFloatField(position, s_XYLabels, s_Vector2Floats);
            if (EndChangeCheck())
            {
                value.x = s_Vector2Floats[0];
                value.y = s_Vector2Floats[1];
            }
            return value;
        }

        public static Vector2 Vector2Field(Rect position, string label, Vector2 value)
        {
            return Vector2Field(position, EditorGUIUtility.TempContent(label), value);
        }

        private static void Vector2Field(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 2);
            position.height = 16f;
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            MultiPropertyField(position, s_XYLabels, valuesIterator);
        }

        public static Vector2 Vector2Field(Rect position, GUIContent label, Vector2 value)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 2);
            position.height = 16f;
            return Vector2Field(position, value);
        }

        private static Vector3 Vector3Field(Rect position, Vector3 value)
        {
            s_Vector3Floats[0] = value.x;
            s_Vector3Floats[1] = value.y;
            s_Vector3Floats[2] = value.z;
            position.height = 16f;
            BeginChangeCheck();
            MultiFloatField(position, s_XYZLabels, s_Vector3Floats);
            if (EndChangeCheck())
            {
                value.x = s_Vector3Floats[0];
                value.y = s_Vector3Floats[1];
                value.z = s_Vector3Floats[2];
            }
            return value;
        }

        public static Vector3 Vector3Field(Rect position, string label, Vector3 value)
        {
            return Vector3Field(position, EditorGUIUtility.TempContent(label), value);
        }

        private static void Vector3Field(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 3);
            position.height = 16f;
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            MultiPropertyField(position, s_XYZLabels, valuesIterator);
        }

        public static Vector3 Vector3Field(Rect position, GUIContent label, Vector3 value)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 3);
            position.height = 16f;
            return Vector3Field(position, value);
        }

        public static Vector4 Vector4Field(Rect position, string label, Vector4 value)
        {
            s_Vector4Floats[0] = value.x;
            s_Vector4Floats[1] = value.y;
            s_Vector4Floats[2] = value.z;
            s_Vector4Floats[3] = value.w;
            position.height = 16f;
            GUI.Label(IndentedRect(position), label, EditorStyles.label);
            position.y += 16f;
            BeginChangeCheck();
            indentLevel++;
            MultiFloatField(position, s_XYZWLabels, s_Vector4Floats);
            indentLevel--;
            if (EndChangeCheck())
            {
                value.x = s_Vector4Floats[0];
                value.y = s_Vector4Floats[1];
                value.z = s_Vector4Floats[2];
                value.w = s_Vector4Floats[3];
            }
            return value;
        }

        private static void Vector4Field(Rect position, SerializedProperty property, GUIContent label)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = MultiFieldPrefixLabel(position, id, label, 4);
            position.height = 16f;
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            MultiPropertyField(position, s_XYZWLabels, valuesIterator);
        }

        public static bool actionKey
        {
            get
            {
                if (Event.current == null)
                {
                    return false;
                }
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    return Event.current.command;
                }
                return Event.current.control;
            }
        }

        internal static Material alphaMaterial
        {
            get
            {
                return (EditorGUIUtility.LoadRequired("Previews/PreviewAlphaMaterial.mat") as Material);
            }
        }

        internal static float indent
        {
            get
            {
                return (indentLevel * 15f);
            }
        }

        public static int indentLevel
        {
            get
            {
                return ms_IndentLevel;
            }
            set
            {
                ms_IndentLevel = value;
            }
        }

        internal static bool isCollectingTooltips
        {
            get
            {
                return s_CollectingToolTips;
            }
            set
            {
                s_CollectingToolTips = value;
            }
        }

        internal static Material lightmapDoubleLDRMaterial
        {
            get
            {
                return (EditorGUIUtility.LoadRequired("Previews/PreviewEncodedLightmapDoubleLDRMaterial.mat") as Material);
            }
        }

        internal static Material lightmapRGBMMaterial
        {
            get
            {
                return (EditorGUIUtility.LoadRequired("Previews/PreviewEncodedLightmapRGBMMaterial.mat") as Material);
            }
        }

        internal static GUIContent mixedValueContent
        {
            get
            {
                return s_MixedValueContent;
            }
        }

        internal static Material normalmapMaterial
        {
            get
            {
                return (EditorGUIUtility.LoadRequired("Previews/PreviewEncodedNormalsMaterial.mat") as Material);
            }
        }

        public static bool showMixedValue
        {
            get
            {
                return s_ShowMixedValue;
            }
            set
            {
                s_ShowMixedValue = value;
            }
        }

        internal static Texture2D transparentCheckerTexture
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return (EditorGUIUtility.LoadRequired("Previews/Textures/textureCheckerDark.png") as Texture2D);
                }
                return (EditorGUIUtility.LoadRequired("Previews/Textures/textureChecker.png") as Texture2D);
            }
        }

        internal static Material transparentMaterial
        {
            get
            {
                return (EditorGUIUtility.LoadRequired("Previews/PreviewTransparentMaterial.mat") as Material);
            }
        }

        private class ColorBrightnessFieldStateObject
        {
            public float m_Brightness;
            public float m_Hue;
            public float m_Saturation;
        }

        internal sealed class DelayedTextEditor : EditorGUI.RecycledTextEditor
        {
            private int controlThatHadFocus;
            internal string controlThatHadFocusValue = string.Empty;
            private int controlThatLostFocus;
            private bool m_IgnoreBeginGUI;
            private int messageControl;
            private GUIView viewThatHadFocus;

            public void BeginGUI()
            {
                if (!this.m_IgnoreBeginGUI)
                {
                    if (GUIUtility.keyboardControl == base.controlID)
                    {
                        this.controlThatHadFocus = GUIUtility.keyboardControl;
                        this.controlThatHadFocusValue = base.text;
                        this.viewThatHadFocus = GUIView.current;
                    }
                    else
                    {
                        this.controlThatHadFocus = 0;
                    }
                }
            }

            public override void EndEditing()
            {
                base.EndEditing();
                this.messageControl = 0;
            }

            public void EndGUI(EventType type)
            {
                int controlThatHadFocus = 0;
                if (this.controlThatLostFocus != 0)
                {
                    this.messageControl = this.controlThatLostFocus;
                    this.controlThatLostFocus = 0;
                }
                if ((this.controlThatHadFocus != 0) && (this.controlThatHadFocus != GUIUtility.keyboardControl))
                {
                    controlThatHadFocus = this.controlThatHadFocus;
                    this.controlThatHadFocus = 0;
                }
                if (controlThatHadFocus != 0)
                {
                    this.messageControl = controlThatHadFocus;
                    this.m_IgnoreBeginGUI = true;
                    this.viewThatHadFocus.SendEvent(EditorGUIUtility.CommandEvent("DelayedControlShouldCommit"));
                    this.m_IgnoreBeginGUI = false;
                    this.messageControl = 0;
                }
            }

            public string OnGUI(int id, string value, out bool changed)
            {
                Event current = Event.current;
                if (((current.type == EventType.ExecuteCommand) && (current.commandName == "DelayedControlShouldCommit")) && (id == this.messageControl))
                {
                    changed = value != this.controlThatHadFocusValue;
                    current.Use();
                    return this.controlThatHadFocusValue;
                }
                changed = false;
                return value;
            }
        }

        public class DisabledGroupScope : GUI.Scope
        {
            public DisabledGroupScope(bool disabled)
            {
                EditorGUI.BeginDisabledGroup(disabled);
            }

            protected override void CloseScope()
            {
                EditorGUI.EndDisabledGroup();
            }
        }

        internal sealed class GUIContents
        {
            static GUIContents()
            {
                foreach (PropertyInfo info in typeof(EditorGUI.GUIContents).GetProperties(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    IconName[] customAttributes = (IconName[]) info.GetCustomAttributes(typeof(IconName), false);
                    if (customAttributes.Length > 0)
                    {
                        GUIContent content = EditorGUIUtility.IconContent(customAttributes[0].name);
                        info.SetValue(null, content, null);
                    }
                }
            }

            [IconName("_Help")]
            internal static GUIContent helpIcon
            {
                [CompilerGenerated]
                get
                {
                    return <helpIcon>k__BackingField;
                }
                [CompilerGenerated]
                private set
                {
                    <helpIcon>k__BackingField = value;
                }
            }

            [IconName("_Popup")]
            internal static GUIContent titleSettingsIcon
            {
                [CompilerGenerated]
                get
                {
                    return <titleSettingsIcon>k__BackingField;
                }
                [CompilerGenerated]
                private set
                {
                    <titleSettingsIcon>k__BackingField = value;
                }
            }

            private class IconName : Attribute
            {
                private string m_Name;

                public IconName(string name)
                {
                    this.m_Name = name;
                }

                public virtual string name
                {
                    get
                    {
                        return this.m_Name;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KnobContext
        {
            private const int kPixelRange = 250;
            private readonly Rect position;
            private readonly Vector2 knobSize;
            private readonly float currentValue;
            private readonly float start;
            private readonly float end;
            private readonly string unit;
            private readonly Color activeColor;
            private readonly Color backgroundColor;
            private readonly bool showValue;
            private readonly int id;
            private static Material knobMaterial;
            public KnobContext(Rect position, Vector2 knobSize, float currentValue, float start, float end, string unit, Color backgroundColor, Color activeColor, bool showValue, int id)
            {
                this.position = position;
                this.knobSize = knobSize;
                this.currentValue = currentValue;
                this.start = start;
                this.end = end;
                this.unit = unit;
                this.activeColor = activeColor;
                this.backgroundColor = backgroundColor;
                this.showValue = showValue;
                this.id = id;
            }

            public float Handle()
            {
                if (this.KnobState().isEditing && (this.CurrentEventType() != EventType.Repaint))
                {
                    return this.DoKeyboardInput();
                }
                switch (this.CurrentEventType())
                {
                    case EventType.MouseDown:
                        return this.OnMouseDown();

                    case EventType.MouseUp:
                        return this.OnMouseUp();

                    case EventType.MouseDrag:
                        return this.OnMouseDrag();

                    case EventType.Repaint:
                        return this.OnRepaint();
                }
                return this.currentValue;
            }

            private EventType CurrentEventType()
            {
                return this.CurrentEvent().type;
            }

            private bool IsEmptyKnob()
            {
                return (this.start == this.end);
            }

            private Event CurrentEvent()
            {
                return Event.current;
            }

            private float Clamp(float value)
            {
                return Mathf.Clamp(value, this.MinValue(), this.MaxValue());
            }

            private float ClampedCurrentValue()
            {
                return this.Clamp(this.currentValue);
            }

            private float MaxValue()
            {
                return Mathf.Max(this.start, this.end);
            }

            private float MinValue()
            {
                return Mathf.Min(this.start, this.end);
            }

            private float GetCurrentValuePercent()
            {
                return ((this.ClampedCurrentValue() - this.MinValue()) / (this.MaxValue() - this.MinValue()));
            }

            private float MousePosition()
            {
                return (this.CurrentEvent().mousePosition.y - this.position.y);
            }

            private bool WasDoubleClick()
            {
                return ((this.CurrentEventType() == EventType.MouseDown) && (this.CurrentEvent().clickCount == 2));
            }

            private float ValuesPerPixel()
            {
                return (250f / (this.MaxValue() - this.MinValue()));
            }

            private UnityEditor.KnobState KnobState()
            {
                return (UnityEditor.KnobState) GUIUtility.GetStateObject(typeof(UnityEditor.KnobState), this.id);
            }

            private void StartDraggingWithValue(float dragStartValue)
            {
                UnityEditor.KnobState state = this.KnobState();
                state.dragStartPos = this.MousePosition();
                state.dragStartValue = dragStartValue;
                state.isDragging = true;
            }

            private float OnMouseDown()
            {
                if ((this.position.Contains(this.CurrentEvent().mousePosition) && !this.KnobState().isEditing) && !this.IsEmptyKnob())
                {
                    GUIUtility.hotControl = this.id;
                    if (this.WasDoubleClick())
                    {
                        this.KnobState().isEditing = true;
                    }
                    else
                    {
                        this.StartDraggingWithValue(this.ClampedCurrentValue());
                    }
                    this.CurrentEvent().Use();
                }
                return this.currentValue;
            }

            private float OnMouseDrag()
            {
                if (GUIUtility.hotControl != this.id)
                {
                    return this.currentValue;
                }
                UnityEditor.KnobState state = this.KnobState();
                if (!state.isDragging)
                {
                    return this.currentValue;
                }
                GUI.changed = true;
                this.CurrentEvent().Use();
                float num = state.dragStartPos - this.MousePosition();
                float num2 = state.dragStartValue + (num / this.ValuesPerPixel());
                return this.Clamp(num2);
            }

            private float OnMouseUp()
            {
                if (GUIUtility.hotControl == this.id)
                {
                    this.CurrentEvent().Use();
                    GUIUtility.hotControl = 0;
                    this.KnobState().isDragging = false;
                }
                return this.currentValue;
            }

            private void PrintValue()
            {
                Rect position = new Rect((this.position.x + (this.knobSize.x / 2f)) - 8f, (this.position.y + (this.knobSize.y / 2f)) - 8f, this.position.width, 20f);
                GUI.Label(position, this.currentValue.ToString("0.##") + " " + this.unit);
            }

            private float DoKeyboardInput()
            {
                GUI.SetNextControlName("KnobInput");
                GUI.FocusControl("KnobInput");
                EditorGUI.BeginChangeCheck();
                Rect position = new Rect((this.position.x + (this.knobSize.x / 2f)) - 6f, (this.position.y + (this.knobSize.y / 2f)) - 7f, this.position.width, 20f);
                GUIStyle none = GUIStyle.none;
                none.normal.textColor = new Color(0.703f, 0.703f, 0.703f, 1f);
                none.fontStyle = FontStyle.Normal;
                string str = EditorGUI.DelayedTextField(position, this.currentValue.ToString("0.##"), none);
                if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(str))
                {
                    float num;
                    this.KnobState().isEditing = false;
                    if (float.TryParse(str, out num) && (num != this.currentValue))
                    {
                        return this.Clamp(num);
                    }
                }
                return this.currentValue;
            }

            private static void CreateKnobMaterial()
            {
                if (knobMaterial == null)
                {
                    Shader builtinResource = Resources.GetBuiltinResource(typeof(Shader), "Internal-GUITextureClip.shader") as Shader;
                    knobMaterial = new Material(builtinResource);
                    knobMaterial.hideFlags = HideFlags.HideAndDontSave;
                    knobMaterial.mainTexture = EditorGUIUtility.FindTexture("KnobCShape");
                    knobMaterial.name = "Knob Material";
                    if (knobMaterial.mainTexture == null)
                    {
                        Debug.Log("Did not find 'KnobCShape'");
                    }
                }
            }

            private Vector3 GetUVForPoint(Vector3 point)
            {
                return new Vector3((point.x - this.position.x) / this.knobSize.x, ((point.y - this.position.y) - this.knobSize.y) / -this.knobSize.y);
            }

            private void VertexPointWithUV(Vector3 point)
            {
                GL.TexCoord(this.GetUVForPoint(point));
                GL.Vertex(point);
            }

            private void DrawValueArc(float angle)
            {
                CreateKnobMaterial();
                Vector3 point = new Vector3(this.position.x + (this.knobSize.x / 2f), this.position.y + (this.knobSize.y / 2f), 0f);
                Vector3 vector2 = new Vector3(this.position.x + this.knobSize.x, this.position.y + (this.knobSize.y / 2f), 0f);
                Vector3 vector3 = new Vector3(this.position.x + this.knobSize.x, this.position.y + this.knobSize.y, 0f);
                Vector3 vector4 = new Vector3(this.position.x, this.position.y + this.knobSize.y, 0f);
                Vector3 vector5 = new Vector3(this.position.x, this.position.y, 0f);
                Vector3 vector6 = new Vector3(this.position.x + this.knobSize.x, this.position.y, 0f);
                Vector3 vector7 = vector3;
                knobMaterial.SetPass(0);
                GL.Begin(7);
                GL.Color(this.backgroundColor);
                this.VertexPointWithUV(vector5);
                this.VertexPointWithUV(vector6);
                this.VertexPointWithUV(vector3);
                this.VertexPointWithUV(vector4);
                GL.End();
                GL.Begin(4);
                GL.Color((Color) (this.activeColor * (!GUI.enabled ? 0.5f : 1f)));
                if (angle > 0f)
                {
                    this.VertexPointWithUV(point);
                    this.VertexPointWithUV(vector2);
                    this.VertexPointWithUV(vector3);
                    vector7 = vector3;
                    if (angle > 1.570796f)
                    {
                        this.VertexPointWithUV(point);
                        this.VertexPointWithUV(vector3);
                        this.VertexPointWithUV(vector4);
                        vector7 = vector4;
                        if (angle > 3.141593f)
                        {
                            this.VertexPointWithUV(point);
                            this.VertexPointWithUV(vector4);
                            this.VertexPointWithUV(vector5);
                            vector7 = vector5;
                        }
                    }
                    if (angle == 4.712389f)
                    {
                        this.VertexPointWithUV(point);
                        this.VertexPointWithUV(vector5);
                        this.VertexPointWithUV(vector6);
                        this.VertexPointWithUV(point);
                        this.VertexPointWithUV(vector6);
                        this.VertexPointWithUV(vector2);
                    }
                    else
                    {
                        Vector3 vector8;
                        float num = angle + 0.7853982f;
                        if (angle < 1.570796f)
                        {
                            vector8 = vector3 + new Vector3(((this.knobSize.y / 2f) * Mathf.Tan(1.570796f - num)) - (this.knobSize.x / 2f), 0f, 0f);
                        }
                        else if (angle < 3.141593f)
                        {
                            vector8 = vector4 + new Vector3(0f, ((this.knobSize.x / 2f) * Mathf.Tan(3.141593f - num)) - (this.knobSize.y / 2f), 0f);
                        }
                        else
                        {
                            vector8 = vector5 + new Vector3(-((this.knobSize.y / 2f) * Mathf.Tan(4.712389f - num)) + (this.knobSize.x / 2f), 0f, 0f);
                        }
                        this.VertexPointWithUV(point);
                        this.VertexPointWithUV(vector7);
                        this.VertexPointWithUV(vector8);
                    }
                }
                GL.End();
            }

            private float OnRepaint()
            {
                this.DrawValueArc((this.GetCurrentValuePercent() * 3.141593f) * 1.5f);
                if (this.KnobState().isEditing)
                {
                    return this.DoKeyboardInput();
                }
                if (this.showValue)
                {
                    this.PrintValue();
                }
                return this.currentValue;
            }
        }

        internal delegate Object ObjectFieldValidator(Object[] references, Type objType, SerializedProperty property);

        internal enum ObjectFieldVisualType
        {
            IconAndText,
            LargePreview,
            MiniPreivew
        }

        internal sealed class PopupCallbackInfo
        {
            public static EditorGUI.PopupCallbackInfo instance;
            internal const string kPopupMenuChangedMessage = "PopupMenuChanged";
            private int m_ControlID;
            private int m_SelectedIndex;
            private GUIView m_SourceView;

            public PopupCallbackInfo(int controlID)
            {
                this.m_ControlID = controlID;
                this.m_SourceView = GUIView.current;
            }

            public static int GetSelectedValueForControl(int controlID, int selected)
            {
                Event current = Event.current;
                if ((current.type == EventType.ExecuteCommand) && (current.commandName == "PopupMenuChanged"))
                {
                    if (instance == null)
                    {
                        Debug.LogError("Popup menu has no instance");
                        return selected;
                    }
                    if (instance.m_ControlID == controlID)
                    {
                        selected = instance.m_SelectedIndex;
                        instance = null;
                        GUI.changed = true;
                        current.Use();
                    }
                }
                return selected;
            }

            internal void SetEnumValueDelegate(object userData, string[] options, int selected)
            {
                this.m_SelectedIndex = selected;
                if (this.m_SourceView != null)
                {
                    this.m_SourceView.SendEvent(EditorGUIUtility.CommandEvent("PopupMenuChanged"));
                }
            }
        }

        internal sealed class PopupMenuEvent
        {
            public string commandName;
            public GUIView receiver;

            public PopupMenuEvent(string cmd, GUIView v)
            {
                this.commandName = cmd;
                this.receiver = v;
            }

            public void SendEvent()
            {
                if (this.receiver != null)
                {
                    this.receiver.SendEvent(EditorGUIUtility.CommandEvent(this.commandName));
                }
                else
                {
                    Debug.LogError("BUG: We don't have a receiver set up, please report");
                }
            }
        }

        public class PropertyScope : GUI.Scope
        {
            public PropertyScope(Rect totalPosition, GUIContent label, SerializedProperty property)
            {
                this.content = EditorGUI.BeginProperty(totalPosition, label, property);
            }

            protected override void CloseScope()
            {
                EditorGUI.EndProperty();
            }

            public GUIContent content { get; protected set; }
        }

        internal class RecycledTextEditor : TextEditor
        {
            internal static bool s_ActuallyEditing;
            internal static bool s_AllowContextCutOrPaste = true;

            public virtual void BeginEditing(int id, string newText, Rect position, GUIStyle style, bool multiline, bool passwordField)
            {
                if (!this.IsEditingControl(id))
                {
                    if (EditorGUI.activeEditor != null)
                    {
                        EditorGUI.activeEditor.EndEditing();
                    }
                    EditorGUI.activeEditor = this;
                    base.controlID = id;
                    base.text = EditorGUI.s_OriginalText = newText;
                    base.multiline = multiline;
                    base.style = style;
                    base.position = position;
                    base.isPasswordField = passwordField;
                    s_ActuallyEditing = true;
                    base.scrollOffset = Vector2.zero;
                    Undo.IncrementCurrentGroup();
                }
            }

            public virtual void EndEditing()
            {
                if (EditorGUI.activeEditor == this)
                {
                    EditorGUI.activeEditor = null;
                }
                base.controlID = 0;
                s_ActuallyEditing = false;
                s_AllowContextCutOrPaste = true;
                Undo.IncrementCurrentGroup();
            }

            internal bool IsEditingControl(int id)
            {
                return ((((GUIUtility.keyboardControl == id) && (base.controlID == id)) && s_ActuallyEditing) && GUIView.current.hasFocus);
            }
        }

        internal class VUMeter
        {
            private static Texture2D s_HorizontalVUTexture;
            private static Texture2D s_VerticalVUTexture;
            private const float VU_SPLIT = 0.9f;

            public static void HorizontalMeter(Rect position, float value, float peak, Texture2D foregroundTexture, Color peakColor)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Color color = GUI.color;
                    EditorStyles.progressBarBack.Draw(position, false, false, false, false);
                    GUI.color = new Color(1f, 1f, 1f, !GUI.enabled ? 0.5f : 1f);
                    float width = (position.width * value) - 2f;
                    if (width < 2f)
                    {
                        width = 2f;
                    }
                    Rect rect = new Rect(position.x + 1f, position.y + 1f, width, position.height - 2f);
                    Rect texCoords = new Rect(0f, 0f, value, 1f);
                    GUI.DrawTextureWithTexCoords(rect, foregroundTexture, texCoords);
                    GUI.color = peakColor;
                    float num2 = (position.width * peak) - 2f;
                    if (num2 < 2f)
                    {
                        num2 = 2f;
                    }
                    rect = new Rect(position.x + num2, position.y + 1f, 1f, position.height - 2f);
                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                    GUI.color = color;
                }
            }

            public static void HorizontalMeter(Rect position, float value, ref SmoothingData data, Texture2D foregroundTexture, Color peakColor)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    float num;
                    float num2;
                    SmoothVUMeterData(ref value, ref data, out num, out num2);
                    HorizontalMeter(position, num, num2, foregroundTexture, peakColor);
                }
            }

            private static void SmoothVUMeterData(ref float value, ref SmoothingData data, out float renderValue, out float renderPeak)
            {
                if (value <= data.lastValue)
                {
                    value = Mathf.Lerp(data.lastValue, value, Time.smoothDeltaTime * 7f);
                }
                else
                {
                    value = Mathf.Lerp(value, data.lastValue, Time.smoothDeltaTime * 2f);
                    data.peakValue = value;
                    data.peakValueTime = Time.realtimeSinceStartup;
                }
                if (value > 1.111111f)
                {
                    value = 1.111111f;
                }
                if (data.peakValue > 1.111111f)
                {
                    data.peakValue = 1.111111f;
                }
                renderValue = value * 0.9f;
                renderPeak = data.peakValue * 0.9f;
                data.lastValue = value;
            }

            public static void VerticalMeter(Rect position, float value, float peak, Texture2D foregroundTexture, Color peakColor)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Color color = GUI.color;
                    EditorStyles.progressBarBack.Draw(position, false, false, false, false);
                    GUI.color = new Color(1f, 1f, 1f, !GUI.enabled ? 0.5f : 1f);
                    float height = (position.height - 2f) * value;
                    if (height < 2f)
                    {
                        height = 2f;
                    }
                    Rect rect = new Rect(position.x + 1f, ((position.y + position.height) - 1f) - height, position.width - 2f, height);
                    Rect texCoords = new Rect(0f, 0f, 1f, value);
                    GUI.DrawTextureWithTexCoords(rect, foregroundTexture, texCoords);
                    GUI.color = peakColor;
                    float num2 = (position.height - 2f) * peak;
                    if (num2 < 2f)
                    {
                        num2 = 2f;
                    }
                    rect = new Rect(position.x + 1f, ((position.y + position.height) - 1f) - num2, position.width - 2f, 1f);
                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                    GUI.color = color;
                }
            }

            public static void VerticalMeter(Rect position, float value, ref SmoothingData data, Texture2D foregroundTexture, Color peakColor)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    float num;
                    float num2;
                    SmoothVUMeterData(ref value, ref data, out num, out num2);
                    VerticalMeter(position, num, num2, foregroundTexture, peakColor);
                }
            }

            public static Texture2D horizontalVUTexture
            {
                get
                {
                    if (s_HorizontalVUTexture == null)
                    {
                        s_HorizontalVUTexture = EditorGUIUtility.LoadIcon("VUMeterTextureHorizontal");
                    }
                    return s_HorizontalVUTexture;
                }
            }

            public static Texture2D verticalVUTexture
            {
                get
                {
                    if (s_VerticalVUTexture == null)
                    {
                        s_VerticalVUTexture = EditorGUIUtility.LoadIcon("VUMeterTextureVertical");
                    }
                    return s_VerticalVUTexture;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SmoothingData
            {
                public float lastValue;
                public float peakValue;
                public float peakValueTime;
            }
        }
    }
}

