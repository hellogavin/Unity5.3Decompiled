namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    internal static class CommandLineFormatter
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache3;
        private static readonly Regex Quotes = new Regex("\"");
        private static readonly Regex UnescapeableChars = new Regex(@"[\x00-\x08\x10-\x1a\x1c-\x1f\x7f\xff]");
        private static readonly Regex UnsafeCharsWindows = new Regex(@"[^A-Za-z0-9\_\-\.\:\,\/\@\\]");

        public static string EscapeCharsQuote(string input)
        {
            if (input.IndexOf('\'') == -1)
            {
                return ("'" + input + "'");
            }
            if (input.IndexOf('"') == -1)
            {
                return ("\"" + input + "\"");
            }
            return null;
        }

        public static string EscapeCharsWindows(string input)
        {
            if (input.Length == 0)
            {
                return "\"\"";
            }
            if (UnescapeableChars.IsMatch(input))
            {
                Debug.LogWarning("Cannot escape control characters in string");
                return "\"\"";
            }
            if (UnsafeCharsWindows.IsMatch(input))
            {
                return ("\"" + Quotes.Replace(input, "\"\"") + "\"");
            }
            return input;
        }

        internal static string GenerateResponseFile(IEnumerable<string> arguments)
        {
            string uniqueTempPathInProject = FileUtil.GetUniqueTempPathInProject();
            using (StreamWriter writer = new StreamWriter(uniqueTempPathInProject))
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = a => a != null;
                }
                IEnumerator<string> enumerator = arguments.Where<string>(<>f__am$cache3).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        writer.WriteLine(current);
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
            return uniqueTempPathInProject;
        }

        public static string PrepareFileName(string input)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EscapeCharsQuote(input);
            }
            return EscapeCharsWindows(input);
        }
    }
}

