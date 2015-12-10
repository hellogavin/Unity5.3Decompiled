namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Modules;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(PluginImporter))]
    internal class PluginImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static GetComptability <>f__am$cacheA;
        [CompilerGenerated]
        private static GetComptability <>f__am$cacheB;
        private IPluginImporterExtension[] m_AdditionalExtensions;
        private int m_CompatibleWithAnyPlatform;
        private int m_CompatibleWithEditor;
        private int[] m_CompatibleWithPlatform = new int[GetPlatformGroupArraySize()];
        private DesktopPluginImporterExtension m_DesktopExtension = new DesktopPluginImporterExtension();
        private EditorPluginImporterExtension m_EditorExtension = new EditorPluginImporterExtension();
        private bool m_HasModified;
        private Vector2 m_InformationScrollPosition = Vector2.zero;
        private Dictionary<string, string> m_PluginInformation;
        private static readonly BuildTarget[] m_StandaloneTargets = new BuildTarget[] { BuildTarget.StandaloneOSXIntel, BuildTarget.StandaloneOSXIntel64, BuildTarget.StandaloneOSXUniversal, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64, BuildTarget.StandaloneLinux, BuildTarget.StandaloneLinux64, BuildTarget.StandaloneLinuxUniversal };

        internal override void Apply()
        {
            base.Apply();
            foreach (PluginImporter importer in this.importers)
            {
                if (this.m_CompatibleWithAnyPlatform > -1)
                {
                    importer.SetCompatibleWithAnyPlatform(this.m_CompatibleWithAnyPlatform == 1);
                }
                if (this.m_CompatibleWithEditor > -1)
                {
                    importer.SetCompatibleWithEditor(this.m_CompatibleWithEditor == 1);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    if (this.m_CompatibleWithPlatform[(int) target] > -1)
                    {
                        importer.SetCompatibleWithPlatform(target, this.m_CompatibleWithPlatform[(int) target] == 1);
                    }
                }
            }
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.Apply(this);
                }
                foreach (BuildTarget target2 in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target2);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.Apply(this);
                    }
                }
            }
        }

        private BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
        {
            List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
            List<BuildPlayerWindow.BuildPlatform> list2 = new List<BuildPlayerWindow.BuildPlatform>();
            if ((this.m_CompatibleWithAnyPlatform > 0) || (this.m_CompatibleWithEditor > 0))
            {
                BuildPlayerWindow.BuildPlatform item = new BuildPlayerWindow.BuildPlatform("Editor settings", "BuildSettings.Editor", BuildTargetGroup.Unknown, true) {
                    name = BuildPipeline.GetEditorTargetName()
                };
                list2.Add(item);
            }
            foreach (BuildPlayerWindow.BuildPlatform platform2 in validPlatforms)
            {
                if (IgnorePlatform(platform2.DefaultTarget))
                {
                    continue;
                }
                if (platform2.targetGroup == BuildTargetGroup.Standalone)
                {
                    if ((this.m_CompatibleWithAnyPlatform >= 1) || (this.compatibleWithStandalone >= 1))
                    {
                        goto Label_00DD;
                    }
                    continue;
                }
                if (((this.m_CompatibleWithAnyPlatform < 1) && (this.m_CompatibleWithPlatform[(int) platform2.DefaultTarget] < 1)) || (ModuleManager.GetPluginImporterExtension(platform2.targetGroup) == null))
                {
                    continue;
                }
            Label_00DD:
                list2.Add(platform2);
            }
            return list2.ToArray();
        }

        internal bool GetCompatibleWithPlatform(string platformName)
        {
            if (base.targets.Length > 1)
            {
                throw new InvalidOperationException("Cannot be used while multiple plugins are selected");
            }
            return (this.m_CompatibleWithPlatform[(int) BuildPipeline.GetBuildTargetByName(platformName)] == 1);
        }

        private static int GetPlatformGroupArraySize()
        {
            int num = 0;
            foreach (BuildTarget target in typeof(BuildTarget).EnumGetNonObsoleteValues())
            {
                if (num < (target + 1))
                {
                    num = ((int) target) + 1;
                }
            }
            return num;
        }

        private static List<BuildTarget> GetValidBuildTargets()
        {
            List<BuildTarget> list = new List<BuildTarget>();
            foreach (BuildTarget target in typeof(BuildTarget).EnumGetNonObsoleteValues())
            {
                if (!IgnorePlatform(target) && ((!ModuleManager.IsPlatformSupported(target) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(target))) || IsStandaloneTarget(target)))
                {
                    list.Add(target);
                }
            }
            return list;
        }

        internal override bool HasModified()
        {
            bool flag = this.m_HasModified || base.HasModified();
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    flag |= extension.HasModified(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        flag |= pluginImporterExtension.HasModified(this);
                    }
                }
            }
            return flag;
        }

        private static bool IgnorePlatform(BuildTarget platform)
        {
            return (platform == BuildTarget.StandaloneGLESEmu);
        }

        private bool IsEditingPlatformSettingsSupported()
        {
            return (base.targets.Length == 1);
        }

        private static bool IsStandaloneTarget(BuildTarget buildTarget)
        {
            return m_StandaloneTargets.Contains<BuildTarget>(buildTarget);
        }

        private void OnDisable()
        {
            base.OnDisable();
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.OnDisable(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.OnDisable(this);
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.OnEnable(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.OnEnable(this);
                        pluginImporterExtension.ResetValues(this);
                    }
                }
                this.m_PluginInformation = new Dictionary<string, string>();
                this.m_PluginInformation["Path"] = this.importer.assetPath;
                this.m_PluginInformation["Type"] = !this.importer.isNativePlugin ? "Managed" : "Native";
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(false);
            GUILayout.Label("Select platforms for plugin", EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
            this.ShowGeneralOptions();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);
            if (this.IsEditingPlatformSettingsSupported())
            {
                this.ShowPlatformSettings();
            }
            EditorGUI.EndDisabledGroup();
            base.ApplyRevertGUI();
            if (base.targets.Length <= 1)
            {
                GUILayout.Label("Information", EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.m_InformationScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_InformationScrollPosition, new GUILayoutOption[0]);
                foreach (KeyValuePair<string, string> pair in this.m_PluginInformation)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(50f) };
                    GUILayout.Label(pair.Key, options);
                    GUILayout.TextField(pair.Value, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.FlexibleSpace();
                if (this.importer.isNativePlugin)
                {
                    EditorGUILayout.HelpBox("Once a native plugin is loaded from script, it's never unloaded. If you deselect a native plugin and it's already loaded, please restart Unity.", MessageType.Warning);
                }
            }
        }

        private void ResetCompatability(ref int value, GetComptability getComptability)
        {
            value = !getComptability(this.importer) ? 0 : 1;
            foreach (PluginImporter importer in this.importers)
            {
                if (value != (!getComptability(importer) ? 0 : 1))
                {
                    value = -1;
                    break;
                }
            }
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.m_HasModified = false;
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = imp => imp.GetCompatibleWithAnyPlatform();
            }
            this.ResetCompatability(ref this.m_CompatibleWithAnyPlatform, <>f__am$cacheA);
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = imp => imp.GetCompatibleWithEditor();
            }
            this.ResetCompatability(ref this.m_CompatibleWithEditor, <>f__am$cacheB);
            <ResetValues>c__AnonStorey79 storey = new <ResetValues>c__AnonStorey79();
            using (List<BuildTarget>.Enumerator enumerator = GetValidBuildTargets().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    storey.platform = enumerator.Current;
                    this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int) storey.platform], new GetComptability(storey.<>m__120));
                }
            }
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.ResetValues(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.ResetValues(this);
                    }
                }
            }
        }

        internal void SetCompatibleWithPlatform(string platformName, bool enabled)
        {
            if (base.targets.Length > 1)
            {
                throw new InvalidOperationException("Cannot be used while multiple plugins are selected");
            }
            int num = !enabled ? 0 : 1;
            int buildTargetByName = (int) BuildPipeline.GetBuildTargetByName(platformName);
            if (this.m_CompatibleWithPlatform[buildTargetByName] != num)
            {
                this.m_CompatibleWithPlatform[buildTargetByName] = num;
                this.m_HasModified = true;
            }
        }

        private void ShowEditorSettings()
        {
            this.m_EditorExtension.OnPlatformSettingsGUI(this);
        }

        private void ShowGeneralOptions()
        {
            EditorGUI.BeginChangeCheck();
            this.m_CompatibleWithAnyPlatform = this.ToggleWithMixedValue(this.m_CompatibleWithAnyPlatform, "Any Platform");
            EditorGUI.BeginDisabledGroup(this.m_CompatibleWithAnyPlatform == 1);
            this.m_CompatibleWithEditor = this.ToggleWithMixedValue(this.m_CompatibleWithEditor, "Editor");
            EditorGUI.BeginChangeCheck();
            int num = this.ToggleWithMixedValue(this.compatibleWithStandalone, "Standalone");
            if (EditorGUI.EndChangeCheck())
            {
                this.compatibleWithStandalone = num;
                this.m_DesktopExtension.ValidateSingleCPUTargets(this);
            }
            foreach (BuildTarget target in GetValidBuildTargets())
            {
                if (!IsStandaloneTarget(target))
                {
                    this.m_CompatibleWithPlatform[(int) target] = this.ToggleWithMixedValue(this.m_CompatibleWithPlatform[(int) target], target.ToString());
                }
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                this.m_HasModified = true;
            }
        }

        private void ShowPlatformSettings()
        {
            BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = this.GetBuildPlayerValidPlatforms();
            if (buildPlayerValidPlatforms.Length > 0)
            {
                GUILayout.Label("Platform settings", EditorStyles.boldLabel, new GUILayoutOption[0]);
                int index = EditorGUILayout.BeginPlatformGrouping(buildPlayerValidPlatforms, null);
                if (buildPlayerValidPlatforms[index].name == BuildPipeline.GetEditorTargetName())
                {
                    this.ShowEditorSettings();
                }
                else
                {
                    BuildTargetGroup targetGroup = buildPlayerValidPlatforms[index].targetGroup;
                    if (targetGroup == BuildTargetGroup.Standalone)
                    {
                        this.m_DesktopExtension.OnPlatformSettingsGUI(this);
                    }
                    else
                    {
                        IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(targetGroup);
                        if (pluginImporterExtension != null)
                        {
                            pluginImporterExtension.OnPlatformSettingsGUI(this);
                        }
                    }
                }
                EditorGUILayout.EndPlatformGrouping();
            }
        }

        private int ToggleWithMixedValue(int value, string title)
        {
            EditorGUI.showMixedValue = value == -1;
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUILayout.Toggle(title, value == 1, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                return (!flag ? 0 : 1);
            }
            EditorGUI.showMixedValue = false;
            return value;
        }

        internal IPluginImporterExtension[] additionalExtensions
        {
            get
            {
                if (this.m_AdditionalExtensions == null)
                {
                    this.m_AdditionalExtensions = new IPluginImporterExtension[] { this.m_EditorExtension, this.m_DesktopExtension };
                }
                return this.m_AdditionalExtensions;
            }
        }

        private int compatibleWithStandalone
        {
            get
            {
                bool flag = false;
                foreach (BuildTarget target in m_StandaloneTargets)
                {
                    if (this.m_CompatibleWithPlatform[(int) target] == -1)
                    {
                        return -1;
                    }
                    flag |= this.m_CompatibleWithPlatform[(int) target] > 0;
                }
                return (!flag ? 0 : 1);
            }
            set
            {
                foreach (BuildTarget target in m_StandaloneTargets)
                {
                    this.m_CompatibleWithPlatform[(int) target] = value;
                }
            }
        }

        internal PluginImporter importer
        {
            get
            {
                return (this.target as PluginImporter);
            }
        }

        internal PluginImporter[] importers
        {
            get
            {
                return base.targets.Cast<PluginImporter>().ToArray<PluginImporter>();
            }
        }

        internal override bool showImportedObject
        {
            get
            {
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <ResetValues>c__AnonStorey79
        {
            internal BuildTarget platform;

            internal bool <>m__120(PluginImporter imp)
            {
                return imp.GetCompatibleWithPlatform(this.platform);
            }
        }

        private delegate bool GetComptability(PluginImporter imp);
    }
}

