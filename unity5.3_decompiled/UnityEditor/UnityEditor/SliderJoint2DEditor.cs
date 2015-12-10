namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SliderJoint2D))]
    internal class SliderJoint2DEditor : AnchoredJoint2DEditor
    {
        public void OnSceneGUI()
        {
            SliderJoint2D target = (SliderJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 position = Joint2DEditor.TransformPoint(target.transform, (Vector3) target.anchor);
                Vector3 vector2 = position;
                Vector3 vector3 = position;
                Vector3 lhs = (Vector3) Joint2DEditor.RotateVector2(Vector3.right, -target.angle - target.transform.eulerAngles.z);
                Handles.color = Color.green;
                if (target.useLimits)
                {
                    vector2 = position + ((Vector3) (lhs * target.limits.max));
                    vector3 = position + ((Vector3) (lhs * target.limits.min));
                    Vector3 vector5 = Vector3.Cross(lhs, Vector3.forward);
                    float num = HandleUtility.GetHandleSize(vector2) * 0.16f;
                    float num2 = HandleUtility.GetHandleSize(vector3) * 0.16f;
                    Joint2DEditor.DrawAALine(vector2 + ((Vector3) (vector5 * num)), vector2 - ((Vector3) (vector5 * num)));
                    Joint2DEditor.DrawAALine(vector3 + ((Vector3) (vector5 * num2)), vector3 - ((Vector3) (vector5 * num2)));
                }
                else
                {
                    lhs = (Vector3) (lhs * (HandleUtility.GetHandleSize(position) * 0.3f));
                    vector2 += lhs;
                    vector3 -= lhs;
                }
                Joint2DEditor.DrawAALine(vector2, vector3);
                base.OnSceneGUI();
            }
        }
    }
}

