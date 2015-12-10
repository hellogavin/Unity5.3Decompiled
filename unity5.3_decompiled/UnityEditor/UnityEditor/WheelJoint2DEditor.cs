namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(WheelJoint2D)), CanEditMultipleObjects]
    internal class WheelJoint2DEditor : AnchoredJoint2DEditor
    {
        public void OnSceneGUI()
        {
            WheelJoint2D target = (WheelJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 position = Joint2DEditor.TransformPoint(target.transform, (Vector3) target.anchor);
                Vector3 start = position;
                Vector3 end = position;
                Vector3 vector4 = (Vector3) Joint2DEditor.RotateVector2(Vector3.right, -target.suspension.angle - target.transform.eulerAngles.z);
                Handles.color = Color.green;
                vector4 = (Vector3) (vector4 * (HandleUtility.GetHandleSize(position) * 0.3f));
                start += vector4;
                end -= vector4;
                Joint2DEditor.DrawAALine(start, end);
                base.OnSceneGUI();
            }
        }
    }
}

