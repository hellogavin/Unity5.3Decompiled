namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TreeWizard : TerrainWizard
    {
        public float m_BendFactor;
        private bool m_IsValidTree;
        private int m_PrototypeIndex = -1;
        public GameObject m_Tree;

        private void DoApply()
        {
            if (base.terrainData != null)
            {
                TreePrototype[] treePrototypes = base.m_Terrain.terrainData.treePrototypes;
                if (this.m_PrototypeIndex == -1)
                {
                    TreePrototype[] prototypeArray2 = new TreePrototype[treePrototypes.Length + 1];
                    for (int i = 0; i < treePrototypes.Length; i++)
                    {
                        prototypeArray2[i] = treePrototypes[i];
                    }
                    prototypeArray2[treePrototypes.Length] = new TreePrototype();
                    prototypeArray2[treePrototypes.Length].prefab = this.m_Tree;
                    prototypeArray2[treePrototypes.Length].bendFactor = this.m_BendFactor;
                    this.m_PrototypeIndex = treePrototypes.Length;
                    base.m_Terrain.terrainData.treePrototypes = prototypeArray2;
                    TreePainter.selectedTree = this.m_PrototypeIndex;
                }
                else
                {
                    treePrototypes[this.m_PrototypeIndex].prefab = this.m_Tree;
                    treePrototypes[this.m_PrototypeIndex].bendFactor = this.m_BendFactor;
                    base.m_Terrain.terrainData.treePrototypes = treePrototypes;
                }
                base.m_Terrain.Flush();
                EditorUtility.SetDirty(base.m_Terrain);
            }
        }

        protected override bool DrawWizardGUI()
        {
            EditorGUI.BeginChangeCheck();
            bool allowSceneObjects = !EditorUtility.IsPersistent(base.m_Terrain.terrainData);
            this.m_Tree = (GameObject) EditorGUILayout.ObjectField("Tree Prefab", this.m_Tree, typeof(GameObject), allowSceneObjects, new GUILayoutOption[0]);
            if (!TerrainEditorUtility.IsLODTreePrototype(this.m_Tree))
            {
                this.m_BendFactor = EditorGUILayout.FloatField("Bend Factor", this.m_BendFactor, new GUILayoutOption[0]);
            }
            bool flag2 = EditorGUI.EndChangeCheck();
            if (flag2)
            {
                this.m_IsValidTree = IsValidTree(this.m_Tree, this.m_PrototypeIndex, base.m_Terrain);
            }
            return flag2;
        }

        internal void InitializeDefaults(Terrain terrain, int index)
        {
            base.m_Terrain = terrain;
            this.m_PrototypeIndex = index;
            if (this.m_PrototypeIndex == -1)
            {
                this.m_Tree = null;
                this.m_BendFactor = 0f;
            }
            else
            {
                this.m_Tree = base.m_Terrain.terrainData.treePrototypes[this.m_PrototypeIndex].prefab;
                this.m_BendFactor = base.m_Terrain.terrainData.treePrototypes[this.m_PrototypeIndex].bendFactor;
            }
            this.m_IsValidTree = IsValidTree(this.m_Tree, this.m_PrototypeIndex, terrain);
            this.OnWizardUpdate();
        }

        private static bool IsValidTree(GameObject tree, int prototypeIndex, Terrain terrain)
        {
            if (tree == null)
            {
                return false;
            }
            TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
            for (int i = 0; i < treePrototypes.Length; i++)
            {
                if ((i != prototypeIndex) && (treePrototypes[i].m_Prefab == tree))
                {
                    return false;
                }
            }
            return true;
        }

        public void OnEnable()
        {
            base.minSize = new Vector2(400f, 150f);
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
            if (this.m_Tree == null)
            {
                base.errorString = "Please assign a tree";
                base.isValid = false;
            }
            else if (!this.m_IsValidTree)
            {
                base.errorString = "Tree has already been selected as a prototype";
                base.isValid = false;
            }
            else if (this.m_PrototypeIndex != -1)
            {
                this.DoApply();
            }
        }
    }
}

