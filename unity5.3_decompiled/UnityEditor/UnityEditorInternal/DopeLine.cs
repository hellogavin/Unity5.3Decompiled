namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class DopeLine
    {
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache9;
        public static GUIStyle dopekeyStyle = "Dopesheetkeyframe";
        public bool hasChildren;
        public bool isMasterDopeline;
        public List<AnimationWindowKeyframe> keys;
        public AnimationWindowCurve[] m_Curves;
        public int m_HierarchyNodeID;
        public Type objectType;
        public Rect position;
        public bool tallMode;

        public DopeLine(int hierarchyNodeId, AnimationWindowCurve[] curves)
        {
            this.m_HierarchyNodeID = hierarchyNodeId;
            this.m_Curves = curves;
            this.LoadKeyframes();
        }

        public void LoadKeyframes()
        {
            this.keys = new List<AnimationWindowKeyframe>();
            foreach (AnimationWindowCurve curve in this.m_Curves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    this.keys.Add(keyframe);
                }
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = (a, b) => a.time.CompareTo(b.time);
            }
            this.keys.Sort(<>f__am$cache9);
        }

        public bool isPptrDopeline
        {
            get
            {
                if (this.m_Curves.Length <= 0)
                {
                    return false;
                }
                for (int i = 0; i < this.m_Curves.Length; i++)
                {
                    if (!this.m_Curves[i].isPPtrCurve)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Type valueType
        {
            get
            {
                if (this.m_Curves.Length <= 0)
                {
                    return null;
                }
                Type valueType = this.m_Curves[0].m_ValueType;
                for (int i = 1; i < this.m_Curves.Length; i++)
                {
                    if (this.m_Curves[i].m_ValueType != valueType)
                    {
                        return null;
                    }
                }
                return valueType;
            }
        }
    }
}

