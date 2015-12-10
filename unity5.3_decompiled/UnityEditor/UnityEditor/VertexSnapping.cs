namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class VertexSnapping
    {
        private static Vector3 s_VertexSnappingOffset = Vector3.zero;

        private static void DisableVertexSnapping(int id)
        {
            Tools.vertexDragging = false;
            Tools.handleOffset = Vector3.zero;
            if (GUIUtility.hotControl != id)
            {
                s_VertexSnappingOffset = Vector3.zero;
            }
        }

        private static void EnableVertexSnapping(int id)
        {
            Tools.vertexDragging = true;
            if (GUIUtility.hotControl == id)
            {
                Tools.handleOffset = s_VertexSnappingOffset;
            }
            else
            {
                UpdateVertexSnappingOffset();
                s_VertexSnappingOffset = Tools.handleOffset;
            }
        }

        private static Vector3 FindNearestPivot(Transform[] transforms, Vector2 screenPosition)
        {
            bool flag = false;
            Vector3 zero = Vector3.zero;
            foreach (Transform transform in transforms)
            {
                Vector3 vector2 = ScreenToWorld(screenPosition, transform);
                if (flag)
                {
                    Vector3 vector3 = zero - vector2;
                    Vector3 vector4 = transform.position - vector2;
                    if (vector3.magnitude <= vector4.magnitude)
                    {
                        continue;
                    }
                }
                zero = transform.position;
                flag = true;
            }
            return zero;
        }

        public static void HandleKeyAndMouseMove(int id)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseMove:
                    if (Tools.vertexDragging)
                    {
                        EnableVertexSnapping(id);
                        current.Use();
                    }
                    return;

                case EventType.MouseDrag:
                    return;

                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.V)
                    {
                        if (!Tools.vertexDragging && !current.shift)
                        {
                            EnableVertexSnapping(id);
                        }
                        current.Use();
                    }
                    return;

                case EventType.KeyUp:
                    if (current.keyCode != KeyCode.V)
                    {
                        return;
                    }
                    if (!current.shift)
                    {
                        if (Tools.vertexDragging)
                        {
                            Tools.vertexDragging = false;
                        }
                        break;
                    }
                    Tools.vertexDragging = !Tools.vertexDragging;
                    break;

                default:
                    return;
            }
            if (Tools.vertexDragging)
            {
                EnableVertexSnapping(id);
            }
            else
            {
                DisableVertexSnapping(id);
            }
            current.Use();
        }

        private static Vector3 ScreenToWorld(Vector2 screen, Transform target)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(screen);
            float enter = 0f;
            new Plane(target.forward, target.position).Raycast(ray, out enter);
            return ray.GetPoint(enter);
        }

        private static void UpdateVertexSnappingOffset()
        {
            Vector3 vector;
            Vector3 vector3;
            Event current = Event.current;
            Tools.vertexDragging = true;
            Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab | SelectionMode.Deep);
            HandleUtility.ignoreRaySnapObjects = null;
            Vector3 world = FindNearestPivot(transforms, current.mousePosition);
            bool flag = HandleUtility.FindNearestVertex(current.mousePosition, transforms, out vector);
            Vector2 vector4 = HandleUtility.WorldToGUIPoint(vector) - current.mousePosition;
            float magnitude = vector4.magnitude;
            Vector2 vector5 = HandleUtility.WorldToGUIPoint(world) - current.mousePosition;
            float num2 = vector5.magnitude;
            if (flag && (magnitude < num2))
            {
                vector3 = vector;
            }
            else
            {
                vector3 = world;
            }
            Tools.handleOffset = Vector3.zero;
            Tools.handleOffset = vector3 - Tools.handlePosition;
        }
    }
}

