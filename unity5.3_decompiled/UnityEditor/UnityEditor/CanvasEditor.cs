namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(Canvas))]
    internal class CanvasEditor : Editor
    {
        private GUIContent eventCamera = new GUIContent("Event Camera", "The Camera which the events are triggered through. This is used to determine clicking and hover positions if the Canvas is in World Space render mode.");
        private bool m_AllNested;
        private bool m_AllOverlay;
        private bool m_AllRoot;
        private SerializedProperty m_Camera;
        private AnimBool m_CameraMode;
        private bool m_NoneOverlay;
        private AnimBool m_OverlayMode;
        private SerializedProperty m_OverrideSorting;
        private SerializedProperty m_PixelPerfect;
        private SerializedProperty m_PixelPerfectOverride;
        private SerializedProperty m_PlaneDistance;
        private SerializedProperty m_RenderMode;
        private SerializedProperty m_SortingLayerID;
        private GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer");
        private SerializedProperty m_SortingOrder;
        private GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer");
        private AnimBool m_SortingOverride;
        private SerializedProperty m_TargetDisplay;
        private AnimBool m_WorldMode;
        private PixelPerfect pixelPerfect;
        private GUIContent renderCamera = new GUIContent("Render Camera", "The Camera which will render the canvas. This is also the camera used to send events.");
        private static string s_RootAndNestedMessage = "Cannot multi-edit root Canvas together with nested Canvas.";
        private GUIContent sortingOrder = new GUIContent("Sort Order", "The order in which Screen Space - Overlay canvas will render");
        private GUIContent targetDisplay = new GUIContent("Target Display", "Display on which to render the canvas when in overlay mode");

        private void OnDisable()
        {
            this.m_OverlayMode.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_CameraMode.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_WorldMode.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_SortingOverride.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        private void OnEnable()
        {
            this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
            this.m_Camera = base.serializedObject.FindProperty("m_Camera");
            this.m_PixelPerfect = base.serializedObject.FindProperty("m_PixelPerfect");
            this.m_PlaneDistance = base.serializedObject.FindProperty("m_PlaneDistance");
            this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
            this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
            this.m_TargetDisplay = base.serializedObject.FindProperty("m_TargetDisplay");
            this.m_OverrideSorting = base.serializedObject.FindProperty("m_OverrideSorting");
            this.m_PixelPerfectOverride = base.serializedObject.FindProperty("m_OverridePixelPerfect");
            this.m_OverlayMode = new AnimBool(this.m_RenderMode.intValue == 0);
            this.m_OverlayMode.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_CameraMode = new AnimBool(this.m_RenderMode.intValue == 1);
            this.m_CameraMode.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_WorldMode = new AnimBool(this.m_RenderMode.intValue == 2);
            this.m_WorldMode.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_SortingOverride = new AnimBool(this.m_OverrideSorting.boolValue);
            this.m_SortingOverride.valueChanged.AddListener(new UnityAction(this.Repaint));
            if (this.m_PixelPerfectOverride.boolValue)
            {
                this.pixelPerfect = !this.m_PixelPerfect.boolValue ? PixelPerfect.Off : PixelPerfect.On;
            }
            else
            {
                this.pixelPerfect = PixelPerfect.Inherit;
            }
            this.m_AllNested = true;
            this.m_AllRoot = true;
            this.m_AllOverlay = true;
            this.m_NoneOverlay = true;
            for (int i = 0; i < base.targets.Length; i++)
            {
                Canvas canvas = base.targets[i] as Canvas;
                if (canvas.transform.parent == null)
                {
                    this.m_AllNested = false;
                }
                else if (canvas.transform.parent.GetComponentInParent<Canvas>() == null)
                {
                    this.m_AllNested = false;
                }
                else
                {
                    this.m_AllRoot = false;
                }
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    this.m_NoneOverlay = false;
                }
                else
                {
                    this.m_AllOverlay = false;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            if (this.m_AllRoot)
            {
                EditorGUILayout.PropertyField(this.m_RenderMode, new GUILayoutOption[0]);
                this.m_OverlayMode.target = this.m_RenderMode.intValue == 0;
                this.m_CameraMode.target = this.m_RenderMode.intValue == 1;
                this.m_WorldMode.target = this.m_RenderMode.intValue == 2;
                EditorGUI.indentLevel++;
                if (EditorGUILayout.BeginFadeGroup(this.m_OverlayMode.faded))
                {
                    EditorGUILayout.PropertyField(this.m_PixelPerfect, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_SortingOrder, this.sortingOrder, new GUILayoutOption[0]);
                    GUIContent[] displayNames = DisplayUtility.GetDisplayNames();
                    EditorGUILayout.IntPopup(this.m_TargetDisplay, displayNames, DisplayUtility.GetDisplayIndices(), this.targetDisplay, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_CameraMode.faded))
                {
                    EditorGUILayout.PropertyField(this.m_PixelPerfect, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_Camera, this.renderCamera, new GUILayoutOption[0]);
                    if (this.m_Camera.objectReferenceValue != null)
                    {
                        EditorGUILayout.PropertyField(this.m_PlaneDistance, new GUILayoutOption[0]);
                    }
                    EditorGUILayout.Space();
                    if (this.m_Camera.objectReferenceValue != null)
                    {
                        EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup, EditorStyles.label);
                    }
                    EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
                    if (this.m_Camera.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Screen Space - Camera with no specified camera acts like a Overlay Canvas", MessageType.Warning);
                    }
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_WorldMode.faded))
                {
                    EditorGUILayout.PropertyField(this.m_Camera, this.eventCamera, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup);
                    EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }
            else if (this.m_AllNested)
            {
                EditorGUI.BeginChangeCheck();
                this.pixelPerfect = (PixelPerfect) EditorGUILayout.EnumPopup("Pixel Perfect", this.pixelPerfect, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.pixelPerfect == PixelPerfect.Inherit)
                    {
                        this.m_PixelPerfectOverride.boolValue = false;
                    }
                    else if (this.pixelPerfect == PixelPerfect.Off)
                    {
                        this.m_PixelPerfectOverride.boolValue = true;
                        this.m_PixelPerfect.boolValue = false;
                    }
                    else
                    {
                        this.m_PixelPerfectOverride.boolValue = true;
                        this.m_PixelPerfect.boolValue = true;
                    }
                }
                EditorGUILayout.PropertyField(this.m_OverrideSorting, new GUILayoutOption[0]);
                this.m_SortingOverride.target = this.m_OverrideSorting.boolValue;
                if (EditorGUILayout.BeginFadeGroup(this.m_SortingOverride.faded))
                {
                    if (this.m_AllOverlay)
                    {
                        EditorGUILayout.PropertyField(this.m_SortingOrder, this.sortingOrder, new GUILayoutOption[0]);
                    }
                    else if (this.m_NoneOverlay)
                    {
                        EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup);
                        EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            else
            {
                GUILayout.Label(s_RootAndNestedMessage, EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private enum PixelPerfect
        {
            Inherit,
            On,
            Off
        }
    }
}

