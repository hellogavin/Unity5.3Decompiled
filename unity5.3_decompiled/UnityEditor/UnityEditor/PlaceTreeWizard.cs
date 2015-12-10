namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PlaceTreeWizard : TerrainWizard
    {
        public bool keepExistingTrees = true;
        private const int kMaxNumberOfTrees = 0xf4240;
        public int numberOfTrees = 0x2710;

        public void OnEnable()
        {
            base.minSize = new Vector2(250f, 150f);
        }

        private void OnWizardCreate()
        {
            if (this.numberOfTrees > 0xf4240)
            {
                base.isValid = false;
                base.errorString = string.Format("Mass placing more than {0} trees is not supported", 0xf4240);
                Debug.LogError(base.errorString);
            }
            else
            {
                TreePainter.MassPlaceTrees(base.m_Terrain.terrainData, this.numberOfTrees, true, this.keepExistingTrees);
                base.m_Terrain.Flush();
            }
        }
    }
}

