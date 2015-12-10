namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class GradientPresetLibrary : PresetLibrary
    {
        [SerializeField]
        private List<GradientPreset> m_Presets = new List<GradientPreset>();

        public override void Add(object presetObject, string presetName)
        {
            Gradient gradient = presetObject as Gradient;
            if (gradient == null)
            {
                Debug.LogError("Wrong type used in GradientPresetLibrary");
            }
            else
            {
                Gradient preset = new Gradient {
                    alphaKeys = gradient.alphaKeys,
                    colorKeys = gradient.colorKeys
                };
                this.m_Presets.Add(new GradientPreset(preset, presetName));
            }
        }

        public override int Count()
        {
            return this.m_Presets.Count;
        }

        public void DebugCreateTonsOfPresets()
        {
            int num = 150;
            string str = "Preset_";
            for (int i = 0; i < num; i++)
            {
                List<GradientColorKey> list = new List<GradientColorKey>();
                int num3 = Random.Range(3, 8);
                for (int j = 0; j < num3; j++)
                {
                    list.Add(new GradientColorKey(new Color(Random.value, Random.value, Random.value), Random.value));
                }
                List<GradientAlphaKey> list2 = new List<GradientAlphaKey>();
                int num5 = Random.Range(3, 8);
                for (int k = 0; k < num5; k++)
                {
                    list2.Add(new GradientAlphaKey(Random.value, Random.value));
                }
                Gradient presetObject = new Gradient {
                    colorKeys = list.ToArray(),
                    alphaKeys = list2.ToArray()
                };
                this.Add(presetObject, str + (i + 1));
            }
        }

        public override void Draw(Rect rect, int index)
        {
            this.DrawInternal(rect, this.m_Presets[index].gradient);
        }

        public override void Draw(Rect rect, object presetObject)
        {
            this.DrawInternal(rect, presetObject as Gradient);
        }

        private void DrawInternal(Rect rect, Gradient gradient)
        {
            if (gradient != null)
            {
                GradientEditor.DrawGradientWithBackground(rect, GradientPreviewCache.GetGradientPreview(gradient));
            }
        }

        public override string GetName(int index)
        {
            return this.m_Presets[index].name;
        }

        public override object GetPreset(int index)
        {
            return this.m_Presets[index].gradient;
        }

        public override void Move(int index, int destIndex, bool insertAfterDestIndex)
        {
            PresetLibraryHelpers.MoveListItem<GradientPreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
        }

        public override void Remove(int index)
        {
            this.m_Presets.RemoveAt(index);
        }

        public override void Replace(int index, object newPresetObject)
        {
            Gradient gradient = newPresetObject as Gradient;
            if (gradient == null)
            {
                Debug.LogError("Wrong type used in GradientPresetLibrary");
            }
            else
            {
                Gradient gradient2 = new Gradient {
                    alphaKeys = gradient.alphaKeys,
                    colorKeys = gradient.colorKeys
                };
                this.m_Presets[index].gradient = gradient2;
            }
        }

        public override void SetName(int index, string presetName)
        {
            this.m_Presets[index].name = presetName;
        }

        [Serializable]
        private class GradientPreset
        {
            [SerializeField]
            private Gradient m_Gradient;
            [SerializeField]
            private string m_Name;

            public GradientPreset(Gradient preset, string presetName)
            {
                this.gradient = preset;
                this.name = presetName;
            }

            public Gradient gradient
            {
                get
                {
                    return this.m_Gradient;
                }
                set
                {
                    this.m_Gradient = value;
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

