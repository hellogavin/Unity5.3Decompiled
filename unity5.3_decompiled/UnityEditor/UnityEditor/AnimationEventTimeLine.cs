namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class AnimationEventTimeLine
    {
        private bool m_DirtyTooltip;
        [NonSerialized]
        private AnimationEvent[] m_EventsAtMouseDown;
        private bool[] m_EventsSelected;
        [NonSerialized]
        private float[] m_EventTimes;
        private int m_HoverEvent = -1;
        private Vector2 m_InstantTooltipPoint = Vector2.zero;
        private string m_InstantTooltipText;
        private EditorWindow m_Owner;

        public AnimationEventTimeLine(EditorWindow owner)
        {
            this.m_Owner = owner;
        }

        public void AddEvent(AnimationWindowState state)
        {
            float time = ((float) state.frame) / state.frameRate;
            int index = AnimationEventPopup.Create(state.activeRootGameObject, state.activeAnimationClip, time, this.m_Owner);
            this.Select(state.activeAnimationClip, index);
        }

        private void CheckRectsOnMouseMove(Rect eventLineRect, AnimationEvent[] events, Rect[] hitRects)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            if (events.Length == hitRects.Length)
            {
                for (int i = hitRects.Length - 1; i >= 0; i--)
                {
                    if (hitRects[i].Contains(mousePosition))
                    {
                        flag = true;
                        if (this.m_HoverEvent != i)
                        {
                            this.m_HoverEvent = i;
                            this.m_InstantTooltipText = events[this.m_HoverEvent].functionName;
                            this.m_InstantTooltipPoint = new Vector2((hitRects[this.m_HoverEvent].xMin + ((int) (hitRects[this.m_HoverEvent].width / 2f))) + eventLineRect.x, eventLineRect.yMax);
                            this.m_DirtyTooltip = true;
                        }
                    }
                }
            }
            if (!flag)
            {
                this.m_HoverEvent = -1;
                this.m_InstantTooltipText = string.Empty;
            }
        }

        private void DeleteEvents(AnimationClip clip, bool[] deleteIndices)
        {
            bool flag = false;
            List<AnimationEvent> list = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (deleteIndices[i])
                {
                    list.RemoveAt(i);
                    flag = true;
                }
            }
            if (flag)
            {
                AnimationEventPopup.ClosePopup();
                Undo.RegisterCompleteObjectUndo(clip, "Delete Event");
                AnimationUtility.SetAnimationEvents(clip, list.ToArray());
                this.m_EventsSelected = new bool[list.Count];
                this.m_DirtyTooltip = true;
            }
        }

        public void DeselectAll()
        {
            this.m_EventsSelected = null;
        }

        public void DrawInstantTooltip(Rect position)
        {
            if ((this.m_InstantTooltipText != null) && (this.m_InstantTooltipText != string.Empty))
            {
                GUIStyle style = "AnimationEventTooltip";
                style.contentOffset = new Vector2(0f, 0f);
                style.overflow = new RectOffset(10, 10, 0, 0);
                Vector2 vector = style.CalcSize(new GUIContent(this.m_InstantTooltipText));
                Rect rect = new Rect(this.m_InstantTooltipPoint.x - (vector.x * 0.5f), this.m_InstantTooltipPoint.y + 24f, vector.x, vector.y);
                if (rect.xMax > position.width)
                {
                    rect.x = position.width - rect.width;
                }
                GUI.Label(rect, this.m_InstantTooltipText, style);
                rect = new Rect(this.m_InstantTooltipPoint.x - 33f, this.m_InstantTooltipPoint.y, 7f, 25f);
                GUI.Label(rect, string.Empty, "AnimationEventTooltipArrow");
            }
        }

        public void EventLineContextMenuAdd(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            int index = AnimationEventPopup.Create(obj2.m_Animated, obj2.m_Clip, obj2.m_Time, this.m_Owner);
            this.Select(obj2.m_Clip, index);
            this.m_EventsSelected = new bool[AnimationUtility.GetAnimationEvents(obj2.m_Clip).Length];
            this.m_EventsSelected[index] = true;
        }

        public void EventLineContextMenuDelete(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            AnimationClip clip = obj2.m_Clip;
            if (clip != null)
            {
                int index = obj2.m_Index;
                if (this.m_EventsSelected[index])
                {
                    this.DeleteEvents(clip, this.m_EventsSelected);
                }
                else
                {
                    bool[] deleteIndices = new bool[this.m_EventsSelected.Length];
                    deleteIndices[index] = true;
                    this.DeleteEvents(clip, deleteIndices);
                }
            }
        }

        public void EventLineContextMenuEdit(object obj)
        {
            EventLineContextMenuObject obj2 = (EventLineContextMenuObject) obj;
            AnimationEventPopup.Edit(obj2.m_Animated, obj2.m_Clip, obj2.m_Index, this.m_Owner);
            this.Select(obj2.m_Clip, obj2.m_Index);
        }

        public void EventLineGUI(Rect rect, AnimationWindowState state)
        {
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            GameObject activeRootGameObject = state.activeRootGameObject;
            GUI.BeginGroup(rect);
            Color color = GUI.color;
            Rect rect2 = new Rect(0f, 0f, rect.width, rect.height);
            float time = ((float) Mathf.RoundToInt(state.PixelToTime(Event.current.mousePosition.x, rect) * state.frameRate)) / state.frameRate;
            if (activeAnimationClip != null)
            {
                int num8;
                float num9;
                float num10;
                AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(activeAnimationClip);
                Texture image = EditorGUIUtility.IconContent("Animation.EventMarker").image;
                Rect[] hitPositions = new Rect[animationEvents.Length];
                Rect[] positions = new Rect[animationEvents.Length];
                int num2 = 1;
                int num3 = 0;
                for (int i = 0; i < animationEvents.Length; i++)
                {
                    AnimationEvent event2 = animationEvents[i];
                    if (num3 == 0)
                    {
                        num2 = 1;
                        while (((i + num2) < animationEvents.Length) && (animationEvents[i + num2].time == event2.time))
                        {
                            num2++;
                        }
                        num3 = num2;
                    }
                    num3--;
                    float num5 = Mathf.Floor(state.FrameToPixel(event2.time * activeAnimationClip.frameRate, rect));
                    int num6 = 0;
                    if (num2 > 1)
                    {
                        float num7 = Mathf.Min((int) ((num2 - 1) * (image.width - 1)), (int) (((int) state.FrameDeltaToPixel(rect)) - (image.width * 2)));
                        num6 = Mathf.FloorToInt(Mathf.Max((float) 0f, (float) (num7 - ((image.width - 1) * num3))));
                    }
                    Rect rect3 = new Rect((num5 + num6) - (image.width / 2), ((rect.height - 10f) * ((num3 - num2) + 1)) / ((float) Mathf.Max(1, num2 - 1)), (float) image.width, (float) image.height);
                    hitPositions[i] = rect3;
                    positions[i] = rect3;
                }
                if (this.m_DirtyTooltip)
                {
                    if ((this.m_HoverEvent >= 0) && (this.m_HoverEvent < hitPositions.Length))
                    {
                        this.m_InstantTooltipText = AnimationEventPopup.FormatEvent(activeRootGameObject, animationEvents[this.m_HoverEvent]);
                        this.m_InstantTooltipPoint = new Vector2(((hitPositions[this.m_HoverEvent].xMin + ((int) (hitPositions[this.m_HoverEvent].width / 2f))) + rect.x) - 30f, rect.yMax);
                    }
                    this.m_DirtyTooltip = false;
                }
                if ((this.m_EventsSelected == null) || (this.m_EventsSelected.Length != animationEvents.Length))
                {
                    this.m_EventsSelected = new bool[animationEvents.Length];
                    AnimationEventPopup.ClosePopup();
                }
                Vector2 zero = Vector2.zero;
                switch (EditorGUIExt.MultiSelection(rect, positions, new GUIContent(image), hitPositions, ref this.m_EventsSelected, null, out num8, out zero, out num9, out num10, GUIStyle.none))
                {
                    case HighLevelEvent.DoubleClick:
                        if (num8 == -1)
                        {
                            this.EventLineContextMenuAdd(new EventLineContextMenuObject(activeRootGameObject, activeAnimationClip, time, -1));
                            break;
                        }
                        AnimationEventPopup.Edit(activeRootGameObject, state.activeAnimationClip, num8, this.m_Owner);
                        break;

                    case HighLevelEvent.ContextClick:
                    {
                        GenericMenu menu = new GenericMenu();
                        EventLineContextMenuObject userData = new EventLineContextMenuObject(activeRootGameObject, activeAnimationClip, animationEvents[num8].time, num8);
                        menu.AddItem(new GUIContent("Edit Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuEdit), userData);
                        menu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), userData);
                        menu.AddItem(new GUIContent("Delete Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), userData);
                        menu.ShowAsContext();
                        this.m_InstantTooltipText = null;
                        this.m_DirtyTooltip = true;
                        state.Repaint();
                        break;
                    }
                    case HighLevelEvent.BeginDrag:
                        this.m_EventsAtMouseDown = animationEvents;
                        this.m_EventTimes = new float[animationEvents.Length];
                        for (int j = 0; j < animationEvents.Length; j++)
                        {
                            this.m_EventTimes[j] = animationEvents[j].time;
                        }
                        break;

                    case HighLevelEvent.Drag:
                    {
                        for (int k = animationEvents.Length - 1; k >= 0; k--)
                        {
                            if (this.m_EventsSelected[k])
                            {
                                AnimationEvent event4 = this.m_EventsAtMouseDown[k];
                                event4.time = this.m_EventTimes[k] + (zero.x * state.PixelDeltaToTime(rect));
                                event4.time = Mathf.Max(0f, event4.time);
                                event4.time = ((float) Mathf.RoundToInt(event4.time * activeAnimationClip.frameRate)) / activeAnimationClip.frameRate;
                            }
                        }
                        int[] items = new int[this.m_EventsSelected.Length];
                        for (int m = 0; m < items.Length; m++)
                        {
                            items[m] = m;
                        }
                        Array.Sort(this.m_EventsAtMouseDown, items, new EventComparer());
                        bool[] flagArray = (bool[]) this.m_EventsSelected.Clone();
                        float[] numArray2 = (float[]) this.m_EventTimes.Clone();
                        for (int n = 0; n < items.Length; n++)
                        {
                            this.m_EventsSelected[n] = flagArray[items[n]];
                            this.m_EventTimes[n] = numArray2[items[n]];
                        }
                        Undo.RegisterCompleteObjectUndo(activeAnimationClip, "Move Event");
                        AnimationUtility.SetAnimationEvents(activeAnimationClip, this.m_EventsAtMouseDown);
                        this.m_DirtyTooltip = true;
                        break;
                    }
                    case HighLevelEvent.Delete:
                        this.DeleteEvents(activeAnimationClip, this.m_EventsSelected);
                        break;

                    case HighLevelEvent.SelectionChanged:
                        state.ClearKeySelections();
                        if (num8 != -1)
                        {
                            AnimationEventPopup.UpdateSelection(activeRootGameObject, state.activeAnimationClip, num8, this.m_Owner);
                        }
                        break;
                }
                this.CheckRectsOnMouseMove(rect, animationEvents, hitPositions);
            }
            if ((Event.current.type == EventType.ContextClick) && rect2.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                GenericMenu menu2 = new GenericMenu();
                menu2.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new EventLineContextMenuObject(activeRootGameObject, activeAnimationClip, time, -1));
                menu2.ShowAsContext();
            }
            GUI.color = color;
            GUI.EndGroup();
        }

        private void Select(AnimationClip clip, int index)
        {
            this.m_EventsSelected = new bool[AnimationUtility.GetAnimationEvents(clip).Length];
            this.m_EventsSelected[index] = true;
        }

        public class EventComparer : IComparer
        {
            int IComparer.Compare(object objX, object objY)
            {
                AnimationEvent event2 = (AnimationEvent) objX;
                AnimationEvent event3 = (AnimationEvent) objY;
                float time = event2.time;
                float num2 = event3.time;
                if (time != num2)
                {
                    return (int) Mathf.Sign(time - num2);
                }
                int hashCode = event2.GetHashCode();
                int num4 = event3.GetHashCode();
                return (hashCode - num4);
            }
        }

        private class EventLineContextMenuObject
        {
            public GameObject m_Animated;
            public AnimationClip m_Clip;
            public int m_Index;
            public float m_Time;

            public EventLineContextMenuObject(GameObject animated, AnimationClip clip, float time, int index)
            {
                this.m_Animated = animated;
                this.m_Clip = clip;
                this.m_Time = time;
                this.m_Index = index;
            }
        }
    }
}

