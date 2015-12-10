namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class Disc
    {
        private static Vector2 s_CurrentMousePosition;
        private static float s_RotationDist;
        private static Vector3 s_StartAxis;
        private static Vector2 s_StartMousePosition;
        private static Vector3 s_StartPosition;
        private static Quaternion s_StartRotation;

        public static Quaternion Do(int id, Quaternion rotation, Vector3 position, Vector3 axis, float size, bool cutoffPlane, float snap)
        {
            float num;
            if (Mathf.Abs(Vector3.Dot(Camera.current.transform.forward, axis)) > 0.999f)
            {
                cutoffPlane = false;
            }
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2)))
                    {
                        int num3 = id;
                        GUIUtility.keyboardControl = num3;
                        GUIUtility.hotControl = num3;
                        Tools.LockHandlePosition();
                        if (!cutoffPlane)
                        {
                            s_StartPosition = HandleUtility.ClosestPointToDisc(position, axis, size);
                        }
                        else
                        {
                            Vector3 normalized = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
                            s_StartPosition = HandleUtility.ClosestPointToArc(position, axis, normalized, 180f, size);
                        }
                        s_RotationDist = 0f;
                        s_StartRotation = rotation;
                        s_StartAxis = axis;
                        s_CurrentMousePosition = s_StartMousePosition = Event.current.mousePosition;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return rotation;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        Tools.UnlockHandlePosition();
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return rotation;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return rotation;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        if (!(EditorGUI.actionKey && current.shift))
                        {
                            Vector3 constraintDir = Vector3.Cross(axis, position - s_StartPosition).normalized;
                            s_CurrentMousePosition += current.delta;
                            s_RotationDist = (HandleUtility.CalcLineTranslation(s_StartMousePosition, s_CurrentMousePosition, s_StartPosition, constraintDir) / size) * 30f;
                            s_RotationDist = Handles.SnapValue(s_RotationDist, snap);
                            rotation = Quaternion.AngleAxis(s_RotationDist * -1f, s_StartAxis) * s_StartRotation;
                        }
                        else
                        {
                            if (HandleUtility.ignoreRaySnapObjects == null)
                            {
                                Handles.SetupIgnoreRaySnapObjects();
                            }
                            object obj2 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                            if ((obj2 != null) && (Vector3.Dot(axis.normalized, (Vector3) (rotation * Vector3.forward)) < 0.999))
                            {
                                RaycastHit hit = (RaycastHit) obj2;
                                Vector3 lhs = hit.point - position;
                                Vector3 forward = lhs - ((Vector3) (Vector3.Dot(lhs, axis.normalized) * axis.normalized));
                                rotation = Quaternion.LookRotation(forward, (Vector3) (rotation * Vector3.up));
                            }
                        }
                        GUI.changed = true;
                        current.Use();
                    }
                    return rotation;

                case EventType.KeyDown:
                    if ((current.keyCode == KeyCode.Escape) && (GUIUtility.hotControl == id))
                    {
                        Tools.UnlockHandlePosition();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return rotation;

                case EventType.Repaint:
                {
                    Color white = Color.white;
                    if (id == GUIUtility.keyboardControl)
                    {
                        white = Handles.color;
                        Handles.color = Handles.selectedColor;
                    }
                    if (GUIUtility.hotControl == id)
                    {
                        Color color = Handles.color;
                        Vector3 vector12 = s_StartPosition - position;
                        Vector3 from = vector12.normalized;
                        Handles.color = Handles.secondaryColor;
                        Handles.DrawLine(position, position + ((Vector3) ((from * size) * 1.1f)));
                        float angle = Mathf.Repeat(-s_RotationDist - 180f, 360f) - 180f;
                        Vector3 vector7 = (Vector3) (Quaternion.AngleAxis(angle, axis) * from);
                        Handles.DrawLine(position, position + ((Vector3) ((vector7 * size) * 1.1f)));
                        Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 0.2f);
                        Handles.DrawSolidArc(position, axis, from, angle, size);
                        Handles.color = color;
                    }
                    if (cutoffPlane)
                    {
                        Vector3 vector8 = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
                        Handles.DrawWireArc(position, axis, vector8, 180f, size);
                    }
                    else
                    {
                        Handles.DrawWireDisc(position, axis, size);
                    }
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return rotation;
                }
                case EventType.Layout:
                {
                    if (!cutoffPlane)
                    {
                        num = HandleUtility.DistanceToDisc(position, axis, size) / 2f;
                        break;
                    }
                    Vector3 vector = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
                    num = HandleUtility.DistanceToArc(position, axis, vector, 180f, size) / 2f;
                    break;
                }
                default:
                    return rotation;
            }
            HandleUtility.AddControl(id, num);
            return rotation;
        }
    }
}

