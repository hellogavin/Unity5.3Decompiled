namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class HeightmapPainter
    {
        public Brush brush;
        public int size;
        public float strength;
        public float targetHeight;
        public TerrainData terrainData;
        public TerrainTool tool;

        private float ApplyBrush(float height, float brushStrength, int x, int y)
        {
            if (this.tool == TerrainTool.PaintHeight)
            {
                return (height + brushStrength);
            }
            if (this.tool == TerrainTool.SetHeight)
            {
                if (this.targetHeight > height)
                {
                    height += brushStrength;
                    height = Mathf.Min(height, this.targetHeight);
                    return height;
                }
                height -= brushStrength;
                height = Mathf.Max(height, this.targetHeight);
                return height;
            }
            if (this.tool == TerrainTool.SmoothHeight)
            {
                return Mathf.Lerp(height, this.Smooth(x, y), brushStrength);
            }
            return height;
        }

        public void PaintHeight(float xCenterNormalized, float yCenterNormalized)
        {
            int num;
            int num2;
            if ((this.size % 2) == 0)
            {
                num = Mathf.CeilToInt(xCenterNormalized * (this.terrainData.heightmapWidth - 1));
                num2 = Mathf.CeilToInt(yCenterNormalized * (this.terrainData.heightmapHeight - 1));
            }
            else
            {
                num = Mathf.RoundToInt(xCenterNormalized * (this.terrainData.heightmapWidth - 1));
                num2 = Mathf.RoundToInt(yCenterNormalized * (this.terrainData.heightmapHeight - 1));
            }
            int num3 = this.size / 2;
            int num4 = this.size % 2;
            int xBase = Mathf.Clamp(num - num3, 0, this.terrainData.heightmapWidth - 1);
            int yBase = Mathf.Clamp(num2 - num3, 0, this.terrainData.heightmapHeight - 1);
            int num7 = Mathf.Clamp((num + num3) + num4, 0, this.terrainData.heightmapWidth);
            int num8 = Mathf.Clamp((num2 + num3) + num4, 0, this.terrainData.heightmapHeight);
            int width = num7 - xBase;
            int height = num8 - yBase;
            float[,] heights = this.terrainData.GetHeights(xBase, yBase, width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int ix = (xBase + j) - (num - num3);
                    int iy = (yBase + i) - (num2 - num3);
                    float strengthInt = this.brush.GetStrengthInt(ix, iy);
                    float num16 = heights[i, j];
                    num16 = this.ApplyBrush(num16, strengthInt * this.strength, j + xBase, i + yBase);
                    heights[i, j] = num16;
                }
            }
            this.terrainData.SetHeightsDelayLOD(xBase, yBase, heights);
        }

        private float Smooth(int x, int y)
        {
            float num = 0f;
            float num2 = 1f / this.terrainData.size.y;
            num += this.terrainData.GetHeight(x, y) * num2;
            num += this.terrainData.GetHeight(x + 1, y) * num2;
            num += this.terrainData.GetHeight(x - 1, y) * num2;
            num += (this.terrainData.GetHeight(x + 1, y + 1) * num2) * 0.75f;
            num += (this.terrainData.GetHeight(x - 1, y + 1) * num2) * 0.75f;
            num += (this.terrainData.GetHeight(x + 1, y - 1) * num2) * 0.75f;
            num += (this.terrainData.GetHeight(x - 1, y - 1) * num2) * 0.75f;
            num += this.terrainData.GetHeight(x, y + 1) * num2;
            num += this.terrainData.GetHeight(x, y - 1) * num2;
            return (num / 8f);
        }
    }
}

