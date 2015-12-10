namespace UnityEditorInternal
{
    using System;

    internal class ChartData
    {
        public int[] chartOrder;
        public ChartSeries[] charts;
        public int firstFrame;
        public int firstSelectableFrame;
        public float[] grid;
        public string[] gridLabels;
        public bool hasOverlay;
        public float maxValue;
        public float[] scale;
        public string[] selectedLabels;

        public void Assign(ChartSeries[] items, int firstFrame, int firstSelectableFrame)
        {
            this.charts = items;
            this.firstFrame = firstFrame;
            this.firstSelectableFrame = firstSelectableFrame;
            if ((this.chartOrder == null) || (this.chartOrder.Length != items.Length))
            {
                this.chartOrder = new int[items.Length];
                for (int i = 0; i < this.chartOrder.Length; i++)
                {
                    this.chartOrder[i] = (this.chartOrder.Length - 1) - i;
                }
            }
        }

        public void AssignScale(float[] scale)
        {
            this.scale = scale;
        }

        public void SetGrid(float[] grid, string[] labels)
        {
            this.grid = grid;
            this.gridLabels = labels;
        }

        public int NumberOfFrames
        {
            get
            {
                return this.charts[0].data.Length;
            }
        }
    }
}

