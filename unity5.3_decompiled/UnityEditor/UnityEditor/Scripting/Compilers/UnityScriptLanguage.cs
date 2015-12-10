namespace UnityEditor.Scripting.Compilers
{
    using System;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal class UnityScriptLanguage : SupportedLanguage
    {
        public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            return new UnityScriptCompiler(island, runUpdater);
        }

        public override string GetExtensionICanCompile()
        {
            return "js";
        }

        public override string GetLanguageName()
        {
            return "UnityScript";
        }
    }
}

