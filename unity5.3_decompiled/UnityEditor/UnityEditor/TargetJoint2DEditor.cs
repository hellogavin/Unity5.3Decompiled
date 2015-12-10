namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(TargetJoint2D))]
    internal class TargetJoint2DEditor : Joint2DEditor
    {
        public void OnSceneGUI()
        {
            TargetJoint2D target = (TargetJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 vector = Joint2DEditor.TransformPoint(target.transform, (Vector3) target.anchor);
                Vector3 vector2 = (Vector3) target.target;
                Handles.color = Color.green;
                Handles.DrawDottedLine(vector, vector2, 5f);
                if (base.HandleAnchor(ref vector, false))
                {
                    Undo.RecordObject(target, "Move Anchor");
                    target.anchor = Joint2DEditor.InverseTransformPoint(target.transform, vector);
                }
                float num = HandleUtility.GetHandleSize(vector2) * 0.3f;
                Vector3 vector3 = (Vector3) (Vector3.left * num);
                Vector3 vector4 = (Vector3) (Vector3.up * num);
                Joint2DEditor.DrawAALine(vector2 - vector3, vector2 + vector3);
                Joint2DEditor.DrawAALine(vector2 - vector4, vector2 + vector4);
                if (base.HandleAnchor(ref vector2, true))
                {
                    Undo.RecordObject(target, "Move Target");
                    target.target = vector2;
                }
            }
        }
    }
}

