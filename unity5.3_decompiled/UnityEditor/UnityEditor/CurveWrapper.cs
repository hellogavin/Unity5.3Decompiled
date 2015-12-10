namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CurveWrapper
    {
        public EditorCurveBinding binding;
        public bool changed;
        public Color color;
        public GetAxisScalarsCallback getAxisUiScalarsCallback = null;
        public int groupId = -1;
        public bool hidden = false;
        public int id = 0;
        public int listIndex = -1;
        private CurveRenderer m_Renderer;
        public bool readOnly = false;
        public int regionId = -1;
        public SelectionMode selected;
        public SetAxisScalarsCallback setAxisUiScalarsCallback = null;
        public float vRangeMax = float.PositiveInfinity;
        public float vRangeMin = float.NegativeInfinity;

        public AnimationCurve curve
        {
            get
            {
                return this.renderer.GetCurve();
            }
        }

        public CurveRenderer renderer
        {
            get
            {
                return this.m_Renderer;
            }
            set
            {
                this.m_Renderer = value;
            }
        }

        public delegate Vector2 GetAxisScalarsCallback();

        internal enum SelectionMode
        {
            None,
            Selected,
            SemiSelected
        }

        public delegate void SetAxisScalarsCallback(Vector2 newAxisScalars);
    }
}

