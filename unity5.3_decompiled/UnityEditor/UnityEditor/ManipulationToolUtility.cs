namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ManipulationToolUtility
    {
        public static HandleDragChange handleDragChange;

        public static void BeginDragging(string handleName)
        {
            if (handleDragChange != null)
            {
                handleDragChange(handleName, true);
            }
        }

        public static void DetectDraggingBasedOnMouseDownUp(string handleName, EventType typeBefore)
        {
            if ((typeBefore == EventType.MouseDrag) && (Event.current.type != EventType.MouseDrag))
            {
                BeginDragging(handleName);
            }
            else if ((typeBefore == EventType.MouseUp) && (Event.current.type != EventType.MouseUp))
            {
                EndDragging(handleName);
            }
        }

        public static void DisableMinDragDifference()
        {
            minDragDifference = Vector3.zero;
        }

        public static void DisableMinDragDifferenceBasedOnSnapping(Vector3 positionBeforeSnapping, Vector3 positionAfterSnapping)
        {
            for (int i = 0; i < 3; i++)
            {
                if (positionBeforeSnapping[i] != positionAfterSnapping[i])
                {
                    DisableMinDragDifferenceForAxis(i);
                }
            }
        }

        public static void DisableMinDragDifferenceForAxis(int axis)
        {
            Vector2 minDragDifference = ManipulationToolUtility.minDragDifference;
            minDragDifference[axis] = 0f;
            ManipulationToolUtility.minDragDifference = (Vector3) minDragDifference;
        }

        public static void EndDragging(string handleName)
        {
            if (handleDragChange != null)
            {
                handleDragChange(handleName, false);
            }
        }

        public static void SetMinDragDifferenceForPos(Vector3 position)
        {
            minDragDifference = (Vector3) (Vector3.one * (HandleUtility.GetHandleSize(position) / 80f));
        }

        public static Vector3 minDragDifference
        {
            [CompilerGenerated]
            get
            {
                return <minDragDifference>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <minDragDifference>k__BackingField = value;
            }
        }

        public delegate void HandleDragChange(string handleName, bool dragging);
    }
}

