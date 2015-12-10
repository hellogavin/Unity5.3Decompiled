namespace UnityEditor.Modules
{
    using System;
    using UnityEngine;

    internal interface IPlatformSupportModule
    {
        IBuildPostprocessor CreateBuildPostprocessor();
        IBuildWindowExtension CreateBuildWindowExtension();
        ICompilationExtension CreateCompilationExtension();
        IDevice CreateDevice(string id);
        IPluginImporterExtension CreatePluginImporterExtension();
        IPreferenceWindowExtension CreatePreferenceWindowExtension();
        IScriptingImplementations CreateScriptingImplementations();
        ISettingEditorExtension CreateSettingsEditorExtension();
        IUserAssembliesValidator CreateUserAssembliesValidatorExtension();
        GUIContent[] GetDisplayNames();
        void OnActivate();
        void OnDeactivate();
        void OnLoad();
        void OnUnload();

        string[] AssemblyReferencesForUserScripts { get; }

        string ExtensionVersion { get; }

        string JamTarget { get; }

        string TargetName { get; }
    }
}

