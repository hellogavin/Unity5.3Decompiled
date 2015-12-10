namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal interface IPluginImporterExtension
    {
        void Apply(PluginImporterInspector inspector);
        string CalculateFinalPluginPath(string buildTargetName, PluginImporter imp);
        bool CheckFileCollisions(string buildTargetName);
        bool HasModified(PluginImporterInspector inspector);
        void OnDisable(PluginImporterInspector inspector);
        void OnEnable(PluginImporterInspector inspector);
        void OnPlatformSettingsGUI(PluginImporterInspector inspector);
        void ResetValues(PluginImporterInspector inspector);
    }
}

