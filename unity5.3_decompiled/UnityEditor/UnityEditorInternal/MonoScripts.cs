namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    public static class MonoScripts
    {
        public static MonoScript CreateMonoScript(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript)
        {
            MonoScript script = new MonoScript();
            script.Init(scriptContents, className, nameSpace, assemblyName, isEditorScript);
            return script;
        }
    }
}

