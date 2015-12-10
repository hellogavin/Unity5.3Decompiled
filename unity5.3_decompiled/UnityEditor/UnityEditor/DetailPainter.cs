namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class DetailPainter
    {
        public Brush brush;
        public bool clearSelectedOnly;
        public float opacity;
        public bool randomizeDetails;
        public int size;
        public float targetStrength;
        public TerrainData terrainData;
        public TerrainTool tool;

        public void Paint(float xCenterNormalized, float yCenterNormalized, int detailIndex)
        {
            if (detailIndex < this.terrainData.detailPrototypes.Length)
            {
                int num = Mathf.FloorToInt(xCenterNormalized * this.terrainData.detailWidth);
                int num2 = Mathf.FloorToInt(yCenterNormalized * this.terrainData.detailHeight);
                int num3 = Mathf.RoundToInt((float) this.size) / 2;
                int num4 = Mathf.RoundToInt((float) this.size) % 2;
                int xBase = Mathf.Clamp(num - num3, 0, this.terrainData.detailWidth - 1);
                int yBase = Mathf.Clamp(num2 - num3, 0, this.terrainData.detailHeight - 1);
                int num7 = Mathf.Clamp((num + num3) + num4, 0, this.terrainData.detailWidth);
                int num8 = Mathf.Clamp((num2 + num3) + num4, 0, this.terrainData.detailHeight);
                int totalWidth = num7 - xBase;
                int totalHeight = num8 - yBase;
                int[] numArray = new int[] { detailIndex };
                if ((this.targetStrength < 0f) && !this.clearSelectedOnly)
                {
                    numArray = this.terrainData.GetSupportedLayers(xBase, yBase, totalWidth, totalHeight);
                }
                for (int i = 0; i < numArray.Length; i++)
                {
                    int[,] details = this.terrainData.GetDetailLayer(xBase, yBase, totalWidth, totalHeight, numArray[i]);
                    for (int j = 0; j < totalHeight; j++)
                    {
                        for (int k = 0; k < totalWidth; k++)
                        {
                            int ix = (xBase + k) - ((num - num3) + num4);
                            int iy = (yBase + j) - ((num2 - num3) + num4);
                            float t = this.opacity * this.brush.GetStrengthInt(ix, iy);
                            float targetStrength = this.targetStrength;
                            float num18 = Mathf.Lerp((float) details[j, k], targetStrength, t);
                            details[j, k] = Mathf.RoundToInt((num18 - 0.5f) + Random.value);
                        }
                    }
                    this.terrainData.SetDetailLayer(xBase, yBase, numArray[i], details);
                }
            }
        }
    }
}

