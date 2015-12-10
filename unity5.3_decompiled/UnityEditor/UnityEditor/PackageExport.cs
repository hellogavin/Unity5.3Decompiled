namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackageExport : EditorWindow
    {
        [CompilerGenerated]
        private static Func<ExportPackageItem, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<ExportPackageItem, bool> <>f__am$cache6;
        [SerializeField]
        private ExportPackageItem[] m_ExportPackageItems;
        [SerializeField]
        private bool m_IncludeDependencies = true;
        [SerializeField]
        private IncrementalInitialize m_IncrementalInitialize = new IncrementalInitialize();
        [NonSerialized]
        private PackageExportTreeView m_Tree;
        [SerializeField]
        private TreeViewState m_TreeViewState;

        public PackageExport()
        {
            base.position = new Rect(100f, 100f, 400f, 300f);
            base.minSize = new Vector2(350f, 350f);
        }

        private void BottomArea()
        {
            GUILayout.BeginVertical(Styles.bottomBarBg, new GUILayoutOption[0]);
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(Styles.allText, options))
            {
                this.m_Tree.SetAllEnabled(1);
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(Styles.noneText, optionArray2))
            {
                this.m_Tree.SetAllEnabled(0);
            }
            GUILayout.Space(10f);
            EditorGUI.BeginChangeCheck();
            this.m_IncludeDependencies = GUILayout.Toggle(this.m_IncludeDependencies, Styles.includeDependenciesText, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.RefreshAssetList();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TextContent("Export..."), new GUILayoutOption[0]))
            {
                this.Export();
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        private void BuildAssetList()
        {
            this.m_ExportPackageItems = GetAssetItemsForExport(Selection.assetGUIDsDeepSelection, this.m_IncludeDependencies).ToArray<ExportPackageItem>();
            this.m_Tree = null;
            this.m_TreeViewState = null;
        }

        private bool CheckAssetExportList()
        {
            if ((this.m_ExportPackageItems != null) && (this.m_ExportPackageItems.Length != 0))
            {
                return false;
            }
            GUILayout.Space(20f);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            GUILayout.Label("Nothing to import!", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("All assets from this package are already in your project.", "WordWrappedLabel", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", new GUILayoutOption[0]))
            {
                base.Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return true;
        }

        private void Export()
        {
            string fileName = EditorUtility.SaveFilePanel("Export package ...", string.Empty, string.Empty, "unitypackage");
            if (fileName != string.Empty)
            {
                List<string> list = new List<string>();
                foreach (ExportPackageItem item in this.m_ExportPackageItems)
                {
                    if (item.enabledStatus > 0)
                    {
                        list.Add(item.guid);
                    }
                }
                PackageUtility.ExportPackage(list.ToArray(), fileName);
                base.Close();
                GUIUtility.ExitGUI();
            }
        }

        internal static IEnumerable<ExportPackageItem> GetAssetItemsForExport(ICollection<string> guids, bool includeDependencies)
        {
            if (guids.Count == 0)
            {
                string[] collection = new string[0];
                guids = new HashSet<string>(AssetServer.CollectAllChildren(AssetServer.GetRootGUID(), collection));
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = val => val.assetPath != "Assets";
            }
            ExportPackageItem[] source = PackageUtility.BuildExportPackageItemsList(guids.ToArray<string>(), includeDependencies).Where<ExportPackageItem>(<>f__am$cache5).ToArray<ExportPackageItem>();
            if (includeDependencies)
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = asset => InternalEditorUtility.IsScriptOrAssembly(asset.assetPath);
                }
                if (source.Any<ExportPackageItem>(<>f__am$cache6))
                {
                    source = PackageUtility.BuildExportPackageItemsList(guids.Union<string>(InternalEditorUtility.GetAllScriptGUIDs()).ToArray<string>(), includeDependencies);
                }
            }
            return source;
        }

        private bool HasValidAssetList()
        {
            return (this.m_ExportPackageItems != null);
        }

        public void OnGUI()
        {
            this.m_IncrementalInitialize.OnEvent();
            bool showLoadingScreen = false;
            switch (this.m_IncrementalInitialize.state)
            {
                case IncrementalInitialize.State.PreInitialize:
                    showLoadingScreen = true;
                    break;

                case IncrementalInitialize.State.Initialize:
                    this.BuildAssetList();
                    break;
            }
            if (!this.CheckAssetExportList())
            {
                EditorGUI.BeginDisabledGroup(!this.HasValidAssetList());
                this.TopArea();
                EditorGUI.EndDisabledGroup();
                this.TreeViewArea(showLoadingScreen);
                EditorGUI.BeginDisabledGroup(!this.HasValidAssetList());
                this.BottomArea();
                EditorGUI.EndDisabledGroup();
            }
        }

        private void RefreshAssetList()
        {
            this.m_IncrementalInitialize.Restart();
            this.m_ExportPackageItems = null;
        }

        internal static void ShowExportPackage()
        {
            EditorWindow.GetWindow<PackageExport>(true, "Exporting package");
        }

        private void TopArea()
        {
            float height = 53f;
            Rect position = GUILayoutUtility.GetRect(base.position.width, height);
            GUI.Label(position, GUIContent.none, Styles.topBarBg);
            Rect rect2 = new Rect(position.x + 5f, position.yMin, position.width, position.height);
            GUI.Label(rect2, Styles.header, Styles.title);
        }

        private void TreeViewArea(bool showLoadingScreen)
        {
            Rect position = GUILayoutUtility.GetRect(1f, 9999f, (float) 1f, (float) 99999f);
            if (showLoadingScreen)
            {
                GUI.Label(position, "Loading...", Styles.loadingTextStyle);
            }
            else if ((this.m_ExportPackageItems != null) && (this.m_ExportPackageItems.Length > 0))
            {
                if (this.m_TreeViewState == null)
                {
                    this.m_TreeViewState = new TreeViewState();
                }
                if (this.m_Tree == null)
                {
                    this.m_Tree = new PackageExportTreeView(this, this.m_TreeViewState, new Rect());
                }
                this.m_Tree.OnGUI(position);
            }
        }

        public ExportPackageItem[] items
        {
            get
            {
                return this.m_ExportPackageItems;
            }
        }

        internal static class Styles
        {
            public static GUIContent allText = EditorGUIUtility.TextContent("All");
            public static GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
            public static GUIContent header = new GUIContent("Items to Export");
            public static GUIContent includeDependenciesText = EditorGUIUtility.TextContent("Include dependencies");
            public static GUIStyle loadingTextStyle = new GUIStyle(EditorStyles.label);
            public static GUIContent noneText = EditorGUIUtility.TextContent("None");
            public static GUIStyle title = new GUIStyle(EditorStyles.largeLabel);
            public static GUIStyle topBarBg = new GUIStyle("ProjectBrowserHeaderBgTop");

            static Styles()
            {
                topBarBg.fixedHeight = 0f;
                int num = 2;
                topBarBg.border.bottom = num;
                topBarBg.border.top = num;
                title.fontStyle = FontStyle.Bold;
                title.alignment = TextAnchor.MiddleLeft;
                loadingTextStyle.alignment = TextAnchor.MiddleCenter;
            }
        }
    }
}

