namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Joint2D)), CanEditMultipleObjects]
    internal class Joint2DEditor : Editor
    {
        private SerializedProperty m_BreakForce;
        private SerializedProperty m_BreakTorque;
        protected static Styles s_Styles;

        public static void AnchorCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (controlID == GUIUtility.keyboardControl)
            {
                DrawCap(controlID, position, s_Styles.anchorActive);
            }
            else
            {
                DrawCap(controlID, position, s_Styles.anchor);
            }
        }

        public static void ConnectedAnchorCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (controlID == GUIUtility.keyboardControl)
            {
                DrawCap(controlID, position, s_Styles.connectedAnchorActive);
            }
            else
            {
                DrawCap(controlID, position, s_Styles.connectedAnchor);
            }
        }

        public static void DrawAALine(Vector3 start, Vector3 end)
        {
            Vector3[] points = new Vector3[] { start, end };
            Handles.DrawAAPolyLine(points);
        }

        private static void DrawCap(int controlID, Vector3 position, GUIStyle guiStyle)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Handles.BeginGUI();
                position = (Vector3) HandleUtility.WorldToGUIPoint(position);
                float fixedWidth = guiStyle.fixedWidth;
                float fixedHeight = guiStyle.fixedHeight;
                Rect rect = new Rect(position.x - (fixedWidth / 2f), position.y - (fixedHeight / 2f), fixedWidth, fixedHeight);
                guiStyle.Draw(rect, GUIContent.none, controlID);
                Handles.EndGUI();
            }
        }

        public static void DrawDistanceGizmo(Vector3 anchor, Vector3 connectedAnchor, float distance)
        {
            Vector3 vector4 = anchor - connectedAnchor;
            Vector3 normalized = vector4.normalized;
            Vector3 end = connectedAnchor + ((Vector3) (normalized * distance));
            Vector3 vector3 = (Vector3) (Vector3.Cross(normalized, Vector3.forward) * (HandleUtility.GetHandleSize(connectedAnchor) * 0.16f));
            Handles.color = Color.green;
            DrawAALine(anchor, end);
            DrawAALine(connectedAnchor + vector3, connectedAnchor - vector3);
            DrawAALine(end + vector3, end - vector3);
        }

        private static Matrix4x4 GetAnchorSpaceMatrix(Transform transform)
        {
            return Matrix4x4.TRS(transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z), transform.lossyScale);
        }

        protected bool HandleAnchor(ref Vector3 position, bool isConnectedAnchor)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Handles.DrawCapFunction drawFunc = !isConnectedAnchor ? new Handles.DrawCapFunction(Joint2DEditor.AnchorCap) : new Handles.DrawCapFunction(Joint2DEditor.ConnectedAnchorCap);
            int id = this.target.GetInstanceID() + (!isConnectedAnchor ? 0 : 1);
            EditorGUI.BeginChangeCheck();
            position = Handles.Slider2D(id, position, Vector3.back, Vector3.right, Vector3.up, 0f, drawFunc, Vector2.zero);
            return EditorGUI.EndChangeCheck();
        }

        protected static Vector3 InverseTransformPoint(Transform transform, Vector3 position)
        {
            return GetAnchorSpaceMatrix(transform).inverse.MultiplyPoint(position);
        }

        public void OnEnable()
        {
            this.m_BreakForce = base.serializedObject.FindProperty("m_BreakForce");
            this.m_BreakTorque = base.serializedObject.FindProperty("m_BreakTorque");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_BreakForce, new GUILayoutOption[0]);
            Type type = this.target.GetType();
            if (((type != typeof(DistanceJoint2D)) && (type != typeof(TargetJoint2D))) && (type != typeof(SpringJoint2D)))
            {
                EditorGUILayout.PropertyField(this.m_BreakTorque, new GUILayoutOption[0]);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        protected static Vector2 RotateVector2(Vector2 direction, float angle)
        {
            float f = 0.01745329f * -angle;
            float num2 = Mathf.Cos(f);
            float num3 = Mathf.Sin(f);
            float x = (direction.x * num2) - (direction.y * num3);
            return new Vector2(x, (direction.x * num3) + (direction.y * num2));
        }

        protected static Vector3 SnapToPoint(Vector3 position, Vector3 snapPosition, float snapDistance)
        {
            snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
            return ((Vector3.Distance(position, snapPosition) > snapDistance) ? position : snapPosition);
        }

        protected static Vector3 SnapToSprite(SpriteRenderer spriteRenderer, Vector3 position, float snapDistance)
        {
            if (spriteRenderer != null)
            {
                snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
                float x = spriteRenderer.sprite.bounds.size.x / 2f;
                float y = spriteRenderer.sprite.bounds.size.y / 2f;
                Vector2[] vectorArray = new Vector2[] { new Vector2(-x, -y), new Vector2(0f, -y), new Vector2(x, -y), new Vector2(-x, 0f), new Vector2(0f, 0f), new Vector2(x, 0f), new Vector2(-x, y), new Vector2(0f, y), new Vector2(x, y) };
                foreach (Vector2 vector in vectorArray)
                {
                    Vector3 b = spriteRenderer.transform.TransformPoint((Vector3) vector);
                    if (Vector2.Distance(position, b) <= snapDistance)
                    {
                        return b;
                    }
                }
            }
            return position;
        }

        protected static Vector3 TransformPoint(Transform transform, Vector3 position)
        {
            return GetAnchorSpaceMatrix(transform).MultiplyPoint(position);
        }

        public class Styles
        {
            public readonly GUIStyle anchor = "U2D.pivotDot";
            public readonly GUIStyle anchorActive = "U2D.pivotDotActive";
            public readonly GUIStyle connectedAnchor = "U2D.dragDot";
            public readonly GUIStyle connectedAnchorActive = "U2D.dragDotActive";
        }
    }
}

