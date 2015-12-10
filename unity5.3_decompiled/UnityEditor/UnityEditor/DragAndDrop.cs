namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class DragAndDrop
    {
        private static Hashtable ms_GenericData;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void AcceptDrag();
        public static object GetGenericData(string type)
        {
            if ((ms_GenericData != null) && ms_GenericData.Contains(type))
            {
                return ms_GenericData[type];
            }
            return null;
        }

        internal static bool HandleDelayedDrag(Rect position, int id, Object objectToDrag)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                {
                    if (((!position.Contains(current.mousePosition) || (current.clickCount != 1)) || (current.button != 0)) || ((Application.platform == RuntimePlatform.OSXEditor) && current.control))
                    {
                        break;
                    }
                    GUIUtility.hotControl = id;
                    DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                    stateObject.mouseDownPosition = current.mousePosition;
                    return true;
                }
                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                    if (!delay2.CanStartDrag())
                    {
                        break;
                    }
                    GUIUtility.hotControl = 0;
                    PrepareStartDrag();
                    Object[] objArray = new Object[] { objectToDrag };
                    objectReferences = objArray;
                    StartDrag(ObjectNames.GetDragAndDropTitle(objectToDrag));
                    return true;
                }
            }
            return false;
        }

        public static void PrepareStartDrag()
        {
            ms_GenericData = null;
            PrepareStartDrag_Internal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void PrepareStartDrag_Internal();
        public static void SetGenericData(string type, object data)
        {
            if (ms_GenericData == null)
            {
                ms_GenericData = new Hashtable();
            }
            ms_GenericData[type] = data;
        }

        public static void StartDrag(string title)
        {
            if ((Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag))
            {
                StartDrag_Internal(title);
            }
            else
            {
                Debug.LogError("Drags can only be started from MouseDown or MouseDrag events");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void StartDrag_Internal(string title);

        public static int activeControlID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Object[] objectReferences { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string[] paths { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static DragAndDropVisualMode visualMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

