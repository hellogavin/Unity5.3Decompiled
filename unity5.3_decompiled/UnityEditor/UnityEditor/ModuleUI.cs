namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal abstract class ModuleUI : SerializedModule
    {
        public static float k_CompactFixedModuleWidth = 195f;
        protected const float k_minMaxToggleWidth = 13f;
        public static float k_SpaceBetweenModules = 5f;
        protected const float k_toggleWidth = 9f;
        protected const float kDragSpace = 20f;
        protected const string kFormatString = "g7";
        protected const int kPlusAddRemoveButtonSpacing = 5;
        protected const int kPlusAddRemoveButtonWidth = 12;
        protected static readonly Rect kSignedRange = new Rect(0f, -1f, 1f, 2f);
        protected const int kSingleLineHeight = 13;
        protected const int kSpacingSubLabel = 4;
        protected const int kSubLabelWidth = 10;
        protected static readonly Rect kUnsignedRange = new Rect(0f, 0f, 1f, 1f);
        protected static readonly bool kUseSignedRange = true;
        private List<SerializedProperty> m_CurvesRemovedWhenFolded;
        private string m_DisplayName;
        private SerializedProperty m_Enabled;
        public List<SerializedProperty> m_ModuleCurves;
        public ParticleSystemUI m_ParticleSystemUI;
        protected string m_ToolTip;
        private VisibilityState m_VisibilityState;
        private static readonly GUIStyle s_ControlRectStyle;

        static ModuleUI()
        {
            GUIStyle style = new GUIStyle {
                margin = new RectOffset(0, 0, 2, 2)
            };
            s_ControlRectStyle = style;
        }

        public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName) : base(o, name)
        {
            this.m_ToolTip = string.Empty;
            this.m_ModuleCurves = new List<SerializedProperty>();
            this.m_CurvesRemovedWhenFolded = new List<SerializedProperty>();
            this.Setup(owner, o, name, displayName, VisibilityState.NotVisible);
        }

        public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName, VisibilityState initialVisibilityState) : base(o, name)
        {
            this.m_ToolTip = string.Empty;
            this.m_ModuleCurves = new List<SerializedProperty>();
            this.m_CurvesRemovedWhenFolded = new List<SerializedProperty>();
            this.Setup(owner, o, name, displayName, initialVisibilityState);
        }

        public void AddToModuleCurves(SerializedProperty curveProp)
        {
            this.m_ModuleCurves.Add(curveProp);
            if (!this.foldout)
            {
                this.m_CurvesRemovedWhenFolded.Add(curveProp);
            }
        }

        internal void CheckVisibilityState()
        {
            if ((!(this is RendererModuleUI) && !this.m_Enabled.boolValue) && !ParticleEffectUI.GetAllModulesVisible())
            {
                this.SetVisibilityState(VisibilityState.NotVisible);
            }
            if (this.m_Enabled.boolValue && !this.visibleUI)
            {
                this.SetVisibilityState(VisibilityState.VisibleAndFolded);
            }
        }

        private static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth)
        {
            return FloatDraggable(rect, floatProp, remap, dragWidth, "g7");
        }

        public static float FloatDraggable(Rect rect, float floatValue, float remap, float dragWidth, string formatString)
        {
            int id = GUIUtility.GetControlID(0x62dd15e9, FocusType.Keyboard, rect);
            Rect dragHotZone = rect;
            dragHotZone.width = dragWidth;
            Rect position = rect;
            position.xMin += dragWidth;
            return (EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, dragHotZone, id, floatValue * remap, formatString, ParticleSystemStyles.Get().numberField, true) / remap);
        }

        public static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth, string formatString)
        {
            Color color = GUI.color;
            if (floatProp.isAnimated)
            {
                GUI.color = AnimationMode.animatedPropertyColor;
            }
            float floatValue = floatProp.floatValue;
            float num2 = FloatDraggable(rect, floatValue, remap, dragWidth, formatString);
            if (num2 != floatValue)
            {
                floatProp.floatValue = num2;
            }
            GUI.color = color;
            return num2;
        }

        private static Color GetColor(SerializedMinMaxCurve mmCurve)
        {
            return mmCurve.m_Module.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor().GetCurveColor(mmCurve.maxCurve);
        }

        protected static Rect GetControlRect(int height)
        {
            return GUILayoutUtility.GetRect(0f, (float) height, s_ControlRectStyle);
        }

        protected ParticleSystem GetParticleSystem()
        {
            return (this.m_Enabled.serializedObject.targetObject as ParticleSystem);
        }

        public ParticleSystemCurveEditor GetParticleSystemCurveEditor()
        {
            return this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
        }

        private static Rect GetPopupRect(Rect position)
        {
            position.xMin = position.xMax - 13f;
            return position;
        }

        public virtual float GetXAxisScalar()
        {
            return 1f;
        }

        public static bool GUIBoolAsPopup(GUIContent label, SerializedProperty boolProp, string[] options)
        {
            Rect position = PrefixLabel(GetControlRect(13), label);
            int selectedIndex = !boolProp.boolValue ? 0 : 1;
            int num2 = EditorGUI.Popup(position, selectedIndex, options, ParticleSystemStyles.Get().popup);
            if (num2 != selectedIndex)
            {
                boolProp.boolValue = num2 > 0;
            }
            return (num2 > 0);
        }

        private static void GUIColor(Rect rect, SerializedProperty colorProp)
        {
            colorProp.colorValue = EditorGUI.ColorField(rect, colorProp.colorValue, false, true);
        }

        private static void GUICurveField(Rect position, SerializedProperty maxCurve, SerializedProperty minCurve, Color color, Rect ranges, CurveFieldMouseDownCallback mouseDownCallback)
        {
            int controlID = GUIUtility.GetControlID(0x4ec1c30f, EditorGUIUtility.native, position);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((position.Contains(current.mousePosition) && (mouseDownCallback != null)) && mouseDownCallback(current.button, position, ranges))
                    {
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                {
                    Rect rect = position;
                    if (minCurve == null)
                    {
                        EditorGUIUtility.DrawCurveSwatch(rect, null, maxCurve, color, EditorGUI.kCurveBGColor, ranges);
                    }
                    else
                    {
                        EditorGUIUtility.DrawRegionSwatch(rect, maxCurve, minCurve, color, EditorGUI.kCurveBGColor, ranges);
                    }
                    EditorStyles.colorPickerBox.Draw(rect, GUIContent.none, controlID, false);
                    break;
                }
                case EventType.ValidateCommand:
                    if (current.commandName == "UndoRedoPerformed")
                    {
                        AnimationCurvePreviewCache.ClearCache();
                    }
                    break;
            }
        }

        public static float GUIFloat(string label, SerializedProperty floatProp)
        {
            return GUIFloat(GUIContent.Temp(label), floatProp);
        }

        public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp)
        {
            return GUIFloat(guiContent, floatProp, "g7");
        }

        public static float GUIFloat(GUIContent guiContent, float floatValue, string formatString)
        {
            Rect controlRect = GetControlRect(13);
            PrefixLabel(controlRect, guiContent);
            return FloatDraggable(controlRect, floatValue, 1f, EditorGUIUtility.labelWidth, formatString);
        }

        public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp, string formatString)
        {
            Rect controlRect = GetControlRect(13);
            PrefixLabel(controlRect, guiContent);
            return FloatDraggable(controlRect, floatProp, 1f, EditorGUIUtility.labelWidth, formatString);
        }

        private static void GUIGradientAsColor(Rect rect, SerializedProperty gradientProp)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            Color gradientAsColor = SerializedMinMaxGradient.GetGradientAsColor(gradientProp);
            gradientAsColor = EditorGUI.ColorField(rect, gradientAsColor, false, true);
            if (GUI.changed)
            {
                SerializedMinMaxGradient.SetGradientAsColor(gradientProp, gradientAsColor);
            }
            GUI.changed |= changed;
        }

        public static int GUIInt(GUIContent guiContent, int intValue)
        {
            Rect totalPosition = GUILayoutUtility.GetRect((float) 0f, (float) 13f);
            PrefixLabel(totalPosition, guiContent);
            return IntDraggable(totalPosition, null, intValue, EditorGUIUtility.labelWidth);
        }

        public static void GUIInt(GUIContent guiContent, SerializedProperty intProp)
        {
            intProp.intValue = GUIInt(guiContent, intProp.intValue);
        }

        public static int GUIIntDraggable(GUIContent label, int intValue)
        {
            return IntDraggable(GUILayoutUtility.GetRect((float) 0f, (float) 13f), label, intValue, EditorGUIUtility.labelWidth);
        }

        public static int GUIIntDraggable(GUIContent label, SerializedProperty intProp)
        {
            return GUIIntDraggable(label, intProp.intValue);
        }

        public static void GUIIntDraggableX2(GUIContent mainLabel, GUIContent label1, SerializedProperty intProp1, GUIContent label2, SerializedProperty intProp2)
        {
            Rect rect = PrefixLabel(GetControlRect(13), mainLabel);
            float width = (rect.width - 4f) * 0.5f;
            Rect rect2 = new Rect(rect.x, rect.y, width, rect.height);
            IntDraggable(rect2, label1, intProp1, 10f);
            rect2.x += width + 4f;
            IntDraggable(rect2, label2, intProp2, 10f);
        }

        public static void GUILayerMask(GUIContent guiContent, SerializedProperty boolProp)
        {
            EditorGUI.LayerMaskField(PrefixLabel(GetControlRect(13), guiContent), boolProp, GUIContent.none, ParticleSystemStyles.Get().popup);
        }

        public int GUIListOfFloatObjectToggleFields(GUIContent label, SerializedProperty[] objectProps, EditorGUI.ObjectFieldValidator validator, GUIContent buttonTooltip, bool allowCreation)
        {
            int num = -1;
            int length = objectProps.Length;
            Rect totalPosition = GUILayoutUtility.GetRect(0f, (float) (15 * length));
            totalPosition.height = 13f;
            float num3 = 10f;
            float num4 = 35f;
            float num5 = 10f;
            float width = (((totalPosition.width - num3) - num4) - (num5 * 2f)) - 9f;
            PrefixLabel(totalPosition, label);
            for (int i = 0; i < length; i++)
            {
                SerializedProperty property = objectProps[i];
                Rect position = new Rect(((totalPosition.x + num3) + num4) + num5, totalPosition.y, width, totalPosition.height);
                int id = GUIUtility.GetControlID(0x12da2a, EditorGUIUtility.native, position);
                EditorGUI.DoObjectField(position, position, id, null, null, property, validator, true, ParticleSystemStyles.Get().objectField);
                if (property.objectReferenceValue == null)
                {
                    position = new Rect(totalPosition.xMax - 9f, totalPosition.y + 3f, 9f, 9f);
                    if (allowCreation)
                    {
                        if (buttonTooltip == null)
                        {
                        }
                        if (!GUI.Button(position, GUIContent.none, ParticleSystemStyles.Get().plus))
                        {
                            goto Label_0129;
                        }
                    }
                    num = i;
                }
            Label_0129:
                totalPosition.y += 15f;
            }
            return num;
        }

        public void GUIMinMaxColor(GUIContent label, SerializedMinMaxColor minMaxColor)
        {
            Rect rect = PrefixLabel(GetControlRect(13), label);
            float width = (rect.width - 13f) - 5f;
            if (!minMaxColor.minMax.boolValue)
            {
                Rect rect2 = new Rect(rect.x, rect.y, width, rect.height);
                GUIColor(rect2, minMaxColor.maxColor);
            }
            else
            {
                Rect rect3 = new Rect(rect.x, rect.y, (width * 0.5f) - 2f, rect.height);
                GUIColor(rect3, minMaxColor.minColor);
                rect3.x += rect3.width + 4f;
                GUIColor(rect3, minMaxColor.maxColor);
            }
            Rect rect4 = new Rect(rect.xMax - 13f, rect.y, 13f, 13f);
            GUIMMColorPopUp(rect4, minMaxColor.minMax);
        }

        public static void GUIMinMaxCurve(string label, SerializedMinMaxCurve mmCurve)
        {
            GUIMinMaxCurve(GUIContent.Temp(label), mmCurve);
        }

        public static void GUIMinMaxCurve(GUIContent label, SerializedMinMaxCurve mmCurve)
        {
            Rect controlRect = GetControlRect(13);
            Rect popupRect = GetPopupRect(controlRect);
            controlRect = SubtractPopupWidth(controlRect);
            Rect position = PrefixLabel(controlRect, label);
            MinMaxCurveState state = mmCurve.state;
            switch (state)
            {
                case MinMaxCurveState.k_Scalar:
                {
                    float a = FloatDraggable(controlRect, mmCurve.scalar, mmCurve.m_RemapValue, EditorGUIUtility.labelWidth);
                    if (!mmCurve.signedRange)
                    {
                        mmCurve.scalar.floatValue = Mathf.Max(a, 0f);
                    }
                    break;
                }
                case MinMaxCurveState.k_TwoScalars:
                {
                    Rect rect4 = position;
                    rect4.width = (position.width - 20f) * 0.5f;
                    float minConstant = mmCurve.minConstant;
                    float maxConstant = mmCurve.maxConstant;
                    Rect rect = rect4;
                    rect.xMin -= 20f;
                    EditorGUI.BeginChangeCheck();
                    minConstant = FloatDraggable(rect, minConstant, mmCurve.m_RemapValue, 20f, "g5");
                    if (EditorGUI.EndChangeCheck())
                    {
                        mmCurve.minConstant = minConstant;
                    }
                    rect.x += rect4.width + 20f;
                    EditorGUI.BeginChangeCheck();
                    maxConstant = FloatDraggable(rect, maxConstant, mmCurve.m_RemapValue, 20f, "g5");
                    if (EditorGUI.EndChangeCheck())
                    {
                        mmCurve.maxConstant = maxConstant;
                    }
                    break;
                }
                default:
                {
                    Rect ranges = !mmCurve.signedRange ? kUnsignedRange : kSignedRange;
                    SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : mmCurve.minCurve;
                    GUICurveField(position, mmCurve.maxCurve, minCurve, GetColor(mmCurve), ranges, new CurveFieldMouseDownCallback(mmCurve.OnCurveAreaMouseDown));
                    break;
                }
            }
            GUIMMCurveStateList(popupRect, mmCurve);
        }

        public void GUIMinMaxGradient(GUIContent label, SerializedMinMaxGradient minMaxGradient)
        {
            MinMaxGradientState state = minMaxGradient.state;
            bool flag = state >= MinMaxGradientState.k_RandomBetweenTwoColors;
            Rect position = GUILayoutUtility.GetRect(0f, !flag ? ((float) 13) : ((float) 0x1a));
            Rect popupRect = GetPopupRect(position);
            Rect rect = PrefixLabel(SubtractPopupWidth(position), label);
            rect.height = 13f;
            switch (state)
            {
                case MinMaxGradientState.k_Color:
                    GUIColor(rect, minMaxGradient.m_MaxColor);
                    break;

                case MinMaxGradientState.k_Gradient:
                    EditorGUI.GradientField(rect, minMaxGradient.m_MaxGradient);
                    break;

                case MinMaxGradientState.k_RandomBetweenTwoColors:
                    GUIColor(rect, minMaxGradient.m_MaxColor);
                    rect.y += rect.height;
                    GUIColor(rect, minMaxGradient.m_MinColor);
                    break;

                case MinMaxGradientState.k_RandomBetweenTwoGradients:
                    EditorGUI.GradientField(rect, minMaxGradient.m_MaxGradient);
                    rect.y += rect.height;
                    EditorGUI.GradientField(rect, minMaxGradient.m_MinGradient);
                    break;
            }
            GUIMMGradientPopUp(popupRect, minMaxGradient);
        }

        public static void GUIMinMaxRange(GUIContent label, SerializedProperty vec2Prop)
        {
            Rect rect = PrefixLabel(SubtractPopupWidth(GetControlRect(13)), label);
            float num = (rect.width - 20f) * 0.5f;
            Vector2 vector = vec2Prop.vector2Value;
            rect.width = num;
            rect.xMin -= 20f;
            vector.x = FloatDraggable(rect, vector.x, 1f, 20f, "g7");
            vector.x = Mathf.Clamp(vector.x, 0f, vector.y - 0.01f);
            rect.x += num + 20f;
            vector.y = FloatDraggable(rect, vector.y, 1f, 20f, "g7");
            vector.y = Mathf.Max(vector.x + 0.01f, vector.y);
            vec2Prop.vector2Value = vector;
        }

        public static void GUIMinMaxSlider(GUIContent label, SerializedProperty vec2Prop, float a, float b)
        {
            Rect controlRect = GetControlRect(0x1a);
            controlRect.height = 13f;
            controlRect.y += 3f;
            PrefixLabel(controlRect, label);
            Vector2 vector = vec2Prop.vector2Value;
            controlRect.y += 13f;
            EditorGUI.MinMaxSlider(controlRect, ref vector.x, ref vector.y, a, b);
            vec2Prop.vector2Value = vector;
        }

        public static void GUIMMColorPopUp(Rect rect, SerializedProperty boolProp)
        {
            if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
            {
                GenericMenu menu = new GenericMenu();
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Constant Color"), new GUIContent("Random Between Two Colors") };
                bool[] flagArray1 = new bool[2];
                flagArray1[1] = true;
                bool[] flagArray = flagArray1;
                for (int i = 0; i < contentArray.Length; i++)
                {
                    menu.AddItem(contentArray[i], boolProp.boolValue == flagArray[i], new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxColorStateCallback), new ColorCallbackData(flagArray[i], boolProp));
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve minMaxCurves)
        {
            SerializedMinMaxCurve[] curveArray = new SerializedMinMaxCurve[] { minMaxCurves };
            GUIMMCurveStateList(rect, curveArray);
        }

        public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve[] minMaxCurves)
        {
            if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown) && (minMaxCurves.Length != 0))
            {
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Constant"), new GUIContent("Curve"), new GUIContent("Random Between Two Constants"), new GUIContent("Random Between Two Curves") };
                MinMaxCurveState[] stateArray1 = new MinMaxCurveState[4];
                stateArray1[1] = MinMaxCurveState.k_Curve;
                stateArray1[2] = MinMaxCurveState.k_TwoScalars;
                stateArray1[3] = MinMaxCurveState.k_TwoCurves;
                MinMaxCurveState[] stateArray = stateArray1;
                bool[] flagArray = new bool[] { minMaxCurves[0].m_AllowConstant, minMaxCurves[0].m_AllowCurves, minMaxCurves[0].m_AllowRandom, minMaxCurves[0].m_AllowRandom && minMaxCurves[0].m_AllowCurves };
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (flagArray[i])
                    {
                        menu.AddItem(contentArray[i], minMaxCurves[0].state == stateArray[i], new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxCurveStateCallback), new CurveStateCallbackData(stateArray[i], minMaxCurves));
                    }
                }
                menu.DropDown(rect);
                Event.current.Use();
            }
        }

        public static void GUIMMGradientPopUp(Rect rect, SerializedMinMaxGradient gradientProp)
        {
            if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
            {
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Color"), new GUIContent("Gradient"), new GUIContent("Random Between Two Colors"), new GUIContent("Random Between Two Gradients") };
                MinMaxGradientState[] stateArray1 = new MinMaxGradientState[4];
                stateArray1[1] = MinMaxGradientState.k_Gradient;
                stateArray1[2] = MinMaxGradientState.k_RandomBetweenTwoColors;
                stateArray1[3] = MinMaxGradientState.k_RandomBetweenTwoGradients;
                MinMaxGradientState[] stateArray = stateArray1;
                bool[] flagArray = new bool[] { gradientProp.m_AllowColor, gradientProp.m_AllowGradient, gradientProp.m_AllowRandomBetweenTwoColors, gradientProp.m_AllowRandomBetweenTwoGradients };
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (flagArray[i])
                    {
                        menu.AddItem(contentArray[i], gradientProp.state == stateArray[i], new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxGradientStateCallback), new GradientCallbackData(stateArray[i], gradientProp));
                    }
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public static void GUIObject(GUIContent label, SerializedProperty objectProp)
        {
            EditorGUI.ObjectField(PrefixLabel(GetControlRect(13), label), objectProp, null, GUIContent.none, ParticleSystemStyles.Get().objectField);
        }

        public static void GUIObjectFieldAndToggle(GUIContent label, SerializedProperty objectProp, SerializedProperty boolProp)
        {
            Rect position = PrefixLabel(GetControlRect(13), label);
            position.xMax -= 19f;
            EditorGUI.ObjectField(position, objectProp, GUIContent.none);
            if (boolProp != null)
            {
                position.x += position.width + 10f;
                position.width = 9f;
                Toggle(position, boolProp);
            }
        }

        public static int GUIPopup(string name, SerializedProperty intProp, string[] options)
        {
            return GUIPopup(GUIContent.Temp(name), intProp, options);
        }

        public static int GUIPopup(GUIContent label, int intValue, string[] options)
        {
            return EditorGUI.Popup(PrefixLabel(GetControlRect(13), label), intValue, options, ParticleSystemStyles.Get().popup);
        }

        public static int GUIPopup(GUIContent label, SerializedProperty intProp, string[] options)
        {
            Rect position = PrefixLabel(GetControlRect(13), label);
            intProp.intValue = EditorGUI.Popup(position, intProp.intValue, options, ParticleSystemStyles.Get().popup);
            return intProp.intValue;
        }

        public static void GUISlider(SerializedProperty floatProp, float a, float b, float remap)
        {
            GUISlider(string.Empty, floatProp, a, b, remap);
        }

        public static void GUISlider(string name, SerializedProperty floatProp, float a, float b, float remap)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(300f) };
            floatProp.floatValue = EditorGUILayout.Slider(name, floatProp.floatValue * remap, a, b, options) / remap;
        }

        public static bool GUIToggle(string label, SerializedProperty boolProp)
        {
            return GUIToggle(GUIContent.Temp(label), boolProp);
        }

        public static bool GUIToggle(GUIContent guiContent, bool boolValue)
        {
            boolValue = EditorGUI.Toggle(PrefixLabel(GetControlRect(13), guiContent), boolValue, ParticleSystemStyles.Get().toggle);
            return boolValue;
        }

        public static bool GUIToggle(GUIContent guiContent, SerializedProperty boolProp)
        {
            return Toggle(PrefixLabel(GetControlRect(13), guiContent), boolProp);
        }

        public static void GUIToggleWithFloatField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle)
        {
            GUIToggleWithFloatField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle);
        }

        public static void GUIToggleWithFloatField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle)
        {
            Rect rect = PrefixLabel(GUILayoutUtility.GetRect((float) 0f, (float) 13f), guiContent);
            Rect rect2 = rect;
            rect2.xMax = rect2.x + 9f;
            bool flag = Toggle(rect2, boolProp);
            if (!invertToggle ? flag : !flag)
            {
                float dragWidth = 25f;
                Rect rect3 = new Rect((rect.x + EditorGUIUtility.labelWidth) + 9f, rect.y, rect.width - 9f, rect.height);
                FloatDraggable(rect3, floatProp, 1f, dragWidth);
            }
        }

        public static void GUIToggleWithIntField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle)
        {
            GUIToggleWithIntField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle);
        }

        public static void GUIToggleWithIntField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty intProp, bool invertToggle)
        {
            Rect rect = PrefixLabel(GetControlRect(13), guiContent);
            Rect rect2 = rect;
            rect2.xMax = rect2.x + 9f;
            bool flag = Toggle(rect2, boolProp);
            if (!invertToggle ? flag : !flag)
            {
                float dragWidth = 25f;
                Rect rect3 = new Rect((rect.x + EditorGUIUtility.labelWidth) + 9f, rect.y, rect.width - 9f, rect.height);
                intProp.intValue = IntDraggable(rect3, null, intProp.intValue, dragWidth);
            }
        }

        public void GUITripleMinMaxCurve(GUIContent label, GUIContent x, SerializedMinMaxCurve xCurve, GUIContent y, SerializedMinMaxCurve yCurve, GUIContent z, SerializedMinMaxCurve zCurve, SerializedProperty randomizePerFrame)
        {
            MinMaxCurveState state = xCurve.state;
            bool flag = label != GUIContent.none;
            int num = !flag ? 1 : 2;
            if (state == MinMaxCurveState.k_TwoScalars)
            {
                num++;
            }
            Rect controlRect = GetControlRect(13 * num);
            Rect popupRect = GetPopupRect(controlRect);
            controlRect = SubtractPopupWidth(controlRect);
            Rect rect = controlRect;
            float num2 = (controlRect.width - 8f) / 3f;
            if (num > 1)
            {
                rect.height = 13f;
            }
            if (flag)
            {
                PrefixLabel(controlRect, label);
                rect.y += rect.height;
            }
            rect.width = num2;
            GUIContent[] contentArray = new GUIContent[] { x, y, z };
            SerializedMinMaxCurve[] minMaxCurves = new SerializedMinMaxCurve[] { xCurve, yCurve, zCurve };
            switch (state)
            {
                case MinMaxCurveState.k_Scalar:
                    for (int i = 0; i < minMaxCurves.Length; i++)
                    {
                        Label(rect, contentArray[i]);
                        float a = FloatDraggable(rect, minMaxCurves[i].scalar, minMaxCurves[i].m_RemapValue, 10f);
                        if (!minMaxCurves[i].signedRange)
                        {
                            minMaxCurves[i].scalar.floatValue = Mathf.Max(a, 0f);
                        }
                        rect.x += num2 + 4f;
                    }
                    break;

                case MinMaxCurveState.k_TwoScalars:
                    for (int j = 0; j < minMaxCurves.Length; j++)
                    {
                        Label(rect, contentArray[j]);
                        float minConstant = minMaxCurves[j].minConstant;
                        float maxConstant = minMaxCurves[j].maxConstant;
                        EditorGUI.BeginChangeCheck();
                        maxConstant = FloatDraggable(rect, maxConstant, minMaxCurves[j].m_RemapValue, 10f, "g5");
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxCurves[j].maxConstant = maxConstant;
                        }
                        rect.y += 13f;
                        EditorGUI.BeginChangeCheck();
                        minConstant = FloatDraggable(rect, minConstant, minMaxCurves[j].m_RemapValue, 10f, "g5");
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxCurves[j].minConstant = minConstant;
                        }
                        rect.x += num2 + 4f;
                        rect.y -= 13f;
                    }
                    break;

                default:
                {
                    rect.width = num2;
                    Rect ranges = !xCurve.signedRange ? kUnsignedRange : kSignedRange;
                    for (int k = 0; k < minMaxCurves.Length; k++)
                    {
                        Label(rect, contentArray[k]);
                        Rect position = rect;
                        position.xMin += 10f;
                        SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : minMaxCurves[k].minCurve;
                        GUICurveField(position, minMaxCurves[k].maxCurve, minCurve, GetColor(minMaxCurves[k]), ranges, new CurveFieldMouseDownCallback(minMaxCurves[k].OnCurveAreaMouseDown));
                        rect.x += num2 + 4f;
                    }
                    break;
                }
            }
            GUIMMCurveStateList(popupRect, minMaxCurves);
        }

        protected abstract void Init();
        public static int IntDraggable(Rect rect, GUIContent label, int value, float dragWidth)
        {
            float width = rect.width;
            Rect position = rect;
            position.width = width;
            int id = GUIUtility.GetControlID(0xfd15f8, FocusType.Keyboard, position);
            Rect rect3 = position;
            rect3.width = dragWidth;
            if ((label != null) && !string.IsNullOrEmpty(label.text))
            {
                Label(rect3, label);
            }
            Rect rect4 = position;
            rect4.x += dragWidth;
            rect4.width = width - dragWidth;
            float dragSensitivity = Mathf.Max((float) 1f, (float) (Mathf.Pow(Mathf.Abs((float) value), 0.5f) * 0.03f));
            return (int) EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect4, rect3, id, (float) value, EditorGUI.kIntFieldFormatString, ParticleSystemStyles.Get().numberField, true, dragSensitivity);
        }

        public static int IntDraggable(Rect rect, GUIContent label, SerializedProperty intProp, float dragWidth)
        {
            intProp.intValue = IntDraggable(rect, label, intProp.intValue, dragWidth);
            return intProp.intValue;
        }

        private static void Label(Rect rect, GUIContent guiContent)
        {
            GUI.Label(rect, guiContent, ParticleSystemStyles.Get().label);
        }

        protected static bool MinusButton(Rect position)
        {
            return GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Minus");
        }

        public abstract void OnInspectorGUI(ParticleSystem s);
        protected virtual void OnModuleDisable()
        {
            ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
            foreach (SerializedProperty property in this.m_ModuleCurves)
            {
                if (particleSystemCurveEditor.IsAdded(property))
                {
                    particleSystemCurveEditor.RemoveCurve(property);
                }
            }
        }

        protected virtual void OnModuleEnable()
        {
            this.Init();
        }

        public virtual void OnSceneGUI(ParticleSystem s, InitialModuleUI initial)
        {
        }

        internal Object ParticleSystemValidator(Object[] references, Type objType, SerializedProperty property)
        {
            foreach (Object obj2 in references)
            {
                if (obj2 != null)
                {
                    GameObject obj3 = obj2 as GameObject;
                    if (obj3 != null)
                    {
                        ParticleSystem component = obj3.GetComponent<ParticleSystem>();
                        if (component != null)
                        {
                            return component;
                        }
                    }
                }
            }
            return null;
        }

        protected static bool PlusButton(Rect position)
        {
            return GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Plus");
        }

        private static Rect PrefixLabel(Rect totalPosition, GUIContent label)
        {
            Rect labelPosition = new Rect(totalPosition.x + EditorGUI.indent, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUI.indent, 13f);
            Rect rect2 = new Rect(totalPosition.x + EditorGUIUtility.labelWidth, totalPosition.y, totalPosition.width - EditorGUIUtility.labelWidth, totalPosition.height);
            EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, 0, ParticleSystemStyles.Get().label);
            return rect2;
        }

        private static void SelectMinMaxColorStateCallback(object obj)
        {
            ColorCallbackData data = (ColorCallbackData) obj;
            data.boolProp.boolValue = data.selectedState;
        }

        private static void SelectMinMaxCurveStateCallback(object obj)
        {
            CurveStateCallbackData data = (CurveStateCallbackData) obj;
            foreach (SerializedMinMaxCurve curve in data.minMaxCurves)
            {
                curve.state = data.selectedState;
            }
        }

        private static void SelectMinMaxGradientStateCallback(object obj)
        {
            GradientCallbackData data = (GradientCallbackData) obj;
            data.gradientProp.state = data.selectedState;
        }

        private void Setup(ParticleSystemUI owner, SerializedObject o, string name, string displayName, VisibilityState defaultVisibilityState)
        {
            this.m_ParticleSystemUI = owner;
            this.m_DisplayName = displayName;
            if (this is RendererModuleUI)
            {
                this.m_Enabled = base.GetProperty0("m_Enabled");
            }
            else
            {
                this.m_Enabled = base.GetProperty("enabled");
            }
            this.m_VisibilityState = (VisibilityState) SessionState.GetInt(base.GetUniqueModuleName(), (int) defaultVisibilityState);
            this.CheckVisibilityState();
            if (this.foldout)
            {
                this.Init();
            }
        }

        protected virtual void SetVisibilityState(VisibilityState newState)
        {
            if (newState != this.m_VisibilityState)
            {
                if (newState == VisibilityState.VisibleAndFolded)
                {
                    ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
                    foreach (SerializedProperty property in this.m_ModuleCurves)
                    {
                        if (particleSystemCurveEditor.IsAdded(property))
                        {
                            this.m_CurvesRemovedWhenFolded.Add(property);
                            particleSystemCurveEditor.SetVisible(property, false);
                        }
                    }
                    particleSystemCurveEditor.Refresh();
                }
                else if (newState == VisibilityState.VisibleAndFoldedOut)
                {
                    ParticleSystemCurveEditor editor2 = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
                    foreach (SerializedProperty property2 in this.m_CurvesRemovedWhenFolded)
                    {
                        editor2.SetVisible(property2, true);
                    }
                    this.m_CurvesRemovedWhenFolded.Clear();
                    editor2.Refresh();
                }
                this.m_VisibilityState = newState;
                SessionState.SetInt(base.GetUniqueModuleName(), (int) this.m_VisibilityState);
                if (newState == VisibilityState.VisibleAndFoldedOut)
                {
                    this.Init();
                }
            }
        }

        private static Rect SubtractPopupWidth(Rect position)
        {
            position.width -= 14f;
            return position;
        }

        private static bool Toggle(Rect rect, SerializedProperty boolProp)
        {
            Color color = GUI.color;
            if (boolProp.isAnimated)
            {
                GUI.color = AnimationMode.animatedPropertyColor;
            }
            bool boolValue = boolProp.boolValue;
            bool flag2 = EditorGUI.Toggle(rect, boolValue, ParticleSystemStyles.Get().toggle);
            if (boolValue != flag2)
            {
                boolProp.boolValue = flag2;
            }
            GUI.color = color;
            return flag2;
        }

        public virtual void UpdateCullingSupportedString(ref string text)
        {
        }

        public virtual void Validate()
        {
        }

        public string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
        }

        public bool enabled
        {
            get
            {
                return this.m_Enabled.boolValue;
            }
            set
            {
                if (this.m_Enabled.boolValue != value)
                {
                    this.m_Enabled.boolValue = value;
                    if (value)
                    {
                        this.OnModuleEnable();
                    }
                    else
                    {
                        this.OnModuleDisable();
                    }
                }
            }
        }

        public bool foldout
        {
            get
            {
                return (this.m_VisibilityState == VisibilityState.VisibleAndFoldedOut);
            }
            set
            {
                this.SetVisibilityState(!value ? VisibilityState.VisibleAndFolded : VisibilityState.VisibleAndFoldedOut);
            }
        }

        public string toolTip
        {
            get
            {
                return this.m_ToolTip;
            }
        }

        public bool visibleUI
        {
            get
            {
                return (this.m_VisibilityState != VisibilityState.NotVisible);
            }
            set
            {
                this.SetVisibilityState(!value ? VisibilityState.NotVisible : VisibilityState.VisibleAndFolded);
            }
        }

        private class ColorCallbackData
        {
            public SerializedProperty boolProp;
            public bool selectedState;

            public ColorCallbackData(bool state, SerializedProperty bp)
            {
                this.boolProp = bp;
                this.selectedState = state;
            }
        }

        public delegate bool CurveFieldMouseDownCallback(int button, Rect drawRect, Rect curveRanges);

        private class CurveStateCallbackData
        {
            public SerializedMinMaxCurve[] minMaxCurves;
            public MinMaxCurveState selectedState;

            public CurveStateCallbackData(MinMaxCurveState state, SerializedMinMaxCurve[] curves)
            {
                this.minMaxCurves = curves;
                this.selectedState = state;
            }
        }

        private class GradientCallbackData
        {
            public SerializedMinMaxGradient gradientProp;
            public MinMaxGradientState selectedState;

            public GradientCallbackData(MinMaxGradientState state, SerializedMinMaxGradient p)
            {
                this.gradientProp = p;
                this.selectedState = state;
            }
        }

        public enum VisibilityState
        {
            NotVisible,
            VisibleAndFolded,
            VisibleAndFoldedOut
        }
    }
}

