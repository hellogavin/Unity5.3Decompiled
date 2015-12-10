namespace UnityEditor.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting.Compilers;

    internal static class ScriptCompilers
    {
        private static List<SupportedLanguage> _supportedLanguages = new List<SupportedLanguage>();
        [CompilerGenerated]
        private static Func<SupportedLanguage, SupportedLanguageStruct> <>f__am$cache1;

        static ScriptCompilers()
        {
            foreach (Type type in new List<Type> { typeof(CSharpLanguage), typeof(BooLanguage), typeof(UnityScriptLanguage) })
            {
                _supportedLanguages.Add((SupportedLanguage) Activator.CreateInstance(type));
            }
        }

        internal static ScriptCompilerBase CreateCompilerInstance(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            if (island._files.Length == 0)
            {
                throw new ArgumentException("Cannot compile MonoIsland with no files");
            }
            foreach (SupportedLanguage language in _supportedLanguages)
            {
                if (language.GetExtensionICanCompile() == island.GetExtensionOfSourceFiles())
                {
                    return language.CreateCompiler(island, buildingForEditor, targetPlatform, runUpdater);
                }
            }
            throw new ApplicationException(string.Format("Unable to find a suitable compiler for sources with extension '{0}' (Output assembly: {1})", island.GetExtensionOfSourceFiles(), island._output));
        }

        public static string GetExtensionOfSourceFile(string file)
        {
            return Path.GetExtension(file).ToLower().Substring(1);
        }

        internal static string GetNamespace(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Invalid file");
            }
            string extensionOfSourceFile = GetExtensionOfSourceFile(file);
            foreach (SupportedLanguage language in _supportedLanguages)
            {
                if (language.GetExtensionICanCompile() == extensionOfSourceFile)
                {
                    return language.GetNamespace(file);
                }
            }
            throw new ApplicationException("Unable to find a suitable compiler");
        }

        internal static SupportedLanguageStruct[] GetSupportedLanguageStructs()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = lang => new SupportedLanguageStruct { extension = lang.GetExtensionICanCompile(), languageName = lang.GetLanguageName() };
            }
            return _supportedLanguages.Select<SupportedLanguage, SupportedLanguageStruct>(<>f__am$cache1).ToArray<SupportedLanguageStruct>();
        }
    }
}

