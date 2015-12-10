namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngineInternal;

    public class GUILayoutUtility
    {
        internal static LayoutCache current = new LayoutCache();
        private static readonly Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);
        private static GUIStyle s_SpaceStyle;
        private static readonly Dictionary<int, LayoutCache> s_StoredLayouts = new Dictionary<int, LayoutCache>();
        private static readonly Dictionary<int, LayoutCache> s_StoredWindows = new Dictionary<int, LayoutCache>();

        internal static void Begin(int instanceID)
        {
            LayoutCache cache = SelectIDList(instanceID, false);
            if (Event.current.type == EventType.Layout)
            {
                current.topLevel = cache.topLevel = new GUILayoutGroup();
                current.layoutGroups.Clear();
                current.layoutGroups.Push(current.topLevel);
                current.windows = cache.windows = new GUILayoutGroup();
            }
            else
            {
                current.topLevel = cache.topLevel;
                current.layoutGroups = cache.layoutGroups;
                current.windows = cache.windows;
            }
        }

        public static void BeginGroup(string GroupName)
        {
        }

        internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, Type layoutType)
        {
            GUILayoutGroup next;
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Used:
                    next = CreateGUILayoutGroupInstanceOfType(layoutType);
                    next.style = style;
                    current.windows.Add(next);
                    break;

                default:
                    next = current.windows.GetNext() as GUILayoutGroup;
                    if (next == null)
                    {
                        throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
                    }
                    next.ResetCursor();
                    break;
            }
            current.layoutGroups.Push(next);
            current.topLevel = next;
            return next;
        }

        internal static GUILayoutGroup BeginLayoutGroup(GUIStyle style, GUILayoutOption[] options, Type layoutType)
        {
            GUILayoutGroup next;
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Used:
                    next = CreateGUILayoutGroupInstanceOfType(layoutType);
                    next.style = style;
                    if (options != null)
                    {
                        next.ApplyOptions(options);
                    }
                    current.topLevel.Add(next);
                    break;

                default:
                    next = current.topLevel.GetNext() as GUILayoutGroup;
                    if (next == null)
                    {
                        throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
                    }
                    next.ResetCursor();
                    break;
            }
            current.layoutGroups.Push(next);
            current.topLevel = next;
            return next;
        }

        internal static void BeginWindow(int windowID, GUIStyle style, GUILayoutOption[] options)
        {
            LayoutCache cache = SelectIDList(windowID, true);
            if (Event.current.type == EventType.Layout)
            {
                current.topLevel = cache.topLevel = new GUILayoutGroup();
                current.topLevel.style = style;
                current.topLevel.windowID = windowID;
                if (options != null)
                {
                    current.topLevel.ApplyOptions(options);
                }
                current.layoutGroups.Clear();
                current.layoutGroups.Push(current.topLevel);
                current.windows = cache.windows = new GUILayoutGroup();
            }
            else
            {
                current.topLevel = cache.topLevel;
                current.layoutGroups = cache.layoutGroups;
                current.windows = cache.windows;
            }
        }

        [SecuritySafeCritical]
        private static GUILayoutGroup CreateGUILayoutGroupInstanceOfType(Type LayoutType)
        {
            if (!typeof(GUILayoutGroup).IsAssignableFrom(LayoutType))
            {
                throw new ArgumentException("LayoutType needs to be of type GUILayoutGroup");
            }
            return (GUILayoutGroup) Activator.CreateInstance(LayoutType);
        }

        internal static GUILayoutGroup DoBeginLayoutArea(GUIStyle style, Type layoutType)
        {
            return BeginLayoutArea(style, layoutType);
        }

        private static Rect DoGetAspectRect(float aspect, GUIStyle style, GUILayoutOption[] options)
        {
            EventType type = Event.current.type;
            if (type != EventType.Layout)
            {
                if (type == EventType.Used)
                {
                    return kDummyRect;
                }
                return current.topLevel.GetNext().rect;
            }
            current.topLevel.Add(new GUIAspectSizer(aspect, options));
            return kDummyRect;
        }

        private static Rect DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            GUIUtility.CheckOnGUI();
            EventType type = Event.current.type;
            if (type != EventType.Layout)
            {
                if (type == EventType.Used)
                {
                    return kDummyRect;
                }
                return current.topLevel.GetNext().rect;
            }
            if (style.isHeightDependantOnWidth)
            {
                current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
            }
            else
            {
                Vector2 constraints = new Vector2(0f, 0f);
                if (options != null)
                {
                    foreach (GUILayoutOption option in options)
                    {
                        switch (option.type)
                        {
                            case GUILayoutOption.Type.maxWidth:
                                constraints.x = (float) option.value;
                                break;

                            case GUILayoutOption.Type.maxHeight:
                                constraints.y = (float) option.value;
                                break;
                        }
                    }
                }
                Vector2 vector2 = style.CalcSizeWithConstraints(content, constraints);
                current.topLevel.Add(new GUILayoutEntry(vector2.x, vector2.x, vector2.y, vector2.y, style, options));
            }
            return kDummyRect;
        }

        private static Rect DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options)
        {
            EventType type = Event.current.type;
            if (type != EventType.Layout)
            {
                if (type == EventType.Used)
                {
                    return kDummyRect;
                }
                return current.topLevel.GetNext().rect;
            }
            current.topLevel.Add(new GUILayoutEntry(minWidth, maxWidth, minHeight, maxHeight, style, options));
            return kDummyRect;
        }

        public static void EndGroup(string groupName)
        {
        }

        internal static void EndLayoutGroup()
        {
            EventType type = Event.current.type;
            current.layoutGroups.Pop();
            current.topLevel = (GUILayoutGroup) current.layoutGroups.Peek();
        }

        public static Rect GetAspectRect(float aspect)
        {
            return DoGetAspectRect(aspect, GUIStyle.none, null);
        }

        public static Rect GetAspectRect(float aspect, GUIStyle style)
        {
            return DoGetAspectRect(aspect, style, null);
        }

        public static Rect GetAspectRect(float aspect, params GUILayoutOption[] options)
        {
            return DoGetAspectRect(aspect, GUIStyle.none, options);
        }

        public static Rect GetAspectRect(float aspect, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetAspectRect(aspect, GUIStyle.none, options);
        }

        public static Rect GetLastRect()
        {
            EventType type = Event.current.type;
            if (type != EventType.Layout)
            {
                if (type == EventType.Used)
                {
                    return kDummyRect;
                }
                return current.topLevel.GetLast();
            }
            return kDummyRect;
        }

        public static Rect GetRect(float width, float height)
        {
            return DoGetRect(width, width, height, height, GUIStyle.none, null);
        }

        public static Rect GetRect(GUIContent content, GUIStyle style)
        {
            return DoGetRect(content, style, null);
        }

        public static Rect GetRect(float width, float height, GUIStyle style)
        {
            return DoGetRect(width, width, height, height, style, null);
        }

        public static Rect GetRect(float width, float height, params GUILayoutOption[] options)
        {
            return DoGetRect(width, width, height, height, GUIStyle.none, options);
        }

        public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetRect(content, style, options);
        }

        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, null);
        }

        public static Rect GetRect(float width, float height, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetRect(width, width, height, height, style, options);
        }

        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style)
        {
            return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, null);
        }

        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, params GUILayoutOption[] options)
        {
            return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, options);
        }

        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, options);
        }

        internal static Rect GetWindowsBounds()
        {
            Rect rect;
            INTERNAL_CALL_GetWindowsBounds(out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetWindowsBounds(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_GetWindowRect(int windowID, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_MoveWindow(int windowID, ref Rect r);
        private static Rect Internal_GetWindowRect(int windowID)
        {
            Rect rect;
            INTERNAL_CALL_Internal_GetWindowRect(windowID, out rect);
            return rect;
        }

        private static void Internal_MoveWindow(int windowID, Rect r)
        {
            INTERNAL_CALL_Internal_MoveWindow(windowID, ref r);
        }

        internal static void Layout()
        {
            if (current.topLevel.windowID == -1)
            {
                current.topLevel.CalcWidth();
                current.topLevel.SetHorizontal(0f, Mathf.Min(((float) Screen.width) / GUIUtility.pixelsPerPoint, current.topLevel.maxWidth));
                current.topLevel.CalcHeight();
                current.topLevel.SetVertical(0f, Mathf.Min(((float) Screen.height) / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
                LayoutFreeGroup(current.windows);
            }
            else
            {
                LayoutSingleGroup(current.topLevel);
                LayoutFreeGroup(current.windows);
            }
        }

        internal static void LayoutFreeGroup(GUILayoutGroup toplevel)
        {
            foreach (GUILayoutGroup group in toplevel.entries)
            {
                LayoutSingleGroup(group);
            }
            toplevel.ResetCursor();
        }

        internal static void LayoutFromEditorWindow()
        {
            current.topLevel.CalcWidth();
            current.topLevel.SetHorizontal(0f, ((float) Screen.width) / GUIUtility.pixelsPerPoint);
            current.topLevel.CalcHeight();
            current.topLevel.SetVertical(0f, ((float) Screen.height) / GUIUtility.pixelsPerPoint);
            LayoutFreeGroup(current.windows);
        }

        internal static float LayoutFromInspector(float width)
        {
            if ((current.topLevel != null) && (current.topLevel.windowID == -1))
            {
                current.topLevel.CalcWidth();
                current.topLevel.SetHorizontal(0f, width);
                current.topLevel.CalcHeight();
                current.topLevel.SetVertical(0f, Mathf.Min(((float) Screen.height) / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
                float minHeight = current.topLevel.minHeight;
                LayoutFreeGroup(current.windows);
                return minHeight;
            }
            if (current.topLevel != null)
            {
                LayoutSingleGroup(current.topLevel);
            }
            return 0f;
        }

        private static void LayoutSingleGroup(GUILayoutGroup i)
        {
            if (!i.isWindow)
            {
                float minWidth = i.minWidth;
                float maxWidth = i.maxWidth;
                i.CalcWidth();
                i.SetHorizontal(i.rect.x, Mathf.Clamp(i.maxWidth, minWidth, maxWidth));
                float minHeight = i.minHeight;
                float maxHeight = i.maxHeight;
                i.CalcHeight();
                i.SetVertical(i.rect.y, Mathf.Clamp(i.maxHeight, minHeight, maxHeight));
            }
            else
            {
                i.CalcWidth();
                Rect rect = Internal_GetWindowRect(i.windowID);
                i.SetHorizontal(rect.x, Mathf.Clamp(rect.width, i.minWidth, i.maxWidth));
                i.CalcHeight();
                i.SetVertical(rect.y, Mathf.Clamp(rect.height, i.minHeight, i.maxHeight));
                Internal_MoveWindow(i.windowID, i.rect);
            }
        }

        internal static LayoutCache SelectIDList(int instanceID, bool isWindow)
        {
            LayoutCache cache;
            Dictionary<int, LayoutCache> dictionary = !isWindow ? s_StoredLayouts : s_StoredWindows;
            if (!dictionary.TryGetValue(instanceID, out cache))
            {
                cache = new LayoutCache();
                dictionary[instanceID] = cache;
            }
            current.topLevel = cache.topLevel;
            current.layoutGroups = cache.layoutGroups;
            current.windows = cache.windows;
            return cache;
        }

        internal static GUIStyle spaceStyle
        {
            get
            {
                if (s_SpaceStyle == null)
                {
                    s_SpaceStyle = new GUIStyle();
                }
                s_SpaceStyle.stretchWidth = false;
                return s_SpaceStyle;
            }
        }

        internal static GUILayoutGroup topLevel
        {
            get
            {
                return current.topLevel;
            }
        }

        internal sealed class LayoutCache
        {
            internal GenericStack layoutGroups;
            internal GUILayoutGroup topLevel;
            internal GUILayoutGroup windows;

            internal LayoutCache()
            {
                this.topLevel = new GUILayoutGroup();
                this.layoutGroups = new GenericStack();
                this.windows = new GUILayoutGroup();
                this.layoutGroups.Push(this.topLevel);
            }

            internal LayoutCache(GUILayoutUtility.LayoutCache other)
            {
                this.topLevel = new GUILayoutGroup();
                this.layoutGroups = new GenericStack();
                this.windows = new GUILayoutGroup();
                this.topLevel = other.topLevel;
                this.layoutGroups = other.layoutGroups;
                this.windows = other.windows;
            }
        }
    }
}

