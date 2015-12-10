namespace UnityEditor
{
    using System;

    internal class FlattenHeightmap : TerrainWizard
    {
        public float height;

        private void OnWizardCreate()
        {
            Undo.RegisterCompleteObjectUndo(base.terrainData, "Flatten Heightmap");
            HeightmapFilters.Flatten(base.terrainData, this.height / base.terrainData.size.y);
        }

        internal override void OnWizardUpdate()
        {
            if (base.terrainData != null)
            {
                object[] objArray1 = new object[] { this.height, " meters (", (this.height / base.terrainData.size.y) * 100f, "%)" };
                base.helpString = string.Concat(objArray1);
            }
        }
    }
}

