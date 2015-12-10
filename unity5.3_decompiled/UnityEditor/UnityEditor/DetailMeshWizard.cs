namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class DetailMeshWizard : TerrainWizard
    {
        public GameObject m_Detail;
        public Color m_DryColor;
        public Color m_HealthyColor;
        public float m_MaxHeight;
        public float m_MaxWidth;
        public float m_MinHeight;
        public float m_MinWidth;
        public float m_NoiseSpread;
        private int m_PrototypeIndex = -1;
        public DetailMeshRenderMode m_RenderMode;

        private void DoApply()
        {
            if (base.terrainData != null)
            {
                DetailPrototype[] detailPrototypes = base.m_Terrain.terrainData.detailPrototypes;
                if (this.m_PrototypeIndex == -1)
                {
                    DetailPrototype[] destinationArray = new DetailPrototype[detailPrototypes.Length + 1];
                    Array.Copy(detailPrototypes, 0, destinationArray, 0, detailPrototypes.Length);
                    this.m_PrototypeIndex = detailPrototypes.Length;
                    detailPrototypes = destinationArray;
                    detailPrototypes[this.m_PrototypeIndex] = new DetailPrototype();
                }
                detailPrototypes[this.m_PrototypeIndex].renderMode = DetailRenderMode.VertexLit;
                detailPrototypes[this.m_PrototypeIndex].usePrototypeMesh = true;
                detailPrototypes[this.m_PrototypeIndex].prototype = this.m_Detail;
                detailPrototypes[this.m_PrototypeIndex].prototypeTexture = null;
                detailPrototypes[this.m_PrototypeIndex].noiseSpread = this.m_NoiseSpread;
                detailPrototypes[this.m_PrototypeIndex].minWidth = this.m_MinWidth;
                detailPrototypes[this.m_PrototypeIndex].maxWidth = this.m_MaxWidth;
                detailPrototypes[this.m_PrototypeIndex].minHeight = this.m_MinHeight;
                detailPrototypes[this.m_PrototypeIndex].maxHeight = this.m_MaxHeight;
                detailPrototypes[this.m_PrototypeIndex].healthyColor = this.m_HealthyColor;
                detailPrototypes[this.m_PrototypeIndex].dryColor = this.m_DryColor;
                if (this.m_RenderMode == DetailMeshRenderMode.Grass)
                {
                    detailPrototypes[this.m_PrototypeIndex].renderMode = DetailRenderMode.Grass;
                }
                else
                {
                    detailPrototypes[this.m_PrototypeIndex].renderMode = DetailRenderMode.VertexLit;
                }
                base.m_Terrain.terrainData.detailPrototypes = detailPrototypes;
                EditorUtility.SetDirty(base.m_Terrain);
            }
        }

        internal void InitializeDefaults(Terrain terrain, int index)
        {
            DetailPrototype prototype;
            base.m_Terrain = terrain;
            this.m_PrototypeIndex = index;
            if (this.m_PrototypeIndex == -1)
            {
                prototype = new DetailPrototype();
            }
            else
            {
                prototype = base.m_Terrain.terrainData.detailPrototypes[this.m_PrototypeIndex];
            }
            this.m_Detail = prototype.prototype;
            this.m_NoiseSpread = prototype.noiseSpread;
            this.m_MinWidth = prototype.minWidth;
            this.m_MaxWidth = prototype.maxWidth;
            this.m_MinHeight = prototype.minHeight;
            this.m_MaxHeight = prototype.maxHeight;
            this.m_HealthyColor = prototype.healthyColor;
            this.m_DryColor = prototype.dryColor;
            switch (prototype.renderMode)
            {
                case DetailRenderMode.GrassBillboard:
                    Debug.LogError("Detail meshes can't be rendered as billboards");
                    this.m_RenderMode = DetailMeshRenderMode.Grass;
                    break;

                case DetailRenderMode.VertexLit:
                    this.m_RenderMode = DetailMeshRenderMode.VertexLit;
                    break;

                case DetailRenderMode.Grass:
                    this.m_RenderMode = DetailMeshRenderMode.Grass;
                    break;
            }
            this.OnWizardUpdate();
        }

        public void OnEnable()
        {
            base.minSize = new Vector2(400f, 400f);
        }

        private void OnWizardCreate()
        {
            this.DoApply();
        }

        private void OnWizardOtherButton()
        {
            this.DoApply();
        }

        internal override void OnWizardUpdate()
        {
            base.OnWizardUpdate();
            if (this.m_Detail == null)
            {
                base.errorString = "Please assign a detail prefab";
                base.isValid = false;
            }
            else if (this.m_PrototypeIndex != -1)
            {
                this.DoApply();
            }
        }
    }
}

