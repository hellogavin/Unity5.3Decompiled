namespace UnityEditor.Sprites
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackerWindow : SpriteUtilityWindow
    {
        [CompilerGenerated]
        private static Func<Edge, Edge> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<IGrouping<Edge, Edge>, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<IGrouping<Edge, Edge>, Edge> <>f__am$cache9;
        private string[] m_AtlasNames = s_AtlasNamesEmpty;
        private string[] m_PageNames = s_PageNamesEmpty;
        private int m_SelectedAtlas;
        private int m_SelectedPage;
        private Sprite m_SelectedSprite;
        private static string[] s_AtlasNamesEmpty = new string[] { "Sprite atlas cache is empty" };
        private static string[] s_PageNamesEmpty = new string[0];

        private void DoToolbarGUI()
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            if (GUILayout.Button("Pack", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
                this.m_SelectedSprite = null;
                this.RefreshAtlasPageList();
                this.RefreshState();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(Packer.SelectedPolicy == Packer.kDefaultPolicy);
                if (GUILayout.Button("Repack", EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.ForceRegroup);
                    this.m_SelectedSprite = null;
                    this.RefreshAtlasPageList();
                    this.RefreshState();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(this.m_AtlasNames.Length == 0);
            GUILayout.Space(16f);
            GUILayout.Label("View atlas:", new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            this.m_SelectedAtlas = EditorGUILayout.Popup(this.m_SelectedAtlas, this.m_AtlasNames, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.RefreshAtlasPageList();
                this.m_SelectedSprite = null;
            }
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(70f) };
            this.m_SelectedPage = EditorGUILayout.Popup(this.m_SelectedPage, this.m_PageNames, EditorStyles.toolbarPopup, options);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_SelectedSprite = null;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginChangeCheck();
            string[] policies = Packer.Policies;
            int index = EditorGUILayout.Popup(Array.IndexOf<string>(policies, Packer.SelectedPolicy), policies, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Packer.SelectedPolicy = policies[index];
            }
            EditorGUILayout.EndHorizontal();
        }

        protected override void DrawGizmos()
        {
            if ((this.m_SelectedSprite != null) && (base.m_Texture != null))
            {
                Vector2[] spriteUVs = SpriteUtility.GetSpriteUVs(this.m_SelectedSprite, true);
                ushort[] triangles = this.m_SelectedSprite.triangles;
                Edge[] edgeArray = this.FindUniqueEdges(triangles);
                SpriteEditorUtility.BeginLines(new Color(0.3921f, 0.5843f, 0.9294f, 0.75f));
                foreach (Edge edge in edgeArray)
                {
                    this.DrawLineUtility(spriteUVs[edge.v0], spriteUVs[edge.v1]);
                }
                SpriteEditorUtility.EndLines();
            }
        }

        private void DrawLineUtility(Vector2 from, Vector2 to)
        {
            SpriteEditorUtility.DrawLine(new Vector3((from.x * base.m_Texture.width) + (1f / base.m_Zoom), (from.y * base.m_Texture.height) + (1f / base.m_Zoom), 0f), new Vector3((to.x * base.m_Texture.width) + (1f / base.m_Zoom), (to.y * base.m_Texture.height) + (1f / base.m_Zoom), 0f));
        }

        private Edge[] FindUniqueEdges(ushort[] indices)
        {
            Edge[] source = new Edge[indices.Length];
            int num = indices.Length / 3;
            for (int i = 0; i < num; i++)
            {
                source[i * 3] = new Edge(indices[i * 3], indices[(i * 3) + 1]);
                source[(i * 3) + 1] = new Edge(indices[(i * 3) + 1], indices[(i * 3) + 2]);
                source[(i * 3) + 2] = new Edge(indices[(i * 3) + 2], indices[i * 3]);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = x => x;
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = x => x.Count<Edge>() == 1;
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = x => x.First<Edge>();
            }
            return source.GroupBy<Edge, Edge>(<>f__am$cache7).Where<IGrouping<Edge, Edge>>(<>f__am$cache8).Select<IGrouping<Edge, Edge>, Edge>(<>f__am$cache9).ToArray<Edge>();
        }

        private void OnAtlasNameListChanged()
        {
            if (this.m_AtlasNames.Length > 0)
            {
                string[] atlasNames = Packer.atlasNames;
                string str = this.m_AtlasNames[this.m_SelectedAtlas];
                string str2 = (atlasNames.Length > this.m_SelectedAtlas) ? atlasNames[this.m_SelectedAtlas] : null;
                if (str.Equals(str2))
                {
                    this.RefreshAtlasNameList();
                    this.RefreshAtlasPageList();
                    this.m_SelectedSprite = null;
                    return;
                }
            }
            this.Reset();
        }

        private void OnEnable()
        {
            base.minSize = new Vector2(400f, 256f);
            base.titleContent = EditorGUIUtility.TextContent("Sprite Packer");
            this.Reset();
        }

        public void OnGUI()
        {
            if (this.ValidateIsPackingEnabled())
            {
                Matrix4x4 matrix = Handles.matrix;
                base.InitStyles();
                this.RefreshState();
                Rect rect = EditorGUILayout.BeginHorizontal(GUIContent.none, EditorStyles.toolbar, new GUILayoutOption[0]);
                this.DoToolbarGUI();
                GUILayout.FlexibleSpace();
                bool enabled = GUI.enabled;
                GUI.enabled = this.m_AtlasNames.Length > 0;
                base.DoAlphaZoomToolbarGUI();
                GUI.enabled = enabled;
                EditorGUILayout.EndHorizontal();
                if (base.m_Texture != null)
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    base.m_TextureViewRect = new Rect(0f, rect.yMax, base.position.width - 16f, (base.position.height - 16f) - rect.height);
                    GUILayout.FlexibleSpace();
                    base.DoTextureGUI();
                    string text = string.Format("{1}x{2}, {0}", TextureUtil.GetTextureFormatString(base.m_Texture.format), base.m_Texture.width, base.m_Texture.height);
                    EditorGUI.DropShadowLabel(new Rect(this.m_TextureViewRect.x, this.m_TextureViewRect.y + 10f, this.m_TextureViewRect.width, 20f), text);
                    EditorGUILayout.EndHorizontal();
                    Handles.matrix = matrix;
                }
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject != null)
            {
                Sprite activeObject = Selection.activeObject as Sprite;
                if (activeObject != this.m_SelectedSprite)
                {
                    if (activeObject != null)
                    {
                        <OnSelectionChange>c__AnonStoreyAB yab = new <OnSelectionChange>c__AnonStoreyAB();
                        Packer.GetAtlasDataForSprite(activeObject, out yab.selAtlasName, out yab.selAtlasTexture);
                        int num = this.m_AtlasNames.ToList<string>().FindIndex(new Predicate<string>(yab.<>m__20B));
                        if (num == -1)
                        {
                            return;
                        }
                        int num2 = Packer.GetTexturesForAtlas(yab.selAtlasName).ToList<Texture2D>().FindIndex(new Predicate<Texture2D>(yab.<>m__20C));
                        if (num2 == -1)
                        {
                            return;
                        }
                        this.m_SelectedAtlas = num;
                        this.m_SelectedPage = num2;
                        this.RefreshAtlasPageList();
                    }
                    this.m_SelectedSprite = activeObject;
                    base.Repaint();
                }
            }
        }

        private void RefreshAtlasNameList()
        {
            this.m_AtlasNames = Packer.atlasNames;
            if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
            {
                this.m_SelectedAtlas = 0;
            }
        }

        private void RefreshAtlasPageList()
        {
            if (this.m_AtlasNames.Length > 0)
            {
                string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
                Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
                this.m_PageNames = new string[texturesForAtlas.Length];
                for (int i = 0; i < texturesForAtlas.Length; i++)
                {
                    this.m_PageNames[i] = string.Format("Page {0}", i + 1);
                }
            }
            else
            {
                this.m_PageNames = s_PageNamesEmpty;
            }
            if (this.m_SelectedPage >= this.m_PageNames.Length)
            {
                this.m_SelectedPage = 0;
            }
        }

        private void RefreshState()
        {
            string[] atlasNames = Packer.atlasNames;
            if (!atlasNames.SequenceEqual<string>(this.m_AtlasNames))
            {
                if (atlasNames.Length == 0)
                {
                    this.Reset();
                    return;
                }
                this.OnAtlasNameListChanged();
            }
            if (this.m_AtlasNames.Length == 0)
            {
                base.SetNewTexture(null);
            }
            else
            {
                if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
                {
                    this.m_SelectedAtlas = 0;
                }
                string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
                Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
                if (this.m_SelectedPage >= texturesForAtlas.Length)
                {
                    this.m_SelectedPage = 0;
                }
                base.SetNewTexture(texturesForAtlas[this.m_SelectedPage]);
                Texture2D[] alphaTexturesForAtlas = Packer.GetAlphaTexturesForAtlas(atlasName);
                Texture2D alphaTexture = (this.m_SelectedPage >= alphaTexturesForAtlas.Length) ? null : alphaTexturesForAtlas[this.m_SelectedPage];
                base.SetAlphaTextureOverride(alphaTexture);
            }
        }

        private void Reset()
        {
            this.RefreshAtlasNameList();
            this.RefreshAtlasPageList();
            this.m_SelectedAtlas = 0;
            this.m_SelectedPage = 0;
            this.m_SelectedSprite = null;
        }

        private bool ValidateIsPackingEnabled()
        {
            if (EditorSettings.spritePackerMode != SpritePackerMode.Disabled)
            {
                return true;
            }
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Sprite packing is disabled. Enable it in Edit > Project Settings > Editor.", new GUILayoutOption[0]);
            if (GUILayout.Button("Open Project Editor Settings", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
            }
            EditorGUILayout.EndVertical();
            return false;
        }

        [CompilerGenerated]
        private sealed class <OnSelectionChange>c__AnonStoreyAB
        {
            internal string selAtlasName;
            internal Texture2D selAtlasTexture;

            internal bool <>m__20B(string s)
            {
                return (this.selAtlasName == s);
            }

            internal bool <>m__20C(Texture2D t)
            {
                return (this.selAtlasTexture == t);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Edge
        {
            public ushort v0;
            public ushort v1;
            public Edge(ushort a, ushort b)
            {
                this.v0 = a;
                this.v1 = b;
            }

            public override bool Equals(object obj)
            {
                PackerWindow.Edge edge = (PackerWindow.Edge) obj;
                return (((this.v0 == edge.v0) && (this.v1 == edge.v1)) || ((this.v0 == edge.v1) && (this.v1 == edge.v0)));
            }

            public override int GetHashCode()
            {
                int num = (this.v1 << 0x10) | this.v0;
                return (((this.v0 << 0x10) | this.v1) ^ num.GetHashCode());
            }
        }
    }
}

