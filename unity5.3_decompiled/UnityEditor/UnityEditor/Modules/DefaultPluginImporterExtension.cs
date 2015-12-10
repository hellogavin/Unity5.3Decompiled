namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    internal class DefaultPluginImporterExtension : IPluginImporterExtension
    {
        protected bool hasModified;
        protected Property[] properties;
        internal bool propertiesRefreshed;

        public DefaultPluginImporterExtension(Property[] properties)
        {
            this.properties = properties;
        }

        public virtual void Apply(PluginImporterInspector inspector)
        {
            if (this.propertiesRefreshed)
            {
                foreach (Property property in this.properties)
                {
                    property.Apply(inspector);
                }
            }
        }

        public virtual string CalculateFinalPluginPath(string platformName, PluginImporter imp)
        {
            return Path.GetFileName(imp.assetPath);
        }

        public virtual bool CheckFileCollisions(string buildTargetName)
        {
            Dictionary<string, List<PluginImporter>> compatiblePlugins = this.GetCompatiblePlugins(buildTargetName);
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, List<PluginImporter>> pair in compatiblePlugins)
            {
                List<PluginImporter> list = pair.Value;
                if (list.Count != 1)
                {
                    flag = true;
                    builder.AppendLine(string.Format("Plugin '{0}' is used from several locations:", Path.GetFileName(pair.Key)));
                    foreach (PluginImporter importer in list)
                    {
                        builder.AppendLine(" " + importer.assetPath + " would be copied to <PluginPath>/" + pair.Key.Replace(@"\", "/"));
                    }
                }
            }
            if (flag)
            {
                builder.AppendLine("Please fix plugin settings and try again.");
                Debug.LogError(builder.ToString());
            }
            return flag;
        }

        protected Dictionary<string, List<PluginImporter>> GetCompatiblePlugins(string buildTargetName)
        {
            <GetCompatiblePlugins>c__AnonStorey78 storey = new <GetCompatiblePlugins>c__AnonStorey78 {
                buildTargetName = buildTargetName
            };
            PluginImporter[] importerArray = PluginImporter.GetAllImporters().Where<PluginImporter>(new Func<PluginImporter, bool>(storey.<>m__11B)).ToArray<PluginImporter>();
            Dictionary<string, List<PluginImporter>> dictionary = new Dictionary<string, List<PluginImporter>>();
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(storey.buildTargetName);
            foreach (PluginImporter importer in importerArray)
            {
                if (!string.IsNullOrEmpty(importer.assetPath))
                {
                    string str = this.CalculateFinalPluginPath(storey.buildTargetName, importer);
                    if (!string.IsNullOrEmpty(str) && (!importer.isNativePlugin || (buildTargetGroupByName != BuildTargetGroup.WebPlayer)))
                    {
                        List<PluginImporter> list = null;
                        if (!dictionary.TryGetValue(str, out list))
                        {
                            list = new List<PluginImporter>();
                            dictionary[str] = list;
                        }
                        list.Add(importer);
                    }
                }
            }
            return dictionary;
        }

        public virtual bool HasModified(PluginImporterInspector inspector)
        {
            return this.hasModified;
        }

        public virtual void OnDisable(PluginImporterInspector inspector)
        {
        }

        public virtual void OnEnable(PluginImporterInspector inspector)
        {
            this.RefreshProperties(inspector);
        }

        public virtual void OnPlatformSettingsGUI(PluginImporterInspector inspector)
        {
            if (!this.propertiesRefreshed)
            {
                this.RefreshProperties(inspector);
            }
            EditorGUI.BeginChangeCheck();
            foreach (Property property in this.properties)
            {
                property.OnGUI(inspector);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.hasModified = true;
            }
        }

        protected virtual void RefreshProperties(PluginImporterInspector inspector)
        {
            foreach (Property property in this.properties)
            {
                property.Reset(inspector);
            }
            this.propertiesRefreshed = true;
        }

        public virtual void ResetValues(PluginImporterInspector inspector)
        {
            this.hasModified = false;
            this.RefreshProperties(inspector);
        }

        [CompilerGenerated]
        private sealed class <GetCompatiblePlugins>c__AnonStorey78
        {
            internal string buildTargetName;

            internal bool <>m__11B(PluginImporter imp)
            {
                return ((imp.GetCompatibleWithPlatform(this.buildTargetName) || imp.GetCompatibleWithAnyPlatform()) && !string.IsNullOrEmpty(imp.assetPath));
            }
        }

        internal class Property
        {
            internal Property(string name, string key, object defaultValue, string platformName) : this(new GUIContent(name), key, defaultValue, platformName)
            {
            }

            internal Property(GUIContent name, string key, object defaultValue, string platformName)
            {
                this.name = name;
                this.key = key;
                this.defaultValue = defaultValue;
                this.type = defaultValue.GetType();
                this.platformName = platformName;
            }

            internal virtual void Apply(PluginImporterInspector inspector)
            {
                inspector.importer.SetPlatformData(this.platformName, this.key, this.value.ToString());
            }

            internal virtual void OnGUI(PluginImporterInspector inspector)
            {
                if (this.type == typeof(bool))
                {
                    this.value = EditorGUILayout.Toggle(this.name, (bool) this.value, new GUILayoutOption[0]);
                }
                else if (this.type.IsEnum)
                {
                    this.value = EditorGUILayout.EnumPopup(this.name, (Enum) this.value, new GUILayoutOption[0]);
                }
                else
                {
                    if (this.type != typeof(string))
                    {
                        throw new NotImplementedException("Don't know how to display value.");
                    }
                    this.value = EditorGUILayout.TextField(this.name, (string) this.value, new GUILayoutOption[0]);
                }
            }

            internal virtual void Reset(PluginImporterInspector inspector)
            {
                string platformData = inspector.importer.GetPlatformData(this.platformName, this.key);
                try
                {
                    this.value = TypeDescriptor.GetConverter(this.type).ConvertFromString(platformData);
                }
                catch
                {
                    this.value = this.defaultValue;
                    if (!string.IsNullOrEmpty(platformData))
                    {
                        Debug.LogWarning(string.Concat(new object[] { "Failed to parse value ('", platformData, "') for ", this.key, ", platform: ", this.platformName, ", type: ", this.type, ". Default value will be set '", this.defaultValue, "'" }));
                    }
                }
            }

            internal object defaultValue { get; set; }

            internal string key { get; set; }

            internal GUIContent name { get; set; }

            internal string platformName { get; set; }

            internal Type type { get; set; }

            internal object value { get; set; }
        }
    }
}

