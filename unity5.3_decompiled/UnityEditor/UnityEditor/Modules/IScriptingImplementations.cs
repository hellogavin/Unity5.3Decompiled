namespace UnityEditor.Modules
{
    using UnityEditor;

    internal interface IScriptingImplementations
    {
        ScriptingImplementation[] Enabled();
        ScriptingImplementation[] Supported();
    }
}

