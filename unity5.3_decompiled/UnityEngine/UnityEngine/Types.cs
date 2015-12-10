namespace UnityEngine
{
    using System;
    using System.Reflection;

    public static class Types
    {
        public static Type GetType(string typeName, string assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName).GetType(typeName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

