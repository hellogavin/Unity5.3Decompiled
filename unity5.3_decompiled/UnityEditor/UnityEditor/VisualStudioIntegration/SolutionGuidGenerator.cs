namespace UnityEditor.VisualStudioIntegration
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class SolutionGuidGenerator
    {
        private static string ComputeGuidHashFor(string input)
        {
            return HashAsGuid(HashToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(input))));
        }

        public static string GuidForProject(string projectName)
        {
            return ComputeGuidHashFor(projectName + "salt");
        }

        public static string GuidForSolution(string projectName)
        {
            return ComputeGuidHashFor(projectName);
        }

        private static string HashAsGuid(string hash)
        {
            string[] textArray1 = new string[] { hash.Substring(0, 8), "-", hash.Substring(8, 4), "-", hash.Substring(12, 4), "-", hash.Substring(0x10, 4), "-", hash.Substring(20, 12) };
            return string.Concat(textArray1).ToUpper();
        }

        private static string HashToString(byte[] bs)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte num in bs)
            {
                builder.Append(num.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

