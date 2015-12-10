namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(SphereCollider)), CanEditMultipleObjects]
    internal class SphereColliderEditor : Collider3DEditorBase
    {
        private SerializedProperty m_Center;
        private int m_HandleControlID;
        private SerializedProperty m_Radius;

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
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
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            bool flag = GUIUtility.hotControl == this.m_HandleControlID;
            SphereCollider target = (SphereCollider) this.target;
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
                Handles.color = new Color(0f, 0f, 0f, 0.001f);
            }
            Vector3 lossyScale = target.transform.lossyScale;
            float a = Mathf.Abs(lossyScale.x);
            float introduced12 = Mathf.Max(a, Mathf.Abs(lossyScale.y));
            float num = Mathf.Max(introduced12, Mathf.Abs(lossyScale.z));
            float f = num * target.radius;
            f = Mathf.Max(Mathf.Abs(f), 1E-05f);
            Vector3 position = target.transform.TransformPoint(target.center);
            Quaternion rotation = target.transform.rotation;
            int hotControl = GUIUtility.hotControl;
            float num4 = Handles.RadiusHandle(rotation, position, f, true);
            if (GUI.changed)
            {
                Undo.RecordObject(target, "Adjust Radius");
                target.radius = (num4 * 1f) / num;
            }
            if ((hotControl != GUIUtility.hotControl) && (GUIUtility.hotControl != 0))
            {
                this.m_HandleControlID = GUIUtility.hotControl;
            }
            Handles.color = color;
            GUI.enabled = enabled;
        }
    }
}

