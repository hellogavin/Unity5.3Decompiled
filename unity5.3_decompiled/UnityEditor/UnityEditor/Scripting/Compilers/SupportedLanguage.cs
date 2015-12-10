namespace UnityEditor.Scripting.Compilers
{
    using System;
    using UnityEditor;
    using UnityEditor.Scripting;

    internal abstract class SupportedLanguage
    {
        protected SupportedLanguage()
        {
        }

        public abstract ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater);
        public abstract string GetExtensionICanCompile();
        public abstract string GetLanguageName();
        public virtual string GetNamespace(string fileName)
        {
            return string.Empty;
        }
    }
}

