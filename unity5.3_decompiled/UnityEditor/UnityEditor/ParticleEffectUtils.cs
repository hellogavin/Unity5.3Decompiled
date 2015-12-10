namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ParticleEffectUtils
    {
        private static List<GameObject> s_Planes = new List<GameObject>();

        public static void ClearPlanes()
        {
            if (s_Planes.Count > 0)
            {
                foreach (GameObject obj2 in s_Planes)
                {
                    Object.DestroyImmediate(obj2);
                }
                s_Planes.Clear();
            }
        }

        public static GameObject GetPlane(int index)
        {
            while (s_Planes.Count <= index)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Plane);
                item.hideFlags = HideFlags.HideAndDontSave;
                s_Planes.Add(item);
            }
            return s_Planes[index];
        }
    }
}

