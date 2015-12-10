namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class RectTool : ManipulationTool
    {
        internal const string kChangingBottom = "ChangingBottom";
        internal const string kChangingHeight = "ChangingHeight";
        internal const string kChangingLeft = "ChangingLeft";
        internal const string kChangingPivot = "ChangingPivot";
        internal const string kChangingPosX = "ChangingPosX";
        internal const string kChangingPosY = "ChangingPosY";
        internal const string kChangingRight = "ChangingRight";
        internal const string kChangingTop = "ChangingTop";
        internal const string kChangingWidth = "ChangingWidth";
        private const float kMinVisibleSize = 0.2f;
        private static Vector2 s_CurrentMousePos;
        private static RectTool s_Instance;
        private static int s_LockAxis = -1;
        private static int s_MoveHandleHash = "MoveHandle".GetHashCode();
        private static bool s_Moving = false;
        private static int s_PivotHandleHash = "PivotHandle".GetHashCode();
        private static int s_ResizeHandlesHash = "ResizeHandles".GetHashCode();
        private static int s_RotationHandlesHash = "RotationHandles".GetHashCode();
        private static Vector2 s_StartMousePos;
        private static Vector3 s_StartMouseWorldPos;
        private static Vector3 s_StartPosition;
        private static Rect s_StartRect = new Rect();
        private static Vector3 s_StartRectPosition;

        private static float DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 vector2 = b - a;
            float sqrMagnitude = vector2.sqrMagnitude;
            if (sqrMagnitude == 0f)
            {
                Vector2 vector3 = point - a;
                return vector3.magnitude;
            }
            float num2 = Vector2.Dot(point - a, b - a) / sqrMagnitude;
            if (num2 < 0f)
            {
                Vector2 vector4 = point - a;
                return vector4.magnitude;
            }
            if (num2 > 1f)
            {
                Vector2 vector5 = point - b;
                return vector5.magnitude;
            }
            Vector2 vector = a + ((Vector2) (num2 * (b - a)));
            Vector2 vector6 = point - vector;
            return vector6.magnitude;
        }

        private static unsafe float DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
        {
            bool flag = false;
            int num = 4;
            for (int i = 0; i < 5; i++)
            {
                Vector3 vector = *((Vector3*) &(screenPoints[i % 4]));
                Vector3 vector2 = *((Vector3*) &(screenPoints[num % 4]));
                if (((vector.y > mousePos.y) != (vector2.y > mousePos.y)) && (mousePos.x < ((((vector2.x - vector.x) * (mousePos.y - vector.y)) / (vector2.y - vector.y)) + vector.x)))
                {
                    flag = !flag;
                }
                num = i;
            }
            if (flag)
            {
                return 0f;
            }
            float num4 = -1f;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = *((Vector3*) &(screenPoints[j]));
                Vector3 b = *((Vector3*) &(screenPoints[(j + 1) % 4]));
                float num3 = DistancePointToLineSegment(mousePos, a, b);
                if ((num3 < num4) || (num4 < 0f))
                {
                    num4 = num3;
                }
            }
            return num4;
        }

        public static Vector2 GetLocalRectPoint(Rect rect, int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector2(rect.xMin, rect.yMax);

                case 1:
                    return new Vector2(rect.xMax, rect.yMax);

                case 2:
                    return new Vector2(rect.xMax, rect.yMin);

                case 3:
                    return new Vector2(rect.xMin, rect.yMin);
            }
            return Vector3.zero;
        }

        private static Vector3 GetRectPointInWorld(Rect rect, Vector3 pivot, Quaternion rotation, int xHandle, int yHandle)
        {
            Vector3 vector;
            vector = (Vector3) new Vector2(vector.x = Mathf.Lerp(rect.xMin, rect.xMax, xHandle * 0.5f), vector.y = Mathf.Lerp(rect.yMin, rect.yMax, yHandle * 0.5f));
            return (((Vector3) (rotation * vector)) + pivot);
        }

        private static Vector3 MoveHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation)
        {
            bool flag2;
            int controlID = GUIUtility.GetControlID(s_MoveHandleHash, FocusType.Passive);
            Vector3 position = pivot;
            float radius = HandleUtility.GetHandleSize(pivot) * 0.2f;
            float num3 = 1f - GUI.color.a;
            Vector3[] worldPoints = new Vector3[] { (rotation * new Vector2(rect.x, rect.y)) + pivot, (rotation * new Vector2(rect.xMax, rect.y)) + pivot, (rotation * new Vector2(rect.xMax, rect.yMax)) + pivot, (rotation * new Vector2(rect.x, rect.yMax)) + pivot };
            VertexSnapping.HandleKeyAndMouseMove(controlID);
            bool flag = ((Selection.transforms.Length == 1) && InternalEditorUtility.SupportsRectLayout(Selection.activeTransform)) && (Selection.activeTransform.parent.rotation == rotation);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(controlID);
            Plane plane = new Plane(worldPoints[0], worldPoints[1], worldPoints[2]);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    flag2 = false;
                    if (!Tools.vertexDragging)
                    {
                        flag2 = (((current.button == 0) && (current.modifiers == EventModifiers.None)) && RectHandles.RaycastGUIPointToWorldHit(current.mousePosition, plane, out s_StartMouseWorldPos)) && ((SceneViewDistanceToRectangle(worldPoints, current.mousePosition) == 0f) || ((num3 > 0f) && (SceneViewDistanceToDisc(pivot, (Vector3) (rotation * Vector3.forward), radius, current.mousePosition) == 0f)));
                        break;
                    }
                    flag2 = true;
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        if (!s_Moving)
                        {
                            Selection.activeGameObject = SceneViewPicking.PickGameObject(current.mousePosition);
                        }
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        HandleUtility.ignoreRaySnapObjects = null;
                        current.Use();
                    }
                    goto Label_05FC;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        s_CurrentMousePos += current.delta;
                        if (!s_Moving)
                        {
                            Vector2 vector8 = s_CurrentMousePos - s_StartMousePos;
                            if (vector8.magnitude > 3f)
                            {
                                s_Moving = true;
                                RectHandles.RaycastGUIPointToWorldHit(s_CurrentMousePos, plane, out s_StartMouseWorldPos);
                            }
                        }
                        if (s_Moving)
                        {
                            if (!Tools.vertexDragging)
                            {
                                Vector3 vector3;
                                ManipulationToolUtility.SetMinDragDifferenceForPos(pivot);
                                if (RectHandles.RaycastGUIPointToWorldHit(s_CurrentMousePos, plane, out vector3))
                                {
                                    Vector3 vector = vector3 - s_StartMouseWorldPos;
                                    if (current.shift)
                                    {
                                        vector = (Vector3) (Quaternion.Inverse(rotation) * vector);
                                        if (s_LockAxis == -1)
                                        {
                                            float introduced28 = Mathf.Abs(vector.x);
                                            s_LockAxis = (introduced28 <= Mathf.Abs(vector.y)) ? 1 : 0;
                                        }
                                        vector[1 - s_LockAxis] = 0f;
                                        vector = (Vector3) (rotation * vector);
                                    }
                                    else
                                    {
                                        s_LockAxis = -1;
                                    }
                                    if (flag)
                                    {
                                        Transform parent = Selection.activeTransform.parent;
                                        Vector3 vector5 = s_StartRectPosition + parent.InverseTransformVector(vector);
                                        vector5.z = 0f;
                                        Quaternion quaternion = Quaternion.Inverse(rotation);
                                        Vector2 snapDistance = (Vector2) ((Vector2.one * HandleUtility.GetHandleSize(position)) * 0.05f);
                                        Vector3 vector9 = (Vector3) (quaternion * parent.TransformVector(Vector3.right));
                                        snapDistance.x /= vector9.x;
                                        Vector3 vector10 = (Vector3) (quaternion * parent.TransformVector(Vector3.up));
                                        snapDistance.y /= vector10.y;
                                        Vector3 positionAfterSnapping = (Vector3) RectTransformSnapping.SnapToGuides(vector5, snapDistance);
                                        ManipulationToolUtility.DisableMinDragDifferenceBasedOnSnapping(vector5, positionAfterSnapping);
                                        vector = parent.TransformVector(positionAfterSnapping - s_StartRectPosition);
                                    }
                                    position = s_StartPosition + vector;
                                    GUI.changed = true;
                                }
                            }
                            else
                            {
                                Vector3 vector2;
                                if (HandleUtility.ignoreRaySnapObjects == null)
                                {
                                    Handles.SetupIgnoreRaySnapObjects();
                                }
                                if (HandleUtility.FindNearestVertex(s_CurrentMousePos, null, out vector2))
                                {
                                    position = vector2;
                                    GUI.changed = true;
                                }
                                ManipulationToolUtility.minDragDifference = (Vector3) Vector2.zero;
                            }
                        }
                        current.Use();
                    }
                    goto Label_05FC;

                case EventType.Repaint:
                    if (!Tools.vertexDragging)
                    {
                        Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 1.5f * num3);
                        Handles.CircleCap(controlID, pivot, rotation, radius);
                        Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 0.3f * num3);
                        Handles.DrawSolidDisc(pivot, (Vector3) (rotation * Vector3.forward), radius);
                    }
                    else
                    {
                        RectHandles.RectScalingCap(controlID, pivot, rotation, 1f);
                    }
                    goto Label_05FC;

                default:
                    goto Label_05FC;
            }
            if (flag2)
            {
                s_StartPosition = pivot;
                s_StartMousePos = s_CurrentMousePos = current.mousePosition;
                s_Moving = false;
                s_LockAxis = -1;
                int num4 = controlID;
                GUIUtility.keyboardControl = num4;
                GUIUtility.hotControl = num4;
                EditorGUIUtility.SetWantsMouseJumping(1);
                HandleUtility.ignoreRaySnapObjects = null;
                current.Use();
                if (flag)
                {
                    Transform activeTransform = Selection.activeTransform;
                    RectTransform component = activeTransform.GetComponent<RectTransform>();
                    Transform parentSpace = activeTransform.parent;
                    RectTransform parentRect = parentSpace.GetComponent<RectTransform>();
                    s_StartRectPosition = (Vector3) component.anchoredPosition;
                    RectTransformSnapping.CalculatePositionSnapValues(parentSpace, activeTransform, parentRect, component);
                }
            }
        Label_05FC:
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPosX", typeForControl);
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingLeft", typeForControl);
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingRight", typeForControl);
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPosY", typeForControl);
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingTop", typeForControl);
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingBottom", typeForControl);
            return position;
        }

        public static void OnGUI(SceneView view)
        {
            if (s_Instance == null)
            {
                s_Instance = new RectTool();
            }
            s_Instance.OnToolGUI(view);
        }

        private static Vector3 PivotHandleGUI(Rect rect, Vector3 pivot, Quaternion rotation)
        {
            int controlID = GUIUtility.GetControlID(s_PivotHandleHash, FocusType.Passive);
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            if ((GUI.color.a > 0f) || (GUIUtility.hotControl == controlID))
            {
                EventType type2 = typeForControl;
                EditorGUI.BeginChangeCheck();
                Vector3 vector = Handles.Slider2D(controlID, pivot, (Vector3) (rotation * Vector3.forward), (Vector3) (rotation * Vector3.right), (Vector3) (rotation * Vector3.up), HandleUtility.GetHandleSize(pivot) * 0.1f, new Handles.DrawCapFunction(RectHandles.PivotCap), Vector2.zero);
                if ((type2 == EventType.MouseDown) && (GUIUtility.hotControl == controlID))
                {
                    RectTransformSnapping.CalculatePivotSnapValues(rect, pivot, rotation);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Vector2 vector2 = (Vector2) (Quaternion.Inverse(rotation) * (vector - pivot));
                    vector2.x /= rect.width;
                    vector2.y /= rect.height;
                    Vector2 vector3 = new Vector2(-rect.x / rect.width, -rect.y / rect.height);
                    Vector2 vector4 = vector3 + vector2;
                    Vector2 snapDistance = (Vector2) ((HandleUtility.GetHandleSize(pivot) * 0.05f) * new Vector2(1f / rect.width, 1f / rect.height));
                    vector2 = RectTransformSnapping.SnapToGuides(vector4, snapDistance) - vector3;
                    vector2.x *= rect.width;
                    vector2.y *= rect.height;
                    pivot += rotation * vector2;
                }
            }
            ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPivot", typeForControl);
            return pivot;
        }

        private static Vector3 ResizeHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation, out Vector3 scalePivot)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                s_StartRect = rect;
            }
            scalePivot = pivot;
            Vector3 one = Vector3.one;
            Quaternion quaternion = Quaternion.Inverse(rotation);
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if ((i != 1) || (j != 1))
                    {
                        Vector3 vector2 = GetRectPointInWorld(s_StartRect, pivot, rotation, i, j);
                        Vector3 position = GetRectPointInWorld(rect, pivot, rotation, i, j);
                        float size = 0.05f * HandleUtility.GetHandleSize(position);
                        int controlID = GUIUtility.GetControlID(s_ResizeHandlesHash, FocusType.Passive);
                        if ((GUI.color.a > 0f) || (GUIUtility.hotControl == controlID))
                        {
                            Vector3 vector4;
                            EditorGUI.BeginChangeCheck();
                            EventType typeBefore = Event.current.type;
                            if ((i == 1) || (j == 1))
                            {
                                Vector3 sideVector = (i != 1) ? ((Vector3) ((rotation * Vector3.up) * rect.height)) : ((Vector3) ((rotation * Vector3.right) * rect.width));
                                Vector3 direction = (i != 1) ? ((Vector3) (rotation * Vector3.right)) : ((Vector3) (rotation * Vector3.up));
                                vector4 = RectHandles.SideSlider(controlID, position, sideVector, direction, size, null, 0f);
                            }
                            else
                            {
                                Vector3 vector7 = (Vector3) ((rotation * Vector3.right) * (i - 1));
                                Vector3 vector8 = (Vector3) ((rotation * Vector3.up) * (j - 1));
                                vector4 = RectHandles.CornerSlider(controlID, position, (Vector3) (rotation * Vector3.forward), vector7, vector8, size, new Handles.DrawCapFunction(RectHandles.RectScalingCap), Vector2.zero);
                            }
                            bool flag = ((Selection.transforms.Length == 1) && InternalEditorUtility.SupportsRectLayout(Selection.activeTransform)) && (Selection.activeTransform.parent.rotation == rotation);
                            if (flag)
                            {
                                Transform activeTransform = Selection.activeTransform;
                                RectTransform component = activeTransform.GetComponent<RectTransform>();
                                Transform parent = activeTransform.parent;
                                RectTransform parentRect = parent.GetComponent<RectTransform>();
                                if ((typeBefore == EventType.MouseDown) && (Event.current.type != EventType.MouseDown))
                                {
                                    RectTransformSnapping.CalculateOffsetSnapValues(parent, activeTransform, parentRect, component, i, j);
                                }
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                ManipulationToolUtility.SetMinDragDifferenceForPos(position);
                                if (flag)
                                {
                                    Transform transform5 = Selection.activeTransform.parent;
                                    RectTransform transform6 = transform5.GetComponent<RectTransform>();
                                    Vector2 snapDistance = (Vector2) ((Vector2.one * HandleUtility.GetHandleSize(vector4)) * 0.05f);
                                    Vector3 vector14 = (Vector3) (quaternion * transform5.TransformVector(Vector3.right));
                                    snapDistance.x /= vector14.x;
                                    Vector3 vector15 = (Vector3) (quaternion * transform5.TransformVector(Vector3.up));
                                    snapDistance.y /= vector15.y;
                                    Vector3 vector10 = transform5.InverseTransformPoint(vector4) - transform6.rect.min;
                                    Vector3 positionAfterSnapping = (Vector3) (RectTransformSnapping.SnapToGuides(vector10, snapDistance) + (Vector3.forward * vector10.z));
                                    ManipulationToolUtility.DisableMinDragDifferenceBasedOnSnapping(vector10, positionAfterSnapping);
                                    vector4 = transform5.TransformPoint(positionAfterSnapping + transform6.rect.min);
                                }
                                bool alt = Event.current.alt;
                                bool actionKey = EditorGUI.actionKey;
                                bool flag4 = Event.current.shift && !actionKey;
                                if (!alt)
                                {
                                    scalePivot = GetRectPointInWorld(s_StartRect, pivot, rotation, 2 - i, 2 - j);
                                }
                                if (flag4)
                                {
                                    vector4 = Vector3.Project(vector4 - scalePivot, vector2 - scalePivot) + scalePivot;
                                }
                                Vector3 vector12 = (Vector3) (quaternion * (vector2 - scalePivot));
                                Vector3 vector13 = (Vector3) (quaternion * (vector4 - scalePivot));
                                if (i != 1)
                                {
                                    one.x = vector13.x / vector12.x;
                                }
                                if (j != 1)
                                {
                                    one.y = vector13.y / vector12.y;
                                }
                                if (flag4)
                                {
                                    float num5 = (i != 1) ? one.x : one.y;
                                    one = (Vector3) (Vector3.one * num5);
                                }
                                if (actionKey && (i == 1))
                                {
                                    if (Event.current.shift)
                                    {
                                        one.x = one.z = 1f / Mathf.Sqrt(Mathf.Max(one.y, 0.0001f));
                                    }
                                    else
                                    {
                                        one.x = 1f / Mathf.Max(one.y, 0.0001f);
                                    }
                                }
                                if (flag4)
                                {
                                    float num6 = (i != 1) ? one.x : one.y;
                                    one = (Vector3) (Vector3.one * num6);
                                }
                                if (actionKey && (i == 1))
                                {
                                    if (Event.current.shift)
                                    {
                                        one.x = one.z = 1f / Mathf.Sqrt(Mathf.Max(one.y, 0.0001f));
                                    }
                                    else
                                    {
                                        one.x = 1f / Mathf.Max(one.y, 0.0001f);
                                    }
                                }
                                if (actionKey && (j == 1))
                                {
                                    if (Event.current.shift)
                                    {
                                        one.y = one.z = 1f / Mathf.Sqrt(Mathf.Max(one.x, 0.0001f));
                                    }
                                    else
                                    {
                                        one.y = 1f / Mathf.Max(one.x, 0.0001f);
                                    }
                                }
                            }
                            switch (i)
                            {
                                case 0:
                                    ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingLeft", typeBefore);
                                    break;

                                case 2:
                                    ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingRight", typeBefore);
                                    break;
                            }
                            if (i != 1)
                            {
                                ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingWidth", typeBefore);
                            }
                            if (j == 0)
                            {
                                ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingBottom", typeBefore);
                            }
                            if (j == 2)
                            {
                                ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingTop", typeBefore);
                            }
                            if (j != 1)
                            {
                                ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingHeight", typeBefore);
                            }
                        }
                    }
                }
            }
            return one;
        }

        private static Quaternion RotationHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation)
        {
            Vector3 eulerAngles = rotation.eulerAngles;
            for (int i = 0; i <= 2; i += 2)
            {
                for (int j = 0; j <= 2; j += 2)
                {
                    Vector3 position = GetRectPointInWorld(rect, pivot, rotation, i, j);
                    float handleSize = 0.05f * HandleUtility.GetHandleSize(position);
                    int controlID = GUIUtility.GetControlID(s_RotationHandlesHash, FocusType.Passive);
                    if ((GUI.color.a > 0f) || (GUIUtility.hotControl == controlID))
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 vector3 = (Vector3) ((rotation * Vector3.right) * (i - 1));
                        Vector3 vector4 = (Vector3) ((rotation * Vector3.up) * (j - 1));
                        float num5 = RectHandles.RotationSlider(controlID, position, eulerAngles.z, pivot, (Vector3) (rotation * Vector3.forward), vector3, vector4, handleSize, null, Vector2.zero);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (Event.current.shift)
                            {
                                num5 = (Mathf.Round((num5 - eulerAngles.z) / 15f) * 15f) + eulerAngles.z;
                            }
                            eulerAngles.z = num5;
                            rotation = Quaternion.Euler(eulerAngles);
                        }
                    }
                }
            }
            return rotation;
        }

        private static float SceneViewDistanceToDisc(Vector3 center, Vector3 normal, float radius, Vector2 mousePos)
        {
            float num;
            Plane plane = new Plane(normal, center);
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            if (plane.Raycast(ray, out num))
            {
                Vector3 point = ray.GetPoint(num);
                Vector3 vector2 = point - center;
                return Mathf.Max((float) 0f, (float) (vector2.magnitude - radius));
            }
            return float.PositiveInfinity;
        }

        private static float SceneViewDistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos)
        {
            Vector2[] screenPoints = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                screenPoints[i] = HandleUtility.WorldToGUIPoint(worldPoints[i]);
            }
            return DistanceToRectangle(screenPoints, mousePos);
        }

        public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
        {
            Rect handleRect = Tools.handleRect;
            Quaternion handleRectRotation = Tools.handleRectRotation;
            Vector3[] corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vector3 localRectPoint = (Vector3) GetLocalRectPoint(handleRect, i);
                corners[i] = ((Vector3) (handleRectRotation * localRectPoint)) + handlePosition;
            }
            RectHandles.RenderRectWithShadow(false, corners);
            Color color = GUI.color;
            float num2 = 1f;
            if (Camera.current != null)
            {
                Vector3 planeNormal = !Camera.current.orthographic ? ((handlePosition + (handleRectRotation * handleRect.center)) - Camera.current.transform.position) : Camera.current.transform.forward;
                Vector3 vector = (Vector3) ((handleRectRotation * Vector3.right) * handleRect.width);
                Vector3 vector4 = (Vector3) ((handleRectRotation * Vector3.up) * handleRect.height);
                float num3 = Mathf.Sqrt(Vector3.Cross(Vector3.ProjectOnPlane(vector, planeNormal), Vector3.ProjectOnPlane(vector4, planeNormal)).magnitude) / HandleUtility.GetHandleSize(handlePosition);
                num2 = Mathf.Clamp01(((num3 - 0.2f) / 0.2f) * 2f);
                Color color2 = color;
                color2.a *= num2;
                GUI.color = color2;
            }
            Vector3 pivot = Tools.GetHandlePosition();
            if (!Tools.vertexDragging)
            {
                RectTransform component = Selection.activeTransform.GetComponent<RectTransform>();
                bool flag = Selection.transforms.Length > 1;
                bool flag2 = (!flag && (Tools.pivotMode == PivotMode.Pivot)) && (component != null);
                EditorGUI.BeginDisabledGroup(!flag && !flag2);
                EditorGUI.BeginChangeCheck();
                Vector3 vector6 = PivotHandleGUI(handleRect, pivot, handleRectRotation);
                if (EditorGUI.EndChangeCheck() && !isStatic)
                {
                    if (flag)
                    {
                        Tools.localHandleOffset += Quaternion.Inverse(Tools.handleRotation) * (vector6 - pivot);
                    }
                    else if (flag2)
                    {
                        Transform activeTransform = Selection.activeTransform;
                        Undo.RecordObject(component, "Move Rectangle Pivot");
                        Transform transform3 = (!Tools.rectBlueprintMode || !InternalEditorUtility.SupportsRectLayout(activeTransform)) ? activeTransform : activeTransform.parent;
                        Vector2 vector7 = transform3.InverseTransformVector(vector6 - pivot);
                        vector7.x /= component.rect.width;
                        vector7.y /= component.rect.height;
                        Vector2 vector8 = component.pivot + vector7;
                        RectTransformEditor.SetPivotSmart(component, vector8.x, 0, true, transform3 != component.transform);
                        RectTransformEditor.SetPivotSmart(component, vector8.y, 1, true, transform3 != component.transform);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            TransformManipulator.BeginManipulationHandling(true);
            if (!Tools.vertexDragging)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 scalePivot = handlePosition;
                Vector3 scaleDelta = ResizeHandlesGUI(handleRect, handlePosition, handleRectRotation, out scalePivot);
                if (EditorGUI.EndChangeCheck() && !isStatic)
                {
                    TransformManipulator.SetResizeDelta(scaleDelta, scalePivot, handleRectRotation);
                }
                bool flag3 = true;
                if (Tools.rectBlueprintMode)
                {
                    foreach (Transform transform4 in Selection.transforms)
                    {
                        if (transform4.GetComponent<RectTransform>() != null)
                        {
                            flag3 = false;
                        }
                    }
                }
                if (flag3)
                {
                    EditorGUI.BeginChangeCheck();
                    Quaternion quaternion2 = RotationHandlesGUI(handleRect, handlePosition, handleRectRotation);
                    if (EditorGUI.EndChangeCheck() && !isStatic)
                    {
                        float num5;
                        Vector3 vector11;
                        (Quaternion.Inverse(handleRectRotation) * quaternion2).ToAngleAxis(out num5, out vector11);
                        vector11 = (Vector3) (handleRectRotation * vector11);
                        Undo.RecordObjects(Selection.transforms, "Rotate");
                        foreach (Transform transform5 in Selection.transforms)
                        {
                            transform5.RotateAround(handlePosition, vector11, num5);
                            if (transform5.parent != null)
                            {
                                transform5.SendTransformChangedScale();
                            }
                        }
                        Tools.handleRotation = Quaternion.AngleAxis(num5, vector11) * Tools.handleRotation;
                    }
                }
            }
            TransformManipulator.EndManipulationHandling();
            TransformManipulator.BeginManipulationHandling(false);
            EditorGUI.BeginChangeCheck();
            Vector3 vector12 = MoveHandlesGUI(handleRect, handlePosition, handleRectRotation);
            if (EditorGUI.EndChangeCheck() && !isStatic)
            {
                Vector3 positionDelta = vector12 - TransformManipulator.mouseDownHandlePosition;
                TransformManipulator.SetPositionDelta(positionDelta);
            }
            TransformManipulator.EndManipulationHandling();
            GUI.color = color;
        }
    }
}

