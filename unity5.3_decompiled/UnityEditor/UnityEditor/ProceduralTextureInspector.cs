namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(ProceduralTexture))]
    internal class ProceduralTextureInspector : TextureInspector
    {
        private bool m_MightHaveModified;

        protected override void OnDisable()
        {
            base.OnDisable();
            if ((!EditorApplication.isPlaying && !InternalEditorUtility.ignoreInspectorChanges) && this.m_MightHaveModified)
            {
                this.m_MightHaveModified = false;
                string[] strArray = new string[base.targets.GetLength(0)];
                int num = 0;
                foreach (ProceduralTexture texture in base.targets)
                {
                    SubstanceImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as SubstanceImporter;
                    if (atPath != null)
                    {
                        atPath.OnTextureInformationsChanged(texture);
                    }
                    string assetPath = AssetDatabase.GetAssetPath(texture.GetProceduralMaterial());
                    bool flag = false;
                    for (int j = 0; j < num; j++)
                    {
                        if (strArray[j] == assetPath)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        strArray[num++] = assetPath;
                    }
                }
                for (int i = 0; i < num; i++)
                {
                    SubstanceImporter importer2 = AssetImporter.GetAtPath(strArray[i]) as SubstanceImporter;
                    if ((importer2 != null) && EditorUtility.IsDirty(importer2.GetInstanceID()))
                    {
                        AssetDatabase.ImportAsset(strArray[i], ImportAssetOptions.ForceUncompressedImport);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUI.changed)
            {
                this.m_MightHaveModified = true;
            }
            foreach (ProceduralTexture texture in base.targets)
            {
                if (texture != null)
                {
                    ProceduralMaterial proceduralMaterial = texture.GetProceduralMaterial();
                    if ((proceduralMaterial != null) && proceduralMaterial.isProcessing)
                    {
                        base.Repaint();
                        SceneView.RepaintAll();
                        GameView.RepaintAll();
                        break;
                    }
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            if (this.target != null)
            {
                ProceduralMaterial proceduralMaterial = (this.target as ProceduralTexture).GetProceduralMaterial();
                if (((proceduralMaterial != null) && ProceduralMaterialInspector.ShowIsGenerating(proceduralMaterial)) && (r.width > 50f))
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Generating...");
                }
            }
        }
    }
}

