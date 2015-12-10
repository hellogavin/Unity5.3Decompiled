namespace UnityEditor
{
    using System;

    internal static class AssembliesShippedWithWebPlayerProvider
    {
        public static string[] ProvideAsArray()
        {
            return new string[] { "mscorlib.dll", "System.dll", "System.Core.dll", "Mono.Security.dll" };
        }
    }
}

