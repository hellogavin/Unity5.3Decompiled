namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(RelativeJoint2D)), CanEditMultipleObjects]
    internal class RelativeJoint2DEditor : Joint2DEditor
    {
        public void OnSceneGUI()
        {
            RelativeJoint2D target = (RelativeJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 start = (Vector3) target.target;
                Vector3 end = (target.connectedBody == null) ? Vector3.zero : target.connectedBody.transform.position;
                Handles.color = Color.green;
                Joint2DEditor.DrawAALine(start, end);
                float num = HandleUtility.GetHandleSize(end) * 0.16f;
                Vector3 vector3 = (Vector3) (Vector3.left * num);
                Vector3 vector4 = (Vector3) (Vector3.up * num);
                Joint2DEditor.DrawAALine(end - vector3, end + vector3);
                Joint2DEditor.DrawAALine(end - vector4, end + vector4);
                float num2 = HandleUtility.GetHandleSize(start) * 0.16f;
                Vector3 vector5 = (Vector3) (Vector3.left * num2);
                Vector3 vector6 = (Vector3) (Vector3.up * num2);
                Joint2DEditor.DrawAALine(start - vector5, start + vector5);
                Joint2DEditor.DrawAALine(start - vector6, start + vector6);
            }
        }
    }
}

