namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(RectTransform))]
    internal class RectTransformEditor : Editor
    {
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache34;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache35;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache36;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache37;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache38;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache39;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache3A;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache3B;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache3C;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache3D;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache3E;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache3F;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache40;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache41;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache42;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache43;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache44;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache45;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache46;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache47;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache48;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache49;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache4A;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache4B;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache4C;
        [CompilerGenerated]
        private static FloatSetter <>f__am$cache4D;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache4E;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache4F;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache50;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache51;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache52;
        [CompilerGenerated]
        private static FloatGetter <>f__am$cache53;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1D;
        private static Color kAnchorColor = new Color(1f, 1f, 1f, 1f);
        private static Color kAnchorLineColor = new Color(1f, 1f, 1f, 0.6f);
        private const float kDottedLineSize = 5f;
        private static float kDropdownSize = 49f;
        private const string kLockRectPrefName = "RectTransformEditor.lockRect";
        private static Color kParentColor = new Color(1f, 1f, 1f, 0.6f);
        private static Color kRectInParentSpaceColor = new Color(1f, 1f, 1f, 0.4f);
        private static Color kShadowColor = new Color(0f, 0f, 0f, 0.5f);
        private static Vector2 kShadowOffset = new Vector2(1f, -1f);
        private const string kShowAnchorPropsPrefName = "RectTransformEditor.showAnchorProperties";
        private static Color kSiblingColor = new Color(1f, 1f, 1f, 0.2f);
        private SerializedProperty m_AnchoredPosition;
        private SerializedProperty m_AnchorMax;
        private SerializedProperty m_AnchorMin;
        private AnimBool m_ChangingAnchors = new AnimBool();
        private AnimBool m_ChangingBottom = new AnimBool();
        private AnimBool m_ChangingHeight = new AnimBool();
        private AnimBool m_ChangingLeft = new AnimBool();
        private AnimBool m_ChangingPivot = new AnimBool();
        private AnimBool m_ChangingPosX = new AnimBool();
        private AnimBool m_ChangingPosY = new AnimBool();
        private AnimBool m_ChangingRight = new AnimBool();
        private AnimBool m_ChangingTop = new AnimBool();
        private AnimBool m_ChangingWidth = new AnimBool();
        private LayoutDropdownWindow m_DropdownWindow;
        private Dictionary<int, AnimBool> m_KeyboardControlIDs = new Dictionary<int, AnimBool>();
        private SerializedProperty m_LocalPositionZ;
        private SerializedProperty m_LocalScale;
        private SerializedProperty m_Pivot;
        private bool m_RawEditMode;
        private TransformRotationGUI m_RotationGUI;
        private bool m_ShowLayoutOptions;
        private SerializedProperty m_SizeDelta;
        private int m_TargetCount;
        private static AnchorFusedState s_AnchorFusedState = AnchorFusedState.None;
        private static Vector3[] s_Corners = new Vector3[4];
        private static bool s_DragAnchorsTogether;
        private static int s_FloatFieldHash = "EditorTextField".GetHashCode();
        private static int s_FoldoutHash = "Foldout".GetHashCode();
        private static float s_ParentDragId = 0f;
        private static Rect s_ParentDragOrigRect = new Rect();
        private static Rect s_ParentDragPreviewRect = new Rect();
        private static RectTransform s_ParentDragRectTransform = null;
        private static float s_ParentDragTime = 0f;
        private static int s_ParentRectPreviewHandlesHash = "ParentRectPreviewDragHandles".GetHashCode();
        private static bool[] s_ScaleDisabledMask = new bool[3];
        private static Vector2 s_StartDragAnchorMax;
        private static Vector2 s_StartDragAnchorMin;
        private static Vector2 s_StartMousePos;
        private static Vector3 s_StartMouseWorldPos;
        private static Vector3 s_StartPosition;
        private static Styles s_Styles;
        private static GUIContent[] s_XYLabels = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
        private static GUIContent[] s_XYZLabels = new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };

        private void AllAnchorsSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, Transform transform)
        {
            Handles.color = kParentColor;
            this.DrawRect(guiParent.rect, parentSpace, false);
            Handles.color = kSiblingColor;
            IEnumerator enumerator = parentSpace.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.gameObject.activeInHierarchy)
                    {
                        RectTransform component = current.GetComponent<RectTransform>();
                        if (component != null)
                        {
                            Rect rect = component.rect;
                            rect.x += component.transform.localPosition.x;
                            rect.y += component.transform.localPosition.y;
                            this.DrawRect(component.rect, component, false);
                            if (component != transform)
                            {
                                this.AnchorsSceneGUI(component, guiParent, parentSpace, false);
                            }
                        }
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
            Handles.color = kAnchorColor;
            this.AnchorsSceneGUI(gui, guiParent, parentSpace, true);
        }

        private static bool AnchorAllowedOutsideParent(int axis, int minmax)
        {
            if (EditorGUI.actionKey || (GUIUtility.hotControl == 0))
            {
                return true;
            }
            float num = (minmax != 0) ? s_StartDragAnchorMax[axis] : s_StartDragAnchorMin[axis];
            return ((num < -0.001f) || (num > 1.001f));
        }

        private int AnchorPopup(Rect position, string label, int selected, string[] displayedOptions)
        {
            EditorGUIUtility.labelWidth = 12f;
            int num = EditorGUI.Popup(position, label, selected, displayedOptions);
            EditorGUIUtility.labelWidth = 0f;
            return num;
        }

        private unsafe void AnchorSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, bool interactive, int minmaxX, int minmaxY, int id)
        {
            Vector3 anchor = (Vector3) new Vector2();
            anchor.x = (minmaxX != 0) ? gui.anchorMax.x : gui.anchorMin.x;
            anchor.y = (minmaxY != 0) ? gui.anchorMax.y : gui.anchorMin.y;
            anchor = this.GetAnchorLocal(guiParent, anchor);
            anchor = parentSpace.TransformPoint(anchor);
            float handleSize = 0.05f * HandleUtility.GetHandleSize(anchor);
            if (minmaxX < 2)
            {
                anchor += (Vector3) ((parentSpace.right * handleSize) * ((minmaxX * 2) - 1));
            }
            if (minmaxY < 2)
            {
                anchor += (Vector3) ((parentSpace.up * handleSize) * ((minmaxY * 2) - 1));
            }
            if ((minmaxX < 2) && (minmaxY < 2))
            {
                this.DrawAnchor(anchor, (Vector3) (((parentSpace.right * handleSize) * 2f) * ((minmaxX * 2) - 1)), (Vector3) (((parentSpace.up * handleSize) * 2f) * ((minmaxY * 2) - 1)));
            }
            if (interactive)
            {
                Event eventBefore = new Event(Event.current);
                EditorGUI.BeginChangeCheck();
                Vector3 position = Handles.Slider2D(id, anchor, parentSpace.forward, parentSpace.right, parentSpace.up, handleSize, null, Vector2.zero);
                if ((eventBefore.type == EventType.MouseDown) && (GUIUtility.hotControl == id))
                {
                    s_DragAnchorsTogether = EditorGUI.actionKey;
                    s_StartDragAnchorMin = gui.anchorMin;
                    s_StartDragAnchorMax = gui.anchorMax;
                    RectTransformSnapping.CalculateAnchorSnapValues(parentSpace, gui.transform, gui, minmaxX, minmaxY);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(gui, "Move Rectangle Anchors");
                    Vector2 vector3 = parentSpace.InverseTransformVector(position - anchor);
                    for (int i = 0; i <= 1; i++)
                    {
                        ref Vector2 vectorRef;
                        int num8;
                        float num9 = vectorRef[num8];
                        (vectorRef = (Vector2) &vector3)[num8 = i] = num9 / guiParent.rect.size[i];
                        int minmax = (i != 0) ? minmaxY : minmaxX;
                        bool isMax = minmax == 1;
                        float num4 = !isMax ? gui.anchorMin[i] : gui.anchorMax[i];
                        float num5 = num4 + vector3[i];
                        float num6 = num5;
                        if (!AnchorAllowedOutsideParent(i, minmax))
                        {
                            num6 = Mathf.Clamp01(num6);
                        }
                        if (minmax == 0)
                        {
                            num6 = Mathf.Min(num6, gui.anchorMax[i]);
                        }
                        if (minmax == 1)
                        {
                            num6 = Mathf.Max(num6, gui.anchorMin[i]);
                        }
                        float snapDistance = (HandleUtility.GetHandleSize(position) * 0.05f) / guiParent.rect.size[i];
                        snapDistance *= parentSpace.InverseTransformVector((i != null) ? Vector3.up : Vector3.right)[i];
                        num6 = RectTransformSnapping.SnapToGuides(num6, snapDistance, i);
                        bool enforceExactValue = num6 != num5;
                        num5 = num6;
                        if (minmax == 2)
                        {
                            SetAnchorSmart(gui, num5, i, false, !eventBefore.shift, enforceExactValue, false, s_DragAnchorsTogether);
                            SetAnchorSmart(gui, num5, i, true, !eventBefore.shift, enforceExactValue, false, s_DragAnchorsTogether);
                        }
                        else
                        {
                            SetAnchorSmart(gui, num5, i, isMax, !eventBefore.shift, enforceExactValue, true, s_DragAnchorsTogether);
                        }
                        EditorUtility.SetDirty(gui);
                        if (gui.drivenByObject != null)
                        {
                            RectTransform.SendReapplyDrivenProperties(gui);
                        }
                    }
                }
                this.SetFadingBasedOnMouseDownUp(ref this.m_ChangingAnchors, eventBefore);
            }
        }

        private void AnchorsSceneGUI(RectTransform gui, RectTransform guiParent, Transform parentSpace, bool interactive)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                s_AnchorFusedState = AnchorFusedState.None;
                if (gui.anchorMin == gui.anchorMax)
                {
                    s_AnchorFusedState = AnchorFusedState.All;
                }
                else if (gui.anchorMin.x == gui.anchorMax.x)
                {
                    s_AnchorFusedState = AnchorFusedState.Horizontal;
                }
                else if (gui.anchorMin.y == gui.anchorMax.y)
                {
                    s_AnchorFusedState = AnchorFusedState.Vertical;
                }
            }
            this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 0, GUIUtility.GetControlID(FocusType.Passive));
            this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 1, GUIUtility.GetControlID(FocusType.Passive));
            this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 0, GUIUtility.GetControlID(FocusType.Passive));
            this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 1, GUIUtility.GetControlID(FocusType.Passive));
            if (interactive)
            {
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                int id = GUIUtility.GetControlID(FocusType.Passive);
                int num3 = GUIUtility.GetControlID(FocusType.Passive);
                int num4 = GUIUtility.GetControlID(FocusType.Passive);
                int num5 = GUIUtility.GetControlID(FocusType.Passive);
                if (s_AnchorFusedState == AnchorFusedState.All)
                {
                    this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 2, controlID);
                }
                if (s_AnchorFusedState == AnchorFusedState.Horizontal)
                {
                    this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 0, id);
                    this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 2, 1, num3);
                }
                if (s_AnchorFusedState == AnchorFusedState.Vertical)
                {
                    this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 0, 2, num4);
                    this.AnchorSceneGUI(gui, guiParent, parentSpace, interactive, 1, 2, num5);
                }
            }
        }

        private void BlueprintButton(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            bool flag = GUI.Toggle(position, Tools.rectBlueprintMode, styles.blueprintContent, "ButtonLeft");
            if (EditorGUI.EndChangeCheck())
            {
                Tools.rectBlueprintMode = flag;
                Tools.RepaintAllToolViews();
            }
        }

        private void DrawAnchor(Vector3 pos, Vector3 right, Vector3 up)
        {
            pos -= (Vector3) (up * 0.5f);
            pos -= (Vector3) (right * 0.5f);
            up = (Vector3) (up * 1.4f);
            right = (Vector3) (right * 1.4f);
            Vector3[] points = new Vector3[] { pos, (pos + up) + (right * 0.5f), (pos + right) + (up * 0.5f), pos };
            RectHandles.DrawPolyLineWithShadow(kShadowColor, kShadowOffset, points);
        }

        private void DrawAnchorDistances(Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, float alpha)
        {
            if ((guiParent != null) && (alpha > 0f))
            {
                Color kAnchorColor = RectTransformEditor.kAnchorColor;
                kAnchorColor.a *= alpha;
                GUI.color = kAnchorColor;
                Vector3[,] vectorArray = new Vector3[2, 4];
                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 zero = Vector3.zero;
                        switch (k)
                        {
                            case 0:
                                zero = Vector3.zero;
                                break;

                            case 1:
                                zero = (Vector3) gui.anchorMin;
                                break;

                            case 2:
                                zero = (Vector3) gui.anchorMax;
                                break;

                            case 3:
                                zero = Vector3.one;
                                break;
                        }
                        zero[i] = gui.anchorMin[i];
                        zero = parentSpace.TransformPoint(this.GetAnchorLocal(guiParent, zero));
                        vectorArray[i, k] = zero;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    Vector3 vector2 = (Vector3) ((((j != 0) ? parentSpace.up : parentSpace.right) * size) * 2f);
                    int num4 = RoundToInt(gui.anchorMin[1 - j] * 100f);
                    int num5 = RoundToInt((gui.anchorMax[1 - j] - gui.anchorMin[1 - j]) * 100f);
                    int num6 = RoundToInt((1f - gui.anchorMax[1 - j]) * 100f);
                    if (num4 > 0)
                    {
                        this.DrawLabelBetweenPoints(vectorArray[j, 0] - vector2, vectorArray[j, 1] - vector2, GUIContent.Temp(num4.ToString() + "%"));
                    }
                    if (num5 > 0)
                    {
                        this.DrawLabelBetweenPoints(vectorArray[j, 1] - vector2, vectorArray[j, 2] - vector2, GUIContent.Temp(num5.ToString() + "%"));
                    }
                    if (num6 > 0)
                    {
                        this.DrawLabelBetweenPoints(vectorArray[j, 2] - vector2, vectorArray[j, 3] - vector2, GUIContent.Temp(num6.ToString() + "%"));
                    }
                }
            }
        }

        private void DrawAnchorRect(Transform parentSpace, RectTransform gui, RectTransform guiParent, int axis, float alpha)
        {
            if ((guiParent != null) && (alpha > 0f))
            {
                Color kAnchorLineColor = RectTransformEditor.kAnchorLineColor;
                kAnchorLineColor.a *= alpha;
                Handles.color = kAnchorLineColor;
                Vector3[,] vectorArray = new Vector3[2, 2];
                for (int i = 0; i < 2; i++)
                {
                    if ((i != 1) || (gui.anchorMin[axis] != gui.anchorMax[axis]))
                    {
                        vectorArray[i, 0][1 - axis] = Mathf.Min(0f, gui.anchorMin[1 - axis]);
                        vectorArray[i, 1][1 - axis] = Mathf.Max(1f, gui.anchorMax[1 - axis]);
                        for (int j = 0; j < 2; j++)
                        {
                            vectorArray[i, j][axis] = (i != 0) ? gui.anchorMax[axis] : gui.anchorMin[axis];
                            vectorArray[i, j] = parentSpace.TransformPoint(this.GetAnchorLocal(guiParent, vectorArray[i, j]));
                        }
                        RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vectorArray[i, 0], vectorArray[i, 1], 5f);
                    }
                }
            }
        }

        private void DrawLabelBetweenPoints(Vector3 pA, Vector3 pB, GUIContent label)
        {
            if (pA != pB)
            {
                Vector2 vector = HandleUtility.WorldToGUIPoint(pA);
                Vector2 vector2 = HandleUtility.WorldToGUIPoint(pB);
                Vector2 pivotPoint = (Vector2) ((vector + vector2) * 0.5f);
                pivotPoint.x = Round(pivotPoint.x);
                pivotPoint.y = Round(pivotPoint.y);
                float angle = Mathf.Atan2(vector2.y - vector.y, vector2.x - vector.x) * 57.29578f;
                angle = Mathf.Repeat(angle + 89f, 180f) - 89f;
                Handles.BeginGUI();
                Matrix4x4 matrix = GUI.matrix;
                GUIStyle measuringLabelStyle = styles.measuringLabelStyle;
                measuringLabelStyle.alignment = TextAnchor.MiddleCenter;
                GUIUtility.RotateAroundPivot(angle, pivotPoint);
                EditorGUI.DropShadowLabel(new Rect(pivotPoint.x - 50f, pivotPoint.y - 9f, 100f, 16f), label, measuringLabelStyle);
                GUI.matrix = matrix;
                Handles.EndGUI();
            }
        }

        private void DrawPositionDistances(Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, int axis, int side, float alpha)
        {
            if ((guiParent != null) && (alpha > 0f))
            {
                Vector3 vector;
                Vector3 vector2;
                float num;
                Color kAnchorLineColor = RectTransformEditor.kAnchorLineColor;
                kAnchorLineColor.a *= alpha;
                Handles.color = kAnchorLineColor;
                kAnchorLineColor = kAnchorColor;
                kAnchorLineColor.a *= alpha;
                GUI.color = kAnchorLineColor;
                if (side == 0)
                {
                    Vector2 vector3 = Rect.NormalizedToPoint(rectInParentSpace, gui.pivot);
                    vector = (Vector3) vector3;
                    vector2 = (Vector3) vector3;
                    vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMin[axis]);
                    num = gui.anchoredPosition[axis];
                }
                else
                {
                    Vector2 center = rectInParentSpace.center;
                    vector = (Vector3) center;
                    vector2 = (Vector3) center;
                    if (side == 1)
                    {
                        vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMin[axis]);
                        vector2[axis] = rectInParentSpace.min[axis];
                        num = gui.offsetMin[axis];
                    }
                    else
                    {
                        vector[axis] = Mathf.LerpUnclamped(guiParent.rect.min[axis], guiParent.rect.max[axis], gui.anchorMax[axis]);
                        vector2[axis] = rectInParentSpace.max[axis];
                        num = -gui.offsetMax[axis];
                    }
                }
                vector = parentSpace.TransformPoint(vector);
                vector2 = parentSpace.TransformPoint(vector2);
                RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vector, vector2, 5f);
                GUIContent label = new GUIContent(num.ToString());
                this.DrawLabelBetweenPoints(vector, vector2, label);
            }
        }

        private void DrawRect(Rect rect, Transform space, bool dotted)
        {
            Vector3 vector = space.TransformPoint((Vector3) new Vector2(rect.x, rect.y));
            Vector3 vector2 = space.TransformPoint((Vector3) new Vector2(rect.x, rect.yMax));
            Vector3 vector3 = space.TransformPoint((Vector3) new Vector2(rect.xMax, rect.yMax));
            Vector3 vector4 = space.TransformPoint((Vector3) new Vector2(rect.xMax, rect.y));
            if (!dotted)
            {
                Handles.DrawLine(vector, vector2);
                Handles.DrawLine(vector2, vector3);
                Handles.DrawLine(vector3, vector4);
                Handles.DrawLine(vector4, vector);
            }
            else
            {
                RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vector, vector2, 5f);
                RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vector2, vector3, 5f);
                RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vector3, vector4, 5f);
                RectHandles.DrawDottedLineWithShadow(kShadowColor, kShadowOffset, vector4, vector, 5f);
            }
        }

        private void DrawSizeDistances(Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent, float size, int axis, float alpha)
        {
            if (alpha > 0f)
            {
                Color kAnchorColor = RectTransformEditor.kAnchorColor;
                kAnchorColor.a *= alpha;
                GUI.color = kAnchorColor;
                if (userSpace == gui.transform)
                {
                    gui.GetWorldCorners(s_Corners);
                }
                else
                {
                    gui.GetLocalCorners(s_Corners);
                    for (int i = 0; i < 4; i++)
                    {
                        s_Corners[i] += gui.transform.localPosition;
                        s_Corners[i] = userSpace.TransformPoint(s_Corners[i]);
                    }
                }
                float num2 = gui.sizeDelta[axis];
                GUIContent label = new GUIContent(num2.ToString());
                Vector3 vector = (Vector3) ((((axis != 0) ? userSpace.right : userSpace.up) * size) * 2f);
                this.DrawLabelBetweenPoints(s_Corners[0] + vector, s_Corners[(axis != 0) ? 1 : 3] + vector, label);
            }
        }

        private void DrawSizes(Rect rectInUserSpace, Transform userSpace, Rect rectInParentSpace, Transform parentSpace, RectTransform gui, RectTransform guiParent)
        {
            float size = 0.05f * HandleUtility.GetHandleSize(parentSpace.position);
            bool flag = gui.anchorMin.x != gui.anchorMax.x;
            bool flag2 = gui.anchorMin.y != gui.anchorMax.y;
            float[] values = new float[] { this.m_ChangingPosX.faded, this.m_ChangingLeft.faded, this.m_ChangingRight.faded, this.m_ChangingAnchors.faded };
            float alpha = Mathf.Max(values);
            this.DrawAnchorRect(parentSpace, gui, guiParent, 0, alpha);
            float[] singleArray2 = new float[] { this.m_ChangingPosY.faded, this.m_ChangingTop.faded, this.m_ChangingBottom.faded, this.m_ChangingAnchors.faded };
            alpha = Mathf.Max(singleArray2);
            this.DrawAnchorRect(parentSpace, gui, guiParent, 1, alpha);
            this.DrawAnchorDistances(parentSpace, gui, guiParent, size, this.m_ChangingAnchors.faded);
            if (flag)
            {
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 1, this.m_ChangingLeft.faded);
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 2, this.m_ChangingRight.faded);
            }
            else
            {
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, 0, this.m_ChangingPosX.faded);
                this.DrawSizeDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 0, this.m_ChangingWidth.faded);
            }
            if (flag2)
            {
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 1, this.m_ChangingBottom.faded);
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 2, this.m_ChangingTop.faded);
            }
            else
            {
                this.DrawPositionDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, 0, this.m_ChangingPosY.faded);
                this.DrawSizeDistances(userSpace, rectInParentSpace, parentSpace, gui, guiParent, size, 1, this.m_ChangingHeight.faded);
            }
        }

        private void FloatField(Rect position, FloatGetter getter, FloatSetter setter, DrivenTransformProperties driven, GUIContent label)
        {
            <FloatField>c__AnonStorey91 storey = new <FloatField>c__AnonStorey91 {
                driven = driven,
                getter = getter
            };
            EditorGUI.BeginDisabledGroup(base.targets.Any<Object>(new Func<Object, bool>(storey.<>m__1A4)));
            float num = storey.getter(this.target as RectTransform);
            EditorGUI.showMixedValue = base.targets.Select<Object, float>(new Func<Object, float>(storey.<>m__1A5)).Distinct<float>().Count<float>() >= 2;
            EditorGUI.BeginChangeCheck();
            float f = EditorGUI.FloatField(position, label, num);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Inspector");
                foreach (RectTransform transform in base.targets)
                {
                    setter(transform, f);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void FloatFieldLabelAbove(Rect position, FloatGetter getter, FloatSetter setter, DrivenTransformProperties driven, GUIContent label)
        {
            <FloatFieldLabelAbove>c__AnonStorey90 storey = new <FloatFieldLabelAbove>c__AnonStorey90 {
                driven = driven,
                getter = getter
            };
            EditorGUI.BeginDisabledGroup(base.targets.Any<Object>(new Func<Object, bool>(storey.<>m__1A2)));
            float num = storey.getter(this.target as RectTransform);
            EditorGUI.showMixedValue = base.targets.Select<Object, float>(new Func<Object, float>(storey.<>m__1A3)).Distinct<float>().Count<float>() >= 2;
            EditorGUI.BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_FloatFieldHash, FocusType.Keyboard, position);
            Rect labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect rect2 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.HandlePrefixLabel(position, labelPosition, label, id);
            float f = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect2, labelPosition, id, num, EditorGUI.kFloatFieldFormatString, EditorStyles.textField, true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Inspector");
                foreach (RectTransform transform in base.targets)
                {
                    setter(transform, f);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private Vector3 GetAnchorLocal(RectTransform guiParent, Vector2 anchor)
        {
            return (Vector3) NormalizedToPointUnclamped(guiParent.rect, anchor);
        }

        private Rect GetColumnRect(Rect totalRect, int column)
        {
            totalRect.xMin += EditorGUIUtility.labelWidth - 1f;
            Rect rect = totalRect;
            rect.xMin += ((totalRect.width - 4f) * (((float) column) / 3f)) + (column * 2);
            rect.width = (totalRect.width - 4f) / 3f;
            return rect;
        }

        private static Vector3 GetRectReferenceCorner(RectTransform gui, bool worldSpace)
        {
            if (!worldSpace)
            {
                return (((Vector3) gui.rect.min) + gui.transform.localPosition);
            }
            Transform transform = gui.transform;
            gui.GetWorldCorners(s_Corners);
            if (transform.parent != null)
            {
                return transform.parent.InverseTransformPoint(s_Corners[0]);
            }
            return s_Corners[0];
        }

        private void HandleDragChange(string handleName, bool dragging)
        {
            AnimBool changingLeft;
            string key = handleName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map1D == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(9);
                    dictionary.Add("ChangingLeft", 0);
                    dictionary.Add("ChangingRight", 1);
                    dictionary.Add("ChangingPosY", 2);
                    dictionary.Add("ChangingWidth", 3);
                    dictionary.Add("ChangingBottom", 4);
                    dictionary.Add("ChangingTop", 5);
                    dictionary.Add("ChangingPosX", 6);
                    dictionary.Add("ChangingHeight", 7);
                    dictionary.Add("ChangingPivot", 8);
                    <>f__switch$map1D = dictionary;
                }
                if (<>f__switch$map1D.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            changingLeft = this.m_ChangingLeft;
                            goto Label_0140;

                        case 1:
                            changingLeft = this.m_ChangingRight;
                            goto Label_0140;

                        case 2:
                            changingLeft = this.m_ChangingPosY;
                            goto Label_0140;

                        case 3:
                            changingLeft = this.m_ChangingWidth;
                            goto Label_0140;

                        case 4:
                            changingLeft = this.m_ChangingBottom;
                            goto Label_0140;

                        case 5:
                            changingLeft = this.m_ChangingTop;
                            goto Label_0140;

                        case 6:
                            changingLeft = this.m_ChangingPosX;
                            goto Label_0140;

                        case 7:
                            changingLeft = this.m_ChangingHeight;
                            goto Label_0140;

                        case 8:
                            changingLeft = this.m_ChangingPivot;
                            goto Label_0140;
                    }
                }
            }
            changingLeft = null;
        Label_0140:
            if (changingLeft != null)
            {
                changingLeft.target = dragging;
            }
        }

        private void LayoutDropdownButton(bool anyWithoutParent)
        {
            Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 0f);
            position.x += 2f;
            position.y += 17f;
            position.height = kDropdownSize;
            position.width = kDropdownSize;
            EditorGUI.BeginDisabledGroup(anyWithoutParent);
            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.6f) * color;
            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, "box"))
            {
                GUIUtility.keyboardControl = 0;
                this.m_DropdownWindow = new LayoutDropdownWindow(base.serializedObject);
                PopupWindow.Show(position, this.m_DropdownWindow);
            }
            GUI.color = color;
            EditorGUI.EndDisabledGroup();
            if (!anyWithoutParent)
            {
                LayoutDropdownWindow.DrawLayoutMode(new RectOffset(7, 7, 7, 7).Remove(position), this.m_AnchorMin, this.m_AnchorMax, this.m_AnchoredPosition, this.m_SizeDelta);
                LayoutDropdownWindow.DrawLayoutModeHeadersOutsideRect(position, this.m_AnchorMin, this.m_AnchorMax, this.m_AnchoredPosition, this.m_SizeDelta);
            }
        }

        private float LerpUnclamped(float a, float b, float t)
        {
            return ((a * (1f - t)) + (b * t));
        }

        private static Vector2 NormalizedToPointUnclamped(Rect rectangle, Vector2 normalizedRectCoordinates)
        {
            return new Vector2(Mathf.LerpUnclamped(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.LerpUnclamped(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }

        private void OnDisable()
        {
            this.m_ChangingAnchors.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPivot.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingWidth.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingHeight.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPosX.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPosY.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingLeft.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingRight.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingTop.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            this.m_ChangingBottom.valueChanged.RemoveListener(new UnityAction(this.RepaintScene));
            ManipulationToolUtility.handleDragChange = (UnityEditor.ManipulationToolUtility.HandleDragChange) Delegate.Remove(ManipulationToolUtility.handleDragChange, new UnityEditor.ManipulationToolUtility.HandleDragChange(this.HandleDragChange));
            if ((this.m_DropdownWindow != null) && (this.m_DropdownWindow.editorWindow != null))
            {
                this.m_DropdownWindow.editorWindow.Close();
            }
        }

        private void OnEnable()
        {
            this.m_AnchorMin = base.serializedObject.FindProperty("m_AnchorMin");
            this.m_AnchorMax = base.serializedObject.FindProperty("m_AnchorMax");
            this.m_AnchoredPosition = base.serializedObject.FindProperty("m_AnchoredPosition");
            this.m_SizeDelta = base.serializedObject.FindProperty("m_SizeDelta");
            this.m_Pivot = base.serializedObject.FindProperty("m_Pivot");
            this.m_TargetCount = base.targets.Length;
            this.m_LocalPositionZ = base.serializedObject.FindProperty("m_LocalPosition.z");
            this.m_LocalScale = base.serializedObject.FindProperty("m_LocalScale");
            if (this.m_RotationGUI == null)
            {
                this.m_RotationGUI = new TransformRotationGUI();
            }
            this.m_RotationGUI.OnEnable(base.serializedObject.FindProperty("m_LocalRotation"), new GUIContent("Rotation"));
            this.m_ShowLayoutOptions = EditorPrefs.GetBool("RectTransformEditor.showAnchorProperties", false);
            this.m_RawEditMode = EditorPrefs.GetBool("RectTransformEditor.lockRect", false);
            this.m_ChangingAnchors.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPivot.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingWidth.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingHeight.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPosX.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingPosY.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingLeft.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingRight.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingTop.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            this.m_ChangingBottom.valueChanged.AddListener(new UnityAction(this.RepaintScene));
            ManipulationToolUtility.handleDragChange = (UnityEditor.ManipulationToolUtility.HandleDragChange) Delegate.Combine(ManipulationToolUtility.handleDragChange, new UnityEditor.ManipulationToolUtility.HandleDragChange(this.HandleDragChange));
        }

        public override void OnInspectorGUI()
        {
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212f;
            }
            bool flag = false;
            bool anyDrivenX = false;
            bool anyDrivenY = false;
            bool anyWithoutParent = false;
            foreach (RectTransform transform in base.targets)
            {
                if (transform.drivenByObject != null)
                {
                    flag = true;
                    if ((transform.drivenProperties & (DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.AnchoredPositionX)) != DrivenTransformProperties.None)
                    {
                        anyDrivenX = true;
                    }
                    if ((transform.drivenProperties & (DrivenTransformProperties.SizeDeltaY | DrivenTransformProperties.AnchoredPositionY)) != DrivenTransformProperties.None)
                    {
                        anyDrivenY = true;
                    }
                }
                PrefabType prefabType = PrefabUtility.GetPrefabType(transform.gameObject);
                if (((transform.transform.parent == null) || (transform.transform.parent.GetComponent<RectTransform>() == null)) && ((prefabType != PrefabType.Prefab) && (prefabType != PrefabType.ModelPrefab)))
                {
                    anyWithoutParent = true;
                }
            }
            if (flag)
            {
                if (base.targets.Length == 1)
                {
                    EditorGUILayout.HelpBox("Some values driven by " + (this.target as RectTransform).drivenByObject.GetType().Name + ".", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox("Some values in some or all objects are driven.", MessageType.None);
                }
            }
            base.serializedObject.Update();
            this.LayoutDropdownButton(anyWithoutParent);
            this.SmartPositionAndSizeFields(anyWithoutParent, anyDrivenX, anyDrivenY);
            this.SmartAnchorFields();
            this.SmartPivotField();
            EditorGUILayout.Space();
            if (<>f__am$cache34 == null)
            {
                <>f__am$cache34 = x => ((x as RectTransform).drivenProperties & DrivenTransformProperties.Rotation) != DrivenTransformProperties.None;
            }
            this.m_RotationGUI.RotationField(base.targets.Any<Object>(<>f__am$cache34));
            if (<>f__am$cache35 == null)
            {
                <>f__am$cache35 = x => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleX) != DrivenTransformProperties.None;
            }
            s_ScaleDisabledMask[0] = base.targets.Any<Object>(<>f__am$cache35);
            if (<>f__am$cache36 == null)
            {
                <>f__am$cache36 = x => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleY) != DrivenTransformProperties.None;
            }
            s_ScaleDisabledMask[1] = base.targets.Any<Object>(<>f__am$cache36);
            if (<>f__am$cache37 == null)
            {
                <>f__am$cache37 = x => ((x as RectTransform).drivenProperties & DrivenTransformProperties.ScaleZ) != DrivenTransformProperties.None;
            }
            s_ScaleDisabledMask[2] = base.targets.Any<Object>(<>f__am$cache37);
            Vector3FieldWithDisabledMash(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), this.m_LocalScale, styles.transformScaleContent, s_ScaleDisabledMask);
            base.serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            RectTransform target = this.target as RectTransform;
            Rect rect = target.rect;
            Rect rectInUserSpace = rect;
            Rect rect3 = rect;
            Transform transform = target.transform;
            Transform userSpace = transform;
            Transform space = transform;
            RectTransform guiParent = null;
            if (transform.parent != null)
            {
                space = transform.parent;
                rect3.x += transform.localPosition.x;
                rect3.y += transform.localPosition.y;
                guiParent = space.GetComponent<RectTransform>();
            }
            if (Tools.rectBlueprintMode)
            {
                userSpace = space;
                rectInUserSpace = rect3;
            }
            float num = Mathf.Max(this.m_ChangingAnchors.faded, this.m_ChangingPivot.faded);
            if (target.anchorMin != target.anchorMax)
            {
                float[] values = new float[] { num, this.m_ChangingPosX.faded, this.m_ChangingPosY.faded, this.m_ChangingLeft.faded, this.m_ChangingRight.faded, this.m_ChangingTop.faded, this.m_ChangingBottom.faded };
                num = Mathf.Max(values);
            }
            Color kRectInParentSpaceColor = RectTransformEditor.kRectInParentSpaceColor;
            kRectInParentSpaceColor.a *= num;
            Handles.color = kRectInParentSpaceColor;
            this.DrawRect(rect3, space, true);
            if (this.m_TargetCount == 1)
            {
                RectTransformSnapping.OnGUI();
                if (guiParent != null)
                {
                    this.AllAnchorsSceneGUI(target, guiParent, space, transform);
                }
                this.DrawSizes(rectInUserSpace, userSpace, rect3, space, target, guiParent);
                RectTransformSnapping.DrawGuides();
                if (Tools.current == Tool.Rect)
                {
                    this.ParentRectPreviewDragHandles(guiParent, space);
                }
            }
        }

        private void ParentRectPreviewDragHandles(RectTransform gui, Transform space)
        {
            if (gui != null)
            {
                float size = 0.05f * HandleUtility.GetHandleSize(space.position);
                Rect rect = gui.rect;
                for (int i = 0; i <= 2; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        if ((i == 1) != (j == 1))
                        {
                            Vector3 zero = (Vector3) Vector2.zero;
                            for (int k = 0; k < 2; k++)
                            {
                                zero[k] = Mathf.Lerp(rect.min[k], rect.max[k], ((k != 0) ? ((float) j) : ((float) i)) * 0.5f);
                            }
                            zero = space.TransformPoint(zero);
                            int controlID = GUIUtility.GetControlID(s_ParentRectPreviewHandlesHash, FocusType.Native);
                            Vector3 sideVector = (i != 1) ? ((Vector3) (space.up * rect.height)) : ((Vector3) (space.right * rect.width));
                            Vector3 direction = (i != 1) ? space.right : space.up;
                            EditorGUI.BeginChangeCheck();
                            Vector3 position = RectHandles.SideSlider(controlID, zero, sideVector, direction, size, null, 0f, -3f);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Vector2 vector5 = space.InverseTransformPoint(zero);
                                Vector2 vector6 = space.InverseTransformPoint(position);
                                Rect rect2 = rect;
                                Vector2 vector7 = vector6 - vector5;
                                switch (i)
                                {
                                    case 0:
                                        rect2.min = new Vector2(rect2.min.x + vector7.x, rect2.min.y);
                                        break;

                                    case 2:
                                        rect2.max = new Vector2(rect2.max.x + vector7.x, rect2.max.y);
                                        break;
                                }
                                if (j == 0)
                                {
                                    rect2.min = new Vector2(rect2.min.x, rect2.min.y + vector7.y);
                                }
                                if (j == 2)
                                {
                                    rect2.max = new Vector2(rect2.max.x, rect2.max.y + vector7.y);
                                }
                                this.SetTemporaryRect(gui, rect2, controlID);
                            }
                            if (GUIUtility.hotControl == controlID)
                            {
                                Handles.BeginGUI();
                                EditorGUI.DropShadowLabel(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 60f, 16f), "Preview");
                                Handles.EndGUI();
                            }
                        }
                    }
                }
            }
        }

        private void RawButton(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            this.m_RawEditMode = GUI.Toggle(position, this.m_RawEditMode, styles.rawEditContent, "ButtonRight");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("RectTransformEditor.lockRect", this.m_RawEditMode);
            }
        }

        private void RepaintScene()
        {
            SceneView.RepaintAll();
        }

        private static float Round(float value)
        {
            return Mathf.Floor(0.5f + value);
        }

        private static int RoundToInt(float value)
        {
            return Mathf.FloorToInt(0.5f + value);
        }

        public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart)
        {
            SetAnchorSmart(rect, value, axis, isMax, smart, false, false, false);
        }

        public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue)
        {
            SetAnchorSmart(rect, value, axis, isMax, smart, enforceExactValue, false, false);
        }

        public static unsafe void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue, bool enforceMinNoLargerThanMax, bool moveTogether)
        {
            RectTransform component = null;
            if (rect.transform.parent == null)
            {
                smart = false;
            }
            else
            {
                component = rect.transform.parent.GetComponent<RectTransform>();
                if (component == null)
                {
                    smart = false;
                }
            }
            bool flag = !AnchorAllowedOutsideParent(axis, !isMax ? 0 : 1);
            if (flag)
            {
                value = Mathf.Clamp01(value);
            }
            if (enforceMinNoLargerThanMax)
            {
                if (isMax)
                {
                    value = Mathf.Max(value, rect.anchorMin[axis]);
                }
                else
                {
                    value = Mathf.Min(value, rect.anchorMax[axis]);
                }
            }
            float f = 0f;
            float num2 = 0f;
            if (smart)
            {
                float num3 = !isMax ? rect.anchorMin[axis] : rect.anchorMax[axis];
                f = (value - num3) * component.rect.size[axis];
                float num4 = 0f;
                if (ShouldDoIntSnapping(rect))
                {
                    num4 = Mathf.Round(f) - f;
                }
                f += num4;
                if (!enforceExactValue)
                {
                    value += num4 / component.rect.size[axis];
                    if (Mathf.Abs((float) (Round(value * 1000f) - (value * 1000f))) < 0.1f)
                    {
                        value = Round(value * 1000f) * 0.001f;
                    }
                    if (flag)
                    {
                        value = Mathf.Clamp01(value);
                    }
                    if (enforceMinNoLargerThanMax)
                    {
                        if (isMax)
                        {
                            value = Mathf.Max(value, rect.anchorMin[axis]);
                        }
                        else
                        {
                            value = Mathf.Min(value, rect.anchorMax[axis]);
                        }
                    }
                }
                if (moveTogether)
                {
                    num2 = f;
                }
                else
                {
                    num2 = !isMax ? (f * (1f - rect.pivot[axis])) : (f * rect.pivot[axis]);
                }
            }
            if (isMax)
            {
                Vector2 anchorMax = rect.anchorMax;
                anchorMax[axis] = value;
                rect.anchorMax = anchorMax;
                Vector2 anchorMin = rect.anchorMin;
                if (moveTogether)
                {
                    anchorMin[axis] = (s_StartDragAnchorMin[axis] + anchorMax[axis]) - s_StartDragAnchorMax[axis];
                }
                rect.anchorMin = anchorMin;
            }
            else
            {
                Vector2 vector3 = rect.anchorMin;
                vector3[axis] = value;
                rect.anchorMin = vector3;
                Vector2 vector4 = rect.anchorMax;
                if (moveTogether)
                {
                    vector4[axis] = (s_StartDragAnchorMax[axis] + vector3[axis]) - s_StartDragAnchorMin[axis];
                }
                rect.anchorMax = vector4;
            }
            if (smart)
            {
                ref Vector2 vectorRef;
                int num5;
                Vector2 anchoredPosition = rect.anchoredPosition;
                float num6 = vectorRef[num5];
                (vectorRef = (Vector2) &anchoredPosition)[num5 = axis] = num6 - num2;
                rect.anchoredPosition = anchoredPosition;
                if (!moveTogether)
                {
                    ref Vector2 vectorRef2;
                    Vector2 sizeDelta = rect.sizeDelta;
                    num6 = vectorRef2[num5];
                    (vectorRef2 = (Vector2) &sizeDelta)[num5 = axis] = num6 + (f * (!isMax ? ((float) 1) : ((float) (-1))));
                    rect.sizeDelta = sizeDelta;
                }
            }
        }

        private void SetFadingBasedOnControlID(ref AnimBool animBool, int id)
        {
            GUIView view = (EditorWindow.focusedWindow != null) ? EditorWindow.focusedWindow.m_Parent : null;
            if ((GUIUtility.keyboardControl == id) && (GUIView.current == view))
            {
                animBool.value = true;
                this.m_KeyboardControlIDs[id] = animBool;
            }
            else if (((GUIUtility.keyboardControl != id) || (GUIView.current != view)) && this.m_KeyboardControlIDs.ContainsKey(id))
            {
                this.m_KeyboardControlIDs.Remove(id);
                if (!this.m_KeyboardControlIDs.ContainsValue(animBool))
                {
                    animBool.target = false;
                }
            }
        }

        private void SetFadingBasedOnMouseDownUp(ref AnimBool animBool, Event eventBefore)
        {
            if ((eventBefore.type == EventType.MouseDrag) && (Event.current.type != EventType.MouseDrag))
            {
                animBool.value = true;
            }
            else if ((eventBefore.type == EventType.MouseUp) && (Event.current.type != EventType.MouseUp))
            {
                animBool.target = false;
            }
        }

        public static void SetPivotSmart(RectTransform rect, float value, int axis, bool smart, bool parentSpace)
        {
            Vector3 rectReferenceCorner = GetRectReferenceCorner(rect, !parentSpace);
            Vector2 pivot = rect.pivot;
            pivot[axis] = value;
            rect.pivot = pivot;
            if (smart)
            {
                Vector3 vector4 = GetRectReferenceCorner(rect, !parentSpace) - rectReferenceCorner;
                rect.anchoredPosition -= vector4;
                Vector3 position = rect.transform.position;
                position.z -= vector4.z;
                rect.transform.position = position;
            }
        }

        private void SetTemporaryRect(RectTransform gui, Rect rect, int id)
        {
            if (s_ParentDragRectTransform == null)
            {
                s_ParentDragRectTransform = gui;
                s_ParentDragOrigRect = gui.rect;
                s_ParentDragId = id;
            }
            else if (s_ParentDragRectTransform != gui)
            {
                return;
            }
            s_ParentDragPreviewRect = rect;
            s_ParentDragTime = Time.realtimeSinceStartup;
            InternalEditorUtility.SetRectTransformTemporaryRect(gui, rect);
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
        }

        private static bool ShouldDoIntSnapping(RectTransform rect)
        {
            Canvas componentInParent = rect.gameObject.GetComponentInParent<Canvas>();
            return ((componentInParent != null) && (componentInParent.renderMode != RenderMode.WorldSpace));
        }

        private void SmartAnchorFields()
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (!this.m_ShowLayoutOptions ? ((float) 1) : ((float) 3)), new GUILayoutOption[0]);
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            this.m_ShowLayoutOptions = EditorGUI.Foldout(position, this.m_ShowLayoutOptions, styles.anchorsContent);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("RectTransformEditor.showAnchorProperties", this.m_ShowLayoutOptions);
            }
            if (this.m_ShowLayoutOptions)
            {
                EditorGUI.indentLevel++;
                position.y += EditorGUIUtility.singleLineHeight;
                if (<>f__am$cache4E == null)
                {
                    <>f__am$cache4E = rectTransform => rectTransform.anchorMin.x;
                }
                if (<>f__am$cache4F == null)
                {
                    <>f__am$cache4F = rectTransform => rectTransform.anchorMin.y;
                }
                this.Vector2Field(position, <>f__am$cache4E, (rectTransform, val) => SetAnchorSmart(rectTransform, val, 0, false, !this.m_RawEditMode, true), <>f__am$cache4F, (rectTransform, val) => SetAnchorSmart(rectTransform, val, 1, false, !this.m_RawEditMode, true), DrivenTransformProperties.AnchorMinX, DrivenTransformProperties.AnchorMinY, this.m_AnchorMin.FindPropertyRelative("x"), this.m_AnchorMin.FindPropertyRelative("y"), styles.anchorMinContent);
                position.y += EditorGUIUtility.singleLineHeight;
                if (<>f__am$cache50 == null)
                {
                    <>f__am$cache50 = rectTransform => rectTransform.anchorMax.x;
                }
                if (<>f__am$cache51 == null)
                {
                    <>f__am$cache51 = rectTransform => rectTransform.anchorMax.y;
                }
                this.Vector2Field(position, <>f__am$cache50, (rectTransform, val) => SetAnchorSmart(rectTransform, val, 0, true, !this.m_RawEditMode, true), <>f__am$cache51, (rectTransform, val) => SetAnchorSmart(rectTransform, val, 1, true, !this.m_RawEditMode, true), DrivenTransformProperties.AnchorMaxX, DrivenTransformProperties.AnchorMaxY, this.m_AnchorMax.FindPropertyRelative("x"), this.m_AnchorMax.FindPropertyRelative("y"), styles.anchorMaxContent);
                EditorGUI.indentLevel--;
            }
        }

        private void SmartPivotField()
        {
            if (<>f__am$cache52 == null)
            {
                <>f__am$cache52 = rectTransform => rectTransform.pivot.x;
            }
            if (<>f__am$cache53 == null)
            {
                <>f__am$cache53 = rectTransform => rectTransform.pivot.y;
            }
            this.Vector2Field(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), <>f__am$cache52, (rectTransform, val) => SetPivotSmart(rectTransform, val, 0, !this.m_RawEditMode, false), <>f__am$cache53, (rectTransform, val) => SetPivotSmart(rectTransform, val, 1, !this.m_RawEditMode, false), DrivenTransformProperties.PivotX, DrivenTransformProperties.PivotY, this.m_Pivot.FindPropertyRelative("x"), this.m_Pivot.FindPropertyRelative("y"), styles.pivotContent);
        }

        private void SmartPositionAndSizeFields(bool anyWithoutParent, bool anyDrivenX, bool anyDrivenY)
        {
            Rect totalRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 4f, new GUILayoutOption[0]);
            totalRect.height = EditorGUIUtility.singleLineHeight * 2f;
            if (<>f__am$cache38 == null)
            {
                <>f__am$cache38 = x => (x as RectTransform).anchorMin.x != (x as RectTransform).anchorMax.x;
            }
            bool flag = base.targets.Any<Object>(<>f__am$cache38);
            if (<>f__am$cache39 == null)
            {
                <>f__am$cache39 = x => (x as RectTransform).anchorMin.y != (x as RectTransform).anchorMax.y;
            }
            bool flag2 = base.targets.Any<Object>(<>f__am$cache39);
            if (<>f__am$cache3A == null)
            {
                <>f__am$cache3A = x => (x as RectTransform).anchorMin.x == (x as RectTransform).anchorMax.x;
            }
            bool flag3 = base.targets.Any<Object>(<>f__am$cache3A);
            if (<>f__am$cache3B == null)
            {
                <>f__am$cache3B = x => (x as RectTransform).anchorMin.y == (x as RectTransform).anchorMax.y;
            }
            bool flag4 = base.targets.Any<Object>(<>f__am$cache3B);
            Rect columnRect = this.GetColumnRect(totalRect, 0);
            if ((flag3 || anyWithoutParent) || anyDrivenX)
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
                if (<>f__am$cache3C == null)
                {
                    <>f__am$cache3C = rectTransform => rectTransform.anchoredPosition.x;
                }
                if (<>f__am$cache3D == null)
                {
                    <>f__am$cache3D = (FloatSetter) ((rectTransform, val) => (rectTransform.anchoredPosition = new Vector2(val, rectTransform.anchoredPosition.y)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache3C, <>f__am$cache3D, DrivenTransformProperties.AnchoredPositionX, new GUIContent("Pos X"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingPosX, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("x"));
                if (<>f__am$cache3E == null)
                {
                    <>f__am$cache3E = rectTransform => rectTransform.offsetMin.x;
                }
                if (<>f__am$cache3F == null)
                {
                    <>f__am$cache3F = (FloatSetter) ((rectTransform, val) => (rectTransform.offsetMin = new Vector2(val, rectTransform.offsetMin.y)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache3E, <>f__am$cache3F, DrivenTransformProperties.None, new GUIContent("Left"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingLeft, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
                EditorGUI.EndProperty();
            }
            columnRect = this.GetColumnRect(totalRect, 1);
            if ((flag4 || anyWithoutParent) || anyDrivenY)
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
                if (<>f__am$cache40 == null)
                {
                    <>f__am$cache40 = rectTransform => rectTransform.anchoredPosition.y;
                }
                if (<>f__am$cache41 == null)
                {
                    <>f__am$cache41 = (FloatSetter) ((rectTransform, val) => (rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, val)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache40, <>f__am$cache41, DrivenTransformProperties.AnchoredPositionY, new GUIContent("Pos Y"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingPosY, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("y"));
                if (<>f__am$cache42 == null)
                {
                    <>f__am$cache42 = rectTransform => -rectTransform.offsetMax.y;
                }
                if (<>f__am$cache43 == null)
                {
                    <>f__am$cache43 = (FloatSetter) ((rectTransform, val) => (rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -val)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache42, <>f__am$cache43, DrivenTransformProperties.None, new GUIContent("Top"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingTop, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
                EditorGUI.EndProperty();
            }
            columnRect = this.GetColumnRect(totalRect, 2);
            EditorGUI.BeginProperty(columnRect, null, this.m_LocalPositionZ);
            if (<>f__am$cache44 == null)
            {
                <>f__am$cache44 = rectTransform => rectTransform.transform.localPosition.z;
            }
            if (<>f__am$cache45 == null)
            {
                <>f__am$cache45 = (FloatSetter) ((rectTransform, val) => (rectTransform.transform.localPosition = new Vector3(rectTransform.transform.localPosition.x, rectTransform.transform.localPosition.y, val)));
            }
            this.FloatFieldLabelAbove(columnRect, <>f__am$cache44, <>f__am$cache45, DrivenTransformProperties.AnchoredPositionZ, new GUIContent("Pos Z"));
            EditorGUI.EndProperty();
            totalRect.y += EditorGUIUtility.singleLineHeight * 2f;
            columnRect = this.GetColumnRect(totalRect, 0);
            if ((flag3 || anyWithoutParent) || anyDrivenX)
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("x"));
                if (<>f__am$cache46 == null)
                {
                    <>f__am$cache46 = rectTransform => rectTransform.sizeDelta.x;
                }
                if (<>f__am$cache47 == null)
                {
                    <>f__am$cache47 = (FloatSetter) ((rectTransform, val) => (rectTransform.sizeDelta = new Vector2(val, rectTransform.sizeDelta.y)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache46, <>f__am$cache47, DrivenTransformProperties.SizeDeltaX, !flag ? new GUIContent("Width") : new GUIContent("W Delta"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingWidth, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("x"));
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("x"));
                if (<>f__am$cache48 == null)
                {
                    <>f__am$cache48 = rectTransform => -rectTransform.offsetMax.x;
                }
                if (<>f__am$cache49 == null)
                {
                    <>f__am$cache49 = (FloatSetter) ((rectTransform, val) => (rectTransform.offsetMax = new Vector2(-val, rectTransform.offsetMax.y)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache48, <>f__am$cache49, DrivenTransformProperties.None, new GUIContent("Right"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingRight, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
                EditorGUI.EndProperty();
            }
            columnRect = this.GetColumnRect(totalRect, 1);
            if ((flag4 || anyWithoutParent) || anyDrivenY)
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("y"));
                if (<>f__am$cache4A == null)
                {
                    <>f__am$cache4A = rectTransform => rectTransform.sizeDelta.y;
                }
                if (<>f__am$cache4B == null)
                {
                    <>f__am$cache4B = (FloatSetter) ((rectTransform, val) => (rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, val)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache4A, <>f__am$cache4B, DrivenTransformProperties.SizeDeltaY, !flag2 ? new GUIContent("Height") : new GUIContent("H Delta"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingHeight, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(columnRect, null, this.m_AnchoredPosition.FindPropertyRelative("y"));
                EditorGUI.BeginProperty(columnRect, null, this.m_SizeDelta.FindPropertyRelative("y"));
                if (<>f__am$cache4C == null)
                {
                    <>f__am$cache4C = rectTransform => rectTransform.offsetMin.y;
                }
                if (<>f__am$cache4D == null)
                {
                    <>f__am$cache4D = (FloatSetter) ((rectTransform, val) => (rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, val)));
                }
                this.FloatFieldLabelAbove(columnRect, <>f__am$cache4C, <>f__am$cache4D, DrivenTransformProperties.None, new GUIContent("Bottom"));
                this.SetFadingBasedOnControlID(ref this.m_ChangingBottom, EditorGUIUtility.s_LastControlID);
                EditorGUI.EndProperty();
                EditorGUI.EndProperty();
            }
            columnRect = totalRect;
            columnRect.height = EditorGUIUtility.singleLineHeight;
            columnRect.y += EditorGUIUtility.singleLineHeight;
            columnRect.yMin -= 2f;
            columnRect.xMin = columnRect.xMax - 26f;
            columnRect.x -= columnRect.width;
            this.BlueprintButton(columnRect);
            columnRect.x += columnRect.width;
            this.RawButton(columnRect);
        }

        private void UpdateTemporaryRect()
        {
            if (s_ParentDragRectTransform != null)
            {
                if (GUIUtility.hotControl == s_ParentDragId)
                {
                    s_ParentDragTime = Time.realtimeSinceStartup;
                    Canvas.ForceUpdateCanvases();
                    GameView.RepaintAll();
                }
                else
                {
                    float num = Time.realtimeSinceStartup - s_ParentDragTime;
                    float t = Mathf.Clamp01(1f - (num * 8f));
                    if (t > 0f)
                    {
                        Rect rect = new Rect {
                            position = Vector2.Lerp(s_ParentDragOrigRect.position, s_ParentDragPreviewRect.position, t),
                            size = Vector2.Lerp(s_ParentDragOrigRect.size, s_ParentDragPreviewRect.size, t)
                        };
                        InternalEditorUtility.SetRectTransformTemporaryRect(s_ParentDragRectTransform, rect);
                    }
                    else
                    {
                        InternalEditorUtility.SetRectTransformTemporaryRect(s_ParentDragRectTransform, new Rect());
                        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateTemporaryRect));
                        s_ParentDragRectTransform = null;
                    }
                    Canvas.ForceUpdateCanvases();
                    SceneView.RepaintAll();
                    GameView.RepaintAll();
                }
            }
        }

        private void Vector2Field(Rect position, FloatGetter xGetter, FloatSetter xSetter, FloatGetter yGetter, FloatSetter ySetter, DrivenTransformProperties xDriven, DrivenTransformProperties yDriven, SerializedProperty xProperty, SerializedProperty yProperty, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, -1, label);
            float labelWidth = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;
            Rect columnRect = this.GetColumnRect(position, 0);
            Rect rect2 = this.GetColumnRect(position, 1);
            EditorGUIUtility.labelWidth = 13f;
            EditorGUI.indentLevel = 0;
            EditorGUI.BeginProperty(columnRect, s_XYLabels[0], xProperty);
            this.FloatField(columnRect, xGetter, xSetter, xDriven, s_XYLabels[0]);
            EditorGUI.EndProperty();
            EditorGUI.BeginProperty(columnRect, s_XYLabels[1], yProperty);
            this.FloatField(rect2, yGetter, ySetter, yDriven, s_XYLabels[1]);
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;
        }

        private static void Vector3FieldWithDisabledMash(Rect position, SerializedProperty property, GUIContent label, bool[] disabledMask)
        {
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
            position = EditorGUI.MultiFieldPrefixLabel(position, id, label, 3);
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty valuesIterator = property.Copy();
            valuesIterator.NextVisible(true);
            EditorGUI.MultiPropertyField(position, s_XYZLabels, valuesIterator, 13f, disabledMask);
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        [CompilerGenerated]
        private sealed class <FloatField>c__AnonStorey91
        {
            internal DrivenTransformProperties driven;
            internal RectTransformEditor.FloatGetter getter;

            internal bool <>m__1A4(Object x)
            {
                return (((x as RectTransform).drivenProperties & this.driven) != DrivenTransformProperties.None);
            }

            internal float <>m__1A5(Object x)
            {
                return this.getter(x as RectTransform);
            }
        }

        [CompilerGenerated]
        private sealed class <FloatFieldLabelAbove>c__AnonStorey90
        {
            internal DrivenTransformProperties driven;
            internal RectTransformEditor.FloatGetter getter;

            internal bool <>m__1A2(Object x)
            {
                return (((x as RectTransform).drivenProperties & this.driven) != DrivenTransformProperties.None);
            }

            internal float <>m__1A3(Object x)
            {
                return this.getter(x as RectTransform);
            }
        }

        private enum AnchorFusedState
        {
            None,
            All,
            Horizontal,
            Vertical
        }

        private delegate float FloatGetter(RectTransform rect);

        private delegate void FloatSetter(RectTransform rect, float f);

        private class Styles
        {
            public GUIContent anchorMaxContent = new GUIContent("Max", "The normalized position in the parent rectangle that the upper right corner is anchored to.");
            public GUIContent anchorMinContent = new GUIContent("Min", "The normalized position in the parent rectangle that the lower left corner is anchored to.");
            public GUIContent anchorsContent = new GUIContent("Anchors");
            public GUIContent blueprintContent = EditorGUIUtility.IconContent("RectTransformBlueprint", "Blueprint mode. Edit RectTransforms as if they were not rotated and scaled. This enables snapping too.");
            public GUIStyle lockStyle = EditorStyles.miniButton;
            public GUIStyle measuringLabelStyle = new GUIStyle("PreOverlayLabel");
            public GUIContent pivotContent = new GUIContent("Pivot", "The pivot point specified in normalized values between 0 and 1. The pivot point is the origin of this rectangle. Rotation and scaling is around this point.");
            public GUIContent positionContent = new GUIContent("Position", "The local position of the rectangle. The position specifies this rectangle's pivot relative to the anchor reference point.");
            public GUIContent rawEditContent = EditorGUIUtility.IconContent("RectTransformRaw", "Raw edit mode. When enabled, editing pivot and anchor values will not counter-adjust the position and size of the rectangle in order to make it stay in place.");
            public GUIContent sizeContent = new GUIContent("Size", "The size of the rectangle.");
            public GUIContent transformPositionZContent = new GUIContent("Pos Z", "Distance to offset the rectangle along the Z axis of the parent. The effect is visible if the Canvas uses a perspective camera, or if a parent RectTransform is rotated along the X or Y axis.");
            public GUIContent transformScaleContent = new GUIContent("Scale", "The local scaling of this Game Object relative to the parent. This scales everything including image borders and text.");
        }
    }
}

