namespace UnityEditor
{
    using System;
    using UnityEditor.Modules;

    internal class EditorPluginImporterExtension : DefaultPluginImporterExtension
    {
        private EditorPluginCPUArchitecture cpu;
        private EditorPluginOSArchitecture os;

        public EditorPluginImporterExtension() : base(GetProperties())
        {
        }

        private static DefaultPluginImporterExtension.Property[] GetProperties()
        {
            return new DefaultPluginImporterExtension.Property[] { new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("CPU|Is plugin compatible with 32bit or 64bit Editor?"), "CPU", EditorPluginCPUArchitecture.AnyCPU, BuildPipeline.GetEditorTargetName()), new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("OS|Is plugin compatible with Windows, OS X or Linux Editor?"), "OS", EditorPluginOSArchitecture.AnyOS, BuildPipeline.GetEditorTargetName()) };
        }

        internal enum EditorPluginCPUArchitecture
        {
            AnyCPU,
            x86,
            x86_64
        }

        internal enum EditorPluginOSArchitecture
        {
            AnyOS,
            OSX,
            Windows,
            Linux
        }
    }
}

