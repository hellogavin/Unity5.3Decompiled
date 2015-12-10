namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class Utility2D
    {
        public static Vector3 ScreenToLocal(Transform transform, Vector2 screenPosition)
        {
            Ray ray;
            float num;
            Plane plane = new Plane((Vector3) (transform.forward * -1f), transform.position);
            if (Camera.current.orthographic)
            {
                Vector2 vector = GUIClip.Unclip(screenPosition);
                vector.y = Screen.height - vector.y;
                Vector3 origin = Camera.current.ScreenToWorldPoint((Vector3) vector);
                ray = new Ray(origin, Camera.current.transform.forward);
            }
            else
            {
                ray = HandleUtility.GUIPointToWorldRay(screenPosition);
            }
            plane.Raycast(ray, out num);
            Vector3 point = ray.GetPoint(num);
            return transform.InverseTransformPoint(point);
        }
    }
}

