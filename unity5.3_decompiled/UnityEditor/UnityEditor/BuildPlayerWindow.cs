namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor.Connect;
    using UnityEditor.Modules;
    using UnityEditor.SceneManagement;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class BuildPlayerWindow : EditorWindow
    {
        private int initialSelectedLVItem = -1;
        private const string kAssetsFolder = "Assets/";
        private const string kEditorBuildSettingsPath = "ProjectSettings/EditorBuildSettings.asset";
        private ListViewState lv = new ListViewState();
        private static BuildPlatforms s_BuildPlatforms;
        private Vector2 scrollPosition = new Vector2(0f, 0f);
        private bool[] selectedBeforeDrag;
        private bool[] selectedLVItems = new bool[0];
        private static Styles styles;

        public BuildPlayerWindow()
        {
            base.position = new Rect(50f, 50f, 540f, 530f);
            base.minSize = new Vector2(550f, 580f);
            base.titleContent = new GUIContent("Build Settings");
        }

        private void ActiveBuildTargetsGUI()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(255f) };
            GUILayout.BeginVertical(options);
            GUILayout.Label(styles.platformTitle, styles.title, new GUILayoutOption[0]);
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, "OL Box");
            for (int i = 0; i < 2; i++)
            {
                bool flag = i == 0;
                bool flag2 = false;
                foreach (BuildPlatform platform in s_BuildPlatforms.buildPlatforms)
                {
                    if ((IsBuildTargetGroupSupported(platform.DefaultTarget) == flag) && (IsBuildTargetGroupSupported(platform.DefaultTarget) || platform.forceShowTarget))
                    {
                        this.ShowOption(platform, platform.title, !flag2 ? styles.oddRow : styles.evenRow);
                        flag2 = !flag2;
                    }
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            BuildTarget target = CalculateSelectedBuildTarget();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.enabled = BuildPipeline.IsBuildTargetSupported(target) && (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) != BuildPipeline.GetBuildTargetGroup(target));
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(styles.switchPlatform, optionArray2))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(target);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = BuildPipeline.IsBuildTargetSupported(target);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(new GUIContent("Player Settings..."), optionArray3))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUILayout.EndVertical();
        }

        private void ActiveScenesGUI()
        {
            int num;
            int num2;
            int num3 = 0;
            int row = this.lv.row;
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            Event current = Event.current;
            Rect position = GUILayoutUtility.GetRect(styles.scenesInBuild, styles.title);
            ArrayList list = new ArrayList(EditorBuildSettings.scenes);
            this.lv.totalRows = list.Count;
            if (this.selectedLVItems.Length != list.Count)
            {
                Array.Resize<bool>(ref this.selectedLVItems, list.Count);
            }
            int[] numArray = new int[list.Count];
            for (num = 0; num < numArray.Length; num++)
            {
                EditorBuildSettingsScene scene = (EditorBuildSettingsScene) list[num];
                numArray[num] = num3;
                if (scene.enabled)
                {
                    num3++;
                }
            }
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, ListViewOptions.wantsExternalFiles | ListViewOptions.wantsReordering, styles.box, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement element = (ListViewElement) enumerator.Current;
                    EditorBuildSettingsScene scene2 = (EditorBuildSettingsScene) list[element.row];
                    bool flag3 = File.Exists(scene2.path);
                    EditorGUI.BeginDisabledGroup(!flag3);
                    bool on = this.selectedLVItems[element.row];
                    if (on && (current.type == EventType.Repaint))
                    {
                        styles.selected.Draw(element.position, false, false, false, false);
                    }
                    if (!flag3)
                    {
                        scene2.enabled = false;
                    }
                    Rect rect2 = new Rect(element.position.x + 4f, element.position.y, styles.toggleSize.x, styles.toggleSize.y);
                    EditorGUI.BeginChangeCheck();
                    scene2.enabled = GUI.Toggle(rect2, scene2.enabled, string.Empty);
                    if (EditorGUI.EndChangeCheck() && on)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (this.selectedLVItems[i])
                            {
                                ((EditorBuildSettingsScene) list[i]).enabled = scene2.enabled;
                            }
                        }
                    }
                    GUILayout.Space(styles.toggleSize.x);
                    string path = scene2.path;
                    if (path.StartsWith("Assets/"))
                    {
                        path = path.Substring("Assets/".Length);
                    }
                    Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(path), styles.levelString);
                    if (Event.current.type == EventType.Repaint)
                    {
                        styles.levelString.Draw(rect, EditorGUIUtility.TempContent(path), false, false, on, false);
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(36f) };
                    GUILayout.Label(!scene2.enabled ? string.Empty : numArray[element.row].ToString(), styles.levelStringCounter, options);
                    EditorGUI.EndDisabledGroup();
                    if ((ListViewGUILayout.HasMouseUp(element.position) && !shift) && !actionKey)
                    {
                        if (!shift && !actionKey)
                        {
                            ListViewGUILayout.MultiSelection(row, element.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(element.position))
                    {
                        if ((!this.selectedLVItems[element.row] || shift) || actionKey)
                        {
                            ListViewGUILayout.MultiSelection(row, element.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
                        }
                        this.lv.row = element.row;
                        this.selectedBeforeDrag = new bool[this.selectedLVItems.Length];
                        this.selectedLVItems.CopyTo(this.selectedBeforeDrag, 0);
                        this.selectedBeforeDrag[this.lv.row] = true;
                    }
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
            GUI.Label(position, styles.scenesInBuild, styles.title);
            if (GUIUtility.keyboardControl == this.lv.ID)
            {
                if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                {
                    Event.current.Use();
                }
                else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                {
                    for (num = 0; num < this.selectedLVItems.Length; num++)
                    {
                        this.selectedLVItems[num] = true;
                    }
                    this.lv.selectionChanged = true;
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }
            }
            if (this.lv.selectionChanged)
            {
                ListViewGUILayout.MultiSelection(row, this.lv.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
            }
            if (this.lv.fileNames != null)
            {
                Array.Sort<string>(this.lv.fileNames);
                int num6 = 0;
                for (num = 0; num < this.lv.fileNames.Length; num++)
                {
                    if (this.lv.fileNames[num].EndsWith("unity"))
                    {
                        EditorBuildSettingsScene scene3 = new EditorBuildSettingsScene {
                            path = FileUtil.GetProjectRelativePath(this.lv.fileNames[num])
                        };
                        if (scene3.path == string.Empty)
                        {
                            scene3.path = this.lv.fileNames[num];
                        }
                        scene3.enabled = true;
                        list.Insert(this.lv.draggedTo + num6++, scene3);
                    }
                }
                if (num6 != 0)
                {
                    Array.Resize<bool>(ref this.selectedLVItems, list.Count);
                    for (num = 0; num < this.selectedLVItems.Length; num++)
                    {
                        this.selectedLVItems[num] = (num >= this.lv.draggedTo) && (num < (this.lv.draggedTo + num6));
                    }
                }
                this.lv.draggedTo = -1;
            }
            if (this.lv.draggedTo != -1)
            {
                ArrayList c = new ArrayList();
                num2 = 0;
                num = 0;
                while (num < this.selectedLVItems.Length)
                {
                    if (this.selectedBeforeDrag[num])
                    {
                        c.Add(list[num2]);
                        list.RemoveAt(num2);
                        num2--;
                        if (this.lv.draggedTo >= num)
                        {
                            this.lv.draggedTo--;
                        }
                    }
                    num++;
                    num2++;
                }
                this.lv.draggedTo = ((this.lv.draggedTo <= list.Count) && (this.lv.draggedTo >= 0)) ? this.lv.draggedTo : list.Count;
                list.InsertRange(this.lv.draggedTo, c);
                for (num = 0; num < this.selectedLVItems.Length; num++)
                {
                    this.selectedLVItems[num] = (num >= this.lv.draggedTo) && (num < (this.lv.draggedTo + c.Count));
                }
            }
            if (((current.type == EventType.KeyDown) && ((current.keyCode == KeyCode.Backspace) || (current.keyCode == KeyCode.Delete))) && (GUIUtility.keyboardControl == this.lv.ID))
            {
                num2 = 0;
                num = 0;
                while (num < this.selectedLVItems.Length)
                {
                    if (this.selectedLVItems[num])
                    {
                        list.RemoveAt(num2);
                        num2--;
                    }
                    this.selectedLVItems[num] = false;
                    num++;
                    num2++;
                }
                this.lv.row = 0;
                current.Use();
            }
            EditorBuildSettings.scenes = list.ToArray(typeof(EditorBuildSettingsScene)) as EditorBuildSettingsScene[];
        }

        private void AddOpenScenes()
        {
            List<EditorBuildSettingsScene> source = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool flag = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                <AddOpenScenes>c__AnonStorey30 storey = new <AddOpenScenes>c__AnonStorey30 {
                    scene = SceneManager.GetSceneAt(i)
                };
                if (((storey.scene.path.Length != 0) || EditorSceneManager.SaveScene(storey.scene, string.Empty, false)) && !source.Any<EditorBuildSettingsScene>(new Func<EditorBuildSettingsScene, bool>(storey.<>m__47)))
                {
                    EditorBuildSettingsScene item = new EditorBuildSettingsScene {
                        path = storey.scene.path,
                        enabled = true
                    };
                    source.Add(item);
                    flag = true;
                }
            }
            if (flag)
            {
                EditorBuildSettings.scenes = source.ToArray();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        private static void BuildPlayerAndRun()
        {
            if (!BuildPlayerWithDefaultSettings(false, BuildOptions.AutoRunPlayer))
            {
                ShowBuildPlayerWindow();
            }
        }

        private static void BuildPlayerAndSelect()
        {
            if (!BuildPlayerWithDefaultSettings(false, BuildOptions.ShowBuiltPlayer))
            {
                ShowBuildPlayerWindow();
            }
        }

        private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions)
        {
            return BuildPlayerWithDefaultSettings(askForBuildLocation, forceOptions, true);
        }

        private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions, bool first)
        {
            bool updateExistingBuild = false;
            InitBuildPlatforms();
            if (!UnityConnect.instance.canBuildWithUPID && !EditorUtility.DisplayDialog("Missing Project ID", "Because you are not a member of this project this build will not access Unity services.\nDo you want to continue?", "Yes", "No"))
            {
                return false;
            }
            BuildTarget target = CalculateSelectedBuildTarget();
            if (!BuildPipeline.IsBuildTargetSupported(target))
            {
                return false;
            }
            IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(ModuleManager.GetTargetStringFromBuildTargetGroup(s_BuildPlatforms.BuildPlatformFromTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup).targetGroup));
            if (((buildWindowExtension != null) && ((forceOptions & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures)) && !buildWindowExtension.EnabledBuildAndRunButton())
            {
                return false;
            }
            if (Unsupported.IsBleedingEdgeBuild())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("This version of Unity is a BleedingEdge build that has not seen any manual testing.");
                builder.AppendLine("You should consider this build unstable.");
                builder.AppendLine("We strongly recommend that you use a normal version of Unity instead.");
                if (EditorUtility.DisplayDialog("BleedingEdge Build", builder.ToString(), "Cancel", "OK"))
                {
                    return false;
                }
            }
            if (((target == BuildTarget.BlackBerry) && ((forceOptions & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures)) && (string.IsNullOrEmpty(PlayerSettings.BlackBerry.deviceAddress) || string.IsNullOrEmpty(PlayerSettings.BlackBerry.devicePassword)))
            {
                Debug.LogError(EditorGUIUtility.TextContent("Author Id, Device Address and Device Password must all be set in order to use Build and Run").text);
                return false;
            }
            string location = string.Empty;
            bool flag2 = (EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(target)) && ((Unsupported.IsDeveloperBuild() || IsMetroPlayer(target)) || IsWP8Player(target));
            BuildOptions options = forceOptions;
            bool development = EditorUserBuildSettings.development;
            if (development)
            {
                options |= BuildOptions.Development;
            }
            if (EditorUserBuildSettings.allowDebugging && development)
            {
                options |= BuildOptions.AllowDebugging;
            }
            if (EditorUserBuildSettings.symlinkLibraries)
            {
                options |= BuildOptions.SymlinkLibraries;
            }
            if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                options |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            if (EditorUserBuildSettings.webPlayerOfflineDeployment)
            {
                options |= BuildOptions.WebPlayerOfflineDeployment;
            }
            if (EditorUserBuildSettings.enableHeadlessMode)
            {
                options |= BuildOptions.EnableHeadlessMode;
            }
            if (EditorUserBuildSettings.connectProfiler && ((development || (target == BuildTarget.WSAPlayer)) || IsWP8Player(target)))
            {
                options |= BuildOptions.ConnectWithProfiler;
            }
            if (EditorUserBuildSettings.buildScriptsOnly)
            {
                options |= BuildOptions.BuildScriptsOnly;
            }
            if (EditorUserBuildSettings.forceOptimizeScriptCompilation)
            {
                options |= BuildOptions.ForceOptimizeScriptCompilation;
            }
            if (flag2)
            {
                options |= BuildOptions.InstallInBuildFolder;
            }
            if (!flag2)
            {
                if (askForBuildLocation && !PickBuildLocation(target, options, out updateExistingBuild))
                {
                    return false;
                }
                location = EditorUserBuildSettings.GetBuildLocation(target);
                if (location.Length == 0)
                {
                    return false;
                }
                if (!askForBuildLocation)
                {
                    switch (InternalEditorUtility.BuildCanBeAppended(target, location))
                    {
                        case CanAppendBuild.Yes:
                            updateExistingBuild = true;
                            break;

                        case CanAppendBuild.No:
                            if (!PickBuildLocation(target, options, out updateExistingBuild))
                            {
                                return false;
                            }
                            location = EditorUserBuildSettings.GetBuildLocation(target);
                            if ((location.Length == 0) || !Directory.Exists(FileUtil.DeleteLastPathNameComponent(location)))
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            if (updateExistingBuild)
            {
                options |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            ArrayList list = new ArrayList();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    list.Add(scene.path);
                }
            }
            string[] levels = list.ToArray(typeof(string)) as string[];
            bool delayToAfterScriptReload = false;
            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(target))
                {
                    object[] args = new object[] { s_BuildPlatforms.GetBuildTargetDisplayName(target) };
                    Debug.LogErrorFormat("Could not switch to build target '{0}'.", args);
                    return false;
                }
                if (EditorApplication.isCompiling)
                {
                    delayToAfterScriptReload = true;
                }
            }
            uint crc = 0;
            return (BuildPipeline.BuildPlayerInternalNoCheck(levels, location, target, options, delayToAfterScriptReload, out crc).Length == 0);
        }

        private static BuildTarget CalculateSelectedBuildTarget()
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            switch (selectedBuildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    return EditorUserBuildSettings.selectedStandaloneTarget;

                case BuildTargetGroup.WebPlayer:
                    return (!EditorUserBuildSettings.webPlayerStreamed ? BuildTarget.WebPlayer : BuildTarget.WebPlayerStreamed);
            }
            if (s_BuildPlatforms == null)
            {
                throw new Exception("Build platforms are not initialized.");
            }
            BuildPlatform platform = s_BuildPlatforms.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
            if (platform == null)
            {
                throw new Exception("Could not find build platform for target group " + selectedBuildTargetGroup);
            }
            return platform.DefaultTarget;
        }

        private static bool FolderIsEmpty(string path)
        {
            return (!Directory.Exists(path) || ((Directory.GetDirectories(path).Length == 0) && (Directory.GetFiles(path).Length == 0)));
        }

        private static BuildTarget GetBestStandaloneTarget(BuildTarget selectedTarget)
        {
            if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(selectedTarget)))
            {
                return selectedTarget;
            }
            if ((Application.platform != RuntimePlatform.WindowsEditor) || !ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
            {
                if ((Application.platform == RuntimePlatform.OSXEditor) && ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
                {
                    return BuildTarget.StandaloneOSXIntel;
                }
                if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
                {
                    return BuildTarget.StandaloneOSXIntel;
                }
                if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
                {
                    return BuildTarget.StandaloneLinux;
                }
            }
            return BuildTarget.StandaloneWindows;
        }

        internal static List<BuildPlatform> GetValidPlatforms()
        {
            InitBuildPlatforms();
            List<BuildPlatform> list = new List<BuildPlatform>();
            foreach (BuildPlatform platform in s_BuildPlatforms.buildPlatforms)
            {
                if ((platform.targetGroup == BuildTargetGroup.Standalone) || BuildPipeline.IsBuildTargetSupported(platform.DefaultTarget))
                {
                    list.Add(platform);
                }
            }
            return list;
        }

        private static void GUIBuildButtons(bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
        {
            GUIBuildButtons(null, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
        }

        private static void GUIBuildButtons(IBuildWindowExtension buildWindowExtension, bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
        {
            GUILayout.FlexibleSpace();
            if (canInstallInBuildFolder)
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                EditorUserBuildSettings.installInBuildFolder = GUILayout.Toggle(EditorUserBuildSettings.installInBuildFolder, "Install in Builds folder\n(for debugging with source code)", optionArray1);
            }
            else
            {
                EditorUserBuildSettings.installInBuildFolder = false;
            }
            if ((buildWindowExtension != null) && Unsupported.IsDeveloperBuild())
            {
                buildWindowExtension.ShowInternalPlatformBuildOptions();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUIContent build = styles.build;
            if ((platform.targetGroup == BuildTargetGroup.Android) && EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                build = styles.export;
            }
            if ((platform.targetGroup == BuildTargetGroup.iPhone) && (Application.platform != RuntimePlatform.OSXEditor))
            {
                enableBuildAndRunButton = false;
            }
            GUI.enabled = enableBuildButton;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(build, options))
            {
                BuildPlayerWithDefaultSettings(true, BuildOptions.ShowBuiltPlayer);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = enableBuildAndRunButton;
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(styles.buildAndRun, optionArray3))
            {
                BuildPlayerWithDefaultSettings(true, BuildOptions.AutoRunPlayer);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }

        private static void InitBuildPlatforms()
        {
            if (s_BuildPlatforms == null)
            {
                s_BuildPlatforms = new BuildPlatforms();
                RepairSelectedBuildTargetGroup();
            }
        }

        private static bool IsAnyStandaloneModuleLoaded()
        {
            return ((ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel))) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)));
        }

        internal static bool IsBuildTargetGroupSupported(BuildTarget target)
        {
            return ((target == BuildTarget.StandaloneWindows) || BuildPipeline.IsBuildTargetSupported(target));
        }

        private static bool IsMetroPlayer(BuildTarget target)
        {
            return (target == BuildTarget.WSAPlayer);
        }

        private static bool IsWP8Player(BuildTarget target)
        {
            return (target == BuildTarget.WP8Player);
        }

        private void OnGUI()
        {
            if (styles == null)
            {
                styles = new Styles();
                styles.toggleSize = styles.toggle.CalcSize(new GUIContent("X"));
                this.lv.rowHeight = (int) styles.levelString.CalcHeight(new GUIContent("X"), 100f);
            }
            InitBuildPlatforms();
            if (!UnityConnect.instance.canBuildWithUPID)
            {
                this.ShowAlert();
            }
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            string message = string.Empty;
            bool disabled = !AssetDatabase.IsOpenForEdit("ProjectSettings/EditorBuildSettings.asset", out message);
            EditorGUI.BeginDisabledGroup(disabled);
            this.ActiveScenesGUI();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (disabled)
            {
                GUI.enabled = true;
                if (Provider.enabled && GUILayout.Button("Check out", new GUILayoutOption[0]))
                {
                    Asset assetByPath = Provider.GetAssetByPath("ProjectSettings/EditorBuildSettings.asset");
                    Provider.Checkout(new AssetList { assetByPath }, CheckoutMode.Asset);
                }
                GUILayout.Label(message, new GUILayoutOption[0]);
                GUI.enabled = false;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Open Scenes", new GUILayoutOption[0]))
            {
                this.AddOpenScenes();
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(301f) };
            GUILayout.BeginHorizontal(options);
            this.ActiveBuildTargetsGUI();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.ShowBuildTargetSettings();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
        }

        private static bool PickBuildLocation(BuildTarget target, BuildOptions options, out bool updateExistingBuild)
        {
            updateExistingBuild = false;
            string buildLocation = EditorUserBuildSettings.GetBuildLocation(target);
            if ((target == BuildTarget.Android) && EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                string str2 = "Export Google Android Project";
                string location = EditorUtility.SaveFolderPanel(str2, buildLocation, string.Empty);
                EditorUserBuildSettings.SetBuildLocation(target, location);
                return true;
            }
            string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(target, options);
            string directory = FileUtil.DeleteLastPathNameComponent(buildLocation);
            string lastPathNameComponent = FileUtil.GetLastPathNameComponent(buildLocation);
            string title = "Build " + s_BuildPlatforms.GetBuildTargetDisplayName(target);
            string path = EditorUtility.SaveBuildPanel(target, title, directory, lastPathNameComponent, extensionForBuildTarget, out updateExistingBuild);
            if (path == string.Empty)
            {
                return false;
            }
            if ((extensionForBuildTarget != string.Empty) && (FileUtil.GetPathExtension(path).ToLower() != extensionForBuildTarget))
            {
                path = path + '.' + extensionForBuildTarget;
            }
            if (FileUtil.GetLastPathNameComponent(path) == string.Empty)
            {
                return false;
            }
            string str10 = !(extensionForBuildTarget != string.Empty) ? path : FileUtil.DeleteLastPathNameComponent(path);
            if (!Directory.Exists(str10))
            {
                Directory.CreateDirectory(str10);
            }
            if (((target == BuildTarget.iOS) && (Application.platform != RuntimePlatform.OSXEditor)) && (!FolderIsEmpty(path) && !UserWantsToDeleteFiles(path)))
            {
                return false;
            }
            EditorUserBuildSettings.SetBuildLocation(target, path);
            return true;
        }

        private static void RepairSelectedBuildTargetGroup()
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (((selectedBuildTargetGroup == BuildTargetGroup.Unknown) || (s_BuildPlatforms == null)) || (s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(selectedBuildTargetGroup) < 0))
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WebPlayer;
            }
        }

        private static BuildTarget RestoreLastKnownPlatformsBuildTarget(BuildPlatform bp)
        {
            BuildTargetGroup targetGroup = bp.targetGroup;
            if (targetGroup == BuildTargetGroup.Standalone)
            {
                return EditorUserBuildSettings.selectedStandaloneTarget;
            }
            if (targetGroup != BuildTargetGroup.WebPlayer)
            {
                return bp.DefaultTarget;
            }
            return (!EditorUserBuildSettings.webPlayerStreamed ? BuildTarget.WebPlayer : BuildTarget.WebPlayerStreamed);
        }

        private void ShowAlert()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            EditorGUILayout.HelpBox("Because you are not a member of this project this build will not access Unity services.", MessageType.Warning);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
        }

        private static void ShowBuildPlayerWindow()
        {
            EditorUserBuildSettings.selectedBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            EditorWindow.GetWindow<BuildPlayerWindow>(true, "Build Settings");
        }

        private void ShowBuildTargetSettings()
        {
            BuildTarget selectedStandaloneTarget;
            Dictionary<GUIContent, BuildTarget> architecturesForPlatform;
            GUIContent[] contentArray;
            int num4;
            EditorGUIUtility.labelWidth = Mathf.Min((float) 180f, (float) ((base.position.width - 265f) * 0.47f));
            BuildTarget target = CalculateSelectedBuildTarget();
            BuildPlatform platform = s_BuildPlatforms.BuildPlatformFromTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            bool flag = BuildPipeline.LicenseCheck(target);
            GUILayout.Space(18f);
            Rect position = GUILayoutUtility.GetRect((float) 50f, (float) 36f);
            position.x++;
            GUI.Label(new Rect(position.x + 3f, position.y + 3f, 32f, 32f), platform.title.image, GUIStyle.none);
            GUI.Toggle(position, false, platform.title.text, styles.platformSelector);
            GUILayout.Space(10f);
            if (((platform.targetGroup == BuildTargetGroup.WebGL) && !BuildPipeline.IsBuildTargetSupported(target)) && (IntPtr.Size == 4))
            {
                GUILayout.Label("Building for WebGL requires a 64-bit Unity editor.", new GUILayoutOption[0]);
                GUIBuildButtons(false, false, false, platform);
                return;
            }
            if (((flag && !string.IsNullOrEmpty(ModuleManager.GetTargetStringFromBuildTarget(target))) && (ModuleManager.GetBuildPostProcessor(target) == null)) && ((EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone) || !IsAnyStandaloneModuleLoaded()))
            {
                GUILayout.Label("No " + s_BuildPlatforms.GetBuildTargetDisplayName(target) + " module loaded.", new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button("Module Manager", EditorStyles.miniButton, options))
                {
                    InternalEditorUtility.ShowPackageManagerWindow();
                }
                GUIBuildButtons(false, false, false, platform);
                return;
            }
            if (Application.HasProLicense() && !InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(target))
            {
                string text = string.Format("{0} is not included in your Unity Pro license. Your {0} build will include a Unity Personal Edition splash screen.\n\nYou must be eligible to use Unity Personal Edition to use this build option. Please refer to our EULA for further information.", s_BuildPlatforms.GetBuildTargetDisplayName(target));
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("EULA", EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    Application.OpenURL("http://unity3d.com/legal/eula");
                }
                if (GUILayout.Button(string.Format("Add {0} to your Unity Pro license", s_BuildPlatforms.GetBuildTargetDisplayName(target)), EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    Application.OpenURL("http://unity3d.com/get-unity");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUIContent downloadErrorForTarget = styles.GetDownloadErrorForTarget(target);
            if (downloadErrorForTarget != null)
            {
                GUILayout.Label(downloadErrorForTarget, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                GUIBuildButtons(false, false, false, platform);
                return;
            }
            if (!flag)
            {
                int num = s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(platform.targetGroup);
                GUILayout.Label(styles.notLicensedMessages[num, 0], EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                GUILayout.Space(5f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if ((styles.notLicensedMessages[num, 1].text.Length != 0) && GUILayout.Button(styles.notLicensedMessages[num, 1], new GUILayoutOption[0]))
                {
                    Application.OpenURL(styles.notLicensedMessages[num, 2].text);
                }
                GUILayout.EndHorizontal();
                GUIBuildButtons(false, false, false, platform);
                return;
            }
            IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(ModuleManager.GetTargetStringFromBuildTargetGroup(platform.targetGroup));
            if (buildWindowExtension != null)
            {
                buildWindowExtension.ShowPlatformBuildOptions();
            }
            GUI.changed = false;
            switch (platform.targetGroup)
            {
                case BuildTargetGroup.Standalone:
                {
                    BuildTarget bestStandaloneTarget = GetBestStandaloneTarget(EditorUserBuildSettings.selectedStandaloneTarget);
                    selectedStandaloneTarget = EditorUserBuildSettings.selectedStandaloneTarget;
                    int selectedIndex = Math.Max(0, Array.IndexOf<BuildTarget>(s_BuildPlatforms.standaloneSubtargets, BuildPlatforms.DefaultTargetForPlatform(bestStandaloneTarget)));
                    int index = EditorGUILayout.Popup(styles.standaloneTarget, selectedIndex, s_BuildPlatforms.standaloneSubtargetStrings, new GUILayoutOption[0]);
                    if (index != selectedIndex)
                    {
                        selectedStandaloneTarget = s_BuildPlatforms.standaloneSubtargets[index];
                        goto Label_0523;
                    }
                    architecturesForPlatform = BuildPlatforms.GetArchitecturesForPlatform(bestStandaloneTarget);
                    if (architecturesForPlatform == null)
                    {
                        goto Label_0523;
                    }
                    contentArray = new List<GUIContent>(architecturesForPlatform.Keys).ToArray();
                    num4 = 0;
                    if (index == selectedIndex)
                    {
                        foreach (KeyValuePair<GUIContent, BuildTarget> pair in architecturesForPlatform)
                        {
                            if (((BuildTarget) pair.Value) == bestStandaloneTarget)
                            {
                                num4 = Math.Max(0, Array.IndexOf<GUIContent>(contentArray, pair.Key));
                                break;
                            }
                        }
                    }
                    break;
                }
                case BuildTargetGroup.WebPlayer:
                {
                    GUI.enabled = BuildPipeline.LicenseCheck(BuildTarget.WebPlayerStreamed);
                    bool flag2 = EditorGUILayout.Toggle(styles.webPlayerStreamed, EditorUserBuildSettings.webPlayerStreamed, new GUILayoutOption[0]);
                    if (GUI.changed)
                    {
                        EditorUserBuildSettings.webPlayerStreamed = flag2;
                    }
                    EditorUserBuildSettings.webPlayerOfflineDeployment = EditorGUILayout.Toggle(styles.webPlayerOfflineDeployment, EditorUserBuildSettings.webPlayerOfflineDeployment, new GUILayoutOption[0]);
                    goto Label_056E;
                }
                case BuildTargetGroup.iPhone:
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        EditorUserBuildSettings.symlinkLibraries = EditorGUILayout.Toggle(styles.symlinkiOSLibraries, EditorUserBuildSettings.symlinkLibraries, new GUILayoutOption[0]);
                    }
                    goto Label_056E;

                default:
                    goto Label_056E;
            }
            num4 = EditorGUILayout.Popup(styles.architecture, num4, contentArray, new GUILayoutOption[0]);
            selectedStandaloneTarget = architecturesForPlatform[contentArray[num4]];
        Label_0523:
            if (selectedStandaloneTarget != EditorUserBuildSettings.selectedStandaloneTarget)
            {
                EditorUserBuildSettings.selectedStandaloneTarget = selectedStandaloneTarget;
                GUIUtility.ExitGUI();
            }
        Label_056E:
            GUI.enabled = true;
            bool enableBuildButton = (buildWindowExtension == null) || buildWindowExtension.EnabledBuildButton();
            bool enableBuildAndRunButton = false;
            bool flag5 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawScriptDebuggingCheckbox();
            bool flag6 = (buildWindowExtension != null) && buildWindowExtension.ShouldDrawExplicitNullCheckbox();
            bool flag7 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawDevelopmentPlayerCheckbox();
            bool flag8 = ((target == BuildTarget.StandaloneLinux) || (target == BuildTarget.StandaloneLinux64)) || (target == BuildTarget.StandaloneLinuxUniversal);
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            bool flag9 = (buildPostProcessor != null) && buildPostProcessor.SupportsScriptsOnlyBuild();
            bool canInstallInBuildFolder = false;
            if (BuildPipeline.IsBuildTargetSupported(target))
            {
                bool flag11 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawProfilerCheckbox();
                GUI.enabled = flag7;
                if (flag7)
                {
                    EditorUserBuildSettings.development = EditorGUILayout.Toggle(styles.debugBuild, EditorUserBuildSettings.development, new GUILayoutOption[0]);
                }
                bool development = EditorUserBuildSettings.development;
                GUI.enabled = development;
                GUI.enabled = GUI.enabled && (platform.targetGroup != BuildTargetGroup.WebPlayer);
                if (flag11)
                {
                    if (!GUI.enabled)
                    {
                        if (!development)
                        {
                            styles.profileBuild.tooltip = "Profiling only enabled in Development Player";
                        }
                        else if (platform.targetGroup == BuildTargetGroup.WebPlayer)
                        {
                            styles.profileBuild.tooltip = "Autoconnect not available from webplayer. Manually connect in Profiler";
                        }
                    }
                    else
                    {
                        styles.profileBuild.tooltip = string.Empty;
                    }
                    EditorUserBuildSettings.connectProfiler = EditorGUILayout.Toggle(styles.profileBuild, EditorUserBuildSettings.connectProfiler, new GUILayoutOption[0]);
                }
                GUI.enabled = development;
                if (flag5)
                {
                    EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle(styles.allowDebugging, EditorUserBuildSettings.allowDebugging, new GUILayoutOption[0]);
                }
                bool flag13 = false;
                int num5 = 0;
                if (PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref num5, platform.targetGroup))
                {
                    flag13 = num5 == 1;
                }
                if ((((buildWindowExtension != null) && development) && flag13) && buildWindowExtension.ShouldDrawForceOptimizeScriptsCheckbox())
                {
                    EditorUserBuildSettings.forceOptimizeScriptCompilation = EditorGUILayout.Toggle(styles.forceOptimizeScriptCompilation, EditorUserBuildSettings.forceOptimizeScriptCompilation, new GUILayoutOption[0]);
                }
                if (flag6)
                {
                    GUI.enabled = !development;
                    if (!GUI.enabled)
                    {
                        EditorUserBuildSettings.explicitNullChecks = true;
                    }
                    EditorUserBuildSettings.explicitNullChecks = EditorGUILayout.Toggle(styles.explicitNullChecks, EditorUserBuildSettings.explicitNullChecks, new GUILayoutOption[0]);
                    GUI.enabled = development;
                }
                if (flag9)
                {
                    EditorUserBuildSettings.buildScriptsOnly = EditorGUILayout.Toggle(styles.buildScriptsOnly, EditorUserBuildSettings.buildScriptsOnly, new GUILayoutOption[0]);
                }
                GUI.enabled = !development;
                if (flag8)
                {
                    EditorUserBuildSettings.enableHeadlessMode = EditorGUILayout.Toggle(styles.enableHeadlessMode, EditorUserBuildSettings.enableHeadlessMode && !development, new GUILayoutOption[0]);
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                canInstallInBuildFolder = Unsupported.IsDeveloperBuild() && PostprocessBuildPlayer.SupportsInstallInBuildFolder(target);
                if (enableBuildButton)
                {
                    enableBuildAndRunButton = (buildWindowExtension == null) ? !EditorUserBuildSettings.installInBuildFolder : (buildWindowExtension.EnabledBuildAndRunButton() && !EditorUserBuildSettings.installInBuildFolder);
                }
                if (platform.targetGroup == BuildTargetGroup.WebPlayer)
                {
                    string message = string.Format("Please note that the Unity Web Player is deprecated. Building for Web Player will no longer be supported in future versions of Unity.", new object[0]);
                    GUILayout.BeginVertical(new GUILayoutOption[0]);
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                    GUILayout.EndVertical();
                }
            }
            else
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                GUILayout.BeginHorizontal(optionArray2);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                GUILayout.BeginVertical(optionArray3);
                int num6 = s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(platform.targetGroup);
                GUILayout.Label(styles.GetTargetNotInstalled(num6, 0), new GUILayoutOption[0]);
                if ((styles.GetTargetNotInstalled(num6, 1) != null) && GUILayout.Button(styles.GetTargetNotInstalled(num6, 1), new GUILayoutOption[0]))
                {
                    Application.OpenURL(styles.GetTargetNotInstalled(num6, 2).text);
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUIBuildButtons(buildWindowExtension, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
        }

        private void ShowOption(BuildPlatform bp, GUIContent title, GUIStyle background)
        {
            Rect position = GUILayoutUtility.GetRect((float) 50f, (float) 36f);
            position.x++;
            position.y++;
            bool flag = BuildPipeline.LicenseCheck(bp.DefaultTarget);
            GUI.contentColor = new Color(1f, 1f, 1f, !flag ? 0.7f : 1f);
            bool on = EditorUserBuildSettings.selectedBuildTargetGroup == bp.targetGroup;
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(position, GUIContent.none, false, false, on, false);
                GUI.Label(new Rect(position.x + 3f, position.y + 3f, 32f, 32f), title.image, GUIStyle.none);
                if (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == bp.targetGroup)
                {
                    GUI.Label(new Rect((position.xMax - styles.activePlatformIcon.width) - 8f, (position.y + 3f) + ((0x20 - styles.activePlatformIcon.height) / 2), (float) styles.activePlatformIcon.width, (float) styles.activePlatformIcon.height), styles.activePlatformIcon, GUIStyle.none);
                }
            }
            if (GUI.Toggle(position, on, title.text, styles.platformSelector) && (EditorUserBuildSettings.selectedBuildTargetGroup != bp.targetGroup))
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = bp.targetGroup;
                Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(InspectorWindow));
                for (int i = 0; i < objArray.Length; i++)
                {
                    InspectorWindow window = objArray[i] as InspectorWindow;
                    if (window != null)
                    {
                        window.Repaint();
                    }
                }
            }
        }

        private static bool UserWantsToDeleteFiles(string path)
        {
            string message = "WARNING: all files and folders located in target folder: '" + path + "' will be deleted by build process.";
            return EditorUtility.DisplayDialog("Deleting existing files", message, "OK", "Cancel");
        }

        [CompilerGenerated]
        private sealed class <AddOpenScenes>c__AnonStorey30
        {
            internal Scene scene;

            internal bool <>m__47(EditorBuildSettingsScene s)
            {
                return (s.path == this.scene.path);
            }
        }

        public class BuildPlatform
        {
            public bool forceShowTarget;
            public string name;
            public Texture2D smallIcon;
            public BuildTargetGroup targetGroup;
            public GUIContent title;
            public string tooltip;

            public BuildPlatform(string locTitle, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget) : this(locTitle, string.Empty, iconId, targetGroup, forceShowTarget)
            {
            }

            public BuildPlatform(string locTitle, string tooltip, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget)
            {
                this.targetGroup = targetGroup;
                this.name = (targetGroup == BuildTargetGroup.Unknown) ? string.Empty : BuildPipeline.GetBuildTargetGroupName(this.DefaultTarget);
                this.title = EditorGUIUtility.TextContentWithIcon(locTitle, iconId);
                this.smallIcon = EditorGUIUtility.IconContent(iconId + ".Small").image as Texture2D;
                this.tooltip = tooltip;
                this.forceShowTarget = forceShowTarget;
            }

            public BuildTarget DefaultTarget
            {
                get
                {
                    switch (this.targetGroup)
                    {
                        case BuildTargetGroup.Standalone:
                            return BuildTarget.StandaloneWindows;

                        case BuildTargetGroup.WebPlayer:
                            return BuildTarget.WebPlayer;

                        case BuildTargetGroup.iPhone:
                            return BuildTarget.iOS;

                        case BuildTargetGroup.PS3:
                            return BuildTarget.PS3;

                        case BuildTargetGroup.XBOX360:
                            return BuildTarget.XBOX360;

                        case BuildTargetGroup.Android:
                            return BuildTarget.Android;

                        case BuildTargetGroup.GLESEmu:
                            return BuildTarget.StandaloneGLESEmu;

                        case BuildTargetGroup.WebGL:
                            return BuildTarget.WebGL;

                        case BuildTargetGroup.Metro:
                            return BuildTarget.WSAPlayer;

                        case BuildTargetGroup.WP8:
                            return BuildTarget.WP8Player;

                        case BuildTargetGroup.BlackBerry:
                            return BuildTarget.BlackBerry;

                        case BuildTargetGroup.Tizen:
                            return BuildTarget.Tizen;

                        case BuildTargetGroup.PSP2:
                            return BuildTarget.PSP2;

                        case BuildTargetGroup.PS4:
                            return BuildTarget.PS4;

                        case BuildTargetGroup.XboxOne:
                            return BuildTarget.XboxOne;

                        case BuildTargetGroup.SamsungTV:
                            return BuildTarget.SamsungTV;

                        case BuildTargetGroup.Nintendo3DS:
                            return BuildTarget.Nintendo3DS;

                        case BuildTargetGroup.WiiU:
                            return BuildTarget.WiiU;

                        case BuildTargetGroup.tvOS:
                            return BuildTarget.tvOS;
                    }
                    return BuildTarget.iPhone;
                }
            }
        }

        private class BuildPlatforms
        {
            public BuildPlayerWindow.BuildPlatform[] buildPlatforms;
            public BuildTarget[] standaloneSubtargets;
            public GUIContent[] standaloneSubtargetStrings;

            internal BuildPlatforms()
            {
                List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform> {
                    new BuildPlayerWindow.BuildPlatform("Web Player", "BuildSettings.Web", 2, 1),
                    new BuildPlayerWindow.BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", 1, 1),
                    new BuildPlayerWindow.BuildPlatform("iOS", "BuildSettings.iPhone", 4, 1),
                    new BuildPlayerWindow.BuildPlatform("Android", "BuildSettings.Android", 7, 1),
                    new BuildPlayerWindow.BuildPlatform("BlackBerry", "BuildSettings.BlackBerry", 0x10, 1),
                    new BuildPlayerWindow.BuildPlatform("Tizen", "BuildSettings.Tizen", 0x11, 1),
                    new BuildPlayerWindow.BuildPlatform("Xbox 360", "BuildSettings.XBox360", 6, 1),
                    new BuildPlayerWindow.BuildPlatform("Xbox One", "BuildSettings.XboxOne", 0x15, 1),
                    new BuildPlayerWindow.BuildPlatform("PS3", "BuildSettings.PS3", 5, 1),
                    new BuildPlayerWindow.BuildPlatform("PS Vita", "BuildSettings.PSP2", 0x12, 1),
                    new BuildPlayerWindow.BuildPlatform("PS4", "BuildSettings.PS4", 0x13, 1),
                    new BuildPlayerWindow.BuildPlatform("GLES Emulator", "BuildSettings.StandaloneGLESEmu", 9, 0),
                    new BuildPlayerWindow.BuildPlatform("Wii U", "BuildSettings.WiiU", 0x18, 0),
                    new BuildPlayerWindow.BuildPlatform("Windows Store", "BuildSettings.Metro", 14, 1),
                    new BuildPlayerWindow.BuildPlatform("WebGL", "BuildSettings.WebGL", 13, 1),
                    new BuildPlayerWindow.BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", 0x16, 1),
                    new BuildPlayerWindow.BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", 0x17, 0)
                };
                foreach (BuildPlayerWindow.BuildPlatform platform in list)
                {
                    platform.tooltip = BuildPipeline.GetBuildTargetGroupDisplayName(platform.targetGroup) + " settings";
                }
                this.buildPlatforms = list.ToArray();
                this.SetupStandaloneSubtargets();
            }

            public BuildPlayerWindow.BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
            {
                int index = this.BuildPlatformIndexFromTargetGroup(group);
                return ((index == -1) ? null : this.buildPlatforms[index]);
            }

            public int BuildPlatformIndexFromTargetGroup(BuildTargetGroup group)
            {
                for (int i = 0; i < this.buildPlatforms.Length; i++)
                {
                    if (group == this.buildPlatforms[i].targetGroup)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public static BuildTarget DefaultTargetForPlatform(BuildTarget target)
            {
                BuildTarget target2 = target;
                switch (target2)
                {
                    case BuildTarget.StandaloneLinux:
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal:
                        return BuildTarget.StandaloneLinux;

                    case BuildTarget.StandaloneWindows64:
                        break;

                    case BuildTarget.WSAPlayer:
                        return BuildTarget.WSAPlayer;

                    case BuildTarget.WP8Player:
                        return BuildTarget.WP8Player;

                    case BuildTarget.StandaloneOSXIntel64:
                        goto Label_0059;

                    default:
                        switch (target2)
                        {
                            case BuildTarget.StandaloneOSXUniversal:
                            case BuildTarget.StandaloneOSXIntel:
                                goto Label_0059;

                            case ((BuildTarget) 3):
                                return target;

                            case BuildTarget.StandaloneWindows:
                                break;

                            default:
                                return target;
                        }
                        break;
                }
                return BuildTarget.StandaloneWindows;
            Label_0059:
                return BuildTarget.StandaloneOSXIntel;
            }

            public static Dictionary<GUIContent, BuildTarget> GetArchitecturesForPlatform(BuildTarget target)
            {
                Dictionary<GUIContent, BuildTarget> dictionary;
                BuildTarget target2 = target;
                switch (target2)
                {
                    case BuildTarget.StandaloneOSXUniversal:
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
                        dictionary = new Dictionary<GUIContent, BuildTarget>();
                        dictionary.Add(EditorGUIUtility.TextContent("x86"), BuildTarget.StandaloneOSXIntel);
                        dictionary.Add(EditorGUIUtility.TextContent("x86_64"), BuildTarget.StandaloneOSXIntel64);
                        dictionary.Add(EditorGUIUtility.TextContent("Universal"), BuildTarget.StandaloneOSXUniversal);
                        return dictionary;

                    case BuildTarget.StandaloneWindows:
                        break;

                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal:
                        goto Label_0078;

                    default:
                        switch (target2)
                        {
                            case BuildTarget.StandaloneLinux:
                                goto Label_0078;

                            case ((BuildTarget) 0x12):
                                goto Label_00F2;

                            case BuildTarget.StandaloneWindows64:
                                break;

                            default:
                                goto Label_00F2;
                        }
                        break;
                }
                dictionary = new Dictionary<GUIContent, BuildTarget>();
                dictionary.Add(EditorGUIUtility.TextContent("x86"), BuildTarget.StandaloneWindows);
                dictionary.Add(EditorGUIUtility.TextContent("x86_64"), BuildTarget.StandaloneWindows64);
                return dictionary;
            Label_0078:
                dictionary = new Dictionary<GUIContent, BuildTarget>();
                dictionary.Add(EditorGUIUtility.TextContent("x86"), BuildTarget.StandaloneLinux);
                dictionary.Add(EditorGUIUtility.TextContent("x86_64"), BuildTarget.StandaloneLinux64);
                dictionary.Add(EditorGUIUtility.TextContent("x86 + x86_64 (Universal)"), BuildTarget.StandaloneLinuxUniversal);
                return dictionary;
            Label_00F2:
                return null;
            }

            public string GetBuildTargetDisplayName(BuildTarget target)
            {
                foreach (BuildPlayerWindow.BuildPlatform platform in this.buildPlatforms)
                {
                    if (platform.DefaultTarget == target)
                    {
                        return platform.title.text;
                    }
                }
                if (target == BuildTarget.WebPlayerStreamed)
                {
                    return this.BuildPlatformFromTargetGroup(BuildTargetGroup.WebPlayer).title.text;
                }
                for (int i = 0; i < this.standaloneSubtargets.Length; i++)
                {
                    if (this.standaloneSubtargets[i] == DefaultTargetForPlatform(target))
                    {
                        return this.standaloneSubtargetStrings[i].text;
                    }
                }
                return "Unsupported Target";
            }

            private void SetupStandaloneSubtargets()
            {
                List<BuildTarget> list = new List<BuildTarget>();
                List<GUIContent> list2 = new List<GUIContent>();
                if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
                {
                    list.Add(BuildTarget.StandaloneWindows);
                    list2.Add(EditorGUIUtility.TextContent("Windows"));
                }
                if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
                {
                    list.Add(BuildTarget.StandaloneOSXIntel);
                    list2.Add(EditorGUIUtility.TextContent("Mac OS X"));
                }
                if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
                {
                    list.Add(BuildTarget.StandaloneLinux);
                    list2.Add(EditorGUIUtility.TextContent("Linux"));
                }
                this.standaloneSubtargets = list.ToArray();
                this.standaloneSubtargetStrings = list2.ToArray();
            }
        }

        private class ScenePostprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < movedAssets.Length; i++)
                {
                    string path = movedAssets[i];
                    if (Path.GetExtension(path) == ".unity")
                    {
                        foreach (EditorBuildSettingsScene scene in scenes)
                        {
                            if (scene.path.ToLower() == movedFromPath[i].ToLower())
                            {
                                scene.path = path;
                            }
                        }
                    }
                }
                EditorBuildSettings.scenes = scenes;
            }
        }

        public class SceneSorter : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                return new CaseInsensitiveComparer().Compare(y, x);
            }
        }

        private class Styles
        {
            public Texture2D activePlatformIcon = (EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image as Texture2D);
            public GUIContent allowDebugging;
            public GUIContent architecture;
            public GUIStyle box = "OL Box";
            public GUIContent build = EditorGUIUtility.TextContent("Build");
            public GUIContent buildAndRun = EditorGUIUtility.TextContent("Build And Run");
            public GUIContent buildScriptsOnly;
            private GUIContent[,] buildTargetNotInstalled;
            public GUIContent debugBuild;
            public GUIContent enableHeadlessMode;
            public GUIStyle evenRow = "CN EntryBackEven";
            public GUIContent explicitNullChecks;
            public GUIContent export = EditorGUIUtility.TextContent("Export");
            public GUIContent forceOptimizeScriptCompilation;
            public const float kButtonWidth = 110f;
            private const string kDownloadURL = "http://unity3d.com/unity/download/";
            private const string kMailURL = "http://unity3d.com/company/sales?type=sales";
            private const string kShopURL = "https://store.unity3d.com/shop/";
            public GUIStyle levelString = "PlayerSettingsLevel";
            public GUIStyle levelStringCounter = new GUIStyle("Label");
            public GUIContent noSessionDialogText = EditorGUIUtility.TextContent("In order to publish your build to UDN, you need to sign in via the AssetStore and tick the 'Stay signed in' checkbox.");
            public GUIContent[,] notLicensedMessages;
            public GUIStyle oddRow = "CN EntryBackOdd";
            public GUIStyle platformSelector = "PlayerSettingsPlatform";
            public GUIContent platformTitle = EditorGUIUtility.TextContent("Platform|Which platform to build for");
            public GUIContent profileBuild;
            public GUIContent scenesInBuild = EditorGUIUtility.TextContent("Scenes In Build|Which scenes to include in the build");
            public GUIStyle selected = "ServerUpdateChangesetOn";
            public GUIContent standaloneTarget;
            public GUIContent switchPlatform = EditorGUIUtility.TextContent("Switch Platform");
            public GUIContent symlinkiOSLibraries;
            public GUIStyle title = "OL title";
            public GUIStyle toggle = "Toggle";
            public Vector2 toggleSize;
            public GUIContent webPlayerOfflineDeployment;
            public GUIContent webPlayerStreamed;

            public Styles()
            {
                GUIContent[] contentArray1 = new GUIContent[,] { 
                    { EditorGUIUtility.TextContent("Unity Web Player building is disabled during the public preview beta. It will be enabled when Unity ships."), new GUIContent(string.Empty), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Standalone Publishing."), new GUIContent(string.Empty), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover iOS Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Apple TV Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Android Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover BlackBerry Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Tizen Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Xbox 360 Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Xbox One Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover PS3 Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover PS Vita Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover PS4 Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover GLESEmu Publishing"), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Wii U Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Flash Publishing"), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Windows Store Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, 
                    { EditorGUIUtility.TextContent("Your license does not cover Windows Phone 8 Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover SamsungTV Publishing"), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Nintendo 3DS Publishing"), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }
                 };
                this.notLicensedMessages = contentArray1;
                GUIContent[] contentArray2 = new GUIContent[0x13, 3];
                contentArray2[0, 0] = EditorGUIUtility.TextContent("Web Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[0, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[1, 0] = EditorGUIUtility.TextContent("Standalone Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[1, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[2, 0] = EditorGUIUtility.TextContent("iOS Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[2, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[3, 0] = EditorGUIUtility.TextContent("Apple TV Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[3, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[4, 0] = EditorGUIUtility.TextContent("Android Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[4, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[5, 0] = EditorGUIUtility.TextContent("BlackBerry is not supported in this build.\nDownload a build that supports it.");
                contentArray2[5, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[6, 0] = EditorGUIUtility.TextContent("Tizen is not supported in this build.\nDownload a build that supports it.");
                contentArray2[6, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[7, 0] = EditorGUIUtility.TextContent("Xbox 360 Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[7, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[8, 0] = EditorGUIUtility.TextContent("Xbox One Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[8, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[9, 0] = EditorGUIUtility.TextContent("PS3 Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[9, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[10, 0] = EditorGUIUtility.TextContent("PS Vita Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[10, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[11, 0] = EditorGUIUtility.TextContent("PS4 Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[11, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[12, 0] = EditorGUIUtility.TextContent("GLESEmu Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[12, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[13, 0] = EditorGUIUtility.TextContent("Wii U Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[13, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[14, 0] = EditorGUIUtility.TextContent("Flash Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[14, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[15, 0] = EditorGUIUtility.TextContent("Windows Store Player is not supported in\nthis build.\n\nDownload a build that supports it.");
                contentArray2[15, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[0x10, 0] = EditorGUIUtility.TextContent("Windows Phone 8 Player is not supported\nin this build.\n\nDownload a build that supports it.");
                contentArray2[0x10, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[0x11, 0] = EditorGUIUtility.TextContent("SamsungTV Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[0x11, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[0x12, 0] = EditorGUIUtility.TextContent("Ninteno 3DS is not supported in this build.\nDownload a build that supports it.");
                contentArray2[0x12, 2] = new GUIContent("http://unity3d.com/unity/download/");
                this.buildTargetNotInstalled = contentArray2;
                this.standaloneTarget = EditorGUIUtility.TextContent("Target Platform|Destination platform for standalone build");
                this.architecture = EditorGUIUtility.TextContent("Architecture|Build architecture for standalone");
                this.webPlayerStreamed = EditorGUIUtility.TextContent("Streamed|Is the web player streamed in?");
                this.webPlayerOfflineDeployment = EditorGUIUtility.TextContent("Offline Deployment|Web Player will not reference online resources");
                this.debugBuild = EditorGUIUtility.TextContent("Development Build");
                this.profileBuild = EditorGUIUtility.TextContent("Autoconnect Profiler");
                this.allowDebugging = EditorGUIUtility.TextContent("Script Debugging");
                this.symlinkiOSLibraries = EditorGUIUtility.TextContent("Symlink Unity libraries");
                this.explicitNullChecks = EditorGUIUtility.TextContent("Explicit Null Checks");
                this.enableHeadlessMode = EditorGUIUtility.TextContent("Headless Mode");
                this.buildScriptsOnly = EditorGUIUtility.TextContent("Scripts Only Build");
                this.forceOptimizeScriptCompilation = EditorGUIUtility.TextContent("Build Optimized Scripts|Compile IL2CPP using full compiler optimizations. Note this will obfuscate callstack output.");
                this.levelStringCounter.alignment = TextAnchor.MiddleRight;
            }

            public GUIContent GetDownloadErrorForTarget(BuildTarget target)
            {
                return null;
            }

            public GUIContent GetTargetNotInstalled(int index, int item)
            {
                if (index >= this.buildTargetNotInstalled.GetLength(0))
                {
                    index = 0;
                }
                return this.buildTargetNotInstalled[index, item];
            }
        }
    }
}

