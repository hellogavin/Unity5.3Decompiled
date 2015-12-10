namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TreePainter
    {
        public static bool allowHeightVar = true;
        public static bool allowWidthVar = true;
        public static float brushSize = 40f;
        public static bool lockWidthToHeight = true;
        public static bool randomRotation = true;
        public static int selectedTree = -1;
        public static float spacing = 0.8f;
        public static float treeColorAdjustment = 0.4f;
        public static float treeHeight = 1f;
        public static float treeHeightVariation = 0.1f;
        public static float treeWidth = 1f;
        public static float treeWidthVariation = 0.1f;

        private static Color GetTreeColor()
        {
            Color color = (Color) (Color.white * Random.Range((float) 1f, (float) (1f - treeColorAdjustment)));
            color.a = 1f;
            return color;
        }

        private static float GetTreeHeight()
        {
            float num = !allowHeightVar ? 0f : treeHeightVariation;
            return (treeHeight * Random.Range((float) (1f - num), (float) (1f + num)));
        }

        private static float GetTreeRotation()
        {
            return (!randomRotation ? 0f : Random.Range((float) 0f, (float) 6.283185f));
        }

        private static float GetTreeWidth()
        {
            float num = !allowWidthVar ? 0f : treeWidthVariation;
            return (treeWidth * Random.Range((float) (1f - num), (float) (1f + num)));
        }

        public static void MassPlaceTrees(TerrainData terrainData, int numberOfTrees, bool randomTreeColor, bool keepExistingTrees)
        {
            int length = terrainData.treePrototypes.Length;
            if (length == 0)
            {
                Debug.Log("Can't place trees because no prototypes are defined");
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(terrainData, "Mass Place Trees");
                TreeInstance[] sourceArray = new TreeInstance[numberOfTrees];
                int num2 = 0;
                while (num2 < sourceArray.Length)
                {
                    TreeInstance instance = new TreeInstance {
                        position = new Vector3(Random.value, 0f, Random.value)
                    };
                    if (terrainData.GetSteepness(instance.position.x, instance.position.z) < 30f)
                    {
                        instance.color = !randomTreeColor ? Color.white : GetTreeColor();
                        instance.lightmapColor = Color.white;
                        instance.prototypeIndex = Random.Range(0, length);
                        instance.heightScale = GetTreeHeight();
                        instance.widthScale = !lockWidthToHeight ? GetTreeWidth() : instance.heightScale;
                        instance.rotation = GetTreeRotation();
                        sourceArray[num2++] = instance;
                    }
                }
                if (keepExistingTrees)
                {
                    TreeInstance[] treeInstances = terrainData.treeInstances;
                    TreeInstance[] destinationArray = new TreeInstance[treeInstances.Length + sourceArray.Length];
                    Array.Copy(treeInstances, 0, destinationArray, 0, treeInstances.Length);
                    Array.Copy(sourceArray, 0, destinationArray, treeInstances.Length, sourceArray.Length);
                    sourceArray = destinationArray;
                }
                terrainData.treeInstances = sourceArray;
                terrainData.RecalculateTreePositions();
            }
        }

        public static void PlaceTrees(Terrain terrain, float xBase, float yBase)
        {
            int prototypeCount = TerrainInspectorUtil.GetPrototypeCount(terrain.terrainData);
            if (((selectedTree != -1) && (selectedTree < prototypeCount)) && TerrainInspectorUtil.PrototypeIsRenderable(terrain.terrainData, selectedTree))
            {
                TreeInstance instance;
                int num2 = 0;
                instance = new TreeInstance {
                    position = new Vector3(xBase, 0f, yBase),
                    color = GetTreeColor(),
                    lightmapColor = Color.white,
                    prototypeIndex = selectedTree,
                    heightScale = GetTreeHeight(),
                    widthScale = !lockWidthToHeight ? GetTreeWidth() : instance.heightScale,
                    rotation = GetTreeRotation()
                };
                if (((Event.current.type != EventType.MouseDrag) && (brushSize <= 1f)) || TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, instance.position, instance.prototypeIndex, spacing))
                {
                    terrain.AddTreeInstance(instance);
                    num2++;
                }
                Vector3 prototypeExtent = TerrainInspectorUtil.GetPrototypeExtent(terrain.terrainData, selectedTree);
                prototypeExtent.y = 0f;
                float num3 = brushSize / ((prototypeExtent.magnitude * spacing) * 0.5f);
                int num4 = (int) ((num3 * num3) * 0.5f);
                num4 = Mathf.Clamp(num4, 0, 100);
                for (int i = 1; (i < num4) && (num2 < num4); i++)
                {
                    Vector2 insideUnitCircle = Random.insideUnitCircle;
                    insideUnitCircle.x *= brushSize / terrain.terrainData.size.x;
                    insideUnitCircle.y *= brushSize / terrain.terrainData.size.z;
                    Vector3 position = new Vector3(xBase + insideUnitCircle.x, 0f, yBase + insideUnitCircle.y);
                    if ((((position.x >= 0f) && (position.x <= 1f)) && ((position.z >= 0f) && (position.z <= 1f))) && TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, position, selectedTree, spacing * 0.5f))
                    {
                        instance = new TreeInstance {
                            position = position,
                            color = GetTreeColor(),
                            lightmapColor = Color.white,
                            prototypeIndex = selectedTree,
                            heightScale = GetTreeHeight(),
                            widthScale = !lockWidthToHeight ? GetTreeWidth() : instance.heightScale,
                            rotation = GetTreeRotation()
                        };
                        terrain.AddTreeInstance(instance);
                        num2++;
                    }
                }
            }
        }

        public static void RemoveTrees(Terrain terrain, float xBase, float yBase, bool clearSelectedOnly)
        {
            float radius = brushSize / terrain.terrainData.size.x;
            terrain.RemoveTrees(new Vector2(xBase, yBase), radius, !clearSelectedOnly ? -1 : selectedTree);
        }
    }
}

