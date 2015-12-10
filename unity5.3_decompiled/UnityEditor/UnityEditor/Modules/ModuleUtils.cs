namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;

    internal static class ModuleUtils
    {
        internal static string[] GetAdditionalReferencesForUserScripts()
        {
            List<string> list = new List<string>();
            IEnumerator<IPlatformSupportModule> enumerator = ModuleManager.platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    list.AddRange(current.AssemblyReferencesForUserScripts);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list.ToArray();
        }
    }
}

