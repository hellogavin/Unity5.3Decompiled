namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class DoubleCurvePresetLibrary : PresetLibrary
    {
        private readonly Rect kSignedRange = new Rect(0f, -1f, 1f, 2f);
        private readonly Rect kUnsignedRange = new Rect(0f, 0f, 1f, 1f);
        [SerializeField]
        private List<DoubleCurvePreset> m_Presets = new List<DoubleCurvePreset>();

        public override void Add(object presetObject, string presetName)
        {
            DoubleCurve doubleCurvePreset = presetObject as DoubleCurve;
            if (doubleCurvePreset == null)
            {
                Debug.LogError("Wrong type used in DoubleCurvePresetLibrary: Should be a DoubleCurve");
            }
            else
            {
                this.m_Presets.Add(new DoubleCurvePreset(doubleCurvePreset, presetName));
            }
        }

        public override int Count()
        {
            return this.m_Presets.Count;
        }

        public override void Draw(Rect rect, int index)
        {
            this.DrawInternal(rect, this.m_Presets[index].doubleCurve);
        }

        public override void Draw(Rect rect, object presetObject)
        {
            this.DrawInternal(rect, presetObject as DoubleCurve);
        }

        private void DrawInternal(Rect rect, DoubleCurve doubleCurve)
        {
            if (doubleCurve == null)
            {
                Debug.Log("DoubleCurve is null");
            }
            else
            {
                EditorGUIUtility.DrawRegionSwatch(rect, doubleCurve.maxCurve, doubleCurve.minCurve, new Color(0.8f, 0.8f, 0.8f, 1f), EditorGUI.kCurveBGColor, !doubleCurve.signedRange ? this.kUnsignedRange : this.kSignedRange);
            }
        }

        public override string GetName(int index)
        {
            return this.m_Presets[index].name;
        }

        public override object GetPreset(int index)
        {
            return this.m_Presets[index].doubleCurve;
        }

        public override void Move(int index, int destIndex, bool insertAfterDestIndex)
        {
            PresetLibraryHelpers.MoveListItem<DoubleCurvePreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
        }

        public override void Remove(int index)
        {
            this.m_Presets.RemoveAt(index);
        }

        public override void Replace(int index, object newPresetObject)
        {
            DoubleCurve curve = newPresetObject as DoubleCurve;
            if (curve == null)
            {
                Debug.LogError("Wrong type used in DoubleCurvePresetLibrary");
            }
            else
            {
                this.m_Presets[index].doubleCurve = curve;
            }
        }

        public override void SetName(int index, string presetName)
        {
            this.m_Presets[index].name = presetName;
        }

        [Serializable]
        private class DoubleCurvePreset
        {
            [SerializeField]
            private DoubleCurve m_DoubleCurve;
            [SerializeField]
            private string m_Name;

            public DoubleCurvePreset(DoubleCurve doubleCurvePreset, string presetName)
            {
                this.doubleCurve = doubleCurvePreset;
                this.name = presetName;
            }

            public DoubleCurve doubleCurve
            {
                get
                {
                    return this.m_DoubleCurve;
                }
                set
                {
                    this.m_DoubleCurve = value;
                }
            }

            public string name
            {
                get
                {
                    return this.m_Name;
                }
                set
                {
                    this.m_Name = value;
                }
            }
        }
    }
}

