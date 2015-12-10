namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(OcclusionArea))]
    internal class OcclusionAreaEditor : Editor
    {
        private SerializedProperty m_Center;
        private SerializedObject m_Object;
        private SerializedProperty m_Size;

        private void OnDisable()
        {
            this.m_Object.Dispose();
            this.m_Object = null;
        }

        private void OnEnable()
        {
            this.m_Object = new SerializedObject(this.target);
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_Center = base.serializedObject.FindProperty("m_Center");
        }

        private void OnSceneGUI()
        {
            this.m_Object.Update();
            OcclusionArea target = (OcclusionArea) this.target;
            Color color = Handles.color;
            Handles.color = (Color) (new Color(145f, 244f, 139f, 255f) / 255f);
            Vector3 p = target.transform.TransformPoint(this.m_Center.vector3Value);
            Vector3 a = (Vector3) (this.m_Size.vector3Value * 0.5f);
            Vector3 vector3 = (Vector3) (this.m_Size.vector3Value * 0.5f);
            Vector3 lossyScale = target.transform.lossyScale;
            Vector3 b = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
            a = Vector3.Scale(a, lossyScale);
            vector3 = Vector3.Scale(vector3, lossyScale);
            bool changed = GUI.changed;
            a.x = this.SizeSlider(p, -Vector3.right, a.x);
            a.y = this.SizeSlider(p, -Vector3.up, a.y);
            a.z = this.SizeSlider(p, -Vector3.forward, a.z);
            vector3.x = this.SizeSlider(p, Vector3.right, vector3.x);
            vector3.y = this.SizeSlider(p, Vector3.up, vector3.y);
            vector3.z = this.SizeSlider(p, Vector3.forward, vector3.z);
            if (GUI.changed)
            {
                this.m_Center.vector3Value += Vector3.Scale((Vector3) ((Quaternion.Inverse(target.transform.rotation) * (vector3 - a)) * 0.5f), b);
                a = Vector3.Scale(a, b);
                vector3 = Vector3.Scale(vector3, b);
                this.m_Size.vector3Value = vector3 + a;
                base.serializedObject.ApplyModifiedProperties();
            }
            GUI.changed |= changed;
            Handles.color = color;
        }

        private float SizeSlider(Vector3 p, Vector3 d, float r)
        {
            Vector3 position = p + ((Vector3) (d * r));
            Color color = Handles.color;
            if (Vector3.Dot(position - Camera.current.transform.position, d) >= 0f)
            {
                Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * Handles.backfaceAlphaMultiplier);
            }
            float handleSize = HandleUtility.GetHandleSize(position);
            bool changed = GUI.changed;
            GUI.changed = false;
            position = Handles.Slider(position, d, handleSize * 0.1f, new Handles.DrawCapFunction(Handles.CylinderCap), 0f);
            if (GUI.changed)
            {
                r = Vector3.Dot(position - p, d);
            }
            GUI.changed |= changed;
            Handles.color = color;
            return r;
        }
    }
}

