namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainWizard : ScriptableWizard
    {
        internal const int kMaxResolution = 0x1001;
        protected Terrain m_Terrain;

        internal static T DisplayTerrainWizard<T>(string title, string button) where T: TerrainWizard
        {
            T[] localArray = Resources.FindObjectsOfTypeAll<T>();
            if (localArray.Length > 0)
            {
                T local = localArray[0];
                local.titleContent = EditorGUIUtility.TextContent(title);
                local.createButtonName = button;
                local.otherButtonName = string.Empty;
                local.Focus();
                return local;
            }
            return ScriptableWizard.DisplayWizard<T>(title, button);
        }

        internal void FlushHeightmapModification()
        {
            this.m_Terrain.Flush();
        }

        internal void InitializeDefaults(Terrain terrain)
        {
            this.m_Terrain = terrain;
            this.OnWizardUpdate();
        }

        internal virtual void OnWizardUpdate()
        {
            base.isValid = true;
            base.errorString = string.Empty;
            if ((this.m_Terrain == null) || (this.m_Terrain.terrainData == null))
            {
                base.isValid = false;
                base.errorString = "Terrain does not exist";
            }
        }

        protected TerrainData terrainData
        {
            get
            {
                if (this.m_Terrain != null)
                {
                    return this.m_Terrain.terrainData;
                }
                return null;
            }
        }
    }
}

