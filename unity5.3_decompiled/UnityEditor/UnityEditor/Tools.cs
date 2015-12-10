namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    public sealed class Tools : ScriptableObject
    {
        private Tool currentTool = Tool.Move;
        internal static Vector3 handleOffset;
        private static PrefKey kMoveKey = new PrefKey("Tools/Move", "w");
        private static PrefKey kPivotMode = new PrefKey("Tools/Pivot Mode", "z");
        private static PrefKey kPivotRotation = new PrefKey("Tools/Pivot Rotation", "x");
        private static PrefKey kRectKey = new PrefKey("Tools/Rect Handles", "t");
        private static PrefKey kRotateKey = new PrefKey("Tools/Rotate", "e");
        private static PrefKey kScaleKey = new PrefKey("Tools/Scale", "r");
        private static PrefKey kViewKey = new PrefKey("Tools/View", "q");
        internal static Vector3 localHandleOffset;
        internal Quaternion m_GlobalHandleRotation = Quaternion.identity;
        private int m_LockedLayers = -1;
        private PivotMode m_PivotMode;
        private PivotRotation m_PivotRotation;
        private bool m_RectBlueprintMode;
        private ViewTool m_ViewTool = ViewTool.Pan;
        private int m_VisibleLayers = -1;
        internal static OnToolChangedFunc onToolChanged;
        private static int originalTool;
        internal static int s_ButtonDown = -1;
        private static Tools s_Get;
        internal static bool s_Hidden = false;
        internal static ViewTool s_LockedViewTool = ViewTool.None;
        private static Vector3 s_LockHandlePosition;
        private static bool s_LockHandlePositionActive = false;
        private static int s_LockHandleRectAxis;
        private static bool s_LockHandleRectAxisActive = false;
        internal static bool vertexDragging;

        internal static void ControlsHack()
        {
            Event current = Event.current;
            if (kViewKey.activated)
            {
                Tools.current = Tool.View;
                ResetGlobalHandleRotation();
                current.Use();
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
                else
                {
                    Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
                }
            }
            if (kMoveKey.activated)
            {
                Tools.current = Tool.Move;
                ResetGlobalHandleRotation();
                current.Use();
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
                else
                {
                    Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
                }
            }
            if (kRotateKey.activated)
            {
                Tools.current = Tool.Rotate;
                ResetGlobalHandleRotation();
                current.Use();
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
                else
                {
                    Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
                }
            }
            if (kScaleKey.activated)
            {
                Tools.current = Tool.Scale;
                ResetGlobalHandleRotation();
                current.Use();
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
                else
                {
                    Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
                }
            }
            if (kRectKey.activated)
            {
                Tools.current = Tool.Rect;
                ResetGlobalHandleRotation();
                current.Use();
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
                else
                {
                    Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
                }
            }
            if (kPivotMode.activated)
            {
                pivotMode = PivotMode.Pivot - pivotMode;
                ResetGlobalHandleRotation();
                current.Use();
                RepaintAllToolViews();
            }
            if (kPivotRotation.activated)
            {
                pivotRotation = PivotRotation.Global - pivotRotation;
                ResetGlobalHandleRotation();
                current.Use();
                RepaintAllToolViews();
            }
        }

        internal static Vector3 GetHandlePosition()
        {
            Transform activeTransform = Selection.activeTransform;
            if (activeTransform == null)
            {
                return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            }
            Vector3 vector = handleOffset + (handleRotation * localHandleOffset);
            PivotMode pivotMode = get.m_PivotMode;
            if (pivotMode != PivotMode.Center)
            {
                if (pivotMode != PivotMode.Pivot)
                {
                    return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                }
            }
            else
            {
                if (current == Tool.Rect)
                {
                    return (((Vector3) (handleRotation * InternalEditorUtility.CalculateSelectionBoundsInSpace(Vector3.zero, handleRotation, rectBlueprintMode).center)) + vector);
                }
                return (InternalEditorUtility.CalculateSelectionBounds(true, false).center + vector);
            }
            if (((current == Tool.Rect) && rectBlueprintMode) && InternalEditorUtility.SupportsRectLayout(activeTransform))
            {
                return (activeTransform.parent.TransformPoint(new Vector3(activeTransform.localPosition.x, activeTransform.localPosition.y, 0f)) + vector);
            }
            return (activeTransform.position + vector);
        }

        internal static int GetPivotMode()
        {
            return (int) pivotMode;
        }

        private static int GetRectAxisForViewDir(Bounds bounds, Quaternion rotation, Vector3 viewDir)
        {
            if (s_LockHandleRectAxisActive)
            {
                return s_LockHandleRectAxis;
            }
            if (viewDir == Vector3.zero)
            {
                return 2;
            }
            if (bounds.size == Vector3.zero)
            {
                bounds.size = Vector3.one;
            }
            int num = -1;
            float num2 = -1f;
            for (int i = 0; i < 3; i++)
            {
                Vector3 zero = Vector3.zero;
                Vector3 vector2 = Vector3.zero;
                int num4 = (i + 1) % 3;
                int num5 = (i + 2) % 3;
                zero[num4] = bounds.size[num4];
                vector2[num5] = bounds.size[num5];
                float magnitude = Vector3.Cross(Vector3.ProjectOnPlane((Vector3) (rotation * zero), viewDir), Vector3.ProjectOnPlane((Vector3) (rotation * vector2), viewDir)).magnitude;
                if (magnitude > num2)
                {
                    num2 = magnitude;
                    num = i;
                }
            }
            return num;
        }

        private static Rect GetRectFromBoundsForAxis(Bounds bounds, int axis)
        {
            switch (axis)
            {
                case 0:
                    return new Rect(-bounds.max.z, bounds.min.y, bounds.size.z, bounds.size.y);

                case 1:
                    return new Rect(bounds.min.x, -bounds.max.z, bounds.size.x, bounds.size.z);
            }
            return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
        }

        private static Quaternion GetRectRotationForAxis(Quaternion rotation, int axis)
        {
            switch (axis)
            {
                case 0:
                    return (rotation * Quaternion.Euler(0f, 90f, 0f));

                case 1:
                    return (rotation * Quaternion.Euler(-90f, 0f, 0f));

                case 2:
                    return rotation;
            }
            return rotation;
        }

        internal static void HandleKeys()
        {
            ControlsHack();
        }

        internal static void LockHandlePosition()
        {
            LockHandlePosition(handlePosition);
        }

        internal static void LockHandlePosition(Vector3 pos)
        {
            s_LockHandlePosition = pos;
            s_LockHandlePositionActive = true;
        }

        internal static void LockHandleRectRotation()
        {
            s_LockHandleRectAxis = GetRectAxisForViewDir(InternalEditorUtility.CalculateSelectionBoundsInSpace(handlePosition, handleRotation, rectBlueprintMode), handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
            s_LockHandleRectAxisActive = true;
        }

        private void OnDisable()
        {
            EditorApplication.globalEventHandler = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.globalEventHandler, new EditorApplication.CallbackFunction(Tools.ControlsHack));
        }

        private void OnEnable()
        {
            s_Get = this;
            EditorApplication.globalEventHandler = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.globalEventHandler, new EditorApplication.CallbackFunction(Tools.ControlsHack));
            pivotMode = (PivotMode) EditorPrefs.GetInt("PivotMode", 0);
            rectBlueprintMode = EditorPrefs.GetBool("RectBlueprintMode", false);
            pivotRotation = (PivotRotation) EditorPrefs.GetInt("PivotRotation", 0);
            visibleLayers = EditorPrefs.GetInt("VisibleLayers", -1);
            lockedLayers = EditorPrefs.GetInt("LockedLayers", 0);
        }

        internal static void OnSelectionChange()
        {
            ResetGlobalHandleRotation();
            localHandleOffset = Vector3.zero;
        }

        internal static void RepaintAllToolViews()
        {
            if (Toolbar.get != null)
            {
                Toolbar.get.Repaint();
            }
            SceneView.RepaintAll();
            InspectorWindow.RepaintAllInspectors();
        }

        internal static void ResetGlobalHandleRotation()
        {
            get.m_GlobalHandleRotation = Quaternion.identity;
        }

        internal static void UnlockHandlePosition()
        {
            s_LockHandlePositionActive = false;
        }

        internal static void UnlockHandleRectRotation()
        {
            s_LockHandleRectAxisActive = false;
        }

        public static Tool current
        {
            get
            {
                return get.currentTool;
            }
            set
            {
                if (get.currentTool != value)
                {
                    Tool currentTool = get.currentTool;
                    get.currentTool = value;
                    if (onToolChanged != null)
                    {
                        onToolChanged(currentTool, value);
                    }
                    RepaintAllToolViews();
                }
            }
        }

        private static Tools get
        {
            get
            {
                if (s_Get == null)
                {
                    s_Get = ScriptableObject.CreateInstance<Tools>();
                    s_Get.hideFlags = HideFlags.HideAndDontSave;
                }
                return s_Get;
            }
        }

        internal static Quaternion handleLocalRotation
        {
            get
            {
                Transform activeTransform = Selection.activeTransform;
                if (activeTransform == null)
                {
                    return Quaternion.identity;
                }
                if (rectBlueprintMode && InternalEditorUtility.SupportsRectLayout(activeTransform))
                {
                    return activeTransform.parent.rotation;
                }
                return activeTransform.rotation;
            }
        }

        public static Vector3 handlePosition
        {
            get
            {
                if (Selection.activeTransform == null)
                {
                    return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                }
                if (s_LockHandlePositionActive)
                {
                    return s_LockHandlePosition;
                }
                return GetHandlePosition();
            }
        }

        public static Rect handleRect
        {
            get
            {
                Bounds bounds = InternalEditorUtility.CalculateSelectionBoundsInSpace(handlePosition, handleRotation, rectBlueprintMode);
                int axis = GetRectAxisForViewDir(bounds, handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
                return GetRectFromBoundsForAxis(bounds, axis);
            }
        }

        public static Quaternion handleRectRotation
        {
            get
            {
                int axis = GetRectAxisForViewDir(InternalEditorUtility.CalculateSelectionBoundsInSpace(handlePosition, handleRotation, rectBlueprintMode), handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
                return GetRectRotationForAxis(handleRotation, axis);
            }
        }

        public static Quaternion handleRotation
        {
            get
            {
                PivotRotation pivotRotation = get.m_PivotRotation;
                if (pivotRotation == PivotRotation.Local)
                {
                    return handleLocalRotation;
                }
                if (pivotRotation != PivotRotation.Global)
                {
                    return Quaternion.identity;
                }
                return get.m_GlobalHandleRotation;
            }
            set
            {
                if (get.m_PivotRotation == PivotRotation.Global)
                {
                    get.m_GlobalHandleRotation = value;
                }
            }
        }

        public static bool hidden
        {
            get
            {
                return s_Hidden;
            }
            set
            {
                s_Hidden = value;
            }
        }

        public static int lockedLayers
        {
            get
            {
                return get.m_LockedLayers;
            }
            set
            {
                if (get.m_LockedLayers != value)
                {
                    get.m_LockedLayers = value;
                    EditorGUIUtility.SetLockedLayers(value);
                    EditorPrefs.SetInt("LockedLayers", lockedLayers);
                }
            }
        }

        public static PivotMode pivotMode
        {
            get
            {
                return get.m_PivotMode;
            }
            set
            {
                if (get.m_PivotMode != value)
                {
                    get.m_PivotMode = value;
                    EditorPrefs.SetInt("PivotMode", (int) pivotMode);
                }
            }
        }

        public static PivotRotation pivotRotation
        {
            get
            {
                return get.m_PivotRotation;
            }
            set
            {
                if (get.m_PivotRotation != value)
                {
                    get.m_PivotRotation = value;
                    EditorPrefs.SetInt("PivotRotation", (int) pivotRotation);
                }
            }
        }

        public static bool rectBlueprintMode
        {
            get
            {
                return get.m_RectBlueprintMode;
            }
            set
            {
                if (get.m_RectBlueprintMode != value)
                {
                    get.m_RectBlueprintMode = value;
                    EditorPrefs.SetBool("RectBlueprintMode", rectBlueprintMode);
                }
            }
        }

        public static ViewTool viewTool
        {
            get
            {
                Event current = Event.current;
                if ((current != null) && viewToolActive)
                {
                    if (s_LockedViewTool == ViewTool.None)
                    {
                        bool flag = current.control && (Application.platform == RuntimePlatform.OSXEditor);
                        bool actionKey = EditorGUI.actionKey;
                        bool flag3 = (!actionKey && !flag) && !current.alt;
                        if ((((s_ButtonDown <= 0) && flag3) || ((s_ButtonDown <= 0) && actionKey)) || ((s_ButtonDown == 2) || ((((SceneView.lastActiveSceneView != null) && SceneView.lastActiveSceneView.in2DMode) && ((s_ButtonDown != 1) || !current.alt)) && ((s_ButtonDown > 0) || !flag))))
                        {
                            get.m_ViewTool = ViewTool.Pan;
                        }
                        else if (((s_ButtonDown <= 0) && flag) || ((s_ButtonDown == 1) && current.alt))
                        {
                            get.m_ViewTool = ViewTool.Zoom;
                        }
                        else if ((s_ButtonDown <= 0) && current.alt)
                        {
                            get.m_ViewTool = ViewTool.Orbit;
                        }
                        else if ((s_ButtonDown == 1) && !current.alt)
                        {
                            get.m_ViewTool = ViewTool.FPS;
                        }
                    }
                }
                else
                {
                    get.m_ViewTool = ViewTool.Pan;
                }
                return get.m_ViewTool;
            }
            set
            {
                get.m_ViewTool = value;
            }
        }

        internal static bool viewToolActive
        {
            get
            {
                if ((GUIUtility.hotControl != 0) && (s_LockedViewTool == ViewTool.None))
                {
                    return false;
                }
                return ((((s_LockedViewTool != ViewTool.None) || (current == Tool.View)) || (Event.current.alt || (s_ButtonDown == 1))) || (s_ButtonDown == 2));
            }
        }

        public static int visibleLayers
        {
            get
            {
                return get.m_VisibleLayers;
            }
            set
            {
                if (get.m_VisibleLayers != value)
                {
                    get.m_VisibleLayers = value;
                    EditorGUIUtility.SetVisibleLayers(value);
                    EditorPrefs.SetInt("VisibleLayers", visibleLayers);
                }
            }
        }

        internal delegate void OnToolChangedFunc(Tool from, Tool to);
    }
}

