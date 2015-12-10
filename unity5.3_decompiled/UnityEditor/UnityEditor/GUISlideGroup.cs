namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class GUISlideGroup
    {
        private Dictionary<int, Rect> animIDs = new Dictionary<int, Rect>();
        internal static GUISlideGroup current;
        private const float kLerp = 0.1f;
        private const float kSnap = 0.5f;

        public void Begin()
        {
            if (current != null)
            {
                Debug.LogError("You cannot nest animGroups");
            }
            else
            {
                current = this;
            }
        }

        public Rect BeginHorizontal(int id, params GUILayoutOption[] options)
        {
            SlideGroupInternal internal2 = (SlideGroupInternal) GUILayoutUtility.BeginLayoutGroup(GUIStyle.none, options, typeof(SlideGroupInternal));
            internal2.SetID(this, id);
            internal2.isVertical = false;
            return internal2.m_FinalRect;
        }

        public void End()
        {
            current = null;
        }

        public void EndHorizontal()
        {
            GUILayoutUtility.EndLayoutGroup();
        }

        public Rect GetRect(int id, Rect r)
        {
            bool flag;
            if (Event.current.type != EventType.Repaint)
            {
                return r;
            }
            return this.GetRect(id, r, out flag);
        }

        private Rect GetRect(int id, Rect r, out bool changed)
        {
            if (!this.animIDs.ContainsKey(id))
            {
                this.animIDs.Add(id, r);
                changed = false;
                return r;
            }
            Rect rect = this.animIDs[id];
            if (((rect.y != r.y) || (rect.height != r.height)) || ((rect.x != r.x) || (rect.width != r.width)))
            {
                float t = 0.1f;
                if (Mathf.Abs((float) (rect.y - r.y)) > 0.5f)
                {
                    r.y = Mathf.Lerp(rect.y, r.y, t);
                }
                if (Mathf.Abs((float) (rect.height - r.height)) > 0.5f)
                {
                    r.height = Mathf.Lerp(rect.height, r.height, t);
                }
                if (Mathf.Abs((float) (rect.x - r.x)) > 0.5f)
                {
                    r.x = Mathf.Lerp(rect.x, r.x, t);
                }
                if (Mathf.Abs((float) (rect.width - r.width)) > 0.5f)
                {
                    r.width = Mathf.Lerp(rect.width, r.width, t);
                }
                this.animIDs[id] = r;
                changed = true;
                HandleUtility.Repaint();
                return r;
            }
            changed = false;
            return r;
        }

        public void Reset()
        {
            current = null;
            this.animIDs.Clear();
        }

        private class SlideGroupInternal : GUILayoutGroup
        {
            internal Rect m_FinalRect;
            private int m_ID;
            private GUISlideGroup m_Owner;

            public override void SetHorizontal(float x, float width)
            {
                this.m_FinalRect.x = x;
                this.m_FinalRect.width = width;
                base.SetHorizontal(x, width);
            }

            public void SetID(GUISlideGroup owner, int id)
            {
                this.m_ID = id;
                this.m_Owner = owner;
            }

            public override void SetVertical(float y, float height)
            {
                bool flag;
                this.m_FinalRect.y = y;
                this.m_FinalRect.height = height;
                Rect r = new Rect(this.rect.x, y, this.rect.width, height);
                r = this.m_Owner.GetRect(this.m_ID, r, out flag);
                if (flag)
                {
                    base.SetHorizontal(r.x, r.width);
                }
                base.SetVertical(r.y, r.height);
            }
        }
    }
}

