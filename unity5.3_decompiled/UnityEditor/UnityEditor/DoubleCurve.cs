namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class DoubleCurve
    {
        [SerializeField]
        private AnimationCurve m_MaxCurve;
        [SerializeField]
        private AnimationCurve m_MinCurve;
        [SerializeField]
        private bool m_SignedRange;

        public DoubleCurve(AnimationCurve minCurve, AnimationCurve maxCurve, bool signedRange)
        {
            AnimationCurve curve;
            if (minCurve != null)
            {
                curve = new AnimationCurve(minCurve.keys);
                this.m_MinCurve = curve;
            }
            if (maxCurve != null)
            {
                curve = new AnimationCurve(maxCurve.keys);
                this.m_MaxCurve = curve;
            }
            else
            {
                Debug.LogError("Ensure that maxCurve is not null when creating a double curve. The minCurve can be null for single curves");
            }
            this.m_SignedRange = signedRange;
        }

        public bool IsSingleCurve()
        {
            return ((this.minCurve == null) || (this.minCurve.length == 0));
        }

        public AnimationCurve maxCurve
        {
            get
            {
                return this.m_MaxCurve;
            }
            set
            {
                this.m_MaxCurve = value;
            }
        }

        public AnimationCurve minCurve
        {
            get
            {
                return this.m_MinCurve;
            }
            set
            {
                this.m_MinCurve = value;
            }
        }

        public bool signedRange
        {
            get
            {
                return this.m_SignedRange;
            }
            set
            {
                this.m_SignedRange = value;
            }
        }
    }
}

