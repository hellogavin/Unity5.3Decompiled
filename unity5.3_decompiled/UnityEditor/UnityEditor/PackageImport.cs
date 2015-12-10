namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class PackageImport : EditorWindow
    {
        [SerializeField]
        private ImportPackageItem[] m_ImportPackageItems;
        [SerializeField]
        private string m_PackageIconPath;
        [SerializeField]
        private string m_PackageName;
        private bool m_ReInstallPackage;
        private bool m_ShowReInstall;
        [NonSerialized]
        private PackageImportTreeView m_Tree;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private static Constants ms_Constants;
        private static readonly char[] s_InvalidPathChars = Path.GetInvalidPathChars();
        private static string s_LastPreviewPath;
        private static Texture2D s_PackageIcon;
        private static Texture2D s_Preview;

        public PackageImport()
        {
            base.minSize = new Vector2(350f, 350f);
        }

        private void BottomArea()
        {
            GUILayout.BeginVertical(ms_Constants.bottomBarBg, new GUILayoutOption[0]);
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(EditorGUIUtility.TextContent("All"), options))
            {
                this.m_Tree.SetAllEnabled(1);
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(EditorGUIUtility.TextContent("None"), optionArray2))
            {
                this.m_Tree.SetAllEnabled(0);
            }
            this.ReInstallToggle();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TextContent("Cancel"), new GUILayoutOption[0]))
            {
                PopupWindowWithoutFocus.Hide();
                base.Close();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(EditorGUIUtility.TextContent("Import"), new GUILayoutOption[0]))
            {
                bool flag = true;
                if (this.doReInstall)
                {
                    flag = EditorUtility.DisplayDialog("Re-Install?", "Highlighted folders will be completely deleted first! Recommend backing up your project first. Are you sure?", "Do It", "Cancel");
                }
                if (flag)
                {
                    if (this.m_ImportPackageItems != null)
                    {
                        PackageUtility.ImportPackageAssets(this.m_ImportPackageItems, this.doReInstall);
                    }
                    PopupWindowWithoutFocus.Hide();
                    base.Close();
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        private void DestroyCreatedIcons()
        {
            if (s_Preview != null)
            {
                Object.DestroyImmediate(s_Preview);
                s_Preview = null;
                s_LastPreviewPath = null;
            }
            if (s_PackageIcon != null)
            {
                Object.DestroyImmediate(s_PackageIcon);
                s_PackageIcon = null;
            }
        }

        public static void DrawTexture(Rect r, Texture2D tex, bool useDropshadow)
        {
            if (tex != null)
            {
                float width = tex.width;
                float height = tex.height;
                if ((width >= height) && (width > r.width))
                {
                    height = (height * r.width) / width;
                    width = r.width;
                }
                else if ((height > width) && (height > r.height))
                {
                    width = (width * r.height) / height;
                    height = r.height;
                }
                float x = r.x + Mathf.Round((r.width - width) / 2f);
                float y = r.y + Mathf.Round((r.height - height) / 2f);
                r = new Rect(x, y, width, height);
                if (useDropshadow && (Event.current.type == EventType.Repaint))
                {
                    Rect position = new RectOffset(1, 1, 1, 1).Remove(ms_Constants.textureIconDropShadow.border.Add(r));
                    ms_Constants.textureIconDropShadow.Draw(position, GUIContent.none, false, false, false, false);
                }
                GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
            }
        }

        public static Texture2D GetPreview(string previewPath)
        {
            if (previewPath != s_LastPreviewPath)
            {
                s_LastPreviewPath = previewPath;
                LoadTexture(previewPath, ref s_Preview);
            }
            return s_Preview;
        }

        public static bool HasInvalidCharInFilePath(string filePath)
        {
            char ch;
            int num;
            return HasInvalidCharInFilePath(filePath, out ch, out num);
        }

        private static bool HasInvalidCharInFilePath(string filePath, out char invalidChar, out int invalidCharIndex)
        {
            for (int i = 0; i < filePath.Length; i++)
            {
                char ch = filePath[i];
                if (s_InvalidPathChars.Contains<char>(ch))
                {
                    invalidChar = ch;
                    invalidCharIndex = i;
                    return true;
                }
            }
            invalidChar = ' ';
            invalidCharIndex = -1;
            return false;
        }

        private void Init(string packagePath, ImportPackageItem[] items, string packageIconPath, bool allowReInstall)
        {
            this.DestroyCreatedIcons();
            this.m_ShowReInstall = allowReInstall;
            this.m_ReInstallPackage = true;
            this.m_TreeViewState = null;
            this.m_Tree = null;
            this.m_ImportPackageItems = items;
            this.m_PackageName = Path.GetFileNameWithoutExtension(packagePath);
            this.m_PackageIconPath = packageIconPath;
            base.Repaint();
        }

        private static bool IsAllFilePathsValid(ImportPackageItem[] assetItems, out string errorMessage)
        {
            foreach (ImportPackageItem item in assetItems)
            {
                char ch;
                int num2;
                if (!item.isFolder && HasInvalidCharInFilePath(item.destinationAssetPath, out ch, out num2))
                {
                    errorMessage = string.Format("Invalid character found in file path: '{0}'. Invalid ascii value: {1} (at character index {2}).", item.destinationAssetPath, (int) ch, num2);
                    return false;
                }
            }
            errorMessage = string.Empty;
            return true;
        }

        private static void LoadTexture(string filepath, ref Texture2D texture)
        {
            if (texture == null)
            {
                texture = new Texture2D(0x80, 0x80);
            }
            byte[] data = null;
            try
            {
                data = File.ReadAllBytes(filepath);
            }
            catch
            {
            }
            if (((filepath == string.Empty) || (data == null)) || !texture.LoadImage(data))
            {
                Color[] pixels = texture.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(0.5f, 0.5f, 0.5f, 0f);
                }
                texture.SetPixels(pixels);
                texture.Apply();
            }
        }

        private void OnDisable()
        {
            this.DestroyCreatedIcons();
        }

        public void OnGUI()
        {
            if (ms_Constants == null)
            {
                ms_Constants = new Constants();
            }
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            if (this.m_Tree == null)
            {
                this.m_Tree = new PackageImportTreeView(this, this.m_TreeViewState, new Rect());
            }
            if ((this.m_ImportPackageItems != null) && this.ShowTreeGUI(this.doReInstall, this.m_ImportPackageItems))
            {
                this.TopArea();
                this.m_Tree.OnGUI(GUILayoutUtility.GetRect(1f, 9999f, (float) 1f, (float) 99999f));
                this.BottomArea();
            }
            else
            {
                GUILayout.Label("Nothing to import!", EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.Label("All assets from this package are already in your project.", "WordWrappedLabel", new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(ms_Constants.bottomBarBg, new GUILayoutOption[0]);
                GUILayout.Space(8f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                this.ReInstallToggle();
                if (GUILayout.Button("OK", new GUILayoutOption[0]))
                {
                    base.Close();
                    GUIUtility.ExitGUI();
                }
                GUILayout.Space(10f);
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                GUILayout.EndVertical();
            }
        }

        private void ReInstallToggle()
        {
            if (this.m_ShowReInstall)
            {
                EditorGUI.BeginChangeCheck();
                bool flag = GUILayout.Toggle(this.m_ReInstallPackage, "Re-Install Package", new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_ReInstallPackage = flag;
                }
            }
        }

        public static void ShowImportPackage(string packagePath, ImportPackageItem[] items, string packageIconPath, bool allowReInstall)
        {
            if (ValidateInput(items))
            {
                EditorWindow.GetWindow<PackageImport>(true, "Import Unity Package").Init(packagePath, items, packageIconPath, allowReInstall);
            }
        }

        private bool ShowTreeGUI(bool reInstalling, ImportPackageItem[] items)
        {
            if (reInstalling)
            {
                return true;
            }
            if (items.Length != 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (!items[i].isFolder && items[i].assetChanged)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void TopArea()
        {
            Rect rect2;
            if ((s_PackageIcon == null) && !string.IsNullOrEmpty(this.m_PackageIconPath))
            {
                LoadTexture(this.m_PackageIconPath, ref s_PackageIcon);
            }
            bool flag = s_PackageIcon != null;
            float height = !flag ? 52f : 84f;
            Rect position = GUILayoutUtility.GetRect(base.position.width, height);
            GUI.Label(position, GUIContent.none, ms_Constants.topBarBg);
            if (flag)
            {
                Rect r = new Rect(position.x + 10f, position.y + 10f, 64f, 64f);
                DrawTexture(r, s_PackageIcon, true);
                rect2 = new Rect(r.xMax + 10f, r.yMin, position.width, r.height);
            }
            else
            {
                rect2 = new Rect(position.x + 5f, position.yMin, position.width, position.height);
            }
            GUI.Label(rect2, this.m_PackageName, ms_Constants.title);
        }

        private static bool ValidateInput(ImportPackageItem[] items)
        {
            string str;
            if (!IsAllFilePathsValid(items, out str))
            {
                str = str + "\nDo you want to import the valid file paths of the package or cancel importing?";
                return EditorUtility.DisplayDialog("Invalid file path found", str, "Import", "Cancel importing");
            }
            return true;
        }

        public bool canReInstall
        {
            get
            {
                return this.m_ShowReInstall;
            }
        }

        public bool doReInstall
        {
            get
            {
                return (this.m_ShowReInstall && this.m_ReInstallPackage);
            }
        }

        public ImportPackageItem[] packageItems
        {
            get
            {
                return this.m_ImportPackageItems;
            }
        }

        internal class Constants
        {
            public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
            public GUIStyle ConsoleEntryBackEven = "CN EntryBackEven";
            public GUIStyle ConsoleEntryBackOdd = "CN EntryBackOdd";
            public Color lineColor = (!EditorGUIUtility.isProSkin ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.1f, 0.1f));
            public GUIStyle textureIconDropShadow = "ProjectBrowserTextureIconDropShadow";
            public GUIStyle title = new GUIStyle(EditorStyles.largeLabel);
            public GUIStyle topBarBg = new GUIStyle("ProjectBrowserHeaderBgTop");

            public Constants()
            {
                this.topBarBg.fixedHeight = 0f;
                int num = 2;
                this.topBarBg.border.bottom = num;
                this.topBarBg.border.top = num;
                this.title.fontStyle = FontStyle.Bold;
                this.title.alignment = TextAnchor.MiddleLeft;
            }
        }
    }
}

