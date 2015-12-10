namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainEditorUtility
    {
        internal static bool IsLODTreePrototype(GameObject prefab)
        {
            return ((prefab != null) && (prefab.GetComponent<LODGroup>() != null));
        }

        internal static void RemoveDetail(Terrain terrain, int index)
        {
            TerrainData terrainData = terrain.terrainData;
            if (terrainData != null)
            {
                Undo.RegisterCompleteObjectUndo(terrainData, "Remove detail object");
                terrainData.RemoveDetailPrototype(index);
            }
        }

        internal static void RemoveSplatTexture(TerrainData terrainData, int index)
        {
            Undo.RegisterCompleteObjectUndo(terrainData, "Remove texture");
            int alphamapWidth = terrainData.alphamapWidth;
            int alphamapHeight = terrainData.alphamapHeight;
            float[,,] numArray = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
            int length = numArray.GetLength(2);
            int num4 = length - 1;
            float[,,] map = new float[alphamapHeight, alphamapWidth, num4];
            for (int i = 0; i < alphamapHeight; i++)
            {
                for (int n = 0; n < alphamapWidth; n++)
                {
                    for (int num7 = 0; num7 < index; num7++)
                    {
                        map[i, n, num7] = numArray[i, n, num7];
                    }
                    for (int num8 = index + 1; num8 < length; num8++)
                    {
                        map[i, n, num8 - 1] = numArray[i, n, num8];
                    }
                }
            }
            for (int j = 0; j < alphamapHeight; j++)
            {
                for (int num10 = 0; num10 < alphamapWidth; num10++)
                {
                    float num11 = 0f;
                    for (int num12 = 0; num12 < num4; num12++)
                    {
                        num11 += map[j, num10, num12];
                    }
                    if (num11 >= 0.01)
                    {
                        float num13 = 1f / num11;
                        for (int num14 = 0; num14 < num4; num14++)
                        {
                            float single1 = map[j, num10, num14];
                            single1[0] *= num13;
                        }
                    }
                    else
                    {
                        for (int num15 = 0; num15 < num4; num15++)
                        {
                            map[j, num10, num15] = (num15 != 0) ? 0f : 1f;
                        }
                    }
                }
            }
            SplatPrototype[] splatPrototypes = terrainData.splatPrototypes;
            SplatPrototype[] prototypeArray2 = new SplatPrototype[splatPrototypes.Length - 1];
            for (int k = 0; k < index; k++)
            {
                prototypeArray2[k] = splatPrototypes[k];
            }
            for (int m = index + 1; m < length; m++)
            {
                prototypeArray2[m - 1] = splatPrototypes[m];
            }
            terrainData.splatPrototypes = prototypeArray2;
            terrainData.SetAlphamaps(0, 0, map);
        }

        internal static void RemoveTree(Terrain terrain, int index)
        {
            TerrainData terrainData = terrain.terrainData;
            if (terrainData != null)
            {
                Undo.RegisterCompleteObjectUndo(terrainData, "Remove tree");
                terrainData.RemoveTreePrototype(index);
            }
        }
    }
}

