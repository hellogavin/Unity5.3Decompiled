namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider))]
    internal class CapsuleColliderEditor : Collider3DEditorBase
    {
        private SerializedProperty m_Center;
        private SerializedProperty m_Direction;
        private int m_HandleControlID;
        private SerializedProperty m_Height;
        private SerializedProperty m_Radius;

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
            this.m_Height = base.serializedObject.FindProperty("m_Height");
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
            this.m_HandleControlID = -1;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            EditorGUILayout.PropertyField(base.m_IsTrigger, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            bool flag = GUIUtility.hotControl == this.m_HandleControlID;
            CapsuleCollider target = (CapsuleCollider) this.target;
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
            if (!base.editingCollider && !flag)
            {
                GUI.enabled = false;
                Handles.color = new Color(1f, 0f, 0f, 0.001f);
            }
            Vector3 capsuleExtents = ColliderUtil.GetCapsuleExtents(target);
            float num = capsuleExtents.y + (2f * capsuleExtents.x);
            float x = capsuleExtents.x;
            Matrix4x4 matrix = ColliderUtil.CalculateCapsuleTransform(target);
            int hotControl = GUIUtility.hotControl;
            float height = target.height;
            Vector3 localPos = (Vector3) ((Vector3.left * num) * 0.5f);
            float num5 = SizeHandle(localPos, Vector3.left, matrix, true);
            if (!GUI.changed)
            {
                num5 = SizeHandle(-localPos, Vector3.right, matrix, true);
            }
            if (GUI.changed)
            {
                float num6 = num / target.height;
                height += num5 / num6;
            }
            float radius = target.radius;
            num5 = SizeHandle((Vector3) (Vector3.forward * x), Vector3.forward, matrix, true);
            if (!GUI.changed)
            {
                num5 = SizeHandle((Vector3) (-Vector3.forward * x), -Vector3.forward, matrix, true);
            }
            if (!GUI.changed)
            {
                num5 = SizeHandle((Vector3) (Vector3.up * x), Vector3.up, matrix, true);
            }
            if (!GUI.changed)
            {
                num5 = SizeHandle((Vector3) (-Vector3.up * x), -Vector3.up, matrix, true);
            }
            if (GUI.changed)
            {
                float num8 = Mathf.Max((float) (capsuleExtents.z / target.radius), (float) (capsuleExtents.x / target.radius));
                radius += num5 / num8;
            }
            if ((hotControl != GUIUtility.hotControl) && (GUIUtility.hotControl != 0))
            {
                this.m_HandleControlID = GUIUtility.hotControl;
            }
            if (GUI.changed)
            {
                Undo.RecordObject(target, "Modify Capsule Collider");
                target.radius = Mathf.Max(radius, 1E-05f);
                target.height = Mathf.Max(height, 1E-05f);
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

