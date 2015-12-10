namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainTreeContextMenus
    {
        [MenuItem("CONTEXT/TerrainEngineTrees/Add Tree")]
        internal static void AddTree(MenuCommand item)
        {
            TerrainWizard.DisplayTerrainWizard<TreeWizard>("Add Tree", "Add").InitializeDefaults((Terrain) item.context, -1);
        }

        [MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree")]
        internal static void EditTree(MenuCommand item)
        {
            TerrainWizard.DisplayTerrainWizard<TreeWizard>("Edit Tree", "Apply").InitializeDefaults((Terrain) item.context, item.userData);
        }

        [MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree", true)]
        internal static bool EditTreeCheck(MenuCommand item)
        {
            return (TreePainter.selectedTree >= 0);
        }

        [MenuItem("CONTEXT/TerrainEngineTrees/Remove Tree")]
        internal static void RemoveTree(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            TerrainEditorUtility.RemoveTree(context, item.userData);
        }

        [MenuItem("CONTEXT/TerrainEngineTrees/Remove Tree", true)]
        internal static bool RemoveTreeCheck(MenuCommand item)
        {
            return (TreePainter.selectedTree >= 0);
        }
    }
}

