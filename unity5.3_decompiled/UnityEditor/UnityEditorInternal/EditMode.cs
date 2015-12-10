namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class EditMode
    {
        private const float k_EditColliderbuttonHeight = 23f;
        private const float k_EditColliderbuttonWidth = 33f;
        private const float k_SpaceBetweenLabelAndButton = 5f;
        private const string kEditModeStringKey = "EditModeState";
        private const string kOwnerStringKey = "EditModeOwner";
        private const string kPrevToolStringKey = "EditModePrevTool";
        public static OnEditModeStopFunc onEditModeEndDelegate;
        public static OnEditModeStartFunc onEditModeStartDelegate;
        private static bool s_Debug;
        private static GUIStyle s_EditColliderButtonStyle;
        private static SceneViewEditMode s_EditMode;
        private static int s_OwnerID;
        private static GUIStyle s_ToolbarBaseStyle;
        private static Tool s_ToolBeforeEnteringEditMode = Tool.Move;

        static EditMode()
        {
            ownerID = SessionState.GetInt("EditModeOwner", ownerID);
            editMode = (SceneViewEditMode) SessionState.GetInt("EditModeState", (int) editMode);
            toolBeforeEnteringEditMode = (Tool) SessionState.GetInt("EditModePrevTool", (int) toolBeforeEnteringEditMode);
            Selection.selectionChanged = (Action) Delegate.Combine(Selection.selectionChanged, new Action(EditMode.OnSelectionChange));
            if (s_Debug)
            {
                Debug.Log(string.Concat(new object[] { "EditMode static constructor: ", ownerID, " ", editMode, " ", toolBeforeEnteringEditMode }));
            }
        }

        private static bool AnyPointSeenByCamera(Camera camera, Vector3[] points)
        {
            foreach (Vector3 vector in points)
            {
                if (PointSeenByCamera(camera, vector))
                {
                    return true;
                }
            }
            return false;
        }

        private static Vector3[] BoundsToPoints(Bounds bounds)
        {
            return new Vector3[] { new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), new Vector3(bounds.max.x, bounds.max.y, bounds.max.z) };
        }

        private static void ChangeEditMode(SceneViewEditMode mode, Bounds bounds, Editor caller)
        {
            Editor objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(ownerID) as Editor;
            editMode = mode;
            ownerID = (mode == SceneViewEditMode.None) ? 0 : caller.GetInstanceID();
            if (onEditModeEndDelegate != null)
            {
                onEditModeEndDelegate(objectFromInstanceID);
            }
            if ((editMode != SceneViewEditMode.None) && (onEditModeStartDelegate != null))
            {
                onEditModeStartDelegate(caller, editMode);
            }
            EditModeChanged(bounds);
            InspectorWindow.RepaintAllInspectors();
        }

        private static void DetectMainToolChange()
        {
            if ((Tools.current != Tool.None) && (editMode != SceneViewEditMode.None))
            {
                EndSceneViewEditing();
            }
        }

        public static void DoEditModeInspectorModeButton(SceneViewEditMode mode, string label, GUIContent icon, Bounds bounds, Editor caller)
        {
            if (!EditorUtility.IsPersistent(caller.target))
            {
                DetectMainToolChange();
                if (s_EditColliderButtonStyle == null)
                {
                    s_EditColliderButtonStyle = new GUIStyle("Button");
                    s_EditColliderButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                    s_EditColliderButtonStyle.margin = new RectOffset(0, 0, 0, 0);
                }
                Rect rect = EditorGUILayout.GetControlRect(true, 23f, new GUILayoutOption[0]);
                Rect position = new Rect(rect.xMin + EditorGUIUtility.labelWidth, rect.yMin, 33f, 23f);
                GUIContent content = new GUIContent(label);
                Vector2 vector = GUI.skin.label.CalcSize(content);
                Rect rect3 = new Rect(position.xMax + 5f, rect.yMin + ((rect.height - vector.y) * 0.5f), vector.x, rect.height);
                int instanceID = caller.GetInstanceID();
                bool flag = (editMode == mode) && (ownerID == instanceID);
                EditorGUI.BeginChangeCheck();
                bool flag2 = GUI.Toggle(position, flag, icon, s_EditColliderButtonStyle);
                GUI.Label(rect3, label);
                if (EditorGUI.EndChangeCheck())
                {
                    ChangeEditMode(!flag2 ? SceneViewEditMode.None : mode, bounds, caller);
                }
            }
        }

        public static void DoInspectorToolbar(SceneViewEditMode[] modes, GUIContent[] guiContents, Bounds bounds, Editor caller)
        {
            if (!EditorUtility.IsPersistent(caller.target))
            {
                DetectMainToolChange();
                if (s_ToolbarBaseStyle == null)
                {
                    s_ToolbarBaseStyle = "Command";
                }
                int instanceID = caller.GetInstanceID();
                int index = ArrayUtility.IndexOf<SceneViewEditMode>(modes, editMode);
                if (ownerID != instanceID)
                {
                    index = -1;
                }
                EditorGUI.BeginChangeCheck();
                int num3 = GUILayout.Toolbar(index, guiContents, s_ToolbarBaseStyle, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    SceneViewEditMode mode = (num3 != index) ? modes[num3] : SceneViewEditMode.None;
                    ChangeEditMode(mode, bounds, caller);
                }
            }
        }

        private static void EditModeChanged(Bounds bounds)
        {
            if (((editMode != SceneViewEditMode.None) && (SceneView.lastActiveSceneView != null)) && ((SceneView.lastActiveSceneView.camera != null) && !SeenByCamera(SceneView.lastActiveSceneView.camera, bounds)))
            {
                SceneView.lastActiveSceneView.Frame(bounds);
            }
            SceneView.RepaintAll();
        }

        private static void EndSceneViewEditing()
        {
            ChangeEditMode(SceneViewEditMode.None, new Bounds(), null);
        }

        private static Vector3[] GetPoints(Bounds bounds)
        {
            return BoundsToPoints(bounds);
        }

        public static bool IsOwner(Editor editor)
        {
            return (editor.GetInstanceID() == ownerID);
        }

        public static void OnSelectionChange()
        {
            QuitEditMode();
        }

        private static bool PointSeenByCamera(Camera camera, Vector3 point)
        {
            Vector3 vector = camera.WorldToViewportPoint(point);
            return ((((vector.x > 0f) && (vector.x < 1f)) && (vector.y > 0f)) && (vector.y < 1f));
        }

        public static void QuitEditMode()
        {
            if ((Tools.current == Tool.None) && (editMode != SceneViewEditMode.None))
            {
                ResetToolToPrevious();
            }
            EndSceneViewEditing();
        }

        public static void ResetToolToPrevious()
        {
            if (Tools.current == Tool.None)
            {
                Tools.current = toolBeforeEnteringEditMode;
            }
        }

        private static bool SeenByCamera(Camera camera, Bounds bounds)
        {
            return AnyPointSeenByCamera(camera, GetPoints(bounds));
        }

        public static SceneViewEditMode editMode
        {
            get
            {
                return s_EditMode;
            }
            private set
            {
                if ((s_EditMode == SceneViewEditMode.None) && (value != SceneViewEditMode.None))
                {
                    toolBeforeEnteringEditMode = (Tools.current == Tool.None) ? Tool.Move : Tools.current;
                    Tools.current = Tool.None;
                }
                else if ((s_EditMode != SceneViewEditMode.None) && (value == SceneViewEditMode.None))
                {
                    ResetToolToPrevious();
                }
                s_EditMode = value;
                SessionState.SetInt("EditModeState", (int) s_EditMode);
                if (s_Debug)
                {
                    Debug.Log("Set editMode " + s_EditMode);
                }
            }
        }

        private static int ownerID
        {
            get
            {
                return s_OwnerID;
            }
            set
            {
                s_OwnerID = value;
                SessionState.SetInt("EditModeOwner", s_OwnerID);
                if (s_Debug)
                {
                    Debug.Log("Set ownerID " + value);
                }
            }
        }

        private static Tool toolBeforeEnteringEditMode
        {
            get
            {
                return s_ToolBeforeEnteringEditMode;
            }
            set
            {
                s_ToolBeforeEnteringEditMode = value;
                SessionState.SetInt("EditModePrevTool", (int) s_ToolBeforeEnteringEditMode);
                if (s_Debug)
                {
                    Debug.Log("Set toolBeforeEnteringEditMode " + value);
                }
            }
        }

        public delegate void OnEditModeStartFunc(Editor editor, EditMode.SceneViewEditMode mode);

        public delegate void OnEditModeStopFunc(Editor editor);

        public enum SceneViewEditMode
        {
            None,
            Collider,
            Cloth,
            ReflectionProbeBox,
            ReflectionProbeOrigin
        }
    }
}

