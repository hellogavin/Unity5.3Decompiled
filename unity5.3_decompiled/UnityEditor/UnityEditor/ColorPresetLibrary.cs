namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ColorPresetLibrary : PresetLibrary
    {
        public const int kMiniSwatchSize = 8;
        public const int kSwatchSize = 14;
        private Texture2D m_CheckerBoard;
        private Texture2D m_ColorSwatch;
        private Texture2D m_ColorSwatchTriangular;
        private Texture2D m_MiniColorSwatchTriangular;
        [SerializeField]
        private List<ColorPreset> m_Presets = new List<ColorPreset>();

        public override void Add(object presetObject, string presetName)
        {
            Color preset = (Color) presetObject;
            this.m_Presets.Add(new ColorPreset(preset, presetName));
        }

        public override int Count()
        {
            return this.m_Presets.Count;
        }

        private static Texture2D CreateColorSwatchWithBorder(int width, int height, bool triangular)
        {
            Texture2D textured = new Texture2D(width, height, TextureFormat.ARGB32, false) {
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] colors = new Color[width * height];
            Color color = new Color(1f, 1f, 1f, 0f);
            if (triangular)
            {
                for (int n = 0; n < height; n++)
                {
                    for (int num2 = 0; num2 < width; num2++)
                    {
                        if (n < (width - num2))
                        {
                            colors[num2 + (n * width)] = Color.white;
                        }
                        else
                        {
                            colors[num2 + (n * width)] = color;
                        }
                    }
                }
            }
            else
            {
                for (int num3 = 0; num3 < (height * width); num3++)
                {
                    colors[num3] = Color.white;
                }
            }
            for (int i = 0; i < width; i++)
            {
                colors[i] = Color.black;
            }
            for (int j = 0; j < width; j++)
            {
                colors[((height - 1) * width) + j] = Color.black;
            }
            for (int k = 0; k < height; k++)
            {
                colors[k * width] = Color.black;
            }
            for (int m = 0; m < height; m++)
            {
                colors[((m * width) + width) - 1] = Color.black;
            }
            textured.SetPixels(colors);
            textured.Apply();
            return textured;
        }

        public void CreateDebugColors()
        {
            for (int i = 0; i < 0x7d0; i++)
            {
                this.m_Presets.Add(new ColorPreset(new Color(Random.Range((float) 0.2f, (float) 1f), Random.Range((float) 0.2f, (float) 1f), Random.Range((float) 0.2f, (float) 1f), 1f), "Preset Color " + i));
            }
        }

        public override void Draw(Rect rect, int index)
        {
            this.DrawInternal(rect, this.m_Presets[index].color);
        }

        public override void Draw(Rect rect, object presetObject)
        {
            this.DrawInternal(rect, (Color) presetObject);
        }

        private void DrawInternal(Rect rect, Color preset)
        {
            this.Init();
            bool flag = preset.maxColorComponent > 1f;
            if (flag)
            {
                preset = preset.RGBMultiplied((float) (1f / preset.maxColorComponent));
            }
            Color color = GUI.color;
            if (((int) rect.height) == 14)
            {
                if (preset.a > 0.97f)
                {
                    this.RenderSolidSwatch(rect, preset);
                }
                else
                {
                    this.RenderSwatchWithAlpha(rect, preset, this.m_ColorSwatchTriangular);
                }
                if (flag)
                {
                    GUI.Label(rect, "h");
                }
            }
            else
            {
                this.RenderSwatchWithAlpha(rect, preset, this.m_MiniColorSwatchTriangular);
            }
            GUI.color = color;
        }

        public override string GetName(int index)
        {
            return this.m_Presets[index].name;
        }

        public override object GetPreset(int index)
        {
            return this.m_Presets[index].color;
        }

        private void Init()
        {
            if (this.m_ColorSwatch == null)
            {
                this.m_ColorSwatch = CreateColorSwatchWithBorder(14, 14, false);
            }
            if (this.m_ColorSwatchTriangular == null)
            {
                this.m_ColorSwatchTriangular = CreateColorSwatchWithBorder(14, 14, true);
            }
            if (this.m_MiniColorSwatchTriangular == null)
            {
                this.m_MiniColorSwatchTriangular = CreateColorSwatchWithBorder(8, 8, true);
            }
            if (this.m_CheckerBoard == null)
            {
                this.m_CheckerBoard = GradientEditor.CreateCheckerTexture(2, 2, 3, new Color(0.8f, 0.8f, 0.8f), new Color(0.5f, 0.5f, 0.5f));
            }
        }

        public override void Move(int index, int destIndex, bool insertAfterDestIndex)
        {
            PresetLibraryHelpers.MoveListItem<ColorPreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
        }

        private void OnDestroy()
        {
            if (this.m_ColorSwatch != null)
            {
                Object.DestroyImmediate(this.m_ColorSwatch);
            }
            if (this.m_ColorSwatchTriangular != null)
            {
                Object.DestroyImmediate(this.m_ColorSwatchTriangular);
            }
            if (this.m_MiniColorSwatchTriangular != null)
            {
                Object.DestroyImmediate(this.m_MiniColorSwatchTriangular);
            }
            if (this.m_CheckerBoard != null)
            {
                Object.DestroyImmediate(this.m_CheckerBoard);
            }
        }

        public override void Remove(int index)
        {
            this.m_Presets.RemoveAt(index);
        }

        private void RenderSolidSwatch(Rect rect, Color preset)
        {
            GUI.color = preset;
            GUI.DrawTexture(rect, this.m_ColorSwatch);
        }

        private void RenderSwatchWithAlpha(Rect rect, Color preset, Texture2D swatchTexture)
        {
            Rect position = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
            GUI.color = Color.white;
            Rect texCoords = new Rect(0f, 0f, position.width / ((float) this.m_CheckerBoard.width), position.height / ((float) this.m_CheckerBoard.height));
            GUI.DrawTextureWithTexCoords(position, this.m_CheckerBoard, texCoords, false);
            GUI.color = preset;
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            GUI.color = new Color(preset.r, preset.g, preset.b, 1f);
            GUI.DrawTexture(rect, swatchTexture);
        }

        public override void Replace(int index, object newPresetObject)
        {
            Color color = (Color) newPresetObject;
            this.m_Presets[index].color = color;
        }

        public override void SetName(int index, string presetName)
        {
            this.m_Presets[index].name = presetName;
        }

        [Serializable]
        private class ColorPreset
        {
            [SerializeField]
            private Color m_Color;
            [SerializeField]
            private string m_Name;

            public ColorPreset(Color preset, string presetName)
            {
                this.color = preset;
                this.name = presetName;
            }

            public ColorPreset(Color preset, Color preset2, string presetName)
            {
                this.color = preset;
                this.name = presetName;
            }

            public Color color
            {
                get
                {
                    return this.m_Color;
                }
                set
                {
                    this.m_Color = value;
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

