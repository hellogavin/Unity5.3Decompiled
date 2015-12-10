namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LayoutDropdownWindow : PopupWindowContent
    {
        private static string[] kHLabels = new string[] { "custom", "left", "center", "right", "stretch", "%" };
        private static float[] kPivotsForModes = new float[] { 0f, 0.5f, 1f, 0.5f, 0.5f };
        private const int kTopPartHeight = 0x26;
        private static string[] kVLabels = new string[] { "custom", "top", "middle", "bottom", "stretch", "%" };
        private SerializedProperty m_AnchorMax;
        private SerializedProperty m_AnchorMin;
        private Vector2[,] m_InitValues;
        private SerializedProperty m_Pivot;
        private SerializedProperty m_Position;
        private SerializedProperty m_SizeDelta;
        private static Styles s_Styles;

        public LayoutDropdownWindow(SerializedObject so)
        {
            this.m_AnchorMin = so.FindProperty("m_AnchorMin");
            this.m_AnchorMax = so.FindProperty("m_AnchorMax");
            this.m_Position = so.FindProperty("m_Position");
            this.m_SizeDelta = so.FindProperty("m_SizeDelta");
            this.m_Pivot = so.FindProperty("m_Pivot");
            this.m_InitValues = new Vector2[so.targetObjects.Length, 4];
            for (int i = 0; i < so.targetObjects.Length; i++)
            {
                RectTransform transform = so.targetObjects[i] as RectTransform;
                this.m_InitValues[i, 0] = transform.anchorMin;
                this.m_InitValues[i, 1] = transform.anchorMax;
                this.m_InitValues[i, 2] = transform.anchoredPosition;
                this.m_InitValues[i, 3] = transform.sizeDelta;
            }
        }

        private static void DrawArrow(Rect lineRect)
        {
            GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
            if (lineRect.width == 1f)
            {
                GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.y + 1f, 3f, 1f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.y + 2f, 5f, 1f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.x - 1f, lineRect.yMax - 2f, 3f, 1f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.x - 2f, lineRect.yMax - 3f, 5f, 1f), EditorGUIUtility.whiteTexture);
            }
            else
            {
                GUI.DrawTexture(new Rect(lineRect.x + 1f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.x + 2f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.xMax - 2f, lineRect.y - 1f, 1f, 3f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(lineRect.xMax - 3f, lineRect.y - 2f, 1f, 5f), EditorGUIUtility.whiteTexture);
            }
        }

        internal static void DrawLayoutMode(Rect position, LayoutMode hMode, LayoutMode vMode)
        {
            DrawLayoutMode(position, hMode, vMode, false, false);
        }

        internal static void DrawLayoutMode(Rect position, LayoutMode hMode, LayoutMode vMode, bool doPivot)
        {
            DrawLayoutMode(position, hMode, vMode, doPivot, false);
        }

        internal static unsafe void DrawLayoutMode(Rect position, LayoutMode hMode, LayoutMode vMode, bool doPivot, bool doPosition)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Color color = GUI.color;
            int num = (int) Mathf.Min(position.width, position.height);
            if ((num % 2) == 0)
            {
                num--;
            }
            int num2 = num / 2;
            if ((num2 % 2) == 0)
            {
                num2++;
            }
            Vector2 vector = (Vector2) (num * Vector2.one);
            Vector2 vector2 = (Vector2) (num2 * Vector2.one);
            Vector2 vector3 = (Vector2) ((position.size - vector) / 2f);
            vector3.x = Mathf.Floor(vector3.x);
            vector3.y = Mathf.Floor(vector3.y);
            Vector2 vector4 = (Vector2) ((position.size - vector2) / 2f);
            vector4.x = Mathf.Floor(vector4.x);
            vector4.y = Mathf.Floor(vector4.y);
            Rect rect = new Rect(position.x + vector3.x, position.y + vector3.y, vector.x, vector.y);
            Rect rect2 = new Rect(position.x + vector4.x, position.y + vector4.y, vector2.x, vector2.y);
            if (doPosition)
            {
                for (int j = 0; j < 2; j++)
                {
                    int num5;
                    float num6;
                    switch (((j != 0) ? vMode : hMode))
                    {
                        case LayoutMode.Min:
                        {
                            ref Vector2 vectorRef;
                            Vector2 center = rect2.center;
                            num6 = vectorRef[num5];
                            (vectorRef = (Vector2) &center)[num5 = j] = num6 + (rect.min[j] - rect2.min[j]);
                            rect2.center = center;
                            break;
                        }
                        case LayoutMode.Max:
                        {
                            ref Vector2 vectorRef2;
                            Vector2 vector6 = rect2.center;
                            num6 = vectorRef2[num5];
                            (vectorRef2 = (Vector2) &vector6)[num5 = j] = num6 + (rect.max[j] - rect2.max[j]);
                            rect2.center = vector6;
                            break;
                        }
                        case LayoutMode.Stretch:
                        {
                            Vector2 min = rect2.min;
                            Vector2 max = rect2.max;
                            min[j] = rect.min[j];
                            max[j] = rect.max[j];
                            rect2.min = min;
                            rect2.max = max;
                            break;
                        }
                    }
                }
            }
            Rect rect3 = new Rect();
            Vector2 zero = Vector2.zero;
            Vector2 vector10 = Vector2.zero;
            for (int i = 0; i < 2; i++)
            {
                switch (((i != 0) ? vMode : hMode))
                {
                    case LayoutMode.Min:
                        zero[i] = rect.min[i] + 0.5f;
                        vector10[i] = rect.min[i] + 0.5f;
                        break;

                    case LayoutMode.Middle:
                        zero[i] = rect.center[i];
                        vector10[i] = rect.center[i];
                        break;

                    case LayoutMode.Max:
                        zero[i] = rect.max[i] - 0.5f;
                        vector10[i] = rect.max[i] - 0.5f;
                        break;

                    case LayoutMode.Stretch:
                        zero[i] = rect.min[i] + 0.5f;
                        vector10[i] = rect.max[i] - 0.5f;
                        break;
                }
            }
            rect3.min = zero;
            rect3.max = vector10;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = s_Styles.parentColor * color;
                s_Styles.frame.Draw(rect, false, false, false, false);
            }
            if ((hMode != LayoutMode.Undefined) && (hMode != LayoutMode.Stretch))
            {
                GUI.color = s_Styles.simpleAnchorColor * color;
                GUI.DrawTexture(new Rect(rect3.xMin - 0.5f, rect.y + 1f, 1f, rect.height - 2f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect3.xMax - 0.5f, rect.y + 1f, 1f, rect.height - 2f), EditorGUIUtility.whiteTexture);
            }
            if ((vMode != LayoutMode.Undefined) && (vMode != LayoutMode.Stretch))
            {
                GUI.color = s_Styles.simpleAnchorColor * color;
                GUI.DrawTexture(new Rect(rect.x + 1f, rect3.yMin - 0.5f, rect.width - 2f, 1f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect.x + 1f, rect3.yMax - 0.5f, rect.width - 2f, 1f), EditorGUIUtility.whiteTexture);
            }
            if (hMode == LayoutMode.Stretch)
            {
                GUI.color = s_Styles.stretchAnchorColor * color;
                DrawArrow(new Rect(rect2.x + 1f, rect2.center.y - 0.5f, rect2.width - 2f, 1f));
            }
            if (vMode == LayoutMode.Stretch)
            {
                GUI.color = s_Styles.stretchAnchorColor * color;
                DrawArrow(new Rect(rect2.center.x - 0.5f, rect2.y + 1f, 1f, rect2.height - 2f));
            }
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = s_Styles.selfColor * color;
                s_Styles.frame.Draw(rect2, false, false, false, false);
            }
            if ((doPivot && (hMode != LayoutMode.Undefined)) && (vMode != LayoutMode.Undefined))
            {
                float x = Mathf.Lerp(rect2.xMin + 0.5f, rect2.xMax - 0.5f, kPivotsForModes[(int) hMode]);
                Vector2 vector11 = new Vector2(x, Mathf.Lerp(rect2.yMin + 0.5f, rect2.yMax - 0.5f, kPivotsForModes[(int) vMode]));
                GUI.color = s_Styles.pivotColor * color;
                GUI.DrawTexture(new Rect(vector11.x - 2.5f, vector11.y - 1.5f, 5f, 3f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(vector11.x - 1.5f, vector11.y - 2.5f, 3f, 5f), EditorGUIUtility.whiteTexture);
            }
            if ((hMode != LayoutMode.Undefined) && (vMode != LayoutMode.Undefined))
            {
                GUI.color = s_Styles.anchorCornerColor * color;
                GUI.DrawTexture(new Rect(rect3.xMin - 1.5f, rect3.yMin - 1.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect3.xMax - 0.5f, rect3.yMin - 1.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect3.xMin - 1.5f, rect3.yMax - 0.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(rect3.xMax - 0.5f, rect3.yMax - 0.5f, 2f, 2f), EditorGUIUtility.whiteTexture);
            }
            GUI.color = color;
        }

        internal static void DrawLayoutMode(Rect rect, SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta)
        {
            LayoutMode hMode = GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 0);
            LayoutMode vMode = SwappedVMode(GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 1));
            DrawLayoutMode(rect, hMode, vMode);
        }

        internal static void DrawLayoutModeHeaderOutsideRect(Rect position, int axis, LayoutMode mode)
        {
            Rect rect = new Rect(position.x, position.y - 16f, position.width, 16f);
            Matrix4x4 matrix = GUI.matrix;
            if (axis == 1)
            {
                GUIUtility.RotateAroundPivot(-90f, position.center);
            }
            int index = ((int) mode) + 1;
            GUI.Label(rect, (axis != 0) ? kVLabels[index] : kHLabels[index], s_Styles.label);
            GUI.matrix = matrix;
        }

        internal static void DrawLayoutModeHeadersOutsideRect(Rect rect, SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta)
        {
            LayoutMode mode = GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 0);
            LayoutMode mode2 = SwappedVMode(GetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, 1));
            DrawLayoutModeHeaderOutsideRect(rect, 0, mode);
            DrawLayoutModeHeaderOutsideRect(rect, 1, mode2);
        }

        private static LayoutMode GetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, int axis)
        {
            if ((anchorMin.vector2Value[axis] == 0f) && (anchorMax.vector2Value[axis] == 0f))
            {
                return LayoutMode.Min;
            }
            if ((anchorMin.vector2Value[axis] == 0.5f) && (anchorMax.vector2Value[axis] == 0.5f))
            {
                return LayoutMode.Middle;
            }
            if ((anchorMin.vector2Value[axis] == 1f) && (anchorMax.vector2Value[axis] == 1f))
            {
                return LayoutMode.Max;
            }
            if ((anchorMin.vector2Value[axis] == 0f) && (anchorMax.vector2Value[axis] == 1f))
            {
                return LayoutMode.Stretch;
            }
            return LayoutMode.Undefined;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(262f, 300f);
        }

        public override void OnClose()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.editorWindow.Repaint));
        }

        public override void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Return))
            {
                base.editorWindow.Close();
            }
            GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 16f), new GUIContent("Anchor Presets"), EditorStyles.boldLabel);
            GUI.Label(new Rect(rect.x + 5f, (rect.y + 3f) + 16f, rect.width - 10f, 16f), new GUIContent("Shift: Also set pivot     Alt: Also set position"), EditorStyles.label);
            Color color = GUI.color;
            GUI.color = s_Styles.tableLineColor * color;
            GUI.DrawTexture(new Rect(0f, 37f, 400f, 1f), EditorGUIUtility.whiteTexture);
            GUI.color = color;
            GUI.BeginGroup(new Rect(rect.x, rect.y + 38f, rect.width, rect.height - 38f));
            this.TableGUI(rect);
            GUI.EndGroup();
        }

        public override void OnOpen()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.editorWindow.Repaint));
        }

        private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutMode layoutMode)
        {
            SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, false, false, null);
        }

        private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutMode layoutMode, bool doPivot)
        {
            SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, doPivot, false, null);
        }

        private static void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutMode layoutMode, bool doPivot, bool doPosition)
        {
            SetLayoutModeForAxis(anchorMin, anchorMax, position, sizeDelta, pivot, axis, layoutMode, doPivot, doPosition, null);
        }

        private static unsafe void SetLayoutModeForAxis(SerializedProperty anchorMin, SerializedProperty anchorMax, SerializedProperty position, SerializedProperty sizeDelta, SerializedProperty pivot, int axis, LayoutMode layoutMode, bool doPivot, bool doPosition, Vector2[,] defaultValues)
        {
            anchorMin.serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < anchorMin.serializedObject.targetObjects.Length; i++)
            {
                RectTransform objectToUndo = anchorMin.serializedObject.targetObjects[i] as RectTransform;
                Undo.RecordObject(objectToUndo, "Change Rectangle Anchors");
                if ((doPosition && (defaultValues != null)) && (defaultValues.Length > i))
                {
                    Vector2 anchoredPosition = objectToUndo.anchorMin;
                    anchoredPosition[axis] = defaultValues[i, 0][axis];
                    objectToUndo.anchorMin = anchoredPosition;
                    anchoredPosition = objectToUndo.anchorMax;
                    anchoredPosition[axis] = defaultValues[i, 1][axis];
                    objectToUndo.anchorMax = anchoredPosition;
                    anchoredPosition = objectToUndo.anchoredPosition;
                    anchoredPosition[axis] = defaultValues[i, 2][axis];
                    objectToUndo.anchoredPosition = anchoredPosition;
                    anchoredPosition = objectToUndo.sizeDelta;
                    anchoredPosition[axis] = defaultValues[i, 3][axis];
                    objectToUndo.sizeDelta = anchoredPosition;
                }
                if (doPivot && (layoutMode != LayoutMode.Undefined))
                {
                    RectTransformEditor.SetPivotSmart(objectToUndo, kPivotsForModes[(int) layoutMode], axis, true, true);
                }
                Vector2 zero = Vector2.zero;
                switch (layoutMode)
                {
                    case LayoutMode.Min:
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 0f, axis, false, true, true);
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 0f, axis, true, true, true);
                        zero = objectToUndo.offsetMin;
                        EditorUtility.SetDirty(objectToUndo);
                        break;

                    case LayoutMode.Middle:
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 0.5f, axis, false, true, true);
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 0.5f, axis, true, true, true);
                        zero = (Vector2) ((objectToUndo.offsetMin + objectToUndo.offsetMax) * 0.5f);
                        EditorUtility.SetDirty(objectToUndo);
                        break;

                    case LayoutMode.Max:
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 1f, axis, false, true, true);
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 1f, axis, true, true, true);
                        zero = objectToUndo.offsetMax;
                        EditorUtility.SetDirty(objectToUndo);
                        break;

                    case LayoutMode.Stretch:
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 0f, axis, false, true, true);
                        RectTransformEditor.SetAnchorSmart(objectToUndo, 1f, axis, true, true, true);
                        zero = (Vector2) ((objectToUndo.offsetMin + objectToUndo.offsetMax) * 0.5f);
                        EditorUtility.SetDirty(objectToUndo);
                        break;
                }
                if (doPosition)
                {
                    ref Vector2 vectorRef;
                    int num2;
                    Vector2 vector3 = objectToUndo.anchoredPosition;
                    float num3 = vectorRef[num2];
                    (vectorRef = (Vector2) &vector3)[num2 = axis] = num3 - zero[axis];
                    objectToUndo.anchoredPosition = vector3;
                    if (layoutMode == LayoutMode.Stretch)
                    {
                        Vector2 vector4 = objectToUndo.sizeDelta;
                        vector4[axis] = 0f;
                        objectToUndo.sizeDelta = vector4;
                    }
                }
            }
            anchorMin.serializedObject.Update();
        }

        private static LayoutMode SwappedVMode(LayoutMode vMode)
        {
            if (vMode == LayoutMode.Min)
            {
                return LayoutMode.Max;
            }
            if (vMode == LayoutMode.Max)
            {
                return LayoutMode.Min;
            }
            return vMode;
        }

        private void TableGUI(Rect rect)
        {
            int num = 6;
            int num2 = 0x1f + (num * 2);
            int num3 = 0;
            int[] numArray = new int[] { 15, 30, 30, 30, 0x2d, 0x2d };
            Color color = GUI.color;
            int num4 = 0x3e;
            GUI.color = s_Styles.tableHeaderColor * color;
            GUI.DrawTexture(new Rect(0f, 0f, 400f, (float) num4), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(0f, 0f, (float) num4, 400f), EditorGUIUtility.whiteTexture);
            GUI.color = s_Styles.tableLineColor * color;
            GUI.DrawTexture(new Rect(0f, (float) num4, 400f, 1f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect((float) num4, 0f, 1f, 400f), EditorGUIUtility.whiteTexture);
            GUI.color = color;
            LayoutMode mode = GetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, 0);
            LayoutMode mode2 = SwappedVMode(GetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, 1));
            bool shift = Event.current.shift;
            bool alt = Event.current.alt;
            int num5 = 5;
            for (int i = 0; i < num5; i++)
            {
                LayoutMode mode3 = (LayoutMode) (i - 1);
                for (int j = 0; j < num5; j++)
                {
                    LayoutMode mode4 = (LayoutMode) (j - 1);
                    if (((i != 0) || (j != 0)) || ((mode2 < LayoutMode.Min) || (mode < LayoutMode.Min)))
                    {
                        Rect position = new Rect((float) ((i * (num2 + num3)) + numArray[i]), (float) ((j * (num2 + num3)) + numArray[j]), (float) num2, (float) num2);
                        if ((j == 0) && ((i != 0) || (mode == LayoutMode.Undefined)))
                        {
                            DrawLayoutModeHeaderOutsideRect(position, 0, mode3);
                        }
                        if ((i == 0) && ((j != 0) || (mode2 == LayoutMode.Undefined)))
                        {
                            DrawLayoutModeHeaderOutsideRect(position, 1, mode4);
                        }
                        bool flag3 = (mode3 == mode) && (mode4 == mode2);
                        bool flag4 = false;
                        if ((i == 0) && (mode4 == mode2))
                        {
                            flag4 = true;
                        }
                        if ((j == 0) && (mode3 == mode))
                        {
                            flag4 = true;
                        }
                        if (Event.current.type == EventType.Repaint)
                        {
                            if (flag3)
                            {
                                GUI.color = Color.white * color;
                                s_Styles.frame.Draw(position, false, false, false, false);
                            }
                            else if (flag4)
                            {
                                GUI.color = new Color(1f, 1f, 1f, 0.7f) * color;
                                s_Styles.frame.Draw(position, false, false, false, false);
                            }
                        }
                        DrawLayoutMode(new Rect(position.x + num, position.y + num, position.width - (num * 2), position.height - (num * 2)), mode3, mode4, shift, alt);
                        int clickCount = Event.current.clickCount;
                        if (GUI.Button(position, GUIContent.none, GUIStyle.none))
                        {
                            SetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, this.m_Pivot, 0, mode3, shift, alt, this.m_InitValues);
                            SetLayoutModeForAxis(this.m_AnchorMin, this.m_AnchorMax, this.m_Position, this.m_SizeDelta, this.m_Pivot, 1, SwappedVMode(mode4), shift, alt, this.m_InitValues);
                            if (clickCount == 2)
                            {
                                base.editorWindow.Close();
                            }
                            else
                            {
                                base.editorWindow.Repaint();
                            }
                        }
                    }
                }
            }
            GUI.color = color;
        }

        public enum LayoutMode
        {
            Max = 2,
            Middle = 1,
            Min = 0,
            Stretch = 3,
            Undefined = -1
        }

        private class Styles
        {
            public Color anchorCornerColor;
            public GUIStyle frame = new GUIStyle();
            public GUIStyle label = new GUIStyle(EditorStyles.miniLabel);
            public Color parentColor;
            public Color pivotColor;
            public Color selfColor;
            public Color simpleAnchorColor;
            public Color stretchAnchorColor;
            public Color tableHeaderColor;
            public Color tableLineColor;

            public Styles()
            {
                Texture2D textured = new Texture2D(4, 4);
                Color[] colors = new Color[] { Color.white, Color.white, Color.white, Color.white, Color.white, Color.clear, Color.clear, Color.white, Color.white, Color.clear, Color.clear, Color.white, Color.white, Color.white, Color.white, Color.white };
                textured.SetPixels(colors);
                textured.filterMode = FilterMode.Point;
                textured.Apply();
                textured.hideFlags = HideFlags.HideAndDontSave;
                this.frame.normal.background = textured;
                this.frame.border = new RectOffset(2, 2, 2, 2);
                this.label.alignment = TextAnchor.LowerCenter;
                if (EditorGUIUtility.isProSkin)
                {
                    this.tableHeaderColor = new Color(0.18f, 0.18f, 0.18f, 1f);
                    this.tableLineColor = new Color(1f, 1f, 1f, 0.3f);
                    this.parentColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                    this.selfColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                    this.simpleAnchorColor = new Color(0.7f, 0.3f, 0.3f, 1f);
                    this.stretchAnchorColor = new Color(0f, 0.6f, 0.8f, 1f);
                    this.anchorCornerColor = new Color(0.8f, 0.6f, 0f, 1f);
                    this.pivotColor = new Color(0f, 0.6f, 0.8f, 1f);
                }
                else
                {
                    this.tableHeaderColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                    this.tableLineColor = new Color(0f, 0f, 0f, 0.5f);
                    this.parentColor = new Color(0.55f, 0.55f, 0.55f, 1f);
                    this.selfColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                    this.simpleAnchorColor = new Color(0.8f, 0.3f, 0.3f, 1f);
                    this.stretchAnchorColor = new Color(0.2f, 0.5f, 0.9f, 1f);
                    this.anchorCornerColor = new Color(0.6f, 0.4f, 0f, 1f);
                    this.pivotColor = new Color(0.2f, 0.5f, 0.9f, 1f);
                }
            }
        }
    }
}

