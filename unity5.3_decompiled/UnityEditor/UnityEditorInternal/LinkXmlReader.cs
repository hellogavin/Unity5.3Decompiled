namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.XPath;

    internal class LinkXmlReader
    {
        private readonly List<string> _assembliesInALinkXmlFile = new List<string>();

        public LinkXmlReader()
        {
            IEnumerator<string> enumerator = AssemblyStripper.GetUserBlacklistFiles().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    XPathNavigator navigator = new XPathDocument(current).CreateNavigator();
                    navigator.MoveToFirstChild();
                    XPathNodeIterator iterator = navigator.SelectChildren("assembly", string.Empty);
                    while (iterator.MoveNext())
                    {
                        this._assembliesInALinkXmlFile.Add(iterator.Current.GetAttribute("fullname", string.Empty));
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
        }

        public bool IsDLLUsed(string assemblyFileName)
        {
            return this._assembliesInALinkXmlFile.Contains(Path.GetFileNameWithoutExtension(assemblyFileName));
        }
    }
}

