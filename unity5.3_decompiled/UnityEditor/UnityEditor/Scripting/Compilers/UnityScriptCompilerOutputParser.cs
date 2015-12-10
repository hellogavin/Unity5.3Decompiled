namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Text.RegularExpressions;

    internal class UnityScriptCompilerOutputParser : CompilerOutputParserBase
    {
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*[BU]C(?<type>W|E)(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier()
        {
            return "E";
        }

        protected override Regex GetOutputRegex()
        {
            return sCompilerOutput;
        }
    }
}

