namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SnapGuide
    {
        public List<Vector3> lineVertices;
        public bool safe;
        public float value;

        public SnapGuide(float value, params Vector3[] vertices) : this(value, true, vertices)
        {
        }

        public SnapGuide(float value, bool safe, params Vector3[] vertices)
        {
            this.lineVertices = new List<Vector3>();
            this.safe = true;
            this.value = value;
            this.lineVertices.AddRange(vertices);
            this.safe = safe;
        }

        public void Draw()
        {
            Handles.color = !this.safe ? new Color(1f, 0.5f, 0f, 1f) : new Color(0f, 0.5f, 1f, 1f);
            for (int i = 0; i < this.lineVertices.Count; i += 2)
            {
                Vector3 position = this.lineVertices[i];
                Vector3 vector2 = this.lineVertices[i + 1];
                if (position != vector2)
                {
                    Vector3 vector4 = vector2 - position;
                    Vector3 vector3 = (Vector3) (vector4.normalized * 0.05f);
                    position -= (Vector3) (vector3 * HandleUtility.GetHandleSize(position));
                    vector2 += (Vector3) (vector3 * HandleUtility.GetHandleSize(vector2));
                    Handles.DrawLine(position, vector2);
                }
            }
        }
    }
}

