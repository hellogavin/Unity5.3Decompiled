namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(CircleCollider2D)), CanEditMultipleObjects]
    internal class CircleCollider2DEditor : Collider2DEditorBase
    {
        private int m_HandleControlID;

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_HandleControlID = -1;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            base.OnInspectorGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (!Tools.viewToolActive)
            {
                bool flag = GUIUtility.hotControl == this.m_HandleControlID;
                CircleCollider2D target = (CircleCollider2D) this.target;
                Color color = Handles.color;
                Handles.color = Handles.s_ColliderHandleColor;
                bool enabled = GUI.enabled;
                if (!base.editingCollider && !flag)
                {
                    GUI.enabled = false;
                    Handles.color = new Color(0f, 0f, 0f, 0.001f);
                }
                Vector3 lossyScale = target.transform.lossyScale;
                float a = Mathf.Abs(lossyScale.x);
                float introduced11 = Mathf.Max(a, Mathf.Abs(lossyScale.y));
                float num = Mathf.Max(introduced11, Mathf.Abs(lossyScale.z));
                float f = num * target.radius;
                f = Mathf.Max(Mathf.Abs(f), 1E-05f);
                Vector3 position = target.transform.TransformPoint((Vector3) target.offset);
                int hotControl = GUIUtility.hotControl;
                float num4 = Handles.RadiusHandle(Quaternion.identity, position, f, true);
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
}

