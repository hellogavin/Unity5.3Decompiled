namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(SpringJoint2D)), CanEditMultipleObjects]
    internal class SpringJoint2DEditor : AnchoredJoint2DEditor
    {
        public void OnSceneGUI()
        {
            SpringJoint2D target = (SpringJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 anchor = Joint2DEditor.TransformPoint(target.transform, (Vector3) target.anchor);
                Vector3 connectedAnchor = (Vector3) target.connectedAnchor;
                if (target.connectedBody != null)
                {
                    connectedAnchor = Joint2DEditor.TransformPoint(target.connectedBody.transform, connectedAnchor);
                }
                Joint2DEditor.DrawDistanceGizmo(anchor, connectedAnchor, target.distance);
                base.OnSceneGUI();
            }
        }
    }
}

