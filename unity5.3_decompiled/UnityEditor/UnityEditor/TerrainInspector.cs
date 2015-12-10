namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(Terrain))]
    internal class TerrainInspector : Editor
    {
        private const float kHeightmapBrushScale = 0.01f;
        private const float kMinBrushStrength = 0.001678493f;
        private List<ReflectionProbeBlendInfo> m_BlendInfoList = new List<ReflectionProbeBlendInfo>();
        private Brush m_CachedBrush;
        private GUIContent[] m_DetailContents;
        private SavedFloat m_DetailOpacity = new SavedFloat("TerrainDetailOpacity", 1f);
        private SavedFloat m_DetailStrength = new SavedFloat("TerrainDetailStrength", 0.8f);
        private bool m_LODTreePrototypePresent;
        private int m_SelectedBrush;
        private int m_SelectedDetail;
        private int m_SelectedSplat;
        private SavedInt m_SelectedTool = new SavedInt("TerrainSelectedTool", 0);
        private AnimBool m_ShowBuiltinSpecularSettings = new AnimBool();
        private AnimBool m_ShowCustomMaterialSettings = new AnimBool();
        private AnimBool m_ShowReflectionProbesGUI = new AnimBool();
        private SavedInt m_Size = new SavedInt("TerrainBrushSize", 0x19);
        private SavedFloat m_SplatAlpha = new SavedFloat("TerrainBrushSplatAlpha", 1f);
        private Texture2D[] m_SplatIcons;
        private SavedFloat m_Strength = new SavedFloat("TerrainBrushStrength", 0.5f);
        private SavedFloat m_TargetHeight = new SavedFloat("TerrainBrushTargetHeight", 0.2f);
        private Terrain m_Terrain;
        private GUIContent[] m_TreeContents;
        private static Texture2D[] s_BrushTextures = null;
        internal static PrefKey s_NextBrush = new PrefKey("Terrain/Next Brush", ".");
        internal static PrefKey s_NextTexture = new PrefKey("Terrain/Next Detail", "#.");
        internal static PrefKey s_PrevBrush = new PrefKey("Terrain/Previous Brush", ",");
        internal static PrefKey s_PrevTexture = new PrefKey("Terrain/Previous Detail", "#,");
        private static int s_TerrainEditorHash = "TerrainEditor".GetHashCode();
        internal static PrefKey[] s_ToolKeys = new PrefKey[] { new PrefKey("Terrain/Raise Height", "f1"), new PrefKey("Terrain/Set Height", "f2"), new PrefKey("Terrain/Smooth Height", "f3"), new PrefKey("Terrain/Texture Paint", "f4"), new PrefKey("Terrain/Tree Brush", "f5"), new PrefKey("Terrain/Detail Brush", "f6") };
        private static Styles styles;

        public static int AspectSelectionGrid(int selected, Texture[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(10f) };
            GUILayout.BeginVertical("box", options);
            int num = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                float num2 = (Screen.width - 20) / approxSize;
                int num3 = (int) Mathf.Ceil(((float) textures.Length) / num2);
                Rect aspectRect = GUILayoutUtility.GetAspectRect(num2 / ((float) num3));
                Event current = Event.current;
                if (((current.type == EventType.MouseDown) && (current.clickCount == 2)) && aspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                num = GUI.SelectionGrid(aspectRect, selected, textures, (Screen.width - 20) / approxSize, style);
            }
            else
            {
                GUILayout.Label(emptyString, new GUILayoutOption[0]);
            }
            GUILayout.EndVertical();
            return num;
        }

        public static int AspectSelectionGridImageAndText(int selected, GUIContent[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(10f) };
            EditorGUILayout.BeginVertical(GUIContent.none, EditorStyles.helpBox, options);
            int num = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                Rect position = GetBrushAspectRect(textures.Length, approxSize, 12);
                Event current = Event.current;
                if (((current.type == EventType.MouseDown) && (current.clickCount == 2)) && position.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                num = GUI.SelectionGrid(position, selected, textures, (int) Mathf.Ceil((float) ((Screen.width - 20) / approxSize)), style);
            }
            else
            {
                GUILayout.Label(emptyString, new GUILayoutOption[0]);
            }
            GUILayout.EndVertical();
            return num;
        }

        private void CheckKeys()
        {
            for (int i = 0; i < s_ToolKeys.Length; i++)
            {
                if (s_ToolKeys[i].activated)
                {
                    this.selectedTool = (TerrainTool) i;
                    base.Repaint();
                    Event.current.Use();
                }
            }
            if (s_PrevBrush.activated)
            {
                this.m_SelectedBrush--;
                if (this.m_SelectedBrush < 0)
                {
                    this.m_SelectedBrush = s_BrushTextures.Length - 1;
                }
                base.Repaint();
                Event.current.Use();
            }
            if (s_NextBrush.activated)
            {
                this.m_SelectedBrush++;
                if (this.m_SelectedBrush >= s_BrushTextures.Length)
                {
                    this.m_SelectedBrush = 0;
                }
                base.Repaint();
                Event.current.Use();
            }
            int num2 = 0;
            if (s_NextTexture.activated)
            {
                num2 = 1;
            }
            if (s_PrevTexture.activated)
            {
                num2 = -1;
            }
            if (num2 != 0)
            {
                switch (this.selectedTool)
                {
                    case TerrainTool.PaintTexture:
                        this.m_SelectedSplat = (int) Mathf.Repeat((float) (this.m_SelectedSplat + num2), (float) this.m_Terrain.terrainData.splatPrototypes.Length);
                        Event.current.Use();
                        base.Repaint();
                        break;

                    case TerrainTool.PlaceTree:
                        if (TreePainter.selectedTree < 0)
                        {
                            if ((num2 == -1) && (this.m_TreeContents.Length > 0))
                            {
                                TreePainter.selectedTree = this.m_TreeContents.Length - 1;
                            }
                            else if ((num2 == 1) && (this.m_TreeContents.Length > 0))
                            {
                                TreePainter.selectedTree = 0;
                            }
                        }
                        else
                        {
                            TreePainter.selectedTree = (int) Mathf.Repeat((float) (TreePainter.selectedTree + num2), (float) this.m_TreeContents.Length);
                        }
                        Event.current.Use();
                        base.Repaint();
                        return;

                    case TerrainTool.PaintDetail:
                        this.m_SelectedDetail = (int) Mathf.Repeat((float) (this.m_SelectedDetail + num2), (float) this.m_Terrain.terrainData.detailPrototypes.Length);
                        Event.current.Use();
                        base.Repaint();
                        break;
                }
            }
        }

        private void DisableProjector()
        {
            if (this.m_CachedBrush != null)
            {
                this.m_CachedBrush.GetPreviewProjector().enabled = false;
            }
        }

        private Brush GetActiveBrush(int size)
        {
            if (this.m_CachedBrush == null)
            {
                this.m_CachedBrush = new Brush();
            }
            this.m_CachedBrush.Load(s_BrushTextures[this.m_SelectedBrush], size);
            return this.m_CachedBrush;
        }

        private static Rect GetBrushAspectRect(int elementCount, int approxSize, int extraLineHeight)
        {
            int num = (int) Mathf.Ceil((float) ((Screen.width - 20) / approxSize));
            int num2 = elementCount / num;
            if ((elementCount % num) != 0)
            {
                num2++;
            }
            Rect aspectRect = GUILayoutUtility.GetAspectRect(((float) num) / ((float) num2));
            Rect rect = GUILayoutUtility.GetRect(10f, (float) (extraLineHeight * num2));
            aspectRect.height += rect.height;
            return aspectRect;
        }

        public bool HasFrameBounds()
        {
            return (this.m_Terrain != null);
        }

        private void Initialize()
        {
            this.m_Terrain = this.target as Terrain;
            if (s_BrushTextures == null)
            {
                this.LoadBrushIcons();
            }
        }

        private static string IntString(float p)
        {
            int num = (int) p;
            return num.ToString();
        }

        private bool IsBrushPreviewVisible()
        {
            Vector3 vector;
            Vector2 vector2;
            if (!this.IsModificationToolActive())
            {
                return false;
            }
            return this.Raycast(out vector2, out vector);
        }

        private bool IsModificationToolActive()
        {
            if (this.m_Terrain == null)
            {
                return false;
            }
            TerrainTool selectedTool = this.selectedTool;
            if (selectedTool == TerrainTool.TerrainSettings)
            {
                return false;
            }
            return ((selectedTool >= TerrainTool.PaintHeight) && (selectedTool < TerrainTool.TerrainToolCount));
        }

        private void LoadBrushIcons()
        {
            ArrayList list = new ArrayList();
            int num = 1;
            Texture texture = null;
            do
            {
                object[] objArray1 = new object[] { EditorResourcesUtility.brushesPath, "builtin_brush_", num, ".png" };
                texture = (Texture2D) EditorGUIUtility.Load(string.Concat(objArray1));
                if (texture != null)
                {
                    list.Add(texture);
                }
                num++;
            }
            while (texture != null);
            num = 0;
            do
            {
                texture = EditorGUIUtility.FindTexture("brush_" + num + ".png");
                if (texture != null)
                {
                    list.Add(texture);
                }
                num++;
            }
            while (texture != null);
            s_BrushTextures = list.ToArray(typeof(Texture2D)) as Texture2D[];
        }

        private void LoadDetailIcons()
        {
            DetailPrototype[] detailPrototypes = this.m_Terrain.terrainData.detailPrototypes;
            this.m_DetailContents = new GUIContent[detailPrototypes.Length];
            for (int i = 0; i < this.m_DetailContents.Length; i++)
            {
                this.m_DetailContents[i] = new GUIContent();
                if (detailPrototypes[i].usePrototypeMesh)
                {
                    Texture assetPreview = AssetPreview.GetAssetPreview(detailPrototypes[i].prototype);
                    if (assetPreview != null)
                    {
                        this.m_DetailContents[i].image = assetPreview;
                    }
                    if (detailPrototypes[i].prototype != null)
                    {
                        this.m_DetailContents[i].text = detailPrototypes[i].prototype.name;
                    }
                    else
                    {
                        this.m_DetailContents[i].text = "Missing";
                    }
                }
                else
                {
                    Texture prototypeTexture = detailPrototypes[i].prototypeTexture;
                    if (prototypeTexture != null)
                    {
                        this.m_DetailContents[i].image = prototypeTexture;
                    }
                    if (prototypeTexture != null)
                    {
                        this.m_DetailContents[i].text = prototypeTexture.name;
                    }
                    else
                    {
                        this.m_DetailContents[i].text = "Missing";
                    }
                }
            }
        }

        private void LoadSplatIcons()
        {
            SplatPrototype[] splatPrototypes = this.m_Terrain.terrainData.splatPrototypes;
            this.m_SplatIcons = new Texture2D[splatPrototypes.Length];
            for (int i = 0; i < this.m_SplatIcons.Length; i++)
            {
                Texture2D assetPreview = AssetPreview.GetAssetPreview(splatPrototypes[i].texture);
                if (assetPreview == null)
                {
                }
                this.m_SplatIcons[i] = splatPrototypes[i].texture;
            }
        }

        private void LoadTreeIcons()
        {
            TreePrototype[] treePrototypes = this.m_Terrain.terrainData.treePrototypes;
            this.m_TreeContents = new GUIContent[treePrototypes.Length];
            for (int i = 0; i < this.m_TreeContents.Length; i++)
            {
                this.m_TreeContents[i] = new GUIContent();
                Texture assetPreview = AssetPreview.GetAssetPreview(treePrototypes[i].prefab);
                if (assetPreview != null)
                {
                    this.m_TreeContents[i].image = assetPreview;
                }
                if (treePrototypes[i].prefab != null)
                {
                    this.m_TreeContents[i].text = treePrototypes[i].prefab.name;
                }
                else
                {
                    this.m_TreeContents[i].text = "Missing";
                }
            }
        }

        public void MenuButton(GUIContent title, string menuName, int userData)
        {
            GUIContent content = new GUIContent(title.text, styles.settingsIcon, title.tooltip);
            Rect position = GUILayoutUtility.GetRect(content, styles.largeSquare);
            if (GUI.Button(position, content, styles.largeSquare))
            {
                MenuCommand command = new MenuCommand(this.m_Terrain, userData);
                EditorUtility.DisplayPopupMenu(new Rect(position.x, position.y, 0f, 0f), menuName, command);
            }
        }

        public void OnDisable()
        {
            this.m_ShowReflectionProbesGUI.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowCustomMaterialSettings.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowBuiltinSpecularSettings.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            if (this.m_CachedBrush != null)
            {
                this.m_CachedBrush.Dispose();
            }
        }

        public void OnEnable()
        {
            this.m_ShowBuiltinSpecularSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCustomMaterialSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowReflectionProbesGUI.valueChanged.AddListener(new UnityAction(this.Repaint));
            Terrain target = this.target as Terrain;
            if (target != null)
            {
                this.m_ShowBuiltinSpecularSettings.value = target.materialType == Terrain.MaterialType.BuiltInLegacySpecular;
                this.m_ShowCustomMaterialSettings.value = target.materialType == Terrain.MaterialType.Custom;
                this.m_ShowReflectionProbesGUI.value = (target.materialType == Terrain.MaterialType.BuiltInStandard) || (target.materialType == Terrain.MaterialType.Custom);
            }
        }

        public Bounds OnGetFrameBounds()
        {
            Vector2 vector;
            Vector3 vector2;
            if ((Camera.current != null) && this.Raycast(out vector, out vector2))
            {
                Vector3 vector3;
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.viewIsLockedToObject = false;
                }
                Bounds bounds = new Bounds();
                float num = (this.selectedTool != TerrainTool.PlaceTree) ? ((float) this.m_Size) : TreePainter.brushSize;
                vector3.x = (num / ((float) this.m_Terrain.terrainData.heightmapWidth)) * this.m_Terrain.terrainData.size.x;
                vector3.z = (num / ((float) this.m_Terrain.terrainData.heightmapHeight)) * this.m_Terrain.terrainData.size.z;
                vector3.y = (vector3.x + vector3.z) * 0.5f;
                bounds.center = vector2;
                bounds.size = vector3;
                if ((this.selectedTool == TerrainTool.PaintDetail) && (this.m_Terrain.terrainData.detailWidth != 0))
                {
                    vector3.x = ((num / ((float) this.m_Terrain.terrainData.detailWidth)) * this.m_Terrain.terrainData.size.x) * 0.7f;
                    vector3.z = ((num / ((float) this.m_Terrain.terrainData.detailHeight)) * this.m_Terrain.terrainData.size.z) * 0.7f;
                    vector3.y = 0f;
                    bounds.size = vector3;
                }
                return bounds;
            }
            Vector3 position = this.m_Terrain.transform.position;
            Vector3 size = this.m_Terrain.terrainData.size;
            float[,] numArray = this.m_Terrain.terrainData.GetHeights(0, 0, this.m_Terrain.terrainData.heightmapWidth, this.m_Terrain.terrainData.heightmapHeight);
            float minValue = float.MinValue;
            for (int i = 0; i < this.m_Terrain.terrainData.heightmapHeight; i++)
            {
                for (int j = 0; j < this.m_Terrain.terrainData.heightmapWidth; j++)
                {
                    minValue = Mathf.Max(minValue, numArray[j, i]);
                }
            }
            size.y = minValue * size.y;
            return new Bounds(position + ((Vector3) (size * 0.5f)), size);
        }

        public override void OnInspectorGUI()
        {
            this.Initialize();
            if (styles == null)
            {
                styles = new Styles();
            }
            if (this.m_Terrain.terrainData == null)
            {
                GUI.enabled = false;
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Toolbar(-1, styles.toolIcons, styles.command, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label("Terrain Asset Missing", new GUILayoutOption[0]);
                this.m_Terrain.terrainData = EditorGUILayout.ObjectField("Assign:", this.m_Terrain.terrainData, typeof(TerrainData), false, new GUILayoutOption[0]) as TerrainData;
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUI.changed = false;
                int selectedTool = (int) this.selectedTool;
                this.selectedTool = (TerrainTool) GUILayout.Toolbar(selectedTool, styles.toolIcons, styles.command, new GUILayoutOption[0]);
                if ((this.selectedTool != selectedTool) && (Toolbar.get != null))
                {
                    Toolbar.get.Repaint();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                this.CheckKeys();
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                if ((selectedTool >= 0) && (selectedTool < styles.toolIcons.Length))
                {
                    GUILayout.Label(styles.toolNames[selectedTool].text, new GUILayoutOption[0]);
                    GUILayout.Label(styles.toolNames[selectedTool].tooltip, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                }
                else
                {
                    GUILayout.Label("No tool selected", new GUILayoutOption[0]);
                    GUILayout.Label("Please select a tool", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                }
                GUILayout.EndVertical();
                switch (((TerrainTool) selectedTool))
                {
                    case TerrainTool.PaintHeight:
                        this.ShowRaiseHeight();
                        break;

                    case TerrainTool.SetHeight:
                        this.ShowSetHeight();
                        break;

                    case TerrainTool.SmoothHeight:
                        this.ShowSmoothHeight();
                        break;

                    case TerrainTool.PaintTexture:
                        this.ShowTextures();
                        break;

                    case TerrainTool.PlaceTree:
                        this.ShowTrees();
                        break;

                    case TerrainTool.PaintDetail:
                        this.ShowDetails();
                        break;

                    case TerrainTool.TerrainSettings:
                        this.ShowSettings();
                        break;
                }
                GUI.changed = false;
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(this);
                }
                GUILayout.Space(5f);
            }
        }

        private void OnInspectorUpdate()
        {
            if (AssetPreview.HasAnyNewPreviewTexturesAvailable())
            {
                base.Repaint();
            }
        }

        public void OnPreSceneGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.UpdatePreviewBrush();
            }
        }

        public void OnSceneGUI()
        {
            this.Initialize();
            if (this.m_Terrain.terrainData != null)
            {
                Event current = Event.current;
                this.CheckKeys();
                int controlID = GUIUtility.GetControlID(s_TerrainEditorHash, FocusType.Passive);
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                    case EventType.MouseDrag:
                        if ((GUIUtility.hotControl == 0) || (GUIUtility.hotControl == controlID))
                        {
                            Vector2 vector;
                            Vector3 vector2;
                            if ((current.GetTypeForControl(controlID) == EventType.MouseDrag) && (GUIUtility.hotControl != controlID))
                            {
                                return;
                            }
                            if (Event.current.alt)
                            {
                                return;
                            }
                            if (current.button != 0)
                            {
                                return;
                            }
                            if (!this.IsModificationToolActive())
                            {
                                return;
                            }
                            if (HandleUtility.nearestControl != controlID)
                            {
                                return;
                            }
                            if (current.type == EventType.MouseDown)
                            {
                                GUIUtility.hotControl = controlID;
                            }
                            if (this.Raycast(out vector, out vector2))
                            {
                                if ((this.selectedTool == TerrainTool.SetHeight) && Event.current.shift)
                                {
                                    this.m_TargetHeight.value = this.m_Terrain.terrainData.GetInterpolatedHeight(vector.x, vector.y) / this.m_Terrain.terrainData.size.y;
                                    InspectorWindow.RepaintAllInspectors();
                                }
                                else if (this.selectedTool == TerrainTool.PlaceTree)
                                {
                                    if (current.type == EventType.MouseDown)
                                    {
                                        Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Place Tree");
                                    }
                                    if (!Event.current.shift && !Event.current.control)
                                    {
                                        TreePainter.PlaceTrees(this.m_Terrain, vector.x, vector.y);
                                    }
                                    else
                                    {
                                        TreePainter.RemoveTrees(this.m_Terrain, vector.x, vector.y, Event.current.control);
                                    }
                                }
                                else if (this.selectedTool == TerrainTool.PaintTexture)
                                {
                                    SplatPainter painter;
                                    if (current.type == EventType.MouseDown)
                                    {
                                        List<Object> list = new List<Object> {
                                            this.m_Terrain.terrainData
                                        };
                                        list.AddRange(this.m_Terrain.terrainData.alphamapTextures);
                                        Undo.RegisterCompleteObjectUndo(list.ToArray(), "Detail Edit");
                                    }
                                    painter = new SplatPainter {
                                        size = (int) this.m_Size,
                                        strength = (float) this.m_Strength,
                                        terrainData = this.m_Terrain.terrainData,
                                        brush = this.GetActiveBrush(painter.size),
                                        target = (float) this.m_SplatAlpha,
                                        tool = this.selectedTool
                                    };
                                    this.m_Terrain.editorRenderFlags = TerrainRenderFlags.heightmap;
                                    painter.Paint(vector.x, vector.y, this.m_SelectedSplat);
                                    this.m_Terrain.terrainData.SetBasemapDirty(false);
                                }
                                else if (this.selectedTool == TerrainTool.PaintDetail)
                                {
                                    if (current.type == EventType.MouseDown)
                                    {
                                        Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Detail Edit");
                                    }
                                    DetailPainter painter2 = new DetailPainter {
                                        size = (int) this.m_Size,
                                        targetStrength = this.m_DetailStrength * 16f,
                                        opacity = (float) this.m_DetailOpacity
                                    };
                                    if (Event.current.shift || Event.current.control)
                                    {
                                        painter2.targetStrength *= -1f;
                                    }
                                    painter2.clearSelectedOnly = Event.current.control;
                                    painter2.terrainData = this.m_Terrain.terrainData;
                                    painter2.brush = this.GetActiveBrush(painter2.size);
                                    painter2.tool = this.selectedTool;
                                    painter2.randomizeDetails = true;
                                    painter2.Paint(vector.x, vector.y, this.m_SelectedDetail);
                                }
                                else
                                {
                                    if (current.type == EventType.MouseDown)
                                    {
                                        Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Heightmap Edit");
                                    }
                                    HeightmapPainter painter3 = new HeightmapPainter {
                                        size = (int) this.m_Size,
                                        strength = this.m_Strength * 0.01f
                                    };
                                    if (this.selectedTool == TerrainTool.SmoothHeight)
                                    {
                                        painter3.strength = (float) this.m_Strength;
                                    }
                                    painter3.terrainData = this.m_Terrain.terrainData;
                                    painter3.brush = this.GetActiveBrush((int) this.m_Size);
                                    painter3.targetHeight = (float) this.m_TargetHeight;
                                    painter3.tool = this.selectedTool;
                                    this.m_Terrain.editorRenderFlags = TerrainRenderFlags.heightmap;
                                    if ((this.selectedTool == TerrainTool.PaintHeight) && Event.current.shift)
                                    {
                                        painter3.strength = -painter3.strength;
                                    }
                                    painter3.PaintHeight(vector.x, vector.y);
                                }
                                current.Use();
                            }
                            break;
                        }
                        return;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            if (!this.IsModificationToolActive())
                            {
                                return;
                            }
                            if (this.selectedTool == TerrainTool.PaintTexture)
                            {
                                this.m_Terrain.terrainData.SetBasemapDirty(true);
                            }
                            this.m_Terrain.editorRenderFlags = TerrainRenderFlags.all;
                            this.m_Terrain.ApplyDelayedHeightmapModification();
                            current.Use();
                            break;
                        }
                        return;

                    case EventType.MouseMove:
                        if (this.IsBrushPreviewVisible())
                        {
                            HandleUtility.Repaint();
                        }
                        break;

                    case EventType.Repaint:
                        this.DisableProjector();
                        break;

                    case EventType.Layout:
                        if (this.IsModificationToolActive())
                        {
                            HandleUtility.AddDefaultControl(controlID);
                            break;
                        }
                        return;
                }
            }
        }

        private static float PercentSlider(GUIContent content, float valueInPercent, float minVal, float maxVal)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            float num = EditorGUILayout.Slider(content, Mathf.Round(valueInPercent * 100f), minVal * 100f, maxVal * 100f, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                return (num / 100f);
            }
            GUI.changed = changed;
            return valueInPercent;
        }

        public bool Raycast(out Vector2 uv, out Vector3 pos)
        {
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (this.m_Terrain.GetComponent<Collider>().Raycast(ray, out hit, float.PositiveInfinity))
            {
                uv = hit.textureCoord;
                pos = hit.point;
                return true;
            }
            uv = Vector2.zero;
            pos = Vector3.zero;
            return false;
        }

        private void ResizeDetailResolution(TerrainData terrainData, int resolution, int resolutionPerPatch)
        {
            if (resolution == terrainData.detailResolution)
            {
                List<int[,]> list = new List<int[,]>();
                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    list.Add(terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i));
                }
                terrainData.SetDetailResolution(resolution, resolutionPerPatch);
                for (int j = 0; j < list.Count; j++)
                {
                    terrainData.SetDetailLayer(0, 0, j, list[j]);
                }
            }
            else
            {
                terrainData.SetDetailResolution(resolution, resolutionPerPatch);
            }
        }

        public void ShowBrushes()
        {
            bool flag;
            GUILayout.Label(styles.brushes, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_SelectedBrush = AspectSelectionGrid(this.m_SelectedBrush, s_BrushTextures, 0x20, styles.gridList, "No brushes defined.", out flag);
        }

        public void ShowBrushSettings()
        {
            this.m_Size.value = Mathf.RoundToInt(EditorGUILayout.Slider(styles.brushSize, (float) this.m_Size, 1f, 100f, new GUILayoutOption[0]));
            this.m_Strength.value = PercentSlider(styles.opacity, (float) this.m_Strength, 0.001678493f, 1f);
        }

        public void ShowDetails()
        {
            bool flag;
            this.LoadDetailIcons();
            this.ShowBrushes();
            GUI.changed = false;
            GUILayout.Label(styles.details, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_SelectedDetail = AspectSelectionGridImageAndText(this.m_SelectedDetail, this.m_DetailContents, 0x40, styles.gridListText, "No Detail Objects defined", out flag);
            if (flag)
            {
                TerrainDetailContextMenus.EditDetail(new MenuCommand(this.m_Terrain, this.m_SelectedDetail));
                GUIUtility.ExitGUI();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            this.MenuButton(styles.editDetails, "CONTEXT/TerrainEngineDetails", this.m_SelectedDetail);
            this.ShowRefreshPrototypes();
            GUILayout.EndHorizontal();
            GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_Size.value = Mathf.RoundToInt(EditorGUILayout.Slider(styles.brushSize, (float) this.m_Size, 1f, 100f, new GUILayoutOption[0]));
            this.m_DetailOpacity.value = EditorGUILayout.Slider(styles.opacity, (float) this.m_DetailOpacity, 0f, 1f, new GUILayoutOption[0]);
            this.m_DetailStrength.value = EditorGUILayout.Slider(styles.detailTargetStrength, (float) this.m_DetailStrength, 0f, 1f, new GUILayoutOption[0]);
            this.m_DetailStrength.value = Mathf.Round(this.m_DetailStrength * 16f) / 16f;
        }

        public void ShowHeightmaps()
        {
            GUILayout.Label(styles.heightmap, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(styles.importRaw, new GUILayoutOption[0]))
            {
                TerrainMenus.ImportRaw();
            }
            if (GUILayout.Button(styles.exportRaw, new GUILayoutOption[0]))
            {
                TerrainMenus.ExportHeightmapRaw();
            }
            GUILayout.EndHorizontal();
        }

        public void ShowMassPlaceTrees()
        {
            EditorGUI.BeginDisabledGroup(TreePainter.selectedTree == -1);
            if (GUILayout.Button(styles.massPlaceTrees, new GUILayoutOption[0]))
            {
                TerrainMenus.MassPlaceTrees();
            }
            EditorGUI.EndDisabledGroup();
        }

        public void ShowRaiseHeight()
        {
            this.ShowBrushes();
            GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.ShowBrushSettings();
        }

        public void ShowRefreshPrototypes()
        {
            if (GUILayout.Button(styles.refresh, new GUILayoutOption[0]))
            {
                TerrainMenus.RefreshPrototypes();
            }
        }

        public void ShowResolution()
        {
            GUILayout.Label("Resolution", EditorStyles.boldLabel, new GUILayoutOption[0]);
            float x = this.m_Terrain.terrainData.size.x;
            float y = this.m_Terrain.terrainData.size.y;
            float z = this.m_Terrain.terrainData.size.z;
            int heightmapResolution = this.m_Terrain.terrainData.heightmapResolution;
            int detailResolution = this.m_Terrain.terrainData.detailResolution;
            int detailResolutionPerPatch = this.m_Terrain.terrainData.detailResolutionPerPatch;
            int alphamapResolution = this.m_Terrain.terrainData.alphamapResolution;
            int baseMapResolution = this.m_Terrain.terrainData.baseMapResolution;
            EditorGUI.BeginChangeCheck();
            x = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Width"), x, new GUILayoutOption[0]);
            if (x <= 0f)
            {
                x = 1f;
            }
            if (x > 100000f)
            {
                x = 100000f;
            }
            z = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Length"), z, new GUILayoutOption[0]);
            if (z <= 0f)
            {
                z = 1f;
            }
            if (z > 100000f)
            {
                z = 100000f;
            }
            y = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Height"), y, new GUILayoutOption[0]);
            if (y <= 0f)
            {
                y = 1f;
            }
            if (y > 10000f)
            {
                y = 10000f;
            }
            heightmapResolution = Mathf.Clamp(EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Heightmap Resolution"), heightmapResolution, new GUILayoutOption[0]), 0x21, 0x1001);
            heightmapResolution = this.m_Terrain.terrainData.GetAdjustedSize(heightmapResolution);
            detailResolution = Mathf.Clamp(EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Detail Resolution"), detailResolution, new GUILayoutOption[0]), 0, 0xfd0);
            detailResolutionPerPatch = Mathf.Clamp(EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Detail Resolution Per Patch"), detailResolutionPerPatch, new GUILayoutOption[0]), 8, 0x80);
            alphamapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Control Texture Resolution"), alphamapResolution, new GUILayoutOption[0])), 0x10, 0x800);
            baseMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Base Texture Resolution"), baseMapResolution, new GUILayoutOption[0])), 0x10, 0x800);
            if (EditorGUI.EndChangeCheck())
            {
                ArrayList list = new ArrayList();
                list.Add(this.m_Terrain.terrainData);
                list.AddRange(this.m_Terrain.terrainData.alphamapTextures);
                Undo.RegisterCompleteObjectUndo(list.ToArray(typeof(Object)) as Object[], "Set Resolution");
                if (this.m_Terrain.terrainData.heightmapResolution != heightmapResolution)
                {
                    this.m_Terrain.terrainData.heightmapResolution = heightmapResolution;
                }
                this.m_Terrain.terrainData.size = new Vector3(x, y, z);
                if ((this.m_Terrain.terrainData.detailResolution != detailResolution) || (detailResolutionPerPatch != this.m_Terrain.terrainData.detailResolutionPerPatch))
                {
                    this.ResizeDetailResolution(this.m_Terrain.terrainData, detailResolution, detailResolutionPerPatch);
                }
                if (this.m_Terrain.terrainData.alphamapResolution != alphamapResolution)
                {
                    this.m_Terrain.terrainData.alphamapResolution = alphamapResolution;
                }
                if (this.m_Terrain.terrainData.baseMapResolution != baseMapResolution)
                {
                    this.m_Terrain.terrainData.baseMapResolution = baseMapResolution;
                }
                this.m_Terrain.Flush();
            }
            EditorGUILayout.HelpBox("Please note that modifying the resolution of the heightmap, detail map and control texture will clear their contents, respectively.", MessageType.Warning);
        }

        public void ShowSetHeight()
        {
            this.ShowBrushes();
            GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.ShowBrushSettings();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.changed = false;
            float num = this.m_TargetHeight * this.m_Terrain.terrainData.size.y;
            num = EditorGUILayout.Slider("Height", num, 0f, this.m_Terrain.terrainData.size.y, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                this.m_TargetHeight.value = num / this.m_Terrain.terrainData.size.y;
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(styles.flatten, options))
            {
                Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Flatten Heightmap");
                HeightmapFilters.Flatten(this.m_Terrain.terrainData, this.m_TargetHeight.value);
            }
            GUILayout.EndHorizontal();
        }

        public void ShowSettings()
        {
            TerrainData terrainData = this.m_Terrain.terrainData;
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Base Terrain", EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_Terrain.drawHeightmap = EditorGUILayout.Toggle("Draw", this.m_Terrain.drawHeightmap, new GUILayoutOption[0]);
            this.m_Terrain.heightmapPixelError = EditorGUILayout.Slider("Pixel Error", this.m_Terrain.heightmapPixelError, 1f, 200f, new GUILayoutOption[0]);
            this.m_Terrain.basemapDistance = EditorGUILayout.Slider("Base Map Dist.", this.m_Terrain.basemapDistance, 0f, 2000f, new GUILayoutOption[0]);
            this.m_Terrain.castShadows = EditorGUILayout.Toggle("Cast Shadows", this.m_Terrain.castShadows, new GUILayoutOption[0]);
            this.m_Terrain.materialType = (Terrain.MaterialType) EditorGUILayout.EnumPopup("Material", this.m_Terrain.materialType, new GUILayoutOption[0]);
            if (this.m_Terrain.materialType != Terrain.MaterialType.Custom)
            {
                this.m_Terrain.materialTemplate = null;
            }
            this.m_ShowBuiltinSpecularSettings.target = this.m_Terrain.materialType == Terrain.MaterialType.BuiltInLegacySpecular;
            this.m_ShowCustomMaterialSettings.target = this.m_Terrain.materialType == Terrain.MaterialType.Custom;
            this.m_ShowReflectionProbesGUI.target = (this.m_Terrain.materialType == Terrain.MaterialType.BuiltInStandard) || (this.m_Terrain.materialType == Terrain.MaterialType.Custom);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBuiltinSpecularSettings.faded))
            {
                EditorGUI.indentLevel++;
                this.m_Terrain.legacySpecular = EditorGUILayout.ColorField("Specular Color", this.m_Terrain.legacySpecular, new GUILayoutOption[0]);
                this.m_Terrain.legacyShininess = EditorGUILayout.Slider("Shininess", this.m_Terrain.legacyShininess, 0.03f, 1f, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCustomMaterialSettings.faded))
            {
                EditorGUI.indentLevel++;
                this.m_Terrain.materialTemplate = EditorGUILayout.ObjectField("Custom Material", this.m_Terrain.materialTemplate, typeof(Material), false, new GUILayoutOption[0]) as Material;
                if ((this.m_Terrain.materialTemplate != null) && ShaderUtil.HasTangentChannel(this.m_Terrain.materialTemplate.shader))
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Can't use materials with shaders which need tangent geometry on terrain, use shaders in Nature/Terrain instead.").text, MessageType.Warning, false);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowReflectionProbesGUI.faded))
            {
                this.m_Terrain.reflectionProbeUsage = (ReflectionProbeUsage) EditorGUILayout.EnumPopup("Reflection Probes", this.m_Terrain.reflectionProbeUsage, new GUILayoutOption[0]);
                if (this.m_Terrain.reflectionProbeUsage != ReflectionProbeUsage.Off)
                {
                    EditorGUI.indentLevel++;
                    this.m_Terrain.GetClosestReflectionProbes(this.m_BlendInfoList);
                    RendererEditorBase.Probes.ShowClosestReflectionProbes(this.m_BlendInfoList);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndFadeGroup();
            terrainData.thickness = EditorGUILayout.FloatField("Thickness", terrainData.thickness, new GUILayoutOption[0]);
            GUILayout.Label("Tree & Detail Objects", EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_Terrain.drawTreesAndFoliage = EditorGUILayout.Toggle("Draw", this.m_Terrain.drawTreesAndFoliage, new GUILayoutOption[0]);
            this.m_Terrain.bakeLightProbesForTrees = EditorGUILayout.Toggle(styles.bakeLightProbesForTrees, this.m_Terrain.bakeLightProbesForTrees, new GUILayoutOption[0]);
            this.m_Terrain.detailObjectDistance = EditorGUILayout.Slider("Detail Distance", this.m_Terrain.detailObjectDistance, 0f, 250f, new GUILayoutOption[0]);
            this.m_Terrain.collectDetailPatches = EditorGUILayout.Toggle("Collect Detail Patches", this.m_Terrain.collectDetailPatches, new GUILayoutOption[0]);
            this.m_Terrain.detailObjectDensity = EditorGUILayout.Slider("Detail Density", this.m_Terrain.detailObjectDensity, 0f, 1f, new GUILayoutOption[0]);
            this.m_Terrain.treeDistance = EditorGUILayout.Slider("Tree Distance", this.m_Terrain.treeDistance, 0f, 2000f, new GUILayoutOption[0]);
            this.m_Terrain.treeBillboardDistance = EditorGUILayout.Slider("Billboard Start", this.m_Terrain.treeBillboardDistance, 5f, 2000f, new GUILayoutOption[0]);
            this.m_Terrain.treeCrossFadeLength = EditorGUILayout.Slider("Fade Length", this.m_Terrain.treeCrossFadeLength, 0f, 200f, new GUILayoutOption[0]);
            this.m_Terrain.treeMaximumFullLODCount = EditorGUILayout.IntSlider("Max Mesh Trees", this.m_Terrain.treeMaximumFullLODCount, 0, 0x2710, new GUILayoutOption[0]);
            if (Event.current.type == EventType.Layout)
            {
                this.m_LODTreePrototypePresent = false;
                for (int i = 0; i < terrainData.treePrototypes.Length; i++)
                {
                    if (TerrainEditorUtility.IsLODTreePrototype(terrainData.treePrototypes[i].prefab))
                    {
                        this.m_LODTreePrototypePresent = true;
                        break;
                    }
                }
            }
            if (this.m_LODTreePrototypePresent)
            {
                EditorGUILayout.HelpBox("Tree Distance, Billboard Start, Fade Length and Max Mesh Trees have no effect on SpeedTree trees. Please use the LOD Group component on the tree prefab to control LOD settings.", MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.SetSceneRepaintDirty();
                EditorUtility.SetDirty(this.m_Terrain);
            }
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Wind Settings for Grass", EditorStyles.boldLabel, new GUILayoutOption[0]);
            float num2 = EditorGUILayout.Slider("Speed", terrainData.wavingGrassStrength, 0f, 1f, new GUILayoutOption[0]);
            float num3 = EditorGUILayout.Slider("Size", terrainData.wavingGrassSpeed, 0f, 1f, new GUILayoutOption[0]);
            float num4 = EditorGUILayout.Slider("Bending", terrainData.wavingGrassAmount, 0f, 1f, new GUILayoutOption[0]);
            Color color = EditorGUILayout.ColorField("Grass Tint", terrainData.wavingGrassTint, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                terrainData.wavingGrassStrength = num2;
                terrainData.wavingGrassSpeed = num3;
                terrainData.wavingGrassAmount = num4;
                terrainData.wavingGrassTint = color;
            }
            this.ShowResolution();
            this.ShowHeightmaps();
        }

        public void ShowSmoothHeight()
        {
            this.ShowBrushes();
            GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.ShowBrushSettings();
        }

        public void ShowTextures()
        {
            bool flag;
            this.LoadSplatIcons();
            this.ShowBrushes();
            GUILayout.Label(styles.textures, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.changed = false;
            this.m_SelectedSplat = AspectSelectionGrid(this.m_SelectedSplat, this.m_SplatIcons, 0x40, styles.gridList, "No terrain textures defined.", out flag);
            if (flag)
            {
                TerrainSplatContextMenus.EditSplat(new MenuCommand(this.m_Terrain, this.m_SelectedSplat));
                GUIUtility.ExitGUI();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            this.MenuButton(styles.editTextures, "CONTEXT/TerrainEngineSplats", this.m_SelectedSplat);
            GUILayout.EndHorizontal();
            GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.ShowBrushSettings();
            this.m_SplatAlpha.value = EditorGUILayout.Slider("Target Strength", (float) this.m_SplatAlpha, 0f, 1f, new GUILayoutOption[0]);
        }

        public void ShowTrees()
        {
            bool flag;
            this.LoadTreeIcons();
            GUI.changed = false;
            this.ShowUpgradeTreePrototypeScaleUI();
            GUILayout.Label(styles.trees, EditorStyles.boldLabel, new GUILayoutOption[0]);
            TreePainter.selectedTree = AspectSelectionGridImageAndText(TreePainter.selectedTree, this.m_TreeContents, 0x40, styles.gridListText, "No trees defined", out flag);
            if (TreePainter.selectedTree >= this.m_TreeContents.Length)
            {
                TreePainter.selectedTree = -1;
            }
            if (flag)
            {
                TerrainTreeContextMenus.EditTree(new MenuCommand(this.m_Terrain, TreePainter.selectedTree));
                GUIUtility.ExitGUI();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.ShowMassPlaceTrees();
            GUILayout.FlexibleSpace();
            this.MenuButton(styles.editTrees, "CONTEXT/TerrainEngineTrees", TreePainter.selectedTree);
            this.ShowRefreshPrototypes();
            GUILayout.EndHorizontal();
            if (TreePainter.selectedTree != -1)
            {
                GUILayout.Label(styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
                TreePainter.brushSize = EditorGUILayout.Slider(styles.brushSize, TreePainter.brushSize, 1f, 100f, new GUILayoutOption[0]);
                float valueInPercent = (3.3f - TreePainter.spacing) / 3f;
                float num2 = PercentSlider(styles.treeDensity, valueInPercent, 0.1f, 1f);
                if (num2 != valueInPercent)
                {
                    TreePainter.spacing = (1.1f - num2) * 3f;
                }
                GUILayout.Space(5f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth - 6f) };
                GUILayout.Label(styles.treeHeight, options);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                GUILayout.Label(styles.treeHeightRandomLabel, optionArray2);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                TreePainter.allowHeightVar = GUILayout.Toggle(TreePainter.allowHeightVar, styles.treeHeightRandomToggle, optionArray3);
                if (TreePainter.allowHeightVar)
                {
                    EditorGUI.BeginChangeCheck();
                    float minValue = TreePainter.treeHeight * (1f - TreePainter.treeHeightVariation);
                    float maxValue = TreePainter.treeHeight * (1f + TreePainter.treeHeightVariation);
                    EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0.01f, 2f, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        TreePainter.treeHeight = (minValue + maxValue) * 0.5f;
                        TreePainter.treeHeightVariation = (maxValue - minValue) / (minValue + maxValue);
                    }
                }
                else
                {
                    TreePainter.treeHeight = EditorGUILayout.Slider(TreePainter.treeHeight, 0.01f, 2f, new GUILayoutOption[0]);
                    TreePainter.treeHeightVariation = 0f;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                TreePainter.lockWidthToHeight = EditorGUILayout.Toggle(styles.lockWidth, TreePainter.lockWidthToHeight, new GUILayoutOption[0]);
                if (TreePainter.lockWidthToHeight)
                {
                    TreePainter.treeWidth = TreePainter.treeHeight;
                    TreePainter.treeWidthVariation = TreePainter.treeHeightVariation;
                    TreePainter.allowWidthVar = TreePainter.allowHeightVar;
                }
                GUILayout.Space(5f);
                EditorGUI.BeginDisabledGroup(TreePainter.lockWidthToHeight);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth - 6f) };
                GUILayout.Label(styles.treeWidth, optionArray4);
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                GUILayout.Label(styles.treeWidthRandomLabel, optionArray5);
                GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                TreePainter.allowWidthVar = GUILayout.Toggle(TreePainter.allowWidthVar, styles.treeWidthRandomToggle, optionArray6);
                if (TreePainter.allowWidthVar)
                {
                    EditorGUI.BeginChangeCheck();
                    float num5 = TreePainter.treeWidth * (1f - TreePainter.treeWidthVariation);
                    float num6 = TreePainter.treeWidth * (1f + TreePainter.treeWidthVariation);
                    EditorGUILayout.MinMaxSlider(ref num5, ref num6, 0.01f, 2f, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        TreePainter.treeWidth = (num5 + num6) * 0.5f;
                        TreePainter.treeWidthVariation = (num6 - num5) / (num5 + num6);
                    }
                }
                else
                {
                    TreePainter.treeWidth = EditorGUILayout.Slider(TreePainter.treeWidth, 0.01f, 2f, new GUILayoutOption[0]);
                    TreePainter.treeWidthVariation = 0f;
                }
                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(5f);
                if (TerrainEditorUtility.IsLODTreePrototype(this.m_Terrain.terrainData.treePrototypes[TreePainter.selectedTree].m_Prefab))
                {
                    TreePainter.randomRotation = EditorGUILayout.Toggle(styles.treeRotation, TreePainter.randomRotation, new GUILayoutOption[0]);
                }
                else
                {
                    TreePainter.treeColorAdjustment = EditorGUILayout.Slider(styles.treeColorVar, TreePainter.treeColorAdjustment, 0f, 1f, new GUILayoutOption[0]);
                }
            }
        }

        public void ShowUpgradeTreePrototypeScaleUI()
        {
            if ((this.m_Terrain.terrainData != null) && this.m_Terrain.terrainData.NeedUpgradeScaledTreePrototypes())
            {
                GUIContent content = EditorGUIUtility.TempContent("Some of your prototypes have scaling values on the prefab. Since Unity 5.2 these scalings will be applied to terrain tree instances. Do you want to upgrade to this behaviour?", EditorGUIUtility.GetHelpIcon(MessageType.Warning));
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label(content, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                GUILayout.Space(3f);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button("Upgrade", options))
                {
                    this.m_Terrain.terrainData.UpgradeScaledTreePrototype();
                    TerrainMenus.RefreshPrototypes();
                }
                GUILayout.Space(3f);
                GUILayout.EndVertical();
            }
        }

        private void UpdatePreviewBrush()
        {
            if (!this.IsModificationToolActive() || (this.m_Terrain.terrainData == null))
            {
                this.DisableProjector();
            }
            else
            {
                Vector2 vector;
                Vector3 vector2;
                Projector previewProjector = this.GetActiveBrush((int) this.m_Size).GetPreviewProjector();
                float num = 1f;
                float num2 = this.m_Terrain.terrainData.size.x / this.m_Terrain.terrainData.size.z;
                Transform transform = previewProjector.transform;
                bool flag = true;
                if (this.Raycast(out vector, out vector2))
                {
                    if (this.selectedTool == TerrainTool.PlaceTree)
                    {
                        previewProjector.material.mainTexture = (Texture2D) EditorGUIUtility.Load(EditorResourcesUtility.brushesPath + "builtin_brush_4.png");
                        num = TreePainter.brushSize / 0.8f;
                        num2 = 1f;
                    }
                    else if (((this.selectedTool == TerrainTool.PaintHeight) || (this.selectedTool == TerrainTool.SetHeight)) || (this.selectedTool == TerrainTool.SmoothHeight))
                    {
                        if ((this.m_Size % 2) == null)
                        {
                            float num3 = 0.5f;
                            vector.x = (Mathf.Floor(vector.x * (this.m_Terrain.terrainData.heightmapWidth - 1)) + num3) / ((float) (this.m_Terrain.terrainData.heightmapWidth - 1));
                            vector.y = (Mathf.Floor(vector.y * (this.m_Terrain.terrainData.heightmapHeight - 1)) + num3) / ((float) (this.m_Terrain.terrainData.heightmapHeight - 1));
                        }
                        else
                        {
                            vector.x = Mathf.Round(vector.x * (this.m_Terrain.terrainData.heightmapWidth - 1)) / ((float) (this.m_Terrain.terrainData.heightmapWidth - 1));
                            vector.y = Mathf.Round(vector.y * (this.m_Terrain.terrainData.heightmapHeight - 1)) / ((float) (this.m_Terrain.terrainData.heightmapHeight - 1));
                        }
                        vector2.x = vector.x * this.m_Terrain.terrainData.size.x;
                        vector2.z = vector.y * this.m_Terrain.terrainData.size.z;
                        vector2 += this.m_Terrain.transform.position;
                        num = ((((float) this.m_Size) * 0.5f) / ((float) this.m_Terrain.terrainData.heightmapWidth)) * this.m_Terrain.terrainData.size.x;
                    }
                    else if ((this.selectedTool == TerrainTool.PaintTexture) || (this.selectedTool == TerrainTool.PaintDetail))
                    {
                        int alphamapWidth;
                        int alphamapHeight;
                        float num4 = ((this.m_Size % 2) != null) ? 0.5f : 0f;
                        if (this.selectedTool == TerrainTool.PaintTexture)
                        {
                            alphamapWidth = this.m_Terrain.terrainData.alphamapWidth;
                            alphamapHeight = this.m_Terrain.terrainData.alphamapHeight;
                        }
                        else
                        {
                            alphamapWidth = this.m_Terrain.terrainData.detailWidth;
                            alphamapHeight = this.m_Terrain.terrainData.detailHeight;
                        }
                        if ((alphamapWidth == 0) || (alphamapHeight == 0))
                        {
                            flag = false;
                        }
                        vector.x = (Mathf.Floor(vector.x * alphamapWidth) + num4) / ((float) alphamapWidth);
                        vector.y = (Mathf.Floor(vector.y * alphamapHeight) + num4) / ((float) alphamapHeight);
                        vector2.x = vector.x * this.m_Terrain.terrainData.size.x;
                        vector2.z = vector.y * this.m_Terrain.terrainData.size.z;
                        vector2 += this.m_Terrain.transform.position;
                        num = ((((float) this.m_Size) * 0.5f) / ((float) alphamapWidth)) * this.m_Terrain.terrainData.size.x;
                        num2 = ((float) alphamapWidth) / ((float) alphamapHeight);
                    }
                }
                else
                {
                    flag = false;
                }
                previewProjector.enabled = flag;
                if (flag)
                {
                    vector2.y = this.m_Terrain.SampleHeight(vector2);
                    transform.position = vector2 + new Vector3(0f, 50f, 0f);
                }
                previewProjector.orthographicSize = num / num2;
                previewProjector.aspectRatio = num2;
            }
        }

        private TerrainTool selectedTool
        {
            get
            {
                if (Tools.current == Tool.None)
                {
                    return (TerrainTool) this.m_SelectedTool.value;
                }
                return TerrainTool.None;
            }
            set
            {
                if (value != TerrainTool.None)
                {
                    Tools.current = Tool.None;
                }
                this.m_SelectedTool.value = (int) value;
            }
        }

        private class Styles
        {
            public GUIContent bakeLightProbesForTrees = EditorGUIUtility.TextContent("Bake Light Probes For Trees|If the option is enabled, Unity will create internal light probes at the position of each tree (these probes are internal and will not affect other renderers in the scene) and apply them to tree renderers for lighting. Otherwise trees are still affected by LightProbeGroups. The option is only effective for trees that have LightProbe enabled on their prototype prefab.");
            public GUIContent brushes = EditorGUIUtility.TextContent("Brushes");
            public GUIContent brushSize = EditorGUIUtility.TextContent("Brush Size|Size of the brush used to paint");
            public GUIStyle command = "Command";
            public GUIContent details = EditorGUIUtility.TextContent("Details");
            public GUIContent detailTargetStrength = EditorGUIUtility.TextContent("Target Strength|Target amount");
            public GUIContent editDetails = EditorGUIUtility.TextContent("Edit Details...|Add/remove detail meshes");
            public GUIContent editTextures = EditorGUIUtility.TextContent("Edit Textures...");
            public GUIContent editTrees = EditorGUIUtility.TextContent("Edit Trees...|Add/remove tree types.");
            public GUIContent exportRaw = EditorGUIUtility.TextContent("Export Raw...");
            public GUIContent flatten = EditorGUIUtility.TextContent("Flatten");
            public GUIStyle gridList = "GridList";
            public GUIStyle gridListText = "GridListText";
            public GUIContent heightmap = EditorGUIUtility.TextContent("Heightmap");
            public GUIContent importRaw = EditorGUIUtility.TextContent("Import Raw...");
            public GUIStyle label = "RightLabel";
            public GUIStyle largeSquare = "Button";
            public GUIContent lockWidth = EditorGUIUtility.TextContent("Lock Width to Height|Let the tree width be the same with height");
            public GUIContent massPlaceTrees = EditorGUIUtility.TextContent("Mass Place Trees");
            public GUIContent noTrees = EditorGUIUtility.TextContent("No Trees defined|Use edit button below to add new tree types.");
            public GUIContent opacity = EditorGUIUtility.TextContent("Opacity|Strength of the applied effect");
            public GUIContent overrideSmoothness = EditorGUIUtility.TextContent("Override Smoothness|If checked, the smoothness value specified below will be used for all splat layers, otherwise smoothness of each individual splat layer will be controlled by the alpha channel of the splat texture.");
            public GUIContent refresh = EditorGUIUtility.TextContent("Refresh");
            public GUIContent resolution = EditorGUIUtility.TextContent("Resolution");
            public GUIContent settings = EditorGUIUtility.TextContent("Settings");
            public Texture settingsIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
            public GUIContent textures = EditorGUIUtility.TextContent("Textures");
            public GUIContent[] toolIcons = new GUIContent[] { EditorGUIUtility.IconContent("TerrainInspector.TerrainToolRaise", "|Raise and lower the terrain height."), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSetHeight", "|Set the terrain height."), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSmoothHeight", "|Smooth the terrain height."), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "|Paint the terrain texture."), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolTrees", "|Place trees"), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolPlants", "|Place plants, stones and other small foilage"), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSettings", "|Settings for the terrain") };
            public GUIContent[] toolNames = new GUIContent[] { EditorGUIUtility.TextContent("Raise / Lower Terrain|Click to raise. Hold down shift to lower."), EditorGUIUtility.TextContent("Paint Height|Hold shift to sample target height."), EditorGUIUtility.TextContent("Smooth Height"), EditorGUIUtility.TextContent("Paint Texture|Select a texture below, then click to paint"), EditorGUIUtility.TextContent("Place Trees|Hold down shift to erase trees.\nHold down ctrl to erase the selected tree type."), EditorGUIUtility.TextContent("Paint Details|Hold down shift to erase.\nHold down ctrl to erase the selected detail type."), EditorGUIUtility.TextContent("Terrain Settings") };
            public GUIContent treeColorVar = EditorGUIUtility.TextContent("Color Variation|Amount of random shading applied to trees");
            public GUIContent treeDensity = EditorGUIUtility.TextContent("Tree Density|How dense trees are you painting");
            public GUIContent treeHeight = EditorGUIUtility.TextContent("Tree Height|Height of the planted trees");
            public GUIContent treeHeightRandomLabel = EditorGUIUtility.TextContent("Random?|Enable random variation in tree height (variation)");
            public GUIContent treeHeightRandomToggle = EditorGUIUtility.TextContent("|Enable random variation in tree height (variation)");
            public GUIContent treeRotation = EditorGUIUtility.TextContent("Random Tree Rotation|Enable?");
            public GUIContent trees = EditorGUIUtility.TextContent("Trees");
            public GUIContent treeWidth = EditorGUIUtility.TextContent("Tree Width|Width of the planted trees");
            public GUIContent treeWidthRandomLabel = EditorGUIUtility.TextContent("Random?|Enable random variation in tree width (variation)");
            public GUIContent treeWidthRandomToggle = EditorGUIUtility.TextContent("|Enable random variation in tree width (variation)");
        }
    }
}

