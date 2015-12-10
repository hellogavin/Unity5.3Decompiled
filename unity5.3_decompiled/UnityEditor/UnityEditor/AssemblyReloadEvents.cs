namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssemblyReloadEvents
    {
        public static void OnAfterAssemblyReload()
        {
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        public static void OnBeforeAssemblyReload()
        {
            Security.ClearVerifiedAssemblies();
            InternalEditorUtility.AuxWindowManager_OnAssemblyReload();
        }
    }
}

