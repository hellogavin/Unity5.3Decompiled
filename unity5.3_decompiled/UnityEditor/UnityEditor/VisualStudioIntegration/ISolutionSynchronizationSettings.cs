namespace UnityEditor.VisualStudioIntegration
{
    using System;

    internal interface ISolutionSynchronizationSettings
    {
        string GetProjectFooterTemplate(ScriptingLanguage language);
        string GetProjectHeaderTemplate(ScriptingLanguage language);

        string[] Defines { get; }

        string EditorAssemblyPath { get; }

        string EngineAssemblyPath { get; }

        string MonoLibFolder { get; }

        string SolutionProjectConfigurationTemplate { get; }

        string SolutionProjectEntryTemplate { get; }

        string SolutionTemplate { get; }

        int VisualStudioVersion { get; }
    }
}

