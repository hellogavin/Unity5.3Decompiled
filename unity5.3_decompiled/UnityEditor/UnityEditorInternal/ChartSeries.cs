namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    internal class ChartSeries
    {
        public Color color;
        public float[] data;
        public bool enabled;
        public string identifierName;
        public string name;
        public float[] overlayData;

        public ChartSeries(string name, int len, Color clr)
        {
            this.name = name;
            this.identifierName = name;
            this.data = new float[len];
            this.overlayData = null;
            this.color = clr;
            this.enabled = true;
        }

        public void CreateOverlayData()
        {
            this.overlayData = new float[this.data.Length];
        }
    }
}

