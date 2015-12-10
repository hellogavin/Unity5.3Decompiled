namespace UnityEditor
{
    using System;
    using System.Reflection;

    internal static class ShaderGUIUtility
    {
        internal static ShaderGUI CreateShaderGUI(string customEditorName)
        {
            string str = "UnityEditor." + customEditorName;
            Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
            for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
            {
                Assembly assembly = loadedAssemblies[i];
                foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
                {
                    if (type.FullName.Equals(customEditorName, StringComparison.Ordinal) || type.FullName.Equals(str, StringComparison.Ordinal))
                    {
                        if (typeof(ShaderGUI).IsAssignableFrom(type))
                        {
                            return (Activator.CreateInstance(type) as ShaderGUI);
                        }
                        return null;
                    }
                }
            }
            return null;
        }
    }
}

