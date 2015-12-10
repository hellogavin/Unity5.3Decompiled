namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CustomEditor(typeof(PlatformEffector2D), true), CanEditMultipleObjects]
    internal class PlatformEffector2DEditor : Effector2DEditor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache1;

        private static void DrawSideArc(PlatformEffector2D effector)
        {
            float f = 0.01745329f * ((effector.sideArc * 0.5f) + effector.transform.eulerAngles.z);
            float angle = Mathf.Clamp(effector.sideArc, 0.5f, 180f);
            float num3 = angle * 0.01745329f;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = collider => collider.enabled && collider.usedByEffector;
            }
            IEnumerator<Collider2D> enumerator = effector.gameObject.GetComponents<Collider2D>().Where<Collider2D>(<>f__am$cache1).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Collider2D current = enumerator.Current;
                    Vector3 center = current.bounds.center;
                    float radius = HandleUtility.GetHandleSize(center) * 0.8f;
                    Vector3 from = new Vector3(-Mathf.Cos(f), -Mathf.Sin(f), 0f);
                    Vector3 vector3 = new Vector3(-Mathf.Cos(f - num3), -Mathf.Sin(f - num3), 0f);
                    Vector3 vector4 = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
                    Vector3 vector5 = new Vector3(Mathf.Cos(f - num3), Mathf.Sin(f - num3), 0f);
                    Handles.color = new Color(0f, 1f, 0.7f, 0.03f);
                    Handles.DrawSolidArc(center, Vector3.back, from, angle, radius);
                    Handles.DrawSolidArc(center, Vector3.back, vector4, angle, radius);
                    Handles.color = new Color(0f, 1f, 0.7f, 0.7f);
                    Handles.DrawWireArc(center, Vector3.back, from, angle, radius);
                    Handles.DrawWireArc(center, Vector3.back, vector4, angle, radius);
                    Handles.DrawDottedLine(center + ((Vector3) (from * radius)), center + ((Vector3) (vector4 * radius)), 5f);
                    Handles.DrawDottedLine(center + ((Vector3) (vector3 * radius)), center + ((Vector3) (vector5 * radius)), 5f);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static void DrawSurfaceArc(PlatformEffector2D effector)
        {
            float f = 0.01745329f * ((effector.surfaceArc * 0.5f) + effector.transform.eulerAngles.z);
            float angle = Mathf.Clamp(effector.surfaceArc, 0.5f, 360f);
            float num3 = angle * 0.01745329f;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = collider => collider.enabled && collider.usedByEffector;
            }
            IEnumerator<Collider2D> enumerator = effector.gameObject.GetComponents<Collider2D>().Where<Collider2D>(<>f__am$cache0).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Collider2D current = enumerator.Current;
                    Vector3 center = current.bounds.center;
                    float handleSize = HandleUtility.GetHandleSize(center);
                    Vector3 from = new Vector3(-Mathf.Sin(f), Mathf.Cos(f), 0f);
                    Vector3 vector3 = new Vector3(-Mathf.Sin(f - num3), Mathf.Cos(f - num3), 0f);
                    Handles.color = new Color(0f, 1f, 1f, 0.03f);
                    Handles.DrawSolidArc(center, Vector3.back, from, angle, handleSize);
                    Handles.color = new Color(0f, 1f, 1f, 0.7f);
                    Handles.DrawWireArc(center, Vector3.back, from, angle, handleSize);
                    Handles.DrawDottedLine(center, center + ((Vector3) (from * handleSize)), 5f);
                    Handles.DrawDottedLine(center, center + ((Vector3) (vector3 * handleSize)), 5f);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        public void OnSceneGUI()
        {
            PlatformEffector2D target = (PlatformEffector2D) this.target;
            if (target.enabled)
            {
                if (target.useOneWay)
                {
                    DrawSurfaceArc(target);
                }
                if (!target.useSideBounce || !target.useSideFriction)
                {
                    DrawSideArc(target);
                }
            }
        }
    }
}

