namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.XPath;
    using UnityEngine;

    internal class BuildVerifier
    {
        private Dictionary<string, HashSet<string>> m_UnsupportedAssemblies = new Dictionary<string, HashSet<string>>();
        private static BuildVerifier ms_Inst;

        protected BuildVerifier()
        {
            string uri = Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources"), "BuildVerification.xml");
            XPathNavigator navigator = new XPathDocument(uri).CreateNavigator();
            navigator.MoveToFirstChild();
            XPathNodeIterator iterator = navigator.SelectChildren("assembly", string.Empty);
            while (iterator.MoveNext())
            {
                string attribute = iterator.Current.GetAttribute("name", string.Empty);
                if ((attribute == null) || (attribute.Length < 1))
                {
                    throw new ApplicationException(string.Format("Failed to load {0}, <assembly> name attribute is empty", uri));
                }
                string key = iterator.Current.GetAttribute("platform", string.Empty);
                if ((key == null) || (key.Length < 1))
                {
                    key = "*";
                }
                if (!this.m_UnsupportedAssemblies.ContainsKey(key))
                {
                    this.m_UnsupportedAssemblies.Add(key, new HashSet<string>());
                }
                this.m_UnsupportedAssemblies[key].Add(attribute);
            }
        }

        protected bool VerifyAssembly(BuildTarget target, string assembly)
        {
            return ((!this.m_UnsupportedAssemblies.ContainsKey("*") || !this.m_UnsupportedAssemblies["*"].Contains(assembly)) && (!this.m_UnsupportedAssemblies.ContainsKey(target.ToString()) || !this.m_UnsupportedAssemblies[target.ToString()].Contains(assembly)));
        }

        public static void VerifyBuild(BuildTarget target, string managedDllFolder)
        {
            if (ms_Inst == null)
            {
                ms_Inst = new BuildVerifier();
            }
            ms_Inst.VerifyBuildInternal(target, managedDllFolder);
        }

        protected void VerifyBuildInternal(BuildTarget target, string managedDllFolder)
        {
            foreach (string str in Directory.GetFiles(managedDllFolder))
            {
                if (str.EndsWith(".dll"))
                {
                    string fileName = Path.GetFileName(str);
                    if (!this.VerifyAssembly(target, fileName))
                    {
                        object[] args = new object[] { fileName, target.ToString() };
                        Debug.LogWarningFormat("{0} assembly is referenced by user code, but is not supported on {1} platform. Various failures might follow.", args);
                    }
                }
            }
        }
    }
}

