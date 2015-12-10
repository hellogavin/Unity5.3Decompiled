namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(CharacterController))]
    internal class CharacterControllerEditor : Editor
    {
        private SerializedProperty m_Center;
        private SerializedProperty m_Direction;
        private int m_HandleControlID;
        private SerializedProperty m_Height;
        private SerializedProperty m_MinMoveDistance;
        private SerializedProperty m_Radius;
        private SerializedProperty m_SkinWidth;
        private SerializedProperty m_SlopeLimit;
        private SerializedProperty m_StepOffset;

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            this.m_Height = base.serializedObject.FindProperty("m_Height");
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
            this.m_SlopeLimit = base.serializedObject.FindProperty("m_SlopeLimit");
            this.m_StepOffset = base.serializedObject.FindProperty("m_StepOffset");
            this.m_SkinWidth = base.serializedObject.FindProperty("m_SkinWidth");
            this.m_MinMoveDistance = base.serializedObject.FindProperty("m_MinMoveDistance");
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_HandleControlID = -1;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_SlopeLimit, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_StepOffset, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SkinWidth, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MinMoveDistance, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            bool flag = GUIUtility.hotControl == this.m_HandleControlID;
            CharacterController target = (CharacterController) this.target;
            Color color = Handles.color;
            if (target.enabled)
            {
                Handles.color = Handles.s_ColliderHandleColor;
            }
            else
            {
                Handles.color = Handles.s_ColliderHandleColorDisabled;
            }
            bool enabled = GUI.enabled;
            if (!Event.current.shift && !flag)
            {
                GUI.enabled = false;
                Handles.color = new Color(1f, 0f, 0f, 0.001f);
            }
            float a = target.height * target.transform.lossyScale.y;
            float num2 = target.radius * Mathf.Max(target.transform.lossyScale.x, target.transform.lossyScale.z);
            a = Mathf.Max(a, num2 * 2f);
            Matrix4x4 matrix = Matrix4x4.TRS(target.transform.TransformPoint(target.center), Quaternion.identity, Vector3.one);
            int hotControl = GUIUtility.hotControl;
            Vector3 localPos = (Vector3) ((Vector3.up * a) * 0.5f);
            float num4 = SizeHandle(localPos, Vector3.up, matrix, true);
            if (!GUI.changed)
            {
                num4 = SizeHandle(-localPos, Vector3.down, matrix, true);
            }
            if (GUI.changed)
            {
                Undo.RecordObject(target, "Character Controller Resize");
                float num5 = a / target.height;
                target.height += num4 / num5;
            }
            num4 = SizeHandle((Vector3) (Vector3.left * num2), Vector3.left, matrix, true);
            if (!GUI.changed)
            {
                num4 = SizeHandle((Vector3) (-Vector3.left * num2), -Vector3.left, matrix, true);
            }
            if (!GUI.changed)
            {
                num4 = SizeHandle((Vector3) (Vector3.forward * num2), Vector3.forward, matrix, true);
            }
            if (!GUI.changed)
            {
                num4 = SizeHandle((Vector3) (-Vector3.forward * num2), -Vector3.forward, matrix, true);
            }
            if (GUI.changed)
            {
                Undo.RecordObject(target, "Character Controller Resize");
                float num6 = num2 / target.radius;
                target.radius += num4 / num6;
            }
            if ((hotControl != GUIUtility.hotControl) && (GUIUtility.hotControl != 0))
            {
                this.m_HandleControlID = GUIUtility.hotControl;
            }
            if (GUI.changed)
            {
                target.radius = Mathf.Max(target.radius, 1E-05f);
                target.height = Mathf.Max(target.height, 1E-05f);
            }
            Handles.color = color;
            GUI.enabled = enabled;
        }

        private static float SizeHandle(Vector3 localPos, Vector3 localPullDir, Matrix4x4 matrix, bool isEdgeHandle)
        {
            float num3;
            Vector3 rhs = matrix.MultiplyVector(localPullDir);
            Vector3 position = matrix.MultiplyPoint(localPos);
            float handleSize = HandleUtility.GetHandleSize(position);
            bool changed = GUI.changed;
            GUI.changed = false;
            Color color = Handles.color;
            float num2 = 0f;
            if (isEdgeHandle)
            {
                num2 = Mathf.Cos(0.7853982f);
            }
            if (Camera.current.orthographic)
            {
                num3 = Vector3.Dot(-Camera.current.transform.forward, rhs);
            }
            else
            {
                Vector3 vector4 = Camera.current.transform.position - position;
                num3 = Vector3.Dot(vector4.normalized, rhs);
            }
            if (num3 < -num2)
            {
                Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * Handles.backfaceAlphaMultiplier);
            }
            Vector3 point = Handles.Slider(position, rhs, handleSize * 0.03f, new Handles.DrawCapFunction(Handles.DotCap), 0f);
            float num4 = 0f;
            if (GUI.changed)
            {
                num4 = HandleUtility.PointOnLineParameter(point, position, rhs);
            }
            GUI.changed |= changed;
            Handles.color = color;
            return num4;
        }
    }
}

