namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class RectTransformSnapping
    {
        private static float[] kSidesAndMiddle;
        internal const float kSnapThreshold = 0.05f;
        private static Vector3[] s_Corners;
        private static SnapGuideCollection[] s_SnapGuides = new SnapGuideCollection[] { new SnapGuideCollection(), new SnapGuideCollection() };

        static RectTransformSnapping()
        {
            float[] singleArray1 = new float[3];
            singleArray1[1] = 0.5f;
            singleArray1[2] = 1f;
            kSidesAndMiddle = singleArray1;
            s_Corners = new Vector3[4];
        }

        internal static void CalculateAnchorSnapValues(Transform parentSpace, Transform self, RectTransform gui, int minmaxX, int minmaxY)
        {
            for (int i = 0; i < 2; i++)
            {
                s_SnapGuides[i].Clear();
                parentSpace.GetComponent<RectTransform>().GetWorldCorners(s_Corners);
                for (int j = 0; j < kSidesAndMiddle.Length; j++)
                {
                    float alongMainAxis = kSidesAndMiddle[j];
                    Vector3[] vertices = new Vector3[] { GetInterpolatedCorner(s_Corners, i, alongMainAxis, 0f), GetInterpolatedCorner(s_Corners, i, alongMainAxis, 1f) };
                    s_SnapGuides[i].AddGuide(new SnapGuide(alongMainAxis, vertices));
                }
                IEnumerator enumerator = parentSpace.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        if (current != self)
                        {
                            RectTransform component = current.GetComponent<RectTransform>();
                            if (component != null)
                            {
                                s_SnapGuides[i].AddGuide(new SnapGuide(component.anchorMin[i], new Vector3[0]));
                                s_SnapGuides[i].AddGuide(new SnapGuide(component.anchorMax[i], new Vector3[0]));
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                switch (((i != 0) ? minmaxY : minmaxX))
                {
                    case 0:
                        s_SnapGuides[i].AddGuide(new SnapGuide(gui.anchorMax[i], new Vector3[0]));
                        break;

                    case 1:
                        s_SnapGuides[i].AddGuide(new SnapGuide(gui.anchorMin[i], new Vector3[0]));
                        break;
                }
            }
        }

        internal static void CalculateOffsetSnapValues(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect, int xHandle, int yHandle)
        {
            for (int i = 0; i < 2; i++)
            {
                s_SnapGuides[i].Clear();
            }
            if (parentSpace != null)
            {
                for (int j = 0; j < 2; j++)
                {
                    int side = (j != 0) ? yHandle : xHandle;
                    if (side != 1)
                    {
                        foreach (SnapGuide guide in GetSnapGuides(parentSpace, self, parentRect, rect, j, side))
                        {
                            s_SnapGuides[j].AddGuide(guide);
                        }
                    }
                }
            }
        }

        internal static void CalculatePivotSnapValues(Rect rect, Vector3 pivot, Quaternion rotation)
        {
            for (int i = 0; i < 2; i++)
            {
                s_SnapGuides[i].Clear();
                for (int j = 0; j < kSidesAndMiddle.Length; j++)
                {
                    s_SnapGuides[i].AddGuide(new SnapGuide(kSidesAndMiddle[j], GetGuideLineForRect(rect, pivot, rotation, i, kSidesAndMiddle[j])));
                }
            }
        }

        internal static void CalculatePositionSnapValues(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect)
        {
            for (int i = 0; i < 2; i++)
            {
                s_SnapGuides[i].Clear();
            }
            if (parentSpace != null)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < kSidesAndMiddle.Length; k++)
                    {
                        foreach (SnapGuide guide in GetSnapGuides(parentSpace, self, parentRect, rect, j, k))
                        {
                            guide.value = GetGuideValueForRect(rect, guide.value, j, kSidesAndMiddle[k]);
                            s_SnapGuides[j].AddGuide(guide);
                        }
                    }
                }
            }
        }

        internal static void DrawGuides()
        {
            if (!EditorGUI.actionKey)
            {
                s_SnapGuides[0].DrawGuides();
                s_SnapGuides[1].DrawGuides();
            }
        }

        private static Vector3[] GetGuideLineForRect(RectTransform rect, int axis, float side)
        {
            Vector3[] vectorArray = new Vector3[2];
            vectorArray[0][1 - axis] = rect.rect.min[1 - axis];
            vectorArray[1][1 - axis] = rect.rect.max[1 - axis];
            vectorArray[0][axis] = Mathf.Lerp(rect.rect.min[axis], rect.rect.max[axis], side);
            vectorArray[1][axis] = vectorArray[0][axis];
            vectorArray[0] = rect.transform.TransformPoint(vectorArray[0]);
            vectorArray[1] = rect.transform.TransformPoint(vectorArray[1]);
            return vectorArray;
        }

        private static Vector3[] GetGuideLineForRect(Rect rect, Vector3 pivot, Quaternion rotation, int axis, float side)
        {
            Vector3[] vectorArray = new Vector3[2];
            vectorArray[0][1 - axis] = rect.min[1 - axis];
            vectorArray[1][1 - axis] = rect.max[1 - axis];
            vectorArray[0][axis] = Mathf.Lerp(rect.min[axis], rect.max[axis], side);
            vectorArray[1][axis] = vectorArray[0][axis];
            vectorArray[0] = ((Vector3) (rotation * vectorArray[0])) + pivot;
            vectorArray[1] = ((Vector3) (rotation * vectorArray[1])) + pivot;
            return vectorArray;
        }

        private static float GetGuideValueForRect(RectTransform rect, float value, int axis, float side)
        {
            RectTransform component = rect.transform.parent.GetComponent<RectTransform>();
            float num = (component == null) ? 0f : component.rect.size[axis];
            float num2 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], rect.pivot[axis]) * num;
            float num3 = rect.rect.size[axis] * (rect.pivot[axis] - side);
            return ((value - num2) + num3);
        }

        private static Vector3 GetInterpolatedCorner(Vector3[] corners, int mainAxis, float alongMainAxis, float alongOtherAxis)
        {
            if (mainAxis != 0)
            {
                float num = alongMainAxis;
                alongMainAxis = alongOtherAxis;
                alongOtherAxis = num;
            }
            return (Vector3) (((((corners[0] * (1f - alongMainAxis)) * (1f - alongOtherAxis)) + ((corners[1] * (1f - alongMainAxis)) * alongOtherAxis)) + ((corners[3] * alongMainAxis) * (1f - alongOtherAxis))) + ((corners[2] * alongMainAxis) * alongOtherAxis));
        }

        private static List<SnapGuide> GetSnapGuides(Transform parentSpace, Transform self, RectTransform parentRect, RectTransform rect, int axis, int side)
        {
            List<SnapGuide> list = new List<SnapGuide>();
            if (parentRect != null)
            {
                float t = kSidesAndMiddle[side];
                float num2 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], t);
                list.Add(new SnapGuide(num2 * parentRect.rect.size[axis], GetGuideLineForRect(parentRect, axis, num2)));
                float num3 = Mathf.Lerp(rect.anchorMin[axis], rect.anchorMax[axis], t);
                if (t != num3)
                {
                    list.Add(new SnapGuide(t * parentRect.rect.size[axis], false, GetGuideLineForRect(parentRect, axis, t)));
                }
            }
            IEnumerator enumerator = parentSpace.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current != self)
                    {
                        RectTransform component = current.GetComponent<RectTransform>();
                        bool safe = true;
                        if (component != null)
                        {
                            if (side == 0)
                            {
                                safe = component.anchorMin[axis] == rect.anchorMin[axis];
                                list.Add(new SnapGuide(component.GetRectInParentSpace().min[axis], safe, GetGuideLineForRect(component, axis, 0f)));
                                safe = component.anchorMax[axis] == rect.anchorMin[axis];
                                list.Add(new SnapGuide(component.GetRectInParentSpace().max[axis], safe, GetGuideLineForRect(component, axis, 1f)));
                            }
                            if (side == 2)
                            {
                                safe = component.anchorMax[axis] == rect.anchorMax[axis];
                                list.Add(new SnapGuide(component.GetRectInParentSpace().max[axis], safe, GetGuideLineForRect(component, axis, 1f)));
                                safe = component.anchorMin[axis] == rect.anchorMax[axis];
                                list.Add(new SnapGuide(component.GetRectInParentSpace().min[axis], safe, GetGuideLineForRect(component, axis, 0f)));
                            }
                            if (side == 1)
                            {
                                safe = (component.anchorMin[axis] - rect.anchorMin[axis]) == -(component.anchorMax[axis] - rect.anchorMax[axis]);
                                list.Add(new SnapGuide(component.GetRectInParentSpace().center[axis], safe, GetGuideLineForRect(component, axis, 0.5f)));
                            }
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return list;
        }

        internal static void OnGUI()
        {
            s_SnapGuides[0].OnGUI();
            s_SnapGuides[1].OnGUI();
        }

        internal static Vector2 SnapToGuides(Vector2 value, Vector2 snapDistance)
        {
            return new Vector2(SnapToGuides(value.x, snapDistance.x, 0), SnapToGuides(value.y, snapDistance.y, 1));
        }

        internal static float SnapToGuides(float value, float snapDistance, int axis)
        {
            if (EditorGUI.actionKey)
            {
                return value;
            }
            SnapGuideCollection guides = (axis != 0) ? s_SnapGuides[1] : s_SnapGuides[0];
            return guides.SnapToGuides(value, snapDistance);
        }
    }
}

