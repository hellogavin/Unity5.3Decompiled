namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class CurveSelection : IComparable
    {
        private int m_CurveID;
        [NonSerialized]
        internal CurveEditor m_Host;
        private int m_Key;
        internal bool semiSelected;
        internal SelectionType type;

        internal CurveSelection(int curveID, CurveEditor host, int keyIndex)
        {
            this.m_Key = -1;
            this.m_CurveID = curveID;
            this.m_Host = host;
            this.m_Key = keyIndex;
            this.type = SelectionType.Key;
        }

        internal CurveSelection(int curveID, CurveEditor host, int keyIndex, SelectionType t)
        {
            this.m_Key = -1;
            this.m_CurveID = curveID;
            this.m_Host = host;
            this.m_Key = keyIndex;
            this.type = t;
        }

        public int CompareTo(object _other)
        {
            CurveSelection selection = (CurveSelection) _other;
            int num = this.curveID - selection.curveID;
            if (num != 0)
            {
                return num;
            }
            num = this.key - selection.key;
            if (num != 0)
            {
                return num;
            }
            return (int) (this.type - selection.type);
        }

        public override bool Equals(object _other)
        {
            CurveSelection selection = (CurveSelection) _other;
            return (((selection.curveID == this.curveID) && (selection.key == this.key)) && (selection.type == this.type));
        }

        public override int GetHashCode()
        {
            return (((this.curveID * 0x2d9) + (this.key * 0x1b)) + this.type);
        }

        internal bool validKey()
        {
            return (((this.curve != null) && (this.m_Key >= 0)) && (this.m_Key < this.curve.length));
        }

        internal AnimationCurve curve
        {
            get
            {
                return this.curveWrapper.curve;
            }
        }

        public int curveID
        {
            get
            {
                return this.m_CurveID;
            }
            set
            {
                this.m_CurveID = value;
            }
        }

        internal CurveWrapper curveWrapper
        {
            get
            {
                return this.m_Host.GetCurveFromID(this.m_CurveID);
            }
        }

        public int key
        {
            get
            {
                return this.m_Key;
            }
            set
            {
                this.m_Key = value;
            }
        }

        internal Keyframe keyframe
        {
            get
            {
                if (this.validKey())
                {
                    return this.curve[this.m_Key];
                }
                return new Keyframe();
            }
        }

        internal enum SelectionType
        {
            Key,
            InTangent,
            OutTangent,
            Count
        }
    }
}

