namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(BuoyancyEffector2D), true)]
    internal class BuoyancyEffector2DEditor : Effector2DEditor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache0;

        public void OnSceneGUI()
        {
            BuoyancyEffector2D target = (BuoyancyEffector2D) this.target;
            if (target.enabled)
            {
                float y = target.transform.position.y + (target.transform.lossyScale.y * target.surfaceLevel);
                List<Vector3> list = new List<Vector3>();
                float negativeInfinity = float.NegativeInfinity;
                float x = negativeInfinity;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = c => c.enabled && c.usedByEffector;
                }
                IEnumerator<Collider2D> enumerator = target.gameObject.GetComponents<Collider2D>().Where<Collider2D>(<>f__am$cache0).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Collider2D current = enumerator.Current;
                        Bounds bounds = current.bounds;
                        float num4 = bounds.min.x;
                        float num5 = bounds.max.x;
                        if (float.IsNegativeInfinity(negativeInfinity))
                        {
                            negativeInfinity = num4;
                            x = num5;
                        }
                        else
                        {
                            if (num4 < negativeInfinity)
                            {
                                negativeInfinity = num4;
                            }
                            if (num5 > x)
                            {
                                x = num5;
                            }
                        }
                        Vector3 item = new Vector3(num4, y, 0f);
                        Vector3 vector3 = new Vector3(num5, y, 0f);
                        list.Add(item);
                        list.Add(vector3);
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                Handles.color = Color.red;
                Vector3[] points = new Vector3[] { new Vector3(negativeInfinity, y, 0f), new Vector3(x, y, 0f) };
                Handles.DrawAAPolyLine(points);
                Handles.color = Color.cyan;
                for (int i = 0; i < (list.Count - 1); i += 2)
                {
                    Vector3[] vectorArray2 = new Vector3[] { list[i], list[i + 1] };
                    Handles.DrawAAPolyLine(vectorArray2);
                }
            }
        }
    }
}

