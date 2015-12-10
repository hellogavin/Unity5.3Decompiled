namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainDetailContextMenus
    {
        [MenuItem("CONTEXT/TerrainEngineDetails/Add Detail Mesh")]
        internal static void AddDetailMesh(MenuCommand item)
        {
            DetailMeshWizard wizard = TerrainWizard.DisplayTerrainWizard<DetailMeshWizard>("Add Detail Mesh", "Add");
            wizard.m_Detail = null;
            wizard.InitializeDefaults((Terrain) item.context, -1);
        }

        [MenuItem("CONTEXT/TerrainEngineDetails/Add Grass Texture")]
        internal static void AddDetailTexture(MenuCommand item)
        {
            DetailTextureWizard wizard = TerrainWizard.DisplayTerrainWizard<DetailTextureWizard>("Add Grass Texture", "Add");
            wizard.m_DetailTexture = null;
            wizard.InitializeDefaults((Terrain) item.context, -1);
        }

        [MenuItem("CONTEXT/TerrainEngineDetails/Edit")]
        internal static void EditDetail(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            DetailPrototype prototype = context.terrainData.detailPrototypes[item.userData];
            if (prototype.usePrototypeMesh)
            {
                TerrainWizard.DisplayTerrainWizard<DetailMeshWizard>("Edit Detail Mesh", "Apply").InitializeDefaults((Terrain) item.context, item.userData);
            }
            else
            {
                TerrainWizard.DisplayTerrainWizard<DetailTextureWizard>("Edit Grass Texture", "Apply").InitializeDefaults((Terrain) item.context, item.userData);
            }
        }

        [MenuItem("CONTEXT/TerrainEngineDetails/Edit", true)]
        internal static bool EditDetailCheck(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            return ((item.userData >= 0) && (item.userData < context.terrainData.detailPrototypes.Length));
        }

        [MenuItem("CONTEXT/TerrainEngineDetails/Remove")]
        internal static void RemoveDetail(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            TerrainEditorUtility.RemoveDetail(context, item.userData);
        }

        [MenuItem("CONTEXT/TerrainEngineDetails/Remove", true)]
        internal static bool RemoveDetailCheck(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            return ((item.userData >= 0) && (item.userData < context.terrainData.detailPrototypes.Length));
        }
    }
}

