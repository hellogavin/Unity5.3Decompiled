namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SplatPainter
    {
        public Brush brush;
        public int size;
        public float strength;
        public float target;
        public TerrainData terrainData;
        public TerrainTool tool;

        private float ApplyBrush(float height, float brushStrength)
        {
            if (this.target > height)
            {
                height += brushStrength;
                height = Mathf.Min(height, this.target);
                return height;
            }
            height -= brushStrength;
            height = Mathf.Max(height, this.target);
            return height;
        }

        private void Normalize(int x, int y, int splatIndex, float[,,] alphamap)
        {
            float num = alphamap[y, x, splatIndex];
            float num2 = 0f;
            int length = alphamap.GetLength(2);
            for (int i = 0; i < length; i++)
            {
                if (i != splatIndex)
                {
                    num2 += alphamap[y, x, i];
                }
            }
            if (num2 > 0.01)
            {
                float num5 = (1f - num) / num2;
                for (int j = 0; j < length; j++)
                {
                    if (j != splatIndex)
                    {
                        float single1 = alphamap[y, x, j];
                        single1[0] *= num5;
                    }
                }
            }
            else
            {
                for (int k = 0; k < length; k++)
                {
                    alphamap[y, x, k] = (k != splatIndex) ? 0f : 1f;
                }
            }
        }

        public void Paint(float xCenterNormalized, float yCenterNormalized, int splatIndex)
        {
            if (splatIndex < this.terrainData.alphamapLayers)
            {
                int num = Mathf.FloorToInt(xCenterNormalized * this.terrainData.alphamapWidth);
                int num2 = Mathf.FloorToInt(yCenterNormalized * this.terrainData.alphamapHeight);
                int num3 = Mathf.RoundToInt((float) this.size) / 2;
                int num4 = Mathf.RoundToInt((float) this.size) % 2;
                int x = Mathf.Clamp(num - num3, 0, this.terrainData.alphamapWidth - 1);
                int y = Mathf.Clamp(num2 - num3, 0, this.terrainData.alphamapHeight - 1);
                int num7 = Mathf.Clamp((num + num3) + num4, 0, this.terrainData.alphamapWidth);
                int num8 = Mathf.Clamp((num2 + num3) + num4, 0, this.terrainData.alphamapHeight);
                int width = num7 - x;
                int height = num8 - y;
                float[,,] alphamap = this.terrainData.GetAlphamaps(x, y, width, height);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int ix = (x + j) - ((num - num3) + num4);
                        int iy = (y + i) - ((num2 - num3) + num4);
                        float strengthInt = this.brush.GetStrengthInt(ix, iy);
                        float num16 = this.ApplyBrush(alphamap[i, j, splatIndex], strengthInt * this.strength);
                        alphamap[i, j, splatIndex] = num16;
                        this.Normalize(j, i, splatIndex, alphamap);
                    }
                }
                this.terrainData.SetAlphamaps(x, y, alphamap);
            }
        }
    }
}

