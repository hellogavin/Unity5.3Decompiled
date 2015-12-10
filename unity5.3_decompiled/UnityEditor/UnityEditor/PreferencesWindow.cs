namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Connect;
    using UnityEditor.Modules;
    using UnityEditor.VisualStudioIntegration;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PreferencesWindow : EditorWindow
    {
        private static Constants constants = null;
        private int currentPage;
        private const int k_LangListMenuOffset = 2;
        private static int kMaxSpriteCacheSizeInGigabytes = 200;
        private static int kMinSpriteCacheSizeInGigabytes = 1;
        private const int kRecentAppsCount = 10;
        private const string kRecentImageAppsKey = "RecentlyUsedImageApp";
        private const string kRecentScriptAppsKey = "RecentlyUsedScriptApp";
        private bool m_AllowAlphaNumericHierarchy;
        private bool m_AllowAttachedDebuggingOfEditor;
        private bool m_AllowAttachedDebuggingOfEditorStateChangedThisSession;
        private bool m_AutoRefresh;
        private bool m_CompressAssetsOnImport;
        private int m_DiffToolIndex;
        private string[] m_DiffTools;
        private string[] m_EditorLanguageNames;
        private bool m_EnableEditorAnalytics;
        private const string m_ExpressNotSupportedMessage = "Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)";
        private bool m_ExternalEditorSupportsUnityProj;
        private GICacheSettings m_GICacheSettings;
        private string[] m_ImageAppDisplayNames;
        private RefString m_ImageAppPath = new RefString(string.Empty);
        private string[] m_ImageApps;
        private Vector2 m_KeyScrollPos;
        private string m_noDiffToolsMessage = string.Empty;
        private bool m_RefreshCustomPreferences;
        private bool m_ReopenLastUsedProjectOnStartup;
        private string[] m_ScriptAppDisplayNames;
        private string[] m_ScriptApps;
        private string m_ScriptEditorArgs = string.Empty;
        private RefString m_ScriptEditorPath = new RefString(string.Empty);
        private List<Section> m_Sections;
        private Vector2 m_SectionScrollPos;
        private PrefKey m_SelectedKey;
        private SystemLanguage m_SelectedLanguage = SystemLanguage.English;
        private int m_SelectedSectionIndex;
        private bool m_ShowAssetStoreSearchHits;
        private int m_SpriteAtlasCacheSize;
        private bool m_UseOSColorPicker;
        private bool m_VerifySavingAssets;
        private List<IPreferenceWindowExtension> prefWinExtensions;
        private SortedDictionary<string, List<KeyValuePair<string, PrefColor>>> s_CachedColors;
        private static Vector2 s_ColorScrollPos = Vector2.zero;
        private static int s_KeysControlHash = "KeysControlHash".GetHashCode();

        private void AddCustomSections()
        {
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
                {
                    foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        PreferenceItem customAttribute = Attribute.GetCustomAttribute(info, typeof(PreferenceItem)) as PreferenceItem;
                        if (customAttribute != null)
                        {
                            OnGUIDelegate guiFunc = Delegate.CreateDelegate(typeof(OnGUIDelegate), info) as OnGUIDelegate;
                            if (guiFunc != null)
                            {
                                this.m_Sections.Add(new Section(customAttribute.name, guiFunc));
                            }
                        }
                    }
                }
            }
        }

        private void ApplyChangesToPrefs()
        {
            if (GUI.changed)
            {
                this.WritePreferences();
                this.ReadPreferences();
                base.Repaint();
            }
        }

        private void AppsListClick(object userData, string[] options, int selected)
        {
            AppsListUserData data = (AppsListUserData) userData;
            if (options[selected] == "Browse...")
            {
                string str = EditorUtility.OpenFilePanel("Browse for application", string.Empty, (Application.platform != RuntimePlatform.OSXEditor) ? "exe" : "app");
                if (str.Length != 0)
                {
                    data.str.str = str;
                    if (data.onChanged != null)
                    {
                        data.onChanged();
                    }
                }
            }
            else
            {
                data.str.str = data.paths[selected];
                if (data.onChanged != null)
                {
                    data.onChanged();
                }
            }
            this.WritePreferences();
            this.ReadPreferences();
        }

        private string[] BuildAppPathList(string userAppPath, string recentAppsKey, string stringForInternalEditor)
        {
            string[] array = new string[] { stringForInternalEditor };
            if (((userAppPath != null) && (userAppPath.Length != 0)) && (Array.IndexOf<string>(array, userAppPath) == -1))
            {
                ArrayUtility.Add<string>(ref array, userAppPath);
            }
            for (int i = 0; i < 10; i++)
            {
                string path = EditorPrefs.GetString(recentAppsKey + i);
                if (!File.Exists(path))
                {
                    path = string.Empty;
                    EditorPrefs.SetString(recentAppsKey + i, path);
                }
                if ((path.Length != 0) && (Array.IndexOf<string>(array, path) == -1))
                {
                    ArrayUtility.Add<string>(ref array, path);
                }
            }
            return array;
        }

        private string[] BuildFriendlyAppNameList(string[] appPathList, string defaultBuiltIn)
        {
            List<string> list = new List<string>();
            foreach (string str in appPathList)
            {
                switch (str)
                {
                    case "internal":
                    case string.Empty:
                        list.Add(defaultBuiltIn);
                        break;

                    default:
                        list.Add(this.StripMicrosoftFromVisualStudioName(OSUtil.GetAppFriendlyName(str)));
                        break;
                }
            }
            return list.ToArray();
        }

        private void DoUnityProjCheckbox()
        {
            bool flag = false;
            bool externalEditorSupportsUnityProj = false;
            if (this.IsInternalMonoDevelop())
            {
                externalEditorSupportsUnityProj = true;
            }
            else if (this.IsExternalMonoDevelopOrXamarinStudio())
            {
                flag = true;
                externalEditorSupportsUnityProj = this.m_ExternalEditorSupportsUnityProj;
            }
            EditorGUI.BeginDisabledGroup(!flag);
            externalEditorSupportsUnityProj = EditorGUILayout.Toggle("Add .unityproj's to .sln", externalEditorSupportsUnityProj, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (flag)
            {
                this.m_ExternalEditorSupportsUnityProj = externalEditorSupportsUnityProj;
            }
        }

        private void FilePopup(string label, string selectedString, ref string[] names, ref string[] paths, RefString outString, string defaultString, Action onChanged)
        {
            GUIStyle popup = EditorStyles.popup;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(label, popup);
            int[] selected = new int[0];
            if (paths.Contains<string>(selectedString))
            {
                selected = new int[] { Array.IndexOf<string>(paths, selectedString) };
            }
            GUIContent content = new GUIContent((selected.Length != 0) ? names[selected[0]] : defaultString);
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, popup);
            AppsListUserData userData = new AppsListUserData(paths, outString, onChanged);
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Native, popup))
            {
                ArrayUtility.Add<string>(ref names, "Browse...");
                EditorUtility.DisplayCustomMenu(position, names, selected, new EditorUtility.SelectMenuItemFunction(this.AppsListClick), userData);
            }
            GUILayout.EndHorizontal();
        }

        private static string GetProgramFilesFolder()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (environmentVariable != null)
            {
                return environmentVariable;
            }
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void HandleKeys()
        {
            if ((Event.current.type == EventType.KeyDown) && (GUIUtility.keyboardControl == 0))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        this.selectedSectionIndex--;
                        Event.current.Use();
                        break;

                    case KeyCode.DownArrow:
                        this.selectedSectionIndex++;
                        Event.current.Use();
                        break;
                }
            }
        }

        private bool IsExternalMonoDevelopOrXamarinStudio()
        {
            string[] source = new string[] { "Xamarin Studio", "xamarinstudio", "monodevelop" };
            return source.Any<string>(s => (this.m_ScriptEditorPath.str.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1));
        }

        private bool IsInternalMonoDevelop()
        {
            return (this.m_ScriptEditorPath.str == "internal");
        }

        private bool IsSelectedScriptEditorSpecial()
        {
            string str = this.m_ScriptEditorPath.str.ToLower();
            return ((((str == string.Empty) || (str == "internal")) || (str.EndsWith("monodevelop.exe") || str.EndsWith("devenv.exe"))) || str.EndsWith("vcsexpress.exe"));
        }

        private void OnEnable()
        {
            this.prefWinExtensions = ModuleManager.GetPreferenceWindowExtensions();
            this.ReadPreferences();
            this.m_Sections = new List<Section>();
            this.m_Sections.Add(new Section("General", new OnGUIDelegate(this.ShowGeneral)));
            this.m_Sections.Add(new Section("External Tools", new OnGUIDelegate(this.ShowExternalApplications)));
            this.m_Sections.Add(new Section("Colors", new OnGUIDelegate(this.ShowColors)));
            this.m_Sections.Add(new Section("Keys", new OnGUIDelegate(this.ShowKeys)));
            this.m_Sections.Add(new Section("GI Cache", new OnGUIDelegate(this.ShowGICache)));
            this.m_Sections.Add(new Section("2D", new OnGUIDelegate(this.Show2D)));
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                this.m_Sections.Add(new Section("Unity Services", new OnGUIDelegate(this.ShowUnityConnectPrefs)));
            }
            this.m_RefreshCustomPreferences = true;
        }

        private void OnGUI()
        {
            if (this.m_RefreshCustomPreferences)
            {
                this.AddCustomSections();
                this.m_RefreshCustomPreferences = false;
            }
            EditorGUIUtility.labelWidth = 200f;
            if (constants == null)
            {
                constants = new Constants();
            }
            this.HandleKeys();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            this.m_SectionScrollPos = GUILayout.BeginScrollView(this.m_SectionScrollPos, constants.sectionScrollView, options);
            GUILayout.Space(40f);
            for (int i = 0; i < this.m_Sections.Count; i++)
            {
                Section section = this.m_Sections[i];
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect(section.content, constants.sectionElement, optionArray2);
                if ((section == this.selectedSection) && (Event.current.type == EventType.Repaint))
                {
                    constants.selected.Draw(position, false, false, false, false);
                }
                EditorGUI.BeginChangeCheck();
                if (GUI.Toggle(position, this.m_SelectedSectionIndex == i, section.content, constants.sectionElement))
                {
                    this.m_SelectedSectionIndex = i;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    GUIUtility.keyboardControl = 0;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(this.selectedSection.content, constants.sectionHeader, new GUILayoutOption[0]);
            this.selectedSection.guiFunc();
            GUILayout.Space(5f);
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
        }

        private void OnScriptEditorArgsChanged()
        {
            EditorPrefs.SetString("kScriptEditorArgs" + this.m_ScriptEditorPath.str, this.m_ScriptEditorArgs);
            EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
        }

        private void OnScriptEditorChanged()
        {
            if (this.IsSelectedScriptEditorSpecial())
            {
                this.m_ScriptEditorArgs = string.Empty;
            }
            else
            {
                this.m_ScriptEditorArgs = EditorPrefs.GetString("kScriptEditorArgs" + this.m_ScriptEditorPath.str, "\"$(File)\"");
            }
            EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
            UnityVSSupport.ScriptEditorChanged(this.m_ScriptEditorPath.str);
        }

        private SortedDictionary<string, List<KeyValuePair<string, T>>> OrderPrefs<T>(IEnumerable<KeyValuePair<string, T>> input) where T: IPrefType
        {
            SortedDictionary<string, List<KeyValuePair<string, T>>> dictionary = new SortedDictionary<string, List<KeyValuePair<string, T>>>();
            IEnumerator<KeyValuePair<string, T>> enumerator = input.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string str;
                    string key;
                    KeyValuePair<string, T> current = enumerator.Current;
                    int index = current.Key.IndexOf('/');
                    if (index == -1)
                    {
                        str = "General";
                        key = current.Key;
                    }
                    else
                    {
                        str = current.Key.Substring(0, index);
                        key = current.Key.Substring(index + 1);
                    }
                    if (!dictionary.ContainsKey(str))
                    {
                        List<KeyValuePair<string, T>> collection = new List<KeyValuePair<string, T>> {
                            new KeyValuePair<string, T>(key, current.Value)
                        };
                        dictionary.Add(str, new List<KeyValuePair<string, T>>(collection));
                    }
                    else
                    {
                        dictionary[str].Add(new KeyValuePair<string, T>(key, current.Value));
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return dictionary;
        }

        private void ReadPreferences()
        {
            this.m_ScriptEditorPath.str = EditorPrefs.GetString("kScriptsDefaultApp");
            this.m_ScriptEditorArgs = EditorPrefs.GetString("kScriptEditorArgs", "\"$(File)\"");
            this.m_ExternalEditorSupportsUnityProj = EditorPrefs.GetBool("kExternalEditorSupportsUnityProj", false);
            this.m_ImageAppPath.str = EditorPrefs.GetString("kImagesDefaultApp");
            this.m_ScriptApps = this.BuildAppPathList((string) this.m_ScriptEditorPath, "RecentlyUsedScriptApp", "internal");
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                foreach (string str in SyncVS.InstalledVisualStudios.Values)
                {
                    if (Array.IndexOf<string>(this.m_ScriptApps, str) == -1)
                    {
                        if (this.m_ScriptApps.Length < 10)
                        {
                            ArrayUtility.Add<string>(ref this.m_ScriptApps, str);
                        }
                        else
                        {
                            this.m_ScriptApps[1] = str;
                        }
                    }
                }
            }
            this.m_ImageApps = this.BuildAppPathList((string) this.m_ImageAppPath, "RecentlyUsedImageApp", string.Empty);
            this.m_ScriptAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ScriptApps, "MonoDevelop (built-in)");
            this.m_ImageAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ImageApps, "Open by file extension");
            this.m_DiffTools = InternalEditorUtility.GetAvailableDiffTools();
            if (((this.m_DiffTools == null) || (this.m_DiffTools.Length == 0)) && InternalEditorUtility.HasTeamLicense())
            {
                this.m_noDiffToolsMessage = InternalEditorUtility.GetNoDiffToolsDetectedMessage();
            }
            string str2 = EditorPrefs.GetString("kDiffsDefaultApp");
            this.m_DiffToolIndex = ArrayUtility.IndexOf<string>(this.m_DiffTools, str2);
            if (this.m_DiffToolIndex == -1)
            {
                this.m_DiffToolIndex = 0;
            }
            this.m_AutoRefresh = EditorPrefs.GetBool("kAutoRefresh");
            this.m_ReopenLastUsedProjectOnStartup = EditorPrefs.GetBool("ReopenLastUsedProjectOnStartup");
            this.m_UseOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
            this.m_EnableEditorAnalytics = EditorPrefs.GetBool("EnableEditorAnalytics", true);
            this.m_ShowAssetStoreSearchHits = EditorPrefs.GetBool("ShowAssetStoreSearchHits", true);
            this.m_VerifySavingAssets = EditorPrefs.GetBool("VerifySavingAssets", false);
            this.m_GICacheSettings.m_EnableCustomPath = EditorPrefs.GetBool("GICacheEnableCustomPath");
            this.m_GICacheSettings.m_CachePath = EditorPrefs.GetString("GICacheFolder");
            this.m_GICacheSettings.m_MaximumSize = EditorPrefs.GetInt("GICacheMaximumSizeGB");
            this.m_GICacheSettings.m_CompressionLevel = EditorPrefs.GetInt("GICacheCompressionLevel");
            this.m_SpriteAtlasCacheSize = EditorPrefs.GetInt("SpritePackerCacheMaximumSizeGB");
            this.m_AllowAttachedDebuggingOfEditor = EditorPrefs.GetBool("AllowAttachedDebuggingOfEditor", true);
            this.m_SelectedLanguage = LocalizationDatabase.GetCurrentEditorLanguage();
            this.m_AllowAlphaNumericHierarchy = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
            this.m_CompressAssetsOnImport = Unsupported.GetApplicationSettingCompressAssetsOnImport();
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                extension.ReadPreferences();
            }
        }

        private void RevertColors()
        {
            IEnumerator<KeyValuePair<string, PrefColor>> enumerator = Settings.Prefs<PrefColor>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, PrefColor> current = enumerator.Current;
                    current.Value.ResetToDefault();
                    EditorPrefs.SetString(current.Value.Name, current.Value.ToUniqueString());
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private void RevertKeys()
        {
            IEnumerator<KeyValuePair<string, PrefKey>> enumerator = Settings.Prefs<PrefKey>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, PrefKey> current = enumerator.Current;
                    current.Value.ResetToDefault();
                    EditorPrefs.SetString(current.Value.Name, current.Value.ToUniqueString());
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static void SetupDefaultPreferences()
        {
        }

        private void Show2D()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(Styles.spriteMaxCacheSize, EditorStyles.popup);
            this.m_SpriteAtlasCacheSize = EditorGUILayout.IntSlider(this.m_SpriteAtlasCacheSize, kMinSpriteCacheSizeInGigabytes, kMaxSpriteCacheSizeInGigabytes, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                this.WritePreferences();
            }
        }

        private void ShowColors()
        {
            if (this.s_CachedColors == null)
            {
                this.s_CachedColors = this.OrderPrefs<PrefColor>(Settings.Prefs<PrefColor>());
            }
            bool flag = false;
            s_ColorScrollPos = EditorGUILayout.BeginScrollView(s_ColorScrollPos, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            PrefColor color = null;
            foreach (KeyValuePair<string, List<KeyValuePair<string, PrefColor>>> pair in this.s_CachedColors)
            {
                GUILayout.Label(pair.Key, EditorStyles.boldLabel, new GUILayoutOption[0]);
                foreach (KeyValuePair<string, PrefColor> pair2 in pair.Value)
                {
                    EditorGUI.BeginChangeCheck();
                    Color color2 = EditorGUILayout.ColorField(pair2.Key, pair2.Value.Color, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        color = pair2.Value;
                        color.Color = color2;
                        flag = true;
                    }
                }
                if (color != null)
                {
                    Settings.Set<PrefColor>(color.Name, color);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (GUILayout.Button("Use Defaults", options))
            {
                this.RevertColors();
                flag = true;
            }
            if (flag)
            {
                EditorApplication.RequestRepaintAllViews();
            }
        }

        private void ShowExternalApplications()
        {
            GUILayout.Space(10f);
            this.FilePopup("External Script Editor", (string) this.m_ScriptEditorPath, ref this.m_ScriptAppDisplayNames, ref this.m_ScriptApps, this.m_ScriptEditorPath, "internal", new Action(this.OnScriptEditorChanged));
            if (!this.IsSelectedScriptEditorSpecial() && (Application.platform != RuntimePlatform.OSXEditor))
            {
                string scriptEditorArgs = this.m_ScriptEditorArgs;
                this.m_ScriptEditorArgs = EditorGUILayout.TextField("External Script Editor Args", this.m_ScriptEditorArgs, new GUILayoutOption[0]);
                if (scriptEditorArgs != this.m_ScriptEditorArgs)
                {
                    this.OnScriptEditorArgsChanged();
                }
            }
            this.DoUnityProjCheckbox();
            bool allowAttachedDebuggingOfEditor = this.m_AllowAttachedDebuggingOfEditor;
            this.m_AllowAttachedDebuggingOfEditor = EditorGUILayout.Toggle("Editor Attaching", this.m_AllowAttachedDebuggingOfEditor, new GUILayoutOption[0]);
            if (allowAttachedDebuggingOfEditor != this.m_AllowAttachedDebuggingOfEditor)
            {
                this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession = true;
            }
            if (this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession)
            {
                GUILayout.Label("Changing this setting requires a restart to take effect.", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            if (this.m_ScriptEditorPath.str.Contains("VCSExpress"))
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label(string.Empty, "CN EntryWarn", new GUILayoutOption[0]);
                GUILayout.Label("Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)", constants.errorLabel, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
            this.FilePopup("Image application", (string) this.m_ImageAppPath, ref this.m_ImageAppDisplayNames, ref this.m_ImageApps, this.m_ImageAppPath, "internal", null);
            GUILayout.Space(10f);
            EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasTeamLicense());
            this.m_DiffToolIndex = EditorGUILayout.Popup("Revision Control Diff/Merge", this.m_DiffToolIndex, this.m_DiffTools, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (this.m_noDiffToolsMessage != string.Empty)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label(string.Empty, "CN EntryWarn", new GUILayoutOption[0]);
                GUILayout.Label(this.m_noDiffToolsMessage, constants.errorLabel, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                if (extension.HasExternalApplications())
                {
                    GUILayout.Space(10f);
                    extension.ShowExternalApplications();
                }
            }
            this.ApplyChangesToPrefs();
        }

        private void ShowGeneral()
        {
            GUILayout.Space(10f);
            this.m_AutoRefresh = EditorGUILayout.Toggle("Auto Refresh", this.m_AutoRefresh, new GUILayoutOption[0]);
            this.m_ReopenLastUsedProjectOnStartup = EditorGUILayout.Toggle("Load Previous Project on Startup", this.m_ReopenLastUsedProjectOnStartup, new GUILayoutOption[0]);
            bool compressAssetsOnImport = this.m_CompressAssetsOnImport;
            this.m_CompressAssetsOnImport = EditorGUILayout.Toggle("Compress Assets on Import", compressAssetsOnImport, new GUILayoutOption[0]);
            if (GUI.changed && (this.m_CompressAssetsOnImport != compressAssetsOnImport))
            {
                Unsupported.SetApplicationSettingCompressAssetsOnImport(this.m_CompressAssetsOnImport);
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                this.m_UseOSColorPicker = EditorGUILayout.Toggle("OS X Color Picker", this.m_UseOSColorPicker, new GUILayoutOption[0]);
            }
            bool flag2 = Application.HasProLicense();
            EditorGUI.BeginDisabledGroup(!flag2);
            this.m_EnableEditorAnalytics = !EditorGUILayout.Toggle("Disable Editor Analytics (Pro Only)", !this.m_EnableEditorAnalytics, new GUILayoutOption[0]);
            if (!flag2 && !this.m_EnableEditorAnalytics)
            {
                this.m_EnableEditorAnalytics = true;
            }
            EditorGUI.EndDisabledGroup();
            bool flag3 = false;
            EditorGUI.BeginChangeCheck();
            this.m_ShowAssetStoreSearchHits = EditorGUILayout.Toggle("Show Asset Store search hits", this.m_ShowAssetStoreSearchHits, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                flag3 = true;
            }
            this.m_VerifySavingAssets = EditorGUILayout.Toggle("Verify Saving Assets", this.m_VerifySavingAssets, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(!flag2);
            string[] displayedOptions = new string[] { "Personal", "Professional" };
            int num = EditorGUILayout.Popup("Editor Skin", EditorGUIUtility.isProSkin ? 1 : 0, displayedOptions, new GUILayoutOption[0]);
            if ((EditorGUIUtility.isProSkin ? 1 : 0) != num)
            {
                InternalEditorUtility.SwitchSkinAndRepaintAllViews();
            }
            EditorGUI.EndDisabledGroup();
            bool allowAlphaNumericHierarchy = this.m_AllowAlphaNumericHierarchy;
            this.m_AllowAlphaNumericHierarchy = EditorGUILayout.Toggle("Enable Alpha Numeric Sorting", this.m_AllowAlphaNumericHierarchy, new GUILayoutOption[0]);
            bool flag5 = false;
            SystemLanguage selectedLanguage = this.m_SelectedLanguage;
            SystemLanguage[] availableEditorLanguages = LocalizationDatabase.GetAvailableEditorLanguages();
            if (availableEditorLanguages.Length > 1)
            {
                if (this.m_EditorLanguageNames == null)
                {
                    this.m_EditorLanguageNames = new string[availableEditorLanguages.Length];
                    for (int i = 0; i < availableEditorLanguages.Length; i++)
                    {
                        this.m_EditorLanguageNames[i] = availableEditorLanguages[i].ToString();
                    }
                    string item = string.Format("Default ( {0} )", LocalizationDatabase.GetDefaultEditorLanguage().ToString());
                    ArrayUtility.Insert<string>(ref this.m_EditorLanguageNames, 0, string.Empty);
                    ArrayUtility.Insert<string>(ref this.m_EditorLanguageNames, 0, item);
                }
                EditorGUI.BeginChangeCheck();
                selectedLanguage = this.m_SelectedLanguage;
                int selectedIndex = 2 + Array.IndexOf<SystemLanguage>(availableEditorLanguages, this.m_SelectedLanguage);
                int num4 = EditorGUILayout.Popup("Language", selectedIndex, this.m_EditorLanguageNames, new GUILayoutOption[0]);
                this.m_SelectedLanguage = (num4 != 0) ? availableEditorLanguages[num4 - 2] : LocalizationDatabase.GetDefaultEditorLanguage();
                if (EditorGUI.EndChangeCheck() && (selectedLanguage != this.m_SelectedLanguage))
                {
                    flag5 = true;
                }
            }
            this.ApplyChangesToPrefs();
            if (flag5)
            {
                EditorGUIUtility.NotifyLanguageChanged(this.m_SelectedLanguage);
                InternalEditorUtility.RequestScriptReload();
            }
            if (allowAlphaNumericHierarchy != this.m_AllowAlphaNumericHierarchy)
            {
                EditorApplication.DirtyHierarchyWindowSorting();
            }
            if (flag3)
            {
                ProjectBrowser.ShowAssetStoreHitsWhileSearchingLocalAssetsChanged();
            }
        }

        private void ShowGICache()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(Styles.maxCacheSize, EditorStyles.popup);
            this.m_GICacheSettings.m_MaximumSize = EditorGUILayout.IntSlider(this.m_GICacheSettings.m_MaximumSize, 5, 200, new GUILayoutOption[0]);
            this.WritePreferences();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (Lightmapping.isRunning)
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent(Styles.cantChangeCacheSettings.text).text, MessageType.Warning, true);
            }
            GUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(Lightmapping.isRunning);
            this.m_GICacheSettings.m_EnableCustomPath = EditorGUILayout.Toggle(Styles.customCacheLocation, this.m_GICacheSettings.m_EnableCustomPath, new GUILayoutOption[0]);
            if (this.m_GICacheSettings.m_EnableCustomPath)
            {
                GUIStyle popup = EditorStyles.popup;
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.PrefixLabel(Styles.cacheFolderLocation, popup);
                Rect position = GUILayoutUtility.GetRect(GUIContent.none, popup);
                GUIContent content = !string.IsNullOrEmpty(this.m_GICacheSettings.m_CachePath) ? new GUIContent(this.m_GICacheSettings.m_CachePath) : Styles.browse;
                if (EditorGUI.ButtonMouseDown(position, content, FocusType.Native, popup))
                {
                    string cachePath = this.m_GICacheSettings.m_CachePath;
                    string str2 = EditorUtility.OpenFolderPanel(Styles.browseGICacheLocation.text, cachePath, string.Empty);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        this.m_GICacheSettings.m_CachePath = str2;
                        this.WritePreferences();
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                this.m_GICacheSettings.m_CachePath = string.Empty;
            }
            this.m_GICacheSettings.m_CompressionLevel = !EditorGUILayout.Toggle(Styles.cacheCompression, this.m_GICacheSettings.m_CompressionLevel == 1, new GUILayoutOption[0]) ? 0 : 1;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (GUILayout.Button(Styles.cleanCache, options))
            {
                EditorUtility.DisplayProgressBar(Styles.cleanCache.text, Styles.pleaseWait.text, 0f);
                Lightmapping.Clear();
                EditorUtility.DisplayProgressBar(Styles.cleanCache.text, Styles.pleaseWait.text, 0.5f);
                Lightmapping.ClearDiskCache();
                EditorUtility.ClearProgressBar();
            }
            if (Lightmapping.diskCacheSize >= 0L)
            {
                GUILayout.Label(Styles.cacheSizeIs.text + " " + EditorUtility.FormatBytes(Lightmapping.diskCacheSize), new GUILayoutOption[0]);
            }
            else
            {
                GUILayout.Label(Styles.cacheSizeIs.text + " is being calculated...", new GUILayoutOption[0]);
            }
            GUILayout.Label(Styles.cacheFolderLocation.text + ":", new GUILayoutOption[0]);
            GUILayout.Label(Lightmapping.diskCachePath, constants.cacheFolderLocation, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
        }

        private void ShowKeys()
        {
            int controlID = GUIUtility.GetControlID(s_KeysControlHash, FocusType.Keyboard);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(185f) };
            GUILayout.BeginVertical(options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label("Actions", constants.settingsBoxTitle, optionArray2);
            this.m_KeyScrollPos = GUILayout.BeginScrollView(this.m_KeyScrollPos, constants.settingsBox);
            PrefKey key = null;
            PrefKey key2 = null;
            bool flag = false;
            IEnumerator<KeyValuePair<string, PrefKey>> enumerator = Settings.Prefs<PrefKey>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, PrefKey> current = enumerator.Current;
                    if (!flag)
                    {
                        if (current.Value == this.m_SelectedKey)
                        {
                            flag = true;
                        }
                        else
                        {
                            key = current.Value;
                        }
                    }
                    else if (key2 == null)
                    {
                        key2 = current.Value;
                    }
                    EditorGUI.BeginChangeCheck();
                    if (GUILayout.Toggle(current.Value == this.m_SelectedKey, current.Key, constants.keysElement, new GUILayoutOption[0]))
                    {
                        this.m_SelectedKey = current.Value;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        GUIUtility.keyboardControl = controlID;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (this.m_SelectedKey != null)
            {
                Event keyboardEvent = this.m_SelectedKey.KeyboardEvent;
                GUI.changed = false;
                char[] separator = new char[] { '/' };
                string[] strArray = this.m_SelectedKey.Name.Split(separator);
                GUILayout.Label(strArray[0], "boldLabel", new GUILayoutOption[0]);
                GUILayout.Label(strArray[1], "boldLabel", new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("Key:", new GUILayoutOption[0]);
                keyboardEvent = EditorGUILayout.KeyEventField(keyboardEvent, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("Modifiers:", new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    keyboardEvent.command = GUILayout.Toggle(keyboardEvent.command, "Command", new GUILayoutOption[0]);
                }
                keyboardEvent.control = GUILayout.Toggle(keyboardEvent.control, "Control", new GUILayoutOption[0]);
                keyboardEvent.shift = GUILayout.Toggle(keyboardEvent.shift, "Shift", new GUILayoutOption[0]);
                keyboardEvent.alt = GUILayout.Toggle(keyboardEvent.alt, "Alt", new GUILayoutOption[0]);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                if (GUI.changed)
                {
                    this.m_SelectedKey.KeyboardEvent = keyboardEvent;
                    Settings.Set<PrefKey>(this.m_SelectedKey.Name, this.m_SelectedKey);
                }
                else if ((GUIUtility.keyboardControl == controlID) && (Event.current.type == EventType.KeyDown))
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.UpArrow:
                            if (key != null)
                            {
                                this.m_SelectedKey = key;
                            }
                            Event.current.Use();
                            break;

                        case KeyCode.DownArrow:
                            if (key2 != null)
                            {
                                this.m_SelectedKey = key2;
                            }
                            Event.current.Use();
                            break;
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (GUILayout.Button("Use Defaults", optionArray3))
            {
                this.RevertKeys();
            }
        }

        private static void ShowPreferencesWindow()
        {
            EditorWindow.GetWindowWithRect<PreferencesWindow>(new Rect(100f, 100f, 500f, 400f), true, "Unity Preferences").m_Parent.window.m_DontSaveToLayout = true;
        }

        private void ShowUnityConnectPrefs()
        {
            UnityConnectPrefs.ShowPanelPrefUI();
            this.ApplyChangesToPrefs();
        }

        private string StripMicrosoftFromVisualStudioName(string arg)
        {
            if (!arg.Contains("Visual Studio"))
            {
                return arg;
            }
            if (!arg.StartsWith("Microsoft"))
            {
                return arg;
            }
            return arg.Substring("Microsoft ".Length);
        }

        private void WritePreferences()
        {
            EditorPrefs.SetString("kScriptsDefaultApp", (string) this.m_ScriptEditorPath);
            EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
            EditorPrefs.SetBool("kExternalEditorSupportsUnityProj", this.m_ExternalEditorSupportsUnityProj);
            EditorPrefs.SetString("kImagesDefaultApp", (string) this.m_ImageAppPath);
            EditorPrefs.SetString("kDiffsDefaultApp", (this.m_DiffTools.Length != 0) ? this.m_DiffTools[this.m_DiffToolIndex] : string.Empty);
            this.WriteRecentAppsList(this.m_ScriptApps, (string) this.m_ScriptEditorPath, "RecentlyUsedScriptApp");
            this.WriteRecentAppsList(this.m_ImageApps, (string) this.m_ImageAppPath, "RecentlyUsedImageApp");
            EditorPrefs.SetBool("kAutoRefresh", this.m_AutoRefresh);
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                UnityConnectPrefs.StorePanelPrefs();
            }
            EditorPrefs.SetBool("ReopenLastUsedProjectOnStartup", this.m_ReopenLastUsedProjectOnStartup);
            EditorPrefs.SetBool("UseOSColorPicker", this.m_UseOSColorPicker);
            EditorPrefs.SetBool("EnableEditorAnalytics", this.m_EnableEditorAnalytics);
            EditorPrefs.SetBool("ShowAssetStoreSearchHits", this.m_ShowAssetStoreSearchHits);
            EditorPrefs.SetBool("VerifySavingAssets", this.m_VerifySavingAssets);
            EditorPrefs.SetBool("AllowAttachedDebuggingOfEditor", this.m_AllowAttachedDebuggingOfEditor);
            LocalizationDatabase.SetCurrentEditorLanguage(this.m_SelectedLanguage);
            EditorPrefs.SetBool("AllowAlphaNumericHierarchy", this.m_AllowAlphaNumericHierarchy);
            EditorPrefs.SetBool("GICacheEnableCustomPath", this.m_GICacheSettings.m_EnableCustomPath);
            EditorPrefs.SetInt("GICacheMaximumSizeGB", this.m_GICacheSettings.m_MaximumSize);
            EditorPrefs.SetString("GICacheFolder", this.m_GICacheSettings.m_CachePath);
            EditorPrefs.SetInt("GICacheCompressionLevel", this.m_GICacheSettings.m_CompressionLevel);
            EditorPrefs.SetInt("SpritePackerCacheMaximumSizeGB", this.m_SpriteAtlasCacheSize);
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                extension.WritePreferences();
            }
            Lightmapping.UpdateCachePath();
        }

        private void WriteRecentAppsList(string[] paths, string path, string prefsKey)
        {
            int num = 0;
            if (path.Length != 0)
            {
                EditorPrefs.SetString(prefsKey + num, path);
                num++;
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (num >= 10)
                {
                    break;
                }
                if ((paths[i].Length != 0) && (paths[i] != path))
                {
                    EditorPrefs.SetString(prefsKey + num, paths[i]);
                    num++;
                }
            }
        }

        private Section selectedSection
        {
            get
            {
                return this.m_Sections[this.m_SelectedSectionIndex];
            }
        }

        private int selectedSectionIndex
        {
            get
            {
                return this.m_SelectedSectionIndex;
            }
            set
            {
                this.m_SelectedSectionIndex = value;
                if (this.m_SelectedSectionIndex >= this.m_Sections.Count)
                {
                    this.m_SelectedSectionIndex = 0;
                }
                else if (this.m_SelectedSectionIndex < 0)
                {
                    this.m_SelectedSectionIndex = this.m_Sections.Count - 1;
                }
            }
        }

        private class AppsListUserData
        {
            public Action onChanged;
            public string[] paths;
            public PreferencesWindow.RefString str;

            public AppsListUserData(string[] paths, PreferencesWindow.RefString str, Action onChanged)
            {
                this.paths = paths;
                this.str = str;
                this.onChanged = onChanged;
            }
        }

        internal class Constants
        {
            public GUIStyle cacheFolderLocation = new GUIStyle(GUI.skin.label);
            public GUIStyle errorLabel = "WordWrappedLabel";
            public GUIStyle evenRow = "CN EntryBackEven";
            public GUIStyle keysElement = "PreferencesKeysElement";
            public GUIStyle oddRow = "CN EntryBackOdd";
            public GUIStyle sectionElement = "PreferencesSection";
            public GUIStyle sectionHeader = new GUIStyle(EditorStyles.largeLabel);
            public GUIStyle sectionScrollView = "PreferencesSectionBox";
            public GUIStyle selected = "ServerUpdateChangesetOn";
            public GUIStyle settingsBox = "OL Box";
            public GUIStyle settingsBoxTitle = "OL Title";

            public Constants()
            {
                this.sectionScrollView = new GUIStyle(this.sectionScrollView);
                RectOffset overflow = this.sectionScrollView.overflow;
                overflow.bottom++;
                this.sectionHeader.fontStyle = FontStyle.Bold;
                this.sectionHeader.fontSize = 0x12;
                this.sectionHeader.margin.top = 10;
                RectOffset margin = this.sectionHeader.margin;
                margin.left++;
                if (!EditorGUIUtility.isProSkin)
                {
                    this.sectionHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
                else
                {
                    this.sectionHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                this.cacheFolderLocation.wordWrap = true;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GICacheSettings
        {
            public bool m_EnableCustomPath;
            public int m_MaximumSize;
            public string m_CachePath;
            public int m_CompressionLevel;
        }

        private delegate void OnGUIDelegate();

        private class RefString
        {
            public string str;

            public RefString(string s)
            {
                this.str = s;
            }

            public static implicit operator string(PreferencesWindow.RefString s)
            {
                return s.str;
            }

            public override string ToString()
            {
                return this.str;
            }
        }

        private class Section
        {
            public GUIContent content;
            public PreferencesWindow.OnGUIDelegate guiFunc;

            public Section(string name, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = new GUIContent(name);
                this.guiFunc = guiFunc;
            }

            public Section(GUIContent content, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = content;
                this.guiFunc = guiFunc;
            }

            public Section(string name, Texture2D icon, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = new GUIContent(name, icon);
                this.guiFunc = guiFunc;
            }
        }

        internal class Styles
        {
            public static readonly GUIContent browse = EditorGUIUtility.TextContent("Browse...");
            public static readonly GUIContent browseGICacheLocation = EditorGUIUtility.TextContent("Browse for GI Cache location");
            public static readonly GUIContent cacheCompression = EditorGUIUtility.TextContent("Cache compression|Use fast realtime compression for the GI cache files to reduce the size of generated data. Disable it and clean the cache if you need access to the raw data generated by Enlighten.");
            public static readonly GUIContent cacheFolderLocation = EditorGUIUtility.TextContent("Cache Folder Location|The GI Cache folder is shared between all projects.");
            public static readonly GUIContent cacheSizeIs = EditorGUIUtility.TextContent("Cache size is");
            public static readonly GUIContent cantChangeCacheSettings = EditorGUIUtility.TextContent("Cache settings can't be changed while lightmapping is being computed.");
            public static readonly GUIContent cleanCache = EditorGUIUtility.TextContent("Clean Cache");
            public static readonly GUIContent customCacheLocation = EditorGUIUtility.TextContent("Custom cache location|Specify the GI Cache folder location.");
            public static readonly GUIContent maxCacheSize = EditorGUIUtility.TextContent("Maximum Cache Size (GB)|The size of the GI Cache folder will be kept below this maximum value when possible. A background job will periodically clean up the oldest unused files.");
            public static readonly GUIContent pleaseWait = EditorGUIUtility.TextContent("Please wait...");
            public static readonly GUIContent spriteMaxCacheSize = EditorGUIUtility.TextContent("Maximum Sprite Atlas Cache Size (GB)|The size of the Sprite Atlas Cache folder will be kept below this maximum value when possible. Change requires Editor restart");
        }
    }
}

