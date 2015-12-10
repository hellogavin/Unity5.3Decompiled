namespace UnityEditorInternal.VR
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditorInternal;

    internal static class VRPostProcess
    {
        [RegisterPlugins]
        private static IEnumerable<PluginDesc> RegisterPlugins(BuildTarget target)
        {
            if ((target == BuildTarget.Android) && PlayerSettings.virtualRealitySupported)
            {
                PluginDesc desc = new PluginDesc();
                string str = EditorApplication.applicationContentsPath + "/VR/oculus/" + BuildPipeline.GetBuildTargetName(target);
                desc.pluginPath = Path.Combine(str, "ovrplugin.aar");
                return new PluginDesc[] { desc };
            }
            return new PluginDesc[0];
        }
    }
}

