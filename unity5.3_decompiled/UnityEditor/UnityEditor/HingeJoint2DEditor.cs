namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(HingeJoint2D)), CanEditMultipleObjects]
    internal class HingeJoint2DEditor : AnchoredJoint2DEditor
    {
        private void DrawTick(Vector3 center, float radius, float angle, Vector3 up, float length)
        {
            Vector3 normalized = (Vector3) Joint2DEditor.RotateVector2(up, angle).normalized;
            Vector3 vector2 = center + ((Vector3) (normalized * radius));
            Vector3 vector5 = center - vector2;
            Vector3 vector3 = vector2 + ((Vector3) ((vector5.normalized * radius) * length));
            Color[] colors = new Color[] { new Color(0f, 1f, 0f, 0.7f), new Color(0f, 1f, 0f, 0f) };
            Vector3[] points = new Vector3[] { vector2, vector3 };
            Handles.DrawAAPolyLine(colors, points);
        }

        public void OnSceneGUI()
        {
            HingeJoint2D target = (HingeJoint2D) this.target;
            if (target.enabled)
            {
                if (target.useLimits)
                {
                    Vector3 position = Joint2DEditor.TransformPoint(target.transform, (Vector3) target.anchor);
                    float num = Mathf.Min(target.limits.min, target.limits.max);
                    float num2 = Mathf.Max(target.limits.min, target.limits.max);
                    float angle = num2 - num;
                    float radius = HandleUtility.GetHandleSize(position) * 0.8f;
                    float rotation = target.GetComponent<Rigidbody2D>().rotation;
                    Vector3 from = (Vector3) Joint2DEditor.RotateVector2(Vector3.right, -num2 - rotation);
                    Vector3 end = position + ((Vector3) (Joint2DEditor.RotateVector2(Vector3.right, -target.jointAngle - rotation) * radius));
                    Handles.color = new Color(0f, 1f, 0f, 0.7f);
                    Joint2DEditor.DrawAALine(position, end);
                    Handles.color = new Color(0f, 1f, 0f, 0.03f);
                    Handles.DrawSolidArc(position, Vector3.back, from, angle, radius);
                    Handles.color = new Color(0f, 1f, 0f, 0.7f);
                    Handles.DrawWireArc(position, Vector3.back, from, angle, radius);
                    this.DrawTick(position, radius, 0f, from, 1f);
                    this.DrawTick(position, radius, angle, from, 1f);
                }
                base.OnSceneGUI();
            }
        }
    }
}

