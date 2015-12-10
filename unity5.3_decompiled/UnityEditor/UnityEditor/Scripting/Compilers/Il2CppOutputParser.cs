namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class Il2CppOutputParser : CompilerOutputParserBase
    {
        private const string _errorIdentifier = "IL2CPP error";
        private static readonly Regex sErrorRegexWithSourceInformation = new Regex(@"\s*(?<message>.*) in (?<filename>.*):(?<line>\d+)");

        protected override string GetErrorIdentifier()
        {
            return "IL2CPP error";
        }

        protected override Regex GetOutputRegex()
        {
            return sErrorRegexWithSourceInformation;
        }

        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
        {
            List<CompilerMessage> list = new List<CompilerMessage>();
            for (int i = 0; i < standardOutput.Length; i++)
            {
                string input = standardOutput[i];
                if (input.StartsWith("IL2CPP error"))
                {
                    string path = string.Empty;
                    int num2 = 0;
                    StringBuilder builder = new StringBuilder();
                    Match match = sErrorRegexWithSourceInformation.Match(input);
                    if (match.Success)
                    {
                        path = match.Groups["filename"].Value;
                        num2 = int.Parse(match.Groups["line"].Value);
                        builder.AppendFormat("{0} in {1}:{2}", match.Groups["message"].Value, Path.GetFileName(path), num2);
                    }
                    else
                    {
                        builder.Append(input);
                    }
                    if (((i + 1) < standardOutput.Length) && standardOutput[i + 1].StartsWith("Additional information:"))
                    {
                        builder.AppendFormat("{0}{1}", Environment.NewLine, standardOutput[i + 1]);
                        i++;
                    }
                    CompilerMessage item = new CompilerMessage {
                        file = path,
                        line = num2,
                        message = builder.ToString(),
                        type = CompilerMessageType.Error
                    };
                    list.Add(item);
                }
            }
            return list;
        }
    }
}

