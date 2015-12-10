namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class DesktopPluginImporterExtension : DefaultPluginImporterExtension
    {
        private DesktopSingleCPUProperty m_LinuxX86;
        private DesktopSingleCPUProperty m_LinuxX86_X64;
        private DesktopSingleCPUProperty m_OSXX86;
        private DesktopSingleCPUProperty m_OSXX86_X64;
        private DesktopSingleCPUProperty m_WindowsX86;
        private DesktopSingleCPUProperty m_WindowsX86_X64;

        public DesktopPluginImporterExtension() : base(null)
        {
            base.properties = this.GetProperties();
        }

        public override string CalculateFinalPluginPath(string platformName, PluginImporter imp)
        {
            BuildTarget buildTargetByName = BuildPipeline.GetBuildTargetByName(platformName);
            bool flag = (buildTargetByName == BuildTarget.StandaloneWindows) || (buildTargetByName == BuildTarget.StandaloneWindows64);
            bool flag2 = ((buildTargetByName == BuildTarget.StandaloneOSXIntel) || (buildTargetByName == BuildTarget.StandaloneOSXIntel64)) || (buildTargetByName == BuildTarget.StandaloneOSXUniversal);
            bool flag3 = ((buildTargetByName == BuildTarget.StandaloneLinux) || (buildTargetByName == BuildTarget.StandaloneLinux64)) || (buildTargetByName == BuildTarget.StandaloneLinuxUniversal);
            if ((!flag3 && !flag2) && !flag)
            {
                throw new Exception(string.Format("Failed to resolve standalone platform, platform string '{0}', resolved target '{1}'", platformName, buildTargetByName.ToString()));
            }
            if (flag && !this.IsUsableOnWindows(imp))
            {
                return string.Empty;
            }
            if (flag2 && !this.IsUsableOnOSX(imp))
            {
                return string.Empty;
            }
            if (flag3 && !this.IsUsableOnLinux(imp))
            {
                return string.Empty;
            }
            string platformData = imp.GetPlatformData(platformName, "CPU");
            if (string.Compare(platformData, "None", true) == 0)
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(platformData) && (string.Compare(platformData, "AnyCPU", true) != 0))
            {
                return Path.Combine(platformData, Path.GetFileName(imp.assetPath));
            }
            return Path.GetFileName(imp.assetPath);
        }

        private DesktopPluginCPUArchitecture CalculateMultiCPUArchitecture(bool x86, bool x64)
        {
            if (x86 && x64)
            {
                return DesktopPluginCPUArchitecture.AnyCPU;
            }
            if (x86 && !x64)
            {
                return DesktopPluginCPUArchitecture.x86;
            }
            if (!x86 && x64)
            {
                return DesktopPluginCPUArchitecture.x86_64;
            }
            return DesktopPluginCPUArchitecture.None;
        }

        private DefaultPluginImporterExtension.Property[] GetProperties()
        {
            List<DefaultPluginImporterExtension.Property> list = new List<DefaultPluginImporterExtension.Property>();
            this.m_WindowsX86 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneWindows));
            this.m_WindowsX86_X64 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneWindows64));
            this.m_LinuxX86 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinux), DesktopPluginCPUArchitecture.x86);
            this.m_LinuxX86_X64 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinux64), DesktopPluginCPUArchitecture.x86_64);
            this.m_OSXX86 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXIntel));
            this.m_OSXX86_X64 = new DesktopSingleCPUProperty(EditorGUIUtility.TextContent("x86_x64"), BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXIntel64));
            list.Add(this.m_WindowsX86);
            list.Add(this.m_WindowsX86_X64);
            list.Add(this.m_LinuxX86);
            list.Add(this.m_LinuxX86_X64);
            list.Add(this.m_OSXX86);
            list.Add(this.m_OSXX86_X64);
            return list.ToArray();
        }

        private bool IsUsableOnLinux(PluginImporter imp)
        {
            return (!imp.isNativePlugin || (Path.GetExtension(imp.assetPath) == ".so"));
        }

        private bool IsUsableOnOSX(PluginImporter imp)
        {
            if (imp.isNativePlugin)
            {
                string extension = Path.GetExtension(imp.assetPath);
                if (!(extension == ".so") && !(extension == ".bundle"))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsUsableOnWindows(PluginImporter imp)
        {
            return (!imp.isNativePlugin || (Path.GetExtension(imp.assetPath) == ".dll"));
        }

        public override void OnPlatformSettingsGUI(PluginImporterInspector inspector)
        {
            PluginImporter imp = inspector.importer;
            EditorGUI.BeginChangeCheck();
            if (this.IsUsableOnWindows(imp))
            {
                EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Windows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.m_WindowsX86.OnGUI(inspector);
                this.m_WindowsX86_X64.OnGUI(inspector);
                EditorGUILayout.Space();
            }
            if (this.IsUsableOnLinux(imp))
            {
                EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Linux"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.m_LinuxX86.OnGUI(inspector);
                this.m_LinuxX86_X64.OnGUI(inspector);
                EditorGUILayout.Space();
            }
            if (this.IsUsableOnOSX(imp))
            {
                EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Mac OS X"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.m_OSXX86.OnGUI(inspector);
                this.m_OSXX86_X64.OnGUI(inspector);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ValidateUniversalTargets(inspector);
                base.hasModified = true;
            }
        }

        public void ValidateSingleCPUTargets(PluginImporterInspector inspector)
        {
            DesktopSingleCPUProperty[] propertyArray = new DesktopSingleCPUProperty[] { this.m_WindowsX86, this.m_WindowsX86_X64, this.m_LinuxX86, this.m_LinuxX86_X64, this.m_OSXX86, this.m_OSXX86_X64 };
            foreach (DesktopSingleCPUProperty property in propertyArray)
            {
                string str = !property.IsTargetEnabled(inspector) ? DesktopPluginCPUArchitecture.None.ToString() : property.defaultValue.ToString();
                inspector.importer.SetPlatformData(property.platformName, "CPU", str);
            }
            this.ValidateUniversalTargets(inspector);
        }

        private void ValidateUniversalTargets(PluginImporterInspector inspector)
        {
            bool flag = this.m_LinuxX86.IsTargetEnabled(inspector);
            bool flag2 = this.m_LinuxX86_X64.IsTargetEnabled(inspector);
            DesktopPluginCPUArchitecture architecture = this.CalculateMultiCPUArchitecture(flag, flag2);
            inspector.importer.SetPlatformData(BuildTarget.StandaloneLinuxUniversal, "CPU", architecture.ToString());
            inspector.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneLinuxUniversal), flag || flag2);
            bool flag3 = this.m_OSXX86.IsTargetEnabled(inspector);
            bool flag4 = this.m_OSXX86_X64.IsTargetEnabled(inspector);
            DesktopPluginCPUArchitecture architecture2 = this.CalculateMultiCPUArchitecture(flag3, flag4);
            inspector.importer.SetPlatformData(BuildTarget.StandaloneOSXUniversal, "CPU", architecture2.ToString());
            inspector.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(BuildTarget.StandaloneOSXUniversal), flag3 || flag4);
        }

        internal enum DesktopPluginCPUArchitecture
        {
            None,
            AnyCPU,
            x86,
            x86_64
        }

        internal class DesktopSingleCPUProperty : DefaultPluginImporterExtension.Property
        {
            public DesktopSingleCPUProperty(GUIContent name, string platformName) : this(name, platformName, DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.AnyCPU)
            {
            }

            public DesktopSingleCPUProperty(GUIContent name, string platformName, DesktopPluginImporterExtension.DesktopPluginCPUArchitecture architecture) : base(name, "CPU", architecture, platformName)
            {
            }

            internal bool IsTargetEnabled(PluginImporterInspector inspector)
            {
                return inspector.GetCompatibleWithPlatform(base.platformName);
            }

            internal override void OnGUI(PluginImporterInspector inspector)
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle(base.name, this.IsTargetEnabled(inspector), new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    base.value = !enabled ? DesktopPluginImporterExtension.DesktopPluginCPUArchitecture.None : base.defaultValue;
                    inspector.SetCompatibleWithPlatform(base.platformName, enabled);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

