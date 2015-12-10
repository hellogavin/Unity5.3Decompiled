namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class FreeRotate
    {
        private static Vector2 s_CurrentMousePosition;

        public static Quaternion Do(int id, Quaternion rotation, Vector3 position, float size)
        {
            Vector3 center = Handles.matrix.MultiplyPoint(position);
            Matrix4x4 matrix = Handles.matrix;
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2)))
                    {
                        int num = id;
                        GUIUtility.keyboardControl = num;
                        GUIUtility.hotControl = num;
                        Tools.LockHandlePosition();
                        s_CurrentMousePosition = current.mousePosition;
                        HandleUtility.ignoreRaySnapObjects = null;
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
                {
                    if (GUIUtility.hotControl != id)
                    {
                        return rotation;
                    }
                    if (!(EditorGUI.actionKey && current.shift))
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector3 vector2 = Camera.current.transform.TransformDirection(new Vector3(-current.delta.y, -current.delta.x, 0f));
                        rotation = Quaternion.AngleAxis(current.delta.magnitude, vector2.normalized) * rotation;
                        break;
                    }
                    if (HandleUtility.ignoreRaySnapObjects == null)
                    {
                        Handles.SetupIgnoreRaySnapObjects();
                    }
                    object obj2 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                    if (obj2 != null)
                    {
                        RaycastHit hit = (RaycastHit) obj2;
                        Quaternion quaternion = Quaternion.LookRotation(hit.point - position);
                        if (Tools.pivotRotation == PivotRotation.Global)
                        {
                            Transform activeTransform = Selection.activeTransform;
                            if (activeTransform != null)
                            {
                                Quaternion quaternion2 = Quaternion.Inverse(activeTransform.rotation) * rotation;
                                quaternion *= quaternion2;
                            }
                        }
                        rotation = quaternion;
                    }
                    break;
                }
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
                    Handles.matrix = Matrix4x4.identity;
                    Handles.DrawWireDisc(center, Camera.current.transform.forward, size);
                    Handles.matrix = matrix;
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return rotation;
                }
                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(center, size) + 5f);
                    Handles.matrix = matrix;
                    return rotation;

                default:
                    return rotation;
            }
            GUI.changed = true;
            current.Use();
            return rotation;
        }
    }
}

