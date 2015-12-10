namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class SceneViewMotion
    {
        private const float kFlyAcceleration = 1.8f;
        private static PrefKey kFPSBack = new PrefKey("View/FPS Back", "s");
        private static PrefKey kFPSDown = new PrefKey("View/FPS Strafe Down", "q");
        private static PrefKey kFPSForward = new PrefKey("View/FPS Forward", "w");
        private static PrefKey kFPSLeft = new PrefKey("View/FPS Strafe Left", "a");
        private static PrefKey kFPSRight = new PrefKey("View/FPS Strafe Right", "d");
        private static PrefKey kFPSUp = new PrefKey("View/FPS Strafe Up", "e");
        private static bool s_Dragged = false;
        private static float s_FlySpeed = 0f;
        private static TimeHelper s_FPSTiming = new TimeHelper();
        private static Vector3 s_Motion;
        private static float s_StartZoom = 0f;
        private static float s_TotalMotion = 0f;
        private static int s_ViewToolID = GUIUtility.GetPermanentControlID();
        private static float s_ZoomSpeed = 0f;

        public static void ArrowKeys(SceneView sv)
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (((GUIUtility.hotControl == 0) || (GUIUtility.hotControl == controlID)) && !EditorGUI.actionKey)
            {
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.KeyDown:
                        switch (current.keyCode)
                        {
                            case KeyCode.UpArrow:
                                sv.viewIsLockedToObject = false;
                                if (sv.m_Ortho.value)
                                {
                                    s_Motion.y = 1f;
                                }
                                else
                                {
                                    s_Motion.z = 1f;
                                }
                                GUIUtility.hotControl = controlID;
                                current.Use();
                                break;

                            case KeyCode.DownArrow:
                                sv.viewIsLockedToObject = false;
                                if (sv.m_Ortho.value)
                                {
                                    s_Motion.y = -1f;
                                }
                                else
                                {
                                    s_Motion.z = -1f;
                                }
                                GUIUtility.hotControl = controlID;
                                current.Use();
                                break;

                            case KeyCode.RightArrow:
                                sv.viewIsLockedToObject = false;
                                s_Motion.x = 1f;
                                GUIUtility.hotControl = controlID;
                                current.Use();
                                break;

                            case KeyCode.LeftArrow:
                                sv.viewIsLockedToObject = false;
                                s_Motion.x = -1f;
                                GUIUtility.hotControl = controlID;
                                current.Use();
                                break;
                        }
                        break;

                    case EventType.KeyUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            switch (current.keyCode)
                            {
                                case KeyCode.UpArrow:
                                case KeyCode.DownArrow:
                                    s_Motion.z = 0f;
                                    s_Motion.y = 0f;
                                    current.Use();
                                    break;

                                case KeyCode.RightArrow:
                                case KeyCode.LeftArrow:
                                    s_Motion.x = 0f;
                                    current.Use();
                                    break;
                            }
                        }
                        break;

                    case EventType.Layout:
                    {
                        Vector3 forward;
                        if (GUIUtility.hotControl != controlID)
                        {
                            break;
                        }
                        if (sv.m_Ortho.value)
                        {
                            forward = Camera.current.transform.forward;
                        }
                        else
                        {
                            forward = Camera.current.transform.forward + ((Vector3) (Camera.current.transform.up * 0.3f));
                            forward.y = 0f;
                            forward.Normalize();
                        }
                        Vector3 movementDirection = GetMovementDirection();
                        sv.LookAtDirect(sv.pivot + (Quaternion.LookRotation(forward) * movementDirection), sv.rotation);
                        if (s_Motion.sqrMagnitude == 0f)
                        {
                            sv.pivot = sv.pivot;
                            s_FlySpeed = 0f;
                            GUIUtility.hotControl = 0;
                        }
                        else
                        {
                            sv.Repaint();
                        }
                        return;
                    }
                }
            }
        }

        public static void DoViewTool(SceneView view)
        {
            Event current = Event.current;
            int controlID = s_ViewToolID;
            EventType typeForControl = current.GetTypeForControl(controlID);
            if ((view != null) && (Tools.s_LockedViewTool == ViewTool.FPS))
            {
                view.FixNegativeSize();
            }
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    HandleMouseDown(view, controlID, current.button);
                    break;

                case EventType.MouseUp:
                    HandleMouseUp(view, controlID, current.button, current.clickCount);
                    break;

                case EventType.MouseDrag:
                    HandleMouseDrag(view, controlID);
                    break;

                case EventType.KeyDown:
                    HandleKeyDown(view);
                    break;

                case EventType.KeyUp:
                    HandleKeyUp();
                    break;

                case EventType.ScrollWheel:
                    HandleScrollWheel(view, !current.alt);
                    break;

                case EventType.Layout:
                {
                    Vector3 movementDirection = GetMovementDirection();
                    if ((GUIUtility.hotControl == controlID) && (movementDirection.sqrMagnitude != 0f))
                    {
                        view.pivot += view.rotation * movementDirection;
                        view.Repaint();
                    }
                    break;
                }
            }
        }

        private static Vector3 GetMovementDirection()
        {
            float p = s_FPSTiming.Update();
            if (s_Motion.sqrMagnitude == 0f)
            {
                s_FlySpeed = 0f;
                return Vector3.zero;
            }
            float num2 = !Event.current.shift ? ((float) 1) : ((float) 5);
            if (s_FlySpeed == 0f)
            {
                s_FlySpeed = 9f;
            }
            else
            {
                s_FlySpeed *= Mathf.Pow(1.8f, p);
            }
            return (Vector3) (((s_Motion.normalized * s_FlySpeed) * num2) * p);
        }

        private static void HandleKeyDown(SceneView sceneView)
        {
            if ((Event.current.keyCode == KeyCode.Escape) && (GUIUtility.hotControl == s_ViewToolID))
            {
                ResetDragState();
            }
            if (Tools.s_LockedViewTool == ViewTool.FPS)
            {
                Event current = Event.current;
                Vector3 vector = s_Motion;
                if (current.keyCode == kFPSForward.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.z = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSBack.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.z = -1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSLeft.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.x = -1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSRight.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.x = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSUp.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.y = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSDown.keyCode)
                {
                    sceneView.viewIsLockedToObject = false;
                    s_Motion.y = -1f;
                    current.Use();
                }
                if ((current.type != EventType.KeyDown) && (vector.sqrMagnitude == 0f))
                {
                    s_FPSTiming.Begin();
                }
            }
        }

        private static void HandleKeyUp()
        {
            if (Tools.s_LockedViewTool == ViewTool.FPS)
            {
                Event current = Event.current;
                if (current.keyCode == kFPSForward.keyCode)
                {
                    s_Motion.z = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSBack.keyCode)
                {
                    s_Motion.z = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSLeft.keyCode)
                {
                    s_Motion.x = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSRight.keyCode)
                {
                    s_Motion.x = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSUp.keyCode)
                {
                    s_Motion.y = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSDown.keyCode)
                {
                    s_Motion.y = 0f;
                    current.Use();
                }
            }
        }

        private static void HandleMouseDown(SceneView view, int id, int button)
        {
            s_Dragged = false;
            if (Tools.viewToolActive)
            {
                ViewTool viewTool = Tools.viewTool;
                if (Tools.s_LockedViewTool != viewTool)
                {
                    Event current = Event.current;
                    GUIUtility.hotControl = id;
                    Tools.s_LockedViewTool = viewTool;
                    s_StartZoom = view.size;
                    s_ZoomSpeed = Mathf.Max(Mathf.Abs(s_StartZoom), 0.3f);
                    s_TotalMotion = 0f;
                    if (view != null)
                    {
                        view.Focus();
                    }
                    if (Toolbar.get != null)
                    {
                        Toolbar.get.Repaint();
                    }
                    EditorGUIUtility.SetWantsMouseJumping(1);
                    current.Use();
                    GUIUtility.ExitGUI();
                }
            }
        }

        private static void HandleMouseDrag(SceneView view, int id)
        {
            s_Dragged = true;
            if (GUIUtility.hotControl != id)
            {
                return;
            }
            Event current = Event.current;
            switch (Tools.s_LockedViewTool)
            {
                case ViewTool.Orbit:
                    if (!view.in2DMode)
                    {
                        OrbitCameraBehavior(view);
                        view.svRot.UpdateGizmoLabel(view, (Vector3) (view.rotation * Vector3.forward), view.m_Ortho.target);
                    }
                    goto Label_02E4;

                case ViewTool.Pan:
                {
                    view.viewIsLockedToObject = false;
                    view.FixNegativeSize();
                    Vector3 position = view.camera.WorldToScreenPoint(view.pivot) + new Vector3(-Event.current.delta.x, Event.current.delta.y, 0f);
                    Vector3 vector3 = Camera.current.ScreenToWorldPoint(position) - view.pivot;
                    if (current.shift)
                    {
                        vector3 = (Vector3) (vector3 * 4f);
                    }
                    view.pivot += vector3;
                    goto Label_02E4;
                }
                case ViewTool.Zoom:
                {
                    float num = HandleUtility.niceMouseDeltaZoom * (!current.shift ? ((float) 3) : ((float) 9));
                    if (!view.orthographic)
                    {
                        s_TotalMotion += num;
                        if (s_TotalMotion < 0f)
                        {
                            view.size = s_StartZoom * (1f + (s_TotalMotion * 0.001f));
                        }
                        else
                        {
                            view.size += (num * s_ZoomSpeed) * 0.003f;
                        }
                    }
                    else
                    {
                        view.size = Mathf.Max((float) 0.0001f, (float) (view.size * (1f + (num * 0.001f))));
                    }
                    goto Label_02E4;
                }
                case ViewTool.FPS:
                {
                    if (view.in2DMode)
                    {
                        goto Label_02E4;
                    }
                    if (view.orthographic)
                    {
                        OrbitCameraBehavior(view);
                        break;
                    }
                    view.viewIsLockedToObject = false;
                    Vector3 vector = view.pivot - ((Vector3) ((view.rotation * Vector3.forward) * view.cameraDistance));
                    Quaternion rotation = view.rotation;
                    rotation = Quaternion.AngleAxis((current.delta.y * 0.003f) * 57.29578f, (Vector3) (rotation * Vector3.right)) * rotation;
                    rotation = Quaternion.AngleAxis((current.delta.x * 0.003f) * 57.29578f, Vector3.up) * rotation;
                    view.rotation = rotation;
                    view.pivot = vector + ((Vector3) ((rotation * Vector3.forward) * view.cameraDistance));
                    break;
                }
                default:
                    Debug.Log("Enum value Tools.s_LockViewTool not handled");
                    goto Label_02E4;
            }
            view.svRot.UpdateGizmoLabel(view, (Vector3) (view.rotation * Vector3.forward), view.m_Ortho.target);
        Label_02E4:
            current.Use();
        }

        private static void HandleMouseUp(SceneView view, int id, int button, int clickCount)
        {
            if (GUIUtility.hotControl == id)
            {
                RaycastHit hit;
                ResetDragState();
                if (((button == 2) && !s_Dragged) && RaycastWorld(Event.current.mousePosition, out hit))
                {
                    Vector3 vector = view.pivot - ((Vector3) ((view.rotation * Vector3.forward) * view.cameraDistance));
                    float size = view.size;
                    if (!view.orthographic)
                    {
                        size = (view.size * Vector3.Dot(hit.point - vector, (Vector3) (view.rotation * Vector3.forward))) / view.cameraDistance;
                    }
                    view.LookAt(hit.point, view.rotation, size);
                }
                Event.current.Use();
            }
        }

        private static void HandleScrollWheel(SceneView view, bool zoomTowardsCenter)
        {
            float cameraDistance = view.cameraDistance;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 vector = ray.origin + ((Vector3) (ray.direction * view.cameraDistance));
            Vector3 vector2 = vector - view.pivot;
            float y = Event.current.delta.y;
            if (!view.orthographic)
            {
                float num3 = (Mathf.Abs(view.size) * y) * 0.015f;
                if ((num3 > 0f) && (num3 < 0.3f))
                {
                    num3 = 0.3f;
                }
                else if ((num3 < 0f) && (num3 > -0.3f))
                {
                    num3 = -0.3f;
                }
                view.size += num3;
            }
            else
            {
                view.size = Mathf.Abs(view.size) * ((y * 0.015f) + 1f);
            }
            float num5 = 1f - (view.cameraDistance / cameraDistance);
            if (!zoomTowardsCenter)
            {
                view.pivot += (Vector3) (vector2 * num5);
            }
            Event.current.Use();
        }

        private static void OrbitCameraBehavior(SceneView view)
        {
            Event current = Event.current;
            view.FixNegativeSize();
            Quaternion target = view.m_Rotation.target;
            target = Quaternion.AngleAxis((current.delta.y * 0.003f) * 57.29578f, (Vector3) (target * Vector3.right)) * target;
            target = Quaternion.AngleAxis((current.delta.x * 0.003f) * 57.29578f, Vector3.up) * target;
            if (view.size < 0f)
            {
                view.pivot = view.camera.transform.position;
                view.size = 0f;
            }
            view.rotation = target;
        }

        private static bool RaycastWorld(Vector2 position, out RaycastHit hit)
        {
            hit = new RaycastHit();
            GameObject obj2 = HandleUtility.PickGameObject(position, false);
            if (obj2 == null)
            {
                return false;
            }
            Ray ray = HandleUtility.GUIPointToWorldRay(position);
            MeshFilter[] componentsInChildren = obj2.GetComponentsInChildren<MeshFilter>();
            float positiveInfinity = float.PositiveInfinity;
            foreach (MeshFilter filter in componentsInChildren)
            {
                RaycastHit hit2;
                Mesh sharedMesh = filter.sharedMesh;
                if ((sharedMesh != null) && (HandleUtility.IntersectRayMesh(ray, sharedMesh, filter.transform.localToWorldMatrix, out hit2) && (hit2.distance < positiveInfinity)))
                {
                    hit = hit2;
                    positiveInfinity = hit.distance;
                }
            }
            if (positiveInfinity == float.PositiveInfinity)
            {
                Vector3 introduced9 = Vector3.Project(obj2.transform.position - ray.origin, ray.direction);
                hit.point = introduced9 + ray.origin;
            }
            return true;
        }

        private static void ResetDragState()
        {
            GUIUtility.hotControl = 0;
            Tools.s_LockedViewTool = ViewTool.None;
            Tools.s_ButtonDown = -1;
            s_Motion = Vector3.zero;
            if (Toolbar.get != null)
            {
                Toolbar.get.Repaint();
            }
            EditorGUIUtility.SetWantsMouseJumping(0);
        }

        public static void ResetMotion()
        {
            s_Motion = Vector3.zero;
        }
    }
}

