namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Animations;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(BlendTree))]
    internal class BlendTreeInspector : Editor
    {
        [CompilerGenerated]
        private static Func<Vector2, bool> <>f__am$cache2B;
        [CompilerGenerated]
        private static Func<Vector2, float> <>f__am$cache2C;
        [CompilerGenerated]
        private static Func<float, float> <>f__am$cache2D;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache2E;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache2F;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache30;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache31;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache32;
        [CompilerGenerated]
        private static GetFloatFromMotion <>f__am$cache33;
        internal static Action<BlendTree> blendParameterInputChanged = null;
        internal static Animator currentAnimator = null;
        internal static AnimatorController currentController = null;
        private int kNumCirclePoints = 20;
        private const int kVisResolution = 0x40;
        private readonly int m_BlendAnimationID = "BlendAnimationIDHash".GetHashCode();
        private SerializedProperty m_BlendParameter;
        private SerializedProperty m_BlendParameterY;
        private Rect m_BlendRect;
        private Texture2D m_BlendTex;
        private BlendTree m_BlendTree;
        private SerializedProperty m_BlendType;
        private SerializedProperty m_Childs;
        private readonly int m_ClickDragFloatID = "ClickDragFloatIDHash".GetHashCode();
        private SerializedProperty m_MaxThreshold;
        private SerializedProperty m_MinThreshold;
        private SerializedProperty m_Name;
        private SerializedProperty m_NormalizedBlendValues;
        private PreviewBlendTree m_PreviewBlendTree;
        private ReorderableList m_ReorderableList;
        private int m_SelectedPoint = -1;
        private AnimBool m_ShowAdjust = new AnimBool();
        private AnimBool m_ShowCompute = new AnimBool();
        private AnimBool m_ShowGraph = new AnimBool();
        private bool m_ShowGraphValue;
        private SerializedProperty m_UseAutomaticThresholds;
        private VisualizationBlendTree m_VisBlendTree;
        private GameObject m_VisInstance;
        private string m_WarningMessage;
        private float[] m_Weights;
        private List<Texture2D> m_WeightTexs = new List<Texture2D>();
        internal static BlendTree parentBlendTree = null;
        private static float s_ClickDragFloatDistance;
        private static bool s_ClickDragFloatDragged;
        private bool s_DraggingPoint;
        private static Color s_VisBgColor = (EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.95f, 0.95f, 1f));
        private static Color s_VisPointColor = (EditorGUIUtility.isProSkin ? new Color(0.5f, 0.7f, 1f) : new Color(0.5f, 0.7f, 1f));
        private static Color s_VisPointEmptyColor = (EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.8f, 0.8f, 0.8f));
        private static Color s_VisPointOverlayColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.4f) : new Color(0f, 0f, 0f, 0.2f));
        private static Color s_VisSamplerColor = (EditorGUIUtility.isProSkin ? new Color(1f, 0.4f, 0.4f) : new Color(1f, 0.4f, 0.4f));
        private static Color s_VisWeightColor = (EditorGUIUtility.isProSkin ? new Color(0.65f, 0.75f, 1f, 0.65f) : new Color(0.5f, 0.6f, 0.9f, 0.8f));
        private static Color s_VisWeightLineColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.6f) : new Color(0f, 0f, 0f, 0.3f));
        private static Color s_VisWeightShapeColor = (EditorGUIUtility.isProSkin ? new Color(0.4f, 0.65f, 1f, 0.12f) : new Color(0.4f, 0.65f, 1f, 0.15f));
        private static Styles styles;

        private void AddBlendTreeCallback()
        {
            BlendTree tree = this.m_BlendTree.CreateBlendTreeChild((float) 0f);
            int length = this.m_BlendTree.children.Length;
            if (currentController != null)
            {
                tree.blendParameter = this.m_BlendTree.blendParameter;
                this.m_BlendTree.SetDirectBlendTreeParameter(length - 1, currentController.GetDefaultBlendTreeParameter());
            }
            this.SetNewThresholdAndPosition(length - 1);
            this.m_ReorderableList.index = this.m_Childs.arraySize - 1;
        }

        public void AddButton(Rect rect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Motion Field"), false, new GenericMenu.MenuFunction(this.AddChildAnimation));
            menu.AddItem(EditorGUIUtility.TempContent("New Blend Tree"), false, new GenericMenu.MenuFunction(this.AddBlendTreeCallback));
            menu.Popup(rect, 0);
        }

        private void AddChildAnimation()
        {
            this.m_BlendTree.AddChild(null);
            int length = this.m_BlendTree.children.Length;
            this.m_BlendTree.SetDirectBlendTreeParameter(length - 1, currentController.GetDefaultBlendTreeParameter());
            this.SetNewThresholdAndPosition(length - 1);
            this.m_ReorderableList.index = length - 1;
        }

        private void AddComputeMenuItems(GenericMenu menu, string menuItemPrefix, ChildPropertyToCompute prop)
        {
            menu.AddItem(new GUIContent(menuItemPrefix + "Speed"), false, new GenericMenu.MenuFunction2(this.ComputeFromSpeed), prop);
            menu.AddItem(new GUIContent(menuItemPrefix + "Velocity X"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityX), prop);
            menu.AddItem(new GUIContent(menuItemPrefix + "Velocity Y"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityY), prop);
            menu.AddItem(new GUIContent(menuItemPrefix + "Velocity Z"), false, new GenericMenu.MenuFunction2(this.ComputeFromVelocityZ), prop);
            menu.AddItem(new GUIContent(menuItemPrefix + "Angular Speed (Rad)"), false, new GenericMenu.MenuFunction2(this.ComputeFromAngularSpeedRadians), prop);
            menu.AddItem(new GUIContent(menuItemPrefix + "Angular Speed (Deg)"), false, new GenericMenu.MenuFunction2(this.ComputeFromAngularSpeedDegrees), prop);
        }

        private bool AllMotions()
        {
            bool flag = true;
            for (int i = 0; (i < this.m_Childs.arraySize) && flag; i++)
            {
                flag = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion").objectReferenceValue is AnimationClip;
            }
            return flag;
        }

        private void AutoCompute()
        {
            if (this.m_BlendType.intValue == 0)
            {
                EditorGUILayout.PropertyField(this.m_UseAutomaticThresholds, EditorGUIUtility.TempContent("Automate Thresholds"), new GUILayoutOption[0]);
                this.m_ShowCompute.target = !this.m_UseAutomaticThresholds.boolValue;
            }
            else if (this.m_BlendType.intValue == 4)
            {
                this.m_ShowCompute.target = false;
            }
            else
            {
                this.m_ShowCompute.target = true;
            }
            this.m_ShowAdjust.target = this.AllMotions();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCompute.faded))
            {
                Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
                GUIContent label = (this.ParameterCount != 1) ? EditorGUIUtility.TempContent("Compute Positions") : EditorGUIUtility.TempContent("Compute Thresholds");
                controlRect = EditorGUI.PrefixLabel(controlRect, 0, label);
                if (EditorGUI.ButtonMouseDown(controlRect, EditorGUIUtility.TempContent("Select"), FocusType.Passive, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    if (this.ParameterCount == 1)
                    {
                        this.AddComputeMenuItems(menu, string.Empty, ChildPropertyToCompute.Threshold);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Velocity XZ"), false, new GenericMenu.MenuFunction(this.ComputePositionsFromVelocity));
                        menu.AddItem(new GUIContent("Speed And Angular Speed"), false, new GenericMenu.MenuFunction(this.ComputePositionsFromSpeedAndAngularSpeed));
                        this.AddComputeMenuItems(menu, "X Position From/", ChildPropertyToCompute.PositionX);
                        this.AddComputeMenuItems(menu, "Y Position From/", ChildPropertyToCompute.PositionY);
                    }
                    menu.DropDown(controlRect);
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowAdjust.faded))
            {
                Rect position = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), 0, EditorGUIUtility.TempContent("Adjust Time Scale"));
                if (EditorGUI.ButtonMouseDown(position, EditorGUIUtility.TempContent("Select"), FocusType.Passive, EditorStyles.popup))
                {
                    GenericMenu menu2 = new GenericMenu();
                    menu2.AddItem(new GUIContent("Homogeneous Speed"), false, new GenericMenu.MenuFunction(this.ComputeTimeScaleFromSpeed));
                    menu2.AddItem(new GUIContent("Reset Time Scale"), false, new GenericMenu.MenuFunction(this.ResetTimeScale));
                    menu2.DropDown(position);
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void BlendGraph(Rect area)
        {
            area.xMin++;
            area.xMax--;
            int controlID = GUIUtility.GetControlID(this.m_BlendAnimationID, FocusType.Passive);
            int arraySize = this.m_Childs.arraySize;
            float[] values = new float[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold");
                values[i] = property2.floatValue;
            }
            float a = Mathf.Min(values);
            float b = Mathf.Max(values);
            for (int j = 0; j < values.Length; j++)
            {
                values[j] = area.x + (Mathf.InverseLerp(a, b, values[j]) * area.width);
            }
            string blendParameter = this.m_BlendTree.blendParameter;
            float num7 = area.x + (Mathf.InverseLerp(a, b, this.m_BlendTree.GetInputBlendValue(blendParameter)) * area.width);
            Rect position = new Rect(num7 - 4f, area.y, 9f, 42f);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if (!position.Contains(current.mousePosition))
                    {
                        if (area.Contains(current.mousePosition))
                        {
                            current.Use();
                            GUIUtility.hotControl = controlID;
                            GUIUtility.keyboardControl = controlID;
                            float x = current.mousePosition.x;
                            float positiveInfinity = float.PositiveInfinity;
                            for (int k = 0; k < values.Length; k++)
                            {
                                float num15 = (k != 0) ? values[k - 1] : values[k];
                                float num16 = (k != (values.Length - 1)) ? values[k + 1] : values[k];
                                if (((Mathf.Abs((float) (x - values[k])) < positiveInfinity) && (x < num16)) && (x > num15))
                                {
                                    positiveInfinity = Mathf.Abs((float) (x - values[k]));
                                    this.m_ReorderableList.index = k;
                                }
                            }
                            this.m_UseAutomaticThresholds.boolValue = false;
                        }
                        break;
                    }
                    current.Use();
                    GUIUtility.hotControl = controlID;
                    this.m_ReorderableList.index = -1;
                    this.m_ReorderableList.index = -1;
                    float t = Mathf.InverseLerp(0f, area.width, current.mousePosition.x - 4f);
                    t = Mathf.Lerp(a, b, t);
                    this.m_BlendTree.SetInputBlendValue(blendParameter, t);
                    if (parentBlendTree != null)
                    {
                        parentBlendTree.SetInputBlendValue(blendParameter, t);
                        if (blendParameterInputChanged != null)
                        {
                            blendParameterInputChanged(parentBlendTree);
                        }
                    }
                    if (blendParameterInputChanged != null)
                    {
                        blendParameterInputChanged(this.m_BlendTree);
                    }
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        current.Use();
                        GUIUtility.hotControl = 0;
                        this.m_ReorderableList.index = -1;
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        current.Use();
                        if (this.m_ReorderableList.index == -1)
                        {
                            float num17 = Mathf.InverseLerp(0f, area.width, current.mousePosition.x - 4f);
                            num17 = Mathf.Lerp(a, b, num17);
                            this.m_BlendTree.SetInputBlendValue(blendParameter, num17);
                            if (parentBlendTree != null)
                            {
                                parentBlendTree.SetInputBlendValue(blendParameter, num17);
                                if (blendParameterInputChanged != null)
                                {
                                    blendParameterInputChanged(parentBlendTree);
                                }
                            }
                            if (blendParameterInputChanged != null)
                            {
                                blendParameterInputChanged(this.m_BlendTree);
                            }
                        }
                        else
                        {
                            float num18 = Mathf.InverseLerp(0f, area.width, current.mousePosition.x);
                            num18 = Mathf.Lerp(a, b, num18);
                            SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index);
                            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
                            SerializedProperty property5 = (this.m_ReorderableList.index > 0) ? this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index - 1) : arrayElementAtIndex;
                            SerializedProperty property6 = (this.m_ReorderableList.index != (this.m_Childs.arraySize - 1)) ? this.m_Childs.GetArrayElementAtIndex(this.m_ReorderableList.index + 1) : arrayElementAtIndex;
                            SerializedProperty property7 = property5.FindPropertyRelative("m_Threshold");
                            SerializedProperty property8 = property6.FindPropertyRelative("m_Threshold");
                            float num19 = (b - a) / area.width;
                            float num20 = current.delta.x;
                            property4.floatValue += num20 * num19;
                            if ((property4.floatValue < property7.floatValue) && (this.m_ReorderableList.index != 0))
                            {
                                this.m_Childs.MoveArrayElement(this.m_ReorderableList.index, this.m_ReorderableList.index - 1);
                                this.m_ReorderableList.index--;
                            }
                            if ((property4.floatValue > property8.floatValue) && (this.m_ReorderableList.index < (this.m_Childs.arraySize - 1)))
                            {
                                this.m_Childs.MoveArrayElement(this.m_ReorderableList.index, this.m_ReorderableList.index + 1);
                                this.m_ReorderableList.index++;
                            }
                            float num21 = 3f * ((b - a) / area.width);
                            if ((property4.floatValue - property7.floatValue) <= num21)
                            {
                                property4.floatValue = property7.floatValue;
                            }
                            else if ((property8.floatValue - property4.floatValue) <= num21)
                            {
                                property4.floatValue = property8.floatValue;
                            }
                            this.SetMinMaxThresholds();
                        }
                        break;
                    }
                    break;

                case EventType.Repaint:
                {
                    styles.background.Draw(area, GUIContent.none, false, false, false, false);
                    if (this.m_Childs.arraySize < 2)
                    {
                        GUI.Label(area, EditorGUIUtility.TempContent("Please Add Motion Fields or Blend Trees"), styles.errorStyle);
                        break;
                    }
                    for (int m = 0; m < values.Length; m++)
                    {
                        float min = (m != 0) ? values[m - 1] : values[m];
                        float max = (m != (values.Length - 1)) ? values[m + 1] : values[m];
                        bool selected = this.m_ReorderableList.index == m;
                        this.DrawAnimation(values[m], min, max, selected, area);
                    }
                    Color color = Handles.color;
                    Handles.color = new Color(0f, 0f, 0f, 0.25f);
                    Handles.DrawLine(new Vector3(area.x, area.y + area.height, 0f), new Vector3(area.x + area.width, area.y + area.height, 0f));
                    Handles.color = color;
                    styles.blendPosition.Draw(position, GUIContent.none, false, false, false, false);
                    break;
                }
            }
        }

        private void BlendGraph2D(Rect area)
        {
            if (this.m_VisBlendTree.controllerDirty)
            {
                this.UpdateBlendVisualization();
                this.ValidatePositions();
            }
            Vector2[] motionPositions = this.GetMotionPositions();
            int[] motionToActiveMotionIndices = this.GetMotionToActiveMotionIndices();
            Vector2 vector = new Vector2(this.m_BlendRect.xMin, this.m_BlendRect.yMin);
            Vector2 vector2 = new Vector2(this.m_BlendRect.xMax, this.m_BlendRect.yMax);
            for (int i = 0; i < motionPositions.Length; i++)
            {
                motionPositions[i].x = this.ConvertFloat(motionPositions[i].x, vector.x, vector2.x, area.xMin, area.xMax);
                motionPositions[i].y = this.ConvertFloat(motionPositions[i].y, vector.y, vector2.y, area.yMax, area.yMin);
            }
            string blendParameter = this.m_BlendTree.blendParameter;
            string blendParameterY = this.m_BlendTree.blendParameterY;
            float inputBlendValue = this.m_BlendTree.GetInputBlendValue(blendParameter);
            float blendY = this.m_BlendTree.GetInputBlendValue(blendParameterY);
            int length = this.GetActiveMotionPositions().Length;
            if ((this.m_Weights == null) || (length != this.m_Weights.Length))
            {
                this.m_Weights = new float[length];
            }
            BlendTreePreviewUtility.CalculateRootBlendTreeChildWeights(this.m_VisBlendTree.animator, 0, this.m_VisBlendTree.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_Weights, inputBlendValue, blendY);
            inputBlendValue = area.x + (Mathf.InverseLerp(vector.x, vector2.x, inputBlendValue) * area.width);
            blendY = area.y + ((1f - Mathf.InverseLerp(vector.y, vector2.y, blendY)) * area.height);
            Rect position = new Rect(inputBlendValue - 5f, blendY - 5f, 11f, 11f);
            int controlID = GUIUtility.GetControlID(this.m_BlendAnimationID, FocusType.Native);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (!position.Contains(current.mousePosition))
                    {
                        if (area.Contains(current.mousePosition))
                        {
                            this.m_ReorderableList.index = -1;
                            for (int k = 0; k < motionPositions.Length; k++)
                            {
                                Rect rect3 = new Rect(motionPositions[k].x - 4f, motionPositions[k].y - 4f, 9f, 9f);
                                if (rect3.Contains(current.mousePosition))
                                {
                                    current.Use();
                                    GUIUtility.hotControl = controlID;
                                    this.m_SelectedPoint = k;
                                    this.m_ReorderableList.index = k;
                                }
                            }
                            current.Use();
                        }
                    }
                    else
                    {
                        current.Use();
                        GUIUtility.hotControl = controlID;
                        this.m_SelectedPoint = -1;
                    }
                    goto Label_0807;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        current.Use();
                        GUIUtility.hotControl = 0;
                        this.s_DraggingPoint = false;
                    }
                    goto Label_0807;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        if (this.m_SelectedPoint == -1)
                        {
                            Vector2 vector3;
                            vector3.x = this.ConvertFloat(current.mousePosition.x, area.xMin, area.xMax, vector.x, vector2.x);
                            vector3.y = this.ConvertFloat(current.mousePosition.y, area.yMax, area.yMin, vector.y, vector2.y);
                            this.m_BlendTree.SetInputBlendValue(blendParameter, vector3.x);
                            this.m_BlendTree.SetInputBlendValue(blendParameterY, vector3.y);
                            if (parentBlendTree != null)
                            {
                                parentBlendTree.SetInputBlendValue(blendParameter, vector3.x);
                                parentBlendTree.SetInputBlendValue(blendParameterY, vector3.y);
                                if (blendParameterInputChanged != null)
                                {
                                    blendParameterInputChanged(parentBlendTree);
                                }
                            }
                            if (blendParameterInputChanged != null)
                            {
                                blendParameterInputChanged(this.m_BlendTree);
                            }
                            current.Use();
                        }
                        else
                        {
                            for (int m = 0; m < motionPositions.Length; m++)
                            {
                                if (this.m_SelectedPoint == m)
                                {
                                    Vector2 vector4;
                                    vector4.x = this.ConvertFloat(current.mousePosition.x, area.xMin, area.xMax, vector.x, vector2.x);
                                    vector4.y = this.ConvertFloat(current.mousePosition.y, area.yMax, area.yMin, vector.y, vector2.y);
                                    float minDifference = (vector2.x - vector.x) / area.width;
                                    vector4.x = MathUtils.RoundBasedOnMinimumDifference(vector4.x, minDifference);
                                    vector4.y = MathUtils.RoundBasedOnMinimumDifference(vector4.y, minDifference);
                                    vector4.x = Mathf.Clamp(vector4.x, -10000f, 10000f);
                                    vector4.y = Mathf.Clamp(vector4.y, -10000f, 10000f);
                                    this.m_Childs.GetArrayElementAtIndex(m).FindPropertyRelative("m_Position").vector2Value = vector4;
                                    current.Use();
                                    this.s_DraggingPoint = true;
                                }
                            }
                        }
                    }
                    goto Label_0807;

                case EventType.Repaint:
                {
                    GUI.color = s_VisBgColor;
                    GUI.DrawTexture(area, EditorGUIUtility.whiteTexture);
                    if (this.m_ReorderableList.index >= 0)
                    {
                        if (motionToActiveMotionIndices[this.m_ReorderableList.index] >= 0)
                        {
                            GUI.color = s_VisWeightColor;
                            GUI.DrawTexture(area, this.m_WeightTexs[motionToActiveMotionIndices[this.m_ReorderableList.index]]);
                        }
                        break;
                    }
                    Color color = s_VisWeightColor;
                    color.a *= 0.75f;
                    GUI.color = color;
                    GUI.DrawTexture(area, this.m_BlendTex);
                    break;
                }
                default:
                    goto Label_0807;
            }
            GUI.color = Color.white;
            if (!this.s_DraggingPoint)
            {
                for (int n = 0; n < motionPositions.Length; n++)
                {
                    if (motionToActiveMotionIndices[n] >= 0)
                    {
                        this.DrawWeightShape(motionPositions[n], this.m_Weights[motionToActiveMotionIndices[n]], 0);
                    }
                }
                for (int num7 = 0; num7 < motionPositions.Length; num7++)
                {
                    if (motionToActiveMotionIndices[num7] >= 0)
                    {
                        this.DrawWeightShape(motionPositions[num7], this.m_Weights[motionToActiveMotionIndices[num7]], 1);
                    }
                }
            }
            for (int j = 0; j < motionPositions.Length; j++)
            {
                Rect rect2 = new Rect(motionPositions[j].x - 6f, motionPositions[j].y - 6f, 13f, 13f);
                bool flag = this.m_ReorderableList.index == j;
                if (motionToActiveMotionIndices[j] < 0)
                {
                    GUI.color = s_VisPointEmptyColor;
                }
                else
                {
                    GUI.color = s_VisPointColor;
                }
                GUI.DrawTexture(rect2, !flag ? styles.pointIcon : styles.pointIconSelected);
                if (flag)
                {
                    GUI.color = s_VisPointOverlayColor;
                    GUI.DrawTexture(rect2, styles.pointIconOverlay);
                }
            }
            if (!this.s_DraggingPoint)
            {
                GUI.color = s_VisSamplerColor;
                GUI.DrawTexture(position, styles.samplerIcon);
            }
            GUI.color = Color.white;
        Label_0807:
            if ((this.m_ReorderableList.index >= 0) && (motionToActiveMotionIndices[this.m_ReorderableList.index] < 0))
            {
                this.ShowHelp(area, EditorGUIUtility.TempContent("The selected child has no Motion assigned."));
            }
            else if (this.m_WarningMessage != null)
            {
                this.ShowHelp(area, EditorGUIUtility.TempContent(this.m_WarningMessage));
            }
        }

        public float ClickDragFloat(Rect position, float value)
        {
            return this.ClickDragFloat(position, value, false);
        }

        public float ClickDragFloat(Rect position, float value, bool alignRight)
        {
            bool flag;
            string str2;
            string allowedletters = "inftynaeINFTYNAE0123456789.,-";
            int id = GUIUtility.GetControlID(this.m_ClickDragFloatID, FocusType.Keyboard, position);
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if ((GUIUtility.keyboardControl != id) || !EditorGUIUtility.editingTextField)
                    {
                        if (position.Contains(current.mousePosition))
                        {
                            current.Use();
                            s_ClickDragFloatDragged = false;
                            s_ClickDragFloatDistance = 0f;
                            GUIUtility.hotControl = id;
                            GUIUtility.keyboardControl = id;
                            EditorGUIUtility.editingTextField = false;
                        }
                        break;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        current.Use();
                        if (position.Contains(current.mousePosition) && !s_ClickDragFloatDragged)
                        {
                            EditorGUIUtility.editingTextField = true;
                        }
                        else
                        {
                            GUIUtility.keyboardControl = 0;
                            GUIUtility.hotControl = 0;
                            s_ClickDragFloatDragged = false;
                        }
                        break;
                    }
                    break;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == id) && !EditorGUIUtility.editingTextField)
                    {
                        s_ClickDragFloatDistance += Mathf.Abs(HandleUtility.niceMouseDelta);
                        if (s_ClickDragFloatDistance >= 5f)
                        {
                            s_ClickDragFloatDragged = true;
                            value += HandleUtility.niceMouseDelta * 0.03f;
                            value = MathUtils.RoundBasedOnMinimumDifference(value, 0.03f);
                            GUI.changed = true;
                        }
                        current.Use();
                        break;
                    }
                    break;
            }
            GUIStyle style = ((GUIUtility.keyboardControl != id) || !EditorGUIUtility.editingTextField) ? (!alignRight ? styles.clickDragFloatLabelLeft : styles.clickDragFloatLabelRight) : (!alignRight ? styles.clickDragFloatFieldLeft : styles.clickDragFloatFieldRight);
            if (GUIUtility.keyboardControl == id)
            {
                if (!EditorGUI.s_RecycledEditor.IsEditingControl(id))
                {
                    str2 = EditorGUI.s_RecycledCurrentEditingString = value.ToString("g7");
                }
                else
                {
                    str2 = EditorGUI.s_RecycledCurrentEditingString;
                    if ((current.type == EventType.ValidateCommand) && (current.commandName == "UndoRedoPerformed"))
                    {
                        str2 = value.ToString("g7");
                    }
                }
                str2 = EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, id, position, str2, style, allowedletters, out flag, false, false, false);
                if (flag)
                {
                    GUI.changed = true;
                    EditorGUI.s_RecycledCurrentEditingString = str2;
                    switch (str2.ToLower())
                    {
                        case "inf":
                        case "infinity":
                            value = float.PositiveInfinity;
                            return value;

                        case "-inf":
                        case "-infinity":
                            value = float.NegativeInfinity;
                            return value;
                    }
                    if (!float.TryParse(str2.Replace(',', '.'), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out value))
                    {
                        EditorGUI.s_RecycledCurrentEditingFloat = 0.0;
                        value = 0f;
                        return value;
                    }
                    if (float.IsNaN(value))
                    {
                        value = 0f;
                    }
                    EditorGUI.s_RecycledCurrentEditingFloat = value;
                }
                return value;
            }
            str2 = value.ToString("g7");
            str2 = EditorGUI.DoTextField(EditorGUI.s_RecycledEditor, id, position, str2, style, allowedletters, out flag, false, false, false);
            return value;
        }

        private List<string> CollectParameters(AnimatorController controller)
        {
            List<string> list = new List<string>();
            if (controller != null)
            {
                foreach (AnimatorControllerParameter parameter in controller.parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Float)
                    {
                        list.Add(parameter.name);
                    }
                }
            }
            return list;
        }

        private void ComputeFromAngularSpeedDegrees(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache32 == null)
            {
                <>f__am$cache32 = (m, mirrorMultiplier) => ((m.averageAngularSpeed * 180f) / 3.141593f) * mirrorMultiplier;
            }
            this.ComputeProperty(<>f__am$cache32, prop);
        }

        private void ComputeFromAngularSpeedRadians(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache33 == null)
            {
                <>f__am$cache33 = (m, mirrorMultiplier) => m.averageAngularSpeed * mirrorMultiplier;
            }
            this.ComputeProperty(<>f__am$cache33, prop);
        }

        private void ComputeFromSpeed(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache2E == null)
            {
                <>f__am$cache2E = (m, mirrorMultiplier) => m.apparentSpeed;
            }
            this.ComputeProperty(<>f__am$cache2E, prop);
        }

        private void ComputeFromVelocityX(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache2F == null)
            {
                <>f__am$cache2F = (m, mirrorMultiplier) => m.averageSpeed.x * mirrorMultiplier;
            }
            this.ComputeProperty(<>f__am$cache2F, prop);
        }

        private void ComputeFromVelocityY(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache30 == null)
            {
                <>f__am$cache30 = (m, mirrorMultiplier) => m.averageSpeed.y;
            }
            this.ComputeProperty(<>f__am$cache30, prop);
        }

        private void ComputeFromVelocityZ(object obj)
        {
            ChildPropertyToCompute prop = (ChildPropertyToCompute) ((int) obj);
            if (<>f__am$cache31 == null)
            {
                <>f__am$cache31 = (m, mirrorMultiplier) => m.averageSpeed.z;
            }
            this.ComputeProperty(<>f__am$cache31, prop);
        }

        private void ComputePositionsFromSpeedAndAngularSpeed()
        {
            this.ComputeFromAngularSpeedRadians(ChildPropertyToCompute.PositionX);
            this.ComputeFromSpeed(ChildPropertyToCompute.PositionY);
        }

        private void ComputePositionsFromVelocity()
        {
            this.ComputeFromVelocityX(ChildPropertyToCompute.PositionX);
            this.ComputeFromVelocityZ(ChildPropertyToCompute.PositionY);
        }

        private void ComputeProperty(GetFloatFromMotion func, ChildPropertyToCompute prop)
        {
            float num = 0f;
            float[] numArray = new float[this.m_Childs.arraySize];
            this.m_UseAutomaticThresholds.boolValue = false;
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                SerializedProperty property = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion");
                SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Mirror");
                Motion objectReferenceValue = property.objectReferenceValue as Motion;
                if (objectReferenceValue != null)
                {
                    float num3 = func(objectReferenceValue, !property2.boolValue ? ((float) 1) : ((float) (-1)));
                    numArray[i] = num3;
                    num += num3;
                    if (prop == ChildPropertyToCompute.Threshold)
                    {
                        this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold").floatValue = num3;
                    }
                    else
                    {
                        SerializedProperty property4 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Position");
                        Vector2 vector = property4.vector2Value;
                        if (prop == ChildPropertyToCompute.PositionX)
                        {
                            vector.x = num3;
                        }
                        else
                        {
                            vector.y = num3;
                        }
                        property4.vector2Value = vector;
                    }
                }
            }
            num /= (float) this.m_Childs.arraySize;
            float num4 = 0f;
            for (int j = 0; j < numArray.Length; j++)
            {
                num4 += Mathf.Pow(numArray[j] - num, 2f);
            }
            num4 /= (float) numArray.Length;
            if (num4 < Mathf.Epsilon)
            {
                Debug.LogWarning("Could not compute threshold for '" + this.m_BlendTree.name + "' there is not enough data");
                base.m_SerializedObject.Update();
            }
            else
            {
                base.m_SerializedObject.ApplyModifiedProperties();
                if (prop == ChildPropertyToCompute.Threshold)
                {
                    this.SortByThreshold();
                    this.SetMinMaxThreshold();
                }
            }
        }

        private void ComputeTimeScaleFromSpeed()
        {
            float apparentSpeed = this.m_BlendTree.apparentSpeed;
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                AnimationClip objectReferenceValue = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion").objectReferenceValue as AnimationClip;
                if (objectReferenceValue != null)
                {
                    if (!objectReferenceValue.legacy)
                    {
                        if (objectReferenceValue.apparentSpeed < Mathf.Epsilon)
                        {
                            Debug.LogWarning("Could not adjust time scale for " + objectReferenceValue.name + " because it has no speed");
                        }
                        else
                        {
                            this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_TimeScale").floatValue = apparentSpeed / objectReferenceValue.apparentSpeed;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Could not adjust time scale for " + objectReferenceValue.name + " because it is not a muscle clip");
                    }
                }
            }
            base.m_SerializedObject.ApplyModifiedProperties();
        }

        private float ConvertFloat(float input, float fromMin, float fromMax, float toMin, float toMax)
        {
            float num = (input - fromMin) / (fromMax - fromMin);
            return ((toMin * (1f - num)) + (toMax * num));
        }

        public static bool DeleteBlendTreeDialog(string toDelete)
        {
            string title = "Delete selected Blend Tree asset?";
            string message = toDelete;
            return EditorUtility.DisplayDialog(title, message, "Delete", "Cancel");
        }

        private void DrawAnimation(float val, float min, float max, bool selected, Rect area)
        {
            float y = area.y;
            Rect position = new Rect(min, y, val - min, area.height);
            Rect rect2 = new Rect(val, y, max - val, area.height);
            styles.triangleLeft.Draw(position, selected, selected, false, false);
            styles.triangleRight.Draw(rect2, selected, selected, false, false);
            area.height--;
            Color color = Handles.color;
            Color color2 = !selected ? new Color(1f, 1f, 1f, 0.4f) : new Color(1f, 1f, 1f, 0.6f);
            Handles.color = color2;
            if (selected)
            {
                Handles.DrawLine(new Vector3(val, y, 0f), new Vector3(val, y + area.height, 0f));
            }
            Vector3[] points = new Vector3[] { new Vector3(min, y + area.height, 0f), new Vector3(val, y, 0f) };
            Handles.DrawAAPolyLine(points);
            points = new Vector3[] { new Vector3(val, y, 0f), new Vector3(max, y + area.height, 0f) };
            Handles.DrawAAPolyLine(points);
            Handles.color = color;
        }

        public void DrawChild(Rect r, int index, bool isActive, bool isFocused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(index);
            SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_Motion");
            r.y++;
            r.height = 16f;
            Rect[] rowRects = this.GetRowRects(r, this.m_BlendType.intValue);
            int num = 0;
            EditorGUI.BeginChangeCheck();
            Motion motion = this.m_BlendTree.children[index].motion;
            EditorGUI.PropertyField(rowRects[num], property, GUIContent.none);
            num++;
            if ((EditorGUI.EndChangeCheck() && (motion is BlendTree)) && (motion != (property.objectReferenceValue as Motion)))
            {
                if (EditorUtility.DisplayDialog("Changing BlendTree will delete previous BlendTree", "You cannot undo this action.", "Delete", "Cancel"))
                {
                    MecanimUtilities.DestroyBlendTreeRecursive(motion as BlendTree);
                }
                else
                {
                    property.objectReferenceValue = motion;
                }
            }
            if (this.m_BlendType.intValue == 0)
            {
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
                EditorGUI.BeginDisabledGroup(this.m_UseAutomaticThresholds.boolValue);
                float floatValue = property3.floatValue;
                EditorGUI.BeginChangeCheck();
                string s = EditorGUI.DelayedTextFieldInternal(rowRects[num], floatValue.ToString(), "inftynaeINFTYNAE0123456789.,-", EditorStyles.textField);
                num++;
                if (EditorGUI.EndChangeCheck() && float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out floatValue))
                {
                    property3.floatValue = floatValue;
                    base.serializedObject.ApplyModifiedProperties();
                    this.m_BlendTree.SortChildren();
                    this.SetMinMaxThresholds();
                    GUI.changed = true;
                }
                EditorGUI.EndDisabledGroup();
            }
            else if (this.m_BlendType.intValue == 4)
            {
                List<string> list = this.CollectParameters(currentController);
                ChildMotion[] children = this.m_BlendTree.children;
                string directBlendParameter = children[index].directBlendParameter;
                EditorGUI.BeginChangeCheck();
                directBlendParameter = EditorGUI.TextFieldDropDown(rowRects[num], directBlendParameter, list.ToArray());
                num++;
                if (EditorGUI.EndChangeCheck())
                {
                    children[index].directBlendParameter = directBlendParameter;
                    this.m_BlendTree.children = children;
                }
            }
            else
            {
                SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_Position");
                Vector2 vector = property4.vector2Value;
                for (int i = 0; i < 2; i++)
                {
                    float num4;
                    EditorGUI.BeginChangeCheck();
                    string str3 = EditorGUI.DelayedTextFieldInternal(rowRects[num], vector[i].ToString(), "inftynaeINFTYNAE0123456789.,-", EditorStyles.textField);
                    num++;
                    if (EditorGUI.EndChangeCheck() && float.TryParse(str3, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4))
                    {
                        vector[i] = Mathf.Clamp(num4, -10000f, 10000f);
                        property4.vector2Value = vector;
                        base.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                }
            }
            if (property.objectReferenceValue is AnimationClip)
            {
                SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("m_TimeScale");
                EditorGUI.PropertyField(rowRects[num], property5, GUIContent.none);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.IntField(rowRects[num], 1);
                EditorGUI.EndDisabledGroup();
            }
            num++;
            if ((property.objectReferenceValue is AnimationClip) && (property.objectReferenceValue as AnimationClip).isHumanMotion)
            {
                SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("m_Mirror");
                EditorGUI.PropertyField(rowRects[num], property6, GUIContent.none);
                arrayElementAtIndex.FindPropertyRelative("m_CycleOffset").floatValue = !property6.boolValue ? 0f : 0.5f;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Toggle(rowRects[num], false);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void DrawHeader(Rect headerRect)
        {
            headerRect.xMin += 14f;
            headerRect.y++;
            headerRect.height = 16f;
            Rect[] rowRects = this.GetRowRects(headerRect, this.m_BlendType.intValue);
            int index = 0;
            rowRects[index].xMin -= 14f;
            GUI.Label(rowRects[index], EditorGUIUtility.TempContent("Motion"), EditorStyles.label);
            index++;
            if (this.m_Childs.arraySize >= 1)
            {
                if (this.m_BlendType.intValue == 0)
                {
                    GUI.Label(rowRects[index], EditorGUIUtility.TempContent("Threshold"), EditorStyles.label);
                    index++;
                }
                else if (this.m_BlendType.intValue == 4)
                {
                    GUI.Label(rowRects[index], EditorGUIUtility.TempContent("Parameter"), EditorStyles.label);
                    index++;
                }
                else
                {
                    GUI.Label(rowRects[index], EditorGUIUtility.TempContent("Pos X"), EditorStyles.label);
                    index++;
                    GUI.Label(rowRects[index], EditorGUIUtility.TempContent("Pos Y"), EditorStyles.label);
                    index++;
                }
                GUI.Label(rowRects[index], styles.speedIcon, styles.headerIcon);
                index++;
                GUI.Label(rowRects[index], styles.mirrorIcon, styles.headerIcon);
            }
        }

        private void DrawWeightShape(Vector2 point, float weight, int pass)
        {
            if (weight > 0f)
            {
                point.x = Mathf.Round(point.x);
                point.y = Mathf.Round(point.y);
                float radius = 20f * Mathf.Sqrt(weight);
                Vector3[] points = new Vector3[this.kNumCirclePoints + 2];
                for (int i = 0; i < this.kNumCirclePoints; i++)
                {
                    float num3 = ((float) i) / ((float) this.kNumCirclePoints);
                    points[i + 1] = new Vector3(point.x + 0.5f, point.y + 0.5f, 0f) + ((Vector3) (new Vector3(Mathf.Sin((num3 * 2f) * 3.141593f), Mathf.Cos((num3 * 2f) * 3.141593f), 0f) * radius));
                }
                points[0] = points[this.kNumCirclePoints + 1] = (Vector3) ((points[1] + points[this.kNumCirclePoints]) * 0.5f);
                if (pass == 0)
                {
                    Handles.color = s_VisWeightShapeColor;
                    Handles.DrawSolidDisc((Vector3) (point + new Vector2(0.5f, 0.5f)), -Vector3.forward, radius);
                }
                else
                {
                    Handles.color = s_VisWeightLineColor;
                    Handles.DrawAAPolyLine(points);
                }
            }
        }

        public void EndDragChild(ReorderableList list)
        {
            List<float> list2 = new List<float>();
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold");
                list2.Add(property2.floatValue);
            }
            list2.Sort();
            for (int j = 0; j < this.m_Childs.arraySize; j++)
            {
                this.m_Childs.GetArrayElementAtIndex(j).FindPropertyRelative("m_Threshold").floatValue = list2[j];
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private Rect Get2DBlendRect(Vector2[] points)
        {
            Vector2 zero = Vector2.zero;
            float a = 0f;
            if (points.Length == 0)
            {
                return new Rect();
            }
            if (this.m_BlendType.intValue == 3)
            {
                Vector2 vector2 = points[0];
                Vector2 vector3 = points[0];
                for (int i = 1; i < points.Length; i++)
                {
                    vector3.x = Mathf.Max(vector3.x, points[i].x);
                    vector3.y = Mathf.Max(vector3.y, points[i].y);
                    vector2.x = Mathf.Min(vector2.x, points[i].x);
                    vector2.y = Mathf.Min(vector2.y, points[i].y);
                }
                zero = (Vector2) ((vector2 + vector3) * 0.5f);
                a = Mathf.Max((float) (vector3.x - vector2.x), (float) (vector3.y - vector2.y)) * 0.5f;
            }
            else
            {
                for (int j = 0; j < points.Length; j++)
                {
                    a = Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(a, points[j].x), -points[j].x), points[j].y), -points[j].y);
                }
            }
            if (a == 0f)
            {
                a = 1f;
            }
            a *= 1.35f;
            return new Rect(zero.x - a, zero.y - a, a * 2f, a * 2f);
        }

        private Vector2[] GetActiveMotionPositions()
        {
            List<Vector2> list = new List<Vector2>();
            int arraySize = this.m_Childs.arraySize;
            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(i);
                if (arrayElementAtIndex.FindPropertyRelative("m_Motion").objectReferenceValue != null)
                {
                    SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_Position");
                    list.Add(property3.vector2Value);
                }
            }
            return list.ToArray();
        }

        private Vector2[] GetMotionPositions()
        {
            int arraySize = this.m_Childs.arraySize;
            Vector2[] vectorArray = new Vector2[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Position");
                vectorArray[i] = property2.vector2Value;
            }
            return vectorArray;
        }

        private int[] GetMotionToActiveMotionIndices()
        {
            int arraySize = this.m_Childs.arraySize;
            int[] numArray = new int[arraySize];
            int num2 = 0;
            for (int i = 0; i < arraySize; i++)
            {
                if (this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion").objectReferenceValue == null)
                {
                    numArray[i] = -1;
                }
                else
                {
                    numArray[i] = num2;
                    num2++;
                }
            }
            return numArray;
        }

        private Rect[] GetRowRects(Rect r, int blendType)
        {
            int num = ((blendType <= 0) || (blendType >= 4)) ? 1 : 2;
            Rect[] rectArray = new Rect[3 + num];
            float width = r.width;
            float num3 = 16f;
            width -= num3;
            width -= 0x18 + (4 * (num - 1));
            float num4 = Mathf.FloorToInt(width * 0.2f);
            float num5 = width - (num4 * (num + 1));
            float x = r.x;
            int index = 0;
            rectArray[index] = new Rect(x, r.y, num5, r.height);
            x += num5 + 8f;
            index++;
            for (int i = 0; i < num; i++)
            {
                rectArray[index] = new Rect(x, r.y, num4, r.height);
                x += num4 + 4f;
                index++;
            }
            x += 4f;
            rectArray[index] = new Rect(x, r.y, num4, r.height);
            x += num4 + 8f;
            index++;
            rectArray[index] = new Rect(x, r.y, num3, r.height);
            return rectArray;
        }

        public override bool HasPreviewGUI()
        {
            return ((this.m_PreviewBlendTree != null) && this.m_PreviewBlendTree.HasPreviewGUI());
        }

        private void Init()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
            if (this.m_BlendTree == null)
            {
                this.m_BlendTree = this.target as BlendTree;
            }
            if (styles == null)
            {
                styles = new Styles();
            }
            if (this.m_PreviewBlendTree == null)
            {
                this.m_PreviewBlendTree = new PreviewBlendTree();
            }
            if (this.m_VisBlendTree == null)
            {
                this.m_VisBlendTree = new VisualizationBlendTree();
            }
            if (this.m_Childs == null)
            {
                this.m_Childs = base.serializedObject.FindProperty("m_Childs");
                this.m_ReorderableList = new ReorderableList(base.serializedObject, this.m_Childs);
                this.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawHeader);
                this.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawChild);
                this.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
                this.m_ReorderableList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.AddButton);
                this.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
                if (this.m_BlendType.intValue == 0)
                {
                    this.SortByThreshold();
                }
                this.m_ShowGraphValue = (this.m_BlendType.intValue != 4) ? (this.m_Childs.arraySize >= 2) : (this.m_Childs.arraySize >= 1);
                this.m_ShowGraph.value = this.m_ShowGraphValue;
                this.m_ShowAdjust.value = this.AllMotions();
                this.m_ShowCompute.value = !this.m_UseAutomaticThresholds.boolValue;
                this.m_ShowGraph.valueChanged.AddListener(new UnityAction(this.Repaint));
                this.m_ShowAdjust.valueChanged.AddListener(new UnityAction(this.Repaint));
                this.m_ShowCompute.valueChanged.AddListener(new UnityAction(this.Repaint));
            }
            this.m_PreviewBlendTree.Init(this.m_BlendTree, currentAnimator);
            bool flag = false;
            if (this.m_VisInstance == null)
            {
                GameObject original = (GameObject) EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx");
                this.m_VisInstance = EditorUtility.InstantiateForAnimatorPreview(original);
                foreach (Renderer renderer in this.m_VisInstance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }
                flag = true;
            }
            this.m_VisBlendTree.Init(this.m_BlendTree, this.m_VisInstance.GetComponent<Animator>());
            if (flag && (((this.m_BlendType.intValue == 1) || (this.m_BlendType.intValue == 2)) || (this.m_BlendType.intValue == 3)))
            {
                this.UpdateBlendVisualization();
                this.ValidatePositions();
            }
        }

        public void OnDestroy()
        {
            if (this.m_PreviewBlendTree != null)
            {
                this.m_PreviewBlendTree.OnDestroy();
            }
            if (this.m_VisBlendTree != null)
            {
                this.m_VisBlendTree.Destroy();
            }
            if (this.m_VisInstance != null)
            {
                Object.DestroyImmediate(this.m_VisInstance);
            }
            for (int i = 0; i < this.m_WeightTexs.Count; i++)
            {
                Object.DestroyImmediate(this.m_WeightTexs[i]);
            }
            if (this.m_BlendTex != null)
            {
                Object.DestroyImmediate(this.m_BlendTex);
            }
        }

        public void OnDisable()
        {
            if (this.m_PreviewBlendTree != null)
            {
                this.m_PreviewBlendTree.OnDisable();
            }
        }

        public void OnEnable()
        {
            this.m_Name = base.serializedObject.FindProperty("m_Name");
            this.m_BlendParameter = base.serializedObject.FindProperty("m_BlendParameter");
            this.m_BlendParameterY = base.serializedObject.FindProperty("m_BlendParameterY");
            this.m_UseAutomaticThresholds = base.serializedObject.FindProperty("m_UseAutomaticThresholds");
            this.m_NormalizedBlendValues = base.serializedObject.FindProperty("m_NormalizedBlendValues");
            this.m_MinThreshold = base.serializedObject.FindProperty("m_MinThreshold");
            this.m_MaxThreshold = base.serializedObject.FindProperty("m_MaxThreshold");
            this.m_BlendType = base.serializedObject.FindProperty("m_BlendType");
        }

        internal override void OnHeaderControlsGUI()
        {
            EditorGUIUtility.labelWidth = 80f;
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_BlendType, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        internal override void OnHeaderIconGUI(Rect iconRect)
        {
            Texture2D miniThumbnail = AssetPreview.GetMiniThumbnail(this.target);
            GUI.Label(iconRect, miniThumbnail);
        }

        internal override void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            base.serializedObject.Update();
            Rect position = titleRect;
            position.height = 16f;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
            string str = EditorGUI.DelayedTextField(position, this.m_Name.stringValue, EditorStyles.textField);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(str))
            {
                foreach (Object obj2 in base.targets)
                {
                    ObjectNames.SetNameSmart(obj2, str);
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            this.Init();
            base.serializedObject.Update();
            if (this.m_BlendType.intValue != 4)
            {
                this.ParameterGUI();
            }
            this.m_ShowGraphValue = (this.m_BlendType.intValue != 4) ? (this.m_Childs.arraySize >= 2) : (this.m_Childs.arraySize >= 1);
            this.m_ShowGraph.target = this.m_ShowGraphValue;
            this.m_UseAutomaticThresholds = base.serializedObject.FindProperty("m_UseAutomaticThresholds");
            GUI.enabled = true;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowGraph.faded))
            {
                if (this.m_BlendType.intValue == 0)
                {
                    this.BlendGraph(EditorGUILayout.GetControlRect(false, 40f, styles.background, new GUILayoutOption[0]));
                    this.ThresholdValues();
                }
                else if (this.m_BlendType.intValue == 4)
                {
                    for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
                    {
                        string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
                        float recursiveBlendParameterMin = this.m_BlendTree.GetRecursiveBlendParameterMin(i);
                        float recursiveBlendParameterMax = this.m_BlendTree.GetRecursiveBlendParameterMax(i);
                        EditorGUI.BeginChangeCheck();
                        float num4 = EditorGUILayout.Slider(recursiveBlendParameter, this.m_BlendTree.GetInputBlendValue(recursiveBlendParameter), recursiveBlendParameterMin, recursiveBlendParameterMax, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (parentBlendTree != null)
                            {
                                parentBlendTree.SetInputBlendValue(recursiveBlendParameter, num4);
                                if (blendParameterInputChanged != null)
                                {
                                    blendParameterInputChanged(parentBlendTree);
                                }
                            }
                            this.m_BlendTree.SetInputBlendValue(recursiveBlendParameter, num4);
                            if (blendParameterInputChanged != null)
                            {
                                blendParameterInputChanged(this.m_BlendTree);
                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Space(1f);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(235f) };
                    Rect aspectRect = GUILayoutUtility.GetAspectRect(1f, options);
                    GUI.Label(new Rect(aspectRect.x - 1f, aspectRect.y - 1f, aspectRect.width + 2f, aspectRect.height + 2f), GUIContent.none, EditorStyles.textField);
                    GUI.BeginGroup(aspectRect);
                    aspectRect.x = 0f;
                    aspectRect.y = 0f;
                    this.BlendGraph2D(aspectRect);
                    GUI.EndGroup();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5f);
            }
            EditorGUILayout.EndFadeGroup();
            if (this.m_ReorderableList != null)
            {
                this.m_ReorderableList.DoLayoutList();
            }
            if (this.m_BlendType.intValue == 4)
            {
                EditorGUILayout.PropertyField(this.m_NormalizedBlendValues, EditorGUIUtility.TempContent("Normalized Blend Values"), new GUILayoutOption[0]);
            }
            if (this.m_ShowGraphValue)
            {
                GUILayout.Space(10f);
                this.AutoCompute();
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_PreviewBlendTree != null)
            {
                this.m_PreviewBlendTree.OnInteractivePreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.m_PreviewBlendTree != null)
            {
                this.m_PreviewBlendTree.OnPreviewSettings();
            }
        }

        private void ParameterGUI()
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (this.ParameterCount > 1)
            {
                EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent("Parameters"));
            }
            else
            {
                EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent("Parameter"));
            }
            base.serializedObject.Update();
            string blendParameter = this.m_BlendTree.blendParameter;
            string blendParameterY = this.m_BlendTree.blendParameterY;
            List<string> list = this.CollectParameters(currentController);
            EditorGUI.BeginChangeCheck();
            blendParameter = EditorGUILayout.DelayedTextFieldDropDown(blendParameter, list.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                this.m_BlendParameter.stringValue = blendParameter;
            }
            if (this.ParameterCount > 1)
            {
                EditorGUI.BeginChangeCheck();
                blendParameterY = EditorGUILayout.TextFieldDropDown(blendParameterY, list.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_BlendParameterY.stringValue = blendParameterY;
                }
            }
            base.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        public void RemoveButton(ReorderableList list)
        {
            Motion objectReferenceValue = this.m_Childs.GetArrayElementAtIndex(list.index).FindPropertyRelative("m_Motion").objectReferenceValue as Motion;
            if ((objectReferenceValue == null) || DeleteBlendTreeDialog(objectReferenceValue.name))
            {
                this.m_Childs.DeleteArrayElementAtIndex(list.index);
                if (list.index >= this.m_Childs.arraySize)
                {
                    list.index = this.m_Childs.arraySize - 1;
                }
                this.SetMinMaxThresholds();
            }
        }

        private void ResetTimeScale()
        {
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                AnimationClip objectReferenceValue = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Motion").objectReferenceValue as AnimationClip;
                if ((objectReferenceValue != null) && !objectReferenceValue.legacy)
                {
                    this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_TimeScale").floatValue = 1f;
                }
            }
            base.m_SerializedObject.ApplyModifiedProperties();
        }

        private void SetMinMaxThreshold()
        {
            base.m_SerializedObject.Update();
            SerializedProperty property = this.m_Childs.GetArrayElementAtIndex(0).FindPropertyRelative("m_Threshold");
            SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1).FindPropertyRelative("m_Threshold");
            this.m_MinThreshold.floatValue = Mathf.Min(property.floatValue, property2.floatValue);
            this.m_MaxThreshold.floatValue = Mathf.Max(property.floatValue, property2.floatValue);
            base.m_SerializedObject.ApplyModifiedProperties();
        }

        private void SetMinMaxThresholds()
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold");
                positiveInfinity = (property2.floatValue >= positiveInfinity) ? positiveInfinity : property2.floatValue;
                negativeInfinity = (property2.floatValue <= negativeInfinity) ? negativeInfinity : property2.floatValue;
            }
            this.m_MinThreshold.floatValue = (this.m_Childs.arraySize <= 0) ? 0f : positiveInfinity;
            this.m_MaxThreshold.floatValue = (this.m_Childs.arraySize <= 0) ? 1f : negativeInfinity;
        }

        private void SetNewThresholdAndPosition(int index)
        {
            base.serializedObject.Update();
            if (!this.m_UseAutomaticThresholds.boolValue)
            {
                float num = 0f;
                if ((this.m_Childs.arraySize >= 3) && (index == (this.m_Childs.arraySize - 1)))
                {
                    float floatValue = this.m_Childs.GetArrayElementAtIndex(index - 2).FindPropertyRelative("m_Threshold").floatValue;
                    float num3 = this.m_Childs.GetArrayElementAtIndex(index - 1).FindPropertyRelative("m_Threshold").floatValue;
                    num = num3 + (num3 - floatValue);
                }
                else if (this.m_Childs.arraySize == 1)
                {
                    num = 0f;
                }
                else
                {
                    num = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1).FindPropertyRelative("m_Threshold").floatValue + 1f;
                }
                this.m_Childs.GetArrayElementAtIndex(index).FindPropertyRelative("m_Threshold").floatValue = num;
                this.SetMinMaxThresholds();
            }
            Vector2 zero = Vector2.zero;
            if (this.m_Childs.arraySize >= 1)
            {
                Vector2 center = this.m_BlendRect.center;
                Vector2[] motionPositions = this.GetMotionPositions();
                float num4 = this.m_BlendRect.width * 0.07f;
                bool flag = false;
                for (int i = 0; i < 0x18; i++)
                {
                    flag = true;
                    for (int j = 0; (j < motionPositions.Length) && flag; j++)
                    {
                        if ((j != index) && (Vector2.Distance(motionPositions[j], zero) < num4))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                    float f = (i * 15) * 0.01745329f;
                    zero = center + ((Vector2) ((new Vector2(-Mathf.Cos(f), Mathf.Sin(f)) * 0.37f) * this.m_BlendRect.width));
                    zero.x = MathUtils.RoundBasedOnMinimumDifference(zero.x, this.m_BlendRect.width * 0.005f);
                    zero.y = MathUtils.RoundBasedOnMinimumDifference(zero.y, this.m_BlendRect.width * 0.005f);
                }
            }
            this.m_Childs.GetArrayElementAtIndex(index).FindPropertyRelative("m_Position").vector2Value = zero;
            base.serializedObject.ApplyModifiedProperties();
        }

        private void ShowHelp(Rect area, GUIContent content)
        {
            float height = EditorStyles.helpBox.CalcHeight(content, area.width);
            GUI.Label(new Rect(area.x, area.y, area.width, height), content, EditorStyles.helpBox);
        }

        private void SortByThreshold()
        {
            base.m_SerializedObject.Update();
            for (int i = 0; i < this.m_Childs.arraySize; i++)
            {
                float positiveInfinity = float.PositiveInfinity;
                int srcIndex = -1;
                for (int j = i; j < this.m_Childs.arraySize; j++)
                {
                    float floatValue = this.m_Childs.GetArrayElementAtIndex(j).FindPropertyRelative("m_Threshold").floatValue;
                    if (floatValue < positiveInfinity)
                    {
                        positiveInfinity = floatValue;
                        srcIndex = j;
                    }
                }
                if (srcIndex != i)
                {
                    this.m_Childs.MoveArrayElement(srcIndex, i);
                }
            }
            base.m_SerializedObject.ApplyModifiedProperties();
        }

        private void ThresholdValues()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            Rect position = controlRect;
            Rect rect3 = controlRect;
            position.width /= 4f;
            rect3.width /= 4f;
            rect3.x = (controlRect.x + controlRect.width) - rect3.width;
            float floatValue = this.m_MinThreshold.floatValue;
            float num2 = this.m_MaxThreshold.floatValue;
            EditorGUI.BeginChangeCheck();
            floatValue = this.ClickDragFloat(position, floatValue);
            num2 = this.ClickDragFloat(rect3, num2, true);
            if (EditorGUI.EndChangeCheck())
            {
                if (this.m_Childs.arraySize >= 2)
                {
                    SerializedProperty arrayElementAtIndex = this.m_Childs.GetArrayElementAtIndex(0);
                    SerializedProperty property2 = this.m_Childs.GetArrayElementAtIndex(this.m_Childs.arraySize - 1);
                    SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_Threshold");
                    SerializedProperty property4 = property2.FindPropertyRelative("m_Threshold");
                    property3.floatValue = Mathf.Min(floatValue, num2);
                    property4.floatValue = Mathf.Max(floatValue, num2);
                }
                if (!this.m_UseAutomaticThresholds.boolValue)
                {
                    for (int i = 0; i < this.m_Childs.arraySize; i++)
                    {
                        SerializedProperty property6 = this.m_Childs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Threshold");
                        float t = Mathf.InverseLerp(this.m_MinThreshold.floatValue, this.m_MaxThreshold.floatValue, property6.floatValue);
                        property6.floatValue = Mathf.Lerp(Mathf.Min(floatValue, num2), Mathf.Max(floatValue, num2), t);
                    }
                }
                this.m_MinThreshold.floatValue = Mathf.Min(floatValue, num2);
                this.m_MaxThreshold.floatValue = Mathf.Max(floatValue, num2);
            }
        }

        private void UpdateBlendVisualization()
        {
            Vector2[] activeMotionPositions = this.GetActiveMotionPositions();
            if (this.m_BlendTex == null)
            {
                this.m_BlendTex = new Texture2D(0x40, 0x40, TextureFormat.RGBA32, false);
                this.m_BlendTex.hideFlags = HideFlags.HideAndDontSave;
                this.m_BlendTex.wrapMode = TextureWrapMode.Clamp;
            }
            while (this.m_WeightTexs.Count < activeMotionPositions.Length)
            {
                Texture2D item = new Texture2D(0x40, 0x40, TextureFormat.RGBA32, false) {
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave
                };
                this.m_WeightTexs.Add(item);
            }
            while (this.m_WeightTexs.Count > activeMotionPositions.Length)
            {
                Object.DestroyImmediate(this.m_WeightTexs[this.m_WeightTexs.Count - 1]);
                this.m_WeightTexs.RemoveAt(this.m_WeightTexs.Count - 1);
            }
            if (GUIUtility.hotControl == 0)
            {
                this.m_BlendRect = this.Get2DBlendRect(this.GetMotionPositions());
            }
            this.m_VisBlendTree.Reset();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Texture2D[] weightTextures = this.m_WeightTexs.ToArray();
            if ((GUIUtility.hotControl != 0) && (this.m_ReorderableList.index >= 0))
            {
                int[] motionToActiveMotionIndices = this.GetMotionToActiveMotionIndices();
                for (int i = 0; i < weightTextures.Length; i++)
                {
                    if (motionToActiveMotionIndices[this.m_ReorderableList.index] != i)
                    {
                        weightTextures[i] = null;
                    }
                }
            }
            BlendTreePreviewUtility.CalculateBlendTexture(this.m_VisBlendTree.animator, 0, this.m_VisBlendTree.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_BlendTex, weightTextures, this.m_BlendRect);
            stopwatch.Stop();
        }

        private void ValidatePositions()
        {
            this.m_WarningMessage = null;
            Vector2[] motionPositions = this.GetMotionPositions();
            bool flag = (this.m_BlendRect.width == 0f) || (this.m_BlendRect.height == 0f);
            for (int i = 0; i < motionPositions.Length; i++)
            {
                for (int j = 0; (j < i) && !flag; j++)
                {
                    Vector2 vector = (Vector2) ((motionPositions[i] - motionPositions[j]) / this.m_BlendRect.height);
                    if (vector.sqrMagnitude < 0.0001f)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                this.m_WarningMessage = "Two or more of the positions are too close to each other.";
            }
            else if (this.m_BlendType.intValue == 1)
            {
                if (<>f__am$cache2B == null)
                {
                    <>f__am$cache2B = e => e != Vector2.zero;
                }
                if (<>f__am$cache2C == null)
                {
                    <>f__am$cache2C = e => Mathf.Atan2(e.y, e.x);
                }
                if (<>f__am$cache2D == null)
                {
                    <>f__am$cache2D = e => e;
                }
                List<float> list = motionPositions.Where<Vector2>(<>f__am$cache2B).Select<Vector2, float>(<>f__am$cache2C).OrderBy<float, float>(<>f__am$cache2D).ToList<float>();
                float num3 = 0f;
                float num4 = 180f;
                for (int k = 0; k < list.Count; k++)
                {
                    float num6 = list[(k + 1) % list.Count] - list[k];
                    if (k == (list.Count - 1))
                    {
                        num6 += 6.283185f;
                    }
                    if (num6 > num3)
                    {
                        num3 = num6;
                    }
                    if (num6 < num4)
                    {
                        num4 = num6;
                    }
                }
                if ((num3 * 57.29578f) >= 180f)
                {
                    this.m_WarningMessage = "Simple Directional blend should have motions with directions less than 180 degrees apart.";
                }
                else if ((num4 * 57.29578f) < 2f)
                {
                    this.m_WarningMessage = "Simple Directional blend should not have multiple motions in almost the same direction.";
                }
            }
            else if (this.m_BlendType.intValue == 2)
            {
                bool flag2 = false;
                for (int m = 0; m < motionPositions.Length; m++)
                {
                    if (motionPositions[m] == Vector2.zero)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    this.m_WarningMessage = "Freeform Directional blend should have one motion at position (0,0) to avoid discontinuities.";
                }
            }
        }

        private int ParameterCount
        {
            get
            {
                return ((this.m_BlendType.intValue <= 0) ? 1 : ((this.m_BlendType.intValue >= 4) ? 0 : 2));
            }
        }

        private enum ChildPropertyToCompute
        {
            Threshold,
            PositionX,
            PositionY
        }

        private delegate float GetFloatFromMotion(Motion motion, float mirrorMultiplier);

        private class Styles
        {
            public readonly GUIStyle background = "MeBlendBackground";
            public readonly GUIStyle blendPosition = "MeBlendPosition";
            public GUIStyle clickDragFloatFieldLeft = new GUIStyle(EditorStyles.miniTextField);
            public GUIStyle clickDragFloatFieldRight = new GUIStyle(EditorStyles.miniTextField);
            public GUIStyle clickDragFloatLabelLeft = new GUIStyle(EditorStyles.miniLabel);
            public GUIStyle clickDragFloatLabelRight = new GUIStyle(EditorStyles.miniLabel);
            public GUIStyle errorStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            public GUIStyle headerIcon = new GUIStyle();
            public GUIContent mirrorIcon = new GUIContent(EditorGUIUtility.IconContent("Mirror"));
            public Texture2D pointIcon = EditorGUIUtility.LoadIcon("blendKey");
            public Texture2D pointIconOverlay = EditorGUIUtility.LoadIcon("blendKeyOverlay");
            public Texture2D pointIconSelected = EditorGUIUtility.LoadIcon("blendKeySelected");
            public Texture2D samplerIcon = EditorGUIUtility.LoadIcon("blendSampler");
            public GUIContent speedIcon = new GUIContent(EditorGUIUtility.IconContent("SpeedScale"));
            public readonly GUIStyle triangleLeft = "MeBlendTriangleLeft";
            public readonly GUIStyle triangleRight = "MeBlendTriangleRight";

            public Styles()
            {
                this.errorStyle.alignment = TextAnchor.MiddleCenter;
                this.speedIcon.tooltip = "Changes animation speed.";
                this.mirrorIcon.tooltip = "Mirror animation.";
                this.headerIcon.alignment = TextAnchor.MiddleCenter;
                this.clickDragFloatFieldLeft.alignment = TextAnchor.MiddleLeft;
                this.clickDragFloatFieldRight.alignment = TextAnchor.MiddleRight;
                this.clickDragFloatLabelLeft.alignment = TextAnchor.MiddleLeft;
                this.clickDragFloatLabelRight.alignment = TextAnchor.MiddleRight;
            }
        }
    }
}

