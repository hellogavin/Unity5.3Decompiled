namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;
    using UnityEditor;
    using UnityEditorInternal;

    internal static class UWPReferences
    {
        private static UWPExtension[] GetExtensions(string folder, string version)
        {
            string path = Path.Combine(folder, "Extension SDKs");
            string referencesFolder = Path.Combine(folder, "References");
            List<UWPExtension> list = new List<UWPExtension>();
            foreach (string str3 in Directory.GetDirectories(path))
            {
                string[] paths = new string[] { str3, version, "SDKManifest.xml" };
                string str4 = FileUtil.CombinePaths(paths);
                if (File.Exists(str4))
                {
                    try
                    {
                        UWPExtension item = new UWPExtension(str4, referencesFolder);
                        list.Add(item);
                    }
                    catch
                    {
                    }
                }
            }
            return list.ToArray();
        }

        private static string[] GetPlatform(string folder, string version)
        {
            string referencesFolder = Path.Combine(folder, "References");
            string[] paths = new string[] { folder, @"Platforms\UAP", version, "Platform.xml" };
            string uri = FileUtil.CombinePaths(paths);
            XElement element = XDocument.Load(uri).Element("ApplicationPlatform");
            if (element.Attribute("name").Value != "UAP")
            {
                throw new Exception(string.Format("Invalid platform manifest at \"{0}\".", uri));
            }
            XElement containedApiContractsElement = element.Element("ContainedApiContracts");
            return GetReferences(referencesFolder, containedApiContractsElement);
        }

        public static string[] GetReferences()
        {
            string str;
            Version version;
            GetWindowsKit10(out str, out version);
            string str2 = version.ToString();
            if (version.Minor == -1)
            {
                str2 = str2 + ".0";
            }
            if (version.Build == -1)
            {
                str2 = str2 + ".0";
            }
            if (version.Revision == -1)
            {
                str2 = str2 + ".0";
            }
            HashSet<string> source = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            string item = Path.Combine(str, @"UnionMetadata\Facade\Windows.winmd");
            source.Add(item);
            foreach (string str4 in GetPlatform(str, str2))
            {
                source.Add(str4);
            }
            foreach (UWPExtension extension in GetExtensions(str, str2))
            {
                foreach (string str5 in extension.References)
                {
                    source.Add(str5);
                }
            }
            return source.ToArray<string>();
        }

        private static string[] GetReferences(string referencesFolder, XElement containedApiContractsElement)
        {
            List<string> list = new List<string>();
            IEnumerator<XElement> enumerator = containedApiContractsElement.Elements("ApiContract").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XElement current = enumerator.Current;
                    string str = current.Attribute("name").Value;
                    string str2 = current.Attribute("version").Value;
                    string[] paths = new string[] { referencesFolder, str, str2, str + ".winmd" };
                    string path = FileUtil.CombinePaths(paths);
                    if (File.Exists(path))
                    {
                        list.Add(path);
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
            return list.ToArray();
        }

        private static void GetWindowsKit10(out string folder, out Version version)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            folder = Path.Combine(environmentVariable, @"Windows Kits\10\");
            version = new Version(10, 0, 0x2800);
            try
            {
                folder = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", folder);
                string str3 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "ProductVersion", version.ToString());
                version = new Version(str3);
            }
            catch
            {
            }
        }

        private sealed class UWPExtension
        {
            public UWPExtension(string manifest, string referencesFolder)
            {
                XElement element = XDocument.Load(manifest).Element("FileList");
                if (element.Attribute("TargetPlatform").Value != "UAP")
                {
                    throw new Exception(string.Format("Invalid extension manifest at \"{0}\".", manifest));
                }
                this.Name = element.Attribute("DisplayName").Value;
                XElement containedApiContractsElement = element.Element("ContainedApiContracts");
                this.References = UWPReferences.GetReferences(referencesFolder, containedApiContractsElement);
            }

            public string Name { get; private set; }

            public string[] References { get; private set; }
        }
    }
}

