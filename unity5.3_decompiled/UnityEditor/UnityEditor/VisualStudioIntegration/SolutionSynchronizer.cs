namespace UnityEditor.VisualStudioIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditorInternal;

    internal class SolutionSynchronizer
    {
        private static readonly Regex _MonoDevelopPropertyHeader;
        private readonly string _projectDirectory;
        private readonly string _projectName;
        private readonly ISolutionSynchronizationSettings _settings;
        [CompilerGenerated]
        private static Func<MonoIsland, bool> <>f__am$cacheC;
        internal static readonly Dictionary<string, ScriptingLanguage> BuiltinSupportedExtensions;
        private static readonly string DefaultMonoDevelopSolutionProperties;
        public static readonly ISolutionSynchronizationSettings DefaultSynchronizationSettings = new DefaultSolutionSynchronizationSettings();
        public static readonly string MSBuildNamespaceUri;
        private static readonly Dictionary<ScriptingLanguage, string> ProjectExtensions;
        private string[] ProjectSupportedExtensions;
        public static readonly Regex scriptReferenceExpression;
        private static readonly string WindowsNewline = "\r\n";

        static SolutionSynchronizer()
        {
            Dictionary<string, ScriptingLanguage> dictionary = new Dictionary<string, ScriptingLanguage>();
            dictionary.Add("cs", ScriptingLanguage.CSharp);
            dictionary.Add("js", ScriptingLanguage.UnityScript);
            dictionary.Add("boo", ScriptingLanguage.Boo);
            dictionary.Add("shader", ScriptingLanguage.None);
            dictionary.Add("compute", ScriptingLanguage.None);
            dictionary.Add("cginc", ScriptingLanguage.None);
            dictionary.Add("glslinc", ScriptingLanguage.None);
            BuiltinSupportedExtensions = dictionary;
            Dictionary<ScriptingLanguage, string> dictionary2 = new Dictionary<ScriptingLanguage, string>();
            dictionary2.Add(ScriptingLanguage.Boo, ".booproj");
            dictionary2.Add(ScriptingLanguage.CSharp, ".csproj");
            dictionary2.Add(ScriptingLanguage.UnityScript, ".unityproj");
            dictionary2.Add(ScriptingLanguage.None, ".csproj");
            ProjectExtensions = dictionary2;
            _MonoDevelopPropertyHeader = new Regex(@"^\s*GlobalSection\(MonoDevelopProperties.*\)");
            MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";
            string[] textArray1 = new string[] { "    GlobalSection(MonoDevelopProperties) = preSolution", "        StartupItem = Assembly-CSharp.csproj", "    EndGlobalSection" };
            DefaultMonoDevelopSolutionProperties = string.Join("\r\n", textArray1).Replace("    ", "\t");
            scriptReferenceExpression = new Regex("^Library.ScriptAssemblies.(?<project>Assembly-(?<language>[^-]+)(?<editor>-Editor)?(?<firstpass>-firstpass)?).dll$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public SolutionSynchronizer(string projectDirectory) : this(projectDirectory, DefaultSynchronizationSettings)
        {
        }

        public SolutionSynchronizer(string projectDirectory, ISolutionSynchronizationSettings settings)
        {
            this.ProjectSupportedExtensions = new string[0];
            this._projectDirectory = projectDirectory;
            this._settings = settings;
            this._projectName = Path.GetFileName(this._projectDirectory);
        }

        private static void DumpIsland(MonoIsland island)
        {
            Console.WriteLine("{0} ({1})", island._output, island._classlib_profile);
            Console.WriteLine("Files: ");
            Console.WriteLine(string.Join("\n", island._files));
            Console.WriteLine("References: ");
            Console.WriteLine(string.Join("\n", island._references));
            Console.WriteLine(string.Empty);
        }

        private string EscapedRelativePathFor(string file)
        {
            string str = this._projectDirectory.Replace("/", @"\");
            file = file.Replace("/", @"\");
            return SecurityElement.Escape(!file.StartsWith(str) ? file : file.Substring(this._projectDirectory.Length + 1));
        }

        private string GenerateAllAssetProjectPart()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in AssetDatabase.GetAllAssetPaths())
            {
                string extension = Path.GetExtension(str);
                if (this.IsSupportedExtension(extension) && (ScriptingLanguageFor(extension) == ScriptingLanguage.None))
                {
                    builder.AppendFormat("     <None Include=\"{0}\" />{1}", this.EscapedRelativePathFor(str), WindowsNewline);
                }
            }
            return builder.ToString();
        }

        private string GetProjectActiveConfigurations(string projectGuid)
        {
            return string.Format(DefaultSynchronizationSettings.SolutionProjectConfigurationTemplate, projectGuid);
        }

        private string GetProjectEntries(IEnumerable<MonoIsland> islands)
        {
            IEnumerable<string> source = from i in islands select string.Format(DefaultSynchronizationSettings.SolutionProjectEntryTemplate, new object[] { this.SolutionGuid(), this._projectName, Path.GetFileName(this.ProjectFile(i)), this.ProjectGuid(i._output) });
            return string.Join(WindowsNewline, source.ToArray<string>());
        }

        public static string GetProjectExtension(ScriptingLanguage language)
        {
            if (!ProjectExtensions.ContainsKey(language))
            {
                throw new ArgumentException("Unsupported language", "language");
            }
            return ProjectExtensions[language];
        }

        [Obsolete("Use AssemblyHelper.IsManagedAssembly")]
        public static bool IsManagedAssembly(string file)
        {
            return AssemblyHelper.IsManagedAssembly(file);
        }

        private static bool IsSelectedEditorInternalMonoDevelop()
        {
            return (InternalEditorUtility.GetExternalScriptEditor() == "internal");
        }

        private static bool IsSelectedEditorVisualStudio()
        {
            string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
            return (externalScriptEditor.EndsWith("devenv.exe") || externalScriptEditor.EndsWith("vcsexpress.exe"));
        }

        private bool IsSupportedExtension(string extension)
        {
            char[] trimChars = new char[] { '.' };
            extension = extension.TrimStart(trimChars);
            return (BuiltinSupportedExtensions.ContainsKey(extension) || this.ProjectSupportedExtensions.Contains<string>(extension));
        }

        private static Mode ModeForCurrentExternalEditor()
        {
            if (IsSelectedEditorVisualStudio())
            {
                return Mode.UntiyScriptAsPrecompiledAssembly;
            }
            if (IsSelectedEditorInternalMonoDevelop())
            {
                return Mode.UnityScriptAsUnityProj;
            }
            return (!EditorPrefs.GetBool("kExternalEditorSupportsUnityProj", false) ? Mode.UntiyScriptAsPrecompiledAssembly : Mode.UnityScriptAsUnityProj);
        }

        public bool ProjectExists(MonoIsland island)
        {
            return File.Exists(this.ProjectFile(island));
        }

        public string ProjectFile(MonoIsland island)
        {
            ScriptingLanguage language = ScriptingLanguageFor(island);
            return Path.Combine(this._projectDirectory, string.Format("{0}{1}", Path.GetFileNameWithoutExtension(island._output), ProjectExtensions[language]));
        }

        private string ProjectFooter(MonoIsland island)
        {
            return string.Format(this._settings.GetProjectFooterTemplate(ScriptingLanguageFor(island)), this.ReadExistingMonoDevelopProjectProperties(island));
        }

        private string ProjectGuid(string assembly)
        {
            return SolutionGuidGenerator.GuidForProject(this._projectName + Path.GetFileNameWithoutExtension(assembly));
        }

        private string ProjectHeader(MonoIsland island)
        {
            string str3;
            string str = "4.0";
            string str2 = "10.0.20506";
            ScriptingLanguage language = ScriptingLanguageFor(island);
            if (this._settings.VisualStudioVersion == 9)
            {
                str = "3.5";
                str2 = "9.0.21022";
            }
            object[] objArray1 = new object[9];
            objArray1[0] = str;
            objArray1[1] = str2;
            objArray1[2] = this.ProjectGuid(island._output);
            objArray1[3] = this._settings.EngineAssemblyPath;
            objArray1[4] = this._settings.EditorAssemblyPath;
            string[] first = new string[] { "DEBUG", "TRACE" };
            objArray1[5] = string.Join(";", first.Concat<string>(this._settings.Defines).Concat<string>(island._defines).Distinct<string>().ToArray<string>());
            objArray1[6] = MSBuildNamespaceUri;
            objArray1[7] = Path.GetFileNameWithoutExtension(island._output);
            objArray1[8] = EditorSettings.projectGenerationRootNamespace;
            object[] args = objArray1;
            try
            {
                str3 = string.Format(this._settings.GetProjectHeaderTemplate(language), args);
            }
            catch (Exception)
            {
                throw new NotSupportedException("Failed creating c# project because the c# project header did not have the correct amount of arguments, which is " + args.Length);
            }
            return str3;
        }

        private string ProjectText(MonoIsland island, Mode mode, string allAssetsProject)
        {
            StringBuilder builder = new StringBuilder(this.ProjectHeader(island));
            List<string> first = new List<string>();
            List<Match> list2 = new List<Match>();
            foreach (string str3 in island._files)
            {
                string str = Path.GetExtension(str3).ToLower();
                string file = !Path.IsPathRooted(str3) ? Path.Combine(this._projectDirectory, str3) : str3;
                if (".dll" != str)
                {
                    string str4 = "Compile";
                    builder.AppendFormat("     <{0} Include=\"{1}\" />{2}", str4, this.EscapedRelativePathFor(file), WindowsNewline);
                }
                else
                {
                    first.Add(file);
                }
            }
            builder.Append(allAssetsProject);
            IEnumerator<string> enumerator = first.Union<string>(island._references).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (((!current.EndsWith("/UnityEditor.dll") && !current.EndsWith("/UnityEngine.dll")) && !current.EndsWith(@"\UnityEditor.dll")) && !current.EndsWith(@"\UnityEngine.dll"))
                    {
                        Match item = scriptReferenceExpression.Match(current);
                        if (item.Success && ((mode == Mode.UnityScriptAsUnityProj) || (((int) Enum.Parse(typeof(ScriptingLanguage), item.Groups["language"].Value, true)) == 2)))
                        {
                            list2.Add(item);
                        }
                        else
                        {
                            string str6 = !Path.IsPathRooted(current) ? Path.Combine(this._projectDirectory, current) : current;
                            if (AssemblyHelper.IsManagedAssembly(str6) && !AssemblyHelper.IsInternalAssembly(str6))
                            {
                                str6 = str6.Replace(@"\", "/").Replace(@"\\", "/");
                                builder.AppendFormat(" <Reference Include=\"{0}\">{1}", Path.GetFileNameWithoutExtension(str6), WindowsNewline);
                                builder.AppendFormat(" <HintPath>{0}</HintPath>{1}", str6, WindowsNewline);
                                builder.AppendFormat(" </Reference>{0}", WindowsNewline);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (0 < list2.Count)
            {
                builder.AppendLine("  </ItemGroup>");
                builder.AppendLine("  <ItemGroup>");
                foreach (Match match2 in list2)
                {
                    string str7 = match2.Groups["project"].Value;
                    builder.AppendFormat("    <ProjectReference Include=\"{0}{1}\">{2}", str7, GetProjectExtension((ScriptingLanguage) ((int) Enum.Parse(typeof(ScriptingLanguage), match2.Groups["language"].Value, true))), WindowsNewline);
                    builder.AppendFormat("      <Project>{{{0}}}</Project>", this.ProjectGuid(Path.Combine("Temp", match2.Groups["project"].Value + ".dll")), WindowsNewline);
                    builder.AppendFormat("      <Name>{0}</Name>", str7, WindowsNewline);
                    builder.AppendLine("    </ProjectReference>");
                }
            }
            builder.Append(this.ProjectFooter(island));
            return builder.ToString();
        }

        private string ReadExistingMonoDevelopProjectProperties(MonoIsland island)
        {
            XmlNamespaceManager manager;
            if (!this.ProjectExists(island))
            {
                return string.Empty;
            }
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(this.ProjectFile(island));
                manager = new XmlNamespaceManager(document.NameTable);
                manager.AddNamespace("msb", MSBuildNamespaceUri);
            }
            catch (Exception exception)
            {
                if (!(exception is IOException) && !(exception is XmlException))
                {
                    throw;
                }
                return string.Empty;
            }
            XmlNodeList list = document.SelectNodes("/msb:Project/msb:ProjectExtensions", manager);
            if (list.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            IEnumerator enumerator = list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    builder.AppendLine(current.OuterXml);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return builder.ToString();
        }

        private string ReadExistingMonoDevelopSolutionProperties()
        {
            if (this.SolutionExists())
            {
                string[] strArray;
                try
                {
                    strArray = File.ReadAllLines(this.SolutionFile());
                }
                catch (IOException)
                {
                    return DefaultMonoDevelopSolutionProperties;
                }
                StringBuilder builder = new StringBuilder();
                bool flag = false;
                foreach (string str in strArray)
                {
                    if (_MonoDevelopPropertyHeader.IsMatch(str))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (str.Contains("EndGlobalSection"))
                        {
                            builder.Append(str);
                            flag = false;
                        }
                        else
                        {
                            builder.AppendFormat("{0}{1}", str, WindowsNewline);
                        }
                    }
                }
                if (0 < builder.Length)
                {
                    return builder.ToString();
                }
            }
            return DefaultMonoDevelopSolutionProperties;
        }

        private static IEnumerable<MonoIsland> RelevantIslandsForMode(IEnumerable<MonoIsland> islands, Mode mode)
        {
            <RelevantIslandsForMode>c__AnonStoreyB9 yb = new <RelevantIslandsForMode>c__AnonStoreyB9 {
                mode = mode
            };
            return islands.Where<MonoIsland>(new Func<MonoIsland, bool>(yb.<>m__227));
        }

        private static ScriptingLanguage ScriptingLanguageFor(string extension)
        {
            ScriptingLanguage language;
            char[] trimChars = new char[] { '.' };
            if (BuiltinSupportedExtensions.TryGetValue(extension.TrimStart(trimChars), out language))
            {
                return language;
            }
            return ScriptingLanguage.None;
        }

        private static ScriptingLanguage ScriptingLanguageFor(MonoIsland island)
        {
            return ScriptingLanguageFor(island.GetExtensionOfSourceFiles());
        }

        private void SetupProjectSupportedExtensions()
        {
            this.ProjectSupportedExtensions = EditorSettings.projectGenerationUserExtensions;
        }

        public bool ShouldFileBePartOfSolution(string file)
        {
            string extension = Path.GetExtension(file);
            return ((extension == ".dll") || this.IsSupportedExtension(extension));
        }

        public bool SolutionExists()
        {
            return File.Exists(this.SolutionFile());
        }

        internal string SolutionFile()
        {
            return Path.Combine(this._projectDirectory, string.Format("{0}.sln", this._projectName));
        }

        private string SolutionGuid()
        {
            return SolutionGuidGenerator.GuidForSolution(this._projectName);
        }

        private string SolutionText(IEnumerable<MonoIsland> islands, Mode mode)
        {
            string str = "11.00";
            if (this._settings.VisualStudioVersion == 9)
            {
                str = "10.00";
            }
            IEnumerable<MonoIsland> enumerable = RelevantIslandsForMode(islands, mode);
            string projectEntries = this.GetProjectEntries(enumerable);
            string str3 = string.Join(WindowsNewline, (from i in enumerable select this.GetProjectActiveConfigurations(this.ProjectGuid(i._output))).ToArray<string>());
            object[] args = new object[] { str, projectEntries, str3, this.ReadExistingMonoDevelopSolutionProperties() };
            return string.Format(this._settings.SolutionTemplate, args);
        }

        public void Sync()
        {
            this.SetupProjectSupportedExtensions();
            if (!AssetPostprocessingInternal.OnPreGeneratingCSProjectFiles())
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = i => 0 < i._files.Length;
                }
                IEnumerable<MonoIsland> islands = InternalEditorUtility.GetMonoIslands().Where<MonoIsland>(<>f__am$cacheC);
                string otherAssetsProjectPart = this.GenerateAllAssetProjectPart();
                this.SyncSolution(islands);
                IEnumerator<MonoIsland> enumerator = RelevantIslandsForMode(islands, ModeForCurrentExternalEditor()).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        MonoIsland current = enumerator.Current;
                        this.SyncProject(current, otherAssetsProjectPart);
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                AssetPostprocessingInternal.CallOnGeneratedCSProjectFiles();
            }
        }

        private static void SyncFileIfNotChanged(string filename, string newContents)
        {
            if (!File.Exists(filename) || (newContents != File.ReadAllText(filename)))
            {
                File.WriteAllText(filename, newContents);
            }
        }

        public bool SyncIfNeeded(IEnumerable<string> affectedFiles)
        {
            this.SetupProjectSupportedExtensions();
            if (this.SolutionExists() && affectedFiles.Any<string>(new Func<string, bool>(this.ShouldFileBePartOfSolution)))
            {
                this.Sync();
                return true;
            }
            return false;
        }

        private void SyncProject(MonoIsland island, string otherAssetsProjectPart)
        {
            SyncFileIfNotChanged(this.ProjectFile(island), this.ProjectText(island, ModeForCurrentExternalEditor(), otherAssetsProjectPart));
        }

        private void SyncSolution(IEnumerable<MonoIsland> islands)
        {
            SyncFileIfNotChanged(this.SolutionFile(), this.SolutionText(islands, ModeForCurrentExternalEditor()));
        }

        [CompilerGenerated]
        private sealed class <RelevantIslandsForMode>c__AnonStoreyB9
        {
            internal SolutionSynchronizer.Mode mode;

            internal bool <>m__227(MonoIsland i)
            {
                return ((this.mode == SolutionSynchronizer.Mode.UnityScriptAsUnityProj) || (ScriptingLanguage.CSharp == SolutionSynchronizer.ScriptingLanguageFor(i)));
            }
        }

        private enum Mode
        {
            UnityScriptAsUnityProj,
            UntiyScriptAsPrecompiledAssembly
        }
    }
}

