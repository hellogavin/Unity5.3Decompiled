namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Text.RegularExpressions;

    internal class MicrosoftCSharpCompilerOutputParser : CompilerOutputParserBase
    {
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*(?<type>warning|error)\s*(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier()
        {
            return "error";
        }

        protected override Regex GetOutputRegex()
        {
            return sCompilerOutput;
        }
    }
}

