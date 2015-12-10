namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(SubstanceArchive))]
    internal class SubstanceImporterInspector : Editor
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1A;
        private const int kMaxRows = 2;
        private const float kPreviewHeight = 76f;
        private const float kPreviewWidth = 60f;
        public int lightMode = 1;
        private EditorCache m_EditorCache;
        protected bool m_IsVisible;
        private Vector2 m_ListScroll = Vector2.zero;
        private Editor m_MaterialInspector;
        private PreviewRenderUtility m_PreviewUtility;
        [NonSerialized]
        private string[] m_PrototypeNames;
        private string m_SelectedMaterialInstanceName;
        private SubstanceStyles m_SubstanceStyles;
        public Vector2 previewDir = new Vector2(0f, -20f);
        private static int previewNoDragDropHash = "PreviewWithoutDragAndDrop".GetHashCode();
        private static string s_CachedSelectedMaterialInstanceName = null;
        private static SubstanceArchive s_LastSelectedPackage = null;
        private static GUIContent[] s_LightIcons = new GUIContent[2];
        private static Mesh[] s_Meshes = new Mesh[4];
        private static GUIContent[] s_MeshIcons = new GUIContent[4];
        public int selectedMesh;

        private void ApplyAndRefresh(bool exitGUI)
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this.target), ImportAssetOptions.ForceUncompressedImport);
            if (exitGUI)
            {
                GUIUtility.ExitGUI();
            }
            base.Repaint();
        }

        protected void DoRenderPreview(Object[] subAssets)
        {
            if ((this.m_PreviewUtility.m_RenderTexture.width > 0) && (this.m_PreviewUtility.m_RenderTexture.height > 0))
            {
                Color color;
                List<ProceduralMaterial> list = new List<ProceduralMaterial>();
                foreach (Object obj2 in subAssets)
                {
                    if (obj2 is ProceduralMaterial)
                    {
                        list.Add(obj2 as ProceduralMaterial);
                    }
                }
                int num2 = 1;
                int num3 = 1;
                while ((num3 * num3) < list.Count)
                {
                    num3++;
                }
                num2 = Mathf.CeilToInt(((float) list.Count) / ((float) num3));
                this.m_PreviewUtility.m_Camera.transform.position = (Vector3) ((-Vector3.forward * 5f) * num3);
                this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
                this.m_PreviewUtility.m_Camera.farClipPlane = (5 * num3) + 5f;
                this.m_PreviewUtility.m_Camera.nearClipPlane = (5 * num3) - 3f;
                if (this.lightMode == 0)
                {
                    this.m_PreviewUtility.m_Light[0].intensity = 1f;
                    this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
                    this.m_PreviewUtility.m_Light[1].intensity = 0f;
                    color = new Color(0.2f, 0.2f, 0.2f, 0f);
                }
                else
                {
                    this.m_PreviewUtility.m_Light[0].intensity = 1f;
                    this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
                    this.m_PreviewUtility.m_Light[1].intensity = 1f;
                    color = new Color(0.2f, 0.2f, 0.2f, 0f);
                }
                InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, color);
                for (int i = 0; i < list.Count; i++)
                {
                    ProceduralMaterial mat = list[i];
                    Vector3 pos = new Vector3((i % num3) - ((num3 - 1) * 0.5f), (-i / num3) + ((num2 - 1) * 0.5f), 0f);
                    pos = (Vector3) (pos * ((Mathf.Tan((this.m_PreviewUtility.m_Camera.fieldOfView * 0.5f) * 0.01745329f) * 5f) * 2f));
                    this.m_PreviewUtility.DrawMesh(s_Meshes[this.selectedMesh], pos, Quaternion.Euler(this.previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.previewDir.x, 0f), mat, 0);
                }
                bool fog = RenderSettings.fog;
                Unsupported.SetRenderSettingsUseFogNoDirty(false);
                this.m_PreviewUtility.m_Camera.Render();
                Unsupported.SetRenderSettingsUseFogNoDirty(fog);
                InternalEditorUtility.RemoveCustomLighting();
            }
        }

        private SubstanceImporter GetImporter()
        {
            return (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.target)) as SubstanceImporter);
        }

        public override string GetInfoString()
        {
            Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
            if (selectedMaterialInspector != null)
            {
                return (selectedMaterialInspector.targetTitle + "\n" + selectedMaterialInspector.GetInfoString());
            }
            return string.Empty;
        }

        private ProceduralMaterial GetSelectedMaterial()
        {
            if (this.GetImporter() != null)
            {
                ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
                if (this.m_SelectedMaterialInstanceName != null)
                {
                    return Array.Find<ProceduralMaterial>(sortedMaterials, element => element.name == this.m_SelectedMaterialInstanceName);
                }
                if (sortedMaterials.Length > 0)
                {
                    this.m_SelectedMaterialInstanceName = sortedMaterials[0].name;
                    return sortedMaterials[0];
                }
            }
            return null;
        }

        private Editor GetSelectedMaterialInspector()
        {
            ProceduralMaterial selectedMaterial = this.GetSelectedMaterial();
            if (((selectedMaterial == null) || (this.m_MaterialInspector == null)) || (this.m_MaterialInspector.target != selectedMaterial))
            {
                EditorGUI.EndEditingActiveTextField();
                Object.DestroyImmediate(this.m_MaterialInspector);
                this.m_MaterialInspector = null;
                if (selectedMaterial != null)
                {
                    this.m_MaterialInspector = Editor.CreateEditor(selectedMaterial);
                    if (!(this.m_MaterialInspector is ProceduralMaterialInspector) && (this.m_MaterialInspector != null))
                    {
                        if (selectedMaterial.shader != null)
                        {
                            Debug.LogError("The shader: '" + selectedMaterial.shader.name + "' is using a custom editor deriving from MaterialEditor, please derive from ShaderGUI instead. Only the ShaderGUI approach works with Procedural Materials. Search the docs for 'ShaderGUI'");
                        }
                        Object.DestroyImmediate(this.m_MaterialInspector);
                        this.m_MaterialInspector = Editor.CreateEditor(selectedMaterial, typeof(ProceduralMaterialInspector));
                    }
                    ((ProceduralMaterialInspector) this.m_MaterialInspector).DisableReimportOnDisable();
                }
            }
            return this.m_MaterialInspector;
        }

        private ProceduralMaterial[] GetSortedMaterials()
        {
            ProceduralMaterial[] materials = this.GetImporter().GetMaterials();
            Array.Sort(materials, new SubstanceNameComparer());
            return materials;
        }

        public override bool HasPreviewGUI()
        {
            return (this.GetSelectedMaterialInspector() != null);
        }

        private void Init()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
            }
            if (s_Meshes[0] == null)
            {
                GameObject obj2 = (GameObject) EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
                obj2.SetActive(false);
                IEnumerator enumerator = obj2.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        MeshFilter component = current.GetComponent<MeshFilter>();
                        string name = current.name;
                        if (name != null)
                        {
                            int num;
                            if (<>f__switch$map1A == null)
                            {
                                Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                                dictionary.Add("sphere", 0);
                                dictionary.Add("cube", 1);
                                dictionary.Add("cylinder", 2);
                                dictionary.Add("torus", 3);
                                <>f__switch$map1A = dictionary;
                            }
                            if (<>f__switch$map1A.TryGetValue(name, out num))
                            {
                                switch (num)
                                {
                                    case 0:
                                    {
                                        s_Meshes[0] = component.sharedMesh;
                                        continue;
                                    }
                                    case 1:
                                    {
                                        s_Meshes[1] = component.sharedMesh;
                                        continue;
                                    }
                                    case 2:
                                    {
                                        s_Meshes[2] = component.sharedMesh;
                                        continue;
                                    }
                                    case 3:
                                    {
                                        s_Meshes[3] = component.sharedMesh;
                                        continue;
                                    }
                                }
                            }
                        }
                        Debug.Log("Something is wrong, weird object found: " + current.name);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                s_MeshIcons[0] = EditorGUIUtility.IconContent("PreMatSphere");
                s_MeshIcons[1] = EditorGUIUtility.IconContent("PreMatCube");
                s_MeshIcons[2] = EditorGUIUtility.IconContent("PreMatCylinder");
                s_MeshIcons[3] = EditorGUIUtility.IconContent("PreMatTorus");
                s_LightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
                s_LightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
            }
        }

        public void InstanciatePrototype(object prototypeName)
        {
            this.m_SelectedMaterialInstanceName = this.GetImporter().InstantiateMaterial(prototypeName as string);
            this.ApplyAndRefresh(false);
        }

        private void MaterialListing()
        {
            ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
            foreach (ProceduralMaterial material in sortedMaterials)
            {
                if (material.isProcessing)
                {
                    base.Repaint();
                    SceneView.RepaintAll();
                    GameView.RepaintAll();
                    break;
                }
            }
            int length = sortedMaterials.Length;
            float num3 = ((GUIView.current.position.width - 16f) - 18f) - 2f;
            if ((num3 * 2f) < (length * 60f))
            {
                num3 -= 16f;
            }
            int num4 = Mathf.Max(1, Mathf.FloorToInt(num3 / 60f));
            int num5 = Mathf.CeilToInt(((float) length) / ((float) num4));
            Rect viewRect = new Rect(0f, 0f, num4 * 60f, num5 * 76f);
            Rect rect = GUILayoutUtility.GetRect(viewRect.width, Mathf.Clamp(viewRect.height, 76f, 152f) + 1f);
            Rect position = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 1f);
            GUI.Box(rect, GUIContent.none, this.m_SubstanceStyles.gridBackground);
            GUI.Box(position, GUIContent.none, this.m_SubstanceStyles.background);
            this.m_ListScroll = GUI.BeginScrollView(position, this.m_ListScroll, viewRect, false, false);
            if (this.m_EditorCache == null)
            {
                this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
            }
            for (int i = 0; i < sortedMaterials.Length; i++)
            {
                ProceduralMaterial target = sortedMaterials[i];
                if (target != null)
                {
                    float x = (i % num4) * 60f;
                    float y = (i / num4) * 76f;
                    Rect rect4 = new Rect(x, y, 60f, 76f);
                    bool on = target.name == this.m_SelectedMaterialInstanceName;
                    Event current = Event.current;
                    int controlID = GUIUtility.GetControlID(previewNoDragDropHash, FocusType.Native, rect4);
                    switch (current.GetTypeForControl(controlID))
                    {
                        case EventType.MouseDown:
                            if ((current.button == 0) && rect4.Contains(current.mousePosition))
                            {
                                if (current.clickCount == 1)
                                {
                                    this.m_SelectedMaterialInstanceName = target.name;
                                    current.Use();
                                }
                                else if (current.clickCount == 2)
                                {
                                    AssetDatabase.OpenAsset(target);
                                    GUIUtility.ExitGUI();
                                    current.Use();
                                }
                            }
                            break;

                        case EventType.Repaint:
                        {
                            Rect rect5 = rect4;
                            rect5.y = rect4.yMax - 16f;
                            rect5.height = 16f;
                            this.m_SubstanceStyles.resultsGridLabel.Draw(rect5, EditorGUIUtility.TempContent(target.name), false, false, on, on);
                            break;
                        }
                    }
                    rect4.height -= 16f;
                    this.m_EditorCache[target].OnPreviewGUI(rect4, this.m_SubstanceStyles.background);
                }
            }
            GUI.EndScrollView();
        }

        private void MaterialManagement()
        {
            SubstanceImporter importer = this.GetImporter();
            if (this.m_PrototypeNames == null)
            {
                this.m_PrototypeNames = importer.GetPrototypeNames();
            }
            ProceduralMaterial selectedMaterial = this.GetSelectedMaterial();
            GUILayout.BeginHorizontal(this.m_SubstanceStyles.toolbar, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            if (this.m_PrototypeNames.Length > 1)
            {
                Rect position = GUILayoutUtility.GetRect(this.m_SubstanceStyles.iconToolbarPlus, this.m_SubstanceStyles.toolbarDropDown);
                if (EditorGUI.ButtonMouseDown(position, this.m_SubstanceStyles.iconToolbarPlus, FocusType.Passive, this.m_SubstanceStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < this.m_PrototypeNames.Length; i++)
                    {
                        menu.AddItem(new GUIContent(this.m_PrototypeNames[i]), false, new GenericMenu.MenuFunction2(this.InstanciatePrototype), this.m_PrototypeNames[i]);
                    }
                    menu.DropDown(position);
                }
            }
            else if ((this.m_PrototypeNames.Length == 1) && GUILayout.Button(this.m_SubstanceStyles.iconToolbarPlus, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.m_SelectedMaterialInstanceName = this.GetImporter().InstantiateMaterial(this.m_PrototypeNames[0]);
                this.ApplyAndRefresh(true);
            }
            EditorGUI.BeginDisabledGroup(selectedMaterial == null);
            if (GUILayout.Button(this.m_SubstanceStyles.iconToolbarMinus, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]) && (this.GetSortedMaterials().Length > 1))
            {
                this.SelectNextMaterial();
                importer.DestroyMaterial(selectedMaterial);
                this.ApplyAndRefresh(true);
            }
            if (GUILayout.Button(this.m_SubstanceStyles.iconDuplicate, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]))
            {
                string str = importer.CloneMaterial(selectedMaterial);
                if (str != string.Empty)
                {
                    this.m_SelectedMaterialInstanceName = str;
                    this.ApplyAndRefresh(true);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        public void OnDisable()
        {
            if (this.m_EditorCache != null)
            {
                this.m_EditorCache.Dispose();
            }
            if (this.m_MaterialInspector != null)
            {
                ((ProceduralMaterialInspector) this.m_MaterialInspector).ReimportSubstancesIfNeeded();
                Object.DestroyImmediate(this.m_MaterialInspector);
            }
            s_CachedSelectedMaterialInstanceName = this.m_SelectedMaterialInstanceName;
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        public void OnEnable()
        {
            if (this.target == s_LastSelectedPackage)
            {
                this.m_SelectedMaterialInstanceName = s_CachedSelectedMaterialInstanceName;
            }
            else
            {
                s_LastSelectedPackage = this.target as SubstanceArchive;
            }
        }

        public override void OnInspectorGUI()
        {
            if (this.m_SubstanceStyles == null)
            {
                this.m_SubstanceStyles = new SubstanceStyles();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            this.MaterialListing();
            this.MaterialManagement();
            EditorGUILayout.EndVertical();
            Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
            if (selectedMaterialInspector != null)
            {
                selectedMaterialInspector.DrawHeader();
                selectedMaterialInspector.OnInspectorGUI();
            }
        }

        public override void OnPreviewGUI(Rect position, GUIStyle style)
        {
            Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
            if (selectedMaterialInspector != null)
            {
                selectedMaterialInspector.OnPreviewGUI(position, style);
            }
        }

        public override void OnPreviewSettings()
        {
            Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
            if (selectedMaterialInspector != null)
            {
                selectedMaterialInspector.OnPreviewSettings();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.Init();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview(subAssets);
            return this.m_PreviewUtility.EndStaticPreview();
        }

        private void SelectNextMaterial()
        {
            if (this.GetImporter() != null)
            {
                string name = null;
                ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
                for (int i = 0; i < sortedMaterials.Length; i++)
                {
                    if (sortedMaterials[i].name == this.m_SelectedMaterialInstanceName)
                    {
                        int index = Math.Min((int) (i + 1), (int) (sortedMaterials.Length - 1));
                        if (index == i)
                        {
                            index--;
                        }
                        if (index >= 0)
                        {
                            name = sortedMaterials[index].name;
                        }
                        break;
                    }
                }
                this.m_SelectedMaterialInstanceName = name;
            }
        }

        public class SubstanceNameComparer : IComparer
        {
            public int Compare(object o1, object o2)
            {
                Object obj2 = o1 as Object;
                Object obj3 = o2 as Object;
                return EditorUtility.NaturalCompare(obj2.name, obj3.name);
            }
        }

        private class SubstanceStyles
        {
            public GUIStyle background = "ObjectPickerBackground";
            public GUIStyle gridBackground = "TE NodeBackground";
            public GUIContent iconDuplicate = EditorGUIUtility.IconContent("TreeEditor.Duplicate", "Duplicate selected substance.");
            public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selected substance.");
            public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add substance from prototype.");
            public GUIStyle resultsGrid = "ObjectPickerResultsGrid";
            public GUIStyle resultsGridLabel = "ObjectPickerResultsGridLabel";
            public GUIStyle toolbar = "TE Toolbar";
            public GUIStyle toolbarButton = "TE toolbarbutton";
            public GUIStyle toolbarDropDown = "TE toolbarDropDown";
        }
    }
}

