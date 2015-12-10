namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class HeightmapFilters
    {
        public static void Flatten(TerrainData terrain, float height)
        {
            int heightmapWidth = terrain.heightmapWidth;
            float[,] heights = new float[terrain.heightmapHeight, heightmapWidth];
            for (int i = 0; i < heights.GetLength(0); i++)
            {
                for (int j = 0; j < heights.GetLength(1); j++)
                {
                    heights[i, j] = height;
                }
            }
            terrain.SetHeights(0, 0, heights);
        }

        private static void Noise(float[,] heights, TerrainData terrain)
        {
            for (int i = 0; i < heights.GetLength(0); i++)
            {
                for (int j = 0; j < heights.GetLength(1); j++)
                {
                    float single1 = heights[i, j];
                    single1[0] += Random.value * 0.01f;
                }
            }
        }

        public static void Smooth(TerrainData terrain)
        {
            int heightmapWidth = terrain.heightmapWidth;
            int heightmapHeight = terrain.heightmapHeight;
            float[,] heights = terrain.GetHeights(0, 0, heightmapWidth, heightmapHeight);
            Smooth(heights, terrain);
            terrain.SetHeights(0, 0, heights);
        }

        public static void Smooth(float[,] heights, TerrainData terrain)
        {
            float[,] numArray = heights.Clone() as float[,];
            int length = heights.GetLength(1);
            int num2 = heights.GetLength(0);
            for (int i = 1; i < (num2 - 1); i++)
            {
                for (int j = 1; j < (length - 1); j++)
                {
                    float num5 = 0f;
                    num5 += numArray[i, j];
                    num5 += numArray[i, j - 1];
                    num5 += numArray[i, j + 1];
                    num5 += numArray[i - 1, j];
                    num5 += numArray[i + 1, j];
                    num5 /= 5f;
                    heights[i, j] = num5;
                }
            }
        }

        private static void WobbleStuff(float[,] heights, TerrainData terrain)
        {
            for (int i = 0; i < heights.GetLength(0); i++)
            {
                for (int j = 0; j < heights.GetLength(1); j++)
                {
                    heights[i, j] = (heights[i, j] + 1f) / 2f;
                }
            }
        }
    }
}

