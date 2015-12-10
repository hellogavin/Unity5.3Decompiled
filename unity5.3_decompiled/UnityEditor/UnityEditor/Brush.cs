namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class Brush
    {
        internal const int kMinBrushSize = 3;
        private Texture2D m_Brush;
        private Projector m_BrushProjector;
        private Texture2D m_Preview;
        private int m_Size;
        private float[] m_Strength;

        private void CreatePreviewBrush()
        {
            Type[] components = new Type[] { typeof(Projector) };
            GameObject obj2 = EditorUtility.CreateGameObjectWithHideFlags("TerrainInspectorBrushPreview", HideFlags.HideAndDontSave, components);
            this.m_BrushProjector = obj2.GetComponent(typeof(Projector)) as Projector;
            this.m_BrushProjector.enabled = false;
            this.m_BrushProjector.nearClipPlane = -1000f;
            this.m_BrushProjector.farClipPlane = 1000f;
            this.m_BrushProjector.orthographic = true;
            this.m_BrushProjector.orthographicSize = 10f;
            this.m_BrushProjector.transform.Rotate((float) 90f, 0f, (float) 0f);
            Material material = EditorGUIUtility.LoadRequired("SceneView/TerrainBrushMaterial.mat") as Material;
            material.SetTexture("_CutoutTex", (Texture2D) EditorGUIUtility.Load(EditorResourcesUtility.brushesPath + "brush_cutout.png"));
            this.m_BrushProjector.material = material;
            this.m_BrushProjector.enabled = false;
        }

        public void Dispose()
        {
            if (this.m_BrushProjector != null)
            {
                Object.DestroyImmediate(this.m_BrushProjector.gameObject);
                this.m_BrushProjector = null;
            }
            Object.DestroyImmediate(this.m_Preview);
            this.m_Preview = null;
        }

        public Projector GetPreviewProjector()
        {
            return this.m_BrushProjector;
        }

        public float GetStrengthInt(int ix, int iy)
        {
            ix = Mathf.Clamp(ix, 0, this.m_Size - 1);
            iy = Mathf.Clamp(iy, 0, this.m_Size - 1);
            return this.m_Strength[(iy * this.m_Size) + ix];
        }

        public bool Load(Texture2D brushTex, int size)
        {
            if (((this.m_Brush == brushTex) && (size == this.m_Size)) && (this.m_Strength != null))
            {
                return true;
            }
            if (brushTex != null)
            {
                float num = size;
                this.m_Size = size;
                this.m_Strength = new float[this.m_Size * this.m_Size];
                if (this.m_Size > 3)
                {
                    for (int j = 0; j < this.m_Size; j++)
                    {
                        for (int k = 0; k < this.m_Size; k++)
                        {
                            this.m_Strength[(j * this.m_Size) + k] = brushTex.GetPixelBilinear((k + 0.5f) / num, ((float) j) / num).a;
                        }
                    }
                }
                else
                {
                    for (int m = 0; m < this.m_Strength.Length; m++)
                    {
                        this.m_Strength[m] = 1f;
                    }
                }
                Object.DestroyImmediate(this.m_Preview);
                this.m_Preview = new Texture2D(this.m_Size, this.m_Size, TextureFormat.ARGB32, false);
                this.m_Preview.hideFlags = HideFlags.HideAndDontSave;
                this.m_Preview.wrapMode = TextureWrapMode.Repeat;
                this.m_Preview.filterMode = FilterMode.Point;
                Color[] colors = new Color[this.m_Size * this.m_Size];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new Color(1f, 1f, 1f, this.m_Strength[i]);
                }
                this.m_Preview.SetPixels(0, 0, this.m_Size, this.m_Size, colors, 0);
                this.m_Preview.Apply();
                if (this.m_BrushProjector == null)
                {
                    this.CreatePreviewBrush();
                }
                this.m_BrushProjector.material.mainTexture = this.m_Preview;
                this.m_Brush = brushTex;
                return true;
            }
            this.m_Strength = new float[] { 1f };
            this.m_Size = 1;
            return false;
        }
    }
}

