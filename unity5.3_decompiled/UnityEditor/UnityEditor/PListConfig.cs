namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class PListConfig
    {
        private string fileName;
        private string xml;

        public PListConfig(string fileName)
        {
            if (File.Exists(fileName))
            {
                StreamReader reader = new StreamReader(fileName);
                this.xml = reader.ReadToEnd();
                reader.Close();
            }
            else
            {
                this.Clear();
            }
            this.fileName = fileName;
        }

        public void Clear()
        {
            this.xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n<plist version=\"1.0\">\n<dict>\n</dict>\n</plist>\n";
        }

        private static Regex GetRegex(string paramName)
        {
            return new Regex("(?<Part1><key>" + paramName + @"</key>\s*<string>)(?<Value>.*)</string>");
        }

        public void Save()
        {
            StreamWriter writer = new StreamWriter(this.fileName);
            writer.Write(this.xml);
            writer.Close();
        }

        private void WriteNewValue(string key, string val)
        {
            this.xml = new Regex("</dict>").Replace(this.xml, "\t<key>" + key + "</key>\n\t<string>" + val + "</string>\n</dict>");
        }

        public string this[string paramName]
        {
            get
            {
                Match match = GetRegex(paramName).Match(this.xml);
                return (!match.Success ? string.Empty : match.Groups["Value"].Value);
            }
            set
            {
                if (GetRegex(paramName).Match(this.xml).Success)
                {
                    this.xml = GetRegex(paramName).Replace(this.xml, "${Part1}" + value + "</string>");
                }
                else
                {
                    this.WriteNewValue(paramName, value);
                }
            }
        }
    }
}

