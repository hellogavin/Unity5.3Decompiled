namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class EventManipulationHandler
    {
        private Rect[] m_EventRects = new Rect[0];
        private static AnimationEvent[] m_EventsAtMouseDown;
        private bool[] m_EventsSelected;
        private static float[] m_EventTimes;
        private int m_HoverEvent = -1;
        private Vector2 m_InstantTooltipPoint = Vector2.zero;
        private string m_InstantTooltipText;
        private TimeArea m_Timeline;

        public EventManipulationHandler(TimeArea timeArea)
        {
            this.m_Timeline = timeArea;
        }

        private void CheckRectsOnMouseMove(Rect eventLineRect, AnimationEvent[] events, Rect[] hitRects)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            this.m_InstantTooltipText = string.Empty;
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
                            this.m_InstantTooltipPoint = new Vector2(mousePosition.x, mousePosition.y);
                        }
                    }
                }
            }
            if (!flag)
            {
                this.m_HoverEvent = -1;
            }
        }

        private bool DeleteEvents(ref AnimationEvent[] eventList, bool[] deleteIndices)
        {
            bool flag = false;
            for (int i = eventList.Length - 1; i >= 0; i--)
            {
                if (deleteIndices[i])
                {
                    ArrayUtility.RemoveAt<AnimationEvent>(ref eventList, i);
                    flag = true;
                }
            }
            if (flag)
            {
                AnimationEventPopup.ClosePopup();
                this.m_EventsSelected = new bool[eventList.Length];
            }
            return flag;
        }

        public void DrawInstantTooltip(Rect window)
        {
            if ((this.m_InstantTooltipText != null) && (this.m_InstantTooltipText != string.Empty))
            {
                GUIStyle style = "AnimationEventTooltip";
                Vector2 vector = style.CalcSize(new GUIContent(this.m_InstantTooltipText));
                Rect position = new Rect(window.x + this.m_InstantTooltipPoint.x, window.y + this.m_InstantTooltipPoint.y, vector.x, vector.y);
                if (position.xMax > window.width)
                {
                    position.x = window.width - position.width;
                }
                GUI.Label(position, this.m_InstantTooltipText, style);
            }
        }

        public void EventLineContextMenuAdd(object obj)
        {
            EventModificationContextMenuObjet objet = (EventModificationContextMenuObjet) obj;
            objet.m_Info.AddEvent(objet.m_Time);
            this.SelectEvent(objet.m_Info.GetEvents(), objet.m_Info.GetEventCount() - 1, objet.m_Info);
        }

        public void EventLineContextMenuDelete(object obj)
        {
            EventModificationContextMenuObjet objet = (EventModificationContextMenuObjet) obj;
            objet.m_Info.RemoveEvent(objet.m_Index);
        }

        public bool HandleEventManipulation(Rect rect, ref AnimationEvent[] events, AnimationClipInfoProperties clipInfo)
        {
            int num8;
            float num9;
            float num10;
            Texture image = EditorGUIUtility.IconContent("Animation.EventMarker").image;
            bool flag = false;
            Rect[] hitPositions = new Rect[events.Length];
            Rect[] positions = new Rect[events.Length];
            int num = 1;
            int num2 = 0;
            for (int i = 0; i < events.Length; i++)
            {
                AnimationEvent event2 = events[i];
                if (num2 == 0)
                {
                    num = 1;
                    while (((i + num) < events.Length) && (events[i + num].time == event2.time))
                    {
                        num++;
                    }
                    num2 = num;
                }
                num2--;
                float num4 = Mathf.Floor(this.m_Timeline.TimeToPixel(event2.time, rect));
                int num5 = 0;
                if (num > 1)
                {
                    float num6 = Mathf.Min((int) ((num - 1) * (image.width - 1)), (int) (((int) (1f / this.m_Timeline.PixelDeltaToTime(rect))) - (image.width * 2)));
                    num5 = Mathf.FloorToInt(Mathf.Max((float) 0f, (float) (num6 - ((image.width - 1) * num2))));
                }
                Rect rect2 = new Rect((num4 + num5) - (image.width / 2), ((rect.height - 10f) * ((num2 - num) + 1)) / ((float) Mathf.Max(1, num - 1)), (float) image.width, (float) image.height);
                hitPositions[i] = rect2;
                positions[i] = rect2;
            }
            this.m_EventRects = new Rect[hitPositions.Length];
            for (int j = 0; j < hitPositions.Length; j++)
            {
                this.m_EventRects[j] = new Rect(hitPositions[j].x + rect.x, hitPositions[j].y + rect.y, hitPositions[j].width, hitPositions[j].height);
            }
            if (((this.m_EventsSelected == null) || (this.m_EventsSelected.Length != events.Length)) || (this.m_EventsSelected.Length == 0))
            {
                this.m_EventsSelected = new bool[events.Length];
                AnimationEventPopup.ClosePopup();
            }
            Vector2 zero = Vector2.zero;
            switch (EditorGUIExt.MultiSelection(rect, positions, new GUIContent(image), hitPositions, ref this.m_EventsSelected, null, out num8, out zero, out num9, out num10, GUIStyle.none))
            {
                case HighLevelEvent.ContextClick:
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new EventModificationContextMenuObjet(clipInfo, events[num8].time, num8));
                    menu.AddItem(new GUIContent("Delete Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), new EventModificationContextMenuObjet(clipInfo, events[num8].time, num8));
                    menu.ShowAsContext();
                    this.m_InstantTooltipText = null;
                    break;
                }
                case HighLevelEvent.BeginDrag:
                    m_EventsAtMouseDown = events;
                    m_EventTimes = new float[events.Length];
                    for (int k = 0; k < events.Length; k++)
                    {
                        m_EventTimes[k] = events[k].time;
                    }
                    break;

                case HighLevelEvent.Drag:
                {
                    for (int m = events.Length - 1; m >= 0; m--)
                    {
                        if (this.m_EventsSelected[m])
                        {
                            AnimationEvent event4 = m_EventsAtMouseDown[m];
                            event4.time = Mathf.Clamp01(m_EventTimes[m] + (zero.x / rect.width));
                        }
                    }
                    int[] items = new int[this.m_EventsSelected.Length];
                    for (int n = 0; n < items.Length; n++)
                    {
                        items[n] = n;
                    }
                    Array.Sort(m_EventsAtMouseDown, items, new AnimationEventTimeLine.EventComparer());
                    bool[] flagArray = (bool[]) this.m_EventsSelected.Clone();
                    float[] numArray2 = (float[]) m_EventTimes.Clone();
                    for (int num14 = 0; num14 < items.Length; num14++)
                    {
                        this.m_EventsSelected[num14] = flagArray[items[num14]];
                        m_EventTimes[num14] = numArray2[items[num14]];
                    }
                    events = m_EventsAtMouseDown;
                    flag = true;
                    break;
                }
                case HighLevelEvent.Delete:
                    flag = this.DeleteEvents(ref events, this.m_EventsSelected);
                    break;

                case HighLevelEvent.SelectionChanged:
                    if (num8 < 0)
                    {
                        AnimationEventPopup.ClosePopup();
                        break;
                    }
                    AnimationEventPopup.Edit(clipInfo, num8);
                    break;
            }
            this.CheckRectsOnMouseMove(rect, events, hitPositions);
            return flag;
        }

        public void SelectEvent(AnimationEvent[] events, int index, AnimationClipInfoProperties clipInfo)
        {
            this.m_EventsSelected = new bool[events.Length];
            this.m_EventsSelected[index] = true;
            AnimationEventPopup.Edit(clipInfo, index);
        }

        private class EventModificationContextMenuObjet
        {
            public int m_Index;
            public AnimationClipInfoProperties m_Info;
            public float m_Time;

            public EventModificationContextMenuObjet(AnimationClipInfoProperties info, float time, int index)
            {
                this.m_Info = info;
                this.m_Time = time;
                this.m_Index = index;
            }
        }
    }
}

