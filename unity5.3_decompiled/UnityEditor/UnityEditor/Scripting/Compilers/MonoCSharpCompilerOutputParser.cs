namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Text.RegularExpressions;

    internal class MonoCSharpCompilerOutputParser : CompilerOutputParserBase
    {
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*(?<type>warning|error)\s*(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sMissingMember = new Regex("[^`]*`(?<type_name>[^']+)'[^`]+`(?<member_name>[^']+)'", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier()
        {
            return "error";
        }

        protected override Regex GetOutputRegex()
        {
            return sCompilerOutput;
        }

        protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
        {
            return CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "CS0117", sMissingMember);
        }
    }
}

