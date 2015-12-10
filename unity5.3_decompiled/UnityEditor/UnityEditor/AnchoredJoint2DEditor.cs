namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(AnchoredJoint2D), true), CanEditMultipleObjects]
    internal class AnchoredJoint2DEditor : Joint2DEditor
    {
        private AnchoredJoint2D anchorJoint2D;
        private const float k_SnapDistance = 0.13f;

        public void OnSceneGUI()
        {
            this.anchorJoint2D = (AnchoredJoint2D) this.target;
            if (this.anchorJoint2D.enabled)
            {
                Vector3 position = Joint2DEditor.TransformPoint(this.anchorJoint2D.transform, (Vector3) this.anchorJoint2D.anchor);
                Vector3 connectedAnchor = (Vector3) this.anchorJoint2D.connectedAnchor;
                if (this.anchorJoint2D.connectedBody != null)
                {
                    connectedAnchor = Joint2DEditor.TransformPoint(this.anchorJoint2D.connectedBody.transform, connectedAnchor);
                }
                Vector3 vector4 = connectedAnchor - position;
                Vector3 vector3 = position + ((Vector3) ((vector4.normalized * HandleUtility.GetHandleSize(position)) * 0.1f));
                Handles.color = Color.green;
                Vector3[] points = new Vector3[] { vector3, connectedAnchor };
                Handles.DrawAAPolyLine(points);
                if (base.HandleAnchor(ref connectedAnchor, true))
                {
                    connectedAnchor = Joint2DEditor.SnapToPoint(this.SnapToSprites(connectedAnchor), position, 0.13f);
                    if (this.anchorJoint2D.connectedBody != null)
                    {
                        connectedAnchor = Joint2DEditor.InverseTransformPoint(this.anchorJoint2D.connectedBody.transform, connectedAnchor);
                    }
                    Undo.RecordObject(this.anchorJoint2D, "Move Connected Anchor");
                    this.anchorJoint2D.connectedAnchor = connectedAnchor;
                }
                if (base.HandleAnchor(ref position, false))
                {
                    position = Joint2DEditor.SnapToPoint(this.SnapToSprites(position), connectedAnchor, 0.13f);
                    Undo.RecordObject(this.anchorJoint2D, "Move Anchor");
                    this.anchorJoint2D.anchor = Joint2DEditor.InverseTransformPoint(this.anchorJoint2D.transform, position);
                }
            }
        }

        private Vector3 SnapToSprites(Vector3 position)
        {
            position = Joint2DEditor.SnapToSprite(this.anchorJoint2D.GetComponent<SpriteRenderer>(), position, 0.13f);
            if (this.anchorJoint2D.connectedBody != null)
            {
                position = Joint2DEditor.SnapToSprite(this.anchorJoint2D.connectedBody.GetComponent<SpriteRenderer>(), position, 0.13f);
            }
            return position;
        }
    }
}

