namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ProjectStateRestHandler : Handler
    {
        [CompilerGenerated]
        private static Func<MonoIsland, Island> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Island, IEnumerable<string>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Island, JSONValue> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<JSONValue, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache4;

        private static string[] FindEmptyDirectories(string path, string[] files)
        {
            <FindEmptyDirectories>c__AnonStorey26 storey = new <FindEmptyDirectories>c__AnonStorey26 {
                files = files
            };
            return FindPotentialEmptyDirectories(path).Where<string>(new Func<string, bool>(storey.<>m__37)).ToArray<string>();
        }

        private static IEnumerable<string> FindPotentialEmptyDirectories(string path)
        {
            List<string> result = new List<string>();
            FindPotentialEmptyDirectories(path, result);
            return result;
        }

        private static void FindPotentialEmptyDirectories(string path, ICollection<string> result)
        {
            string[] directories = Directory.GetDirectories(path);
            if (directories.Length == 0)
            {
                result.Add(path.Replace('\\', '/'));
            }
            else
            {
                foreach (string str in directories)
                {
                    FindPotentialEmptyDirectories(str, result);
                }
            }
        }

        private static IEnumerable<string> GetAllSupportedFiles()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = asset => IsSupportedExtension(Path.GetExtension(asset));
            }
            return AssetDatabase.GetAllAssetPaths().Where<string>(<>f__am$cache4);
        }

        protected override JSONValue HandleGet(Request request, JSONValue payload)
        {
            AssetDatabase.Refresh();
            return JsonForProject();
        }

        private static bool IsSupportedExtension(string extension)
        {
            <IsSupportedExtension>c__AnonStorey25 storey = new <IsSupportedExtension>c__AnonStorey25 {
                extension = extension
            };
            if (storey.extension.StartsWith("."))
            {
                storey.extension = storey.extension.Substring(1);
            }
            return EditorSettings.projectGenerationBuiltinExtensions.Concat<string>(EditorSettings.projectGenerationUserExtensions).Any<string>(new Func<string, bool>(storey.<>m__35));
        }

        private static JSONValue JsonForIsland(Island island)
        {
            if ((island.Name == "UnityEngine") || (island.Name == "UnityEditor"))
            {
                return 0;
            }
            JSONValue value2 = new JSONValue();
            value2["name"] = island.Name;
            value2["language"] = !island.Name.Contains("Boo") ? (!island.Name.Contains("UnityScript") ? "C#" : "UnityScript") : "Boo";
            value2["files"] = Handler.ToJSON(island.MonoIsland._files);
            value2["defines"] = Handler.ToJSON(island.MonoIsland._defines);
            value2["references"] = Handler.ToJSON(island.MonoIsland._references);
            value2["basedirectory"] = ProjectPath;
            return value2;
        }

        private static JSONValue JsonForProject()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = i => new Island { MonoIsland = i, Name = Path.GetFileNameWithoutExtension(i._output), References = i._references.ToList<string>() };
            }
            List<Island> source = InternalEditorUtility.GetMonoIslands().Select<MonoIsland, Island>(<>f__am$cache0).ToList<Island>();
            foreach (Island island in source)
            {
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                foreach (string str in island.References)
                {
                    <JsonForProject>c__AnonStorey24 storey = new <JsonForProject>c__AnonStorey24 {
                        refName = Path.GetFileNameWithoutExtension(str)
                    };
                    if (str.StartsWith("Library/") && source.Any<Island>(new Func<Island, bool>(storey.<>m__31)))
                    {
                        list2.Add(storey.refName);
                        list3.Add(str);
                    }
                    if ((str.EndsWith("/UnityEditor.dll") || str.EndsWith("/UnityEngine.dll")) || (str.EndsWith(@"\UnityEditor.dll") || str.EndsWith(@"\UnityEngine.dll")))
                    {
                        list3.Add(str);
                    }
                }
                island.References.Add(InternalEditorUtility.GetEditorAssemblyPath());
                island.References.Add(InternalEditorUtility.GetEngineAssemblyPath());
                foreach (string str2 in list2)
                {
                    island.References.Add(str2);
                }
                foreach (string str3 in list3)
                {
                    island.References.Remove(str3);
                }
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = (Func<Island, IEnumerable<string>>) (i => i.MonoIsland._files);
            }
            string[] files = source.SelectMany<Island, string>(<>f__am$cache1).Concat<string>(GetAllSupportedFiles()).Distinct<string>().ToArray<string>();
            string[] strings = RelativeToProjectPath(FindEmptyDirectories(AssetsPath, files));
            JSONValue value2 = new JSONValue();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = i => JsonForIsland(i);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = i2 => !i2.IsNull();
            }
            value2["islands"] = new JSONValue(source.Select<Island, JSONValue>(<>f__am$cache2).Where<JSONValue>(<>f__am$cache3).ToList<JSONValue>());
            value2["basedirectory"] = ProjectPath;
            JSONValue value3 = new JSONValue();
            value3["files"] = Handler.ToJSON(files);
            value3["emptydirectories"] = Handler.ToJSON(strings);
            value2["assetdatabase"] = value3;
            return value2;
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/projectstate", new ProjectStateRestHandler());
        }

        private static string[] RelativeToProjectPath(string[] paths)
        {
            <RelativeToProjectPath>c__AnonStorey28 storey = new <RelativeToProjectPath>c__AnonStorey28 {
                projectPath = !ProjectPath.EndsWith("/") ? (ProjectPath + "/") : ProjectPath
            };
            return paths.Select<string, string>(new Func<string, string>(storey.<>m__38)).ToArray<string>();
        }

        private static string AssetsPath
        {
            get
            {
                return (ProjectPath + "/Assets");
            }
        }

        private static string ProjectPath
        {
            get
            {
                return Path.GetDirectoryName(Application.dataPath);
            }
        }

        [CompilerGenerated]
        private sealed class <FindEmptyDirectories>c__AnonStorey26
        {
            internal string[] files;

            internal bool <>m__37(string d)
            {
                <FindEmptyDirectories>c__AnonStorey27 storey = new <FindEmptyDirectories>c__AnonStorey27 {
                    <>f__ref$38 = this,
                    d = d
                };
                return !this.files.Any<string>(new Func<string, bool>(storey.<>m__39));
            }

            private sealed class <FindEmptyDirectories>c__AnonStorey27
            {
                internal ProjectStateRestHandler.<FindEmptyDirectories>c__AnonStorey26 <>f__ref$38;
                internal string d;

                internal bool <>m__39(string f)
                {
                    return f.StartsWith(this.d);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <IsSupportedExtension>c__AnonStorey25
        {
            internal string extension;

            internal bool <>m__35(string s)
            {
                return string.Equals(s, this.extension, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        [CompilerGenerated]
        private sealed class <JsonForProject>c__AnonStorey24
        {
            internal string refName;

            internal bool <>m__31(ProjectStateRestHandler.Island i)
            {
                return (i.Name == this.refName);
            }
        }

        [CompilerGenerated]
        private sealed class <RelativeToProjectPath>c__AnonStorey28
        {
            internal string projectPath;

            internal string <>m__38(string d)
            {
                return (!d.StartsWith(this.projectPath) ? d : d.Substring(this.projectPath.Length));
            }
        }

        public class Island
        {
            public UnityEditor.Scripting.MonoIsland MonoIsland { get; set; }

            public string Name { get; set; }

            public List<string> References { get; set; }
        }
    }
}

