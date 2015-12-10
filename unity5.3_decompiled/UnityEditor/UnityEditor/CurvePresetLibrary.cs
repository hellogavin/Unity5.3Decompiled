namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CurvePresetLibrary : PresetLibrary
    {
        [SerializeField]
        private List<CurvePreset> m_Presets = new List<CurvePreset>();

        public override void Add(object presetObject, string presetName)
        {
            AnimationCurve curve = presetObject as AnimationCurve;
            if (curve == null)
            {
                Debug.LogError("Wrong type used in CurvePresetLibrary");
            }
            else
            {
                AnimationCurve preset = new AnimationCurve(curve.keys) {
                    preWrapMode = curve.preWrapMode,
                    postWrapMode = curve.postWrapMode
                };
                this.m_Presets.Add(new CurvePreset(preset, presetName));
            }
        }

        public override int Count()
        {
            return this.m_Presets.Count;
        }

        public override void Draw(Rect rect, int index)
        {
            this.DrawInternal(rect, this.m_Presets[index].curve);
        }

        public override void Draw(Rect rect, object presetObject)
        {
            this.DrawInternal(rect, presetObject as AnimationCurve);
        }

        private void DrawInternal(Rect rect, AnimationCurve animCurve)
        {
            if (animCurve != null)
            {
                EditorGUIUtility.DrawCurveSwatch(rect, animCurve, null, new Color(0.8f, 0.8f, 0.8f, 1f), EditorGUI.kCurveBGColor);
            }
        }

        public override string GetName(int index)
        {
            return this.m_Presets[index].name;
        }

        public override object GetPreset(int index)
        {
            return this.m_Presets[index].curve;
        }

        public override void Move(int index, int destIndex, bool insertAfterDestIndex)
        {
            PresetLibraryHelpers.MoveListItem<CurvePreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
        }

        public override void Remove(int index)
        {
            this.m_Presets.RemoveAt(index);
        }

        public override void Replace(int index, object newPresetObject)
        {
            AnimationCurve curve = newPresetObject as AnimationCurve;
            if (curve == null)
            {
                Debug.LogError("Wrong type used in CurvePresetLibrary");
            }
            else
            {
                AnimationCurve curve2 = new AnimationCurve(curve.keys) {
                    preWrapMode = curve.preWrapMode,
                    postWrapMode = curve.postWrapMode
                };
                this.m_Presets[index].curve = curve2;
            }
        }

        public override void SetName(int index, string presetName)
        {
            this.m_Presets[index].name = presetName;
        }

        [Serializable]
        private class CurvePreset
        {
            [SerializeField]
            private AnimationCurve m_Curve;
            [SerializeField]
            private string m_Name;

            public CurvePreset(AnimationCurve preset, string presetName)
            {
                this.curve = preset;
                this.name = presetName;
            }

            public CurvePreset(AnimationCurve preset, AnimationCurve preset2, string presetName)
            {
                this.curve = preset;
                this.name = presetName;
            }

            public AnimationCurve curve
            {
                get
                {
                    return this.m_Curve;
                }
                set
                {
                    this.m_Curve = value;
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

