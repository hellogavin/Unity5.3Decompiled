namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal static class PopupLocationHelper
    {
        private static Rect FitRect(Rect rect, ContainerWindow popupContainerWindow)
        {
            if (popupContainerWindow != null)
            {
                return popupContainerWindow.FitWindowRectToScreen(rect, true, true);
            }
            return ContainerWindow.FitRectToScreen(rect, true, true);
        }

        public static Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow)
        {
            return GetDropDownRect(buttonRect, minSize, maxSize, popupContainerWindow, null);
        }

        public static Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, PopupLocation[] locationPriorityOrder)
        {
            if (locationPriorityOrder == null)
            {
                PopupLocation[] locationArray1 = new PopupLocation[2];
                locationArray1[1] = PopupLocation.Above;
                locationPriorityOrder = locationArray1;
            }
            List<Rect> rects = new List<Rect>();
            PopupLocation[] locationArray = locationPriorityOrder;
            for (int i = 0; i < locationArray.Length; i++)
            {
                Rect rect;
                switch (locationArray[i])
                {
                    case PopupLocation.Below:
                        if (!PopupBelow(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
                        {
                            break;
                        }
                        return rect;

                    case PopupLocation.Above:
                        if (!PopupAbove(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
                        {
                            goto Label_0079;
                        }
                        return rect;

                    case PopupLocation.Left:
                        if (!PopupLeft(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
                        {
                            goto Label_0099;
                        }
                        return rect;

                    case PopupLocation.Right:
                        if (!PopupRight(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
                        {
                            goto Label_00B9;
                        }
                        return rect;

                    default:
                    {
                        continue;
                    }
                }
                rects.Add(rect);
                continue;
            Label_0079:
                rects.Add(rect);
                continue;
            Label_0099:
                rects.Add(rect);
                continue;
            Label_00B9:
                rects.Add(rect);
            }
            return GetLargestRect(rects);
        }

        private static Rect GetLargestRect(List<Rect> rects)
        {
            Rect rect = new Rect();
            foreach (Rect rect2 in rects)
            {
                if ((rect2.height * rect2.width) > (rect.height * rect.width))
                {
                    rect = rect2;
                }
            }
            return rect;
        }

        private static bool PopupAbove(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
        {
            Rect rect = new Rect(buttonRect.x, buttonRect.y - maxSize.y, maxSize.x, maxSize.y);
            float num = 0f;
            rect.yMin -= num;
            rect = FitRect(rect, popupContainerWindow);
            float a = Mathf.Max((float) ((buttonRect.y - rect.y) - num), (float) 0f);
            if (a >= minSize.y)
            {
                float height = Mathf.Min(a, maxSize.y);
                resultRect = new Rect(rect.x, buttonRect.y - height, rect.width, height);
                return true;
            }
            resultRect = new Rect(rect.x, buttonRect.y - a, rect.width, a);
            return false;
        }

        private static bool PopupBelow(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
        {
            Rect rect;
            rect = new Rect(buttonRect.x, buttonRect.yMax, maxSize.x, maxSize.y) {
                height = rect.height + k_SpaceFromBottom
            };
            rect = FitRect(rect, popupContainerWindow);
            float a = Mathf.Max((float) ((rect.yMax - buttonRect.yMax) - k_SpaceFromBottom), (float) 0f);
            if (a >= minSize.y)
            {
                float height = Mathf.Min(a, maxSize.y);
                resultRect = new Rect(rect.x, buttonRect.yMax, rect.width, height);
                return true;
            }
            resultRect = new Rect(rect.x, buttonRect.yMax, rect.width, a);
            return false;
        }

        private static bool PopupLeft(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
        {
            Rect rect = new Rect(buttonRect.x - maxSize.x, buttonRect.y, maxSize.x, maxSize.y);
            float num = 0f;
            rect.xMin -= num;
            rect.height += k_SpaceFromBottom;
            rect = FitRect(rect, popupContainerWindow);
            float a = Mathf.Max((float) ((buttonRect.x - rect.x) - num), (float) 0f);
            float width = Mathf.Min(a, maxSize.x);
            resultRect = new Rect(rect.x, rect.y, width, rect.height - k_SpaceFromBottom);
            return (a >= minSize.x);
        }

        private static bool PopupRight(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
        {
            Rect rect = new Rect(buttonRect.xMax, buttonRect.y, maxSize.x, maxSize.y);
            float num = 0f;
            rect.xMax += num;
            rect.height += k_SpaceFromBottom;
            rect = FitRect(rect, popupContainerWindow);
            float a = Mathf.Max((float) ((rect.xMax - buttonRect.xMax) - num), (float) 0f);
            float width = Mathf.Min(a, maxSize.x);
            resultRect = new Rect(rect.x, rect.y, width, rect.height - k_SpaceFromBottom);
            return (a >= minSize.x);
        }

        private static float k_SpaceFromBottom
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    return 10f;
                }
                return 0f;
            }
        }

        public enum PopupLocation
        {
            Below,
            Above,
            Left,
            Right
        }
    }
}

