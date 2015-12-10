namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SnapGuideCollection
    {
        private List<SnapGuide> currentGuides;
        private Dictionary<float, List<SnapGuide>> guides = new Dictionary<float, List<SnapGuide>>();

        public void AddGuide(SnapGuide guide)
        {
            List<SnapGuide> list;
            if (!this.guides.TryGetValue(guide.value, out list))
            {
                list = new List<SnapGuide>();
                this.guides.Add(guide.value, list);
            }
            list.Add(guide);
        }

        public void Clear()
        {
            this.guides.Clear();
        }

        public void DrawGuides()
        {
            if (this.currentGuides != null)
            {
                foreach (SnapGuide guide in this.currentGuides)
                {
                    guide.Draw();
                }
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.MouseUp)
            {
                this.currentGuides = null;
            }
        }

        public float SnapToGuides(float value, float snapDistance)
        {
            if (this.guides.Count != 0)
            {
                KeyValuePair<float, List<SnapGuide>> pair = new KeyValuePair<float, List<SnapGuide>>();
                float positiveInfinity = float.PositiveInfinity;
                foreach (KeyValuePair<float, List<SnapGuide>> pair2 in this.guides)
                {
                    float num2 = Mathf.Abs((float) (value - pair2.Key));
                    if (num2 < positiveInfinity)
                    {
                        pair = pair2;
                        positiveInfinity = num2;
                    }
                }
                if (positiveInfinity <= snapDistance)
                {
                    value = pair.Key;
                    this.currentGuides = pair.Value;
                    return value;
                }
                this.currentGuides = null;
            }
            return value;
        }
    }
}

