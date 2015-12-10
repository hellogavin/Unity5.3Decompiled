namespace UnityEditor.Modules
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    internal interface IBuildPostprocessor
    {
        string[] FindPluginFilesToCopy(string basePluginFolder, out bool shouldRetainStructure);
        string GetExtension(BuildTarget target, BuildOptions options);
        string GetScriptLayoutFileFromBuild(BuildOptions options, string installPath, string fileName);
        void LaunchPlayer(BuildLaunchPlayerArgs args);
        void PostProcess(BuildPostProcessArgs args);
        void PostProcessScriptsOnly(BuildPostProcessArgs args);
        bool SupportsInstallInBuildFolder();
        bool SupportsScriptsOnlyBuild();
    }
}

